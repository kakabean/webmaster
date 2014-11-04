using com;
using WebMaster.com;
namespace WebMaster.ide.editor.propview
{
    partial class ParamPropView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParamPropView));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckb_pwd = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.tb_des = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_type = new WebMaster.com.ComboBoxEx(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label_msg = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_setAccess = new WebMaster.com.ComboBoxEx(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_setType = new WebMaster.com.ComboBoxEx(this.components);
            this.rtb_value = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_set = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckb_pwd);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_name);
            this.groupBox1.Controls.Add(this.tb_des);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cb_type);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label_msg);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.SizeChanged += new System.EventHandler(this.groupBox1_SizeChanged);
            // 
            // ckb_pwd
            // 
            resources.ApplyResources(this.ckb_pwd, "ckb_pwd");
            this.ckb_pwd.Name = "ckb_pwd";
            this.ckb_pwd.UseVisualStyleBackColor = true;
            this.ckb_pwd.CheckedChanged += new System.EventHandler(this.ckb_pwd_CheckedChanged);
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
            this.tb_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyDown);
            this.tb_name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyUp);
            this.tb_name.Leave += new System.EventHandler(this.tb_name_Leave);
            // 
            // tb_des
            // 
            resources.ApplyResources(this.tb_des, "tb_des");
            this.tb_des.Name = "tb_des";
            this.tb_des.TextChanged += new System.EventHandler(this.tb_des_TextChanged);
            this.tb_des.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_des_KeyDown);
            this.tb_des.Leave += new System.EventHandler(this.tb_des_Leave);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cb_type
            // 
            this.cb_type.BackColor = System.Drawing.SystemColors.Control;
            this.cb_type.FormattingEnabled = true;
            resources.ApplyResources(this.cb_type, "cb_type");
            this.cb_type.Name = "cb_type";
            this.cb_type.Readonly = true;
            this.cb_type.SelectedIndexChanged += new System.EventHandler(this.cb_type_SelectedIndexChanged);
            this.cb_type.SelectionChangeCommitted += new System.EventHandler(this.cb_type_SelectionChangeCommitted);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label_msg
            // 
            resources.ApplyResources(this.label_msg, "label_msg");
            this.label_msg.Name = "label_msg";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_setAccess);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cb_setType);
            this.groupBox2.Controls.Add(this.rtb_value);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btn_set);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cb_setAccess
            // 
            this.cb_setAccess.BackColor = System.Drawing.SystemColors.Control;
            this.cb_setAccess.FormattingEnabled = true;
            resources.ApplyResources(this.cb_setAccess, "cb_setAccess");
            this.cb_setAccess.Name = "cb_setAccess";
            this.cb_setAccess.Readonly = true;
            this.cb_setAccess.SelectedIndexChanged += new System.EventHandler(this.cb_setAccess_SelectedIndexChanged);
            this.cb_setAccess.SelectionChangeCommitted += new System.EventHandler(this.cb_setAccess_SelectionChangeCommitted);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // cb_setType
            // 
            this.cb_setType.BackColor = System.Drawing.SystemColors.Control;
            this.cb_setType.FormattingEnabled = true;
            resources.ApplyResources(this.cb_setType, "cb_setType");
            this.cb_setType.Name = "cb_setType";
            this.cb_setType.Readonly = true;
            this.cb_setType.SelectedIndexChanged += new System.EventHandler(this.cb_setType_SelectedIndexChanged);
            this.cb_setType.SelectionChangeCommitted += new System.EventHandler(this.cb_setType_SelectionChangeCommitted);
            // 
            // rtb_value
            // 
            resources.ApplyResources(this.rtb_value, "rtb_value");
            this.rtb_value.Name = "rtb_value";
            this.rtb_value.TextChanged += new System.EventHandler(this.rtb_value_TextChanged);
            this.rtb_value.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtb_value_KeyDown);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // btn_set
            // 
            resources.ApplyResources(this.btn_set, "btn_set");
            this.btn_set.Name = "btn_set";
            this.btn_set.UseVisualStyleBackColor = true;
            this.btn_set.Click += new System.EventHandler(this.btn_set_Click);
            // 
            // ParamPropView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "ParamPropView";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_des;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_set;
        private System.Windows.Forms.Label label_msg;
        private ComboBoxEx cb_setType;
        private System.Windows.Forms.Label label7;
        private ComboBoxEx cb_type;
        private System.Windows.Forms.Label label6;
        private ComboBoxEx cb_setAccess;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rtb_value;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox ckb_pwd;
    }
}
