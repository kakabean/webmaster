using com;
using WebMaster.com;
namespace WebMaster.ide.editor.mapping
{
    partial class ParamMappingDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParamMappingDlg));
            this.panel_title = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.rtb_header = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.grp_src = new System.Windows.Forms.GroupBox();
            this.panel_src = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cb_elemType = new WebMaster.com.ComboBoxEx(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.cb_mappingType = new WebMaster.com.ComboBoxEx(this.components);
            this.panel_op = new System.Windows.Forms.Panel();
            this.btn_unbindAll = new System.Windows.Forms.Button();
            this.btn_unbind = new System.Windows.Forms.Button();
            this.btn_update = new System.Windows.Forms.Button();
            this.btn_bind = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_cmd = new WebMaster.com.ComboBoxEx(this.components);
            this.grp_target = new System.Windows.Forms.GroupBox();
            this.tv_tgtParam = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_targetProc = new WebMaster.com.ComboBoxEx(this.components);
            this.panel7 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_set_type = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_p_type = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lvex_mappings = new WebMaster.com.ListViewEx();
            this.colFrom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCmt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lb_msg = new System.Windows.Forms.Label();
            this.panel_title.SuspendLayout();
            this.panel2.SuspendLayout();
            this.grp_src.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel_op.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grp_target.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_title
            // 
            this.panel_title.Controls.Add(this.label2);
            this.panel_title.Controls.Add(this.rtb_header);
            resources.ApplyResources(this.panel_title, "panel_title");
            this.panel_title.Name = "panel_title";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // rtb_header
            // 
            resources.ApplyResources(this.rtb_header, "rtb_header");
            this.rtb_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtb_header.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.rtb_header.Name = "rtb_header";
            this.rtb_header.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.grp_src);
            this.panel2.Controls.Add(this.panel_op);
            this.panel2.Controls.Add(this.grp_target);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            this.panel2.SizeChanged += new System.EventHandler(this.panel2_SizeChanged);
            // 
            // grp_src
            // 
            this.grp_src.Controls.Add(this.panel_src);
            this.grp_src.Controls.Add(this.panel8);
            resources.ApplyResources(this.grp_src, "grp_src");
            this.grp_src.Name = "grp_src";
            this.grp_src.TabStop = false;
            // 
            // panel_src
            // 
            resources.ApplyResources(this.panel_src, "panel_src");
            this.panel_src.Name = "panel_src";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel6);
            this.panel8.Controls.Add(this.cb_mappingType);
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.Name = "panel8";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.cb_elemType);
            this.panel6.Controls.Add(this.label1);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // cb_elemType
            // 
            this.cb_elemType.BackColor = System.Drawing.SystemColors.Control;
            this.cb_elemType.FormattingEnabled = true;
            resources.ApplyResources(this.cb_elemType, "cb_elemType");
            this.cb_elemType.Name = "cb_elemType";
            this.cb_elemType.Readonly = true;
            this.cb_elemType.SelectedIndexChanged += new System.EventHandler(this.cb_elemType_SelectedIndexChanged);
            this.cb_elemType.SelectionChangeCommitted += new System.EventHandler(this.cb_elemType_SelectionChangeCommitted);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cb_mappingType
            // 
            this.cb_mappingType.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cb_mappingType, "cb_mappingType");
            this.cb_mappingType.FormattingEnabled = true;
            this.cb_mappingType.Name = "cb_mappingType";
            this.cb_mappingType.Readonly = true;
            this.cb_mappingType.SelectedIndexChanged += new System.EventHandler(this.cb_mappingType_SelectedIndexChanged);
            this.cb_mappingType.SelectionChangeCommitted += new System.EventHandler(this.cb_mappingType_SelectionChangeCommitted);
            // 
            // panel_op
            // 
            this.panel_op.Controls.Add(this.btn_unbindAll);
            this.panel_op.Controls.Add(this.btn_unbind);
            this.panel_op.Controls.Add(this.btn_update);
            this.panel_op.Controls.Add(this.btn_bind);
            this.panel_op.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.panel_op, "panel_op");
            this.panel_op.Name = "panel_op";
            // 
            // btn_unbindAll
            // 
            resources.ApplyResources(this.btn_unbindAll, "btn_unbindAll");
            this.btn_unbindAll.Name = "btn_unbindAll";
            this.btn_unbindAll.UseVisualStyleBackColor = true;
            this.btn_unbindAll.Click += new System.EventHandler(this.btn_unbindAll_Click);
            // 
            // btn_unbind
            // 
            resources.ApplyResources(this.btn_unbind, "btn_unbind");
            this.btn_unbind.Name = "btn_unbind";
            this.btn_unbind.UseVisualStyleBackColor = true;
            this.btn_unbind.Click += new System.EventHandler(this.btn_unbind_Click);
            // 
            // btn_update
            // 
            resources.ApplyResources(this.btn_update, "btn_update");
            this.btn_update.Name = "btn_update";
            this.btn_update.UseVisualStyleBackColor = true;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // btn_bind
            // 
            resources.ApplyResources(this.btn_bind, "btn_bind");
            this.btn_bind.Name = "btn_bind";
            this.btn_bind.UseVisualStyleBackColor = true;
            this.btn_bind.Click += new System.EventHandler(this.btn_bind_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_cmd);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cb_cmd
            // 
            this.cb_cmd.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cb_cmd, "cb_cmd");
            this.cb_cmd.FormattingEnabled = true;
            this.cb_cmd.Name = "cb_cmd";
            this.cb_cmd.Readonly = true;
            this.cb_cmd.SelectedIndexChanged += new System.EventHandler(this.cb_cmd_SelectedIndexChanged);
            this.cb_cmd.SelectionChangeCommitted += new System.EventHandler(this.cb_cmd_SelectionChangeCommitted);
            // 
            // grp_target
            // 
            this.grp_target.Controls.Add(this.tv_tgtParam);
            this.grp_target.Controls.Add(this.panel1);
            this.grp_target.Controls.Add(this.panel5);
            resources.ApplyResources(this.grp_target, "grp_target");
            this.grp_target.Name = "grp_target";
            this.grp_target.TabStop = false;
            // 
            // tv_tgtParam
            // 
            resources.ApplyResources(this.tv_tgtParam, "tv_tgtParam");
            this.tv_tgtParam.FullRowSelect = true;
            this.tv_tgtParam.HideSelection = false;
            this.tv_tgtParam.ImageList = this.imageList1;
            this.tv_tgtParam.Name = "tv_tgtParam";
            this.tv_tgtParam.ShowNodeToolTips = true;
            this.tv_tgtParam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tv_tgtParam_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "paramgrp16.gif");
            this.imageList1.Images.SetKeyName(1, "param16.gif");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cb_targetProc);
            this.panel1.Controls.Add(this.panel7);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cb_targetProc
            // 
            this.cb_targetProc.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cb_targetProc, "cb_targetProc");
            this.cb_targetProc.FormattingEnabled = true;
            this.cb_targetProc.Name = "cb_targetProc";
            this.cb_targetProc.Readonly = true;
            this.cb_targetProc.SelectedIndexChanged += new System.EventHandler(this.cb_targetProc_SelectedIndexChanged);
            this.cb_targetProc.SelectionChangeCommitted += new System.EventHandler(this.cb_targetProc_SelectionChangeCommitted);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label4);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label_set_type);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.label_p_type);
            this.panel5.Controls.Add(this.label3);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // label_set_type
            // 
            resources.ApplyResources(this.label_set_type, "label_set_type");
            this.label_set_type.ForeColor = System.Drawing.Color.Blue;
            this.label_set_type.Name = "label_set_type";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label_p_type
            // 
            resources.ApplyResources(this.label_p_type, "label_p_type");
            this.label_p_type.ForeColor = System.Drawing.Color.MediumBlue;
            this.label_p_type.Name = "label_p_type";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btn_Cancel);
            this.panel3.Controls.Add(this.btn_OK);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lvex_mappings);
            this.panel4.Controls.Add(this.lb_msg);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // lvex_mappings
            // 
            this.lvex_mappings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFrom,
            this.colTo,
            this.colCmt});
            resources.ApplyResources(this.lvex_mappings, "lvex_mappings");
            this.lvex_mappings.DoubleClickActivation = false;
            this.lvex_mappings.FullRowSelect = true;
            this.lvex_mappings.GridLines = true;
            this.lvex_mappings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvex_mappings.HideSelection = false;
            this.lvex_mappings.MultiSelect = false;
            this.lvex_mappings.Name = "lvex_mappings";
            this.lvex_mappings.UseCompatibleStateImageBehavior = false;
            this.lvex_mappings.View = System.Windows.Forms.View.Details;
            this.lvex_mappings.SubItemClicked += new WebMaster.com.SubItemEventHandler(this.lvex_mappings_SubItemClicked);
            this.lvex_mappings.SizeChanged += new System.EventHandler(this.lvex_mappings_SizeChanged);
            this.lvex_mappings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvex_mappings_MouseDown);
            // 
            // colFrom
            // 
            resources.ApplyResources(this.colFrom, "colFrom");
            // 
            // colTo
            // 
            resources.ApplyResources(this.colTo, "colTo");
            // 
            // colCmt
            // 
            resources.ApplyResources(this.colCmt, "colCmt");
            // 
            // lb_msg
            // 
            resources.ApplyResources(this.lb_msg, "lb_msg");
            this.lb_msg.ForeColor = System.Drawing.Color.Red;
            this.lb_msg.Name = "lb_msg";
            // 
            // ParamMappingDlg
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel_title);
            this.DoubleBuffered = true;
            this.MinimizeBox = false;
            this.Name = "ParamMappingDlg";
            this.ShowInTaskbar = false;
            this.SizeChanged += new System.EventHandler(this.ParamMappingDlg_SizeChanged);
            this.panel_title.ResumeLayout(false);
            this.panel_title.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.grp_src.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel_op.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.grp_target.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_title;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rtb_header;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox grp_src;
        private System.Windows.Forms.Panel panel_op;
        private System.Windows.Forms.GroupBox grp_target;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private WebMaster.com.ListViewEx lvex_mappings;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.TreeView tv_tgtParam;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label_set_type;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_p_type;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel_src;
        private System.Windows.Forms.Panel panel8;
        private ComboBoxEx cb_mappingType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_unbindAll;
        private System.Windows.Forms.Button btn_unbind;
        private System.Windows.Forms.Button btn_update;
        private System.Windows.Forms.Button btn_bind;
        private ComboBoxEx cb_cmd;
        private System.Windows.Forms.ColumnHeader colFrom;
        private System.Windows.Forms.ColumnHeader colTo;
        private System.Windows.Forms.ColumnHeader colCmt;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel6;
        private ComboBoxEx cb_elemType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label4;
        private ComboBoxEx cb_targetProc;
    }
}