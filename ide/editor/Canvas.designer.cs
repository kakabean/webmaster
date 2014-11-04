namespace WebMaster.ide.editor
{
    partial class Canvas
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
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
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Canvas";
            this.Size = new System.Drawing.Size(200, 201);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.GraphControl_Scroll);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.GraphControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.GraphControl_DragEnter);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphControl_MouseUp);
            this.Resize += new System.EventHandler(this.GraphControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
