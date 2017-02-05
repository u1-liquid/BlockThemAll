namespace BlockThemAll
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDummy;
            System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Followings");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Followers");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("UserDefined");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBlockDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBlockDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.twitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildBlockDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageAPIKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerAPIKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAPIKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonresettarget = new System.Windows.Forms.Button();
            this.buttonextractscreenname = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbMuteUserOnly = new System.Windows.Forms.CheckBox();
            this.cbAutoSwitchCredApiLimit = new System.Windows.Forms.CheckBox();
            this.cbAutoRetryApiLimit = new System.Windows.Forms.CheckBox();
            this.cbSimulateOnly = new System.Windows.Forms.CheckBox();
            this.cbKeepFollower = new System.Windows.Forms.CheckBox();
            this.cbKeepFollowing = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelknownblocks = new System.Windows.Forms.Label();
            this.labellastupdate = new System.Windows.Forms.Label();
            this.labelfollowers = new System.Windows.Forms.Label();
            this.labelfollowings = new System.Windows.Forms.Label();
            this.labelusername = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonrefreshmyinfo = new System.Windows.Forms.Button();
            this.buttonstartstop = new System.Windows.Forms.Button();
            this.buttonpausecontinue = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.skipThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripStatusLabelDummy = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripStatusLabelDummy
            // 
            toolStripStatusLabelDummy.Name = "toolStripStatusLabelDummy";
            toolStripStatusLabelDummy.Size = new System.Drawing.Size(726, 20);
            toolStripStatusLabelDummy.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.AutoSize = false;
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new System.Drawing.Size(20, 20);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.twitterToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(977, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadTargetsToolStripMenuItem,
            this.loadBlockDBToolStripMenuItem,
            this.saveBlockDBToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadTargetsToolStripMenuItem
            // 
            this.loadTargetsToolStripMenuItem.Name = "loadTargetsToolStripMenuItem";
            this.loadTargetsToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.loadTargetsToolStripMenuItem.Text = "Load Targets";
            this.loadTargetsToolStripMenuItem.Click += new System.EventHandler(this.loadTargetsToolStripMenuItem_Click);
            // 
            // loadBlockDBToolStripMenuItem
            // 
            this.loadBlockDBToolStripMenuItem.Name = "loadBlockDBToolStripMenuItem";
            this.loadBlockDBToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.loadBlockDBToolStripMenuItem.Text = "Load BlockDB";
            this.loadBlockDBToolStripMenuItem.Click += new System.EventHandler(this.loadBlockDBToolStripMenuItem_Click);
            // 
            // saveBlockDBToolStripMenuItem
            // 
            this.saveBlockDBToolStripMenuItem.Name = "saveBlockDBToolStripMenuItem";
            this.saveBlockDBToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.saveBlockDBToolStripMenuItem.Text = "Save BlockDB";
            this.saveBlockDBToolStripMenuItem.Click += new System.EventHandler(this.saveBlockDBToolStripMenuItem_Click);
            // 
            // twitterToolStripMenuItem
            // 
            this.twitterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.buildBlockDBToolStripMenuItem,
            this.manageAPIKeysToolStripMenuItem});
            this.twitterToolStripMenuItem.Name = "twitterToolStripMenuItem";
            this.twitterToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.twitterToolStripMenuItem.Text = "Twitter";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // buildBlockDBToolStripMenuItem
            // 
            this.buildBlockDBToolStripMenuItem.Name = "buildBlockDBToolStripMenuItem";
            this.buildBlockDBToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.buildBlockDBToolStripMenuItem.Text = "Build BlockDB";
            this.buildBlockDBToolStripMenuItem.Click += new System.EventHandler(this.buildBlockDBToolStripMenuItem_Click);
            // 
            // manageAPIKeysToolStripMenuItem
            // 
            this.manageAPIKeysToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerAPIKeyToolStripMenuItem,
            this.removeAPIKeyToolStripMenuItem,
            this.toolStripSeparator1});
            this.manageAPIKeysToolStripMenuItem.Name = "manageAPIKeysToolStripMenuItem";
            this.manageAPIKeysToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.manageAPIKeysToolStripMenuItem.Text = "Manage API keys";
            this.manageAPIKeysToolStripMenuItem.DropDownOpening += new System.EventHandler(this.manageAPIKeysToolStripMenuItem_DropDownOpening);
            this.manageAPIKeysToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.manageAPIKeysToolStripMenuItem_DropDownItemClicked);
            // 
            // registerAPIKeyToolStripMenuItem
            // 
            this.registerAPIKeyToolStripMenuItem.Name = "registerAPIKeyToolStripMenuItem";
            this.registerAPIKeyToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.registerAPIKeyToolStripMenuItem.Text = "Register API key";
            this.registerAPIKeyToolStripMenuItem.Click += new System.EventHandler(this.registerAPIKeyToolStripMenuItem_Click);
            // 
            // removeAPIKeyToolStripMenuItem
            // 
            this.removeAPIKeyToolStripMenuItem.Name = "removeAPIKeyToolStripMenuItem";
            this.removeAPIKeyToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.removeAPIKeyToolStripMenuItem.Text = "Remove API key";
            this.removeAPIKeyToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.removeAPIKeyToolStripMenuItem_DropDownItemClicked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel1,
            toolStripStatusLabelDummy,
            this.toolStripStatusLabel2,
            toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 624);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(977, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 19);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(65, 20);
            this.toolStripStatusLabel1.Text = "Progress";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(49, 20);
            this.toolStripStatusLabel2.Text = "Status";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 200);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(953, 421);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonresettarget);
            this.tabPage1.Controls.Add(this.buttonextractscreenname);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(945, 392);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Input";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonresettarget
            // 
            this.buttonresettarget.Location = new System.Drawing.Point(864, 363);
            this.buttonresettarget.Name = "buttonresettarget";
            this.buttonresettarget.Size = new System.Drawing.Size(75, 23);
            this.buttonresettarget.TabIndex = 2;
            this.buttonresettarget.Text = "Reset";
            this.buttonresettarget.UseVisualStyleBackColor = true;
            this.buttonresettarget.Click += new System.EventHandler(this.buttonresettarget_Click);
            // 
            // buttonextractscreenname
            // 
            this.buttonextractscreenname.Location = new System.Drawing.Point(6, 363);
            this.buttonextractscreenname.Name = "buttonextractscreenname";
            this.buttonextractscreenname.Size = new System.Drawing.Size(176, 23);
            this.buttonextractscreenname.TabIndex = 1;
            this.buttonextractscreenname.Text = "Extract @screenname";
            this.buttonextractscreenname.UseVisualStyleBackColor = true;
            this.buttonextractscreenname.Click += new System.EventHandler(this.buttonextractscreenname_Click);
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.AllowDrop = true;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(939, 354);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox1_DragDrop);
            this.textBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox1_DragEnter);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.treeView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(945, 392);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Targets";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(939, 386);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "IN_PROGRESS");
            this.imageList1.Images.SetKeyName(1, "DONE");
            this.imageList1.Images.SetKeyName(2, "STAR");
            this.imageList1.Images.SetKeyName(3, "SKIP");
            this.imageList1.Images.SetKeyName(4, "UNKNOWN");
            this.imageList1.Images.SetKeyName(5, "WARNING");
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.treeView2);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(945, 392);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Whitelists";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // treeView2
            // 
            this.treeView2.CheckBoxes = true;
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.Location = new System.Drawing.Point(3, 3);
            this.treeView2.Name = "treeView2";
            treeNode1.Name = "Followings";
            treeNode1.Text = "Followings";
            treeNode2.Name = "Followers";
            treeNode2.Text = "Followers";
            treeNode3.Name = "UserDefined";
            treeNode3.Text = "UserDefined";
            this.treeView2.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.treeView2.Size = new System.Drawing.Size(939, 386);
            this.treeView2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(945, 392);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "LogView";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(939, 386);
            this.listBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbMuteUserOnly);
            this.groupBox1.Controls.Add(this.cbAutoSwitchCredApiLimit);
            this.groupBox1.Controls.Add(this.cbAutoRetryApiLimit);
            this.groupBox1.Controls.Add(this.cbSimulateOnly);
            this.groupBox1.Controls.Add(this.cbKeepFollower);
            this.groupBox1.Controls.Add(this.cbKeepFollowing);
            this.groupBox1.Location = new System.Drawing.Point(477, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 163);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // cbMuteUserOnly
            // 
            this.cbMuteUserOnly.AutoSize = true;
            this.cbMuteUserOnly.Location = new System.Drawing.Point(252, 136);
            this.cbMuteUserOnly.Name = "cbMuteUserOnly";
            this.cbMuteUserOnly.Size = new System.Drawing.Size(188, 21);
            this.cbMuteUserOnly.TabIndex = 0;
            this.cbMuteUserOnly.Text = "Mute user instad of block";
            this.cbMuteUserOnly.UseVisualStyleBackColor = true;
            this.cbMuteUserOnly.CheckedChanged += new System.EventHandler(this.cbSettings_CheckedChanged);
            // 
            // cbAutoSwitchCredApiLimit
            // 
            this.cbAutoSwitchCredApiLimit.AutoSize = true;
            this.cbAutoSwitchCredApiLimit.Enabled = false;
            this.cbAutoSwitchCredApiLimit.Location = new System.Drawing.Point(252, 49);
            this.cbAutoSwitchCredApiLimit.Name = "cbAutoSwitchCredApiLimit";
            this.cbAutoSwitchCredApiLimit.Size = new System.Drawing.Size(210, 21);
            this.cbAutoSwitchCredApiLimit.TabIndex = 0;
            this.cbAutoSwitchCredApiLimit.Text = "Auto switch cred. on API limit";
            this.cbAutoSwitchCredApiLimit.UseVisualStyleBackColor = true;
            this.cbAutoSwitchCredApiLimit.CheckedChanged += new System.EventHandler(this.cbSettings_CheckedChanged);
            // 
            // cbAutoRetryApiLimit
            // 
            this.cbAutoRetryApiLimit.AutoSize = true;
            this.cbAutoRetryApiLimit.Checked = true;
            this.cbAutoRetryApiLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoRetryApiLimit.Location = new System.Drawing.Point(252, 21);
            this.cbAutoRetryApiLimit.Name = "cbAutoRetryApiLimit";
            this.cbAutoRetryApiLimit.Size = new System.Drawing.Size(165, 21);
            this.cbAutoRetryApiLimit.TabIndex = 0;
            this.cbAutoRetryApiLimit.Text = "Auto retry on API limit";
            this.cbAutoRetryApiLimit.UseVisualStyleBackColor = true;
            this.cbAutoRetryApiLimit.CheckedChanged += new System.EventHandler(this.cbSettings_CheckedChanged);
            // 
            // cbSimulateOnly
            // 
            this.cbSimulateOnly.AutoSize = true;
            this.cbSimulateOnly.Location = new System.Drawing.Point(7, 136);
            this.cbSimulateOnly.Name = "cbSimulateOnly";
            this.cbSimulateOnly.Size = new System.Drawing.Size(114, 21);
            this.cbSimulateOnly.TabIndex = 0;
            this.cbSimulateOnly.Text = "Simulate only";
            this.cbSimulateOnly.UseVisualStyleBackColor = true;
            this.cbSimulateOnly.CheckedChanged += new System.EventHandler(this.cbSettings_CheckedChanged);
            // 
            // cbKeepFollower
            // 
            this.cbKeepFollower.AutoSize = true;
            this.cbKeepFollower.Location = new System.Drawing.Point(7, 49);
            this.cbKeepFollower.Name = "cbKeepFollower";
            this.cbKeepFollower.Size = new System.Drawing.Size(148, 21);
            this.cbKeepFollower.TabIndex = 0;
            this.cbKeepFollower.Text = "Keep my Followers";
            this.cbKeepFollower.UseVisualStyleBackColor = true;
            this.cbKeepFollower.CheckedChanged += new System.EventHandler(this.cbSettings_CheckedChanged);
            // 
            // cbKeepFollowing
            // 
            this.cbKeepFollowing.AutoSize = true;
            this.cbKeepFollowing.Checked = true;
            this.cbKeepFollowing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbKeepFollowing.Location = new System.Drawing.Point(7, 22);
            this.cbKeepFollowing.Name = "cbKeepFollowing";
            this.cbKeepFollowing.Size = new System.Drawing.Size(154, 21);
            this.cbKeepFollowing.TabIndex = 0;
            this.cbKeepFollowing.Text = "Keep my Followings";
            this.cbKeepFollowing.UseVisualStyleBackColor = true;
            this.cbKeepFollowing.CheckedChanged += new System.EventHandler(this.cbSettings_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelknownblocks);
            this.groupBox2.Controls.Add(this.labellastupdate);
            this.groupBox2.Controls.Add(this.labelfollowers);
            this.groupBox2.Controls.Add(this.labelfollowings);
            this.groupBox2.Controls.Add(this.labelusername);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.buttonrefreshmyinfo);
            this.groupBox2.Location = new System.Drawing.Point(16, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(455, 100);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "My Info";
            // 
            // labelknownblocks
            // 
            this.labelknownblocks.AutoSize = true;
            this.labelknownblocks.Location = new System.Drawing.Point(107, 80);
            this.labelknownblocks.Name = "labelknownblocks";
            this.labelknownblocks.Size = new System.Drawing.Size(16, 17);
            this.labelknownblocks.TabIndex = 3;
            this.labelknownblocks.Text = "0";
            // 
            // labellastupdate
            // 
            this.labellastupdate.Location = new System.Drawing.Point(307, 52);
            this.labellastupdate.Name = "labellastupdate";
            this.labellastupdate.Size = new System.Drawing.Size(142, 17);
            this.labellastupdate.TabIndex = 3;
            this.labellastupdate.Text = "never";
            this.labellastupdate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelfollowers
            // 
            this.labelfollowers.AutoSize = true;
            this.labelfollowers.Location = new System.Drawing.Point(89, 52);
            this.labelfollowers.Name = "labelfollowers";
            this.labelfollowers.Size = new System.Drawing.Size(16, 17);
            this.labelfollowers.TabIndex = 3;
            this.labelfollowers.Text = "0";
            // 
            // labelfollowings
            // 
            this.labelfollowings.AutoSize = true;
            this.labelfollowings.Location = new System.Drawing.Point(89, 35);
            this.labelfollowings.Name = "labelfollowings";
            this.labelfollowings.Size = new System.Drawing.Size(16, 17);
            this.labelfollowings.TabIndex = 3;
            this.labelfollowings.Text = "0";
            // 
            // labelusername
            // 
            this.labelusername.AutoSize = true;
            this.labelusername.Location = new System.Drawing.Point(89, 18);
            this.labelusername.Name = "labelusername";
            this.labelusername.Size = new System.Drawing.Size(135, 17);
            this.labelusername.TabIndex = 3;
            this.labelusername.Text = "@screen_name / id ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(224, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Last update";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Known Blocks";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "Followers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Followings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // buttonrefreshmyinfo
            // 
            this.buttonrefreshmyinfo.Location = new System.Drawing.Point(380, 74);
            this.buttonrefreshmyinfo.Name = "buttonrefreshmyinfo";
            this.buttonrefreshmyinfo.Size = new System.Drawing.Size(75, 23);
            this.buttonrefreshmyinfo.TabIndex = 0;
            this.buttonrefreshmyinfo.Text = "Refresh";
            this.buttonrefreshmyinfo.UseVisualStyleBackColor = true;
            this.buttonrefreshmyinfo.Click += new System.EventHandler(this.buttonrefreshmyinfo_Click);
            // 
            // buttonstartstop
            // 
            this.buttonstartstop.Location = new System.Drawing.Point(53, 148);
            this.buttonstartstop.Name = "buttonstartstop";
            this.buttonstartstop.Size = new System.Drawing.Size(176, 23);
            this.buttonstartstop.TabIndex = 5;
            this.buttonstartstop.Text = "Preview";
            this.buttonstartstop.UseVisualStyleBackColor = true;
            this.buttonstartstop.Click += new System.EventHandler(this.buttonstartstop_Click);
            // 
            // buttonpausecontinue
            // 
            this.buttonpausecontinue.Enabled = false;
            this.buttonpausecontinue.Location = new System.Drawing.Point(243, 148);
            this.buttonpausecontinue.Name = "buttonpausecontinue";
            this.buttonpausecontinue.Size = new System.Drawing.Size(176, 23);
            this.buttonpausecontinue.TabIndex = 5;
            this.buttonpausecontinue.Text = "Pause";
            this.buttonpausecontinue.UseVisualStyleBackColor = true;
            this.buttonpausecontinue.Click += new System.EventHandler(this.buttonpausecontinue_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.skipThisToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(140, 30);
            // 
            // skipThisToolStripMenuItem
            // 
            this.skipThisToolStripMenuItem.Name = "skipThisToolStripMenuItem";
            this.skipThisToolStripMenuItem.Size = new System.Drawing.Size(139, 26);
            this.skipThisToolStripMenuItem.Text = "Skip this";
            this.skipThisToolStripMenuItem.Click += new System.EventHandler(this.skipThisToolStripMenuItem_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator2,
            this.importFromListToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(192, 88);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // importFromListToolStripMenuItem
            // 
            this.importFromListToolStripMenuItem.Name = "importFromListToolStripMenuItem";
            this.importFromListToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.importFromListToolStripMenuItem.Text = "Import from List";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 649);
            this.Controls.Add(this.buttonpausecontinue);
            this.Controls.Add(this.buttonstartstop);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "BlockThemAll";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem twitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadBlockDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTargetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem saveBlockDBToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button buttonresettarget;
        private System.Windows.Forms.Button buttonextractscreenname;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbSimulateOnly;
        private System.Windows.Forms.CheckBox cbKeepFollower;
        private System.Windows.Forms.CheckBox cbKeepFollowing;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonrefreshmyinfo;
        private System.Windows.Forms.Button buttonstartstop;
        private System.Windows.Forms.Button buttonpausecontinue;
        private System.Windows.Forms.CheckBox cbAutoRetryApiLimit;
        private System.Windows.Forms.ToolStripMenuItem buildBlockDBToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbMuteUserOnly;
        private System.Windows.Forms.Label labelknownblocks;
        private System.Windows.Forms.Label labellastupdate;
        private System.Windows.Forms.Label labelfollowers;
        private System.Windows.Forms.Label labelfollowings;
        private System.Windows.Forms.Label labelusername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbAutoSwitchCredApiLimit;
        private System.Windows.Forms.ToolStripMenuItem manageAPIKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registerAPIKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem skipThisToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem removeAPIKeyToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importFromListToolStripMenuItem;
    }
}