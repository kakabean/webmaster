namespace WebMaster.browser
{
    partial class WMBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WMBrowser));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_back = new System.Windows.Forms.ToolStripButton();
            this.tsb_fwd = new System.Windows.Forms.ToolStripButton();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_refresh = new System.Windows.Forms.ToolStripButton();
            this.ts_url = new System.Windows.Forms.ToolStripTextBox();
            this.tsddb_settings = new System.Windows.Forms.ToolStripDropDownButton();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tss5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_login = new System.Windows.Forms.ToolStripButton();
            this.tsl_space = new System.Windows.Forms.ToolStripLabel();
            this.tss4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_run = new System.Windows.Forms.ToolStripButton();
            this.tss3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_Load = new System.Windows.Forms.ToolStripButton();
            this.tss2 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssl_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_user = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_help = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_home = new System.Windows.Forms.ToolStripStatusLabel();
            this.tss_msg = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControlEx1 = new WebMaster.com.table.TabControlEx();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_back,
            this.tsb_fwd,
            this.tss1,
            this.tsb_refresh,
            this.ts_url,
            this.tsddb_settings,
            this.tss5,
            this.tsb_login,
            this.tsl_space,
            this.tss4,
            this.tsb_run,
            this.tss3,
            this.tsb_Load,
            this.tss2});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.SizeChanged += new System.EventHandler(this.toolStrip1_SizeChanged);
            // 
            // tsb_back
            // 
            this.tsb_back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_back, "tsb_back");
            this.tsb_back.Image = global::browser.Properties.Resources.back16;
            this.tsb_back.Name = "tsb_back";
            this.tsb_back.Click += new System.EventHandler(this.tsb_back_Click);
            // 
            // tsb_fwd
            // 
            this.tsb_fwd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_fwd, "tsb_fwd");
            this.tsb_fwd.Image = global::browser.Properties.Resources.forward16;
            this.tsb_fwd.Name = "tsb_fwd";
            this.tsb_fwd.Click += new System.EventHandler(this.tsb_fwd_Click);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            resources.ApplyResources(this.tss1, "tss1");
            // 
            // tsb_refresh
            // 
            this.tsb_refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_refresh.Image = global::browser.Properties.Resources.refresh16;
            resources.ApplyResources(this.tsb_refresh, "tsb_refresh");
            this.tsb_refresh.Name = "tsb_refresh";
            this.tsb_refresh.Click += new System.EventHandler(this.tsb_refresh_Click);
            // 
            // ts_url
            // 
            this.ts_url.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ts_url.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.ts_url.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ts_url.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ts_url.Name = "ts_url";
            resources.ApplyResources(this.ts_url, "ts_url");
            this.ts_url.Tag = "flag";
            this.ts_url.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ts_url_KeyDown);
            // 
            // tsddb_settings
            // 
            this.tsddb_settings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsddb_settings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsddb_settings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutUsToolStripMenuItem});
            this.tsddb_settings.Image = global::browser.Properties.Resources.settings16;
            resources.ApplyResources(this.tsddb_settings, "tsddb_settings");
            this.tsddb_settings.Name = "tsddb_settings";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            resources.ApplyResources(this.updateToolStripMenuItem, "updateToolStripMenuItem");
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // aboutUsToolStripMenuItem
            // 
            this.aboutUsToolStripMenuItem.Name = "aboutUsToolStripMenuItem";
            resources.ApplyResources(this.aboutUsToolStripMenuItem, "aboutUsToolStripMenuItem");
            this.aboutUsToolStripMenuItem.Click += new System.EventHandler(this.aboutUsToolStripMenuItem_Click);
            // 
            // tss5
            // 
            this.tss5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss5.Name = "tss5";
            resources.ApplyResources(this.tss5, "tss5");
            // 
            // tsb_login
            // 
            this.tsb_login.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_login.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tsb_login, "tsb_login");
            this.tsb_login.Name = "tsb_login";
            this.tsb_login.Click += new System.EventHandler(this.tsbLogin_Click);
            // 
            // tsl_space
            // 
            this.tsl_space.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsl_space.Name = "tsl_space";
            resources.ApplyResources(this.tsl_space, "tsl_space");
            // 
            // tss4
            // 
            this.tss4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss4.Name = "tss4";
            resources.ApplyResources(this.tss4, "tss4");
            // 
            // tsb_run
            // 
            this.tsb_run.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_run, "tsb_run");
            this.tsb_run.Image = global::browser.Properties.Resources.run_script16;
            this.tsb_run.Name = "tsb_run";
            this.tsb_run.Tag = "debug";
            this.tsb_run.Click += new System.EventHandler(this.tsb_run_Click);
            // 
            // tss3
            // 
            this.tss3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss3.Name = "tss3";
            resources.ApplyResources(this.tss3, "tss3");
            // 
            // tsb_Load
            // 
            this.tsb_Load.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_Load.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_Load, "tsb_Load");
            this.tsb_Load.Image = global::browser.Properties.Resources.load16;
            this.tsb_Load.Name = "tsb_Load";
            this.tsb_Load.Click += new System.EventHandler(this.tsb_Click_loadScript);
            // 
            // tss2
            // 
            this.tss2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss2.Name = "tss2";
            resources.ApplyResources(this.tss2, "tss2");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssl_status,
            this.tssl_user,
            this.tssl_help,
            this.tssl_home,
            this.tss_msg});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            // 
            // tssl_status
            // 
            resources.ApplyResources(this.tssl_status, "tssl_status");
            this.tssl_status.Name = "tssl_status";
            // 
            // tssl_user
            // 
            this.tssl_user.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tssl_user.AutoToolTip = true;
            this.tssl_user.Image = global::browser.Properties.Resources.userOffline16;
            this.tssl_user.Name = "tssl_user";
            resources.ApplyResources(this.tssl_user, "tssl_user");
            // 
            // tssl_help
            // 
            this.tssl_help.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tssl_help.Image = global::browser.Properties.Resources.help16;
            this.tssl_help.Name = "tssl_help";
            resources.ApplyResources(this.tssl_help, "tssl_help");
            // 
            // tssl_home
            // 
            this.tssl_home.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tssl_home.Image = global::browser.Properties.Resources.home16;
            this.tssl_home.Name = "tssl_home";
            resources.ApplyResources(this.tssl_home, "tssl_home");
            // 
            // tss_msg
            // 
            this.tss_msg.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss_msg.Image = global::browser.Properties.Resources.newmsg16;
            this.tss_msg.Name = "tss_msg";
            resources.ApplyResources(this.tss_msg, "tss_msg");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "ws";
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // tabControlEx1
            // 
            this.tabControlEx1.barBGColor1 = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.tabControlEx1, "tabControlEx1");
            this.tabControlEx1.Name = "tabControlEx1";
            this.tabControlEx1.ActiveTabPage += new System.EventHandler<WebMaster.com.table.TabPageActiveEventArgs>(this.tabControlEx1_ActiveTabPage);
            this.tabControlEx1.NewTabPage += new System.EventHandler<WebMaster.com.table.TabPageActiveEventArgs>(this.tabControlEx1_NewTabPage);
            // 
            // WMBrowser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlEx1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "WMBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WMBrowser_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WMBrowser_FormClosed);
            this.Load += new System.EventHandler(this.WMBrowser_Load);
            this.Move += new System.EventHandler(this.WMBrowser_Move);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_back;
        private System.Windows.Forms.ToolStripButton tsb_fwd;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripButton tsb_refresh;
        private System.Windows.Forms.ToolStripTextBox ts_url;
        private System.Windows.Forms.ToolStripDropDownButton tsddb_settings;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutUsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsb_login;
        private System.Windows.Forms.ToolStripLabel tsl_space;
        private System.Windows.Forms.ToolStripSeparator tss5;
        private System.Windows.Forms.ToolStripSeparator tss4;
        private System.Windows.Forms.ToolStripButton tsb_run;
        private System.Windows.Forms.ToolStripSeparator tss3;
        private System.Windows.Forms.ToolStripButton tsb_Load;
        private System.Windows.Forms.ToolStripSeparator tss2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssl_status;
        private System.Windows.Forms.ToolStripStatusLabel tssl_help;
        private System.Windows.Forms.ToolStripStatusLabel tssl_home;
        private System.Windows.Forms.ToolStripStatusLabel tss_msg;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private com.table.TabControlEx tabControlEx1;
        private System.Windows.Forms.ToolStripStatusLabel tssl_user;
    }
}