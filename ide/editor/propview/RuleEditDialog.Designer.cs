using WebMaster.com;
namespace WebMaster.ide.editor.propview
{
    partial class RuleEditDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuleEditDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lv_action = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.cb_trigger = new WebMaster.com.ComboBoxEx(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.grp_params = new System.Windows.Forms.GroupBox();
            this.lv_params = new WebMaster.com.ListViewEx();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tb_msg = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.rtb_des = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grp_params.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Controls.Add(this.btn_OK);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
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
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox1);
            this.panel4.Controls.Add(this.grp_params);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lv_action);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cb_trigger);
            this.groupBox1.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lv_action
            // 
            this.lv_action.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lv_action.FullRowSelect = true;
            this.lv_action.GridLines = true;
            this.lv_action.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_action.HideSelection = false;
            resources.ApplyResources(this.lv_action, "lv_action");
            this.lv_action.MultiSelect = false;
            this.lv_action.Name = "lv_action";
            this.lv_action.UseCompatibleStateImageBehavior = false;
            this.lv_action.View = System.Windows.Forms.View.Details;
            this.lv_action.SelectedIndexChanged += new System.EventHandler(this.lv_action_SelectedIndexChanged);
            this.lv_action.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_action_MouseDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cb_trigger
            // 
            this.cb_trigger.BackColor = System.Drawing.SystemColors.Control;
            this.cb_trigger.FormattingEnabled = true;
            resources.ApplyResources(this.cb_trigger, "cb_trigger");
            this.cb_trigger.Name = "cb_trigger";
            this.cb_trigger.Readonly = true;
            this.cb_trigger.SelectedIndexChanged += new System.EventHandler(this.cb_trigger_SelectedIndexChanged);
            this.cb_trigger.TextUpdate += new System.EventHandler(this.cb_trigger_TextUpdate);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // grp_params
            // 
            this.grp_params.Controls.Add(this.lv_params);
            this.grp_params.Controls.Add(this.tb_msg);
            resources.ApplyResources(this.grp_params, "grp_params");
            this.grp_params.Name = "grp_params";
            this.grp_params.TabStop = false;
            // 
            // lv_params
            // 
            this.lv_params.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colValue});
            resources.ApplyResources(this.lv_params, "lv_params");
            this.lv_params.DoubleClickActivation = false;
            this.lv_params.FullRowSelect = true;
            this.lv_params.GridLines = true;
            this.lv_params.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lv_params.HideSelection = false;
            this.lv_params.LabelEdit = true;
            this.lv_params.MultiSelect = false;
            this.lv_params.Name = "lv_params";
            this.lv_params.UseCompatibleStateImageBehavior = false;
            this.lv_params.View = System.Windows.Forms.View.Details;
            this.lv_params.SubItemClicked += new WebMaster.com.SubItemEventHandler(this.lv_params_SubItemClicked);
            this.lv_params.SubItemEndEditing += new WebMaster.com.SubItemEndEditingEventHandler(this.lv_params_SubItemEndEditing);
            this.lv_params.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_params_MouseDown);
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // colValue
            // 
            resources.ApplyResources(this.colValue, "colValue");
            // 
            // tb_msg
            // 
            resources.ApplyResources(this.tb_msg, "tb_msg");
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.ReadOnly = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.rtb_des);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // rtb_des
            // 
            resources.ApplyResources(this.rtb_des, "rtb_des");
            this.rtb_des.Name = "rtb_des";
            this.rtb_des.TextChanged += new System.EventHandler(this.rtb_des_TextChanged);
            // 
            // RuleEditDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RuleEditDialog";
            this.ShowInTaskbar = false;
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grp_params.ResumeLayout(false);
            this.grp_params.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lv_action;
        private System.Windows.Forms.Label label3;
        private ComboBoxEx cb_trigger;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grp_params;
        private WebMaster.com.ListViewEx lv_params;
        private System.Windows.Forms.TextBox tb_msg;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtb_des;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}