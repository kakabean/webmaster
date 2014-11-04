namespace WebMaster.ide.ui
{
    partial class DebugPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugPanel));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtb = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_DbgRun = new System.Windows.Forms.ToolStripButton();
            this.tsb_Stop = new System.Windows.Forms.ToolStripButton();
            this.tsb_StepOver = new System.Windows.Forms.ToolStripButton();
            this.tsb_StepInto = new System.Windows.Forms.ToolStripButton();
            this.tsb_max = new System.Windows.Forms.ToolStripButton();
            this.tsb_min = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_deleteLog = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1Collapsed = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtb);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rtb
            // 
            this.rtb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.rtb, "rtb");
            this.rtb.Name = "rtb";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_DbgRun,
            this.tsb_Stop,
            this.tsb_StepOver,
            this.tsb_StepInto,
            this.tsb_max,
            this.tsb_min,
            this.toolStripSeparator2,
            this.toolStripSeparator1,
            this.tsb_deleteLog});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // tsb_DbgRun
            // 
            this.tsb_DbgRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_DbgRun, "tsb_DbgRun");
            this.tsb_DbgRun.Image = global::ide.Properties.Resources.run16;
            this.tsb_DbgRun.Name = "tsb_DbgRun";
            this.tsb_DbgRun.Click += new System.EventHandler(this.tsb_DbgRun_Click);
            // 
            // tsb_Stop
            // 
            this.tsb_Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_Stop, "tsb_Stop");
            this.tsb_Stop.Image = global::ide.Properties.Resources.dbg_stop16;
            this.tsb_Stop.Name = "tsb_Stop";
            this.tsb_Stop.Click += new System.EventHandler(this.tsb_Stop_Click);
            // 
            // tsb_StepOver
            // 
            this.tsb_StepOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_StepOver, "tsb_StepOver");
            this.tsb_StepOver.Image = global::ide.Properties.Resources.dbg_stepover16;
            this.tsb_StepOver.Name = "tsb_StepOver";
            this.tsb_StepOver.Click += new System.EventHandler(this.tsb_StepOver_Click);
            // 
            // tsb_StepInto
            // 
            this.tsb_StepInto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsb_StepInto, "tsb_StepInto");
            this.tsb_StepInto.Image = global::ide.Properties.Resources.dbg_stepinto16;
            this.tsb_StepInto.Name = "tsb_StepInto";
            this.tsb_StepInto.Click += new System.EventHandler(this.tsb_StepInto_Click);
            // 
            // tsb_max
            // 
            this.tsb_max.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_max.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_max.Image = global::ide.Properties.Resources.max16;
            resources.ApplyResources(this.tsb_max, "tsb_max");
            this.tsb_max.Name = "tsb_max";
            this.tsb_max.Tag = "max";
            this.tsb_max.Click += new System.EventHandler(this.tsb_Max_Click);
            // 
            // tsb_min
            // 
            this.tsb_min.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_min.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_min.Image = global::ide.Properties.Resources.min16;
            resources.ApplyResources(this.tsb_min, "tsb_min");
            this.tsb_min.Name = "tsb_min";
            this.tsb_min.Click += new System.EventHandler(this.tsb_min_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsb_deleteLog
            // 
            this.tsb_deleteLog.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_deleteLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_deleteLog.Image = global::ide.Properties.Resources.delete16;
            resources.ApplyResources(this.tsb_deleteLog, "tsb_deleteLog");
            this.tsb_deleteLog.Name = "tsb_deleteLog";
            this.tsb_deleteLog.Click += new System.EventHandler(this.tsb_deleteLog_Click);
            // 
            // DebugPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DebugPanel";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_DbgRun;
        private System.Windows.Forms.ToolStripButton tsb_Stop;
        private System.Windows.Forms.ToolStripButton tsb_StepOver;
        private System.Windows.Forms.ToolStripButton tsb_StepInto;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox rtb;
        private System.Windows.Forms.ToolStripButton tsb_max;
        private System.Windows.Forms.ToolStripButton tsb_min;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsb_deleteLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
