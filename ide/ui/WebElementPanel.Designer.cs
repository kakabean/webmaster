using WebMaster.lib.engine;
namespace WebMaster.ide.ui
{
    partial class WebElementPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebElementPanel));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.ts_ddb = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmi_keystring = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_attribute = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_location = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_color = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_image = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_capture = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_PropCheck = new System.Windows.Forms.ToolStripButton();
            this.tsb_PropNav = new System.Windows.Forms.ToolStripButton();
            this.tsb_weUpdate = new System.Windows.Forms.ToolStripButton();
            this.tsb_msg = new System.Windows.Forms.ToolStripLabel();
            this.tsb_resetPVSize = new System.Windows.Forms.ToolStripButton();
            this.tsb_PropMin = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsddb_refineWE = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmi_autoRefWE = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_customizeRefWE = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddb_AdCap = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmi_capParent = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_capSibling = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_ddb,
            this.toolStripSeparator4,
            this.tsb_capture,
            this.toolStripSeparator1,
            this.tsb_PropCheck,
            this.tsb_PropNav,
            this.tsb_weUpdate,
            this.tsb_msg,
            this.tsb_resetPVSize,
            this.tsb_PropMin,
            this.toolStripSeparator5,
            this.tsddb_refineWE,
            this.tsddb_AdCap});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // ts_ddb
            // 
            resources.ApplyResources(this.ts_ddb, "ts_ddb");
            this.ts_ddb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ts_ddb.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_keystring,
            this.tsmi_attribute,
            this.tsmi_location,
            this.tsmi_color,
            this.tsmi_image});
            this.ts_ddb.Image = global::ide.Properties.Resources.we_attr16;
            this.ts_ddb.Name = "ts_ddb";
            this.ts_ddb.Tag = WebMaster.lib.engine.WEType.CODE;
            this.ts_ddb.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddb_DropDownItemClicked);
            // 
            // tsmi_keystring
            // 
            resources.ApplyResources(this.tsmi_keystring, "tsmi_keystring");
            this.tsmi_keystring.Image = global::ide.Properties.Resources.we_attr16;
            this.tsmi_keystring.Name = "tsmi_keystring";
            this.tsmi_keystring.Tag = WebMaster.lib.engine.WEType.CODE;
            // 
            // tsmi_attribute
            // 
            resources.ApplyResources(this.tsmi_attribute, "tsmi_attribute");
            this.tsmi_attribute.Image = global::ide.Properties.Resources.we_attr16;
            this.tsmi_attribute.Name = "tsmi_attribute";
            this.tsmi_attribute.Tag = "ATTRIBUTE";
            // 
            // tsmi_location
            // 
            resources.ApplyResources(this.tsmi_location, "tsmi_location");
            this.tsmi_location.Image = global::ide.Properties.Resources.we_location16;
            this.tsmi_location.Name = "tsmi_location";
            this.tsmi_location.Tag = "LOCATION";
            // 
            // tsmi_color
            // 
            resources.ApplyResources(this.tsmi_color, "tsmi_color");
            this.tsmi_color.Image = global::ide.Properties.Resources.we_color16;
            this.tsmi_color.Name = "tsmi_color";
            this.tsmi_color.Tag = WebMaster.lib.engine.WEType.COLOR;
            // 
            // tsmi_image
            // 
            resources.ApplyResources(this.tsmi_image, "tsmi_image");
            this.tsmi_image.Image = global::ide.Properties.Resources.we_image16;
            this.tsmi_image.Name = "tsmi_image";
            this.tsmi_image.Tag = "IMAGE";
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // tsb_capture
            // 
            resources.ApplyResources(this.tsb_capture, "tsb_capture");
            this.tsb_capture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_capture.Image = global::ide.Properties.Resources.we_capture16;
            this.tsb_capture.Name = "tsb_capture";
            this.tsb_capture.Click += new System.EventHandler(this.tsb_capture_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // tsb_PropCheck
            // 
            resources.ApplyResources(this.tsb_PropCheck, "tsb_PropCheck");
            this.tsb_PropCheck.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_PropCheck.Image = global::ide.Properties.Resources.check16;
            this.tsb_PropCheck.Name = "tsb_PropCheck";
            this.tsb_PropCheck.Click += new System.EventHandler(this.tsb_Click_checkWE);
            // 
            // tsb_PropNav
            // 
            resources.ApplyResources(this.tsb_PropNav, "tsb_PropNav");
            this.tsb_PropNav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_PropNav.Image = global::ide.Properties.Resources.we_nav16;
            this.tsb_PropNav.Name = "tsb_PropNav";
            this.tsb_PropNav.Click += new System.EventHandler(this.toolStripBtnWeNav_Click);
            // 
            // tsb_weUpdate
            // 
            resources.ApplyResources(this.tsb_weUpdate, "tsb_weUpdate");
            this.tsb_weUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_weUpdate.Image = global::ide.Properties.Resources.we_confirm16;
            this.tsb_weUpdate.Name = "tsb_weUpdate";
            this.tsb_weUpdate.Click += new System.EventHandler(this.tsb_weUpdate_Click);
            // 
            // tsb_msg
            // 
            resources.ApplyResources(this.tsb_msg, "tsb_msg");
            this.tsb_msg.Name = "tsb_msg";
            // 
            // tsb_resetPVSize
            // 
            resources.ApplyResources(this.tsb_resetPVSize, "tsb_resetPVSize");
            this.tsb_resetPVSize.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_resetPVSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_resetPVSize.Image = global::ide.Properties.Resources.restoreSize16;
            this.tsb_resetPVSize.Name = "tsb_resetPVSize";
            this.tsb_resetPVSize.Click += new System.EventHandler(this.tsb_resetPVSize_Click);
            // 
            // tsb_PropMin
            // 
            resources.ApplyResources(this.tsb_PropMin, "tsb_PropMin");
            this.tsb_PropMin.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_PropMin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_PropMin.Image = global::ide.Properties.Resources.min16;
            this.tsb_PropMin.Name = "tsb_PropMin";
            this.tsb_PropMin.Click += new System.EventHandler(this.tsb_PropMin_Click);
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // tsddb_refineWE
            // 
            resources.ApplyResources(this.tsddb_refineWE, "tsddb_refineWE");
            this.tsddb_refineWE.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsddb_refineWE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddb_refineWE.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_autoRefWE,
            this.tsmi_customizeRefWE});
            this.tsddb_refineWE.Name = "tsddb_refineWE";
            // 
            // tsmi_autoRefWE
            // 
            resources.ApplyResources(this.tsmi_autoRefWE, "tsmi_autoRefWE");
            this.tsmi_autoRefWE.Name = "tsmi_autoRefWE";
            this.tsmi_autoRefWE.Click += new System.EventHandler(this.tsmi_autoRefWE_Click);
            // 
            // tsmi_customizeRefWE
            // 
            resources.ApplyResources(this.tsmi_customizeRefWE, "tsmi_customizeRefWE");
            this.tsmi_customizeRefWE.Name = "tsmi_customizeRefWE";
            this.tsmi_customizeRefWE.Click += new System.EventHandler(this.tsmi_customizeRefWE_Click);
            // 
            // tsddb_AdCap
            // 
            resources.ApplyResources(this.tsddb_AdCap, "tsddb_AdCap");
            this.tsddb_AdCap.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsddb_AdCap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddb_AdCap.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_capParent,
            this.tsmi_capSibling});
            this.tsddb_AdCap.Name = "tsddb_AdCap";
            // 
            // tsmi_capParent
            // 
            resources.ApplyResources(this.tsmi_capParent, "tsmi_capParent");
            this.tsmi_capParent.Name = "tsmi_capParent";
            this.tsmi_capParent.Click += new System.EventHandler(this.tsmi_capParent_Click);
            // 
            // tsmi_capSibling
            // 
            resources.ApplyResources(this.tsmi_capSibling, "tsmi_capSibling");
            this.tsmi_capSibling.Name = "tsmi_capSibling";
            this.tsmi_capSibling.Click += new System.EventHandler(this.tsmi_capSibling_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // WebElementPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip2);
            this.Name = "WebElementPanel";
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsb_PropCheck;
        private System.Windows.Forms.ToolStripButton tsb_PropNav;
        private System.Windows.Forms.ToolStripLabel tsb_msg;
        private System.Windows.Forms.ToolStripButton tsb_PropMin;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton tsb_weUpdate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton tsb_capture;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsb_resetPVSize;
        private System.Windows.Forms.ToolStripDropDownButton ts_ddb;
        private System.Windows.Forms.ToolStripMenuItem tsmi_keystring;
        private System.Windows.Forms.ToolStripMenuItem tsmi_attribute;
        private System.Windows.Forms.ToolStripMenuItem tsmi_location;
        private System.Windows.Forms.ToolStripMenuItem tsmi_color;
        private System.Windows.Forms.ToolStripMenuItem tsmi_image;
        private System.Windows.Forms.ToolStripDropDownButton tsddb_refineWE;
        private System.Windows.Forms.ToolStripMenuItem tsmi_autoRefWE;
        private System.Windows.Forms.ToolStripMenuItem tsmi_customizeRefWE;
        private System.Windows.Forms.ToolStripDropDownButton tsddb_AdCap;
        private System.Windows.Forms.ToolStripMenuItem tsmi_capParent;
        private System.Windows.Forms.ToolStripMenuItem tsmi_capSibling;
    }
}
