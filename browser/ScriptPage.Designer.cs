namespace WebMaster.browser
{
    partial class ScriptPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptPage));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_browser = new System.Windows.Forms.TabPage();
            this.tab_script = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.globalParamConfigView1 = new WebMaster.com.script.GlobalParamConfigView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.scriptCommonInfoView1 = new WebMaster.com.script.ScriptCommonInfoView();
            this.tab_log = new System.Windows.Forms.TabPage();
            this.btn_cleanLog = new System.Windows.Forms.Button();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.tab_script.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tab_log.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tab_browser);
            this.tabControl1.Controls.Add(this.tab_script);
            this.tabControl1.Controls.Add(this.tab_log);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tab_browser
            // 
            resources.ApplyResources(this.tab_browser, "tab_browser");
            this.tab_browser.Name = "tab_browser";
            this.tab_browser.UseVisualStyleBackColor = true;
            // 
            // tab_script
            // 
            this.tab_script.Controls.Add(this.panel2);
            resources.ApplyResources(this.tab_script, "tab_script");
            this.tab_script.Name = "tab_script";
            this.tab_script.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.globalParamConfigView1);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.scriptCommonInfoView1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // globalParamConfigView1
            // 
            resources.ApplyResources(this.globalParamConfigView1, "globalParamConfigView1");
            this.globalParamConfigView1.Name = "globalParamConfigView1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.btn_save);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.ForeColor = System.Drawing.Color.Red;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // btn_save
            // 
            resources.ApplyResources(this.btn_save, "btn_save");
            this.btn_save.Name = "btn_save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // scriptCommonInfoView1
            // 
            resources.ApplyResources(this.scriptCommonInfoView1, "scriptCommonInfoView1");
            this.scriptCommonInfoView1.Name = "scriptCommonInfoView1";
            this.scriptCommonInfoView1.textBoxName_TextChanged = null;
            // 
            // tab_log
            // 
            this.tab_log.Controls.Add(this.btn_cleanLog);
            this.tab_log.Controls.Add(this.rtb_Log);
            resources.ApplyResources(this.tab_log, "tab_log");
            this.tab_log.Name = "tab_log";
            this.tab_log.UseVisualStyleBackColor = true;
            this.tab_log.SizeChanged += new System.EventHandler(this.tab_log_SizeChanged);
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
            this.rtb_Log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtb_Log, "rtb_Log");
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // ScriptPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "ScriptPage";
            this.tabControl1.ResumeLayout(false);
            this.tab_script.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tab_log.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_browser;
        private System.Windows.Forms.TabPage tab_script;
        private System.Windows.Forms.TabPage tab_log;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private WebMaster.com.script.ScriptCommonInfoView scriptCommonInfoView1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_save;
        private com.script.GlobalParamConfigView globalParamConfigView1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_cleanLog;
        private System.Windows.Forms.TextBox textBox1;

    }
}
