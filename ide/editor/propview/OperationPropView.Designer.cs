using com;
using WebMaster.com;
namespace WebMaster.ide.editor.propview
{
    partial class OperationPropView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperationPropView));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tb_des = new System.Windows.Forms.TextBox();
            this.panelTime = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nud_sec = new System.Windows.Forms.NumericUpDown();
            this.nud_min = new System.Windows.Forms.NumericUpDown();
            this.nud_hour = new System.Windows.Forms.NumericUpDown();
            this.ckb_time = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cb_OpenURLType = new WebMaster.com.ComboBoxEx(this.components);
            this.tb_opInputWE = new System.Windows.Forms.TextBox();
            this.bt_searchWE = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_updateParam = new System.Windows.Forms.Button();
            this.label_msg = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_waittime1 = new System.Windows.Forms.NumericUpDown();
            this.bt_input = new System.Windows.Forms.Button();
            this.cb_waittime = new WebMaster.com.ComboBoxEx(this.components);
            this.cb_inputType = new WebMaster.com.ComboBoxEx(this.components);
            this.rtb_input = new System.Windows.Forms.RichTextBox();
            this.tb_waittime = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panelTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_sec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_hour)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_waittime1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_waittime)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.panel1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.tb_des);
            this.panel5.Controls.Add(this.panelTime);
            this.panel5.Controls.Add(this.label2);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // tb_des
            // 
            resources.ApplyResources(this.tb_des, "tb_des");
            this.tb_des.Name = "tb_des";
            this.tb_des.TextChanged += new System.EventHandler(this.tb_des_TextChanged);
            this.tb_des.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_des_KeyDown);
            this.tb_des.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_des_KeyUp);
            this.tb_des.Leave += new System.EventHandler(this.tb_des_Leave);
            // 
            // panelTime
            // 
            this.panelTime.Controls.Add(this.label8);
            this.panelTime.Controls.Add(this.label6);
            this.panelTime.Controls.Add(this.nud_sec);
            this.panelTime.Controls.Add(this.nud_min);
            this.panelTime.Controls.Add(this.nud_hour);
            this.panelTime.Controls.Add(this.ckb_time);
            resources.ApplyResources(this.panelTime, "panelTime");
            this.panelTime.Name = "panelTime";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
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
            // ckb_time
            // 
            resources.ApplyResources(this.ckb_time, "ckb_time");
            this.ckb_time.Name = "ckb_time";
            this.ckb_time.UseVisualStyleBackColor = true;
            this.ckb_time.CheckedChanged += new System.EventHandler(this.ckb_time_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tb_name);
            this.panel4.Controls.Add(this.label1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // tb_name
            // 
            resources.ApplyResources(this.tb_name, "tb_name");
            this.tb_name.Name = "tb_name";
            this.tb_name.TextChanged += new System.EventHandler(this.tb_name_TextChanged);
            this.tb_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyDown);
            this.tb_name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyUp);
            this.tb_name.Leave += new System.EventHandler(this.tb_name_Leave);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.bt_searchWE);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.cb_OpenURLType);
            this.panel6.Controls.Add(this.tb_opInputWE);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // cb_OpenURLType
            // 
            this.cb_OpenURLType.BackColor = System.Drawing.SystemColors.Control;
            this.cb_OpenURLType.FormattingEnabled = true;
            resources.ApplyResources(this.cb_OpenURLType, "cb_OpenURLType");
            this.cb_OpenURLType.Name = "cb_OpenURLType";
            this.cb_OpenURLType.Readonly = true;
            this.cb_OpenURLType.SelectedIndexChanged += new System.EventHandler(this.cb_OpenURLType_SelectedIndexChanged);
            this.cb_OpenURLType.SelectionChangeCommitted += new System.EventHandler(this.cb_OpenURLType_SelectionChangeCommitted);
            // 
            // tb_opInputWE
            // 
            resources.ApplyResources(this.tb_opInputWE, "tb_opInputWE");
            this.tb_opInputWE.Name = "tb_opInputWE";
            this.tb_opInputWE.ReadOnly = true;
            // 
            // bt_searchWE
            // 
            resources.ApplyResources(this.bt_searchWE, "bt_searchWE");
            this.bt_searchWE.Name = "bt_searchWE";
            this.bt_searchWE.UseVisualStyleBackColor = true;
            this.bt_searchWE.Click += new System.EventHandler(this.bt_searchWE_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_updateParam);
            this.groupBox2.Controls.Add(this.label_msg);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tb_waittime1);
            this.groupBox2.Controls.Add(this.bt_input);
            this.groupBox2.Controls.Add(this.cb_waittime);
            this.groupBox2.Controls.Add(this.cb_inputType);
            this.groupBox2.Controls.Add(this.rtb_input);
            this.groupBox2.Controls.Add(this.tb_waittime);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btn_updateParam
            // 
            resources.ApplyResources(this.btn_updateParam, "btn_updateParam");
            this.btn_updateParam.Name = "btn_updateParam";
            this.btn_updateParam.UseVisualStyleBackColor = true;
            this.btn_updateParam.Click += new System.EventHandler(this.btn_updateParam_Click);
            // 
            // label_msg
            // 
            resources.ApplyResources(this.label_msg, "label_msg");
            this.label_msg.Name = "label_msg";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // tb_waittime1
            // 
            resources.ApplyResources(this.tb_waittime1, "tb_waittime1");
            this.tb_waittime1.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.tb_waittime1.Name = "tb_waittime1";
            this.tb_waittime1.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.tb_waittime1.ValueChanged += new System.EventHandler(this.tb_waittime1_ValueChanged);
            this.tb_waittime1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_waittime1_KeyDown);
            // 
            // bt_input
            // 
            resources.ApplyResources(this.bt_input, "bt_input");
            this.bt_input.Name = "bt_input";
            this.bt_input.UseVisualStyleBackColor = true;
            this.bt_input.Click += new System.EventHandler(this.bt_input_Click);
            // 
            // cb_waittime
            // 
            this.cb_waittime.BackColor = System.Drawing.SystemColors.Control;
            this.cb_waittime.FormattingEnabled = true;
            this.cb_waittime.Items.AddRange(new object[] {
            resources.GetString("cb_waittime.Items"),
            resources.GetString("cb_waittime.Items1")});
            resources.ApplyResources(this.cb_waittime, "cb_waittime");
            this.cb_waittime.Name = "cb_waittime";
            this.cb_waittime.Readonly = true;
            this.cb_waittime.SelectedIndexChanged += new System.EventHandler(this.cb_waittime_SelectedIndexChanged);
            this.cb_waittime.SelectionChangeCommitted += new System.EventHandler(this.cb_waittime_SelectionChangeCommitted);
            // 
            // cb_inputType
            // 
            this.cb_inputType.BackColor = System.Drawing.SystemColors.Control;
            this.cb_inputType.FormattingEnabled = true;
            resources.ApplyResources(this.cb_inputType, "cb_inputType");
            this.cb_inputType.Name = "cb_inputType";
            this.cb_inputType.Readonly = true;
            this.cb_inputType.SelectedIndexChanged += new System.EventHandler(this.cb_inputType_SelectedIndexChanged);
            this.cb_inputType.SelectionChangeCommitted += new System.EventHandler(this.cb_inputType_SelectionChangeCommitted);
            // 
            // rtb_input
            // 
            resources.ApplyResources(this.rtb_input, "rtb_input");
            this.rtb_input.Name = "rtb_input";
            this.rtb_input.TextChanged += new System.EventHandler(this.rtb_input_TextChanged);
            this.rtb_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtb_input_KeyDown);
            this.rtb_input.Leave += new System.EventHandler(this.rtb_input_Leave);
            // 
            // tb_waittime
            // 
            resources.ApplyResources(this.tb_waittime, "tb_waittime");
            this.tb_waittime.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.tb_waittime.Name = "tb_waittime";
            this.tb_waittime.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.tb_waittime.ValueChanged += new System.EventHandler(this.tb_waittime_ValueChanged);
            this.tb_waittime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_waittime_KeyDown);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // OperationPropView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "OperationPropView";
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panelTime.ResumeLayout(false);
            this.panelTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_sec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_hour)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_waittime1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_waittime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_opInputWE;
        private System.Windows.Forms.TextBox tb_des;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private ComboBoxEx cb_inputType;
        private System.Windows.Forms.RichTextBox rtb_input;
        private System.Windows.Forms.NumericUpDown tb_waittime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private ComboBoxEx cb_waittime;
        private System.Windows.Forms.Button bt_searchWE;
        private System.Windows.Forms.Button bt_input;
        private System.Windows.Forms.NumericUpDown tb_waittime1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_msg;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel6;
        private ComboBoxEx cb_OpenURLType;
        private System.Windows.Forms.Button btn_updateParam;
        private System.Windows.Forms.Panel panelTime;
        private System.Windows.Forms.CheckBox ckb_time;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nud_sec;
        private System.Windows.Forms.NumericUpDown nud_min;
        private System.Windows.Forms.NumericUpDown nud_hour;
    }
}
