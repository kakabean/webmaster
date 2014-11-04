using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using WebMaster.lib.engine;

namespace WebMaster.lib.engine
{
    #region basic data
    [Serializable]
    public class DnDData
    {
        private DnDType ddType = DnDType.NONE;
        /// <summary>
        /// operation type, 
        /// </summary>
        public DnDType DDType {
            get { return ddType;}
            set { ddType = value; }
        }
        private Object data;
        /// <summary>
        /// drag drop data, its type depends on the ddType
        /// e.g if ddType = CREATE, data is an OPERATION
        /// if ddType = MOVE_NODE, data is an Node
        /// </summary>
        public Object Data {
            get { return data; }
            set { data = value; }
        }
        private Object appendix = null;
        /// <summary>
        /// appendix info, it is used to do some extension. 
        /// e.g a start point when do Node move
        /// </summary>
        public Object Appendix {
            get { return appendix; }
            set { appendix = value; }
        }
    }
    [Serializable]
    public enum DnDType
    {
        NONE,
        /// <summary>
        /// data is a OPERATION type
        /// graphcal control will response this type
        /// </summary>
        CREATE_NODE,
        /// <summary>
        /// data is connection from node 
        /// Node and graph control will response this type
        /// </summary>
        CREATE_CONN,
        /// <summary>
        /// data is node that be moved, 
        /// graph control reponse this type
        /// </summary>
        MOVE_NODE,
        /// <summary>
        /// data is connector that be moved
        /// NodeView is will handle this type
        /// </summary>
        MOVE_CTR, 
        /// <summary>
        /// data is list with {from Node},{ToNode},{anchor point}
        /// graph control reponse this type
        /// </summary>
        MOVE_CONNECTION,
        /// <summary>
        /// data is the Node, graph control response this type
        /// </summary>
        RESIZE_NODE,
        /// <summary>
        /// data is a WebElement
        /// Node response this type 
        /// </summary>
        LINK_ELEMENT,
        /// <summary>
        /// data is a Node
        /// Graph control response this type 
        /// it is used to copy an existed Node and relative script model
        /// for reuse
        /// </summary>
        COPY
    }
    #endregion basic data
    #region collections
    [Serializable]
    public class Nodes : CollectionBase
    {
        public Nodes() { }

        public int Add(Node node) {
            return this.InnerList.Add(node);
        }

        public Node this[int index] {
            get { return this.InnerList[index] as Node; }
        }
        public void Remove(Node node) {
            this.InnerList.Remove(node);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (Node node in this) {
                sb.Append("\n  ").Append(node.ToString());
            }
            return sb.ToString();
        }
    }
    [Serializable]
    public class Connections : CollectionBase
    {
        public Connections() {
        }
        public int Add(Connection con) {
            return this.InnerList.Add(con);
        }
        public Connection this[int index] {
            get { return this.InnerList[index] as Connection; }
        }
        public void Remove(Connection con) {
            this.InnerList.Remove(con);
        }
        public int indexOf(Connection con) {
            return this.InnerList.IndexOf(con);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (Connection con in this) {
                sb.Append("\n  ").Append(con.ToString());
            }
            return sb.ToString();
        }
    }
    [Serializable]
    public class Connectors : CollectionBase
    {
        public Connectors() {

        }
        public int Add(Connector con) {
            return this.InnerList.Add(con);
        }
        public Connector this[int index] {
            get { return this.InnerList[index] as Connector; }
        }
        public void Remove(Connector c) {            
            this.InnerList.Remove(c);
        }
    }
    [Serializable]
    public class Diagrams : CollectionBase
    {
        public Diagrams() {
        }
        public int Add(Diagram diagram) {
            return this.InnerList.Add(diagram);
        }
        public Diagram this[int index] {
            get { return this.InnerList[index] as Diagram; }
        }
        public void Remove(Diagram diagram) {
            this.InnerList.Remove(diagram);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n========== diagrams ===============\n");
            foreach (Diagram diagram in this) {
                sb.Append(diagram.ToString());
                sb.Append("\n");
            }
            sb.Append("\n=========== end diagrams ==========\n");
            return sb.ToString();
        }
    }
    #endregion collections
    #region view model data
    [Serializable]
    public class Connection {
        public Connection(Point from, Point to) {
            this.from = new Connector(from);
            this.from.Connection = this;
            this.to = new Connector(to);
            this.to.Connection = this;
            this.points.Add(from);
            this.points.Add(to);
        }
        private List<Point> points = new List<Point>();
        /// <summary>
        /// all points of the points that indicate the connection, from connector is the first element
        /// while the To connector is the last element.
        /// </summary>
        public List<Point> Points {
            get { return points; }
            set { points = value; }
        }
        private Connector from;
        /// <summary>
        /// from point
        /// </summary>
        public Connector From {
            get { return from; }            
        }
        private Connector to;
        /// <summary>
        /// To point
        /// </summary>
        public Connector To {
            get { return to; }
        }
        private OpCondition refOpCon = null;
        /// <summary>
        /// referenced OpCondition 
        /// </summary>
        public OpCondition RefCon {
            get { return refOpCon; }
            set { refOpCon = value; }
        }
        /// <summary>
        /// update the from and to points in the points list if needed
        /// </summary>
        public void updateEnds() {            
            points.RemoveAt(0);
            points.RemoveAt(points.Count - 1);
            points.Insert(0, new Point(from.Location.X, from.Location.Y));
            points.Add(new Point(to.Location.X,to.Location.Y));
        }
        public void addInterPoint(Point np) { 
            ///TODO
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("connection points: ");
            foreach (Point p in Points) {
                sb.Append(p.ToString()).Append(" ");
            }
            
            return sb.ToString();
        }
    }
    /// <summary>
    /// node data info
    /// </summary>
    [Serializable]
    public class Connector {
        public Connector(Point location) {
            this.location = location;
        }
        private Point location;
        /// <summary>
        /// a connector's location 
        /// </summary>
        public Point Location {
            get { return location; }
            set { location = value; }
        }
        private Connection conn = null;
        /// <summary>
        /// connection this connector belongs to
        /// </summary>
        public Connection Connection {
            get { return conn; }
            set { conn = value; }
        }
        private Node refNode = null;
        /// <summary>
        /// Node the connector data belongs to 
        /// </summary>
        public Node RefNode {
            get { return refNode; }
            set {                
                refNode = value;
                if (RefNode != null) {
                    RefNode.Ctrs.Add(this);
                }
            }
        }        
    }    
    /// <summary>
    /// node data info
    /// </summary>
    [Serializable]
    public class Node {        
        private Point location;
        /// <summary>
        /// location of the Node, it will used to set the Node's location
        /// it is the same value with the node location.
        /// </summary>
        public Point Location {
            get { return location; }
            set { location = value; }
        }
        private Size size;
        /// <summary>
        /// size of the node 
        /// </summary>
        public Size Size {
            get { return size; }
            set { size = value; }
        }
        private Operation refOp = null;
        /// <summary>
        /// referenced operation model 
        /// </summary>
        public Operation RefOp {
            get { return refOp; }
            set { refOp = value; }
        }
        private Connectors ctrs = new Connectors();
        /// <summary>
        /// owned connnectors 
        /// </summary>
        public Connectors Ctrs {
            get { return ctrs; }            
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("Node ").Append(RefOp.Name).Append(" : location").Append(Location.ToString()).Append(", size").Append(Size.ToString());
            return sb.ToString();
        }
    }
    /// <summary>
    /// diagram data
    /// </summary>
    [Serializable]
    public class Diagram
    {
        private String _name = null;

