namespace WebMaster.ide.editor.mapping
{
    partial class MSWEAPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSWEAPanel));
            this.rootPanel = new System.Windows.Forms.Panel();
            this.grpWEA = new System.Windows.Forms.GroupBox();
            this.tv_wea = new System.Windows.Forms.TreeView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_msg = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtb_exp = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rootPanel.SuspendLayout();
            this.grpWEA.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.grpWEA);
            this.rootPanel.Controls.Add(this.panel1);
            this.rootPanel.Controls.Add(this.panel7);
            resources.ApplyResources(this.rootPanel, "rootPanel");
            this.rootPanel.Name = "rootPanel";
            // 
            // grpWEA
            // 
            this.grpWEA.Controls.Add(this.tv_wea);
            resources.ApplyResources(this.grpWEA, "grpWEA");
            this.grpWEA.Name = "grpWEA";
            this.grpWEA.TabStop = false;
            // 
            // tv_wea
            // 
            resources.ApplyResources(this.tv_wea, "tv_wea");
            this.tv_wea.FullRowSelect = true;
            this.tv_wea.HideSelection = false;
            this.tv_wea.ImageList = this.imgList;
            this.tv_wea.Name = "tv_wea";
            this.tv_wea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tv_wea_MouseDown);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "wegrp16.gif");
            this.imgList.Images.SetKeyName(1, "web_elem16.gif");
            this.imgList.Images.SetKeyName(2, "we_attr16.png");
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
            this.panel7.Controls.Add(this.panel2);
            this.panel7.Controls.Add(this.label7);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtb_exp);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
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
            // MSWEAPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(200, 240);
            this.Name = "MSWEAPanel";
            this.rootPanel.ResumeLayout(false);
            this.grpWEA.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.GroupBox grpWEA;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.RichTextBox rtb_exp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TreeView tv_wea;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Panel panel2;

    }
}
