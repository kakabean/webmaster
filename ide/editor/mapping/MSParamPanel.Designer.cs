namespace WebMaster.ide.editor.mapping
{
    partial class MSParamPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSParamPanel));
            this.rootPanel = new System.Windows.Forms.Panel();
            this.grpParams = new System.Windows.Forms.GroupBox();
            this.tv_params = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_procs = new WebMaster.com.ComboBoxEx(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.lb_msg = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rtb_exp = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rootPanel.SuspendLayout();
            this.grpParams.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.grpParams);
            this.rootPanel.Controls.Add(this.panel2);
            this.rootPanel.Controls.Add(this.panel7);
            resources.ApplyResources(this.rootPanel, "rootPanel");
            this.rootPanel.Name = "rootPanel";
            // 
            // grpParams
            // 
            this.grpParams.Controls.Add(this.tv_params);
            this.grpParams.Controls.Add(this.panel1);
            resources.ApplyResources(this.grpParams, "grpParams");
            this.grpParams.Name = "grpParams";
            this.grpParams.TabStop = false;
            // 
            // tv_params
            // 
            resources.ApplyResources(this.tv_params, "tv_params");
            this.tv_params.FullRowSelect = true;
            this.tv_params.HideSelection = false;
            this.tv_params.ImageList = this.imageList1;
            this.tv_params.Name = "tv_params";
            this.tv_params.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tv_params_MouseDown);
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
            this.panel1.Controls.Add(this.cb_procs);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // cb_procs
            // 
            this.cb_procs.FormattingEnabled = true;
            resources.ApplyResources(this.cb_procs, "cb_procs");
            this.cb_procs.Name = "cb_procs";
            this.cb_procs.SelectedIndexChanged += new System.EventHandler(this.cb_procs_SelectedIndexChanged);
            this.cb_procs.SelectionChangeCommitted += new System.EventHandler(this.cb_procs_SelectionChangeCommitted);
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
            // MSParamPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MSParamPanel";
            this.rootPanel.ResumeLayout(false);
            this.grpParams.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.GroupBox grpParams;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.RichTextBox rtb_exp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private WebMaster.com.ComboBoxEx cb_procs;
        private System.Windows.Forms.TreeView tv_params;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel3;
    }
}
