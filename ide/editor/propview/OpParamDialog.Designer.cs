namespace WebMaster.ide.editor.propview
{
    partial class OpParamDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpParamDialog));
            this.opImgList = new System.Windows.Forms.ImageList(this.components);
            this.tv_op = new System.Windows.Forms.TreeView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel3 = new System.Windows.Forms.Panel();
            this.tb_des = new System.Windows.Forms.TextBox();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // opImgList
            // 
            this.opImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("opImgList.ImageStream")));
            this.opImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.opImgList.Images.SetKeyName(0, "op_start16.gif");
            this.opImgList.Images.SetKeyName(1, "op_stop16.gif");
            this.opImgList.Images.SetKeyName(2, "op_url16.png");
            this.opImgList.Images.SetKeyName(3, "op_click16.gif");
            this.opImgList.Images.SetKeyName(4, "op_input16.gif");
            this.opImgList.Images.SetKeyName(5, "op_process16.png");
            this.opImgList.Images.SetKeyName(6, "op_nop16.gif");
            // 
            // tv_op
            // 
            resources.ApplyResources(this.tv_op, "tv_op");
            this.tv_op.FullRowSelect = true;
            this.tv_op.HideSelection = false;
            this.tv_op.ImageList = this.opImgList;
            this.tv_op.Name = "tv_op";
            this.tv_op.ShowNodeToolTips = true;
            this.tv_op.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_op_MouseDoubleClick);
            this.tv_op.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_op_MouseDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tb_des);
            this.panel3.Controls.Add(this.tb_name);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // tb_des
            // 
            resources.ApplyResources(this.tb_des, "tb_des");
            this.tb_des.Name = "tb_des";
            this.tb_des.ReadOnly = true;
            // 
            // tb_name
            // 
            resources.ApplyResources(this.tb_name, "tb_name");
            this.tb_name.Name = "tb_name";
            this.tb_name.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_Cancel);
            this.panel2.Controls.Add(this.btn_OK);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
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
            // OpParamDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tv_op);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpParamDialog";
            this.ShowInTaskbar = false;
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList opImgList;
        private System.Windows.Forms.TreeView tv_op;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox tb_des;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.ColumnHeader columnHeader1;

    }
}