using com;
using WebMaster.com;
namespace WebMaster.ide.editor.propview
{
    partial class ConPropView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConPropView));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tv_con = new System.Windows.Forms.TreeView();
            this.conImgList = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtn_OR = new System.Windows.Forms.RadioButton();
            this.rbtn_AND = new System.Windows.Forms.RadioButton();
            this.ckb_Not = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_msg = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_catagory = new WebMaster.com.ComboBoxEx(this.components);
            this.labelIType2 = new System.Windows.Forms.Label();
            this.btn_Input2 = new System.Windows.Forms.Button();
            this.btn_Input1 = new System.Windows.Forms.Button();
            this.cb_pattern = new WebMaster.com.ComboBoxEx(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.tb_input2 = new System.Windows.Forms.TextBox();
            this.tb_input1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_des = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cms_conTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.labelIType1 = new System.Windows.Forms.Label();
            this.tsmi_newCon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_newGrp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_remove = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.cms_conTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tv_con);
            this.groupBox1.Controls.Add(this.panel1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tv_con
            // 
            resources.ApplyResources(this.tv_con, "tv_con");
            this.tv_con.HideSelection = false;
            this.tv_con.ImageList = this.conImgList;
            this.tv_con.Name = "tv_con";
            this.tv_con.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_con_AfterSelect);
            this.tv_con.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseClick);
            // 
            // conImgList
            // 
            this.conImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("conImgList.ImageStream")));
            this.conImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.conImgList.Images.SetKeyName(0, "con16.png");
            this.conImgList.Images.SetKeyName(1, "con_err16.png");
            this.conImgList.Images.SetKeyName(2, "con_not16.png");
            this.conImgList.Images.SetKeyName(3, "con_not_err16.png");
            this.conImgList.Images.SetKeyName(4, "congrp_and16.png");
            this.conImgList.Images.SetKeyName(5, "congrp_and_err16.png");
            this.conImgList.Images.SetKeyName(6, "congrp_and_not16.png");
            this.conImgList.Images.SetKeyName(7, "congrp_and_not_err16.png");
            this.conImgList.Images.SetKeyName(8, "congrp_or16.png");
            this.conImgList.Images.SetKeyName(9, "congrp_or_err16.png");
            this.conImgList.Images.SetKeyName(10, "congrp_or_not16.png");
            this.conImgList.Images.SetKeyName(11, "congrp_or_not_err16.png");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtn_OR);
            this.panel1.Controls.Add(this.rbtn_AND);
            this.panel1.Controls.Add(this.ckb_Not);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // rbtn_OR
            // 
            resources.ApplyResources(this.rbtn_OR, "rbtn_OR");
            this.rbtn_OR.Name = "rbtn_OR";
            this.rbtn_OR.UseVisualStyleBackColor = true;
            this.rbtn_OR.Click += new System.EventHandler(this.rbtn_OR_Click);
            // 
            // rbtn_AND
            // 
            resources.ApplyResources(this.rbtn_AND, "rbtn_AND");
            this.rbtn_AND.Name = "rbtn_AND";
            this.rbtn_AND.UseVisualStyleBackColor = true;
            this.rbtn_AND.Click += new System.EventHandler(this.rbtn_AND_Click);
            // 
            // ckb_Not
            // 
            resources.ApplyResources(this.ckb_Not, "ckb_Not");
            this.ckb_Not.Name = "ckb_Not";
            this.ckb_Not.UseVisualStyleBackColor = true;
            this.ckb_Not.Click += new System.EventHandler(this.ckbNot_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_msg);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.tb_des);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tb_name);
            this.groupBox2.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label_msg
            // 
            resources.ApplyResources(this.label_msg, "label_msg");
            this.label_msg.Name = "label_msg";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_catagory);
            this.groupBox3.Controls.Add(this.labelIType2);
            this.groupBox3.Controls.Add(this.labelIType1);
            this.groupBox3.Controls.Add(this.btn_Input2);
            this.groupBox3.Controls.Add(this.btn_Input1);
            this.groupBox3.Controls.Add(this.cb_pattern);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.tb_input2);
            this.groupBox3.Controls.Add(this.tb_input1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cb_catagory
            // 
            this.cb_catagory.BackColor = System.Drawing.SystemColors.Control;
            this.cb_catagory.FormattingEnabled = true;
            resources.ApplyResources(this.cb_catagory, "cb_catagory");
            this.cb_catagory.Name = "cb_catagory";
            this.cb_catagory.Readonly = true;
            this.cb_catagory.SelectedIndexChanged += new System.EventHandler(this.cb_catagory_SelectedIndexChanged);
            this.cb_catagory.SelectionChangeCommitted += new System.EventHandler(this.cb_catagory_SelectionChangeCommitted);
            this.cb_catagory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cb_catagory_KeyDown);
            // 
            // labelIType2
            // 
            resources.ApplyResources(this.labelIType2, "labelIType2");
            this.labelIType2.Name = "labelIType2";
            // 
            // btn_Input2
            // 
            resources.ApplyResources(this.btn_Input2, "btn_Input2");
            this.btn_Input2.Name = "btn_Input2";
            this.btn_Input2.UseVisualStyleBackColor = true;
            this.btn_Input2.Click += new System.EventHandler(this.btn_Input2_Click);
            // 
            // btn_Input1
            // 
            resources.ApplyResources(this.btn_Input1, "btn_Input1");
            this.btn_Input1.Name = "btn_Input1";
            this.btn_Input1.UseVisualStyleBackColor = true;
            this.btn_Input1.Click += new System.EventHandler(this.btn_Input1_Click);
            // 
            // cb_pattern
            // 
            this.cb_pattern.BackColor = System.Drawing.SystemColors.Control;
            this.cb_pattern.FormattingEnabled = true;
            resources.ApplyResources(this.cb_pattern, "cb_pattern");
            this.cb_pattern.Name = "cb_pattern";
            this.cb_pattern.Readonly = true;
            this.cb_pattern.SelectedIndexChanged += new System.EventHandler(this.cb_pattern_SelectedIndexChanged);
            this.cb_pattern.SelectionChangeCommitted += new System.EventHandler(this.cb_pattern_SelectionChangeCommitted);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // tb_input2
            // 
            resources.ApplyResources(this.tb_input2, "tb_input2");
            this.tb_input2.Name = "tb_input2";
            this.tb_input2.TextChanged += new System.EventHandler(this.tb_input2_TextChanged);
            this.tb_input2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_input2_KeyDown);
            // 
            // tb_input1
            // 
            resources.ApplyResources(this.tb_input1, "tb_input1");
            this.tb_input1.Name = "tb_input1";
            this.tb_input1.TextChanged += new System.EventHandler(this.tb_input1_TextChanged);
            this.tb_input1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_input1_KeyDown);
            this.tb_input1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tb_input1_MouseDown);
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
            // cms_conTree
            // 
            this.cms_conTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_newCon,
            this.tsmi_newGrp,
            this.tsmi_remove});
            this.cms_conTree.Name = "cms_conTree";
            resources.ApplyResources(this.cms_conTree, "cms_conTree");
            // 
            // labelIType1
            // 
            resources.ApplyResources(this.labelIType1, "labelIType1");
            this.labelIType1.Name = "labelIType1";
            // 
            // tsmi_newCon
            // 
            this.tsmi_newCon.Image = global::ide.Properties.Resources.con16;
            this.tsmi_newCon.Name = "tsmi_newCon";
            resources.ApplyResources(this.tsmi_newCon, "tsmi_newCon");
            this.tsmi_newCon.Click += new System.EventHandler(this.tsmi_newCon_Click);
            // 
            // tsmi_newGrp
            // 
            this.tsmi_newGrp.Image = global::ide.Properties.Resources.newCategory16;
            this.tsmi_newGrp.Name = "tsmi_newGrp";
            resources.ApplyResources(this.tsmi_newGrp, "tsmi_newGrp");
            this.tsmi_newGrp.Click += new System.EventHandler(this.tsmi_newGrp_Click);
            // 
            // tsmi_remove
            // 
            this.tsmi_remove.Image = global::ide.Properties.Resources.delete16;
            this.tsmi_remove.Name = "tsmi_remove";
            resources.ApplyResources(this.tsmi_remove, "tsmi_remove");
            this.tsmi_remove.Click += new System.EventHandler(this.tsmi_remove_Click);
            // 
            // ConPropView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "ConPropView";
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.cms_conTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckb_Not;
        private System.Windows.Forms.TreeView tv_con;
        private System.Windows.Forms.RadioButton rbtn_OR;
        private System.Windows.Forms.RadioButton rbtn_AND;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private ComboBoxEx cb_pattern;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_input2;
        private System.Windows.Forms.TextBox tb_input1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_des;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Input2;
        private System.Windows.Forms.Button btn_Input1;
        private System.Windows.Forms.Label label_msg;
        private System.Windows.Forms.ContextMenuStrip cms_conTree;
        private System.Windows.Forms.ToolStripMenuItem tsmi_newCon;
        private System.Windows.Forms.ToolStripMenuItem tsmi_newGrp;
        private System.Windows.Forms.ToolStripMenuItem tsmi_remove;
        private System.Windows.Forms.Label labelIType2;
        private System.Windows.Forms.Label labelIType1;
        private System.Windows.Forms.Panel panel1;
        private ComboBoxEx cb_catagory;
        private System.Windows.Forms.ImageList conImgList;
    }
}
