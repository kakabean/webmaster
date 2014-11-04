namespace WebMaster.com.table
{
    public partial class TabHeader
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
            //components = new System.ComponentModel.Container();
            this.DoubleBuffered = true;
            
            this.imgLabel = new System.Windows.Forms.Label();
            this.imgLabel.AutoSize = false;
            int ix = this.PaddingLeft;
            int iy = iconTop;
            this.imgLabel.Location = new System.Drawing.Point(ix, iy);
            this.imgLabel.Size = new System.Drawing.Size(iconWidth, iconWidth);
            this.imgLabel.Image = global::com.Properties.Resources.browser16;
            this.imgLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(imgLabel_MouseDown);
            this.imgLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(imgLabel_MouseMove);
            this.imgLabel.Click += new System.EventHandler(imgLabel_Click);
            
            this.Controls.Add(this.imgLabel);
        }

        #endregion
    }
}
