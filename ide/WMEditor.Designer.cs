using WebMaster.ide.ui;
using WebMaster.lib.engine;
using WebMaster.com.script;
namespace WebMaster.ide
{
    partial class WMEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WMEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.webBrowserEx1 = new WebMaster.lib.ui.browser.WebBrowserEx();
            this.wePanel = new WebMaster.ide.ui.WebElementPanel();
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
            this.tss6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_login = new System.Windows.Forms.ToolStripButton();
            this.tsl_space = new System.Windows.Forms.ToolStripLabel();
            this.tsb_publish = new System.Windows.Forms.ToolStripButton();
            this.tss5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_check = new System.Windows.Forms.ToolStripButton();
            this.tss4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_PropertyPanel = new System.Windows.Forms.ToolStripButton();
            this.tss3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_Load = new System.Windows.Forms.ToolStripButton();
            this.tsb_Save = new System.Windows.Forms.ToolStripButton();
            this.tsb_New = new System.Windows.Forms.ToolStripButton();
            this.tss2 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_user = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.flowEditor1 = new WebMaster.ide.editor.FlowEditor();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.scriptAdvancedConfigView1 = new WebMaster.ide.ui.ScriptAdvancedConfigView();
            this.scriptCommonInfoView1 = new WebMaster.com.script.ScriptCommonInfoView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btn_cleanLog = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.webBrowserEx1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.wePanel);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // webBrowserEx1
            // 
            this.webBrowserEx1.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowserEx1, "webBrowserEx1");
            this.webBrowserEx1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserEx1.MinimumSize = new System.Drawing.Size(20, 22);
            this.webBrowserEx1.Name = "webBrowserEx1";
            this.webBrowserEx1.ScriptErrorsSuppressed = true;
            this.webBrowserEx1.Downloading += new System.EventHandler(this.webBrowserEx1_Downloading);
            this.webBrowserEx1.DownloadComplete += new System.EventHandler(this.webBrowserEx1_DownloadComplete);
            this.webBrowserEx1.StartNewWindow += new System.EventHandler<WebMaster.lib.ui.browser.BrowserExtendedNavigatingEventArgs>(this.webBrowserEx1_StartNewWindow);
            this.webBrowserEx1.CanGoBackChanged += new System.EventHandler(this.webBrowserEx1_CanGoBackChanged);
            this.webBrowserEx1.CanGoForwardChanged += new System.EventHandler(this.webBrowserEx1_CanGoForwardChanged);
            this.webBrowserEx1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserEx1_DocumentCompleted);
            this.webBrowserEx1.DocumentTitleChanged += new System.EventHandler(this.webBrowserEx1_DocumentTitleChanged);
            this.webBrowserEx1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserEx1_Navigated);
            this.webBrowserEx1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserEx1_Navigating);
            this.webBrowserEx1.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowserEx1_NewWindow);
            this.webBrowserEx1.StatusTextChanged += new System.EventHandler(this.webBrowserEx1_StatusTextChanged);
            // 
            // wePanel
            // 
            resources.ApplyResources(this.wePanel, "wePanel");
            this.wePanel.Name = "wePanel";
            this.wePanel.NewWebElementAddedEvt += new System.EventHandler<WebMaster.lib.engine.CommonEventArgs>(this.wePanel_NewWebElementAddedEvt);
            this.wePanel.UpdateExistedWebElementEvt += new System.EventHandler<WebMaster.lib.engine.CommonEventArgs>(this.wePanel_UpdateExistedWebElementEvt);
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
            this.tss6,
            this.tsb_login,
            this.tsl_space,
            this.tsb_publish,
            this.tss5,
            this.tsb_check,
            this.tss4,
            this.tsb_PropertyPanel,
            this.tss3,
            this.tsb_Load,
            this.tsb_Save,
            this.tsb_New,
            this.tss2});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.SizeChanged += new System.EventHandler(this.toolStrip1_SizeChanged);
            // 
            // tsb_back
            // 
            this.tsb_back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_back, "tsb_back");
            this.tsb_back.Image = global::ide.Properties.Resources.back16;
            this.tsb_back.Name = "tsb_back";
            this.tsb_back.Click += new System.EventHandler(this.tsb_back_Click);
            // 
            // tsb_fwd
            // 
            this.tsb_fwd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_fwd, "tsb_fwd");
            this.tsb_fwd.Image = global::ide.Properties.Resources.forward16;
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
            this.tsb_refresh.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_refresh.Image = global::ide.Properties.Resources.refresh16;
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
            this.ts_url.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripUrl_KeyDown);
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
            this.tsddb_settings.Image = global::ide.Properties.Resources.settings16;
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
            // tss6
            // 
            this.tss6.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss6.Name = "tss6";
            resources.ApplyResources(this.tss6, "tss6");
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
            // tsb_publish
            // 
            this.tsb_publish.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_publish.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_publish, "tsb_publish");
            this.tsb_publish.Image = global::ide.Properties.Resources.publish16;
            this.tsb_publish.Name = "tsb_publish";
            this.tsb_publish.Click += new System.EventHandler(this.tsb_publish_Click);
            // 
            // tss5
            // 
            this.tss5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss5.Name = "tss5";
            resources.ApplyResources(this.tss5, "tss5");
            // 
            // tsb_check
            // 
            this.tsb_check.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_check.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_check, "tsb_check");
            this.tsb_check.Image = global::ide.Properties.Resources.check16;
            this.tsb_check.Name = "tsb_check";
            this.tsb_check.Click += new System.EventHandler(this.tsb_check_Click);
            // 
            // tss4
            // 
            this.tss4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tss4.Name = "tss4";
            resources.ApplyResources(this.tss4, "tss4");
            // 
            // tsb_PropertyPanel
            // 
            this.tsb_PropertyPanel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_PropertyPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_PropertyPanel, "tsb_PropertyPanel");
            this.tsb_PropertyPanel.Image = global::ide.Properties.Resources.debug16;
            this.tsb_PropertyPanel.Name = "tsb_PropertyPanel";
            this.tsb_PropertyPanel.Tag = "debug";
            this.tsb_PropertyPanel.Click += new System.EventHandler(this.tsb_PropertyPanel_Click);
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
            this.tsb_Load.Image = global::ide.Properties.Resources.load16;
            this.tsb_Load.Name = "tsb_Load";
            this.tsb_Load.Click += new System.EventHandler(this.tsb_Click_loadScript);
            // 
            // tsb_Save
            // 
            this.tsb_Save.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_Save, "tsb_Save");
            this.tsb_Save.Image = global::ide.Properties.Resources.save16;
            this.tsb_Save.Name = "tsb_Save";
            this.tsb_Save.Click += new System.EventHandler(this.tsb_Click_saveScript);
            // 
            // tsb_New
            // 
            this.tsb_New.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_New.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_New.Image = global::ide.Properties.Resources.new16;
            resources.ApplyResources(this.tsb_New, "tsb_New");
            this.tsb_New.Name = "tsb_New";
            this.tsb_New.Click += new System.EventHandler(this.tsb_Click_CreateScript);
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
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel2,
            this.tssl_user});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripStatusLabel4.Image = global::ide.Properties.Resources.help16;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            resources.ApplyResources(this.toolStripStatusLabel4, "toolStripStatusLabel4");
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripStatusLabel3.Image = global::ide.Properties.Resources.home16;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            resources.ApplyResources(this.toolStripStatusLabel3, "toolStripStatusLabel3");
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripStatusLabel2.Image = global::ide.Properties.Resources.newmsg16;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // tssl_user
            // 
            this.tssl_user.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tssl_user.AutoToolTip = true;
            this.tssl_user.Image = global::ide.Properties.Resources.userOffline16;
            this.tssl_user.Name = "tssl_user";
            resources.ApplyResources(this.tssl_user, "tssl_user");
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.flowEditor1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // flowEditor1
            // 
            this.flowEditor1.App = null;
            resources.ApplyResources(this.flowEditor1, "flowEditor1");
            this.flowEditor1.Name = "flowEditor1";
            this.flowEditor1.SelectedObj = null;
            this.flowEditor1.UpdateExistedWebElementEvt += new System.EventHandler<WebMaster.lib.engine.CommonEventArgs>(this.flowEditor1_UpdateWebElement);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.scriptAdvancedConfigView1);
            this.tabPage3.Controls.Add(this.scriptCommonInfoView1);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // scriptAdvancedConfigView1
            // 
            resources.ApplyResources(this.scriptAdvancedConfigView1, "scriptAdvancedConfigView1");
            this.scriptAdvancedConfigView1.Name = "scriptAdvancedConfigView1";
            // 
            // scriptCommonInfoView1
            // 
            resources.ApplyResources(this.scriptCommonInfoView1, "scriptCommonInfoView1");
            this.scriptCommonInfoView1.Name = "scriptCommonInfoView1";
            this.scriptCommonInfoView1.textBoxName_TextChanged = null;
            this.scriptCommonInfoView1.UpdateUrlTextBoxEvt += new System.EventHandler<WebMaster.lib.engine.CommonEventArgs>(this.scriptCommonInfoView1_UpdateUrlTextBoxEvt);
            this.scriptCommonInfoView1.UpdateModelEvt += new System.EventHandler<WebMaster.lib.engine.CommonEventArgs>(this.scriptCommonInfoView1_UpdateModelEvt);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btn_cleanLog);
            this.tabPage4.Controls.Add(this.rtb_Log);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.SizeChanged += new System.EventHandler(this.tabPage4_SizeChanged);
            // 
            // btn_cleanLog
            // 
            resources.ApplyResources(this.btn_cleanLog, "btn_cleanLog");
            this.btn_cleanLog.Name = "btn_cleanLog";
            this.btn_cleanLog.UseVisualStyleBackColor = true;
            this.btn_cleanLog.Click += new System.EventHandler(this.btn_cleanLog_Click);
            // 
            // rtb_Log
            // 
            resources.ApplyResources(this.rtb_Log, "rtb_Log");
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "wd";
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // WMEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "WMEditor";
            this.Deactivate += new System.EventHandler(this.WMEditor_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WMEditor_FormClosed);
            this.Load += new System.EventHandler(this.WMEditor_Load);
            this.Move += new System.EventHandler(this.WMEditor_Move);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripButton tsb_back;
        private System.Windows.Forms.ToolStripButton tsb_fwd;
        private System.Windows.Forms.ToolStripTextBox ts_url;
        private System.Windows.Forms.ToolStripButton tsb_refresh;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripButton tsb_New;
        private System.Windows.Forms.ToolStripButton tsb_Save;
        private System.Windows.Forms.ToolStripButton tsb_Load;
        private System.Windows.Forms.ToolStripSeparator tss3;
        private System.Windows.Forms.ToolStripButton tsb_login;
        private System.Windows.Forms.ToolStripSeparator tss6;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private WebMaster.lib.ui.browser.WebBrowserEx webBrowserEx1;        
        private WebElementPanel wePanel;
        private System.Windows.Forms.TabPage tabPage3;
        private ScriptCommonInfoView scriptCommonInfoView1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private editor.FlowEditor flowEditor1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripSeparator tss5;
        private System.Windows.Forms.ToolStripButton tsb_PropertyPanel;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ToolStripButton tsb_check;
        private System.Windows.Forms.ToolStripSeparator tss4;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.ToolStripSeparator tss2;
        private System.Windows.Forms.ToolStripDropDownButton tsddb_settings;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutUsToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel tsl_space;
        private ScriptAdvancedConfigView scriptAdvancedConfigView1;
        private System.Windows.Forms.ToolStripButton tsb_publish;
        private System.Windows.Forms.Button btn_cleanLog;
        private System.Windows.Forms.ToolStripStatusLabel tssl_user;
    }
}