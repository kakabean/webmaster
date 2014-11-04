namespace WebMaster.ide.editor.mapping
{
    partial class MSGFPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSGFPanel));
            this.rootPanel = new System.Windows.Forms.Panel();
            this.grpGF = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lv_params = new WebMaster.com.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.tv_GF_CMD = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tb_filter = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_msg = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rtb_exp = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rootPanel.SuspendLayout();
            this.grpGF.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.grpGF);
            this.rootPanel.Controls.Add(this.panel1);
            this.rootPanel.Controls.Add(this.panel7);
            resources.ApplyResources(this.rootPanel, "rootPanel");
            this.rootPanel.Name = "rootPanel";
            // 
            // grpGF
            // 
            this.grpGF.Controls.Add(this.panel3);
            this.grpGF.Controls.Add(this.panel2);
            resources.ApplyResources(this.grpGF, "grpGF");
            this.grpGF.Name = "grpGF";
            this.grpGF.TabStop = false;
            this.grpGF.Resize += new System.EventHandler(this.grpGF_Resize);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lv_params);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // lv_params
            // 
            this.lv_params.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            resources.ApplyResources(this.lv_params, "lv_params");
            this.lv_params.DoubleClickActivation = false;
            this.lv_params.FullRowSelect = true;
            this.lv_params.GridLines = true;
            this.lv_params.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lv_params.HideSelection = false;
            this.lv_params.MultiSelect = false;
            this.lv_params.Name = "lv_params";
            this.lv_params.ShowItemToolTips = true;
            this.lv_params.UseCompatibleStateImageBehavior = false;
            this.lv_params.View = System.Windows.Forms.View.Details;
            this.lv_params.SubItemClicked += new WebMaster.com.SubItemEventHandler(this.lv_params_SubItemClicked);
            this.lv_params.SizeChanged += new System.EventHandler(this.lv_params_SizeChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tv_GF_CMD);
            this.panel2.Controls.Add(this.tb_filter);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tv_GF_CMD
            // 
            resources.ApplyResources(this.tv_GF_CMD, "tv_GF_CMD");
            this.tv_GF_CMD.FullRowSelect = true;
            this.tv_GF_CMD.HideSelection = false;
            this.tv_GF_CMD.ImageList = this.imageList1;
            this.tv_GF_CMD.Name = "tv_GF_CMD";
            this.tv_GF_CMD.ShowNodeToolTips = true;
            this.tv_GF_CMD.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_GF_CMD_AfterSelect);
            this.tv_GF_CMD.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tv_GF_CMD_MouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "gf_category16.gif");
            this.imageList1.Images.SetKeyName(1, "gf_cmd16.gif");
            // 
            // tb_filter
            // 
            resources.ApplyResources(this.tb_filter, "tb_filter");
            this.tb_filter.Name = "tb_filter";
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
            // panel7
            // 
            this.panel7.Controls.Add(this.panel4);
            this.panel7.Controls.Add(this.label7);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rtb_exp);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
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
            // MSGFPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MSGFPanel";
            this.rootPanel.ResumeLayout(false);
            this.grpGF.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.GroupBox grpGF;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.RichTextBox rtb_exp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TreeView tv_GF_CMD;
        private System.Windows.Forms.TextBox tb_filter;
        private WebMaster.com.ListViewEx lv_params;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
    }
}
