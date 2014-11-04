namespace WebMaster.ide.editor.propview
{
    partial class UserLogComponent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserLogComponent));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.logImgList = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_color = new System.Windows.Forms.Button();
            this.btn_const = new System.Windows.Forms.Button();
            this.btn_Obj = new System.Windows.Forms.Button();
            this.btn_time = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rb_log = new System.Windows.Forms.RadioButton();
            this.rb_ignore = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_item = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cms_tree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_up = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_down = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.cms_tree.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.panel4);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.treeView1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // treeView1
            // 
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageList = this.logImgList;
            this.treeView1.Name = "treeView1";
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // logImgList
            // 
            this.logImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("logImgList.ImageStream")));
            this.logImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.logImgList.Images.SetKeyName(0, "time16.gif");
            this.logImgList.Images.SetKeyName(1, "object16.png");
            this.logImgList.Images.SetKeyName(2, "string16.png");
            this.logImgList.Images.SetKeyName(3, "we_attr16.png");
            this.logImgList.Images.SetKeyName(4, "param16.gif");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btn_color);
            this.panel2.Controls.Add(this.btn_const);
            this.panel2.Controls.Add(this.btn_Obj);
            this.panel2.Controls.Add(this.btn_time);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // btn_color
            // 
            this.btn_color.BackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.btn_color, "btn_color");
            this.btn_color.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btn_color.Name = "btn_color";
            this.btn_color.UseVisualStyleBackColor = false;
            this.btn_color.Click += new System.EventHandler(this.btn_color_Click);
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
            this.btn_Obj.Image = global::ide.Properties.Resources.object16;
            resources.ApplyResources(this.btn_Obj, "btn_Obj");
            this.btn_Obj.Name = "btn_Obj";
            this.btn_Obj.UseVisualStyleBackColor = true;
            this.btn_Obj.Click += new System.EventHandler(this.btn_Obj_Click);
            // 
            // btn_time
            // 
            this.btn_time.Image = global::ide.Properties.Resources.time16;
            resources.ApplyResources(this.btn_time, "btn_time");
            this.btn_time.Name = "btn_time";
            this.btn_time.UseVisualStyleBackColor = true;
            this.btn_time.Click += new System.EventHandler(this.btn_time_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rb_log);
            this.panel4.Controls.Add(this.rb_ignore);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // rb_log
            // 
            resources.ApplyResources(this.rb_log, "rb_log");
            this.rb_log.Name = "rb_log";
            this.rb_log.TabStop = true;
            this.rb_log.UseVisualStyleBackColor = true;
            this.rb_log.Click += new System.EventHandler(this.rb_log_Click);
            // 
            // rb_ignore
            // 
            resources.ApplyResources(this.rb_ignore, "rb_ignore");
            this.rb_ignore.Name = "rb_ignore";
            this.rb_ignore.TabStop = true;
            this.rb_ignore.UseVisualStyleBackColor = true;
            this.rb_ignore.Click += new System.EventHandler(this.rb_ignore_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.richTextBox1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox3_Paint);
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_item);
            this.groupBox2.Controls.Add(this.button1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tb_item
            // 
            resources.ApplyResources(this.tb_item, "tb_item");
            this.tb_item.Name = "tb_item";
            this.tb_item.TextChanged += new System.EventHandler(this.tb_item_TextChanged);
            this.tb_item.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_item_KeyDown);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Image = global::ide.Properties.Resources.edit16;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cms_tree
            // 
            this.cms_tree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_delete,
            this.toolStripSeparator1,
            this.tsmi_up,
            this.tsmi_down});
            this.cms_tree.Name = "cms_tree";
            resources.ApplyResources(this.cms_tree, "cms_tree");
            // 
            // tsmi_delete
            // 
            this.tsmi_delete.Image = global::ide.Properties.Resources.delete16;
            this.tsmi_delete.Name = "tsmi_delete";
            resources.ApplyResources(this.tsmi_delete, "tsmi_delete");
            this.tsmi_delete.Click += new System.EventHandler(this.tsmi_delete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsmi_up
            // 
            this.tsmi_up.Image = global::ide.Properties.Resources.moveup16;
            this.tsmi_up.Name = "tsmi_up";
            resources.ApplyResources(this.tsmi_up, "tsmi_up");
            this.tsmi_up.Click += new System.EventHandler(this.tsmi_up_Click);
            // 
            // tsmi_down
            // 
            this.tsmi_down.Image = global::ide.Properties.Resources.movedown16;
            this.tsmi_down.Name = "tsmi_down";
            resources.ApplyResources(this.tsmi_down, "tsmi_down");
            this.tsmi_down.Click += new System.EventHandler(this.tsmi_down_Click);
            // 
            // UserLogComponent
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "UserLogComponent";
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.cms_tree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_item;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btn_const;
        private System.Windows.Forms.Button btn_Obj;
        private System.Windows.Forms.Button btn_time;
        private System.Windows.Forms.ContextMenuStrip cms_tree;
        private System.Windows.Forms.ToolStripMenuItem tsmi_delete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_up;
        private System.Windows.Forms.ToolStripMenuItem tsmi_down;
        private System.Windows.Forms.ImageList logImgList;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton rb_log;
        private System.Windows.Forms.RadioButton rb_ignore;
        private System.Windows.Forms.Button btn_color;
        private System.Windows.Forms.ColorDialog colorDialog1;

    }
}
