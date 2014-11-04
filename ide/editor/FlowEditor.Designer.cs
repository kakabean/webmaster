using System.Windows.Forms;
namespace WebMaster.ide.editor
{
    partial class FlowEditor
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
            if (disposing && (components != null))
            {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowEditor));
            this.TreeAndWorkarea = new System.Windows.Forms.SplitContainer();
            this.elemTabCtrl = new System.Windows.Forms.TabControl();
            this.wePage = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.weTreeView1 = new System.Windows.Forms.TreeView();
            this.cms_wetree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiNewWEG = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmEditWE = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Spe_Ad = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_Advanced = new System.Windows.Forms.ToolStripMenuItem();
            this.weLVImgList = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_TreeSearch = new System.Windows.Forms.Button();
            this.paramPage = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.paramTV = new System.Windows.Forms.TreeView();
            this.cms_paramTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_AddParam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_paramSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_AddParamGrp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_paramSep = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_ParamTVDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lable_proc = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btn_paraSearch = new System.Windows.Forms.Button();
            this.WorkareaAndProp = new System.Windows.Forms.SplitContainer();
            this.workareaTabCtrl = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_Start = new System.Windows.Forms.ToolStripButton();
            this.tsb_End = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_OpenURL = new System.Windows.Forms.ToolStripButton();
            this.tsb_Click = new System.Windows.Forms.ToolStripButton();
            this.tsb_Input = new System.Windows.Forms.ToolStripButton();
            this.tsb_Process = new System.Windows.Forms.ToolStripButton();
            this.tsb_Link = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_CloseDiagram = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_Nop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.propTabCtrl = new System.Windows.Forms.TabControl();
            this.genPage = new System.Windows.Forms.TabPage();
            this.conditionPage = new System.Windows.Forms.TabPage();
            this.rulePage = new System.Windows.Forms.TabPage();
            this.errPage = new System.Windows.Forms.TabPage();
            this.logPage = new System.Windows.Forms.TabPage();
            this.cms_canvas = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiDeleteCanvasNode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_debug = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.TreeAndWorkarea)).BeginInit();
            this.TreeAndWorkarea.Panel1.SuspendLayout();
            this.TreeAndWorkarea.Panel2.SuspendLayout();
            this.TreeAndWorkarea.SuspendLayout();
            this.elemTabCtrl.SuspendLayout();
            this.wePage.SuspendLayout();
            this.panel2.SuspendLayout();
            this.cms_wetree.SuspendLayout();
            this.panel1.SuspendLayout();
            this.paramPage.SuspendLayout();
            this.panel4.SuspendLayout();
            this.cms_paramTree.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorkareaAndProp)).BeginInit();
            this.WorkareaAndProp.Panel1.SuspendLayout();
            this.WorkareaAndProp.Panel2.SuspendLayout();
            this.WorkareaAndProp.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.propTabCtrl.SuspendLayout();
            this.cms_canvas.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeAndWorkarea
            // 
            resources.ApplyResources(this.TreeAndWorkarea, "TreeAndWorkarea");
            this.TreeAndWorkarea.Name = "TreeAndWorkarea";
            // 
            // TreeAndWorkarea.Panel1
            // 
            this.TreeAndWorkarea.Panel1.Controls.Add(this.elemTabCtrl);
            // 
            // TreeAndWorkarea.Panel2
            // 
            this.TreeAndWorkarea.Panel2.Controls.Add(this.WorkareaAndProp);
            this.TreeAndWorkarea.TabStop = false;
            // 
            // elemTabCtrl
            // 
            this.elemTabCtrl.AllowDrop = true;
            this.elemTabCtrl.Controls.Add(this.wePage);
            this.elemTabCtrl.Controls.Add(this.paramPage);
            resources.ApplyResources(this.elemTabCtrl, "elemTabCtrl");
            this.elemTabCtrl.Name = "elemTabCtrl";
            this.elemTabCtrl.SelectedIndex = 0;
            this.elemTabCtrl.SelectedIndexChanged += new System.EventHandler(this.elemTabCtrl_SelectedIndexChanged);
            // 
            // wePage
            // 
            this.wePage.Controls.Add(this.panel2);
            this.wePage.Controls.Add(this.panel1);
            resources.ApplyResources(this.wePage, "wePage");
            this.wePage.Name = "wePage";
            this.wePage.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.weTreeView1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // weTreeView1
            // 
            this.weTreeView1.AllowDrop = true;
            this.weTreeView1.ContextMenuStrip = this.cms_wetree;
            resources.ApplyResources(this.weTreeView1, "weTreeView1");
            this.weTreeView1.HideSelection = false;
            this.weTreeView1.ImageList = this.weLVImgList;
            this.weTreeView1.Name = "weTreeView1";
            this.weTreeView1.ShowNodeToolTips = true;
            this.weTreeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.weTreeView1_BeforeLabelEdit);
            this.weTreeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.weTreeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.weTreeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.weTreeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            this.weTreeView1.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.treeView_GiveFeedback);
            this.weTreeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseClick);
            this.weTreeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.weTreeView1_MouseDown);
            // 
            // cms_wetree
            // 
            this.cms_wetree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNewWEG,
            this.toolStripSeparator4,
            this.tsmEditWE,
            this.tsmiDelete,
            this.ts_Spe_Ad,
            this.tsmi_Advanced});
            this.cms_wetree.Name = "contextMenuStripWETree";
            resources.ApplyResources(this.cms_wetree, "cms_wetree");
            // 
            // tsmiNewWEG
            // 
            this.tsmiNewWEG.Image = global::ide.Properties.Resources.newCategory16;
            this.tsmiNewWEG.Name = "tsmiNewWEG";
            resources.ApplyResources(this.tsmiNewWEG, "tsmiNewWEG");
            this.tsmiNewWEG.Click += new System.EventHandler(this.tsmiNewWEG_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // tsmEditWE
            // 
            this.tsmEditWE.Image = global::ide.Properties.Resources.edit16;
            this.tsmEditWE.Name = "tsmEditWE";
            resources.ApplyResources(this.tsmEditWE, "tsmEditWE");
            this.tsmEditWE.Click += new System.EventHandler(this.tsmiEditWE_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Image = global::ide.Properties.Resources.delete16;
            this.tsmiDelete.Name = "tsmiDelete";
            resources.ApplyResources(this.tsmiDelete, "tsmiDelete");
            this.tsmiDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tsmiDelete_MouseUp);
            // 
            // ts_Spe_Ad
            // 
            this.ts_Spe_Ad.Name = "ts_Spe_Ad";
            resources.ApplyResources(this.ts_Spe_Ad, "ts_Spe_Ad");
            // 
            // tsmi_Advanced
            // 
            this.tsmi_Advanced.Name = "tsmi_Advanced";
            resources.ApplyResources(this.tsmi_Advanced, "tsmi_Advanced");
            this.tsmi_Advanced.Click += new System.EventHandler(this.tsmi_Advanced_Click);
            // 
            // weLVImgList
            // 
            this.weLVImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("weLVImgList.ImageStream")));
            this.weLVImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.weLVImgList.Images.SetKeyName(0, "script16.png");
            this.weLVImgList.Images.SetKeyName(1, "script_err16.png");
            this.weLVImgList.Images.SetKeyName(2, "script_warn16.png");
            this.weLVImgList.Images.SetKeyName(3, "web_elem16.gif");
            this.weLVImgList.Images.SetKeyName(4, "web_elem_err16.gif");
            this.weLVImgList.Images.SetKeyName(5, "web_elem_warn16.gif");
            this.weLVImgList.Images.SetKeyName(6, "wegrp16.gif");
            this.weLVImgList.Images.SetKeyName(7, "wegrp_err16.gif");
            this.weLVImgList.Images.SetKeyName(8, "wegrp_warn16.gif");
            this.weLVImgList.Images.SetKeyName(9, "param16.gif");
            this.weLVImgList.Images.SetKeyName(10, "param_err16.gif");
            this.weLVImgList.Images.SetKeyName(11, "param_warn16.gif");
            this.weLVImgList.Images.SetKeyName(12, "paramgrp16.gif");
            this.weLVImgList.Images.SetKeyName(13, "paramgrp_err16.gif");
            this.weLVImgList.Images.SetKeyName(14, "paramgrp_warn16.gif");
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.btn_TreeSearch);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // btn_TreeSearch
            // 
            resources.ApplyResources(this.btn_TreeSearch, "btn_TreeSearch");
            this.btn_TreeSearch.Image = global::ide.Properties.Resources.search16;
            this.btn_TreeSearch.Name = "btn_TreeSearch";
            this.btn_TreeSearch.UseVisualStyleBackColor = true;
            // 
            // paramPage
            // 
            this.paramPage.Controls.Add(this.panel4);
            this.paramPage.Controls.Add(this.panel3);
            resources.ApplyResources(this.paramPage, "paramPage");
            this.paramPage.Name = "paramPage";
            this.paramPage.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.paramTV);
            this.panel4.Controls.Add(this.panel5);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // paramTV
            // 
            this.paramTV.AllowDrop = true;
            this.paramTV.ContextMenuStrip = this.cms_paramTree;
            resources.ApplyResources(this.paramTV, "paramTV");
            this.paramTV.FullRowSelect = true;
            this.paramTV.HideSelection = false;
            this.paramTV.ImageList = this.weLVImgList;
            this.paramTV.Name = "paramTV";
            this.paramTV.ShowNodeToolTips = true;
            this.paramTV.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.paramTV_AfterLabelEdit);
            this.paramTV.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.paramTV_ItemDrag);
            this.paramTV.DragDrop += new System.Windows.Forms.DragEventHandler(this.paramTV_DragDrop);
            this.paramTV.DragOver += new System.Windows.Forms.DragEventHandler(this.paramTV_DragOver);
            this.paramTV.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.treeView_GiveFeedback);
            this.paramTV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.paramTV_MouseDown);
            this.paramTV.MouseUp += new System.Windows.Forms.MouseEventHandler(this.paramTV_MouseUp);
            // 
            // cms_paramTree
            // 
            this.cms_paramTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_AddParam,
            this.tsmi_paramSep1,
            this.tsmi_AddParamGrp,
            this.tsmi_paramSep,
            this.tsmi_ParamTVDelete});
            this.cms_paramTree.Name = "cms_paramTree";
            resources.ApplyResources(this.cms_paramTree, "cms_paramTree");
            // 
            // tsmi_AddParam
            // 
            this.tsmi_AddParam.Image = global::ide.Properties.Resources.param16;
            this.tsmi_AddParam.Name = "tsmi_AddParam";
            resources.ApplyResources(this.tsmi_AddParam, "tsmi_AddParam");
            this.tsmi_AddParam.Click += new System.EventHandler(this.tsmi_AddParam_Click);
            // 
            // tsmi_paramSep1
            // 
            this.tsmi_paramSep1.Name = "tsmi_paramSep1";
            resources.ApplyResources(this.tsmi_paramSep1, "tsmi_paramSep1");
            // 
            // tsmi_AddParamGrp
            // 
            this.tsmi_AddParamGrp.Image = global::ide.Properties.Resources.paramgrp16;
            this.tsmi_AddParamGrp.Name = "tsmi_AddParamGrp";
            resources.ApplyResources(this.tsmi_AddParamGrp, "tsmi_AddParamGrp");
            this.tsmi_AddParamGrp.Click += new System.EventHandler(this.tsmi_AddParamGrp_Click);
            // 
            // tsmi_paramSep
            // 
            this.tsmi_paramSep.Name = "tsmi_paramSep";
            resources.ApplyResources(this.tsmi_paramSep, "tsmi_paramSep");
            // 
            // tsmi_ParamTVDelete
            // 
            this.tsmi_ParamTVDelete.Image = global::ide.Properties.Resources.delete16;
            this.tsmi_ParamTVDelete.Name = "tsmi_ParamTVDelete";
            resources.ApplyResources(this.tsmi_ParamTVDelete, "tsmi_ParamTVDelete");
            this.tsmi_ParamTVDelete.Click += new System.EventHandler(this.tsmi_ParamTVDelete_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lable_proc);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // lable_proc
            // 
            resources.ApplyResources(this.lable_proc, "lable_proc");
            this.lable_proc.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lable_proc.Name = "lable_proc";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LightGray;
            this.panel3.Controls.Add(this.textBox2);
            this.panel3.Controls.Add(this.btn_paraSearch);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // textBox2
            // 
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.Name = "textBox2";
            // 
            // btn_paraSearch
            // 
            resources.ApplyResources(this.btn_paraSearch, "btn_paraSearch");
            this.btn_paraSearch.Image = global::ide.Properties.Resources.search16;
            this.btn_paraSearch.Name = "btn_paraSearch";
            this.btn_paraSearch.UseVisualStyleBackColor = true;
            // 
            // WorkareaAndProp
            // 
            resources.ApplyResources(this.WorkareaAndProp, "WorkareaAndProp");
            this.WorkareaAndProp.Name = "WorkareaAndProp";
            // 
            // WorkareaAndProp.Panel1
            // 
            this.WorkareaAndProp.Panel1.Controls.Add(this.workareaTabCtrl);
            this.WorkareaAndProp.Panel1.Controls.Add(this.toolStrip1);
            // 
            // WorkareaAndProp.Panel2
            // 
            this.WorkareaAndProp.Panel2.Controls.Add(this.propTabCtrl);
            this.WorkareaAndProp.TabStop = false;
            this.WorkareaAndProp.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.WorkareaAndProp_SplitterMoved);
            this.WorkareaAndProp.Resize += new System.EventHandler(this.WorkareaAndProp_Resize);
            // 
            // workareaTabCtrl
            // 
            resources.ApplyResources(this.workareaTabCtrl, "workareaTabCtrl");
            this.workareaTabCtrl.Name = "workareaTabCtrl";
            this.workareaTabCtrl.SelectedIndex = 0;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_Start,
            this.tsb_End,
            this.toolStripSeparator1,
            this.tsb_OpenURL,
            this.tsb_Click,
            this.tsb_Input,
            this.tsb_Process,
            this.tsb_Link,
            this.toolStripSeparator2,
            this.tsb_CloseDiagram,
            this.toolStripSeparator3,
            this.tsb_Nop,
            this.toolStripSeparator6});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // tsb_Start
            // 
            this.tsb_Start.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_Start.Image = global::ide.Properties.Resources.op_start16;
            resources.ApplyResources(this.tsb_Start, "tsb_Start");
            this.tsb_Start.Name = "tsb_Start";
            this.tsb_Start.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_Start_MouseDown);
            // 
            // tsb_End
            // 
            this.tsb_End.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_End.Image = global::ide.Properties.Resources.op_stop16;
            resources.ApplyResources(this.tsb_End, "tsb_End");
            this.tsb_End.Name = "tsb_End";
            this.tsb_End.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_End_MouseDown);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsb_OpenURL
            // 
            this.tsb_OpenURL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_OpenURL.Image = global::ide.Properties.Resources.op_url16;
            resources.ApplyResources(this.tsb_OpenURL, "tsb_OpenURL");
            this.tsb_OpenURL.Name = "tsb_OpenURL";
            this.tsb_OpenURL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_OpenURL_MouseDown);
            // 
            // tsb_Click
            // 
            this.tsb_Click.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_Click.Image = global::ide.Properties.Resources.op_click16;
            resources.ApplyResources(this.tsb_Click, "tsb_Click");
            this.tsb_Click.Name = "tsb_Click";
            this.tsb_Click.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_Click_MouseDown);
            // 
            // tsb_Input
            // 
            this.tsb_Input.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_Input.Image = global::ide.Properties.Resources.op_input16;
            resources.ApplyResources(this.tsb_Input, "tsb_Input");
            this.tsb_Input.Name = "tsb_Input";
            this.tsb_Input.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_Input_MouseDown);
            // 
            // tsb_Process
            // 
            this.tsb_Process.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_Process.Image = global::ide.Properties.Resources.op_process16;
            resources.ApplyResources(this.tsb_Process, "tsb_Process");
            this.tsb_Process.Name = "tsb_Process";
            this.tsb_Process.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_Process_MouseDown);
            // 
            // tsb_Link
            // 
            this.tsb_Link.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_Link.Image = global::ide.Properties.Resources.op_link16;
            resources.ApplyResources(this.tsb_Link, "tsb_Link");
            this.tsb_Link.Name = "tsb_Link";
            this.tsb_Link.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_Link_MouseDown);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tsb_CloseDiagram
            // 
            this.tsb_CloseDiagram.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsb_CloseDiagram.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_CloseDiagram.Image = global::ide.Properties.Resources.close16;
            resources.ApplyResources(this.tsb_CloseDiagram, "tsb_CloseDiagram");
            this.tsb_CloseDiagram.Name = "tsb_CloseDiagram";
            this.tsb_CloseDiagram.Click += new System.EventHandler(this.tsb_CloseDiagram_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // tsb_Nop
            // 
            this.tsb_Nop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_Nop.Image = global::ide.Properties.Resources.op_nop16;
            resources.ApplyResources(this.tsb_Nop, "tsb_Nop");
            this.tsb_Nop.Name = "tsb_Nop";
            this.tsb_Nop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_Nop_MouseDown);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // propTabCtrl
            // 
            this.propTabCtrl.Controls.Add(this.genPage);
            this.propTabCtrl.Controls.Add(this.conditionPage);
            this.propTabCtrl.Controls.Add(this.rulePage);
            this.propTabCtrl.Controls.Add(this.errPage);
            this.propTabCtrl.Controls.Add(this.logPage);
            resources.ApplyResources(this.propTabCtrl, "propTabCtrl");
            this.propTabCtrl.Name = "propTabCtrl";
            this.propTabCtrl.SelectedIndex = 0;
            // 
            // genPage
            // 
            resources.ApplyResources(this.genPage, "genPage");
            this.genPage.Name = "genPage";
            this.genPage.UseVisualStyleBackColor = true;
            // 
            // conditionPage
            // 
            resources.ApplyResources(this.conditionPage, "conditionPage");
            this.conditionPage.Name = "conditionPage";
            this.conditionPage.UseVisualStyleBackColor = true;
            // 
            // rulePage
            // 
            resources.ApplyResources(this.rulePage, "rulePage");
            this.rulePage.Name = "rulePage";
            this.rulePage.UseVisualStyleBackColor = true;
            // 
            // errPage
            // 
            resources.ApplyResources(this.errPage, "errPage");
            this.errPage.Name = "errPage";
            this.errPage.UseVisualStyleBackColor = true;
            // 
            // logPage
            // 
            resources.ApplyResources(this.logPage, "logPage");
            this.logPage.Name = "logPage";
            this.logPage.UseVisualStyleBackColor = true;
            // 
            // cms_canvas
            // 
            this.cms_canvas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDeleteCanvasNode,
            this.toolStripSeparator5,
            this.tsmi_debug});
            this.cms_canvas.Name = "cms_canvas";
            resources.ApplyResources(this.cms_canvas, "cms_canvas");
            // 
            // tsmiDeleteCanvasNode
            // 
            this.tsmiDeleteCanvasNode.Image = global::ide.Properties.Resources.delete16;
            this.tsmiDeleteCanvasNode.Name = "tsmiDeleteCanvasNode";
            resources.ApplyResources(this.tsmiDeleteCanvasNode, "tsmiDeleteCanvasNode");
            this.tsmiDeleteCanvasNode.Click += new System.EventHandler(this.tsmiDeleteCanvasNode_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // tsmi_debug
            // 
            this.tsmi_debug.Image = global::ide.Properties.Resources.debug16;
            this.tsmi_debug.Name = "tsmi_debug";
            resources.ApplyResources(this.tsmi_debug, "tsmi_debug");
            this.tsmi_debug.Click += new System.EventHandler(this.tsmi_debug_Click);
            // 
            // FlowEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TreeAndWorkarea);
            this.Name = "FlowEditor";
            this.TreeAndWorkarea.Panel1.ResumeLayout(false);
            this.TreeAndWorkarea.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TreeAndWorkarea)).EndInit();
            this.TreeAndWorkarea.ResumeLayout(false);
            this.elemTabCtrl.ResumeLayout(false);
            this.wePage.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.cms_wetree.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.paramPage.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.cms_paramTree.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.WorkareaAndProp.Panel1.ResumeLayout(false);
            this.WorkareaAndProp.Panel1.PerformLayout();
            this.WorkareaAndProp.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.WorkareaAndProp)).EndInit();
            this.WorkareaAndProp.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.propTabCtrl.ResumeLayout(false);
            this.cms_canvas.ResumeLayout(false);
            this.ResumeLayout(false);

        }        
        #endregion

        private System.Windows.Forms.SplitContainer TreeAndWorkarea;
        private System.Windows.Forms.TabControl elemTabCtrl;
        private System.Windows.Forms.TabPage wePage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage paramPage;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer WorkareaAndProp;
        private System.Windows.Forms.TabControl workareaTabCtrl;
        private System.Windows.Forms.TabControl propTabCtrl;
        private System.Windows.Forms.TabPage genPage;
        private System.Windows.Forms.TabPage conditionPage;
        private System.Windows.Forms.TabPage rulePage;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_TreeSearch;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_Start;
        private System.Windows.Forms.ToolStripButton tsb_End;
        private System.Windows.Forms.ToolStripButton tsb_Click;
        private System.Windows.Forms.ToolStripButton tsb_Input;
        private System.Windows.Forms.ToolStripButton tsb_Process;
        private System.Windows.Forms.ToolStripButton tsb_Link;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsb_CloseDiagram;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btn_paraSearch;
        private System.Windows.Forms.TreeView weTreeView1;
        private System.Windows.Forms.ContextMenuStrip cms_wetree;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewWEG;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmEditWE;
        private System.Windows.Forms.ContextMenuStrip cms_canvas;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteCanvasNode;
        private System.Windows.Forms.TabPage errPage;
        private System.Windows.Forms.ImageList weLVImgList;
        private ToolStripButton tsb_OpenURL;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem tsmi_debug;
        private TabPage logPage;
        private TreeView paramTV;
        private ContextMenuStrip cms_paramTree;
        private ToolStripMenuItem tsmi_AddParamGrp;
        private ToolStripMenuItem tsmi_AddParam;
        private ToolStripSeparator tsmi_paramSep;
        private ToolStripMenuItem tsmi_ParamTVDelete;
        private ToolStripButton tsb_Nop;
        private ToolStripSeparator toolStripSeparator6;
        private Panel panel5;
        private Label lable_proc;
        private ToolStripSeparator tsmi_paramSep1;
        private ToolStripMenuItem tsmi_Advanced;
        private ToolStripSeparator ts_Spe_Ad;
    }
}
