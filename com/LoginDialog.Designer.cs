namespace WebMaster.com
{
    partial class LoginDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.tb_pwd = new System.Windows.Forms.TextBox();
            this.btn_login = new System.Windows.Forms.Button();
            this.ckb_savepwd = new System.Windows.Forms.CheckBox();
            this.linkLostPwd = new System.Windows.Forms.LinkLabel();
            this.linkNewAcc = new System.Windows.Forms.LinkLabel();
            this.btn_close = new System.Windows.Forms.Button();
            this.labelMsg = new System.Windows.Forms.Label();
            this.lb_loading = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tb_name
            // 
            resources.ApplyResources(this.tb_name, "tb_name");
            this.tb_name.Name = "tb_name";
            this.tb_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyDown);
            // 
            // tb_pwd
            // 
            resources.ApplyResources(this.tb_pwd, "tb_pwd");
            this.tb_pwd.Name = "tb_pwd";
            this.tb_pwd.UseSystemPasswordChar = true;
            this.tb_pwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_pwd_KeyDown);
            // 
            // btn_login
            // 
            resources.ApplyResources(this.btn_login, "btn_login");
            this.btn_login.Name = "btn_login";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // ckb_savepwd
            // 
            resources.ApplyResources(this.ckb_savepwd, "ckb_savepwd");
            this.ckb_savepwd.Name = "ckb_savepwd";
            this.ckb_savepwd.UseVisualStyleBackColor = true;
            // 
            // linkLostPwd
            // 
            resources.ApplyResources(this.linkLostPwd, "linkLostPwd");
            this.linkLostPwd.Name = "linkLostPwd";
            this.linkLostPwd.TabStop = true;
            this.linkLostPwd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLostPwd_LinkClicked);
            // 
            // linkNewAcc
            // 
            resources.ApplyResources(this.linkNewAcc, "linkNewAcc");
            this.linkNewAcc.Name = "linkNewAcc";
            this.linkNewAcc.TabStop = true;
            this.linkNewAcc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkNewAcc_LinkClicked);
            // 
            // btn_close
            // 
            this.btn_close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_close, "btn_close");
            this.btn_close.Name = "btn_close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // labelMsg
            // 
            resources.ApplyResources(this.labelMsg, "labelMsg");
            this.labelMsg.ForeColor = System.Drawing.Color.Red;
            this.labelMsg.Name = "labelMsg";
            // 
            // lb_loading
            // 
            this.lb_loading.Image = global::com.Properties.Resources.loading16;
            resources.ApplyResources(this.lb_loading, "lb_loading");
            this.lb_loading.Name = "lb_loading";
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            // 
            // LoginDialog
            // 
            this.AcceptButton = this.btn_login;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_close;
            this.Controls.Add(this.lb_loading);
            this.Controls.Add(this.labelMsg);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.linkNewAcc);
            this.Controls.Add(this.linkLostPwd);
            this.Controls.Add(this.ckb_savepwd);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.tb_pwd);
            this.Controls.Add(this.tb_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.TextBox tb_pwd;
        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.CheckBox ckb_savepwd;
        private System.Windows.Forms.LinkLabel linkLostPwd;
        private System.Windows.Forms.LinkLabel linkNewAcc;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label labelMsg;
        private System.Windows.Forms.Label lb_loading;
        private System.Windows.Forms.Timer timer1;
    }
}