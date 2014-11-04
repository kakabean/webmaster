namespace WebMaster.ide.editor.propview
{
    partial class WebElementAdvancedEditDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebElementAdvancedEditDlg));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_edit = new System.Windows.Forms.Button();
            this.cb_pattern = new System.Windows.Forms.ComboBox();
            this.label_msg = new System.Windows.Forms.Label();
            this.tb_value = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.weaGrp = new System.Windows.Forms.GroupBox();
            this.lv_wea = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.weaGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btn_edit);
            this.panel1.Controls.Add(this.cb_pattern);
            this.panel1.Controls.Add(this.label_msg);
            this.panel1.Controls.Add(this.tb_value);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Name = "panel1";
            // 
            // btn_edit
            // 
            resources.ApplyResources(this.btn_edit, "btn_edit");
            this.btn_edit.Name = "btn_edit";
            this.btn_edit.UseVisualStyleBackColor = true;
            this.btn_edit.Click += new System.EventHandler(this.btn_edit_Click);
            // 
            // cb_pattern
            // 
            resources.ApplyResources(this.cb_pattern, "cb_pattern");
            this.cb_pattern.FormattingEnabled = true;
            this.cb_pattern.Name = "cb_pattern";
            this.cb_pattern.SelectedIndexChanged += new System.EventHandler(this.cb_pattern_SelectedIndexChanged);
            this.cb_pattern.SelectionChangeCommitted += new System.EventHandler(this.cb_pattern_SelectionChangeCommitted);
            // 
            // label_msg
            // 
            resources.ApplyResources(this.label_msg, "label_msg");
            this.label_msg.Name = "label_msg";
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
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.btn_Cancel);
            this.panel2.Controls.Add(this.btn_OK);
            this.panel2.Name = "panel2";
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // weaGrp
            // 
            resources.ApplyResources(this.weaGrp, "weaGrp");
            this.weaGrp.Controls.Add(this.lv_wea);
            this.weaGrp.Name = "weaGrp";
            this.weaGrp.TabStop = false;
            // 
            // lv_wea
            // 
            resources.ApplyResources(this.lv_wea, "lv_wea");
            this.lv_wea.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lv_wea.FullRowSelect = true;
            this.lv_wea.GridLines = true;
            this.lv_wea.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_wea.HideSelection = false;
            this.lv_wea.MultiSelect = false;
            this.lv_wea.Name = "lv_wea";
            this.lv_wea.UseCompatibleStateImageBehavior = false;
            this.lv_wea.View = System.Windows.Forms.View.Details;
            this.lv_wea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_wea_MouseDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // WebElementAdvancedEditDlg
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
            this.Name = "WebElementAdvancedEditDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.weaGrp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.GroupBox weaGrp;
        private System.Windows.Forms.ListView lv_wea;
        private System.Windows.Forms.Label label_msg;
        private System.Windows.Forms.TextBox tb_value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_pattern;
        private System.Windows.Forms.Button btn_edit;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}