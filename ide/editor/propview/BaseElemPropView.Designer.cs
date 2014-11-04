namespace WebMaster.ide.editor.propview
{
    partial class BaseElemPropView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseElemPropView));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.tb_Description = new System.Windows.Forms.TextBox();
            this.labelType = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_msg = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tb_name
            // 
            resources.ApplyResources(this.tb_name, "tb_name");
            this.tb_name.Name = "tb_name";
            this.tb_name.TextChanged += new System.EventHandler(this.tb_name_TextChanged);
            this.tb_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyDown);
            this.tb_name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_name_KeyUp);
            this.tb_name.Leave += new System.EventHandler(this.tb_Name_Leave);
            // 
            // tb_Description
            // 
            resources.ApplyResources(this.tb_Description, "tb_Description");
            this.tb_Description.Name = "tb_Description";
            this.tb_Description.TextChanged += new System.EventHandler(this.tb_Description_TextChanged);
            this.tb_Description.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_Description_KeyDown);
            this.tb_Description.Leave += new System.EventHandler(this.tb_Description_Leave);
            // 
            // labelType
            // 
            resources.ApplyResources(this.labelType, "labelType");
            this.labelType.ForeColor = System.Drawing.SystemColors.Desktop;
            this.labelType.Name = "labelType";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel6);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Controls.Add(this.panel1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label_msg);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // label_msg
            // 
            resources.ApplyResources(this.label_msg, "label_msg");
            this.label_msg.Name = "label_msg";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tb_name);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.Window;
            this.panel6.Controls.Add(this.labelType);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tb_Description);
            this.panel3.Controls.Add(this.label2);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // BaseElemPropView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "BaseElemPropView";
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.TextBox tb_Description;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label_msg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel6;
    }
}
