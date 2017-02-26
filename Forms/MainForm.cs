using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlockThemAll.Base;
using BlockThemAll.Properties;
using BlockThemAll.Twitter;

namespace BlockThemAll.Forms
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }
        public IniSettings settings;
        public HashSet<string> blockDB = new HashSet<string>();
        public HashSet<string> followingDB = new HashSet<string>();
        public HashSet<string> followerDB = new HashSet<string>();
        private readonly object userdatarefreshlock = new object();
        private readonly object blockdbbuildlock = new object();
        public WorkStatus workStatus = WorkStatus.READY;
        private TwitterApi loginTemp;

        public MainForm()
        {
            Instance = this;

            InitializeComponent();

            cbKeepFollowing.Checked = Settings.Default.KeepFollowing;
            cbKeepFollower.Checked = Settings.Default.KeepFollower;
            cbSimulateOnly.Checked = Settings.Default.SimulateOnly;
            cbAutoRetryApiLimit.Checked = Settings.Default.AutoRetryApiLimit;
            cbAutoSwitchCredApiLimit.Checked = Settings.Default.AutoSwitchCredApiLimit;
            cbMuteUserOnly.Checked = Settings.Default.MuteUserOnly;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        public void Log(string text)
        {
            Action action = () => { listBox1.Items.Add(text); };
            listBox1.BeginInvoke(action);
        }

        public void SetStatusLabel(string text)
        {
            toolStripStatusLabel2.Text = text;
        }

        public void SetProgressLabel(string text)
        {
            toolStripStatusLabel1.Text = text;
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null || files.Length == 0) return;
                if (!textBox1.Text.EndsWith(Environment.NewLine))
                    textBox1.Text += Environment.NewLine;

                foreach (string file in files)
                {
                    textBox1.Text += file + Environment.NewLine;
                }
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
            {
                string str = (string) e.Data.GetData(DataFormats.UnicodeText, true);
                if (string.IsNullOrEmpty(str)) return;
                if (!textBox1.Text.EndsWith(Environment.NewLine))
                    textBox1.Text += Environment.NewLine;

                textBox1.Text += str;
            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) || e.Data.GetDataPresent(DataFormats.UnicodeText, true))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void buttonresettarget_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void buttonextractscreenname_Click(object sender, EventArgs e)
        {
            MatchCollection mc = Regex.Matches(textBox1.Text, @"(?<=^|(?<=[^a-zA-Z0-9-\.]))@([A-Za-z0-9_]+)", RegexOptions.Multiline);
            if (mc.Count == 0) return;

            textBox1.Text = string.Empty;
            foreach (Match m in mc)
            {
                textBox1.Text += m.Value + Environment.NewLine;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            settings = new IniSettings(new FileInfo("BlockThemAll.ini"));
            loginTemp = TwitterApi.Login(settings);
            ProcessTwitterLogin();
            
            SetProgressLabel(workStatus.ToString());

            foreach (KeyValuePair<string, Dictionary<string, object>> item in settings)
            {
                if (item.Key.Equals("Authenticate")) continue;

                loginTemp = TwitterApi.Login(settings, item.Key);
                ProcessTwitterLogin(item.Key);
            }

            CredentialManager.Instance.SelectCredential("Default");

            if (CredentialManager.Instance.Status == UserStatus.LOGIN_SUCCESS)
                RefreshMyInfo();
        }

        private void loadTargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                RestoreDirectory = true,
                CheckFileExists = true,
                Multiselect = true
            };

            if (DialogResult.OK != ofd.ShowDialog(this)) return;

            foreach (string name in ofd.FileNames)
            {
                if (!textBox1.Text.EndsWith(Environment.NewLine))
                    textBox1.Text += Environment.NewLine;

                textBox1.Text += File.ReadAllText(name);
            }
        }

        private void loadBlockDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                RestoreDirectory = true,
                CheckFileExists = true,
                Multiselect = false
            };

            if (DialogResult.OK != ofd.ShowDialog(this)) return;

            lock(blockdbbuildlock) blockDB.UnionWith(File.ReadAllText(ofd.FileName).Split(new[] {"\r\n", "\n", ","}, StringSplitOptions.RemoveEmptyEntries));
            labelknownblocks.Text = blockDB.Count.ToString();
        }

        private void saveBlockDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                CreatePrompt = false,
                OverwritePrompt = true,
                RestoreDirectory = true,
                FileName = $"BlockDB_{CredentialManager.Instance.MyUserInfo?.screen_name}_{DateTime.Now:yyyy-MM-dd_HHmm}.csv"
            };

            if (DialogResult.OK != sfd.ShowDialog(this)) return;
            
            File.WriteAllText(sfd.FileName, string.Join(",", blockDB));
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessTwitterLogin();
        }

        private void ProcessTwitterLogin(string SettingsSection = "Authenticate")
        {
            TwitterApi twitter = loginTemp ?? (loginTemp = TwitterApi.Login(settings, SettingsSection));

            switch (twitter.Status)
            {
                case UserStatus.NO_APIKEY:
                    RegisterApiKeyForm rakform = new RegisterApiKeyForm() { TargetSection = SettingsSection };
                    if (DialogResult.OK == rakform.ShowDialog(this))
                    {
                        loginTemp = twitter = TwitterApi.Login(settings, SettingsSection);
                        SetStatusLabel(twitter.Status.ToString());
                        ProcessTwitterLogin(SettingsSection);
                    }
                    break;
                case UserStatus.NO_CREDITIONAL:
                    MessageBox.Show(this, @"Unable to access your account! Please try login again.", @"No Creditional", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    ProcessTwitterLogin(SettingsSection);
                    break;
                case UserStatus.LOGIN_REQUESTED:
                    LoginProgressForm lpform = new LoginProgressForm(twitter);
                    if (DialogResult.OK == lpform.ShowDialog(this))
                    {
                        loginTemp = twitter = TwitterApi.Login(settings, SettingsSection);
                        SetStatusLabel(twitter.Status.ToString());
                        ProcessTwitterLogin(SettingsSection);
                    }
                    break;
                case UserStatus.INVALID_CREDITIONAL:
                    loginTemp = null;
                    MessageBox.Show(this, @"Unable to access your account! Please try login from menu.", @"Invalid Creditional",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                case UserStatus.LOGIN_SUCCESS:
                    try
                    {
                        loginTemp = null;
                        string apikey = SettingsSection.Equals("Authenticate") ? "Default" : SettingsSection;
                        CredentialManager.Instance.AddCredential(apikey, twitter);
                        manageAPIKeysToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(apikey) { Name = apikey });
                        removeAPIKeyToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(apikey) { Name = apikey });

                        loginToolStripMenuItem.Enabled = false;
                        loginToolStripMenuItem.Text = @"Logged in as " + twitter.MyUserInfo.screen_name;
                    }
                    catch (InvalidCredentialException e)
                    {
                        settings.DeleteSection(SettingsSection);
                        settings.Save();

                        MessageBox.Show(this, e.Message, @"Invalid Creditional", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    break;
            }
        }

        private void manageAPIKeysToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string current = CredentialManager.Instance.Credentials.FirstOrDefault(x => ReferenceEquals(x.Value, CredentialManager.Instance.Current)).Key;
            foreach (object dropDownItem in manageAPIKeysToolStripMenuItem.DropDownItems)
            {
                ToolStripMenuItem item = dropDownItem as ToolStripMenuItem;
                if (item != null) item.Checked = item.Text.Equals(current);
            }
        }

        private void manageAPIKeysToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.GetType() != typeof(ToolStripMenuItem) || e.ClickedItem == registerAPIKeyToolStripMenuItem || e.ClickedItem == removeAPIKeyToolStripMenuItem) return;

            CredentialManager.Instance.SelectCredential(e.ClickedItem.Text);
            foreach (object dropDownItem in manageAPIKeysToolStripMenuItem.DropDownItems)
            {
                ToolStripMenuItem item = dropDownItem as ToolStripMenuItem;
                if (item != null) item.Checked = item.Text.Equals(e.ClickedItem.Text);
            }
        }

        private void removeAPIKeyToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.Equals("Default"))
            {
                if (DialogResult.Yes ==
                    MessageBox.Show(this, @"Do you want to change default API key?", @"Change key", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question))
                {
                    CredentialManager.Instance.RemoveCredential("Default");
                    settings.DeleteSection("Authenticate");
                    manageAPIKeysToolStripMenuItem.DropDownItems.RemoveByKey(e.ClickedItem.Text);
                    removeAPIKeyToolStripMenuItem.DropDownItems.RemoveByKey(e.ClickedItem.Text);

                    loginTemp = TwitterApi.Login(settings);
                    ProcessTwitterLogin();
                    
                    CredentialManager.Instance.SelectCredential("Default");

                    if (CredentialManager.Instance.Status == UserStatus.LOGIN_SUCCESS)
                        RefreshMyInfo();
                }
            }
            else
            {
                if (DialogResult.Yes ==
                    MessageBox.Show(this, @"Do you want to remove " + e.ClickedItem.Text + @" API key?", @"Remove key", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question))
                {
                    TwitterApi target = CredentialManager.Instance.Credentials[e.ClickedItem.Text];
                    CredentialManager.Instance.RemoveCredential(e.ClickedItem.Text);

                    if (ReferenceEquals(CredentialManager.Instance.Current, target))
                        CredentialManager.Instance.SelectCredential("Default");

                    settings.DeleteSection(e.ClickedItem.Text);
                    manageAPIKeysToolStripMenuItem.DropDownItems.RemoveByKey(e.ClickedItem.Text);
                    removeAPIKeyToolStripMenuItem.DropDownItems.RemoveByKey(e.ClickedItem.Text);
                }
            }

            if (CredentialManager.Instance.Credentials.Count == 0 || !CredentialManager.Instance.Credentials.ContainsKey("Default"))
            {
                loginToolStripMenuItem.Enabled = true;
                loginToolStripMenuItem.Text = @"Login";
            }

            settings.Save();
        }

        private void registerAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegisterApiKeyForm rakform = new RegisterApiKeyForm() { TargetSection = string.Empty };
            if (DialogResult.OK == rakform.ShowDialog(this))
            {
                loginTemp = TwitterApi.Login(settings, rakform.TargetSection);
                ProcessTwitterLogin(rakform.TargetSection);
            }
        }

        private void RefreshMyInfo()
        {
            lock (userdatarefreshlock)
            {
                labellastupdate.Text = @"in progress...";
                followingDB.Clear();
                followerDB.Clear();

                Task.Factory.StartNew(() =>
                {
                    UserIdsObject result;
                    string cursor = "-1";

                    while(true)
                        try
                        {
                            result = CredentialManager.Instance.getMyFriends(cursor);
                            while (result != null)
                            {
                                followingDB.UnionWith(result.ids);
                                if (result.next_cursor == 0)
                                    break;
                                result = CredentialManager.Instance.getMyFriends(cursor = result.next_cursor_str);
                            }

                            break;
                        }
                        catch (RateLimitException)
                        {
                            if (Settings.Default.AutoRetryApiLimit) Thread.Sleep(TimeSpan.FromMinutes(15));
                        }

                    cursor = "-1";
                    while(true)
                        try
                        {
                            result = CredentialManager.Instance.getMyFollowers(cursor);
                            while (result != null)
                            {
                                followerDB.UnionWith(result.ids);
                                if (result.next_cursor == 0)
                                    break;
                                result = CredentialManager.Instance.getMyFollowers(cursor = result.next_cursor_str);
                            }

                            break;
                        }
                        catch (RateLimitException)
                        {
                            if (Settings.Default.AutoRetryApiLimit) Thread.Sleep(TimeSpan.FromMinutes(15));
                        }

                    Action action = () =>
                    {
                        labelusername.Text = @"@" + CredentialManager.Instance.MyUserInfo.screen_name + @" / " + CredentialManager.Instance.MyUserInfo.id_str;
                        labelfollowings.Text = followingDB.Count.ToString();
                        labelfollowers.Text = followerDB.Count.ToString();
                        labellastupdate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    };

                    BeginInvoke(action);
                });
            }
        }

        private void buttonrefreshmyinfo_Click(object sender, EventArgs e)
        {
            RefreshMyInfo();
        }
        
        private void buildBlockDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (blockdbbuildlock)
            {
                blockDB.Clear();

                Task.Factory.StartNew(() =>
                {
                    string cursor = "-1";

                    Action knownblocksupdateaction = () =>
                    {
                        labelknownblocks.Text = blockDB.Count.ToString();
                    };

                    Action waitforratelimitlabelupdateaction = () =>
                    {
                        labelknownblocks.Text += @" (Rate Limit, Retry at : " + DateTime.Now.AddMinutes(15).ToString("hh:mm:ss") + @")";
                    };

                    while (true)
                        try
                        {
                            UserIdsObject result = CredentialManager.Instance.getMyBlockList(cursor);
                            while (result != null)
                            {
                                blockDB.UnionWith(result.ids);
                                BeginInvoke(knownblocksupdateaction);
                                if (result.next_cursor == 0)
                                    break;
                                result = CredentialManager.Instance.getMyBlockList(cursor = result.next_cursor_str);
                            }
                            break;
                        }
                        catch (RateLimitException)
                        {
                            if (!Settings.Default.AutoRetryApiLimit) break; 
                            BeginInvoke(waitforratelimitlabelupdateaction);
                            Thread.Sleep(TimeSpan.FromMinutes(15));
                        }

                    BeginInvoke(knownblocksupdateaction);
                });
            }
        }

        private void buttonstartstop_Click(object sender, EventArgs e)
        {
            switch (buttonstartstop.Text)
            {
                case "Preview":
                    treeView1.Nodes.Clear();
                    List<string> targets = new List<string>(textBox1.Text.Split(new [] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries));
                    foreach (string target in targets)
                    {
                        EntityType type = EntityType.KEYWORD_BLOCK;
                        if (target.StartsWith("-"))
                            type = EntityType.KEYWORD_UNBLOCK;
                        if (target.StartsWith("@"))
                            type = EntityType.FOLLOWER_BLOCK;
                        if (target.StartsWith("-@"))
                            type = EntityType.FOLLOWER_UNBLOCK;
                        if (target.StartsWith("!@"))
                            type = EntityType.FOLLOWING_BLOCK;
                        if (target.StartsWith("-!@"))
                            type = EntityType.FOLLOWING_UNBLOCK;
                        if (target.StartsWith("&"))
                            type = EntityType.REPLY_BLOCK;
                        if (target.StartsWith("-&"))
                            type = EntityType.REPLY_UNBLOCK;
                        if (target.StartsWith("%"))
                            type = EntityType.RETWEET_BLOCK;
                        if (target.StartsWith("-%"))
                            type = EntityType.RETWEET_UNBLOCK;
                        
                        if (Settings.Default.MuteUserOnly) type += (int)EntityType.BLOCK_TO_MUTE;
                        
                        string t = target.TrimStart('-', '!', '@', '&', '%');

                        TreeNode node = new TreeNode($"({type}) {t}") {Tag = new TreeEntity(type, t), ContextMenuStrip = contextMenuStrip1};
                        treeView1.Nodes.Add(node);
                    }
                    tabControl1.SelectedIndex = 1;
                    workStatus = WorkStatus.PREVIEW;
                    buttonstartstop.Text = @"Start";
                    break;
                case "Start":
                    workStatus = WorkStatus.STARTING;
                    backgroundWorker1.RunWorkerAsync();
                    buttonstartstop.Text = @"Stop";
                    buttonpausecontinue.Enabled = true;
                    break;
                case "Stop":
                    workStatus = WorkStatus.STOPPING;
                    backgroundWorker1.CancelAsync();
                    buttonstartstop.Text = @"Preview";
                    buttonpausecontinue.Text = @"Pause";
                    buttonpausecontinue.Enabled = false;
                    break;
            }
        }

        private void buttonpausecontinue_Click(object sender, EventArgs e)
        {
            switch (buttonpausecontinue.Text)
            {
                case "Pause":
                    workStatus = WorkStatus.PAUSED;
                    buttonpausecontinue.Text = @"Continue";
                    break;
                case "Continue":
                    workStatus = WorkStatus.STARTED;
                    buttonpausecontinue.Text = @"Pause";
                    break;
            }
        }

        private void cbSettings_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb?.Name)
            {
                case "cbKeepFollowing":
                    Settings.Default.KeepFollowing = cbKeepFollowing.Checked;
                    break;
                case "cbKeepFollower":
                    Settings.Default.KeepFollower = cbKeepFollower.Checked;
                    break;
                case "cbSimulateOnly":
                    Settings.Default.SimulateOnly = cbSimulateOnly.Checked;
                    break;
                case "cbAutoRetryApiLimit":
                    Settings.Default.AutoRetryApiLimit = cbAutoRetryApiLimit.Checked;
                    break;
                case "cbAutoSwitchCredApiLimit":
                    Settings.Default.AutoSwitchCredApiLimit = cbAutoSwitchCredApiLimit.Checked;
                    break;
                case "cbMuteUserOnly":
                    Settings.Default.MuteUserOnly = cbMuteUserOnly.Checked;
                    break;
            }
            Settings.Default.Save();
        }
        
        private void skipThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeViewHitTestInfo hitTest = treeView1.HitTest(treeView1.PointToClient(new Point(contextMenuStrip1.Left, contextMenuStrip1.Top)));
                TreeEntity te = hitTest.Node?.Tag as TreeEntity;
                if (te == null) return;

                treeView1.SuspendLayout();
                te.Type = EntityType.SKIP;
                hitTest.Node.Tag = te;
                hitTest.Node.Text = $@"({te.Type}) {te.Target}";
                hitTest.Node.ImageIndex = 3;
                hitTest.Node.SelectedImageIndex = 3;
                treeView1.ResumeLayout();
            }
            catch
            {
                // ignore
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (workStatus == WorkStatus.PREVIEW)
            {
                workStatus = WorkStatus.READY;
                buttonstartstop.Text = @"Preview";
            }   
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox) sender)?.SelectAll();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Action action = () =>
            {
                if (toolStripProgressBar?.ProgressBar != null) toolStripProgressBar.ProgressBar.Value = e.ProgressPercentage;
                toolStripStatusLabel1.Text = workStatus.ToString();
            };

            if (InvokeRequired)
                BeginInvoke(action);
            else
                action.BeginInvoke(null, null);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
