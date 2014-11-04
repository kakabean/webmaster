namespace WebMaster.com.script
{
    partial class UserScriptListDlg
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
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Test Mapping");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("新浪微博");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("TestConnection");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("");
            this.grpDev = new System.Windows.Forms.GroupBox();
            this.lv_myscripts = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpBook = new System.Windows.Forms.GroupBox();
            this.lv_bookedscript = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.grpDev.SuspendLayout();
            this.grpBook.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDev
            // 
            this.grpDev.Controls.Add(this.lv_myscripts);
            this.grpDev.Location = new System.Drawing.Point(0, 0);
            this.grpDev.Name = "grpDev";
            this.grpDev.Size = new System.Drawing.Size(573, 145);
            this.grpDev.TabIndex = 0;
            this.grpDev.TabStop = false;
            this.grpDev.Text = "My owned Scripts";
            // 
            // lv_myscripts
            // 
            this.lv_myscripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lv_myscripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_myscripts.FullRowSelect = true;
            this.lv_myscripts.GridLines = true;
            this.lv_myscripts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_myscripts.HideSelection = false;
            listViewItem11.Tag = "D:\\ZhangHui\\mywork\\WebMasterAll\\TestCases\\Test_Mapping1.ws";
            listViewItem12.Tag = "D:\\ZhangHui\\mywork\\WebMasterAll\\TestCases\\新浪微博.ws";
            listViewItem13.Tag = "D:\\ZhangHui\\mywork\\WebMasterAll\\TestCases\\testConnection.ws";
            this.lv_myscripts.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15});
            this.lv_myscripts.Location = new System.Drawing.Point(3, 18);
            this.lv_myscripts.MultiSelect = false;
            this.lv_myscripts.Name = "lv_myscripts";
            this.lv_myscripts.Size = new System.Drawing.Size(567, 124);
            this.lv_myscripts.TabIndex = 0;
            this.lv_myscripts.UseCompatibleStateImageBehavior = false;
            this.lv_myscripts.View = System.Windows.Forms.View.Details;
            this.lv_myscripts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_myscripts_MouseDoubleClick);
            this.lv_myscripts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_myscripts_MouseDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 560;
            // 
            // grpBook
            // 
            this.grpBook.Controls.Add(this.lv_bookedscript);
            this.grpBook.Location = new System.Drawing.Point(0, 147);
            this.grpBook.Name = "grpBook";
            this.grpBook.Size = new System.Drawing.Size(573, 145);
            this.grpBook.TabIndex = 1;
            this.grpBook.TabStop = false;
            this.grpBook.Text = "My booked scripts";
            // 
            // lv_bookedscript
            // 
            this.lv_bookedscript.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lv_bookedscript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_bookedscript.FullRowSelect = true;
            this.lv_bookedscript.GridLines = true;
            this.lv_bookedscript.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_bookedscript.HideSelection = false;
            this.lv_bookedscript.Location = new System.Drawing.Point(3, 18);
            this.lv_bookedscript.MultiSelect = false;
            this.lv_bookedscript.Name = "lv_bookedscript";
            this.lv_bookedscript.Size = new System.Drawing.Size(567, 124);
            this.lv_bookedscript.TabIndex = 1;
            this.lv_bookedscript.UseCompatibleStateImageBehavior = false;
            this.lv_bookedscript.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_bookedscript_MouseDoubleClick);
            this.lv_bookedscript.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_bookedscript_MouseDown);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 560;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_Cancel);
            this.panel1.Controls.Add(this.btn_OK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 296);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 54);
            this.panel1.TabIndex = 2;
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
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // UserScriptListDlg
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(573, 350);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpBook);
            this.Controls.Add(this.grpDev);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UserScriptListDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UserScriptListDlg";
            this.grpDev.ResumeLayout(false);
            this.grpBook.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDev;
        private System.Windows.Forms.GroupBox grpBook;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.ListView lv_myscripts;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView lv_bookedscript;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}