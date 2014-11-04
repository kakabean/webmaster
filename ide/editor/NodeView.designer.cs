using System.Windows.Forms;
namespace WebMaster.ide.editor.model
{
    partial class NodeView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_Img = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.textPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.textPanel);
            this.panel1.Controls.Add(this.panel_Img);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(100, 64);
            this.panel1.TabIndex = 0;
            // 
            // textPanel
            // 
            this.textPanel.AllowDrop = true;
            this.textPanel.Controls.Add(this.label1);
            this.textPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.textPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textPanel.ForeColor = System.Drawing.Color.Black;
            this.textPanel.Location = new System.Drawing.Point(24, 0);
            this.textPanel.Margin = new System.Windows.Forms.Padding(0);
            this.textPanel.Name = "textPanel";
            this.textPanel.Size = new System.Drawing.Size(76, 64);
            this.textPanel.TabIndex = 1;
            this.textPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.textPanel_DragDrop);
            this.textPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.textPanel_DragEnter);
            this.textPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textPanel_MouseDoubleClick);
            this.textPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textPanel_MouseDown);
            this.textPanel.MouseEnter += new System.EventHandler(this.textPanel_MouseEnter);
            this.textPanel.MouseLeave += new System.EventHandler(this.textPanel_MouseLeave);
            this.textPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textPanel_MouseMove);
            this.textPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textPanel_MouseUp);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 64);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            this.label1.DragDrop += new System.Windows.Forms.DragEventHandler(this.label1_DragDrop);
            this.label1.DragEnter += new System.Windows.Forms.DragEventHandler(this.label1_DragEnter);
            this.label1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDoubleClick);
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            this.label1.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label1_MouseUp);
            // 
            // panel_Img
            // 
            this.panel_Img.AllowDrop = true;
            this.panel_Img.BackColor = System.Drawing.SystemColors.Control;
            this.panel_Img.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel_Img.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_Img.Location = new System.Drawing.Point(0, 0);
            this.panel_Img.Margin = new System.Windows.Forms.Padding(0);
            this.panel_Img.Name = "panel_Img";
            this.panel_Img.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.panel_Img.Size = new System.Drawing.Size(24, 64);
            this.panel_Img.TabIndex = 2;
            this.panel_Img.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelImg_DragDrop);
            this.panel_Img.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelImg_DragEnter);
            this.panel_Img.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Img_Paint);
            this.panel_Img.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelImg_MouseDoubleClick);
            this.panel_Img.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelImg_MouseDown);
            this.panel_Img.MouseEnter += new System.EventHandler(this.panelImg_MouseEnter);
            this.panel_Img.MouseLeave += new System.EventHandler(this.panelImg_MouseLeave);
            this.panel_Img.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelImg_MouseMove);
            this.panel_Img.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelImg_MouseUp);
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(2, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 64);
            this.textBox1.TabIndex = 1;
            this.textBox1.Visible = false;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // NodeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NodeView";
            this.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.Size = new System.Drawing.Size(102, 64);
            this.LocationChanged += new System.EventHandler(this.NodeView_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.NodeView_SizeChanged);
            this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.Node_GiveFeedback);
            this.panel1.ResumeLayout(false);
            this.textPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel_Img;
        private Panel textPanel;
        private TextBox textBox1;
        private Label label1;

    }
}
