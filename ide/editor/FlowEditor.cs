using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.ide.editor.model;
using WebMaster.lib.engine;
using WebMaster.ide.editor.propview;
using WebMaster.ide.ui;
using WebMaster.ide.editor.commands;
using System.Collections;
using WebMaster.lib.ui;
using WebMaster.lib;
using WebMaster.com;
using WebMaster.com.script;

namespace WebMaster.ide.editor
{
    public partial class FlowEditor : UserControl
    {
        #region constants 
        private static readonly int TREE_ICON_ROOT_VALID = 0;
        private static readonly int TREE_ICON_ROOT_ERROR = 1;
        private static readonly int TREE_ICON_ROOT_WARNING = 2;
        private static readonly int TREE_ICON_WE_VALID = 3 ;
        private static readonly int TREE_ICON_WE_ERROR = 4 ;
        private static readonly int TREE_ICON_WE_WARNING = 5 ;
        private static readonly int TREE_ICON_WEG_VALID = 6 ;
        private static readonly int TREE_ICON_WEG_ERROR = 7 ;
        private static readonly int TREE_ICON_WEG_WARNING = 8 ;
        private static readonly int TREE_ICON_PARAM_VALID = 9;
        private static readonly int TREE_ICON_PARAM_ERROR = 10;
        private static readonly int TREE_ICON_PARAM_WARNING = 11;
        private static readonly int TREE_ICON_PARAMGRP_VALID = 12;
        private static readonly int TREE_ICON_PARAMGRP_ERROR = 13;
        private static readonly int TREE_ICON_PARAMGRP_WARNING = 14;
        #endregion contants 
        #region variables
        private FlowPropViewManager flowPVManager = null;
        private DiagramManager diagramManager = null;
        /// <summary>
        /// current big model
        /// </summary>
        private BigModel bigmodel = null;
        /// <summary>
        /// WebElement tree view DnD start time, this is used to auto
        /// expand the target tree node if need 
        /// </summary>
        private DateTime _weTVDnDStartTime;
        /// <summary>
        /// WebElement tree view DnD target Node, the Node is current target node
        /// </summary>
        private TreeNode _weTreeCurrentTgtNode;
        /// <summary>
        /// WebElement tree view previous DnD target Node
        /// </summary>
        private TreeNode _weTreePreTgtNode;
        /// <summary>
        /// Parameter tree view DnD start time, this is used to auto
        /// expand the target tree node if need 
        /// </summary>
        private DateTime _paramTVDnDStartTime;
        /// <summary>
        /// Parameter tree view DnD target Node, the Node is current target node
        /// </summary>
        private TreeNode _paramTreeCurrentTgtNode;
        /// <summary>
        /// Parameter tree view previous DnD target Node
        /// </summary>
        private TreeNode _paramTreePreTgtNode;
        /// <summary>
        /// selected object in flow editor, e.g WebElement,WebElementGroup
        /// Operation, OpCondition,Process,Parameter
        /// </summary>
        private BaseElement _selectedObj = null;
        /// <summary>
        /// selected object in flow editor, e.g WebElement,WebElementGroup
        /// Operation, OpCondition, Process, Parameter, ParamGroup
        /// </summary>
        public BaseElement SelectedObj {
            get { return _selectedObj; }
            set {
                //if the _selectedObj changed, do a validation 
                if (_selectedObj != null && _selectedObj != value) {
                    validateElement(_selectedObj);
                }
                if (value != null && !value.Equals(_selectedObj)) {
                    selectionObjChanged(value);
                }
                _selectedObj = value; }
        }
        private WMEditor app = null;
        /// <summary>
        /// WMEditor object 
        /// </summary>
        public WMEditor App {
            get { return app; }
            set { app = value; }
        }
        /// <summary>
        /// validate whether the element is a valid, and update the error Log info, 
        /// 1. validate the name unique, 
        /// 2. validate based on specific element type
        /// </summary>
        /// <param name="be"></param>
        private void validateElement(BaseElement be) {
            //TOOD to be implemented next release 
        }
        /// <summary>
        /// 1. update current properties value into previous selected object. 
        ///    set input for all properties views and update proerties view in flow editor
        /// 2. clean active canvas status if WebElement/Parameter tree element selected. 
        /// </summary>
        /// <param name="selectedObj">WebElement,Operation, OpCondition,Process,Parameter,ParamGroup</param>
        private void selectionObjChanged(BaseElement selectedObj) {
            //1. update current properties value into previous selected object. 
            //   set input for all properties views and update proerties view in flow editor
            this.flowPVManager.inputChanged(selectedObj);
            // 2. clean active canvas status if WebElement tree element selected. 
            //    comments : if the parameter/ParamGroup selected, not clean the canvas selection status.
            if (selectedObj is WebElement || selectedObj is WebElementGroup/* || selectedObj is Parameter || selectedObj is ParamGroup*/) {
                this.diagramManager.ActiveCanvas.cleanSelectStatus();
            }
            // 3. clean WETree selecton status
            if (selectedObj is Operation || selectedObj is Process || selectedObj is OpCondition) {
                if (this.weTreeView1.SelectedNode != null) {
                    if (this.weTreeView1.SelectedNode.IsEditing) {                       
                        this.weTreeView1.SelectedNode.EndEdit(true);
                    }
                    if (this.weTreeView1.SelectedNode.IsSelected) {
                        this.weTreeView1.SelectedNode = null;
                    }
                }
            }
            // 4. clean Parameter Tree selection status and update Parameter tree
            handleParamTreeWhenSelectedObjChanged(selectedObj);
            // 5. handle the close buttons status 
            if (selectedObj == this.bigmodel.SRoot.ProcRoot) {
                this.tsb_CloseDiagram.Enabled = false;
            } else {
                this.tsb_CloseDiagram.Enabled = true;
            }
        }
        
        #endregion variables
        public FlowEditor()
        {
            InitializeComponent();
            doResize();
            initData();
        }

        private void initData() {
            // initial canvas toolEntry
            this.tsb_Start.Tag = OPERATION.START;
            this.tsb_End.Tag = OPERATION.END;
            this.tsb_Click.Tag = OPERATION.CLICK;
            this.tsb_Input.Tag = OPERATION.INPUT;
            this.tsb_Process.Tag = OPERATION.PROCESS;
            this.tsb_OpenURL.Tag = OPERATION.OPEN_URL_T;
            this.tsb_Nop.Tag = OPERATION.NOP;
            // initial flow manager 
            this.flowPVManager = new FlowPropViewManager(this);
            flowPVManager.initViews(propTabCtrl);
            flowPVManager.InputUpdatedEvt += new EventHandler<CommonEventArgs>(flowPVManager_InputUpdatedEvt);
            // initial diagram manager 
            diagramManager = new DiagramManager(this.workareaTabCtrl, this.cms_canvas,this);
            diagramManager.DiagramSelectionChanged += new EventHandler<CommonEventArgs>(diagramManager_DiagramSelectionChanged);
            diagramManager.ModelUpdateEvt += new EventHandler<CommonEventArgs>(diagramManager_ModelUpdateEvt);            
        }
        #region event handler
        void flowPVManager_InputUpdatedEvt(object sender, CommonEventArgs e) {
            if (e.Data is WebElement) {
                WebElement we = e.Data as WebElement;
                updateWETreeNodeText(we);
            } else if(e.Data is WebElementGroup){
                updateWETreeNodeText(e.Data as WebElementGroup);
            } else if (e.Data is Operation || e.Data is Process) {
                updateOpNodeText(e.Data as Operation);
                if (e.Data is Process) {
                    diagramManager.updateCanvasTitle(e.Data as Process);
                }
            } else if (e.Data is ConditionGroup) {
                updateOpConditionText(e.Data as ConditionGroup);
            } else if (e.Data is Parameter || e.Data is ParamGroup) {
                BaseElement be = e.Data as BaseElement;
                updateParamTreeNodeText(be);
            }

            this.App.markDirty();
        }

