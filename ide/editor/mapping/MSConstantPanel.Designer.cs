using com;
using WebMaster.com;
namespace WebMaster.ide.editor.mapping
{
    partial class MSConstantPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSConstantPanel));
            this.rootPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_msg = new System.Windows.Forms.Label();
            this.grp_const = new System.Windows.Forms.GroupBox();
            this.panelDate = new System.Windows.Forms.Panel();
            this.panelTime = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.rbt_time = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.nud_sec = new System.Windows.Forms.NumericUpDown();
            this.nud_min = new System.Windows.Forms.NumericUpDown();
            this.nud_hour = new System.Windows.Forms.NumericUpDown();
            this.rbt_now = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tb_const = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rtb_exp = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rootPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grp_const.SuspendLayout();
            this.panelDate.SuspendLayout();
            this.panelTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_sec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_hour)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.panel1);
            this.rootPanel.Controls.Add(this.grp_const);
            this.rootPanel.Controls.Add(this.panel7);
            resources.ApplyResources(this.rootPanel, "rootPanel");
            this.rootPanel.MinimumSize = new System.Drawing.Size(200, 240);
            this.rootPanel.Name = "rootPanel";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lb_msg);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lb_msg
            // 
            resources.ApplyResources(this.lb_msg, "lb_msg");
            this.lb_msg.ForeColor = System.Drawing.Color.Red;
            this.lb_msg.Name = "lb_msg";
            // 
            // grp_const
            // 
            this.grp_const.Controls.Add(this.panelDate);
            this.grp_const.Controls.Add(this.panel2);
            resources.ApplyResources(this.grp_const, "grp_const");
            this.grp_const.Name = "grp_const";
            this.grp_const.TabStop = false;
            // 
            // panelDate
            // 
            this.panelDate.Controls.Add(this.panelTime);
            this.panelDate.Controls.Add(this.rbt_now);
            resources.ApplyResources(this.panelDate, "panelDate");
            this.panelDate.Name = "panelDate";
            // 
            // panelTime
            // 
            this.panelTime.Controls.Add(this.label8);
            this.panelTime.Controls.Add(this.rbt_time);
            this.panelTime.Controls.Add(this.label6);
            this.panelTime.Controls.Add(this.nud_sec);
            this.panelTime.Controls.Add(this.nud_min);
            this.panelTime.Controls.Add(this.nud_hour);
            resources.ApplyResources(this.panelTime, "panelTime");
            this.panelTime.Name = "panelTime";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // rbt_time
            // 
            resources.ApplyResources(this.rbt_time, "rbt_time");
            this.rbt_time.Name = "rbt_time";
            this.rbt_time.TabStop = true;
            this.rbt_time.UseVisualStyleBackColor = true;
            this.rbt_time.Click += new System.EventHandler(this.rbt_time_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // nud_sec
            // 
            resources.ApplyResources(this.nud_sec, "nud_sec");
            this.nud_sec.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nud_sec.Name = "nud_sec";
            this.nud_sec.ValueChanged += new System.EventHandler(this.nud_sec_ValueChanged);
            // 
            // nud_min
            // 
            resources.ApplyResources(this.nud_min, "nud_min");
            this.nud_min.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.nud_min.Name = "nud_min";
            this.nud_min.ValueChanged += new System.EventHandler(this.nud_min_ValueChanged);
            // 
            // nud_hour
            // 
            resources.ApplyResources(this.nud_hour, "nud_hour");
            this.nud_hour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.nud_hour.Name = "nud_hour";
            this.nud_hour.ValueChanged += new System.EventHandler(this.nud_hour_ValueChanged);
            // 
            // rbt_now
            // 
            resources.ApplyResources(this.rbt_now, "rbt_now");
            this.rbt_now.Name = "rbt_now";
            this.rbt_now.TabStop = true;
            this.rbt_now.UseVisualStyleBackColor = true;
            this.rbt_now.Click += new System.EventHandler(this.rbt_now_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tb_const);
            this.panel2.Controls.Add(this.label2);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tb_const
            // 
            resources.ApplyResources(this.tb_const, "tb_const");
            this.tb_const.Name = "tb_const";
            this.tb_const.TextChanged += new System.EventHandler(this.tb_const_TextChanged);
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
            this.panel3.ForeColor = System.Drawing.SystemColors.ButtonShadow;
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
            // MSConstantPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MSConstantPanel";
            this.rootPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grp_const.ResumeLayout(false);
            this.panelDate.ResumeLayout(false);
            this.panelDate.PerformLayout();
            this.panelTime.ResumeLayout(false);
            this.panelTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_sec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_hour)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.GroupBox grp_const;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox tb_const;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panelDate;
        private System.Windows.Forms.RadioButton rbt_time;
        private System.Windows.Forms.RadioButton rbt_now;
        private System.Windows.Forms.Panel panelTime;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nud_sec;
        private System.Windows.Forms.NumericUpDown nud_min;
        private System.Windows.Forms.NumericUpDown nud_hour;
    }
}
