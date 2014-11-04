using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.ide.editor.model;
using WebMaster.lib.engine;
using System.Drawing;
using WebMaster.lib.ui;

namespace WebMaster.ide.editor
{
    public class DiagramManager
    {
        #region variables 
        private BigModel bigmodel = null;
        // current script model 
        public BigModel BigModel {
            get { return bigmodel; }
        }
        /// <summary>
        /// diagram area table page container 
        /// </summary>
        private TabControl workareaTabCtrl = null;
        private FlowEditor flowEditor = null;
        /// <summary>
        /// the current active canvas 
        /// </summary>
        private Canvas activeCanvas = null;
        /// <summary>
        /// active canvas 
        /// </summary>
        public Canvas ActiveCanvas {
            get { return activeCanvas; }
            set {
                if (value != null && !value.Equals(activeCanvas)) {
                    SelectedObj = getSelectedObj(value.SelectedObj);
                }
                activeCanvas = value;
            }
        }
        private BaseElement getSelectedObj(object obj) {
            NodeView nv = obj as NodeView;
            if (nv != null) {
                return nv.Node.RefOp;
            }
            Connection con = obj as Connection;
            if (con != null) {
                return con.RefCon;
            }
            Diagram diagram = obj as Diagram;
            if (diagram != null) {
                return diagram.Proc;
            }
            Canvas canvas = obj as Canvas;
            if (canvas != null && canvas.Diagram!=null) {
                return canvas.Diagram.Proc;
            }
            return null;
        }
        /// <summary>
        /// data is Process, Operation, OpCondition 
        /// </summary>
        private BaseElement _selectedObj = null;
        /// <summary>
        /// data is Process, Operation, OpCondition 
        /// </summary>
        public BaseElement SelectedObj {
            get { return _selectedObj; }
            set {
                //if (value != null && !value.Equals(_selectedObj)) {
                    raiseDiagramSelectionChanged(ActiveCanvas, value);
                //}
                _selectedObj = value; 
            }
        }

