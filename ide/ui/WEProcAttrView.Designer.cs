using WebMaster.lib;
using System.Windows.Forms;
namespace WebMaster.ide.ui
{
    partial class WEProcAttrView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WEProcAttrView));
            this.panel1 = new System.Windows.Forms.Panel();
            this.errmsg = new System.Windows.Forms.Label();
            this.tb_des = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewEx1 = new WebMaster.com.ListViewEx();
            this.colKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPattern = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.errmsg);
            this.panel1.Controls.Add(this.tb_des);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tb_name);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // errmsg
            // 
            resources.ApplyResources(this.errmsg, "errmsg");
            this.errmsg.ForeColor = System.Drawing.Color.Red;
            this.errmsg.Name = "errmsg";
            // 
            // tb_des
            // 
            resources.ApplyResources(this.tb_des, "tb_des");
            this.tb_des.Name = "tb_des";
            this.tb_des.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox2_KeyDown);
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
            this.tb_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewEx1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // listViewEx1
            // 
            this.listViewEx1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewEx1.CheckBoxes = true;
            this.listViewEx1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colKey,
            this.colValue,
            this.colPattern});
            resources.ApplyResources(this.listViewEx1, "listViewEx1");
            this.listViewEx1.DoubleClickActivation = false;
            this.listViewEx1.FullRowSelect = true;
            this.listViewEx1.GridLines = true;
            this.listViewEx1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewEx1.HideSelection = false;
            this.listViewEx1.MinimumSize = new System.Drawing.Size(500, 220);
            this.listViewEx1.MultiSelect = false;
            this.listViewEx1.Name = "listViewEx1";
            this.listViewEx1.UseCompatibleStateImageBehavior = false;
            this.listViewEx1.View = System.Windows.Forms.View.Details;
            this.listViewEx1.SubItemClicked += new WebMaster.com.SubItemEventHandler(this.listViewEx1_SubItemClicked_1);
            this.listViewEx1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewEx1_ItemCheck);
            this.listViewEx1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewEx1_ItemChecked);
            // 
            // colKey
            // 
            resources.ApplyResources(this.colKey, "colKey");
            // 
            // colValue
            // 
            resources.ApplyResources(this.colValue, "colValue");
            // 
            // colPattern
            // 
            resources.ApplyResources(this.colPattern, "colPattern");
            // 
            // WEProcAttrView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Name = "WEProcAttrView";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label errmsg;
        private System.Windows.Forms.TextBox tb_des;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.Label label1;
        private GroupBox groupBox1;
        private WebMaster.com.ListViewEx listViewEx1;
        private ColumnHeader colKey;
        private ColumnHeader colValue;
        private ColumnHeader colPattern;
    }
}
