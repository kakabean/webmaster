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
using System.Drawing.Drawing2D;
using WebMaster.lib;
using WebMaster.lib.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor
{
    public partial class Canvas : UserControl
    {
        #region constants 
        private static readonly int CON_STATUS_VALID = 0;
        private static readonly int CON_STATUS_ERROR = 1;
        private static readonly int CON_STATUS_WARNING = 2;        
        #endregion constants 
        #region fields
        /// <summary>
        /// diagram model of the control 
        /// </summary>
        private Diagram diagram = null;
        /// <summary>
        /// diagram node of the control
        /// </summary>
        public Diagram Diagram {
            get { return diagram; }
            set { diagram = value; }
        }
        private HashtableEx connValidationTable = new HashtableEx();
        /// <summary>
        /// a canvas has a table to maintain the connectoin validation status. 
        /// this table just maintain the invalid connection and their status. 
        /// Key = Connectoin instance, value = MsgType
        /// </summary>
        internal HashtableEx ConnValidationTable {
            get { return connValidationTable; }
        }

        private Object selectedObj = null;
        /// <summary>
        /// selected Object can be NodeView, Connection or Connector, or Canvas, selected obj will be 
        /// internal used to paint the canvas node/connectoin status, selected() method will raise some 
        /// event for outer to reflect the selection changes. 
        /// </summary>
        public Object SelectedObj {
            get { return selectedObj; }
            set {
                if (selectedObj != null && !selectedObj.Equals(value)) {
                    unSelected(selectedObj);
                    selectedObj = value;
                    selected(selectedObj);
                }
                if (value!=null && selectedObj == null) {
                    selectedObj = value;
                    selected(selectedObj);
                }
            }
        }
        private ACTION _action = ACTION.NONE;
        /// <summary>
        /// selected object on going action 
        /// </summary>
        public ACTION Action {
            get { return _action; }
            set { _action = value; }
        }
        private Point move_sp = DiagramUtil.PointNull;
        /// <summary>
        /// take effect when Action == ACTION.MOVE_NODE, start point for the moving node 
        /// </summary>
        public Point Move_startPoint {
            get { return move_sp; }
            set { move_sp = value; }
        }
        private Point move_ep = DiagramUtil.PointNull;
        /// <summary>
        /// take effect when Action == ACTION.MOVE_NODE, end point for the moving node 
        /// </summary>
        public Point Move_endPoint {
            get { return move_ep; }
            set { move_ep = value; }
        }
        /// <summary>
        /// cache list when do connection point dragging, 
        /// when a new mouse down on a connection, check if it is a new TOBE dragging point, clone
        /// the connection points, add the new point to temp list. update the ddpIndex. 
        /// 
        /// when moving evt fired, update connection with the tplist.
        /// 
        /// when mouse up fired or draw area lost focus, update the tplist into connection points
        /// clean tplist, reset ddpIndex = -1 
        /// </summary>
        private List<Point> tpList = new List<Point>(16);         
        /// <summary>
        /// drag point index in the tplist 
        /// </summary>
        protected int ddpIndex = -1;
        /// <summary>
        /// this point is used to save the moving cursor location when do node resize
        /// </summary>
        private Point resizeRefp = DiagramUtil.PointNull;
        /// <summary>
        /// this point is used to save the moving cursor location when do node resize
        /// </summary>
        public Point ResizeRefp {
            get { return resizeRefp; }
            set { resizeRefp = value; }
        }
        /// <summary>
        /// diagram manager of the flow editor
        /// </summary>
        private DiagramManager diagramManager = null;
        /// <summary>
        /// diagram manager of the flow editor, one application has only one DiagramManager
        /// </summary>
        public DiagramManager DiagramManager {
            get { return diagramManager; }
        }        
        private bool needScroll = false;
        /// <summary>
        /// update diagram manager's SelectedObject 
        /// </summary>
        /// <param name="selectedObj"></param>
        private void selected(object selectedObj) {
            NodeView nv = selectedObj as NodeView;
            if (nv != null) {
                diagramManager.SelectedObj = nv.Node.RefOp;                
                return;
            }
            Connection con = selectedObj as Connection;
            if (con != null) {
                diagramManager.SelectedObj = con.RefCon;
                return;
            }
            if (selectedObj is Canvas) {
                diagramManager.SelectedObj = this.Diagram.Proc;
                return;
            }
        }
        /// <summary>
        /// update the status when object unselected
        /// </summary>
        /// <param name="selectedObj"></param>
        private void unSelected(object selectedObj) {
            NodeView nv = selectedObj as NodeView;
            if (nv != null) {
                nv.updateModelAndView();
            }
        }
        /// <summary>
        /// this is used to mark the top left point of the canvas
        /// </summary>
        private UserControl topLeftMarker = null;
        /// <summary>
        /// this is used to mark bottom right point of the canvas can be show 
        /// </summary>
        private UserControl bottomRightMarker = null;
        /// <summary>
        /// this one is used to help to control the selected connection point or node 
        /// when changed. 
        /// </summary>
        private UserControl selectMarker = null;
        #endregion fields
        #region evt
        /// <summary>
        /// sender is canvas, data is the canvas.SelectedObj, can be NodeView, Connection or Connector, or Canvas
        /// </summary>
        public event EventHandler<CommonEventArgs> MouseRightClickEvt;
        protected virtual void OnMouseRightClickEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> mouseRightClickEvt = MouseRightClickEvt;
            if (mouseRightClickEvt != null) {
                mouseRightClickEvt(this, e);
            }
        }
        public void raiseMouseRightClickEvt(Object sender, Object obj) {
            if (obj != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnMouseRightClickEvt(evt);
            }
        }
        /// <summary>
        /// This event is used when model updated, if data is Node(Process/Operation), sentder will be NodeView,
        /// if data is Connection, sender will is the canvas. 
        /// </summary>
        public event EventHandler<CommonEventArgs> ModelUpdateEvt;
        protected virtual void OnModelUpdateEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> modelUpdateEvt = ModelUpdateEvt;
            if (modelUpdateEvt != null) {
                modelUpdateEvt(this, e);
            }
        }
        /// <summary>
        /// This event is used when model updated, if data is Node(Process/Operation), sentder will be NodeView,
        /// if data is Connection, sender will is the canvas. 
        /// </summary>
        /// <param name="sender">sender is node view(Process,Operation), or canvas (Connection) </param>
        /// <param name="obj">Node, Connection</param>
        public void raiseModelUpdateEvt(Object sender, object obj) {
            if (obj is Process || obj is Operation || obj is Connection || obj is Node) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnModelUpdateEvt(evt);
            }
        }
        #endregion evt
        public Canvas(DiagramManager diagramManager) {
            InitializeComponent();
            initData();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();
            
            this.diagramManager = diagramManager;            
        }

        private void initData() {
            Color bgcolor = Color.FromName(UIConstants.COLOR_BG_CANVAS);
            topLeftMarker = new UserControl();
            topLeftMarker.Size = new Size(2, 2);
            topLeftMarker.BackColor = bgcolor;//Color.Green;
            topLeftMarker.Location = new Point(this.ClientSize.Width-DiagramUtil.ANCHOR_SIZE,this.ClientSize.Height);
            this.Controls.Add(topLeftMarker);

            bottomRightMarker = new UserControl();
            bottomRightMarker.Size = new Size(2, 2);
            bottomRightMarker.BackColor = bgcolor;//Color.Red;
            bottomRightMarker.Location = new Point(this.ClientSize.Width, this.ClientSize.Height - DiagramUtil.ANCHOR_SIZE);
            this.Controls.Add(bottomRightMarker);            

            selectMarker = new UserControl();
            selectMarker.Size = new Size(2, 2);
            selectMarker.BackColor = bgcolor;//Color.Blue;
            selectMarker.Location = new Point(this.ClientSize.Width, this.ClientSize.Height - DiagramUtil.ANCHOR_SIZE);
            this.Controls.Add(selectMarker);
            
            // update the canvas background color 
            this.BackColor = bgcolor;
        }
        #region override methods
        /// <summary>
        /// Paints the control
        /// </summary>
        /// <remarks>
        /// If you switch the painting order of connections and shapes the connection line
        /// will be underneath/above the shape
        /// </remarks>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e) {            
            Graphics g = e.Graphics;
            SmoothingMode sm = e.Graphics.SmoothingMode;
            //use the best quality, with a performance penalty
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;            
            doPaint(g);
            e.Graphics.SmoothingMode = sm;
        }
        protected override Point ScrollToControl(Control activeControl) {
            //Log.println_graph("scrollToControl 1 = " + this.AutoScrollPosition+", need scroll = "+this.needScroll+", selected Obj = "+this.SelectedObj);
            if (this.needScroll) {
                this.needScroll = false;
                Point p = base.ScrollToControl(activeControl);
                //Log.println_graph("scrollToControl 2 = " + p);
                return p;
            } else {
                return this.AutoScrollPosition;
            }
        }
        protected override bool ProcessDialogKey(Keys keyData) {
            if (keyData == Keys.Delete) {
                DiagramUtil.delete(this,this.DiagramManager);
            }
            return base.ProcessDialogKey(keyData);
        }
        #endregion override methods
        #region paint handling
        /// <summary>
        /// this is the main paint method to paint the connection, trackers and feedback
        /// </summary>
        /// <param name="g"></param>
        private void doPaint(Graphics g) {
            printCons("paint connection ");
            printNodes();
            //transfer the coodinates for paint connections 
            g.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            // draw all connections except the selected connection, if have 
            for (int i = 0; i < Diagram.Connections.Count; i++) {
                Connection con = Diagram.Connections[i];
                if (con != SelectedObj || ddpIndex == -1) {
                    int status = getConnectionStatus(con);
                    drawConnection(g, con.Points, status);
                    int num = getConditionNumber(con);
                    drawConnectionLabel(g, con.Points, num);
                }
            }
            g.ResetTransform();
            // handle the selected connection
            Connection conn = SelectedObj as Connection;
            if (conn != null) {
                //transfer the coodinates for paint connections 
                g.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
                int status = getConnectionStatus(conn);
                if (ddpIndex != -1 && tpList != null && tpList.Count > 0) {
                    drawConnection(g, tpList, status);                    
                    drawConnTrackers(tpList, g);
                    int num = getConditionNumber(conn);
                    drawConnectionLabel(g, tpList, num);
                } else {
                    drawConnection(g, conn.Points, status);                    
                    drawConnTrackers(conn.Points, g);
                    int num = getConditionNumber(conn);
                    drawConnectionLabel(g, conn.Points, num);
                }
                g.ResetTransform();
            } else {                
                drawNodeBorderAndTrackers(this, g);
            }
        }
        /// <summary>
        /// Get condition -> ref OpCondition order in the owner collectoin. or -1 if errors
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private int getConditionNumber(Connection con) {
            if (con != null && con.RefCon != null && con.RefCon.Collection != null) {
                return con.RefCon.Collection.IndexOf(con.RefCon);
            }
            return -1;
        }
        /// <summary>
        /// draw a connection with pen 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="plist"></param>
        /// <param name="status"></param>
        public void drawConnection(Graphics g, List<Point> plist, int status) {
            if (plist == null || plist.Count <= 0) {
                return;
            }
            Pen pen1 = Pens.Black;
            if (status == CON_STATUS_ERROR) {
                pen1 = Pens.Red;
            } else if (status == CON_STATUS_WARNING) {
                pen1 = Pens.Yellow;
            }
            for (int i = 0; i < plist.Count - 2; i++) {
                g.DrawLine(pen1, plist[i], plist[i + 1]);
            }
            using (Pen cPen = new Pen(Color.Black)) {
                using (AdjustableArrowCap endCap = new System.Drawing.Drawing2D.AdjustableArrowCap(7, 7, false)) {
                    // draw the arrow line
                    cPen.CustomEndCap = endCap;
                    if (status == CON_STATUS_ERROR) {
                        cPen.Color = Color.Red;
                    } else if (status == CON_STATUS_WARNING) {
                        cPen.Color = Color.Yellow;
                    }
                    g.DrawLine(cPen, plist[plist.Count - 2], plist[plist.Count - 1]);
                }
            }
        }
        private void drawConnectionLabel(Graphics g, List<Point> plist, int labelNumber) {
            // NOTES: Because it is not good look for add so many numbers in a canvas
            //        if the flow is a bit complex, so just comments this method. 
            //if (labelNumber != -1) {
            //    Point p = DiagramUtil.getConnectionLabelPosition(plist);
            //    g.DrawString(labelNumber.ToString(), SystemFonts.DefaultFont, Brushes.Black, p);
            //    Log.println_graph("draw connection label, label = " + labelNumber + ", pos = " + p);
            //}
        }
        /// <summary>
        /// draw trackers on the point list
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="g"></param>
        public void drawConnTrackers(List<Point> ps, Graphics g) {
            int delta = DiagramUtil.ANCHOR_SIZE / 2;
            foreach (Point p in ps) {
                g.FillRectangle(Brushes.White, new Rectangle(p.X - delta, p.Y - delta, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
                g.DrawRectangle(Pens.Black, new Rectangle(p.X - delta, p.Y - delta, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
            }
        }
        private void doDrawNodeTrackers(NodeView node, Graphics g) {
            // draw trackers for node
            int delta = DiagramUtil.ANCHOR_SIZE / 2;
            // east anchor
            g.FillRectangle(Brushes.White, new Rectangle(node.Location.X + node.Width - 1, node.Location.Y + node.Height / 2 - delta, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
            g.DrawRectangle(Pens.Black, new Rectangle(node.Location.X + node.Width - 1, node.Location.Y + node.Height / 2 - delta, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
            // south east anchor
            g.FillRectangle(Brushes.White, new Rectangle(node.Location.X + node.Width - 1, node.Location.Y + node.Height - 1, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
            g.DrawRectangle(Pens.Black, new Rectangle(node.Location.X + node.Width - 1, node.Location.Y + node.Height - 1, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
            // south anchor
            g.FillRectangle(Brushes.White, new Rectangle(node.Location.X + node.Width / 2 - delta, node.Location.Y + node.Height - 1, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
            g.DrawRectangle(Pens.Black, new Rectangle(node.Location.X + node.Width / 2 - delta, node.Location.Y + node.Height - 1, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE));
        }
        /// <summary>
        /// draw node selected feedback rectangle and 3 anchors
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="g"></param>
        internal void drawNodeBorderAndTrackers(Canvas canvas, Graphics g) {
            NodeView node = canvas.SelectedObj as NodeView;
            if (node != null && canvas != null) {
                using (Pen pen = new Pen(Color.Blue)) {                    
                    // draw select feedback rectangle for node
                    pen.DashStyle = DashStyle.DashDot;
                    g.DrawRectangle(pen, node.Location.X - 2, node.Location.Y - 2, node.Width + 3, node.Height + 3);

                    // draw feedback rectangle when resize node 
                    if (canvas.ResizeRefp != null && canvas.ResizeRefp != DiagramUtil.PointNull) {
                        pen.DashStyle = DashStyle.DashDotDot;
                        if (canvas.Action == ACTION.RESIZE_EAST) {
                            g.DrawRectangle(pen, node.Location.X - 1, node.Location.Y - 1, canvas.ResizeRefp.X + this.AutoScrollPosition.X - node.Location.X + 2, node.Height + 2);
                        } else if (canvas.Action == ACTION.RESIZE_SOUTH) {
                            g.DrawRectangle(pen, node.Location.X - 1, node.Location.Y - 1, node.Width + 2, canvas.ResizeRefp.Y + this.AutoScrollPosition.Y - node.Location.Y + 2);
                        } else if (canvas.Action == ACTION.RESIZE_SOUTH_EAST) {
                            g.DrawRectangle(pen, node.Location.X - 1, node.Location.Y - 1, canvas.ResizeRefp.X + this.AutoScrollPosition.X - node.Location.X + 2, canvas.ResizeRefp.Y + this.AutoScrollPosition.Y - node.Location.Y + 2);
                        }
                    }
                    // draw feedback rectangle when move node 
                    if (canvas.Action == ACTION.MOVE_NODE) {
                        Point mp = DiagramUtil.getMoveNodePoint(canvas, node);
                        if (mp != DiagramUtil.PointNull) {
                            pen.DashStyle = DashStyle.DashDot;
                            g.DrawRectangle(pen, mp.X, mp.Y, node.Width, node.Height);
                        }
                    }
                    // draw anchors for node 
                    // start node and end node do not allowed to resize
                    if (node.Node.RefOp.OpType != OPERATION.START && node.Node.RefOp.OpType != OPERATION.END) {
                        doDrawNodeTrackers(node, g);
                    }
                }
            }
        }        
        #endregion paint handling 
        #region DnD methods 
        private void GraphControl_DragEnter(object sender, DragEventArgs e) {
            DnDData ddd = (DnDData)e.Data.GetData(typeof(DnDData));
            if (ddd == null) {
                return;
            }
            switch (ddd.DDType) {
                case DnDType.CREATE_NODE:
                case DnDType.COPY:
                    e.Effect = DragDropEffects.Copy;
                    break;
                case DnDType.NONE:
                    e.Effect = DragDropEffects.None;
                    break;
            }
        }
        /// <summary>
        /// this method is used to handle the drag/drop scenario, it is that: press the tool entry, then keep
        /// mouse pressed, then move the mouse onto canvas and then release mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphControl_DragDrop(object sender, DragEventArgs e) {
            DnDData ddd = (DnDData)e.Data.GetData(typeof(DnDData));
            switch (ddd.DDType) {
                case DnDType.CREATE_NODE:
                    NodeView node = ddCreateNode(ddd.Data, this.PointToClient(new Point(e.X, e.Y)));
                    SelectedObj = node;
                    DiagramManager.resetActiveToolStatus();
                    this.Refresh(false);
                    break;

            }
        }
        #endregion Dnd method 
        #region graph event
        private void GraphControl_MouseDown(object sender, MouseEventArgs e) {
            Point ep = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);
            Log.println_graph("-- Graph control. mouse down. cursor position = " + e.Location + ", updated ep = "+ep+", e.button=" + e.Button + ", scroll=" + this.AutoScrollPosition);
            // create a new Node, it is for such scenario : click the tool entry and then move the mouse onto canva, then click. 
            if (diagramManager.ActiveTool != null) {
                // release the DnD status if right clicked. 
                if (e.Button == System.Windows.Forms.MouseButtons.Right || DiagramManager.ActiveTool.Tag == null) {
                    diagramManager.resetActiveToolStatus();                    
                }else if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                    // just the left button is used to create a new node. 
                    OPERATION optype = (OPERATION)DiagramManager.ActiveTool.Tag;
                    NodeView node = ddCreateNode(optype, new Point(e.X, e.Y));
                    SelectedObj = node;
                    diagramManager.resetActiveToolStatus();
                    // doesn't need to reset the status, just update the selectedObj will be ok for refresh
                    this.Refresh(false);
                }
            } else {
                // update current selectedObj status if it is a NodeView previously selected, 
                // if the node is in edit status, set it with edit finished status. 
                NodeView tn = SelectedObj as NodeView;
                if (tn != null) {
                    tn.updateModelAndView();
                }
                // check the newly to be selected object 
                Object obj = DiagramUtil.getSelectedObject(this, ep,this.AutoScrollPosition);                
                // handle node anchor press down                
                if (obj is NodeView) {
                    SelectedObj = obj as NodeView;
                    ACTION at = DiagramUtil.getProperAnchorType(obj as NodeView, e.Location);
                    if (at != ACTION.NONE) {
                        this.Action = at;
                        if (this.Action == ACTION.RESIZE_EAST) {
                            this.Cursor = Cursors.SizeWE;
                        } else if (this.Action == ACTION.RESIZE_SOUTH) {
                            this.Cursor = Cursors.SizeNS;
                        } else if (this.Action == ACTION.RESIZE_SOUTH_EAST) {
                            this.Cursor = Cursors.SizeNWSE;
                        }
                    }
                } else if (obj is Connector) {
                    Connector ctr = obj as Connector;
                    // Make sure only the To connector can be moved 
                    if (ctr.Connection.To == ctr) {
                        SelectedObj = ctr;
                        this.Action = ACTION.MOVE_CTR;
                        this.Cursor = Cursors.Hand;
                        DnDData ddd = new DnDData();
                        ddd.DDType = DnDType.MOVE_CTR;
                        ddd.Data = ctr;
                        this.DoDragDrop(ddd, DragDropEffects.Link);
                    } else {
                        // handle connection press down
                        Connection con = ctr.Connection;
                        SelectedObj = con;
                        ddpIndex = DiagramUtil.updateTmpList(con, tpList, ep);
                        if (ddpIndex != -1) {
                            this.Cursor = Cursors.SizeAll;
                        }
                    }
                } else if (obj is Connection) {
                    // handle connection press down
                    Connection con = obj as Connection;
                    SelectedObj = con;
                    ddpIndex = DiagramUtil.updateTmpList(con, tpList, ep);
                    if (ddpIndex != -1) {
                        this.Cursor = Cursors.SizeAll;
                    }
                } else {
                    // handle diagram press down
                    SelectedObj = this;
                    this.Cursor = Cursors.Default;
                }
                
            }

            if (e.Button == MouseButtons.Right && SelectedObj is Connection) {
                raiseMouseRightClickEvt(this, SelectedObj);
            }
        }
        private void GraphControl_MouseMove(object sender, MouseEventArgs e) {
            Point ep = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);
            //Log.println_graph("Mouse move = "+e.Location + ", scroll = "+this.AutoScrollPosition);
            // update cursor style when mouse move 
            updateCursorStyle(ep.X,ep.Y);
            // do mouse move action 
            if (diagramManager.ActiveTool == null && e.Button == MouseButtons.Left) {                
                if(SelectedObj is Connection){
                    Connection con = SelectedObj as Connection;
                    if (ddpIndex != -1) {
                        this.Cursor = Cursors.SizeAll;
                        if (doConnectionMove(ep)) {
                            this.Refresh(false);
                        }
                    }
                } else if (SelectedObj is NodeView) {
                    NodeView node = SelectedObj as NodeView;
                    // update resize node feedback layer 
                    if (this.Action == ACTION.RESIZE_EAST || this.Action == ACTION.RESIZE_SOUTH || this.Action == ACTION.RESIZE_SOUTH_EAST) {
                        // first point initial 
                        if (ResizeRefp == DiagramUtil.PointNull
                             || (ep.X > ResizeRefp.X + 1 || ep.X < ResizeRefp.X - 1 || ep.Y > ResizeRefp.Y + 1 || ep.Y < ResizeRefp.Y - 1)) {
                            ResizeRefp = new Point(ep.X, ep.Y);
                            Refresh(false);
                        }                        
                    } else if (this.Action == ACTION.MOVE_NODE) {
                        // update move node feedback layer
                        this.Move_endPoint = e.Location;
                        Log.println_graph("Mouse move, endPoint = "+this.Move_endPoint);
                        Refresh(false);
                    }

                }
            }
        }
        private void GraphControl_MouseUp(object sender, MouseEventArgs e) {
            if (diagramManager.ActiveTool == null) {
                Connection con = SelectedObj as Connection;
                if (con != null) {
                    // This is used to handle some potential risks
                    if (tpList.Count == 0) {
                        resetStatus();
                        this.Refresh(false);
                        return;
                    }
                    Point tp = tpList[ddpIndex];
                    DiagramUtil.cleanDuplicatePoints(tpList,ddpIndex);
                    bool changed = DiagramUtil.isConnPointsChanged(con,tpList);
                    if (changed) {
                        con.Points.Clear();
                        con.Points.AddRange(tpList);
                        printCons("Canvas Mouse up, auto scroll = " + this.AutoScrollPosition);
                        // update the marker 
                        updateMarkers();
                        // scroll point to view if need 
                        if (con.Points.Contains(tp)) {
                            this.adjustSelectMarker(tp);
                        }
                    }
                    resetStatus();
                    this.Refresh(false);
                    
                    if (changed) {
                        this.raiseModelUpdateEvt(this, con);
                    }
                } else {
                    NodeView node = SelectedObj as NodeView;
                    if (node != null) {
                        // handle resize node 
                        if (this.Action == ACTION.RESIZE_EAST || this.Action == ACTION.RESIZE_SOUTH || this.Action == ACTION.RESIZE_SOUTH_EAST) {
                            resizeNode(node, e.Location);
                            ddpIndex = -1;
                            this.Action = ACTION.NONE;
                            this.Refresh(false);
                            this.raiseModelUpdateEvt(node, node.Node);
                        } else if (this.Action == ACTION.MOVE_NODE) {
                            // handle move node 
                            moveNode();
                            ddpIndex = -1;
                            this.Action = ACTION.NONE;
                            this.Refresh(false);
                            this.raiseModelUpdateEvt(node, node.Node);
                        }
                    } else {
                        SelectedObj = null;
                        ddpIndex = -1;
                        this.Action = ACTION.NONE;
                        this.Refresh(false);
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }
        /// <summary>
        /// update the top left marker and bottom right marker when conneciton/Node add/removed or connection changed. 
        /// </summary>
        internal void updateMarkers() {
            Point[] ps = DiagramUtil.getXYOAjustment(null, this, Point.Empty);
            Point plt = new Point(ps[0].X + this.AutoScrollPosition.X - ps[2].X, ps[0].Y + this.AutoScrollPosition.Y-ps[2].Y);
            this.topLeftMarker.Location = plt;
            Point brp = new Point(ps[1].X + DiagramUtil.ANCHOR_SIZE + this.AutoScrollPosition.X-ps[2].X, ps[1].Y + DiagramUtil.ANCHOR_SIZE + this.AutoScrollPosition.Y-ps[2].Y);
            this.bottomRightMarker.Location = brp;

        }
        /// <summary>
        /// this method is used to update the select marker at a proper position, e.g move node, 
        /// move connection point, create node, delete node, delete connection. 
        /// if just want to adjust the select marker, set nodeview as null. 
        /// <param name="nodeview">node view </param>
        /// </summary>
        internal void adjustSelectMarker(NodeView nodeview) {
            // justify selected marker
            if (nodeview == null) {
                int x = this.selectMarker.Location.X;
                int y = this.selectMarker.Location.Y;
                if (x > bottomRightMarker.Location.X) {
                    x = bottomRightMarker.Location.X;
                }
                if (y > bottomRightMarker.Location.Y) {
                    y = bottomRightMarker.Location.Y;
                }
                selectMarker.Location = new Point(x, y);
            } else {
                scrollNodeToView(nodeview);
            }
        }
        /// <summary>
        /// this method is used to update the select marker at a proper position, e.g move node, 
        /// move connection point, create node, delete node, delete connection. 
        /// If just adjust the selectMarker, set point as DiagramUtil.PointNull. 
        /// 
        /// <param name="point">connection point, If just adjust the selectMarker, set point as DiagramUtil.PointNull.</param>
        /// </summary>
        internal void adjustSelectMarker(Point point) {
            // justify selected marker
            if (point == DiagramUtil.PointNull) {
                int x = this.selectMarker.Location.X;
                int y = this.selectMarker.Location.Y;
                if (x > bottomRightMarker.Location.X) {
                    x = bottomRightMarker.Location.X;                
                }
                if (y > bottomRightMarker.Location.Y) {
                    y = bottomRightMarker.Location.Y;
                }
                selectMarker.Location = new Point(x, y);
            } else {
                scrollConnectionPointToView(point);
            }
        }
        private void GraphControl_Scroll(object sender, ScrollEventArgs e) {
            Log.println_graph("canvas scrolled , paiting...");
            this.Refresh(false);
        }
        private void GraphControl_Resize(object sender, EventArgs e) {
            Log.println_graph("canvas resized , paiting...");
            this.Refresh(false);
        }
 
        #endregion graph event 
        #region common methods 
        /// <summary>
        /// it is used to get the connection validation status, return CON_STATUS_VALID as default. 
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private int getConnectionStatus(Connection con) {
            object oenum = this.ConnValidationTable.Get(con);
            if (oenum == null || !Enum.IsDefined(typeof(MsgType), oenum)) {
                return CON_STATUS_VALID;
            }

            MsgType type = (MsgType)oenum;
            if (type == MsgType.ERROR) {
                return CON_STATUS_ERROR;
            } else if (type == MsgType.WARNING) {
                return CON_STATUS_WARNING;
            }
            return CON_STATUS_VALID;
        }
        /// <summary>
        /// update cursor style when mouse move on the draw area 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void updateCursorStyle(int x, int y) {
            if (diagramManager.ActiveTool != null){
                if (diagramManager.ActiveCanvas.Action != ACTION.LINK) {
                    this.Cursor = Cursors.PanNW;
                    return;
                } else {
                    this.Cursor = Cursors.No;
                    return;
                }
            } else {                
                Point p = new Point(x, y);                
                // check connections 
                foreach (Connection con in Diagram.Connections) {
                    if (DiagramUtil.hitConnection(con, p) != -1) {
                        // check connectors first 
                        if (DiagramUtil.hitConnector(con, p) != null) {
                            this.Cursor = Cursors.Hand;
                        } else {
                            this.Cursor = Cursors.SizeAll;
                        }
                        return;
                    }
                }
                // check node anchor point 
                NodeView node = SelectedObj as NodeView ;
                if (node != null) {
                    ACTION at = this.Action;
                    if (at == ACTION.NONE) {
                        at = DiagramUtil.getProperAnchorType(node, new Point(x+this.AutoScrollPosition.X,y+this.AutoScrollPosition.Y));
                    }
                    if (at == ACTION.RESIZE_EAST) {
                        this.Cursor = Cursors.SizeWE;
                        return;
                    } else if (at == ACTION.RESIZE_SOUTH) {
                        this.Cursor = Cursors.SizeNS;
                        return;
                    } else if (at == ACTION.RESIZE_SOUTH_EAST) {
                        this.Cursor = Cursors.SizeNWSE;
                        return;
                    }
                }
               
               this.Cursor = Cursors.Default;               
            }
        }
        ///// <summary>
        ///// the event point is less then 5 pixels will be a valid connector position. 
        ///// true: means the connector updated, false, means nothing changed. 
        ///// </summary>
        ///// <param name="ep"></param>
        //private bool doConnectorMove(Point ep) {
        //    Connector ctr = SelectedObj as Connector;
        //    if (ctr == null) {
        //        return false;
        //    }
        //    Point np = formatConnectionPoint(ep);
        //    int index = -1;
        //    if (ctr.Equals(ctr.Connection.From)) {
        //        index = ctr.Connection.Points.IndexOf(ctr.Location);
        //    } else {
        //        index = ctr.Connection.Points.LastIndexOf(ctr.Location);
        //    }
        //    Log.println_graph("do Connector move, ep = " + ep + ", np = " + np + ", connector location = " + ctr.Location + ", To = " + ctr.Connection.Points[ctr.Connection.Points.Count - 1]);
        //    if (index != -1) {
        //        // just make sure the move distance is bigger than 3 pixels, the canvas will refresh.
        //        Rectangle rect = new Rectangle(ctr.Location, new Size(6, 6));
        //        Rectangle r1 = new Rectangle(ctr.Location, new Size(3, 3));
        //        if (rect.Contains(np) && !r1.Contains(np)) {
        //            int x = ctr.Location.X;
        //            int y = ctr.Location.Y;
        //            Point p1 = ctr.RefNode.Location ; // node left top point
        //            // node bottom right point 
        //            Point p2 = new Point(ctr.RefNode.Location.X + ctr.RefNode.Size.Width, ctr.RefNode.Location.Y + ctr.RefNode.Size.Height);
        //            if (x == p1.X || x == p2.X) {
        //                if (y >= p1.Y && y <= p2.Y) {
        //                    y = np.Y - this.AutoScrollPosition.Y;
        //                } else {
        //                    return false;
        //                }
        //            } else {
        //                if (x >= p1.X && x <= p2.X) {
        //                    x = np.X - this.AutoScrollPosition.X;
        //                } else {
        //                    return false;
        //                }
        //            }

        //            Point nloc = new Point(x, y);
        //            ctr.Location = nloc;
        //            ctr.Connection.updateEnds();
                    
        //            Log.println_graph("connector moved to new location , p = ("+nloc+", To = "+ctr.Connection.Points[ctr.Connection.Points.Count-1]);
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        /// <summary>
        /// if the connection updated return true, else return fasle 
        /// </summary>
        /// <param name="ep"></param>
        private bool doConnectionMove(Point ep) {
            if (ddpIndex < 0 || ddpIndex >= tpList.Count) {
                return false;
            }

            ep = formatConnectionPoint(ep);
            Point dp = tpList[ddpIndex];
            Rectangle r = new Rectangle(dp.X - 2, dp.Y - 2, 4, 4);
            if (r.Contains(ep)) {
                return false;
            }
            Point tp = new Point(ep.X, ep.Y);
            Connection con = SelectedObj as Connection;
            if (con != null) {
                if (ddpIndex == 0) { // move from anchor point 
                    Point p = DiagramUtil.getProperAnchorPoint(con.From.RefNode, tp, tpList[1],this.AutoScrollPosition);                    
                    tp = new Point(p.X, p.Y);
                } else if (ddpIndex == tpList.Count - 1) { // move to anchor point 
                    Point p = DiagramUtil.getProperAnchorPoint(con.To.RefNode, tp,tpList[tpList.Count-2],this.AutoScrollPosition);
                    tp = new Point(p.X, p.Y);
                }
                if (tpList.Count > 2) {
                    if (ddpIndex == 1) { // from anchor point should be updated based on the moving point
                        Point p = DiagramUtil.getProperAnchorPoint(con.From.RefNode, tp,this.AutoScrollPosition);
                        tpList.RemoveAt(0);
                        tpList.Insert(0, p);
                    }
                    if (ddpIndex == tpList.Count - 2) { // to anchor point should be updated based on the moving point
                        Point p = DiagramUtil.getProperAnchorPoint(con.To.RefNode, tp,this.AutoScrollPosition);
                        tpList.Add(p);
                        tpList.RemoveAt(tpList.Count-2);
                    }
                }
            }
            if (tp != DiagramUtil.PointNull) {
                if (ddpIndex == tpList.Count - 1) {
                    tpList.Add(tp);
                } else {
                    tpList.Insert(ddpIndex + 1, tp);
                }
                tpList.RemoveAt(ddpIndex);
                return true;
            }
            return false;
        }
        /// <summary>
        /// make sure the connection point is in the canvas can-be visible area
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        private Point formatConnectionPoint(Point ep) {
            int x = ep.X;
            int y = ep.Y;
            if (ep.X <= 0) {
                x = 0 + DiagramUtil.CANVAS_PADDING;
            }
            if (this.HorizontalScroll.Visible) {
                if (ep.X >= this.HorizontalScroll.Maximum) {
                    x = this.HorizontalScroll.Maximum - DiagramUtil.CANVAS_PADDING;
                }
            } else {
                if (ep.X >= this.ClientSize.Width) {
                    x = this.ClientSize.Width - DiagramUtil.CANVAS_PADDING;
                }
            }
            if (ep.Y <= 0) {
                y = 0 + DiagramUtil.CANVAS_PADDING;
            }
            if (this.VerticalScroll.Visible) {
                if (ep.Y >= this.VerticalScroll.Maximum) {
                    y = this.VerticalScroll.Maximum - DiagramUtil.CANVAS_PADDING;
                }
            } else {
                if (ep.Y >= this.ClientSize.Height) {
                    y = this.ClientSize.Height - DiagramUtil.CANVAS_PADDING;
                }
            }
            
            return new Point(x,y);
        }
        /// <summary>
        /// update connections points when scroll
        /// </summary>
        /// <param name="needRefresh"></param>
        private void doScroll() {
        }
        public void resizeNode(NodeView node, Point p) {
            if (node == null) {
                return;
            }
            // start node and end node do not allowed to resize
            if (node.Node.RefOp.OpType == OPERATION.START || node.Node.RefOp.OpType == OPERATION.END) {
                return;
            }
            doResizeNode(node, p);
        }
        private void doResizeNode(NodeView node, Point p) {            
            if (this.Action == ACTION.RESIZE_EAST) {
                int w = node.Width + (p.X - node.Location.X - node.Width);
                w = w < NodeView.MIN_WIDTH ? NodeView.MIN_WIDTH : w;
                node.Width = w;
            } else if (this.Action == ACTION.RESIZE_SOUTH) {
                int h = node.Height + (p.Y - node.Location.Y - node.Height);
                h = h < NodeView.MIN_HEIGHT ? NodeView.MIN_HEIGHT : h;
                node.Height = h;
            } else if (this.Action == ACTION.RESIZE_SOUTH_EAST) {
                int w = node.Width + (p.X - node.Location.X - node.Width);
                w = w < NodeView.MIN_WIDTH ? NodeView.MIN_WIDTH : w;
                int h = node.Height + (p.Y - node.Location.Y - node.Height);
                h = h < NodeView.MIN_HEIGHT ? NodeView.MIN_HEIGHT : h;
                node.Size = new Size(w, h);
            }
            updateMarkers();
            this.adjustSelectMarker(node);
        }
        /// <summary>
        /// move node to the new location 
        /// </summary>
        public void moveNode() {
            NodeView node = SelectedObj as NodeView;
            if (node != null) {
                if (this.Move_startPoint.X < this.Move_endPoint.X + DiagramUtil.MOVE_NODE_MIN && this.Move_startPoint.X > this.Move_endPoint.X - DiagramUtil.MOVE_NODE_MIN
                    && this.Move_startPoint.Y < this.Move_endPoint.Y + DiagramUtil.MOVE_NODE_MIN && this.Move_startPoint.Y > this.Move_endPoint.Y - DiagramUtil.MOVE_NODE_MIN) {
                        adjustSelectMarker(node);                        
                } else {
                    doMoveNode(node, this.Move_startPoint, this.Move_endPoint);
                }
            }
        }
        private void doMoveNode(NodeView node, Point startPoint, Point endPoint) {
            int deltaX = endPoint.X - startPoint.X;
            int deltaY = endPoint.Y - startPoint.Y;
            int x = node.Location.X + deltaX;
            int y = node.Location.Y + deltaY;
            Log.println_graph("Move on canvas, startP =" + startPoint + "endP= " + endPoint + ",e = " + endPoint+", node = " + node.Location + ", scroll = " + this.AutoScrollPosition + ", new location = (" + x + "," + y + ")");

            if (endPoint.X >= startPoint.X + DiagramUtil.MOVE_NODE_MIN || endPoint.X <= startPoint.X - DiagramUtil.MOVE_NODE_MIN || endPoint.Y >= startPoint.Y + DiagramUtil.MOVE_NODE_MIN || endPoint.Y <= startPoint.Y - DiagramUtil.MOVE_NODE_MIN) {
                Point[] ps = DiagramUtil.getXYOAjustment(node,this,new Point(x-this.AutoScrollPosition.X,y-this.AutoScrollPosition.Y));

                Log.println_graph("top left = "+ps[0]+", bottom right = "+ps[1]+", delta = "+ps[2]+", size = "+ps[3]);

                //Log.println_graph("delta xy=" + ps[2] +", size"+ps[3] + ",scroll=" + this.AutoScrollPosition + ", node = " + node.Location + ", client size=" + this.ClientSize + ", minSize = " + this.AutoScrollMinSize);
                node.Location = new Point(x, y);
                Point plt = new Point(ps[0].X + this.AutoScrollPosition.X, ps[0].Y + this.AutoScrollPosition.Y);                
                this.topLeftMarker.Location = plt;
                Point brp = new Point(ps[1].X + DiagramUtil.ANCHOR_SIZE+this.AutoScrollPosition.X, ps[1].Y + DiagramUtil.ANCHOR_SIZE+this.AutoScrollPosition.Y);
                
                this.bottomRightMarker.Location = brp;

                Log.println_graph("after update location : node = "+node.Location+", scroll = "+this.AutoScrollPosition+", dxdy = "+ps[2]);
                printNodes();
                updateClientSize(ps[2].X, ps[2].Y);
                Log.println_graph("after update client size : node = " + node.Location + ", scroll = " + this.AutoScrollPosition+", dxdy = "+ps[2]);
                // this only for debug 
                if (this.topLeftMarker.Location.X - this.AutoScrollPosition.X < 0 || this.topLeftMarker.Location.Y - this.AutoScrollPosition.Y < 0
                    || (this.HScroll && this.bottomRightMarker.Location.X - this.AutoScrollPosition.X > this.HorizontalScroll.Maximum)
                    || (this.VScroll && this.bottomRightMarker.Location.Y - this.AutoScrollPosition.Y > this.VerticalScroll.Maximum)) {
                        Log.println_graph("drag move node errors ..............................................");
                }
                //printCons("1");
                adjustSelectMarker(node);
                printNodes();
                // update moved nodes anchors location
                Log.println_graph("drag node position = " + node.Location + ", scroll = " + this.AutoScrollPosition);
                DiagramUtil.updateConnectors(node,this.AutoScrollPosition);                
                //printCons("2");
                Log.println_graph("after scroll : Move node delta xy=" + ps[2]+ ",scroll=" + this.AutoScrollPosition + ", new node position = " + node.Location + ", client size=" + this.ClientSize + ", minSize = " + this.AutoScrollMinSize + ", H max=" + this.HorizontalScroll.Maximum);
                
                this.raiseModelUpdateEvt(node, node.Node);
                Log.println_graph("After  : Move node ,autoscroll=" + this.AutoScrollPosition + ", client size=" + this.ClientSize + ", minSize = " + this.AutoScrollMinSize + ", H max=" + this.HorizontalScroll.Maximum);                
            }
        }
        private void printNodes() {
            Log.println_graph(" Nodes : -- autoscroll position = " + this.AutoScrollPosition + "\n  ");
            foreach (Control uc in this.Controls) {
                //if (uc is NodeView) {
                  //  NodeView n = uc as NodeView;
                    Log.println_graph(" , " + uc.Location);
                //}
            }
        }
        /// <summary>
        /// update canvas elements location with dx, dy, dx>0 means that the absolute coordinate o
        /// should be moved X+/Right direction, o moved left and means that the connections moved right. 
        /// </summary>
        /// <param name="dx"> means how many pixels the Xo point will be moved</param>
        /// <param name="dy"> means how many pixels the Yo will be moved </param>
        private void updateClientSize(int dx, int dy) {
            if (dx == 0 && dy == 0) {
                return;
            }
            foreach (Connection con in this.Diagram.Connections) {
                int count = con.Points.Count;
                for (int i = 0; i < count; i++) {
                    Point np = new Point(con.Points[0].X - dx, con.Points[0].Y - dy);
                    con.Points.RemoveAt(0);
                    con.Points.Add(np);
                }
                if (count >= 2) {
                    // update the connector location. 
                    con.From.Location = con.Points[0];
                    con.To.Location = con.Points[con.Points.Count - 1];
                }
                //Log.println_graph("update size, " + this.AutoScrollPosition + ", connection = " + con.ToString());
            }
            foreach (Control c in this.Controls) {
                c.Location = new Point(c.Location.X - dx, c.Location.Y - dy);
            }
        }
        /// <summary>
        /// create a new node view and add it into the canvas control 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private NodeView ddCreateNode(object data, Point location) {
            Log.println_graph("ddCreateNode, location = "+location);
            NodeView node = DiagramUtil.createNode((OPERATION)data, location, this, diagramManager);
            this.Controls.Add(node);
            this.raiseModelUpdateEvt(node, node.Node);
            // update canvas marker 
            updateMarkers();
            adjustSelectMarker(node);
            return node;
        }
         /// <summary>
        /// It is used for control the scroll bar connection point position changed. 
        /// First it will check whether the point is near the left/right border, near means distance less than ANCHOR_SIZE,
        /// if so, set the marker point as (x,selectMarkerPoint.y), and set marker
        /// if not, check the top/bottom border and find proper point, and set marker, if not, do nothing
        /// </summary>
        /// <param name="selectMarkerPoint">it should be the marker's real position in the canvas, include the scroll info</param>
        private void scrollConnectionPointToView(Point selectMarkerPoint) {
            int x = int.MinValue;
            int y = int.MinValue;
            int delta = DiagramUtil.ANCHOR_SIZE;
            Point rp = new Point(x, y);

            // check left border distance 
            int tx = 0 - this.AutoScrollPosition.X;
            if (selectMarkerPoint.X - tx < delta) {
                x = selectMarkerPoint.X - delta; // the maker is near the left canvas border 
            }
            // check the right border distance
            if (x == int.MinValue) {
                tx = this.ClientSize.Width - this.AutoScrollPosition.X; // right border 
                if (selectMarkerPoint.X - tx > 0 - delta) {
                    x = selectMarkerPoint.X + delta; // the marker is near the right canvas border 
                }
            }
            // proper x value find 
            if (x != int.MinValue) {
                rp.X = x;
                rp.Y = selectMarkerPoint.Y;
            } else {
                // check the top border distance 
                int ty = 0 - this.AutoScrollPosition.Y;
                if (selectMarkerPoint.Y - ty < delta) {
                    y = selectMarkerPoint.Y - delta;
                }
                // check the bottom border distance 
                if (y == int.MinValue) {
                    ty = this.ClientSize.Height - this.AutoScrollPosition.Y;
                    if (selectMarkerPoint.Y - ty > 0 - delta) {
                        y = selectMarkerPoint.Y + delta;
                    }
                }
                if (y != int.MinValue) {
                    rp.X = selectMarkerPoint.X;
                    rp.Y = y;
                }
            }
            Log.println_graph("scroll connection, p = "+rp+", scroll = "+this.AutoScrollPosition);
            // scroll marker 
            if (rp.X != int.MinValue && rp.Y != int.MinValue) {
                rp.X += this.AutoScrollPosition.X;
                rp.Y += this.AutoScrollPosition.Y;
                selectMarker.Location = rp;

                needScroll = true;
                ScrollControlIntoView(selectMarker);
            }

        }
        /// <summary>
        /// It is used for control the scroll bar node view changed. if it is out of visual scope, use the bottom right point 
        /// as the scroll to point        
        /// </summary>
        /// <param name="node"></param>
        private void scrollNodeToView(NodeView nodeview) {            
            if (nodeview == null) {
                return;
            }
            Log.println_graph("scrollNodeToView, scroll = "+true);            

            int x = nodeview.Location.X + nodeview.Width + DiagramUtil.ANCHOR_SIZE;
            int y = nodeview.Location.Y + nodeview.Height + DiagramUtil.ANCHOR_SIZE;
            x = x == selectMarker.Location.X ? x - 1 : x;
            y = y == selectMarker.Location.Y ? y - 1 : y;
            this.selectMarker.Location = new Point(x, y);
            Log.println_graph("scroll to View, selector = "+this.selectMarker.Location+", canvas size = "+this.ClientSize+", scroll 1 = "+this.AutoScrollPosition+", offset = "+this.AutoScrollOffset);
            
            this.needScroll = true;
            this.ScrollControlIntoView(selectMarker);           

            Log.println_graph("scroll 2 = " + this.AutoScrollPosition + ", offset = " + this.AutoScrollOffset);
        }
        /// <summary>
        /// reset canvas all monitors status, cursor, but not refresh 
        /// </summary>
        public void resetStatus() {
            // reset strip tool button
            this.diagramManager.resetActiveToolStatus();
            // reset moving node status 
            if (this.Action != ACTION.NONE) {
                this.Action = ACTION.NONE;
            }
            if (this.Move_startPoint != DiagramUtil.PointNull) {
                this.Move_startPoint = DiagramUtil.PointNull;
            }
            if (this.Move_endPoint != DiagramUtil.PointNull) {
                this.Move_endPoint = DiagramUtil.PointNull;
            }
            if (this.tpList.Count > 0) {
                this.tpList.Clear();
            }
            //this.ddpIndex = -1;
            if (ResizeRefp != DiagramUtil.PointNull) {
                this.ResizeRefp = DiagramUtil.PointNull;
            }
                     
            this.Cursor = Cursors.Default;
        }
        /// <summary>
        /// clean the selected Object status if have 
        /// </summary>
        internal void cleanSelectStatus() {
            this.SelectedObj = null;
            this.tpList.Clear();
            this.ddpIndex = -1;
            this.Refresh(false);
        }
        public void printCons(String msg) {
            Log.println_graph("--------- " + msg + " --------------");
            foreach (Connection con in this.diagram.Connections) {
                Log.println_graph("  " + this.AutoScrollPosition + ", connection = " + con.ToString());
            }
        }

        public void Refresh(bool validateChildren) {
            if (validateChildren) {
                this.Refresh();
            } else {
                Invalidate(false);
            }
        }
        public bool HasHScroll() {
            return HScroll;
        }
        public bool HashVScroll() {
            return VScroll;
        }
        #endregion common methods
               
    }        
    /// <summary>
    /// selected object action 
    /// </summary>
    public enum ACTION
    {
        NONE,
        RESIZE_EAST,
        RESIZE_SOUTH_EAST,
        RESIZE_SOUTH,
        MOVE_CONN,
        MOVE_NODE,
        // move connector
        MOVE_CTR,
        /// <summary>
        /// just used to handle link in the diagram, internal use
        /// </summary>
        LINK
    }
}
