namespace WebMaster.ide.editor.propview
{
    partial class WEAPValuesEditDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WEAPValuesEditDlg));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tb_item = new System.Windows.Forms.TextBox();
            this.tb_value = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.weaGrp = new System.Windows.Forms.GroupBox();
            this.lv_wea = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pvImgList = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_del = new System.Windows.Forms.Button();
            this.btn_const = new System.Windows.Forms.Button();
            this.btn_Obj = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.cms_item = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.weaGrp.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.cms_item.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tb_item);
            this.panel1.Controls.Add(this.tb_value);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tb_item
            // 
            resources.ApplyResources(this.tb_item, "tb_item");
            this.tb_item.Name = "tb_item";
            this.tb_item.TextChanged += new System.EventHandler(this.tb_item_TextChanged);
            this.tb_item.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_item_KeyDown);
            // 
            // tb_value
            // 
            resources.ApplyResources(this.tb_value, "tb_value");
            this.tb_value.Name = "tb_value";
            this.tb_value.ReadOnly = true;
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
            // weaGrp
            // 
            this.weaGrp.Controls.Add(this.lv_wea);
            this.weaGrp.Controls.Add(this.panel3);
            resources.ApplyResources(this.weaGrp, "weaGrp");
            this.weaGrp.Name = "weaGrp";
            this.weaGrp.TabStop = false;
            // 
            // lv_wea
            // 
            this.lv_wea.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            resources.ApplyResources(this.lv_wea, "lv_wea");
            this.lv_wea.FullRowSelect = true;
            this.lv_wea.GridLines = true;
            this.lv_wea.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_wea.HideSelection = false;
            this.lv_wea.LargeImageList = this.pvImgList;
            this.lv_wea.MultiSelect = false;
            this.lv_wea.Name = "lv_wea";
            this.lv_wea.SmallImageList = this.pvImgList;
            this.lv_wea.StateImageList = this.pvImgList;
            this.lv_wea.UseCompatibleStateImageBehavior = false;
            this.lv_wea.View = System.Windows.Forms.View.Details;
            this.lv_wea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_wea_MouseDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // pvImgList
            // 
            this.pvImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("pvImgList.ImageStream")));
            this.pvImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.pvImgList.Images.SetKeyName(0, "param16.gif");
            this.pvImgList.Images.SetKeyName(1, "string16.png");
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btn_del);
            this.panel3.Controls.Add(this.btn_const);
            this.panel3.Controls.Add(this.btn_Obj);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // btn_del
            // 
            this.btn_del.Image = global::ide.Properties.Resources.delete16;
            resources.ApplyResources(this.btn_del, "btn_del");
            this.btn_del.Name = "btn_del";
            this.btn_del.UseVisualStyleBackColor = true;
            this.btn_del.Click += new System.EventHandler(this.btn_del_Click);
            // 
            // btn_const
            // 
            this.btn_const.Image = global::ide.Properties.Resources.string16;
            resources.ApplyResources(this.btn_const, "btn_const");
            this.btn_const.Name = "btn_const";
            this.btn_const.UseVisualStyleBackColor = true;
            this.btn_const.Click += new System.EventHandler(this.btn_const_Click);
            // 
            // btn_Obj
            // 
            this.btn_Obj.Image = global::ide.Properties.Resources.param16;
            resources.ApplyResources(this.btn_Obj, "btn_Obj");
            this.btn_Obj.Name = "btn_Obj";
            this.btn_Obj.UseVisualStyleBackColor = true;
            this.btn_Obj.Click += new System.EventHandler(this.btn_Obj_Click);
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
            // cms_item
            // 
            this.cms_item.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.cms_item.Name = "cms_item";
            resources.ApplyResources(this.cms_item, "cms_item");
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            resources.ApplyResources(this.moveUpToolStripMenuItem, "moveUpToolStripMenuItem");
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            resources.ApplyResources(this.moveDownToolStripMenuItem, "moveDownToolStripMenuItem");
            // 
            // WEAPValuesEditDlg
            // 
            this.AcceptButton = this.btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.weaGrp);
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WEAPValuesEditDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.weaGrp.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.cms_item.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tb_value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox weaGrp;
        private System.Windows.Forms.ListView lv_wea;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.TextBox tb_item;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ContextMenuStrip cms_item;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.Button btn_del;
        private System.Windows.Forms.Button btn_const;
        private System.Windows.Forms.Button btn_Obj;
        private System.Windows.Forms.ImageList pvImgList;
    }
}