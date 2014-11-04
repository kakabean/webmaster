namespace WebMaster.updateserver
{
    partial class UpdateServiceUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateServiceUI));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lvMsg = new System.Windows.Forms.ListView();
            this.msgcol0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.msgcol1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.msgcol2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lvApps = new System.Windows.Forms.ListView();
            this.appcol0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.appcol1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_appDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.rtb_logs = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_start = new System.Windows.Forms.ToolStripButton();
            this.tsb_stop = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 25);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(758, 329);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lvMsg);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(750, 303);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Latest access info";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lvMsg
            // 
            this.lvMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.msgcol0,
            this.msgcol1,
            this.msgcol2});
            this.lvMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMsg.FullRowSelect = true;
            this.lvMsg.GridLines = true;
            this.lvMsg.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvMsg.Location = new System.Drawing.Point(3, 3);
            this.lvMsg.Name = "lvMsg";
            this.lvMsg.Size = new System.Drawing.Size(744, 297);
            this.lvMsg.TabIndex = 0;
            this.lvMsg.UseCompatibleStateImageBehavior = false;
            this.lvMsg.View = System.Windows.Forms.View.Details;
            // 
            // msgcol0
            // 
            this.msgcol0.Text = "Time";
            this.msgcol0.Width = 100;
            // 
            // msgcol1
            // 
            this.msgcol1.Text = "IP address";
            this.msgcol1.Width = 163;
            // 
            // msgcol2
            // 
            this.msgcol2.Text = "Message";
            this.msgcol2.Width = 473;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lvApps);
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(750, 303);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvApps
            // 
            this.lvApps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.appcol0,
            this.appcol1});
            this.lvApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvApps.FullRowSelect = true;
            this.lvApps.GridLines = true;
            this.lvApps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvApps.Location = new System.Drawing.Point(3, 43);
            this.lvApps.Name = "lvApps";
            this.lvApps.Size = new System.Drawing.Size(744, 257);
            this.lvApps.TabIndex = 2;
            this.lvApps.UseCompatibleStateImageBehavior = false;
            this.lvApps.View = System.Windows.Forms.View.Details;
            // 
            // appcol0
            // 
            this.appcol0.Text = "File Name";
            this.appcol0.Width = 556;
            // 
            // appcol1
            // 
            this.appcol1.Text = "Version";
            this.appcol1.Width = 171;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(3, 43);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(744, 257);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.tb_appDir);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(744, 40);
            this.panel2.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(626, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "publish new version";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_appDir
            // 
            this.tb_appDir.Location = new System.Drawing.Point(51, 9);
            this.tb_appDir.Name = "tb_appDir";
            this.tb_appDir.Size = new System.Drawing.Size(569, 20);
            this.tb_appDir.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "App dir";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.rtb_logs);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(750, 303);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Server log";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // rtb_logs
            // 
            this.rtb_logs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtb_logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_logs.Location = new System.Drawing.Point(0, 0);
            this.rtb_logs.Name = "rtb_logs";
            this.rtb_logs.ReadOnly = true;
            this.rtb_logs.Size = new System.Drawing.Size(750, 303);
            this.rtb_logs.TabIndex = 0;
            this.rtb_logs.Text = "";
            this.rtb_logs.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 379);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(758, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_start,
            this.tsb_stop});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(758, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_start
            // 
            this.tsb_start.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_start.Image = ((System.Drawing.Image)(resources.GetObject("tsb_start.Image")));
            this.tsb_start.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_start.Name = "tsb_start";
            this.tsb_start.Size = new System.Drawing.Size(70, 22);
            this.tsb_start.Text = "Start Server";
            this.tsb_start.Click += new System.EventHandler(this.tsb_start_Click);
            // 
            // tsb_stop
            // 
            this.tsb_stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_stop.Image = ((System.Drawing.Image)(resources.GetObject("tsb_stop.Image")));
            this.tsb_stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_stop.Name = "tsb_stop";
            this.tsb_stop.Size = new System.Drawing.Size(70, 22);
            this.tsb_stop.Text = "Stop Server";
            this.tsb_stop.Click += new System.EventHandler(this.tsb_stop_Click);
            // 
            // UpdateServiceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 401);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "UpdateServiceUI";
            this.Text = "Update service";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lvMsg;
        private System.Windows.Forms.ColumnHeader msgcol0;
        private System.Windows.Forms.ColumnHeader msgcol1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ColumnHeader msgcol2;
        private System.Windows.Forms.ListView lvApps;
        private System.Windows.Forms.ColumnHeader appcol0;
        private System.Windows.Forms.ColumnHeader appcol1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb_appDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox rtb_logs;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_start;
        private System.Windows.Forms.ToolStripButton tsb_stop;
    }
}

