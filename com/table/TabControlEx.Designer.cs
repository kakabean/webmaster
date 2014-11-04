using WebMaster.com;
namespace WebMaster.com.table
{
    partial class TabControlEx
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
            this.pageContainer = new System.Windows.Forms.Panel();
            this.headerBar = new WebMaster.com.CustomPanel();
            this.SuspendLayout();
            // 
            // pageContainer
            // 
            this.pageContainer.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageContainer.Location = new System.Drawing.Point(0, 24);
            this.pageContainer.Name = "pageContainer";
            this.pageContainer.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.pageContainer.Size = new System.Drawing.Size(870, 552);
            this.pageContainer.TabIndex = 1;
            // 
            // headerBar
            // 
            this.headerBar.BackColor = System.Drawing.SystemColors.Control;
            this.headerBar.CloseBorder = true;
            this.headerBar.Curvature = 0;
            this.headerBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerBar.ExtendHeight = false;
            this.headerBar.Location = new System.Drawing.Point(0, 0);
            this.headerBar.Name = "headerBar";
            this.headerBar.Size = new System.Drawing.Size(870, 24);
            this.headerBar.TabIndex = 0;
            // 
            // TabControlEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pageContainer);
            this.Controls.Add(this.headerBar);
            this.Name = "TabControlEx";
            this.Size = new System.Drawing.Size(870, 576);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomPanel headerBar;
        private System.Windows.Forms.Panel pageContainer;
    }
}
