using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib.ui;
using WebMaster.lib;

namespace WebMaster.ide.editor.model
{
    public partial class NodeView : UserControl
    {
        #region constructor
        public NodeView(Canvas ctrl) {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();
            this.canvas = ctrl;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }
        #endregion constructor
        #region variable
        public static readonly int MIN_WIDTH = 30;
        public static readonly int MIN_HEIGHT = 30;
        /// <summary>
        /// hover status of icon panel
        /// </summary>
        private bool hovered1 = false;
        /// <summary>
        /// hovered status of text box
        /// </summary>
        private bool hovered2 = false;
        /// <summary>
        /// tells whether the current entity is hovered by the mouse
        /// </summary>
        public bool Hovered {
            get { return hovered1 || hovered2; }
        }
        /// <summary>
        /// the control to which the eneity belongs
        /// </summary>
        protected Canvas canvas;
        /// <summary>
        /// select status of the node
        /// </summary>
        private bool selected = false;
        /// <summary>
        /// whether the node is selected 
        /// </summary>
        public bool Selected {
            get { return selected; }
            set { selected = value; }
        }
        private Node _model = null;
        /// <summary>
        /// real model applied to the entity
        /// </summary>
        public Node Node {
            get { return _model; }
            set { _model = value; }
        }        
        /// <summary>
        /// label of the node 
        /// </summary>
        public string Label {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }
        /// <summary>
        /// icon of the node 
        /// </summary>
        public Image Image {
            get { return this.panel_Img.BackgroundImage; }
            set { this.panel_Img.BackgroundImage = value; }
        }
        private MsgType validType = MsgType.VALID;
        /// <summary>
        /// Validation type of the node, it is used control the validation image
        /// at the top-left
        /// </summary>
        public MsgType ValidType {
            get { return validType; }
            set { validType = value; }
        }
        /// <summary>
        /// if the node is in a moving status, it will record the last refreshed end point
        /// it is used reduce the refresh times 
        /// </summary>
        private Point moveRefreshedEnd = DiagramUtil.PointNull;
        #endregion variab
        #region Image area events
        private void panelImg_MouseEnter(object sender, EventArgs e) {
            hovered1 = true;
            updateNodeStatus();
        }
        private void panelImg_MouseMove(object sender, MouseEventArgs e) {
            if (this.canvas.Action == ACTION.MOVE_NODE && e.Button == MouseButtons.Left && sender is Control) {
                Control ctrl = sender as Control;
                Point sp = ctrl.PointToScreen(e.Location);
                this.canvas.Move_endPoint = this.canvas.PointToClient(sp);
                //Log.println_graph("Node image , Mouse move, endPoint = " + this.canvas.Move_endPoint);
                if (this.isMoveNeedRefresh(canvas.Move_startPoint, canvas.Move_endPoint)) {
                    Refresh();
                }
            }
        }
        private void panelImg_MouseLeave(object sender, EventArgs e) {
            hovered1 = false;
            updateNodeStatus();
        }
        private void panelImg_MouseUp(object sender, MouseEventArgs e) {
            hovered1 = false;
            updateNodeStatus();
            doMouseUp(sender,e);
        }
        private void panelImg_MouseDown(object sender, MouseEventArgs e) {
            hovered1 = false;
            selected = true;

            updateNodeStatus();

            this.canvas.SelectedObj = this;
            if (this.Location.X < 0 || this.Location.Y < 0) {
                canvas.ScrollControlIntoView(this);
            }

            doMouseDown(sender,e);
        }
        private void panelImg_MouseDoubleClick(object sender, MouseEventArgs e) {
            doDoubleClick(e);
        }
        private void panelImg_DragDrop(object sender, DragEventArgs e) {
            handleDragDrop(sender, e);
        }
        private void panelImg_DragEnter(object sender, DragEventArgs e) {
            handleDragEnter(sender, e);
        }
        private void panel_Img_Paint(object sender, PaintEventArgs e) {
            if (validType == MsgType.ERROR) {
                e.Graphics.DrawImage(global::ide.Properties.Resources.error_ovr, 0, 0);
            } else if (validType == MsgType.WARNING) {
                e.Graphics.DrawImage(global::ide.Properties.Resources.warn_ovr, 0, 0);
            }
        }
        #endregion Image area events
        #region text area events
        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                updateModelAndView();
                hideEditBox();
                canvas.raiseModelUpdateEvt(this, Node);
            }
        }
        private void textPanel_MouseDown(object sender, MouseEventArgs e) {
            hovered2 = false;
            selected = true;

            if (this.canvas.Action != ACTION.LINK && this.canvas.Action != ACTION.MOVE_CTR) {
                this.canvas.SelectedObj = this;
                if (this.Location.X < 0 || this.Location.Y < 0) {
                    canvas.ScrollControlIntoView(this);
                }
            }
            hideEditBox();
            if (this.textPanel.Focused) {
                this.panel_Img.Focus();
            }
            updateNodeStatus();

            doMouseDown(sender, e);
        }
        private void textPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
            doDoubleClick(e);
        }
        private void textPanel_MouseEnter(object sender, EventArgs e) {
            hovered2 = true;
            updateNodeStatus();
        }
        private void textPanel_MouseMove(object sender, MouseEventArgs e) {
            if (this.canvas.Action == ACTION.MOVE_NODE && e.Button == MouseButtons.Left && sender is Control) {
                Control ctrl = sender as Control ;                
                Point sp = ctrl.PointToScreen(e.Location);
                this.canvas.Move_endPoint = this.canvas.PointToClient(sp);
                //Log.println_graph("Node text , Mouse move, endPoint = " + this.canvas.Move_endPoint);
                if (isMoveNeedRefresh(this.canvas.Move_startPoint, this.canvas.Move_endPoint)) {
                    Refresh();
                }
            }
            if (this.canvas.Action == ACTION.LINK || this.canvas.Action == ACTION.MOVE_CTR) {
                this.Cursor = Cursors.Cross;
            } else {
                this.Cursor = Cursors.SizeAll;
            }
        }
        private void textPanel_MouseLeave(object sender, EventArgs e) {
            hovered2 = false;
            updateNodeStatus();
        }
        private void textPanel_MouseUp(object sender, MouseEventArgs e) {
            hovered2 = false;
            updateNodeStatus();

            this.doMouseUp(sender,e);
        }
        private void textPanel_DragDrop(object sender, DragEventArgs e) {
            handleDragDrop(sender, e);
        }
        private void textPanel_DragEnter(object sender, DragEventArgs e) {
            handleDragEnter(sender, e);
        }
        void label1_MouseUp(object sender, MouseEventArgs e) {
            this.textPanel_MouseUp(sender, e);
        }
        void label1_MouseMove(object sender, MouseEventArgs e) {
            this.textPanel_MouseMove(sender, e);
        }
        void label1_MouseLeave(object sender, System.EventArgs e) {
            this.textPanel_MouseLeave(sender, e);
        }
        void label1_MouseEnter(object sender, System.EventArgs e) {
            this.textPanel_MouseEnter(sender, e);
        }
        void label1_MouseDown(object sender, MouseEventArgs e) {
            this.textPanel_MouseDown(sender, e);
        }
        void label1_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.textPanel_MouseDoubleClick(sender, e);
        }
        void label1_DragEnter(object sender, DragEventArgs e) {
            this.textPanel_DragEnter(sender, e);
        }
        void label1_DragDrop(object sender, DragEventArgs e) {
            this.textPanel_DragDrop(sender, e);
        }
        #endregion text area events
        #region node area event
        private void Node_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
            e.UseDefaultCursors = false;
            if (e.Effect == DragDropEffects.Link) {
                Cursor.Current = Cursors.Cross;
            } else if (e.Effect == DragDropEffects.Move) {
                Cursor.Current = Cursors.SizeAll;
            } else {                
                Cursor.Current = Cursors.Default;                
            }            
        }
        private void NodeView_LocationChanged(object sender, EventArgs e) {
            if (this.Node!=null && this.Location != this.Node.Location) {
                this.Node.Location = new Point(this.Location.X, this.Location.Y);
                //DiagramUtil.updateConnectors(this,this.canvas.AutoScrollPosition);
            }
        }
        private void NodeView_SizeChanged(object sender, EventArgs e) {
            if (this.Node!=null && this.Size != this.Node.Size) {
                this.Node.Size = new Size(this.Size.Width, this.Size.Height);
                DiagramUtil.updateConnectors(this,this.canvas.AutoScrollPosition);
                this.canvas.raiseModelUpdateEvt(this, this.Node);
            }            
        }
        #endregion node area event 
        #region methods
        private void doDoubleClick(MouseEventArgs e) {
            this.Selected = true;
            if (this.Node != null && this.Node.RefOp != null && this.Node.RefOp.OpType == OPERATION.PROCESS) {
                // handle process double click 
                doProcessDClick();
            } else { //handle update node text except process
                this.showEditBox();
                updateNodeStatus();
            }
        }
        /// <summary>
        /// open a tab page for diagram, if not have create a new, if opened, set is active diagram 
        /// </summary>
        private void doProcessDClick() {
            canvas.DiagramManager.openProcessDiagram((Process)this.Node.RefOp);
        }
        private void hideEditBox() {
            if (this.textBox1.Visible == true) {
                this.textBox1.Visible = false;
            }
        }
        private void showEditBox() {
            if (textBox1.Visible == false) {                
                this.textBox1.Visible = true;
                this.textBox1.Focus();
                this.textBox1.Text = this.Node.RefOp.Name;
            }
        }
        /// <summary>
        /// update the selected status, e.g cursor status
        /// </summary>
        public void updateNodeStatus() {
            if (this.canvas.Action == ACTION.LINK || this.canvas.Action == ACTION.MOVE_CTR) {
                this.Cursor = Cursors.Cross;
            } else {
                if (Hovered || Selected) {
                    this.Cursor = Cursors.SizeAll;
                } else {                    
                    this.Cursor = Cursors.Default;                    
                }
            }
        }
        private void handleDragDrop(object sender,DragEventArgs e) {
            DnDData ddd = (DnDData)e.Data.GetData(typeof(DnDData));
            if (ddd.DDType == DnDType.CREATE_CONN) {
                NodeView fromNode = (NodeView)ddd.Data;
                handleCreateConnection(fromNode, this);

                this.canvas.resetStatus();
                Refresh();
                // update editor status 
                canvas.raiseModelUpdateEvt(this.canvas, canvas.SelectedObj);
            } else if(ddd.DDType == DnDType.MOVE_CTR){
                Connector ctr = ddd.Data as Connector;
                handleMoveConnector(ctr, this);
                this.canvas.resetStatus();
                Refresh();

                // update editor status 
                canvas.raiseModelUpdateEvt(this.canvas, canvas.SelectedObj);
            }
        }

        private void handleMoveConnector(Connector ctr, NodeView nodeView) {
            // remove connector from original node
            ctr.RefNode.Ctrs.Remove(ctr);            
            // add connector to the new target node
            ctr.RefNode = nodeView.Node;
            // update the next operation
            ctr.Connection.RefCon.Op = nodeView.Node.RefOp;
            // move connector to the new node
            nodeView.Node.Ctrs.Add(ctr);
            // update connector location 
            DiagramUtil.updateConnector(this.canvas.AutoScrollPosition, ctr);
            // refresh
            this.canvas.SelectedObj = ctr.Connection;
            Refresh();
        }
        private void handleCreateConnection(NodeView fromNode, NodeView toNode) {
            // handle view model
            List<Point> ps = DiagramUtil.getFromToPoints(fromNode.Node, toNode.Node,this.canvas.AutoScrollPosition);
            Point fromP = ps[0];
            Point toP = ps[ps.Count - 1];
            Connection con = ModelFactory.createConnection(fromP, toP);
            
            con.From.RefNode = fromNode.Node;
            con.To.RefNode = toNode.Node;
            if (ps.Count == 5) {
                con.Points.Insert(1,ps[3]);
                con.Points.Insert(1, ps[2]);
                con.Points.Insert(1, ps[1]);
            }
            this.canvas.Diagram.Connections.Add(con);
            
            // update model 
            Operation op = fromNode.Node.RefOp;
            OpCondition opc = DiagramUtil.createUniqueOpCondition(op);
            opc.Op = toNode.Node.RefOp;            
            op.OpConditions.AddUnique(opc);
            con.RefCon = opc;

            this.canvas.SelectedObj = con;
        }
        private void handleDragEnter(object sender,DragEventArgs e) {            
            DnDData ddd = (DnDData)e.Data.GetData(typeof(DnDData));
            if (ddd.DDType == DnDType.CREATE_CONN || ddd.DDType == DnDType.MOVE_CTR) {
                e.Effect = DragDropEffects.Link;
            }
        }
        private void doMouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {                
                if (this.canvas.Action == ACTION.LINK) {
                    DnDData ddd = new DnDData();
                    ddd.DDType = DnDType.CREATE_CONN;
                    ddd.Data = this;
                    this.DoDragDrop(ddd, DragDropEffects.Link);
                } else {
                    // do drag drop
                    Control ctrl = sender as Control;
                    Point sp = ctrl.PointToScreen(e.Location);
                    this.canvas.Action = ACTION.MOVE_NODE;                                       
                    this.canvas.Move_startPoint = this.canvas.PointToClient(sp);
                }
            } else if(e.Button == System.Windows.Forms.MouseButtons.Right){
                this.canvas.raiseMouseRightClickEvt(canvas, canvas.SelectedObj);
            }

            Refresh();
        }
        /// <summary>
        /// if it is a move node action, move the node to new location and reset the status 
        /// if it is create link or move connector action, just reset status 
        /// </summary>
        private void doMouseUp(object sender, MouseEventArgs e) {
            if (this.canvas.Action == ACTION.LINK || this.canvas.Action == ACTION.MOVE_CTR) {
                this.canvas.resetStatus();
                Refresh();
            } else if (this.canvas.Action == ACTION.MOVE_NODE && sender is Control) {
                Control ctrl = sender as Control;
                Point sp = ctrl.PointToScreen(e.Location);
                this.canvas.Move_endPoint = this.canvas.PointToClient(sp);
                Log.println_graph("Node view, do Mouse up, Mouse move, endPoint = " + this.canvas.Move_endPoint);
                
                this.canvas.moveNode();
                this.canvas.resetStatus();
                if (this.isMoveNeedRefresh(canvas.Move_startPoint, canvas.Move_endPoint)) {
                    Refresh();
                }
                this.moveRefreshedEnd = DiagramUtil.PointNull;
            }
        }        
        /// <summary>
        /// update the model operation name and UI text label if it is in editing status 
        /// </summary>
        public void updateModelAndView() {
            if (isEditing()) {
                if (textBox1.Text.Length > 0 && this.textBox1.Text != this.label1.Text) {
                    this.panel_Img.Focus();
                    // update model 
                    this.Node.RefOp.Name = this.textBox1.Text;
                    // update ui 
                    this.label1.Text = this.textBox1.Text;
                }
                hideEditBox();
            }
        }
        /// <summary>
        /// whether the node is in editing status 
        /// </summary>
        /// <returns></returns>
        public bool isEditing() {
            return textBox1.Visible; 
        }
        /// <summary>
        /// whether the node movement should be refreshed, true, need refresh, false not
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool isMoveNeedRefresh(Point start, Point end) {
            int delta = DiagramUtil.ANCHOR_SIZE / 2;
            Rectangle r = new Rectangle(start.X - delta, start.Y - delta, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE);
            if (!r.Contains(end) && moveRefreshedEnd!=end) {
                moveRefreshedEnd.X = end.X;
                moveRefreshedEnd.Y = end.Y;
                return true;
            }
            return false;
        }
        #endregion methods
        #region override
        public override void Refresh() {
            this.canvas.Refresh(false);            
        }
        #endregion override 
    }
}