        public String Name {
            get { return _name; }
            set { _name = value; }
        }

        private String _description = null;

        public String Description {
            get { return _description; }
            set { _description = value; }
        }
        private Nodes _nodes = new Nodes();
        /// <summary>
        /// Shapes node in the diagram
        /// </summary>
        public Nodes Nodes {
            get { return _nodes; }
        }
        private Connections _conns = new Connections();
        /// <summary>
        /// all connections in the diagram 
        /// </summary>
        public Connections Connections {
            get { return _conns; }
        }
        private Process _proc = null;
        /// <summary>
        /// the script model - process of the diagram. 
        /// </summary>
        public Process Proc {
            get { return _proc; }
            set { _proc = value; }
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("---------------- diagram ").Append(Proc.Name);
            sb.Append(Nodes.ToString()).Append("\n");
            sb.Append(Connections.ToString()).Append("\n") ;
            sb.Append("---------------- end diagram ").Append(Proc.Name);
            return sb.ToString();
        }
    }
    /// <summary>
    /// root of the view model, include connections and shapes
    /// </summary>
    [Serializable]
    public class ViewRoot
    {
        private Diagram mainDiagram = null;
        /// <summary>
        /// main diagram of the view model 
        /// </summary>
        public Diagram MainDiagram {
            get { return mainDiagram;  }
            set { mainDiagram = value; }
        }
        /// <summary>
        /// build an empty main diagram 
        /// </summary>
        /// <returns></returns>
        private Diagram createMainDiagram() {
            Diagram diagram = new Diagram();
            return diagram;
        }
        private Diagrams _diagrams = null;
        /// <summary>
        /// all diagrams in the view model
        /// </summary>
        public Diagrams Diagrams {
            get { return _diagrams; }
            //set { _shapes = value; }
        }
        public ViewRoot() {
            _diagrams = new Diagrams();
            mainDiagram = createMainDiagram();
            _diagrams.Add(mainDiagram);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n############## view root ================");
            sb.Append(Diagrams.ToString());
            sb.Append("\n############## end view root ================");

            return sb.ToString();
        }
    }
    #endregion view model data
    /// <summary>
    /// model with script model and view model 
    /// </summary>
    [Serializable]
    public class BigModel {
        private ViewRoot vroot = null;
        /// <summary>
        /// root of view model 
        /// </summary>
        public ViewRoot VRoot {
            get { return vroot; }
            set { vroot = value; }
        }
        private ScriptRoot sroot = null;
        /// <summary>
        /// root of script model 
        /// </summary>
        public ScriptRoot SRoot {
            get { return sroot; }
            set { sroot = value; }
        }
    }
}
