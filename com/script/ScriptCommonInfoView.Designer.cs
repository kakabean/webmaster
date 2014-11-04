namespace WebMaster.com.script
{
    partial class ScriptCommonInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptCommonInfoView));
            this.label1 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_des = new System.Windows.Forms.TextBox();
            this.btn_Icon = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_url = new System.Windows.Forms.TextBox();
            this.btn_getUrl = new System.Windows.Forms.Button();
            this.grpGen = new System.Windows.Forms.GroupBox();
            this.lb_msg_gen = new System.Windows.Forms.Label();
            this.btn_genVer = new System.Windows.Forms.Button();
            this.tb_version = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_timeout = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.secGrp = new System.Windows.Forms.GroupBox();
            this.btn_rmURL = new System.Windows.Forms.Button();
            this.btn_addURL = new System.Windows.Forms.Button();
            this.ckb_enableSec = new System.Windows.Forms.CheckBox();
            this.lv_url = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_contributors = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lb_author = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.grpGen.SuspendLayout();
            this.secGrp.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tb_name
            // 
            resources.ApplyResources(this.tb_name, "tb_name");
            this.tb_name.Name = "tb_name";
            this.tb_name.TextChanged += new System.EventHandler(this.tb_name_TextChanged);
            this.tb_name.Leave += new System.EventHandler(this.textBoxName_Leave);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tb_des
            // 
            resources.ApplyResources(this.tb_des, "tb_des");
            this.tb_des.Name = "tb_des";
            this.tb_des.TextChanged += new System.EventHandler(this.tb_des_TextChanged);
            this.tb_des.Leave += new System.EventHandler(this.tb_des_Leave);
            // 
            // btn_Icon
            // 
            resources.ApplyResources(this.btn_Icon, "btn_Icon");
            this.btn_Icon.Name = "btn_Icon";
            this.btn_Icon.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // tb_url
            // 
            resources.ApplyResources(this.tb_url, "tb_url");
            this.tb_url.Name = "tb_url";
            this.tb_url.TextChanged += new System.EventHandler(this.textBoxUrl_TextChanged);
            // 
            // btn_getUrl
            // 
            this.btn_getUrl.Image = global::com.Properties.Resources.addItem16;
            resources.ApplyResources(this.btn_getUrl, "btn_getUrl");
            this.btn_getUrl.Name = "btn_getUrl";
            this.btn_getUrl.UseVisualStyleBackColor = true;
            this.btn_getUrl.Click += new System.EventHandler(this.btn_getUrl_Click);
            // 
            // grpGen
            // 
            this.grpGen.Controls.Add(this.lb_msg_gen);
            this.grpGen.Controls.Add(this.btn_genVer);
            this.grpGen.Controls.Add(this.tb_version);
            this.grpGen.Controls.Add(this.label4);
            this.grpGen.Controls.Add(this.tb_timeout);
            this.grpGen.Controls.Add(this.label3);
            this.grpGen.Controls.Add(this.btn_getUrl);
            this.grpGen.Controls.Add(this.btn_Icon);
            this.grpGen.Controls.Add(this.tb_des);
            this.grpGen.Controls.Add(this.label2);
            this.grpGen.Controls.Add(this.tb_name);
            this.grpGen.Controls.Add(this.label1);
            this.grpGen.Controls.Add(this.tb_url);
            this.grpGen.Controls.Add(this.label5);
            resources.ApplyResources(this.grpGen, "grpGen");
            this.grpGen.Name = "grpGen";
            this.grpGen.TabStop = false;
            // 
            // lb_msg_gen
            // 
            this.lb_msg_gen.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.lb_msg_gen, "lb_msg_gen");
            this.lb_msg_gen.Name = "lb_msg_gen";
            // 
            // btn_genVer
            // 
            resources.ApplyResources(this.btn_genVer, "btn_genVer");
            this.btn_genVer.Name = "btn_genVer";
            this.btn_genVer.UseVisualStyleBackColor = true;
            this.btn_genVer.Click += new System.EventHandler(this.btn_genVer_Click);
            // 
            // tb_version
            // 
            resources.ApplyResources(this.tb_version, "tb_version");
            this.tb_version.Name = "tb_version";
            this.tb_version.TextChanged += new System.EventHandler(this.tb_version_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tb_timeout
            // 
            resources.ApplyResources(this.tb_timeout, "tb_timeout");
            this.tb_timeout.Name = "tb_timeout";
            this.tb_timeout.TextChanged += new System.EventHandler(this.tb_timeout_TextChanged);
            this.tb_timeout.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_timeout_KeyDown);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // secGrp
            // 
            this.secGrp.Controls.Add(this.btn_rmURL);
            this.secGrp.Controls.Add(this.btn_addURL);
            this.secGrp.Controls.Add(this.ckb_enableSec);
            this.secGrp.Controls.Add(this.lv_url);
            resources.ApplyResources(this.secGrp, "secGrp");
            this.secGrp.Name = "secGrp";
            this.secGrp.TabStop = false;
            // 
            // btn_rmURL
            // 
            this.btn_rmURL.Image = global::com.Properties.Resources.delete16;
            resources.ApplyResources(this.btn_rmURL, "btn_rmURL");
            this.btn_rmURL.Name = "btn_rmURL";
            this.btn_rmURL.UseVisualStyleBackColor = true;
            this.btn_rmURL.Click += new System.EventHandler(this.btn_rmURL_Click);
            // 
            // btn_addURL
            // 
            this.btn_addURL.Image = global::com.Properties.Resources.addItem16;
            resources.ApplyResources(this.btn_addURL, "btn_addURL");
            this.btn_addURL.Name = "btn_addURL";
            this.btn_addURL.UseVisualStyleBackColor = true;
            this.btn_addURL.Click += new System.EventHandler(this.btn_addURL_Click);
            // 
            // ckb_enableSec
            // 
            resources.ApplyResources(this.ckb_enableSec, "ckb_enableSec");
            this.ckb_enableSec.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.ckb_enableSec.Name = "ckb_enableSec";
            this.ckb_enableSec.UseVisualStyleBackColor = true;
            this.ckb_enableSec.CheckedChanged += new System.EventHandler(this.ckb_enableSec_CheckedChanged);
            // 
            // lv_url
            // 
            this.lv_url.BackColor = System.Drawing.SystemColors.Window;
            this.lv_url.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lv_url.FullRowSelect = true;
            this.lv_url.GridLines = true;
            this.lv_url.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lv_url.HideSelection = false;
            resources.ApplyResources(this.lv_url, "lv_url");
            this.lv_url.MultiSelect = false;
            this.lv_url.Name = "lv_url";
            this.lv_url.UseCompatibleStateImageBehavior = false;
            this.lv_url.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_contributors);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.lb_author);
            this.groupBox1.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tb_contributors
            // 
            resources.ApplyResources(this.tb_contributors, "tb_contributors");
            this.tb_contributors.Name = "tb_contributors";
            this.tb_contributors.ReadOnly = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // lb_author
            // 
            resources.ApplyResources(this.lb_author, "lb_author");
            this.lb_author.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lb_author.Name = "lb_author";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // ScriptCommonInfoView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.secGrp);
            this.Controls.Add(this.grpGen);
            this.Name = "ScriptCommonInfoView";
            this.grpGen.ResumeLayout(false);
            this.grpGen.PerformLayout();
            this.secGrp.ResumeLayout(false);
            this.secGrp.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_des;
        private System.Windows.Forms.Button btn_Icon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_url;
        private System.Windows.Forms.Button btn_getUrl;
        private System.Windows.Forms.GroupBox grpGen;
        private System.Windows.Forms.TextBox tb_timeout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox secGrp;
        private System.Windows.Forms.Label lb_msg_gen;
        private System.Windows.Forms.Button btn_genVer;
        private System.Windows.Forms.TextBox tb_version;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_addURL;
        private System.Windows.Forms.CheckBox ckb_enableSec;
        private System.Windows.Forms.ListView lv_url;
        private System.Windows.Forms.Button btn_rmURL;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lb_author;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_contributors;
        private System.Windows.Forms.Label label8;

        public System.Windows.Forms.KeyEventHandler textBoxName_TextChanged { get; set; }
    }
}
