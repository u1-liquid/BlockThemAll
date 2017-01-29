using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlockThemAll.Properties;
using Newtonsoft.Json;

namespace BlockThemAll
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }
        public TwitterApi twitter;
        public Dictionary<string, TwitterApi> twitters = new Dictionary<string, TwitterApi>();
        public IniSettings settings;
        public HashSet<string> blockDB = new HashSet<string>();
        public HashSet<string> followingDB = new HashSet<string>();
        public HashSet<string> followerDB = new HashSet<string>();
        private readonly object userdatarefreshlock = new object();
        private readonly object blockdbbuildlock = new object();

        public MainForm()
        {
            Instance = this;

            InitializeComponent();
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
            twitter = TwitterApi.Login(settings);
            SetStatusLabel(twitter.Status.ToString());
            ProcessTwitterLogin();

            foreach (KeyValuePair<string, Dictionary<string, object>> item in settings)
            {
                if (item.Key.Equals("Authenticate")) continue;

                twitter = TwitterApi.Login(settings, item.Key);
                ProcessTwitterLogin(item.Key);
            }

            if (!twitters.TryGetValue("Default", out twitter))
                twitter = twitters.Values.FirstOrDefault();

            if (twitter?.Status == UserStatus.LOGIN_SUCCESS)
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

            blockDB.UnionWith(File.ReadAllText(ofd.FileName).Split(new[] {"\r\n", "\n", ","}, StringSplitOptions.RemoveEmptyEntries));
            labelknownblocks.Text = blockDB.Count.ToString();
        }

        private void saveBlockDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                CreatePrompt = false,
                OverwritePrompt = true,
                RestoreDirectory = true,
                FileName = $"BlockDB_{twitter?.MyUserInfo?.screen_name}_{DateTime.Now:yyyy-MM-dd_HHmm}.csv"
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
            if (twitter == null) twitter = TwitterApi.Login(settings, SettingsSection);

            switch (twitter.Status)
            {
                case UserStatus.NO_APIKEY:
                    RegisterApiKeyForm rakform = new RegisterApiKeyForm() { TargetSection = SettingsSection };
                    if (DialogResult.OK == rakform.ShowDialog(this))
                    {
                        twitter = TwitterApi.Login(settings, SettingsSection);
                        SetStatusLabel(twitter.Status.ToString());
                        ProcessTwitterLogin(SettingsSection);
                    }
                    break;
                case UserStatus.NO_CREDITIONAL:
                    MessageBox.Show(this, "Unable to access your account! Please try login again.", "No Creditional", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    ProcessTwitterLogin(SettingsSection);
                    break;
                case UserStatus.LOGIN_REQUESTED:
                    LoginProgressForm lpform = new LoginProgressForm();
                    if (DialogResult.OK == lpform.ShowDialog(this))
                    {
                        twitter = TwitterApi.Login(settings, SettingsSection);
                        SetStatusLabel(twitter.Status.ToString());
                        ProcessTwitterLogin(SettingsSection);
                    }
                    break;
                case UserStatus.INVALID_CREDITIONAL:
                    MessageBox.Show(this, "Unable to access your account! Please try login from menu.", "Invalid Creditional",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                case UserStatus.LOGIN_SUCCESS:
                    string apikey = SettingsSection.Equals("Authenticate") ? "Default" : SettingsSection;
                    twitters.Add(apikey, twitter);
                    manageAPIKeysToolStripMenuItem.DropDownItems.Add(apikey);

                    loginToolStripMenuItem.Enabled = false;
                    loginToolStripMenuItem.Text = "Logged in as " + twitter.MyUserInfo.screen_name;
                    break;
            }
        }
        
        private void manageAPIKeysToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == registerAPIKeyToolStripMenuItem) return;

            if (e.ClickedItem.Text.Equals("Default"))
            {
                if (DialogResult.Yes ==
                    MessageBox.Show(this, "Do you want to change default API key?", "change key", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question))
                {
                    twitters.Remove("Default");
                    settings.DeleteSection("Authenticate");
                    manageAPIKeysToolStripMenuItem.DropDownItems.Remove(e.ClickedItem);

                    twitter = TwitterApi.Login(settings);
                    SetStatusLabel(twitter.Status.ToString());
                    ProcessTwitterLogin();
                }
            }
            else
            {
                if (DialogResult.Yes ==
                    MessageBox.Show(this, "Do you want to remove " + e.ClickedItem.Text + " API key?", "Remove key", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question))
                {
                    twitters.Remove(e.ClickedItem.Text);
                    settings.DeleteSection(e.ClickedItem.Text);
                    manageAPIKeysToolStripMenuItem.DropDownItems.Remove(e.ClickedItem);
                }
            }

            settings.Save();

            if (twitters.Count == 0)
            {
                loginToolStripMenuItem.Enabled = true;
                loginToolStripMenuItem.Text = "Login";
            }
        }

        private void registerAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegisterApiKeyForm rakform = new RegisterApiKeyForm() { TargetSection = string.Empty };
            if (DialogResult.OK == rakform.ShowDialog(this))
            {
                twitter = TwitterApi.Login(settings, rakform.TargetSection);
                SetStatusLabel(twitter.Status.ToString());
                ProcessTwitterLogin(rakform.TargetSection);
            }
        }

        private void RefreshMyInfo()
        {
            lock (userdatarefreshlock)
            {
                labellastupdate.Text = "in progress...";
                followingDB.Clear();
                followerDB.Clear();

                Task.Factory.StartNew(() =>
                {
                    UserIdsObject result = twitter.getMyFriends("-1");
                    while (result != null)
                    {
                        followingDB.UnionWith(result.ids);
                        if (result.next_cursor == 0)
                            break;
                        result = twitter.getMyFriends(result.next_cursor_str);
                    }

                    result = twitter.getMyFollowers("-1");
                    while (result != null)
                    {
                        followerDB.UnionWith(result.ids);
                        if (result.next_cursor == 0)
                            break;
                        result = twitter.getMyFollowers(result.next_cursor_str);
                    }

                    Action action = () =>
                    {
                        labelusername.Text = "@" + twitter.MyUserInfo.screen_name + " / " + twitter.MyUserInfo.id_str;
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
                        labelknownblocks.Text += " (Rate Limit, Retry at : " + DateTime.Now.AddMinutes(15).ToString("hh:mm:ss") + ")";
                    };

                    while (true)
                        try
                        {
                            UserIdsObject result = twitter.getMyBlockList(cursor);
                            while (result != null)
                            {
                                blockDB.UnionWith(result.ids);
                                BeginInvoke(knownblocksupdateaction);
                                if (result.next_cursor == 0)
                                    break;
                                result = twitter.getMyBlockList(cursor = result.next_cursor_str);
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
                case "Start":
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

                        string t = target.TrimStart('-', '!', '@');

                        TreeNode node = new TreeNode($"({type}) {t}") {Tag = new TreeEntity(type, t), ContextMenuStrip = contextMenuStrip1};
                        treeView1.Nodes.Add(node);
                    }
                    tabControl1.SelectedIndex = 1;
                    break;
                case "Stop":
                    break;
            }
        }

        private void buttonpausecontinue_Click(object sender, EventArgs e)
        {
            switch (buttonpausecontinue.Text)
            {
                case "Pause":
                    break;
                case "Continue":
                    break;
            }
        }

        private void buttonskip_Click(object sender, EventArgs e)
        {

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
    }
}
