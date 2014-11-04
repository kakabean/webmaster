namespace WebMaster.ide.editor.propview
{
    partial class UserLogPropView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserLogPropView));
            this.op_panel = new System.Windows.Forms.Panel();
            this.log_op = new WebMaster.ide.editor.propview.UserLogComponent();
            this.proc_panel = new System.Windows.Forms.Panel();
            this.log_proc = new WebMaster.ide.editor.propview.UserLogComponent();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_end = new System.Windows.Forms.Button();
            this.op_panel.SuspendLayout();
            this.proc_panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // op_panel
            // 
            this.op_panel.Controls.Add(this.log_op);
            resources.ApplyResources(this.op_panel, "op_panel");
            this.op_panel.Name = "op_panel";
            // 
            // log_op
            // 
            resources.ApplyResources(this.log_op, "log_op");
            this.log_op.BackColor = System.Drawing.SystemColors.Window;
            this.log_op.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.log_op.FlowPVManager = null;
            this.log_op.Name = "log_op";
            // 
            // proc_panel
            // 
            this.proc_panel.Controls.Add(this.log_proc);
            this.proc_panel.Controls.Add(this.panel1);
            resources.ApplyResources(this.proc_panel, "proc_panel");
            this.proc_panel.Name = "proc_panel";
            // 
            // log_proc
            // 
            resources.ApplyResources(this.log_proc, "log_proc");
            this.log_proc.BackColor = System.Drawing.SystemColors.Window;
            this.log_proc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.log_proc.FlowPVManager = null;
            this.log_proc.Name = "log_proc";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_start);
            this.panel1.Controls.Add(this.btn_end);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btn_start
            // 
            resources.ApplyResources(this.btn_start, "btn_start");
            this.btn_start.Name = "btn_start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_end
            // 
            this.btn_end.BackColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.btn_end, "btn_end");
            this.btn_end.Name = "btn_end";
            this.btn_end.UseVisualStyleBackColor = false;
            this.btn_end.Click += new System.EventHandler(this.btn_end_Click);
            // 
            // UserLogPropView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.proc_panel);
            this.Controls.Add(this.op_panel);
            this.Name = "UserLogPropView";
            this.op_panel.ResumeLayout(false);
            this.proc_panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel op_panel;
        private UserLogComponent log_op;
        private System.Windows.Forms.Panel proc_panel;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private UserLogComponent log_proc;
        private System.Windows.Forms.Button btn_end;
        private System.Windows.Forms.Button btn_start;


    }
}