        void diagramManager_DiagramSelectionChanged(object sender, CommonEventArgs e) {
            SelectedObj = e.Data as BaseElement;            
        }
        /// <summary>
        /// update properties views if diagram model updated outside. 
        /// if operation/process updated, update the properties view
        /// if process updated, update the opened diagram title
        /// update editor dirty status if data is Node or Connector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void diagramManager_ModelUpdateEvt(object sender, CommonEventArgs e) {
            if (SelectedObj != null) {
                object data = e.Data;
                if (e.Data is Node) {
                    data = (e.Data as Node).RefOp;
                }
                if ((data is Process || data is Operation) && SelectedObj.Equals(data)) {
                    this.flowPVManager.updatePropViews(data as BaseElement);
                }
                if (SelectedObj is Process) {
                    diagramManager.updateCanvasTitle(SelectedObj as Process);
                }
                this.App.markDirty();
            }      
        }
        #endregion event handler end
        #region UI event handler         
        /// <summary>
        /// resize the properties view as a perferred height 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkareaAndProp_Resize(object sender, EventArgs e) {
            doResize();
        }
        /// <summary>
        /// resize the properties view area with a preferred height. 
        /// </summary>
        private void doResize() {
            int d = WorkareaAndProp.Size.Height - UIConstants.PREFER_PROPVIEW_HEIGHT;
            if (d > 300) {
                this.WorkareaAndProp.SplitterDistance = d;
            }
            //Log.println_prop("do resize, auto scroll = "+genPage.AutoScroll+", scrollMinSize = "+genPage.AutoScrollMinSize+", gen client size = "+genPage.ClientSize);
        }
        private void WorkareaAndProp_SplitterMoved(object sender, SplitterEventArgs e) {
            this.propTabCtrl.SelectedTab.Invalidate();
        }
        /// <summary>
        /// start to debug the script from the selected operaton node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_debug_Click(object sender, EventArgs e) {
            //TODO not supported debug from any op in this version 
            //if (this.SelectedObj is Operation || this.SelectedObj is Process) {
            //    Operation op = this.SelectedObj as Operation;
            //    // start to run the script 
            //    this.App.debug_Run(op);
            //}
        }
        #endregion UI event handler 
        #region events
        /// <summary>
        /// the sender is the WebElement tree node, the data is the WebElement to be 
        /// updated. it is occurred that the flow editor WebElement to be modified in 
        /// the WebElement properties view, e.g user maybe want to re-catch the element for some reason. 
        /// 
        /// </summary>
        public event EventHandler<CommonEventArgs> UpdateExistedWebElementEvt;
        protected virtual void OnUpdateExistedWebElementEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> updateExistedWebElementEvt = UpdateExistedWebElementEvt;
            if (updateExistedWebElementEvt != null) {
                updateExistedWebElementEvt(this, e);
            }
        }
        public void raiseUpdateExistedWebElementEvt(Object sender,WebElement we) {
            if (we != null) {
                CommonEventArgs evt = new CommonEventArgs(sender,we);
                OnUpdateExistedWebElementEvt(evt);
            }
        }
        #endregion events 
        #region common functions
        /// <summary>
        /// when a new script set, the view will be rebuilded 
        /// </summary>
        /// <param name="sroot"></param>
        public void setBigModel(BigModel bmodel) {
            if (bigmodel != bmodel || bmodel == null) {
                bigmodel = bmodel;
                // update properties view 
                flowPVManager.setBigModel(bigmodel);
                // update flow editor UI. WebElement tree, Parameter tree and diagram area 
                resetView();
            }
        }
        /// <summary>
        /// reset WebElement tree, paramter tree and  flow editor area and properties view 
        /// 
        /// if the script root existed, update all views based on the script root
        /// if script root is null, clean and disable view 
        /// </summary>
        private void resetView(){
            resetWETree();
            resetParamTree(null);
            resetDiagramArea();
        }
        /// <summary>
        /// clean flow editor area ui info, clean web element tree, canvas area and properties view 
        /// </summary>
        internal void cleanView() {
            cleanWETreeView();
            cleanParamTreeView();
            cleanDiagramArea();
            cleanFlowPropViews();
        }                
        #endregion common functions
        #region diagram area
        /// <summary>
        /// disable diagram editor area
        /// </summary>
        internal void disableDiagramView() {
            // disable toolstrip buttons 
            tsb_Click.Enabled = false;
            tsb_CloseDiagram.Enabled = false;
            tsb_End.Enabled = false;
            tsb_OpenURL.Enabled = false;
            tsb_Input.Enabled = false;
            tsb_Link.Enabled = false;
            tsb_Process.Enabled = false;
            tsb_Start.Enabled = false;
            tsb_Nop.Enabled = false;
        }
        /// <summary>
        /// enable diagram editor area 
        /// </summary>
        internal void enableDiagramView() {
            // enable toolstrip buttons 
            tsb_Click.Enabled = true;
            tsb_CloseDiagram.Enabled = true;
            tsb_End.Enabled = true;
            tsb_OpenURL.Enabled = true;
            tsb_Input.Enabled = true;
            tsb_Link.Enabled = true;
            tsb_Process.Enabled = true;
            tsb_Start.Enabled = true;
            tsb_Nop.Enabled = true;
        }
        /// <summary>
        /// close all opened editor tabs and open the default main editor of new 
        /// script
        /// if the script root existed, update view based on the script root
        /// if script root is null, clean and disable view 
        /// </summary>
        private void resetDiagramArea() {
            if (this.bigmodel != null) {
                // enable diagram editor
                this.enableDiagramView();
            } else {
                // disable digram editor
                this.disableDiagramView();
            }
            // update diagram area
            diagramManager.setBigModel(bigmodel);
        }
        /// <summary>
        /// update the OpCondition text info if have on the canvas 
        /// </summary>
        /// <param name="opCondition"></param>
        private void updateOpConditionText(ConditionGroup opCondition) {
            //TODO 
        }
        /// <summary>
        /// find the proper operaton/process node in the canvas and update the text if need
        /// </summary>
        /// <param name="operation"></param>
        private void updateOpNodeText(Operation operation) {
            if (this.diagramManager.ActiveCanvas != null && operation != null) {                
                foreach(Control uc in this.diagramManager.ActiveCanvas.Controls){                
                    if (uc is NodeView) {
                        NodeView nv = uc as NodeView;
                        if (operation.Equals(nv.Node.RefOp)) {
                            nv.Label = operation.Name;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// clean diagram area 
        /// </summary>
        private void cleanDiagramArea() {
            this.workareaTabCtrl.Controls.Clear();
        }
        /// <summary>
        /// handle canvas element delete command 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDeleteCanvasNode_Click(object sender, EventArgs e) {
            Canvas canvas = diagramManager.ActiveCanvas;
            DiagramUtil.delete(canvas,diagramManager);
        }
        
        internal void updateCanvasConextMenu() {
            this.tsmiDeleteCanvasNode.Visible = true;
            // TODO not supported debug from any op this version 
            //this.toolStripSeparator5.Visible = true;            
            //this.tsmi_debug.Visible = true;
            if (this.SelectedObj is Operation) {
                Operation op = SelectedObj as Operation;
                if (op.OpType == OPERATION.START) {
                    this.tsmiDeleteCanvasNode.Visible = false;
                    toolStripSeparator5.Visible = false;
                }
            } else if (this.SelectedObj is OpCondition) {
                this.toolStripSeparator5.Visible = false;
                this.tsmi_debug.Visible = false;
            }
        }
        #endregion diagram area
        #region diagram toolbar area
        /// <summary>
        /// start node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Start_MouseDown(object sender, MouseEventArgs e)
        {
            Log.println("mouse down : create new start node ");
            // update button status 
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }            
            diagramManager.ActiveTool = this.tsb_Start;
            diagramManager.ActiveTool.Checked = true;
            // handle DnD operaton
            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.START;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);
        }
        /// <summary>
        /// end node 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_End_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveTool = this.tsb_End;
            diagramManager.ActiveTool.Checked = true;
            
            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.END;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);            
        }
        /// <summary>
        /// open a URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_OpenURL_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveTool = this.tsb_OpenURL;
            diagramManager.ActiveTool.Checked = true;

            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.OPEN_URL_T;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);
        }
        /// <summary>
        /// left click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Click_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveTool = this.tsb_Click;
            diagramManager.ActiveTool.Checked = true;

            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.CLICK;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);                        
        }
        /// <summary>
        /// input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Input_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveTool = this.tsb_Input;
            diagramManager.ActiveTool.Checked = true;

            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.INPUT;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);
        }
        /// <summary>
        /// process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Process_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveTool = this.tsb_Process;
            diagramManager.ActiveTool.Checked = true;

            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.PROCESS;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);            
        }
        /// <summary>
        /// Nop 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Nop_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveTool = this.tsb_Nop;
            diagramManager.ActiveTool.Checked = true;

            DnDData ddd = new DnDData();
            ddd.DDType = DnDType.CREATE_NODE;
            ddd.Data = OPERATION.NOP;
            this.toolStrip1.DoDragDrop(ddd, DragDropEffects.Copy);  
        }
        /// <summary>
        /// create link from 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Link_MouseDown(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool != null) {
                diagramManager.ActiveTool.Checked = false;
            }
            diagramManager.ActiveCanvas.Action = ACTION.LINK;
            diagramManager.ActiveTool = this.tsb_Link;
            diagramManager.ActiveTool.Checked = true;
        }
        /// <summary>
        /// close non-main diagram. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_CloseDiagram_Click(object sender, EventArgs e) {
            if (this.diagramManager.ActiveCanvas.Diagram.Proc == this.bigmodel.SRoot.ProcRoot) {
                return;
            }
            this.diagramManager.closeActiveDiagram();
        }
        #endregion toolbar area
        #region WebElement tree area
        /// <summary>
        /// reset the WebElement tree with the script root 
        /// </summary>
        public void resetWETree() {
            if (this.bigmodel != null) {
                this.weTreeView1.BeginUpdate();
                // clean existed nodes 
                if (this.weTreeView1.Nodes.Count > 0) {
                    this.weTreeView1.Nodes.Clear();
                }
                // build new tree 
                TreeNode rn = createWERootNode(bigmodel.SRoot.WERoot);
                this.weTreeView1.Nodes.Add(rn);

                if (bigmodel.SRoot != null) {
                    // add new updated web element groups 
                    foreach (WebElementGroup weg in bigmodel.SRoot.WERoot.SubGroups) {
                        TreeNode gn = createWEGNode(weg);
                        rn.Nodes.Add(gn);
                    }
                    // add new updated web elements 
                    foreach (WebElement we in bigmodel.SRoot.WERoot.Elems) {
                        TreeNode wn = createWENode(we);
                        rn.Nodes.Add(wn);
                    }
                    // add raw data group 
                    TreeNode rgn = createWEGNode(bigmodel.SRoot.RawElemsGrp);
                    rn.Nodes.Add(rgn);
                }
                this.weTreeView1.EndUpdate();

                // disable Tree area
                this.btn_TreeSearch.Enabled = true;

                this.weTreeView1.Nodes[0].Expand();
            } else {
                this.weTreeView1.Nodes.Clear();
                // enable tree area
                this.btn_TreeSearch.Enabled = false;
            }
        }
        /// <summary>
        /// create WebElement node in the tree 
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        private TreeNode createWENode(WebElement we) {
            TreeNode node = new TreeNode();
            node.Text = we.Name;
            node.ToolTipText = we.Description;
            node.Tag = we;
            node.ImageIndex = TREE_ICON_WE_VALID;
            node.SelectedImageIndex = TREE_ICON_WE_VALID;
            return node;
        }
        /// <summary>
        /// create a WebElementGroup node in the tree 
        /// </summary>
        /// <param name="weg"></param>
        /// <returns></returns>
        private TreeNode createWEGNode(WebElementGroup weg) {
            //create node view 
            TreeNode gnode = new TreeNode();
            gnode.Text = weg.Name;
            gnode.ToolTipText = weg.Description;
            gnode.Tag = weg;
            gnode.ImageIndex = TREE_ICON_WEG_VALID;
            gnode.SelectedImageIndex = TREE_ICON_WEG_VALID;

            foreach (WebElementGroup seg in weg.SubGroups) {
                TreeNode sn = createWEGNode(seg);
                gnode.Nodes.Add(sn);
            }
            foreach (WebElement we in weg.Elems) {
                TreeNode en = createWENode(we);
                gnode.Nodes.Add(en);
            }
            return gnode;
        }
        /// <summary>
        /// create a Root node in the WebElement tree 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private TreeNode createWERootNode(WebElementGroup root) {
            TreeNode node = new TreeNode();
            node.Text = root.Name;
            node.Tag = root;
            node.ToolTipText = "Root of Web Elements";
            node.ImageIndex = TREE_ICON_ROOT_VALID;
            return node;
        }
        /// <summary>
        /// update the TreeNode info when the WebElement was updated out of flow editor, and make the updated 
        /// one selected, and if the properties view selected object is the WebElement, update view info 
        /// </summary>
        /// <param name="we"></param>
        internal void updateExistedWENodeAndPropViewIfNeed(WebElement we) {
            if(we!=null){
                // update tree 
                TreeNode root = this.weTreeView1.Nodes[0];
                TreeNode node = getNodeByBE(root, we);
                node.Text = we.Name;
                node.ToolTipText = we.Description;

                this.weTreeView1.SelectedNode = node;

                // update prop view if need 
                this.updatePropViewIfNeed(we);
            }
        }
        /// <summary>
        /// update the newly added WebElement into raw data group. and if there is a iframe/frame 
        /// referred, the iframe element will be added into script root iframes group. 
        /// 
        /// and set the new added node selected 
        /// </summary>
        /// <param name="we"></param>
        internal void addNewWENode(WebElement we) {
            bool valid = ModelManager.Instance.isUniqueToBeElement(we,this.bigmodel.SRoot.RawElemsGrp.Elems);
            if (valid) {
                ModelManager.Instance.updateSRootInternalWEs(we,this.bigmodel.SRoot);
                // add element to raw data
                bigmodel.SRoot.RawElemsGrp.Elems.AddUnique(we);
                // add tree node 
                TreeNode node = createWENode(we);
                TreeNode root = this.weTreeView1.Nodes[0];
                TreeNode rawNode = getNodeByBE(root, this.bigmodel.SRoot.RawElemsGrp);
                if (rawNode != null) {
                    rawNode.Nodes.Add(node);
                    this.weTreeView1.SelectedNode = node;
                    this.SelectedObj = we;
                }
            }
        }        
        /// <summary>
        /// get the tree node of the WebElement/WebElementGroup or null if not found, it will check all children
        /// under the parent node if needed
        /// </summary>
        /// <param name="pnode">parent node </param>
        /// <param name="be"></param>
        /// <returns></returns>
        private TreeNode getNodeByBE(TreeNode pnode, BaseElement be) {
            if (be!= null && be.Equals(pnode.Tag)) {
                return pnode;
            } else {
                foreach (TreeNode n in pnode.Nodes) {
                    TreeNode fn = getNodeByBE(n, be);
                    if (fn != null) {
                        return fn;
                    }
                }
            }
            return null;
        }
       
        private void weTreeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) {
            TreeNode node = this.weTreeView1.SelectedNode;
            // make sure the raw data node can not be udpated 
            if (node.Tag is WebElementGroup && this.bigmodel.SRoot.RawElemsGrp.Equals(node.Tag)) {
                e.CancelEdit = true;
            }
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (e.Label != null && e.Label != "") {
                TreeNode node = this.weTreeView1.SelectedNode;
                BaseElement be = node.Tag as BaseElement;
                bool valid = false;
                WebElement we = node.Tag as WebElement;
                if (we != null) {
                    WebElement twe = ModelFactory.createWebElement();
                    twe.Name = e.Label;
                    valid = ModelManager.Instance.isUniqueToBeElement(twe, we.Collection);
                } else if (node.Tag is WebElementGroup) {
                    WebElementGroup weg = node.Tag as WebElementGroup;
                    if (weg.Equals(this.bigmodel.SRoot.RawElemsGrp)) {
                        valid = false;
                    } else {
                        WebElementGroup tweg = ModelFactory.createWebElementGroup();
                        tweg.Name = e.Label;
                        if (weg == this.bigmodel.SRoot.WERoot) {
                            valid = true;
                        } else {
                            valid = ModelManager.Instance.isUniqueToBeElement(tweg, weg.Collection);
                        }
                    }
                }
                if (valid) {
                    be.Name = e.Label;
                    this.updatePropViewIfNeed(be);
                    this.App.markDirty();
                }
            }
        }

        private void weTreeView1_MouseDown(object sender, MouseEventArgs e) {
            TreeNode node = weTreeView1.GetNodeAt(e.X, e.Y);
            if (node == null) {
                return;
            }

            if (this.weTreeView1.SelectedNode != null && node == this.weTreeView1.SelectedNode) {
                this.weTreeView1.LabelEdit = true;
                this.weTreeView1.SelectedNode = node;
                this.weTreeView1.SelectedNode.BeginEdit();
            }

            if (node != weTreeView1.SelectedNode) {
                this.weTreeView1.LabelEdit = false;
                weTreeView1.SelectedNode = node;
            }

            //update selected object 
            this.SelectedObj = node.Tag as BaseElement;
        }
        /// <summary>
        /// it is used to control the context menu items 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseClick(object sender, MouseEventArgs e) {
            TreeNode node = weTreeView1.GetNodeAt(e.X, e.Y);

            if (node.Tag is WebElementGroup) {
                WebElementGroup weg = node.Tag as WebElementGroup;
                if (weg.Equals(this.bigmodel.SRoot.RawElemsGrp)) {
                    this.tsmiNewWEG.Visible = false;
                    this.toolStripSeparator4.Visible = false;
                    this.tsmiDelete.Visible = false;
                    this.tsmEditWE.Visible = false;
                    this.ts_Spe_Ad.Visible = false;
                    this.tsmi_Advanced.Visible = false;
                } else {
                    this.tsmiNewWEG.Visible = true;
                    this.toolStripSeparator4.Visible = true;
                    this.tsmiDelete.Visible = true;
                    this.tsmEditWE.Visible = false;
                    this.ts_Spe_Ad.Visible = false;
                    this.tsmi_Advanced.Visible = false;
                }
            } else if (node.Tag is WebElement) {
                this.tsmiNewWEG.Visible = false;
                this.toolStripSeparator4.Visible = false;
                this.tsmiDelete.Visible = true;
                this.tsmEditWE.Visible = true;
                this.ts_Spe_Ad.Visible = true;
                this.tsmi_Advanced.Visible = true;
            }
        }
        
        /// <summary>
        /// create a unique WebElementGroup in the list 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private WebElementGroup createUniqueWEG(BEList<WebElementGroup> list) {
            WebElementGroup weg = ModelFactory.createWebElementGroup();
            weg.Name = ModelManager.Instance.getUniqueElementName(list, weg);
            
            return weg;
        }
        /// <summary>
        /// open the browser view to show the WebElement info in WebElement propties view. 
        /// user can re-capture the element to update the WebElementAttributes info. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiEditWE_Click(object sender, EventArgs e) {
            TreeNode sn = weTreeView1.SelectedNode;
            if (sn != null && sn.Tag is WebElement) {
                this.raiseUpdateExistedWebElementEvt(sn, (WebElement)sn.Tag);
            }
        }
        /// <summary>
        /// Show the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_Advanced_Click(object sender, EventArgs e) {
            TreeNode node = this.weTreeView1.SelectedNode;
            if (node.Tag is WebElement) {
                WebElement we = node.Tag as WebElement;
                WebElementAdvancedEditDlg dlg = new WebElementAdvancedEditDlg();
                DialogResult dr = dlg.showEditDlg(this, we, this.bigmodel.SRoot);
                if (dr == DialogResult.OK) {
                    List<WebElementAttribute> list = new List<WebElementAttribute>();
                    list.AddRange(we.Attributes.ToArray());
                    foreach (WebElementAttribute wea in list) {
                        ModelManager.Instance.removeFromModel(wea);
                    }
                    
                    we.Attributes.AddRange(dlg.Output);

                    this.App.markDirty();
                }
            }
        }
        /// <summary>
        /// create a new category in the WebElement tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiNewWEG_Click(object sender, EventArgs e) {
            TreeNode pnode = weTreeView1.SelectedNode;
            WebElementGroup grp = this.weTreeView1.SelectedNode.Tag as WebElementGroup;
            if (grp != null) {
                WebElementGroup tg = createUniqueWEG(grp.SubGroups);
                grp.SubGroups.AddUnique(tg);
                TreeNode node = createWEGNode(tg);
                pnode.Nodes.Add(node);
                weTreeView1.SelectedNode = node;
                this.SelectedObj = tg;

                this.App.markDirty();
            }
        }
        /// <summary>
        /// delete a selected WebElementGroup or WebElement 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDelete_MouseUp(object sender, MouseEventArgs e) {
            removeWebElement();
        }
        /// <summary>
        /// remove WebElement Node from the tree view and model 
        /// </summary>
        private void removeWebElement() {
            TreeNode pnode = weTreeView1.SelectedNode;
            Object obj = this.weTreeView1.SelectedNode.Tag;

            WebElementGroup grp = obj as WebElementGroup;
            if (grp != null) {
                // check if there is reference for the contained WebElements
                // if there are some reference of the WebElement, notify a dialog for warning
                int count = ModelManager.Instance.getWEGRefTimes(grp);
                if (count > 0) {
                    string msg = UILangUtil.getMsg("ide.feditor.wetree.del.msg1",count);
                    string title = UILangUtil.getMsg("dlg.warn.title");
                    DialogResult dr = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg ,title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                    if (dr != DialogResult.OK) {
                        return;
                    }
                }
                ModelManager.Instance.removeFromModel(grp);
                pnode.Remove();
                this.App.markDirty();
            } else {
                WebElement we = obj as WebElement;
                if (we != null) {
                    // check if there is some reference for the contained WebElement, 
                    // if have, notify a dialog for warning
                    int count = ModelManager.Instance.getWERefTimes(we);
                    if (count > 0) {
                        string msg = UILangUtil.getMsg("ide.feditor.wetree.del.msg2", count);
                        string title = UILangUtil.getMsg("dlg.warn.title");
                        DialogResult r = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                        if (r != DialogResult.OK) {
                            return;
                        }
                    }
                    ModelManager.Instance.removeFromModel(we);
                    pnode.Remove();
                    this.App.markDirty();
                }
            }
        }
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e) {
            TreeNode tn = e.Item as TreeNode;
            if ((e.Button == MouseButtons.Left) && (tn != null) && (tn.Parent != null)) {
                this.weTreeView1.DoDragDrop(tn, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
            }
        }
        /// <summary>
        /// control the cursor style, DragDropEffects.Link means move the src as a child of the target node. 
        /// DragDropEffects.Move means order the src node at the target node position. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_DragOver(object sender, DragEventArgs e) {
            Point p = new Point(e.X, e.Y);
            Point np = weTreeView1.PointToClient(p);
            TreeNode dragNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
            TreeNode tgtNode = weTreeView1.GetNodeAt(np);

            this._weTreePreTgtNode = this._weTreeCurrentTgtNode;
            this._weTreeCurrentTgtNode = tgtNode;

            if(tgtNode == null || dragNode == null || dragNode.Equals(this.bigmodel.SRoot.WERoot) || dragNode.Equals(tgtNode)){            
                e.Effect = DragDropEffects.None;
                return;
            }
            //Console.WriteLine("target node = "+tgtNode.Text+", drag node ="+dragNode.Text);            
            // filter the raw data node
            if (dragNode.Tag is WebElementGroup) {
                WebElementGroup weg = dragNode.Tag as WebElementGroup;
                if(weg.Equals(this.bigmodel.SRoot.RawElemsGrp)){
                    e.Effect = DragDropEffects.None ;
                    return;
                }
            }
            if (tgtNode.Tag is WebElementGroup) {
                WebElementGroup tweg = tgtNode.Tag as WebElementGroup;
                if (tweg.Equals(this.bigmodel.SRoot.RawElemsGrp)) {
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }
            if (tgtNode == null || tgtNode == dragNode || dragNode.Parent == null) {
                e.Effect = DragDropEffects.None;
            }else if (tgtNode.Tag is WebElementGroup) {                
                // control the effect if WebElementGroup
                if (dragNode.Tag is WebElement) {
                    // link means that move the source as a child of the target node
                    e.Effect = DragDropEffects.Link;
                } else {
                    WebElementGroup tweg = tgtNode.Tag as WebElementGroup ;
                    WebElementGroup sweg = dragNode.Tag as WebElementGroup ;
                    // filter raw data node, this node can not be moved, or be moved into
                    if(sweg.Equals(this.bigmodel.SRoot.RawElemsGrp)){
                        e.Effect = DragDropEffects.None ;
                    }else{                        
                        // src node and target node have same owner
                        if(sweg.Collection.Equals(tweg.Collection)){
                            e.Effect = DragDropEffects.Move ;
                        }else{
                            e.Effect = DragDropEffects.Link ;
                        }
                    }                   
                }
                // control how to do node auto-expand. 
                if (this._weTreeCurrentTgtNode != _weTreePreTgtNode) {
                    if (tgtNode.Nodes.Count > 0 && tgtNode.IsExpanded == false) {
                        this._weTVDnDStartTime = DateTime.Now;                        
                    }
                } else {
                    if (tgtNode.Nodes.Count > 0 && tgtNode.IsExpanded == false && this._weTVDnDStartTime != DateTime.Now) {
                        TimeSpan ts = DateTime.Now - this._weTVDnDStartTime;
                        if (ts.TotalMilliseconds > 1000) {
                            tgtNode.Expand();
                            this._weTVDnDStartTime = DateTime.MinValue;
                        }
                    }
                }
            } else if (tgtNode.Tag is WebElement) {
                WebElement twe = tgtNode.Tag as WebElement;
                
                e.Effect = DragDropEffects.None;
                if (dragNode.Tag is WebElement) {
                    WebElement swe = dragNode.Tag as WebElement ;
                    if (swe.Collection.Equals(twe.Collection)) {
                        e.Effect = DragDropEffects.Move;
                    //} else {
                    //    e.Effect = DragDropEffects.Move;
                    }
                }
            } else if (tgtNode.Tag is ScriptRoot) {
                e.Effect = DragDropEffects.Link;
            }
        }
        /// <summary>
        /// handle WebElement or group DnD operation. 
        /// 1. If there is no duplicated node under the target node, just move the drag node under the target node. 
        /// 2. If there is a duplicated node under the target node, 
        ///    2a). if duplicate node and drag node has same type, just remove the duplicated node, and move the drag
        ///         node at that position. 
        ///    2b). if there are diffent type, just ignore.     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_DragDrop(object sender, DragEventArgs e) {
            Point p = new Point(e.X, e.Y);
            Point np = weTreeView1.PointToClient(p);
            TreeNode tgtNode = weTreeView1.GetNodeAt(np);
            TreeNode dragNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
            if (tgtNode == null || tgtNode.Tag == null || tgtNode.Tag.Equals(this.bigmodel.SRoot.RawElemsGrp)) {
                return;
            }
            
            if (e.Effect == DragDropEffects.Link) {
                // target node is the container of drag node 
                if (tgtNode != dragNode.Parent && !tgtNode.Equals(dragNode)) {
                    // check whether the src node is valid in the target node
                    if (isValidTreeDnD(tgtNode, dragNode)) {
                        doWETreeDnD(tgtNode,dragNode);
                        this.App.markDirty();
                    }                    
                }
            } else if (e.Effect == DragDropEffects.Move) {
                // target node is the pair of the drag node 
                if (!tgtNode.Equals(dragNode)) {
                    dragNode.Remove();
                    moveToNode(tgtNode, dragNode);
                    // update model 
                    if (tgtNode.Tag is WebElementGroup && dragNode.Tag is WebElementGroup) {
                        WebElementGroup tweg = (WebElementGroup)tgtNode.Tag;
                        WebElementGroup sweg = (WebElementGroup)dragNode.Tag;
                        sweg.Collection.Remove(sweg);
                        int index = tweg.Collection.IndexOf(tweg);
                        tweg.Collection.Insert(index, sweg);
                    } else if (tgtNode.Tag is WebElement && dragNode.Tag is WebElement) {
                        WebElement twe = (WebElement)tgtNode.Tag;
                        WebElement swe = (WebElement)dragNode.Tag;
                        swe.Collection.Remove(swe);
                        int index = twe.Collection.IndexOf(twe);
                        twe.Collection.Insert(index, swe);
                    }
                    this.App.markDirty();
                }
            }
            
            // update the target node's background 
            if (this._weTreeCurrentTgtNode != null) {
                this._weTreeCurrentTgtNode.BackColor = SystemColors.Window;
                this._weTreeCurrentTgtNode.ForeColor = SystemColors.WindowText;
                this._weTreeCurrentTgtNode = null;
            }
            if (this._weTreePreTgtNode != null) {
                this._weTreePreTgtNode.BackColor = SystemColors.Window;
                this._weTreePreTgtNode.ForeColor = SystemColors.WindowText;
            }

            this.weTreeView1.SelectedNode = dragNode;
        }
        /// <summary>
        /// handle the move a node under target
        /// </summary>
        /// <param name="tgtNode"></param>
        /// <param name="dragNode"></param>
        private void doWETreeDnD(TreeNode tgtNode, TreeNode dragNode) {            
            // remove drag node from existed structure. 
            doRemoveTreeNode(dragNode);
            // move drag node under target node 
            moveUnderNode(tgtNode, dragNode);            
        }    
        /// <summary>
        /// updated the newly added node script model with parent node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="addedNode"></param>
        private void updateAddedNodeModel(TreeNode parentNode, TreeNode addedNode) {
            if (parentNode == null || parentNode.Tag == null || addedNode == null || addedNode.Tag == null) {
                return; 
            }

            if (addedNode.Tag is WebElement) {
                WebElement we = addedNode.Tag as WebElement;
                if (parentNode.Tag is WebElementGroup) {
                    WebElementGroup weg = parentNode.Tag as WebElementGroup;
                    weg.Elems.AddUnique(we);
                } 
            } else if (addedNode.Tag is WebElementGroup) {
                WebElementGroup sweg = addedNode.Tag as WebElementGroup;
                if (parentNode.Tag is WebElementGroup) {
                    WebElementGroup weg = parentNode.Tag as WebElementGroup;
                    weg.SubGroups.AddUnique(sweg);
                } 
            }
        }        
        /// <summary>
        /// expend the WebElement tree root 
        /// </summary>
        internal void expendWERootTree() {
            if (weTreeView1.Nodes.Count > 0) {
                this.weTreeView1.Nodes[0].Expand();
            }
        }
        private void updateWETreeNodeText(BaseElement be) {
            TreeNode tnode = findNode(this.weTreeView1.Nodes[0], be);
            if (tnode != null) {
                tnode.Text = be.Name;
                tnode.ToolTipText = be.Description;
            }
        }
        /// <summary>
        /// find the proper node with tag object, or null if not find 
        /// </summary>
        /// <param name="tnParent"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private TreeNode findNode(TreeNode tnParent, BaseElement tag) {
            if (tnParent == null) return null;
            if (tnParent.Tag != null && tnParent.Tag.Equals(tag)) {
                return tnParent;
            }

            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes) {
                tnRet = findNode(tn, tag);
                if (tnRet != null) {
                    break;
                }
            }
            return tnRet;
        }
        /// <summary>
        /// clean WETree area 
        /// </summary>
        private void cleanWETreeView() {
            this.textBox1.Text = Constants.BLANK_TEXT;
            this.weTreeView1.Nodes.Clear();
        }
        #endregion WebElement tree area
        #region parameter tree area
        /// <summary>
        /// reset the Parameter tree with ParamGroup 
        /// </summary>
        public void resetParamTree(Process proc) {
            if (this.paramTV.Tag == proc) {
                return;
            }
            // clean existed nodes 
            if (this.paramTV.Nodes.Count > 0) {
                this.paramTV.Nodes.Clear();                
            }
            if (proc == null) {
                paramTV.Tag = null;
                this.btn_paraSearch.Enabled = false;
                this.lable_proc.Text = "";
                return;
            } else {
                this.lable_proc.Text = proc.Name;
            }
            this.paramTV.Tag = proc;
            this.paramTV.BeginUpdate();
            // build new parameter tree 
            // build public parameter
            TreeNode gn = createParamGroupNode(proc.ParamPublic);
            this.paramTV.Nodes.Add(gn);
            // build private parameter 
            gn = createParamGroupNode(proc.ParamPrivate);
            this.paramTV.Nodes.Add(gn);
            
            this.paramTV.EndUpdate();
            this.paramTV.ExpandAll();
            // update search button 
            if(proc.ParamPublic!=null || proc.ParamPrivate!=null){
                // disable Tree area
                this.btn_paraSearch.Enabled = true;
            }else{                
                this.btn_paraSearch.Enabled = false;
            }
        }

        private TreeNode createParamGroupNode(ParamGroup grp) {
            TreeNode gnode = new TreeNode();
            //create node view             
            gnode.Text = grp.Name;
            gnode.ToolTipText = grp.Description;
            gnode.Tag = grp;
            gnode.ImageIndex = TREE_ICON_PARAMGRP_VALID;
            gnode.SelectedImageIndex = TREE_ICON_PARAMGRP_VALID;

            foreach (ParamGroup pg in grp.SubGroups) {
                TreeNode gn = createParamGroupNode(pg);
                gnode.Nodes.Add(gn);
            }

            foreach (Parameter param in grp.Params) {
                TreeNode pn = createParamNode(param);
                gnode.Nodes.Add(pn);
            }
            
            return gnode;
        }

        private TreeNode createParamNode(Parameter param) {
            TreeNode pn = new TreeNode();
            pn.Text = param.Name;
            pn.ToolTipText = param.Description;
            pn.Tag = param;
            pn.ImageIndex = TREE_ICON_PARAM_VALID;
            pn.SelectedImageIndex = TREE_ICON_PARAM_VALID;

            return pn;
        }

        private void paramTV_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (e.Label != null && e.Label != "") {
                bool valid = false;

                TreeNode node = this.paramTV.SelectedNode;
                if (node.Tag is ParamGroup) {
                    ParamGroup grp = node.Tag as ParamGroup;
                    ParamGroup tgrp = ModelFactory.createParamGroup();
                    tgrp.Name = e.Label;
                    valid = ModelManager.Instance.isUniqueToBeElement(tgrp, grp.Collection);
                } else if (node.Tag is Parameter) {
                    Parameter param = node.Tag as Parameter;
                    Parameter tparam = ModelFactory.createParameter();
                    tparam.Name = e.Label;
                    valid = ModelManager.Instance.isUniqueToBeElement(tparam, param.Collection);
                }
                
                if (valid) {
                    BaseElement be = node.Tag as BaseElement;
                    be.Name = e.Label;
                    this.updatePropViewIfNeed(be);
                    this.App.markDirty();
                }
            }
        }

        private void paramTV_MouseDown(object sender, MouseEventArgs e) {            
            TreeNode node = paramTV.GetNodeAt(e.X, e.Y);
            if (node == null) {
                this.paramTV.SelectedNode = null;
                // set the SelectedObj as the ParamRoot group 
                this.SelectedObj = this.paramTV.Tag as ParamGroup;
            } else {
                if (this.paramTV.SelectedNode != null && node == this.paramTV.SelectedNode) {
                    this.paramTV.LabelEdit = true;
                    this.paramTV.SelectedNode = node;
                    this.paramTV.SelectedNode.BeginEdit();
                }

                if (node != paramTV.SelectedNode) {
                    this.paramTV.LabelEdit = false;
                    paramTV.SelectedNode = node;
                }

                //update selected object 
                this.SelectedObj = node.Tag as BaseElement;
            }
        }
        /// <summary>
        /// used to control the context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paramTV_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                this.tsmi_AddParamGrp.Visible = false;
                this.tsmi_AddParam.Visible = false;
                this.tsmi_paramSep.Visible = false;
                this.tsmi_paramSep1.Visible = false;
                this.tsmi_ParamTVDelete.Visible = false;
                if (paramTV.Tag == null) {
                    return;
                }

                TreeNode node = paramTV.GetNodeAt(e.X, e.Y);

                if (node == null) {
                    //this.tsmi_AddParamGrp.Visible = true;
                    //this.tsmi_AddParam.Visible = true;
                    if (cms_paramTree.Visible) {
                        cms_paramTree.Visible = false;
                    }
                } else if (node.Tag is ParamGroup) {
                    if (!isGlobal2ndGroup(node.Tag as ParamGroup)) {
                        this.tsmi_AddParamGrp.Visible = true;
                        this.tsmi_paramSep1.Visible = true;
                    }
                    this.tsmi_AddParam.Visible = true;
                    this.tsmi_paramSep.Visible = true;
                    this.tsmi_ParamTVDelete.Visible = true;
                    if (cms_paramTree.Visible == false) {
                        cms_paramTree.Visible = true;
                    }
                } else if (node.Tag is Parameter) {
                    this.tsmi_ParamTVDelete.Visible = true;
                    if (cms_paramTree.Visible == false) {
                        cms_paramTree.Visible = true;
                    }
                }
            } 
        }
        /// <summary>
        /// Whether the paramGroup is the 2nd level of the global public parameters 
        /// </summary>
        /// <param name="paramGroup"></param>
        /// <returns></returns>
        private bool isGlobal2ndGroup(ParamGroup paramGroup) {
            if (paramGroup != null && paramGroup.Collection!=null && paramGroup.Collection.Owner!=null) {
                ParamGroup pgrp = paramGroup.Collection.Owner as ParamGroup;
                if (pgrp == this.bigmodel.SRoot.ProcRoot.ParamPublic) {
                    return true;
                }
            }
            return false;
        }

        private void elemTabCtrl_SelectedIndexChanged(object sender, EventArgs e) {
            if (elemTabCtrl.SelectedTab == paramPage) {
                if (SelectedObj is Process) {
                    Process proc = SelectedObj as Process;
                    if (paramTV.Tag != proc) {
                        handleParamTreeWhenSelectedObjChanged(proc);
                    }
                }
            }
        }

        private void handleParamTreeWhenSelectedObjChanged(BaseElement selectedObj) {
            if (selectedObj is Operation || selectedObj is Process || selectedObj is OpCondition) {
                if (this.paramTV.SelectedNode != null) {
                    if (this.paramTV.SelectedNode.IsEditing) {
                        this.paramTV.SelectedNode.EndEdit(true);
                    }
                    if (this.paramTV.SelectedNode.IsSelected) {
                        this.paramTV.SelectedNode = null;
                    }
                }
                Process proc = null;
                if (selectedObj is Process) {
                    proc = selectedObj as Process;
                } else if (selectedObj is Operation) {
                    proc = ModelManager.Instance.getOwnerProc(selectedObj as Operation);
                } else if (selectedObj is OpCondition) {
                    Operation op = ModelManager.Instance.getOwnerOp(selectedObj as OpCondition);
                    proc = ModelManager.Instance.getOwnerProc(op);
                }

                if (elemTabCtrl.SelectedTab == paramPage) {
                    resetParamTree(proc);
                } else {
                    resetParamTree(null);
                }
            }
        }
        private void tsmi_AddParamGrp_Click(object sender, EventArgs e) {
            if (paramTV.Tag == null) {
                return;
            }
            TreeNode pnode = this.paramTV.SelectedNode;
            ParamGroup grp = null;
            if (pnode == null) {
                grp = this.paramTV.Tag as ParamGroup;
            } else {
                grp = pnode.Tag as ParamGroup;
            }
            
            if (grp != null) {
                ParamGroup sgrp = createUniqueParamGrp(grp);
                grp.SubGroups.AddUnique(sgrp);
                TreeNode node = this.createParamGroupNode(sgrp);
                if (pnode == null) {
                    this.paramTV.Nodes.Add(node);
                } else {
                    pnode.Nodes.Add(node);
                }
                this.paramTV.SelectedNode = node;
                this.SelectedObj = sgrp;

                this.App.markDirty();
            }
        }
        /// <summary>
        /// create a unique ParamGroup in the parent group list
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private ParamGroup createUniqueParamGrp(ParamGroup parent) {
            ParamGroup grp = ModelFactory.createParamGroup();
            grp.Name = ModelManager.Instance.getUniqueElementName(parent.SubGroups, grp);
            return grp;
        }

        private void tsmi_AddParam_Click(object sender, EventArgs e) {
            if (paramTV.Tag == null) {
                return;
            }
            TreeNode pnode = this.paramTV.SelectedNode;
            ParamGroup pgrp = null ;
            if(pnode == null){
                pgrp = this.paramTV.Tag as ParamGroup ;
            }else{
                if(pnode.Tag is ParamGroup){
                    pgrp = pnode.Tag as ParamGroup ;
                }
            }

            if (pgrp != null) {
                Parameter param = createUniqueParam(pgrp);
                pgrp.Params.AddUnique(param);
                TreeNode node = this.createParamNode(param);
                if (pnode == null) {
                    this.paramTV.Nodes.Add(node);
                } else {
                    pnode.Nodes.Add(node);
                }
                this.paramTV.SelectedNode = node;
                this.SelectedObj = param;

                this.App.markDirty();
            }
        }
        /// <summary>
        /// create a name unique parameter under the ParamGroup
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private Parameter createUniqueParam(ParamGroup parent) {
            Parameter param = ModelFactory.createParameter();
            param.Name = ModelManager.Instance.getUniqueElementName(parent.Params, param);

            return param;
        }

        private void tsmi_ParamTVDelete_Click(object sender, EventArgs e) {
            if (paramTV.Tag == null) {
                return;
            }
            TreeNode pnode = this.paramTV.SelectedNode;
            if (pnode != null) {
                ParamGroup parent = null;
                if (pnode.Tag is ParamGroup) {
                    ParamGroup grp = pnode.Tag as ParamGroup;
                    // check if there is reference for the contained Parameters                    
                    int count = ModelManager.Instance.getParamGrpRefTimes(grp);
                    if (count > 0) {
                        //string msg = UILangUtil.getMsg("ide.feditor.ptree.del.msg1", count);
                        //string title = UILangUtil.getMsg("dlg.warn.title");
                        //DialogResult dr = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                        //if (dr != DialogResult.OK) {
                        //    return;
                        //}
                    }                    
                    
                    parent = grp.Collection.Owner as ParamGroup;
                    parent.SubGroups.Remove(grp);
                    this.App.markDirty();
                } else if (pnode.Tag is Parameter) {
                    Parameter param = pnode.Tag as Parameter;

                    // check if there is reference for Parameter
                    int count = ModelManager.Instance.getParamRefTimes(param);
                    if (count > 0) {
                        //string msg = UILangUtil.getMsg("ide.feditor.ptree.del.msg2", count);
                        //string title = UILangUtil.getMsg("dlg.warn.title");
                        //DialogResult dr = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                        //if (dr != DialogResult.OK) {
                        //    return;
                        //}
                    }                    
                    
                    parent = param.Collection.Owner as ParamGroup;
                    parent.Params.Remove(param);
                    this.App.markDirty();
                }
                if (parent != null) {
                    pnode.Remove();
                } else {
                    //TODO log ...
                }
            }
        }

        private void paramTV_ItemDrag(object sender, ItemDragEventArgs e) {
            TreeNode tn = e.Item as TreeNode;
            if (e.Button == MouseButtons.Left && tn != null) {                
                this.paramTV.DoDragDrop(tn, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
            }
        }
        /// <summary>
        /// control the cursor style, DragDropEffects.Link means move the src as a child of the target node. 
        /// DragDropEffects.Move means order the src node at the target node position. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paramTV_DragOver(object sender, DragEventArgs e) {
            Point p = new Point(e.X, e.Y);
            Point np = paramTV.PointToClient(p);
            TreeNode dragNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
            TreeNode tgtNode = paramTV.GetNodeAt(np);

            this._paramTreePreTgtNode = this._paramTreeCurrentTgtNode;
            this._paramTreeCurrentTgtNode = tgtNode;

            if (tgtNode == null || dragNode == null || dragNode.Equals(tgtNode)) {
                e.Effect = DragDropEffects.None;
                return;
            }
            //Console.WriteLine("target node = "+tgtNode.Text+", drag node ="+dragNode.Text);            
            if (tgtNode == null || tgtNode == dragNode) {
                e.Effect = DragDropEffects.None;
            } else if (tgtNode.Tag is ParamGroup) {
                // if the drag node is Parameter 
                if (dragNode.Tag is Parameter) {
                    // link means that move the source as a child of the target node
                    e.Effect = DragDropEffects.Link;
                } else {
                    ParamGroup tg = tgtNode.Tag as ParamGroup;
                    ParamGroup sg = dragNode.Tag as ParamGroup;
                    // src node and target node have same owner, then move 
                    if (sg.Collection.Equals(tg.Collection)) {
                        e.Effect = DragDropEffects.Move;
                    } else {
                        e.Effect = DragDropEffects.Link;
                    }
                }

                // control how to do node auto-expand. 
                if (this._paramTreeCurrentTgtNode != _paramTreePreTgtNode) {
                    if (tgtNode.Nodes.Count > 0 && tgtNode.IsExpanded == false) {
                        this._paramTVDnDStartTime = DateTime.Now;
                    }
                } else {
                    if (tgtNode.Nodes.Count > 0 && tgtNode.IsExpanded == false && this._paramTVDnDStartTime != DateTime.Now) {
                        TimeSpan ts = DateTime.Now - this._paramTVDnDStartTime;
                        if (ts.TotalMilliseconds > 1000) {
                            tgtNode.Expand();
                            this._paramTVDnDStartTime = DateTime.MinValue;
                        }
                    }
                }
            } else if (tgtNode.Tag is Parameter) {
                Parameter param = tgtNode.Tag as Parameter;

                e.Effect = DragDropEffects.None;
                if (dragNode.Tag is Parameter) {
                    Parameter src = dragNode.Tag as Parameter;
                    if (src.Collection.Equals(param.Collection)) {
                        e.Effect = DragDropEffects.Move;
                    }
                }
            }
        }
        /// <summary>
        /// handle Parameter or Paramter Group DnD operation. 
        /// 1. If there is no duplicated node under the target node, just move the drag node under the target node. 
        /// 2. If there is a duplicated node under the target node, 
        ///    2a). if duplicate node and drag node has same type, just remove the duplicated node, and move the drag
        ///         node at that position. 
        ///    2b). if there are diffent type, just ignore.     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paramTV_DragDrop(object sender, DragEventArgs e) {
            Point p = new Point(e.X, e.Y);
            Point np = this.paramTV.PointToClient(p);
            TreeNode tgtNode = this.paramTV.GetNodeAt(np);
            TreeNode dragNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
            if (tgtNode == null || tgtNode.Tag == null) {
                return;
            }
            // do move operation 
            if (e.Effect == DragDropEffects.Link) {
                // target node is the container of drag node 
                if (tgtNode != dragNode.Parent && !tgtNode.Equals(dragNode)) {
                    // check whether the src node is valid in the target node
                    if (isValidTreeDnD(tgtNode, dragNode)) {
                        doParamTreeDnD(tgtNode, dragNode);
                        this.App.markDirty();
                    }
                }
            } else if (e.Effect == DragDropEffects.Move) {
                // target node is the pair of the drag node 
                if (!tgtNode.Equals(dragNode)) {
                    dragNode.Remove();
                    moveToNode(tgtNode, dragNode);
                    // update model 
                    if (tgtNode.Tag is ParamGroup && dragNode.Tag is ParamGroup) {
                        ParamGroup tpg = tgtNode.Tag as ParamGroup;
                        ParamGroup spg = dragNode.Tag as ParamGroup;
                        spg.Collection.Remove(spg);
                        int index = tpg.Collection.IndexOf(tpg);
                        tpg.Collection.Insert(index, spg);
                    } else if (tgtNode.Tag is Parameter && dragNode.Tag is Parameter) {
                        Parameter tp = tgtNode.Tag as Parameter;
                        Parameter sp = dragNode.Tag as Parameter;
                        sp.Collection.Remove(sp);
                        int index = tp.Collection.IndexOf(tp);
                        tp.Collection.Insert(index, sp);
                    }
                    this.App.markDirty();
                }
            }

            // update the target node's background 
            if (this._paramTreeCurrentTgtNode != null) {
                this._paramTreeCurrentTgtNode.BackColor = SystemColors.Window;
                this._paramTreeCurrentTgtNode.ForeColor = SystemColors.WindowText;
                this._paramTreeCurrentTgtNode = null;
            }
            if (this._paramTreePreTgtNode != null) {
                this._paramTreePreTgtNode.BackColor = SystemColors.Window;
                this._paramTreePreTgtNode.ForeColor = SystemColors.WindowText;
            }

            this.paramTV.SelectedNode = dragNode;
        }

        private void doParamTreeDnD(TreeNode tgtNode, TreeNode dragNode) {
            if (isValidParamDnD(tgtNode, dragNode)) {
                // remove drag node from existed structure. 
                doRemoveTreeNode(dragNode);
                // move drag node under target node 
                moveUnderNode(tgtNode, dragNode);
            }
        }
        /// <summary>
        /// Make sure the RootProc's parameters are at least two level. 
        /// </summary>
        /// <param name="tgtNode"></param>
        /// <param name="dragNode"></param>
        /// <returns></returns>
        private bool isValidParamDnD(TreeNode tgtNode, TreeNode dragNode) {
            Process procRoot = null;
            if (this.bigmodel != null) {
                procRoot = this.bigmodel.SRoot.ProcRoot;
            }
            
            if (procRoot!= null && tgtNode.Tag is ParamGroup) {
                ParamGroup tgtGrp = tgtNode.Tag as ParamGroup;
                if (tgtGrp == procRoot.ParamPublic && UIUtils.getNodeLevel(dragNode)>1 ) {
                    return false;
                }
            }
            
            return true;
        }
        /// <summary>
        /// clean Parameter Tree area 
        /// </summary>
        private void cleanParamTreeView() {
            this.textBox2.Text = Constants.BLANK_TEXT;
            this.paramTV.Nodes.Clear();
        }

        private void updateParamTreeNodeText(BaseElement be) {
            TreeNode tnode = findParamNode(this.paramTV.Nodes, be);
            if (tnode != null) {
                tnode.Text = be.Name;
                tnode.ToolTipText = be.Description;
            }
        }
        private TreeNode findParamNode(TreeNodeCollection nodes, BaseElement be) {
            if (nodes == null || be == null) {
                return null;
            }
            foreach (TreeNode pnode in nodes) {
                if (pnode.Tag != null && pnode.Tag.Equals(be)) {
                    return pnode;
                }
                if (pnode.Nodes != null && pnode.Nodes.Count > 0) {
                    TreeNode tnode = findParamNode(pnode.Nodes, be);
                    if (tnode != null) {
                        return tnode;
                    }
                }
            }
            return null;
        }
        #endregion parameter tree area 
        #region flow properties view
        /// <summary>
        /// clean flow prop views UI info  
        /// </summary>
        private void cleanFlowPropViews() {
            //TODO
        }
        /// <summary>
        /// if the selected object is be, so update the property views info on demond. 
        /// </summary>
        /// <param name="be"></param>
        internal void updatePropViewIfNeed(BaseElement be) {
            if (this.SelectedObj != null && this.SelectedObj == be) {
                flowPVManager.updatePropViews(be);
            }
        }
        /// <summary>
        /// update all active propties views info into model. current is 
        /// general page, condition page, and rule page. 
        /// </summary>
        internal void updateExistedPropData() {
            this.flowPVManager.updateExistedPropData();
        }
        #endregion flow properties view
        #region script validation 
        /// <summary>
        /// If there data is valided with different MsgType, just update the UI Element icon to reflect the 
        /// validatoin type. 
        /// 
        /// It will covered WebElement/WebElementGroup -> on WebElement Tree view, Process/Operation/OpCondition -> on diagram view.        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msgType"></param>
        internal void markValidationStatus(object data, MsgType msgType) {
            // mark webelement tree view if need 
            TreeNode node = null;
            if (data is WebElementGroup) {
                node = this.getNodeByBE(this.weTreeView1.Nodes[0], data as WebElementGroup);                
            } else if (data is WebElement) {
                node = this.getNodeByBE(this.weTreeView1.Nodes[0], data as WebElement);
            }
            markWETreeValidation(node,msgType);
            // mark diagram node if need 
            diagramManager.markNodeViewValidation(data,msgType);            
        }
        /// <summary>
        /// If a WebElement/WebElementGroup is marked, all its parent node will be marked, the priority is Error > WARNING > INFO
        /// so, if there is in high icon state, low icon update request will be ignored. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="type"></param>
        private void markWETreeValidation(TreeNode node,MsgType type) {
            if (node.Tag is WebElementGroup) {
                if (type == MsgType.VALID) {
                   node.ImageIndex = TREE_ICON_WEG_VALID;
                } else if (type == MsgType.WARNING) {
                    node.ImageIndex = TREE_ICON_WEG_WARNING;
                } else if (type == MsgType.ERROR) {
                    node.ImageIndex = TREE_ICON_WEG_ERROR;
                }
            } else if (node.Tag is WebElement) {
                if (type == MsgType.VALID) {
                    node.ImageIndex = TREE_ICON_WE_VALID;
                } else if (type == MsgType.WARNING) {
                    node.ImageIndex = TREE_ICON_WE_WARNING;
                } else if (type == MsgType.ERROR) {
                    node.ImageIndex = TREE_ICON_WE_ERROR;
                }
            }

            markParentTreeNode(node);
        }

        private void markParentTreeNode(TreeNode node) {
            if (node.Parent == null) {
                return;
            }
            TreeNode pnode = node.Parent;
            int icon = TREE_ICON_WEG_VALID;
            foreach (TreeNode tnode in pnode.Nodes) {
                if (tnode.Tag is WebElement) {
                    if (tnode.ImageIndex == TREE_ICON_WE_ERROR) {
                        icon = TREE_ICON_WEG_ERROR;
                        break;
                    }
                    if (tnode.ImageIndex == TREE_ICON_WE_WARNING) {
                        icon = TREE_ICON_WEG_WARNING;
                    }
                } else if (tnode.Tag is WebElementGroup) {
                    if (tnode.ImageIndex == TREE_ICON_WEG_ERROR) {
                        icon = TREE_ICON_WEG_ERROR;
                        break;
                    }
                    if (tnode.ImageIndex == TREE_ICON_WEG_WARNING) {
                        icon = TREE_ICON_WEG_WARNING;
                    }
                }
            }
            pnode.ImageIndex = icon;
            // check parent node 
            markParentTreeNode(pnode);
        }
        /// <summary>
        /// validateScriptAll and changed the error log view into display. 
        /// return true if the script is valid or false if there are some invalid msg.
        /// </summary>
        /// <returns></returns>
        public bool isValidScript(){
            validateScriptAll();
            if (this.flowPVManager.ErrLogTable.Count > 0) {                
                foreach (object key in this.flowPVManager.ErrLogTable) {
                    ValidationMsg msg = this.flowPVManager.ErrLogTable.Get(key) as ValidationMsg;
                    if (msg.Type == MsgType.ERROR) {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// full validate script info, 
        /// 1. build error msg table if errors or warning, validate ScriptRoot, WebElement/WebElementGroup, 
        ///    Process, Operation, OpCondition, Parameters. 
        /// 2. update error properties log view
        /// 3. update UI element status, both WebElement tree, OpCondition and Operation Node
        /// </summary>
        public void validateScriptAll() {
            // clean error table and UI info 
            this.flowPVManager.cleanLogView();
            ScriptRoot sroot = this.bigmodel.SRoot;
           
            //1. build error msg table 
            buildupErrorMsgTable(sroot);  
            // 2. update Error Log properties view 
            this.flowPVManager.updateErrorLogView();
            // 3. udpate WebElement tree and OpCondition and Operation Node
            this.updateValidationUIElements(this.flowPVManager.ErrLogTable);
            // 4. show log view 
            this.propTabCtrl.SelectedIndex = 3;
        }
        /// <summary>
        /// mark the all the elements with Error/Warning icon if invalid or remove the 
        /// invalid icon if it is not include in the table but marked invalid before. 
        /// Just handle WebElement/WebElementGroup in the WebElement tree, Process/Operation/OpCondition
        /// in the diagram, and Parameters in the Parameters tree. 
        /// </summary>
        /// <param name="table"> key is the model element, and value is ValidationMsg object </param>
        private void updateValidationUIElements(HashtableEx table) {
            //1. mark invalid/valid WebElement/WebElementGroup in tree view
            updateValidationWETree(this.weTreeView1.Nodes[0],table);
            //2. mark Process/Operation/OpCondition with valid/invalid status 
            updateValidationDiagram(this.workareaTabCtrl, this.bigmodel.VRoot, table);
            //3. mark Parameters with valid/invalid status 

        }
        /// <summary>
        /// update all currently opened diagram elements validation status (Node, and OpCondition), notes that 
        /// just update currently opened diagram's elements. 
        /// </summary>
        /// <param name="diagramTabCtrl"></param>
        /// <param name="vroot"></param>
        /// <param name="table"></param>
        private void updateValidationDiagram(TabControl diagramTabCtrl, ViewRoot vroot, HashtableEx table) {
            if (diagramTabCtrl == null || table == null || vroot == null) {
                return;
            }
            // update all nodes            
            foreach (TabPage page in diagramTabCtrl.TabPages) {
                Canvas canvas = page.Controls[0] as Canvas;
                // because here are added 3 useControl to help to adjust the canvas scroll bar and layout.
                foreach (UserControl uc in canvas.Controls) {
                    if (uc is NodeView) {
                        NodeView nv = uc as NodeView;
                        MsgType type = MsgType.VALID;
                        ValidationMsg msg = table.Get(nv.Node.RefOp) as ValidationMsg;
                        if (msg != null) {
                            type = msg.Type;
                        }
                        nv.ValidType = type;
                    }
                }
            }
            // update all connections 
            foreach (TabPage page in diagramTabCtrl.TabPages) {
                Canvas canvas = page.Controls[0] as Canvas;
                HashtableEx ctable = canvas.ConnValidationTable;
                ctable.clear();

                foreach (Connection con in canvas.Diagram.Connections) {
                    MsgType type = MsgType.VALID;
                    ValidationMsg msg = table.Get(con.RefCon) as ValidationMsg;
                    if (msg != null) {
                        type = msg.Type;
                    }
                    if (type != MsgType.VALID) {
                        ctable.Add(con, type);
                    }
                }
            }
            diagramTabCtrl.SelectedTab.Refresh();
        }
        /// <summary>
        /// mark WebElement/WebElementGroup with valida/invalid status
        /// </summary>
        /// <param name="pnode">parent node </param>
        /// <param name="table"></param>
        private MsgType updateValidationWETree(TreeNode pnode, HashtableEx table) {
            MsgType type = MsgType.VALID ;
            foreach (TreeNode node in pnode.Nodes) {
                MsgType ntype = MsgType.VALID;
                if (node.Nodes.Count > 0) {
                    ntype = updateValidationWETree(node, table);
                } else {
                    ValidationMsg msg = table.Get(node.Tag) as ValidationMsg;
                    int imgIndex = getWETreeImgIndex(node.Tag, MsgType.VALID);
                    if (msg != null) {
                        imgIndex = getWETreeImgIndex(node.Tag, msg.Type);
                    }
                    if (imgIndex != -1) {
                        node.ImageIndex = imgIndex;
                        node.SelectedImageIndex = imgIndex;
                    }
                    if (msg != null) {
                        ntype = msg.Type;
                    }
                }
                // just mark update parent node status 
                if (ntype == MsgType.ERROR) {
                    type = MsgType.ERROR;
                } else if (ntype == MsgType.WARNING && type != MsgType.ERROR) {
                    type = MsgType.WARNING;
                }               
            }
            // update parent node 
            int iindex = getWETreeImgIndex(pnode.Tag, type);
            if (iindex != -1) {
                pnode.ImageIndex = iindex;
                pnode.SelectedImageIndex = iindex;
            }

            return type;
        }
        /// <summary>
        /// return -1 if errors 
        /// </summary>
        /// <param name="be"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int getWETreeImgIndex(Object be, MsgType type) {
            if (type == MsgType.ERROR) {
                if (be is ScriptRoot) {
                    return TREE_ICON_ROOT_ERROR;
                } else if (be is WebElementGroup) {
                    return TREE_ICON_WEG_ERROR;
                } else if (be is WebElement) {
                    return TREE_ICON_WE_ERROR;
                }
            } else if (type == MsgType.VALID) {
                if (be is ScriptRoot) {
                    return TREE_ICON_ROOT_VALID;
                } else if (be is WebElementGroup) {
                    return TREE_ICON_WEG_VALID;
                } else if (be is WebElement) {
                    return TREE_ICON_WE_VALID;
                }
            } else if (type == MsgType.WARNING) {
                if (be is ScriptRoot) {
                    return TREE_ICON_ROOT_WARNING;
                } else if (be is WebElementGroup) {
                    return TREE_ICON_WEG_WARNING;
                } else if (be is WebElement) {
                    return TREE_ICON_WE_WARNING;
                }
            }
            return -1;
        }
        /// <summary>
        /// clean and rebuild error msg table if errors or warning, validate ScriptRoot, WebElement/WebElementGroup, 
        /// Process, Operation, OpCondition, Parameters.
        /// Note: in this method, it will check each element, and just record the INVALID element into table. 
        /// Each element's invalidation just judged by itself. it means that e.g the group is valid while there is an 
        /// invalid children element. 
        /// </summary>
        /// <param name="sroot"></param>
        /// <returns></returns>
        private void buildupErrorMsgTable(ScriptRoot sroot) {
            HashtableEx table = this.flowPVManager.ErrLogTable;
            table.clear();

            ValidationMsg msg = ModelManager.Instance.getValidMsg(sroot);
            // a. check script root 
            if (msg.Type != MsgType.VALID) {
                table.Add(sroot, msg);
            }            
            // b. check WERoot           
            updateErrorTable(sroot.WERoot, table);            
            // c. check Raw ElementGroup 
            updateErrorTable(sroot.RawElemsGrp, table);
            // d. check Operation root 
            updateErrorTable(sroot.ProcRoot, table);                        
            //// e. check Parameters Root 
            //updateErrorTable(sroot.ProcRoot.ParamPublic, table);
            //updateErrorTable(sroot.ProcRoot.ParamPrivate, table);
        }
        /// <summary>
        /// check all paramGroup and all its parameters 
        /// </summary>
        /// <param name="pGrp">parent parmater group</param>
        /// <param name="table"></param>
        private void updateErrorTable(ParamGroup pGrp, HashtableEx table) {
            if (table == null) {
                return;
            }
            ValidationMsg msg = ModelManager.Instance.getValidMsg(pGrp);
            if (msg.Type != MsgType.VALID) {
                table.Add(pGrp, msg);
            }
            // check children parameters 
            foreach (Parameter param in pGrp.Params) {
                msg = ModelManager.Instance.getValidMsg(param);
                if (msg.Type != MsgType.VALID) {
                    table.Add(param, msg);
                }
            }
            // check sub parameter groups 
            foreach (ParamGroup grp in pGrp.SubGroups) {
                updateErrorTable(grp, table);
            }            
        }
        /// <summary>
        /// check process and all its children process and operations, check all 
        /// relative OpConditions and rules 
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="table"></param>
        private void updateErrorTable(Process proc, HashtableEx table) {
            if (table == null) {
                return;
            }
            // check process validation 
            ValidationMsg msg = ModelManager.Instance.getValidMsg(proc);
            if (msg.Type != MsgType.VALID) {
                table.Add(proc, msg);
            }
            // check process OpConditions 
            foreach (OpCondition opc in proc.OpConditions) {
                msg = ModelManager.Instance.getValidMsg(opc);
                if (msg.Type != MsgType.VALID) {
                    table.Add(opc, msg);
                }
            }
            // check process ParamCmds 
            foreach (ParamCmd cmd in proc.Commands) {
                msg = ModelManager.Instance.getValidMsg(cmd);
                if (msg.Type != MsgType.VALID) {
                    table.Add(cmd, msg);
                }
            }
            // check rules 
            foreach (OperationRule rule in proc.Rules) {
                msg = ModelManager.Instance.getValidMsg(rule);
                if (msg.Type != MsgType.VALID) {
                    table.Add(rule, msg);
                }
            }
            // check Operations 
            foreach (Operation op in proc.Ops) {
                updateErrorTable(op, table);
            }
            // check Parameters 
            updateErrorTable(proc.ParamPublic, table);
            updateErrorTable(proc.ParamPrivate, table);

            // check sub processes
            foreach (Process tp in proc.Procs) {
                updateErrorTable(tp, table);   
            }
        }
        /// <summary>
        /// check Operation and all its OpConditions and rules 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="table"></param>
        private void updateErrorTable(Operation op, HashtableEx table) {
            if (table == null) {
                return;
            }
            ValidationMsg msg = ModelManager.Instance.getValidMsg(op);
            if (msg.Type != MsgType.VALID) {
                table.Add(op, msg);
            }
            // check ParamCmds 
            foreach (ParamCmd cmd in op.Commands) {
                msg = ModelManager.Instance.getValidMsg(cmd);
                if (msg.Type != MsgType.VALID) {
                    table.Add(cmd, msg);
                }
            }
            // check rules 
            foreach (OperationRule rule in op.Rules) {
                msg = ModelManager.Instance.getValidMsg(rule);
                if (msg.Type != MsgType.VALID) {
                    table.Add(rule, msg);
                }
            }
            // check all OpConditions 
            foreach (OpCondition opc in op.OpConditions) {
                msg = ModelManager.Instance.getValidMsg(opc);
                if (msg.Type != MsgType.VALID) {
                    table.Add(opc, msg);
                }
            }
        }
        /// <summary>
        /// check weg and all its children element, and add the validation msg 
        /// into table. 
        /// </summary>
        /// <param name="weg"></param>
        /// <param name="table"></param>
        private void updateErrorTable(WebElementGroup weg, HashtableEx table) {
            if (table == null) {
                return;
            }
            ValidationMsg msg = ModelManager.Instance.getValidMsg(weg);
            if (msg.Type != MsgType.VALID) {
                table.Add(weg,msg);
            }
            // check web elemnets 
            foreach (WebElement we in weg.Elems) {
                msg = ModelManager.Instance.getValidMsg(we);
                if (msg.Type != MsgType.VALID) {
                    table.Add(we, msg);
                }
            }
            // check sub groups 
            foreach (WebElementGroup tweg in weg.SubGroups) {
                updateErrorTable(tweg, table);
            }
        }
        /// <summary>
        /// used to mark the WebElement/WebElementGroup/Parameter/ParamGroup as a selected status 
        /// </summary>
        /// <param name="be">WebElement or WebElementGroup</param>
        internal void markSelectedTreeNode(BaseElement be) {
            if (be == null) {
                return;
            }
            TreeNode node = null;
            TabPage page = null;
            TreeView tv = null;
            if (be is WebElement || be is WebElementGroup ){
                page = wePage;
                tv = weTreeView1;                
            }else if(be is Parameter || be is ParamGroup) {
                page = paramPage;
                tv = paramTV;                
            }

            if (page != null && tv!=null) {
                if (elemTabCtrl.SelectedTab != page) {
                    this.elemTabCtrl.SelectedTab = page;
                }                
                node = getNodeByBE(tv.Nodes[0], be);
                if (node == null && tv.Nodes.Count > 1) {
                    // check the private parmameter group.
                    node = getNodeByBE(tv.Nodes[1], be);
                }
                if (node != null) {
                    this.SelectedObj = be;
                    tv.SelectedNode = node;
                }                
            }
        }
        /// <summary>
        /// used to mark the Process/Operation/OpCondition as a selected status 
        /// </summary>
        /// <param name="be"></param>
        internal void markSelectedDiagramElement(BaseElement be) {
            this.diagramManager.markSelectedDiagramElement(be);
        }
        #endregion script validation 
        #region override methods 
        protected override bool ProcessDialogKey(Keys keyData) {
            if (keyData == Keys.Delete) {
                if (this.weTreeView1.Focused) {
                    removeWebElement();
                }
            }
            return base.ProcessDialogKey(keyData);
        }
        #endregion override methods   
        #region common ui functions 
        /// <summary>
        /// For Parameter tree and WebElement tree 
        /// whether a node can be moved under target node, if there is no duplicated node under the target node, 
        /// just return true, or else, if the the duplicated node is not the same type with drag node, just return true, else 
        /// return false
        /// </summary>
        /// <param name="tgtNode"></param>
        /// <param name="dragNode"></param>
        /// <returns></returns>
        private bool isValidTreeDnD(TreeNode tgtNode, TreeNode dragNode) {
            int index = this.getTreeNodeByNameAndType(tgtNode, dragNode.Tag as BaseElement);
            TreeNode duplicateNode = null;
            if (index != -1) {
                duplicateNode = tgtNode.Nodes[index];
            } else {
                return true;
            }

            if (duplicateNode != null) {
                if (duplicateNode.Tag.GetType() == dragNode.Tag.GetType()) {
                    string msg = UILangUtil.getMsg("ide.feditor.tree.dnd.msg1",tgtNode.Text,dragNode.Text);
                    string title = UILangUtil.getMsg("ide.feditor.tree.dnd.title1");
                    DialogResult dr = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                    if (dr == DialogResult.OK) {
                        return true;
                    }
                } else {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// find the index of the child node with base element name and element type return -1 if not find 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="elem">base element, it should be WebElement/WebElementGroup/Parameter/ParamGroup</param>
        /// <returns></returns>
        private int getTreeNodeByNameAndType(TreeNode parentNode, BaseElement elem) {
            if (parentNode == null || parentNode.Nodes == null || elem == null) {
                return -1;
            }
            int index = 0;
            bool find = false;
            foreach (TreeNode tn in parentNode.Nodes) {
                BaseElement be = tn.Tag as BaseElement;
                if (elem.Name.Equals(be.Name) && be.GetType() == elem.GetType()) {
                    find = true;
                    break;
                }
                index++;
            }
            if (find) {
                return index;
            } else {
                return -1;
            }
        }
        /// <summary>
        /// remove the node from WebElement or Parameter tree, update the tree view and script model
        /// </summary>
        /// <param name="node"></param>
        private void doRemoveTreeNode(TreeNode node) {
            if (node != null) {
                node.Remove();
                if (node.Tag is WebElement) {
                    WebElement we = node.Tag as WebElement;
                    we.Collection.Remove(we);
                } else if (node.Tag is WebElementGroup) {
                    WebElementGroup weg = node.Tag as WebElementGroup;
                    if (weg.Collection != null) {
                        weg.Collection.Remove(weg);
                    }
                } else if (node.Tag is Parameter) {
                    Parameter param = node.Tag as Parameter;
                    param.Collection.Remove(param);
                } else if (node.Tag is ParamGroup) {
                    ParamGroup pg = node.Tag as ParamGroup;
                    if (pg.Collection != null) {
                        pg.Collection.Remove(pg);
                    }
                }
            }
        }
        /// <summary>
        /// move the drag node to the target node position if they the same type. 
        /// e.g both WebElementGroup or WebElement or Parameter or ParamGroup
        /// 
        /// </summary>
        /// <param name="tgtNode"></param>
        /// <param name="dragNode"></param>
        private void moveToNode(TreeNode tgtNode, TreeNode dragNode) {
            TreeNode pn = tgtNode.Parent;
            dragNode.Remove();
            pn.Nodes.Insert(pn.Nodes.IndexOf(tgtNode), dragNode);
        }
        /// <summary>
        /// If the target node contains a duplicated node as the dragnode(e.g. currently the node with same name),
        /// it will first remove the existed duplicated node and move the drag node under target node. 
        /// 
        /// It will make sure that the first part is Group elements(WebElementGroup/ParamGroup) and the next part
        /// elements(WebElement/Parameter) move the drag node under target node. 
        /// </summary>
        /// <param name="tgtNode">target node </param>
        /// <param name="dragNode">src node </param>
        private void moveUnderNode(TreeNode tgtNode, TreeNode dragNode) {
            int dindex = getTreeNodeByNameAndType(tgtNode, dragNode.Tag as BaseElement);
            // Find duplicated element under the target node. 
            if (dindex != -1) {
                TreeNode dnode = tgtNode.Nodes[dindex];
                if (dnode != null) {
                    // insert drag node 
                    tgtNode.Nodes.Insert(dindex, dragNode);
                    // remove tree node 
                    dnode.Remove();
                    // update script model 
                    if (dnode.Tag is WebElement) {
                        WebElement we = dnode.Tag as WebElement;
                        int ti = we.Collection.IndexOf(we);
                        if (dragNode.Tag is WebElement) {
                            we.Collection.Insert(ti, dragNode.Tag as WebElement);
                        }
                        we.Collection.Remove(we);
                    } else if (dnode.Tag is WebElementGroup) {
                        WebElementGroup weg = dnode.Tag as WebElementGroup;
                        int ti = weg.Collection.IndexOf(weg);
                        if (dragNode.Tag is WebElementGroup) {
                            weg.Collection.Insert(ti, dragNode.Tag as WebElementGroup);
                        }
                        weg.Collection.Remove(weg);
                    } else if (dnode.Tag is Parameter) {
                        Parameter param = dnode.Tag as Parameter;
                        int ti = param.Collection.IndexOf(param);
                        if (dragNode.Tag is Parameter) {
                            param.Collection.Insert(ti, dragNode.Tag as Parameter);
                        }
                        param.Collection.Remove(param);
                    } else if (dnode.Tag is ParamGroup) {
                        ParamGroup pg = dnode.Tag as ParamGroup;
                        if (pg.Collection != null) {
                            int ti = pg.Collection.IndexOf(pg);
                            if (dragNode.Tag is ParamGroup) {
                                pg.Collection.Insert(ti, dragNode.Tag as ParamGroup);
                            }
                            pg.Collection.Remove(pg);
                        }
                    }
                }
            } else {
                // add the webelement group at the last of the tree group node of the target node. 
                // below is find proper TreeNode position for insert the drag node. 
                if (dragNode.Tag is WebElementGroup) {
                    int i = 0;
                    foreach (TreeNode tn in tgtNode.Nodes) {
                        if (tn.Tag is WebElement) {
                            tgtNode.Nodes.Insert(i, dragNode);
                        }
                        i++;
                    }
                    if (!tgtNode.Nodes.Contains(dragNode)) {
                        tgtNode.Nodes.Add(dragNode);
                    }
                    // update script model 
                    if (tgtNode.Tag is WebElementGroup) {
                        WebElementGroup weg = tgtNode.Tag as WebElementGroup;
                        weg.SubGroups.AddUnique(dragNode.Tag as WebElementGroup);
                    }
                } else if (dragNode.Tag is WebElement) {
                    tgtNode.Nodes.Add(dragNode);

                    // update script model 
                    WebElement we = dragNode.Tag as WebElement;
                    if (tgtNode.Tag is WebElementGroup) {
                        WebElementGroup weg = tgtNode.Tag as WebElementGroup;
                        weg.Elems.AddUnique(we);
                    }
                } else if (dragNode.Tag is Parameter) {
                    tgtNode.Nodes.Add(dragNode);
                    // update script model 
                    Parameter param = dragNode.Tag as Parameter;
                    if (tgtNode.Tag is ParamGroup) {
                        ParamGroup grp = tgtNode.Tag as ParamGroup;
                        grp.Params.AddUnique(param);
                    }
                } else if (dragNode.Tag is ParamGroup) {
                    int i = 0;
                    foreach (TreeNode tn in tgtNode.Nodes) {
                        if (tn.Tag is Parameter) {
                            tgtNode.Nodes.Insert(i, dragNode);
                        }
                        i++;
                    }
                    if (!tgtNode.Nodes.Contains(dragNode)) {
                        tgtNode.Nodes.Add(dragNode);
                    }
                    // update script model 
                    if (tgtNode.Tag is ParamGroup) {
                        ParamGroup tgrp = tgtNode.Tag as ParamGroup;
                        tgrp.SubGroups.AddUnique(dragNode.Tag as ParamGroup);
                    }
                }
            }
        }
        /// <summary>
        /// this is used to extend the customizded cursor and feedback when 
        /// do DnD, it is used for WebElement tree and parameter tree 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
            TreeNode preNode = this._weTreePreTgtNode;
            TreeNode currNode = this._weTreeCurrentTgtNode;
            if (elemTabCtrl.SelectedTab == paramPage) {
                preNode = this._paramTreePreTgtNode;
                currNode = this._paramTreeCurrentTgtNode;
            }
            string pre = preNode != null ? preNode.Text : null;
            string curr = currNode != null ? currNode.Text : null;
            Log.println_wetree("feed back . WebElement, effect = " + e.Effect + ", pre targtet = " + pre + ", current target = " + curr);

            // update background color 
            if (e.Effect == DragDropEffects.None) {
                if (currNode != null) {
                    currNode.BackColor = SystemColors.Window;
                    currNode.ForeColor = SystemColors.WindowText;
                    currNode = null;
                }
                if (preNode != null) {
                    preNode.BackColor = SystemColors.Window;
                    preNode.ForeColor = SystemColors.WindowText;
                    preNode = null;
                }
            } else if (e.Effect == DragDropEffects.Move) { // order the element under a same parent 
                if (currNode != null) {                    
                    currNode.BackColor = SystemColors.Highlight;
                    currNode.ForeColor = SystemColors.HighlightText;
                }
                if (preNode != null && preNode!=currNode) {
                    preNode.BackColor = SystemColors.Window;
                    preNode.ForeColor = SystemColors.WindowText;
                }
            } else if (e.Effect == DragDropEffects.Link) {
                if (currNode != null) {
                    currNode.BackColor = SystemColors.Highlight;
                    currNode.ForeColor = SystemColors.HighlightText;
                }
                if (preNode != null && currNode != preNode) {
                    preNode.BackColor = SystemColors.Window;
                    preNode.ForeColor = SystemColors.WindowText;
                }
            }
        }    
        #endregion common ui functions         
            
    }
}
