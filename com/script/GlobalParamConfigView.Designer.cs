namespace WebMaster.com.script
{
    partial class GlobalParamConfigView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalParamConfigView));
            this.grpParamCfg = new System.Windows.Forms.GroupBox();
            this.lv_params = new WebMaster.com.ListViewEx();
            this.colKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpParamCfg.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpParamCfg
            // 
            resources.ApplyResources(this.grpParamCfg, "grpParamCfg");
            this.grpParamCfg.Controls.Add(this.lv_params);
            this.grpParamCfg.Name = "grpParamCfg";
            this.grpParamCfg.TabStop = false;
            // 
            // lv_params
            // 
            resources.ApplyResources(this.lv_params, "lv_params");
            this.lv_params.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colKey,
            this.colValue});
            this.lv_params.DoubleClickActivation = false;
            this.lv_params.FullRowSelect = true;
            this.lv_params.GridLines = true;
            this.lv_params.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lv_params.MultiSelect = false;
            this.lv_params.Name = "lv_params";
            this.lv_params.ShowItemToolTips = true;
            this.lv_params.UseCompatibleStateImageBehavior = false;
            this.lv_params.View = System.Windows.Forms.View.Details;
            this.lv_params.SubItemClicked += new WebMaster.com.SubItemEventHandler(this.lv_params_SubItemClicked);
            this.lv_params.SubItemEndEditing += new WebMaster.com.SubItemEndEditingEventHandler(this.lv_params_SubItemEndEditing);
            this.lv_params.Resize += new System.EventHandler(this.lv_params_Resize);
            // 
            // colKey
            // 
            resources.ApplyResources(this.colKey, "colKey");
            // 
            // colValue
            // 
            resources.ApplyResources(this.colValue, "colValue");
            // 
            // GlobalParamConfigView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpParamCfg);
            this.Name = "GlobalParamConfigView";
            this.grpParamCfg.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpParamCfg;
        private WebMaster.com.ListViewEx lv_params;
        private System.Windows.Forms.ColumnHeader colKey;
        private System.Windows.Forms.ColumnHeader colValue;
    }
}