        /// <summary>
        /// active tab page in the working area
        /// </summary>
        private TabPage activePage = null;
        private ToolStripButton toolButton = null;
        /// <summary>
        /// active tool strip button
        /// </summary>
        public ToolStripButton ActiveTool {
            get { return toolButton; }
            set { toolButton = value; }
        }
        private ContextMenuStrip cms_diagram = null;
        #endregion variables
        #region events 
        /// <summary>
        /// sender is canvas, data is Process, Operation, OpCondition or ScriptRoot 
        /// </summary>
        public event EventHandler<CommonEventArgs> DiagramSelectionChanged;
        protected virtual void OnDiagramSelectionChanged(CommonEventArgs e) {
            EventHandler<CommonEventArgs> diagramSelectionChanged = DiagramSelectionChanged;
            if (diagramSelectionChanged != null) {
                diagramSelectionChanged(this, e);
            }
        }
        /// <summary>
        /// sender is current active canvas 
        /// data is Process, Operation, OpCondition or ScriptRoot 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void raiseDiagramSelectionChanged(Object sender, BaseElement data) {
            if (data != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, data);
                OnDiagramSelectionChanged(evt);
            }
        }
        /// <summary>
        /// If data is Process or Operation, sender is NodeView: it will used when model updated( op name updated) in canvas
        /// If data is Node, sender is NodeView, data is Conndtion, sender is Canvas, it is used update the editor as dirty status 
        /// </summary>
        public event EventHandler<CommonEventArgs> ModelUpdateEvt;
        protected virtual void OnModelUpdateEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> modelUpdateEvt = ModelUpdateEvt;
            if (modelUpdateEvt != null) {
                modelUpdateEvt(this, e);
            }
        }
        /// <summary>
        /// If data is Process or Operation, sender is NodeView: it will used when model updated( op name updated) in canvas
        /// If data is Node, sender is NodeView, data is Conndtion, sender is Canvas, it is used update the editor as dirty status 
        /// </summary>
        /// <param name="sender">sender is node view(Process,Operation), or canvas (Node,Connection) </param>
        /// <param name="obj">Node, Connection, Process, or Operation</param>
        private void raiseModelUpdateEvt(Object sender, object obj) {
            if (obj is Process || obj is Operation || obj is Connection || obj is Node) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnModelUpdateEvt(evt);
            }
        }
        #endregion events         
        #region methods
        public DiagramManager(TabControl workareaTabCtrl,ContextMenuStrip cms,FlowEditor flowEditor) {
            this.workareaTabCtrl = workareaTabCtrl;
            this.cms_diagram = cms;
            this.flowEditor = flowEditor;
            this.workareaTabCtrl.Selected += new TabControlEventHandler(workareaTabCtrl_Selected);
        }

        void workareaTabCtrl_Selected(object sender, TabControlEventArgs e) {
            if (this.workareaTabCtrl.TabPages.Count == 0) {
                return;
            }
            this.activePage = this.workareaTabCtrl.SelectedTab;
            Canvas canvas = (Canvas)this.activePage.Controls[0];
            if (canvas != this.ActiveCanvas) {
                canvas.SelectedObj = canvas.Diagram;                
            }
            this.ActiveCanvas = canvas;
            Diagram diagram = this.ActiveCanvas.Diagram;
            if (diagram != null) {
                this.SelectedObj = diagram.Proc;
            }
        }
        /// <summary>
        /// update model, and update the diagram view based on model
        /// if model is null, clean views
        /// else build the main diagram from model. 
        /// </summary>
        /// <param name="bmodel"></param>
        internal void setBigModel(BigModel bmodel) {
            if (bmodel == null || this.BigModel != bmodel) {
                this.bigmodel = bmodel;
                resetView();
            }
        }
        private void resetView() {
            this.closeAllDiagrams();
            if (this.BigModel != null) {                
                //build main diagram 
                TabPage page = createDiagramPage(this.workareaTabCtrl,this.BigModel.VRoot.MainDiagram);

                //update the active page and graphical control
                this.activeCanvas = (Canvas)page.Controls[0];
                this.activePage = page;

                this.SelectedObj = this.BigModel.VRoot.MainDiagram.Proc;
            }
        }
        /// <summary>
        /// set the tool strip unchecked, set the ActiveCanvas.OpType invalid
        /// </summary>
        internal void resetActiveToolStatus() {
            if (this.ActiveTool != null) {
                ActiveTool.Checked = false;
                ActiveTool = null;
            }
        }
        #endregion common methods 
        #region diagram methods
        /// <summary>
        /// create a new diagram page with the diagram contents and add it into the tab page area
        /// </summary>
        /// <param name="tabCtrl">diagram page container</param>
        /// <param name="diagram"></param>
        public TabPage createDiagramPage(TabControl tabCtrl,Diagram diagram) {
            if (diagram == null) {                
                //TODO log error 
            }
            // create and initial canvas 
            Canvas canvas = new WebMaster.ide.editor.Canvas(this);
            canvas.Diagram = diagram;
            canvas.AllowDrop = true;
            canvas.AutoScroll = true;
            canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            //canvas.Location = new System.Drawing.Point(0, 0);
            //graphControl1.Name = "graphControl1";
            //canvas.Size = new System.Drawing.Size(579, 341);
            canvas.MouseRightClickEvt += new EventHandler<CommonEventArgs>(canvas_MouseRightClickEvt);
            canvas.ModelUpdateEvt += new EventHandler<CommonEventArgs>(canvas_ModelUpdateEvt);

            // initial canvas with diagram content.
            this.buildDiagram(canvas, diagram);

            // create and initial tabpage 
            TabPage tabPage = new System.Windows.Forms.TabPage();
            tabPage.Controls.Add(canvas);
            //tabPage.Location = new System.Drawing.Point(4, 21);            
            tabPage.Padding = new System.Windows.Forms.Padding(0);
            //tabPage.Size = new System.Drawing.Size(585, 347);
            tabPage.AutoScroll = true;
            tabPage.Text = diagram.Proc.Name;
            tabPage.UseVisualStyleBackColor = true;
            //tabPage.BackColor = Color.Red;

            workareaTabCtrl.Controls.Add(tabPage);

            return tabPage;
        }
        /// <summary>
        /// close all opened diagram UI 
        /// </summary>
        public void closeAllDiagrams() {
            workareaTabCtrl.TabPages.Clear();
        }
        void canvas_ModelUpdateEvt(object sender, CommonEventArgs e) {
            raiseModelUpdateEvt(sender, e.Data);
        }
        void canvas_MouseRightClickEvt(object sender, CommonEventArgs e) {
            flowEditor.updateCanvasConextMenu();
            Canvas canvas = sender as Canvas;
            canvas.ContextMenuStrip = cms_diagram;
        }
        /// <summary>
        /// open and active a diagram for the process, if not have, create a new diagram first 
        /// </summary>
        /// <param name="process"></param>
        internal void openProcessDiagram(Process process) {
            if (process == null) { 
                //TODO log or exception
                return;
            }
            TabPage page = null;
            // check wether there is an existed diagram page 
            foreach (TabPage p in workareaTabCtrl.TabPages) {
                Canvas canvas = (Canvas)p.Controls[0];
                if (canvas.Diagram.Proc.Equals(process)) {
                    page = p;
                    break;
                }
            }
            // create and open a new diagram 
            if (page == null) {
                Diagram diagram = null;                
                foreach (Diagram d in this.BigModel.VRoot.Diagrams) {
                    if (d.Proc.Equals(process)) {
                        diagram = d;
                        break;
                    }
                }               
                // create a new diagram 
                if(diagram == null) { 
                    diagram = ModelFactory.createDiagram();
                    diagram.Proc = process;
                    diagram.Name = diagram.Proc.Name;
                    this.BigModel.VRoot.Diagrams.Add(diagram);

                    // initialize the start node 
                    Node snode = ModelFactory.createNode();
                    snode.RefOp = process.StartOp;
                    snode.Location = new Point(30, 30);
                    snode.Size = new Size(30, 30);
                    diagram.Nodes.Add(snode);
                }
                page = createDiagramPage(this.workareaTabCtrl, diagram);

                //update the active page and graphical control
                this.activeCanvas = (Canvas)page.Controls[0];
                this.activePage = page;                
            }
            // set the diagram page actived 
            workareaTabCtrl.SelectedTab = page;
        }
        /// <summary>
        /// close active diagram 
        /// </summary>
        internal void closeActiveDiagram() {            
            Process process = this.ActiveCanvas.Diagram.Proc as Process;
            if (process != null && process!=this.BigModel.SRoot.ProcRoot) {
                TabPage rpage = this.activePage;
                this.workareaTabCtrl.TabPages.Remove(rpage);
                rpage.Dispose();
            }
        }
        /// <summary>
        /// close the diagram tab if it is opened
        /// </summary>
        /// <param name="diagram"></param>
        internal void closeDiagram(Diagram diagram) {
            if (diagram != null) {
                TabPage dpage = null;
                foreach (TabPage page in this.workareaTabCtrl.TabPages) {
                    Canvas canvas = (Canvas)page.Controls[0];
                    if (diagram == canvas.Diagram) {
                        dpage = page;
                        break;
                    }
                }
                if (dpage != null) {
                    this.workareaTabCtrl.TabPages.Remove(dpage);
                    dpage.Dispose();
                }
            }
        }
        /// <summary>
        /// build a canvas page with diagram contents 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="diagram"></param>
        internal void buildDiagram(Canvas canvas, Diagram diagram) {
            foreach (Node node in diagram.Nodes) {
                buildNodeView(node, canvas);
            }
        }
        /// <summary>
        /// build node view for node 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="canvas"></param>
        private void buildNodeView(Node node, Canvas canvas) {
            NodeView nv = new NodeView(canvas);
            nv.Location = node.Location;
            nv.Size = node.Size;
            nv.Node = node;
            nv.Label = node.RefOp.Name;
            switch (node.RefOp.OpType) {
                case OPERATION.START:
                    nv.Image = global::ide.Properties.Resources.op_start16;
                    break;
                case OPERATION.END:
                    nv.Image = global::ide.Properties.Resources.op_stop16;
                    break;
                case OPERATION.CLICK:
                    nv.Image = global::ide.Properties.Resources.op_click16;
                    break;
                case OPERATION.INPUT:
                    nv.Image = global::ide.Properties.Resources.op_input16;
                    break;
                case OPERATION.PROCESS:
                    nv.Image = global::ide.Properties.Resources.op_process16;
                    break;
                case OPERATION.OPEN_URL_T:
                    nv.Image = global::ide.Properties.Resources.op_url16;
                    break;
                case OPERATION.NOP:
                    nv.Image = global::ide.Properties.Resources.op_nop16;
                    break;
            }
            canvas.Controls.Add(nv);
        }
        /// <summary>
        /// update canvas tab page title if the bind process name updated 
        /// </summary>
        /// <param name="process"></param>
        internal void updateCanvasTitle(Process process) {
            foreach (TabPage page in this.workareaTabCtrl.TabPages) {
                Canvas canvas = page.Controls[0] as Canvas;
                if (canvas.Diagram.Proc.Equals(process)) {
                    page.Text = process.Name;
                }
            }
        }
        /// <summary>
        /// Mark the editor in dirty status 
        /// </summary>
        public void markModelDirty() {
            this.flowEditor.App.markDirty();
        }
        #endregion diagram methods
        #region validation status marker
        /// <summary>
        /// mark NodeView with validation status, data must be Operation or Process
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msgType"></param>
        internal void markNodeViewValidation(object data, MsgType msgType) {
            if (data is Process || data is Operation) {
                foreach (TabPage page in this.workareaTabCtrl.TabPages) {
                    Canvas canvas = page.Controls[0] as Canvas;
                    foreach (NodeView nv in canvas.Controls) {
                        if (nv.Node.RefOp == data) {
                            doMarkNodeView(nv,msgType);
                            return;
                        }
                    }
                }
            }
        }
        private void doMarkNodeView(NodeView nv, MsgType msgType) {
            if (nv != null) {
                nv.ValidType = msgType;
                //TODO... check refresh performance 
                nv.Refresh();
            }
        }
        /// <summary>
        /// Mark the Operation/Process/OpCondition/OperationRule/ParamCmd stub op/proc's container process opened if not open.
        /// and then set the stub element selected. 
        /// </summary>
        /// <param name="be">Process/Operation/OpCondition/OperationRule/ParamCmd</param>
        public void markSelectedDiagramElement(BaseElement be) {
            // find diagram process 
            Process proc = null ;
            Operation stubOp = null;
            if(be is Operation){
                Operation op = be as Operation ;
                proc = ModelManager.Instance.getOwnerProc(op);
                // op is the ScriptRoot process 
                if (proc == null && op is Process) {
                    proc = op as Process;
                }
            }else if(be is OpCondition){
                OpCondition opc = be as OpCondition ;
                Operation op = ModelManager.Instance.getOwnerOp(opc);
                proc = ModelManager.Instance.getOwnerProc(op);
            } else if (be is OperationRule) {
                OperationRule rule = be as OperationRule;
                stubOp = ModelManager.Instance.getOwnerOp(rule);
                proc = ModelManager.Instance.getOwnerProc(stubOp);
                // op is the ScriptRoot process 
                if (proc == null && stubOp is Process) {
                    proc = stubOp as Process;
                }
            } else if (be is ParamCmd) {
                ParamCmd cmd = be as ParamCmd;
                stubOp = ModelManager.Instance.getOwnerOp(cmd);
                proc = ModelManager.Instance.getOwnerProc(stubOp);
                // op is ScriptRoot process
                if (proc == null && stubOp is Process) {
                    proc = stubOp as Process;
                }
            }
            if(proc == null){
                //TODO log error 
                return ;
            }
            Canvas canvas = null ;
            foreach(TabPage page in workareaTabCtrl.TabPages){
                Canvas c = page.Controls[0] as Canvas;
                if(proc.Equals(c.Diagram.Proc)){
                    canvas = c ;
                    this.ActiveCanvas = canvas ;
                    this.activePage = page ;
                    workareaTabCtrl.SelectedTab = page;
                    break ;
                }
            }
            object sel = null ;
            if(canvas == null){
                openProcessDiagram(proc);
                canvas = this.ActiveCanvas ;
            }
            if (be is OperationRule || be is ParamCmd) {
                if (stubOp != null) {
                    be = stubOp;
                }
            }
            // mark the node view selected 
            if (be is Process || be is Operation) {
                foreach (Control c in canvas.Controls) {
                    if (c is NodeView) {
                        NodeView nv = c as NodeView;
                        if (nv.Node.RefOp == be) {
                            sel = nv;
                            break;
                        }
                    }
                }
            }
            // mark the connection selected 
            if (be is OpCondition) {
                foreach (Connection con in canvas.Diagram.Connections) { 
                    if(con.RefCon.Equals(be)){
                        sel = con ;
                        break ;
                    }
                }
            }
            // handle if the Rule and ParamCmd is processRoot 
            if (stubOp is Process) {
                Process tproc = ModelManager.Instance.getOwnerProc(stubOp);
                if (tproc == null) {
                    sel = stubOp;
                }
            }
            if (sel != null) {
                canvas.SelectedObj = sel;
                canvas.Refresh();
            }
        }
        #endregion validation status marker
    }
}
