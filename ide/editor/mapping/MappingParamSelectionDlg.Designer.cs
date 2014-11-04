namespace WebMaster.ide.editor.mapping
{
    partial class MappingParamSelectionDlg
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
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.labelSrcType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_mappingType = new WebMaster.com.ComboBoxEx(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.panel_src = new System.Windows.Forms.Panel();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel6);
            this.panel8.Controls.Add(this.cb_mappingType);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Padding = new System.Windows.Forms.Padding(3, 2, 0, 0);
            this.panel8.Size = new System.Drawing.Size(584, 30);
            this.panel8.TabIndex = 4;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.labelSrcType);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(245, 2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(339, 26);
            this.panel6.TabIndex = 6;
            // 
            // labelSrcType
            // 
            this.labelSrcType.AutoSize = true;
            this.labelSrcType.ForeColor = System.Drawing.Color.RoyalBlue;
            this.labelSrcType.Location = new System.Drawing.Point(88, 3);
            this.labelSrcType.Name = "labelSrcType";
            this.labelSrcType.Size = new System.Drawing.Size(42, 16);
            this.labelSrcType.TabIndex = 1;
            this.labelSrcType.Text = "String";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(42, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type";
            // 
            // cb_mappingType
            // 
            this.cb_mappingType.BackColor = System.Drawing.SystemColors.Control;
            this.cb_mappingType.Dock = System.Windows.Forms.DockStyle.Left;
            this.cb_mappingType.FormattingEnabled = true;
            this.cb_mappingType.Location = new System.Drawing.Point(3, 2);
            this.cb_mappingType.Name = "cb_mappingType";
            this.cb_mappingType.Readonly = true;
            this.cb_mappingType.Size = new System.Drawing.Size(242, 24);
            this.cb_mappingType.TabIndex = 0;
            this.cb_mappingType.SelectedIndexChanged += new System.EventHandler(this.cb_mappingType_SelectedIndexChanged);
            this.cb_mappingType.SelectionChangeCommitted += new System.EventHandler(this.cb_mappingType_SelectionChangeCommitted);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Controls.Add(this.btn_OK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 328);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 54);
            this.panel1.TabIndex = 5;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(398, 16);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 25);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Enabled = false;
            this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_OK.Location = new System.Drawing.Point(229, 16);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 25);
            this.btn_OK.TabIndex = 2;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            // 
            // panel_src
            // 
            this.panel_src.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_src.Location = new System.Drawing.Point(0, 30);
            this.panel_src.Name = "panel_src";
            this.panel_src.Size = new System.Drawing.Size(584, 298);
            this.panel_src.TabIndex = 6;
            // 
            // MappingParamSelectionDlg
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(584, 382);
            this.Controls.Add(this.panel_src);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel8);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MappingParamSelectionDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MappingParamSelectionDlg";
            this.panel8.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label1;
        private WebMaster.com.ComboBoxEx cb_mappingType;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Panel panel_src;
        private System.Windows.Forms.Label labelSrcType;
    }
}