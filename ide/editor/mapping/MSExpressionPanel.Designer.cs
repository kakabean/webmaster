namespace WebMaster.ide.editor.mapping
{
    partial class MSExpressionPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSExpressionPanel));
            this.rootPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lb_msg = new System.Windows.Forms.Label();
            this.grpExp = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_op = new WebMaster.com.ComboBoxEx(this.components);
            this.bt_right = new System.Windows.Forms.Button();
            this.bt_left = new System.Windows.Forms.Button();
            this.tb_right = new System.Windows.Forms.TextBox();
            this.tb_left = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rtb_exp = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rootPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.grpExp.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.panel2);
            this.rootPanel.Controls.Add(this.grpExp);
            this.rootPanel.Controls.Add(this.panel7);
            resources.ApplyResources(this.rootPanel, "rootPanel");
            this.rootPanel.Name = "rootPanel";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lb_msg);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // lb_msg
            // 
            resources.ApplyResources(this.lb_msg, "lb_msg");
            this.lb_msg.ForeColor = System.Drawing.Color.Red;
            this.lb_msg.Name = "lb_msg";
            // 
            // grpExp
            // 
            this.grpExp.Controls.Add(this.panel1);
            resources.ApplyResources(this.grpExp, "grpExp");
            this.grpExp.Name = "grpExp";
            this.grpExp.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.cb_op);
            this.panel1.Controls.Add(this.bt_right);
            this.panel1.Controls.Add(this.bt_left);
            this.panel1.Controls.Add(this.tb_right);
            this.panel1.Controls.Add(this.tb_left);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cb_op
            // 
            this.cb_op.BackColor = System.Drawing.SystemColors.Control;
            this.cb_op.FormattingEnabled = true;
            resources.ApplyResources(this.cb_op, "cb_op");
            this.cb_op.Name = "cb_op";
            this.cb_op.Readonly = true;
            this.cb_op.SelectedIndexChanged += new System.EventHandler(this.cb_op_SelectedIndexChanged);
            this.cb_op.SelectionChangeCommitted += new System.EventHandler(this.cb_op_SelectionChangeCommitted);
            // 
            // bt_right
            // 
            resources.ApplyResources(this.bt_right, "bt_right");
            this.bt_right.Name = "bt_right";
            this.bt_right.UseVisualStyleBackColor = true;
            this.bt_right.Click += new System.EventHandler(this.bt_right_Click);
            // 
            // bt_left
            // 
            resources.ApplyResources(this.bt_left, "bt_left");
            this.bt_left.Name = "bt_left";
            this.bt_left.UseVisualStyleBackColor = true;
            this.bt_left.Click += new System.EventHandler(this.bt_left_Click);
            // 
            // tb_right
            // 
            resources.ApplyResources(this.tb_right, "tb_right");
            this.tb_right.Name = "tb_right";
            this.tb_right.ReadOnly = true;
            // 
            // tb_left
            // 
            resources.ApplyResources(this.tb_left, "tb_left");
            this.tb_left.Name = "tb_left";
            this.tb_left.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel3);
            this.panel7.Controls.Add(this.label7);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rtb_exp);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // rtb_exp
            // 
            this.rtb_exp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtb_exp, "rtb_exp");
            this.rtb_exp.Name = "rtb_exp";
            this.rtb_exp.ReadOnly = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // MSExpressionPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MSExpressionPanel";
            this.rootPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.grpExp.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.RichTextBox rtb_exp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpExp;
        private System.Windows.Forms.Panel panel1;
        private WebMaster.com.ComboBoxEx cb_op;
        private System.Windows.Forms.Button bt_right;
        private System.Windows.Forms.Button bt_left;
        private System.Windows.Forms.TextBox tb_right;
        private System.Windows.Forms.TextBox tb_left;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.Panel panel3;
    }
}
