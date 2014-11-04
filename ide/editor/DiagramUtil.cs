using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WebMaster.ide.editor.model;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.editor.commands;
using WebMaster.lib;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor
{
    internal class DiagramUtil
    {
        #region constants 
        /// <summary>
        /// selected anchor rectangle width and height
        /// </summary>
        public static readonly int ANCHOR_SIZE = 6 ;
        // this is used to make sure the location is inner the canvas, ignore 
        // the scenario that the location is exactly the canvas border. 
        public static readonly int CANVAS_PADDING = 2;
        // how many pixels the node moved at least, it will treat that the node moved.
        public static readonly int MOVE_NODE_MIN = 2; 
        // use to define an invalid point 
        public static readonly Point PointNull = new Point(int.MinValue, int.MinValue);
        #endregion constants
        #region connection/connector utils methods
        /// <summary>
        /// compute the from connector and to connector position when create new connection.
        /// 
        /// 1. The return list contain just 2 Points:{from connector location},{to connector location}
        /// 2. If the from Node and to Node is the same node, 
        /// The return list contains maybe 5 points {from connector location},{3 middle points},{to connector location}
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <param name="scrollOffset">scroll offset of the current canvas</param>
        /// <returns></returns>
        public static List<Point> getFromToPoints(Node fromNode, Node toNode,Point scrollOffset) {
            int x1 = fromNode.Location.X; int x2 = toNode.Location.X;
            int y1 = fromNode.Location.Y; int y2 = toNode.Location.Y;
            int w1 = fromNode.Size.Width; int w2 = toNode.Size.Width;
            int h1 = fromNode.Size.Height; int h2 = toNode.Size.Height;
            Point fp = new Point(-1, -1);
            Point tp = new Point(-1, -1);
            List<Point> ps = new List<Point>();
            if (fromNode != toNode) {
                if (y2 + h2 <= y1) { // toNode's bottom is above the from node 
                    if (x2 + w2 < x1) { // to node is north west
                        fp.X = x1; fp.Y = y1 + h1 / 2;    // use from node left middle point
                    } else if (x2 > x1 + w1) { // to node is north east
                        fp.X = x1 + w1; fp.Y = y1 + h1 / 2;    // use from right middle point
                    } else {
                        fp.X = x1 + w1 / 2; fp.Y = y1; // use from node top middle point
                    }
                    tp.X = x2 + w2 / 2; tp.Y = y2 + h2; // use to node bottom middle point
                } else if (y2 >= y1 + h1) {  // toNode top is under from node
                    if (x2 + w2 < x1) { // to node is south west
                        fp.X = x1; fp.Y = y1 + h1 / 2;    // use from node left middle point
                    } else if (x2 > x1 + w1) { // to node is south east
                        fp.X = x1 + w1; fp.Y = y1 + h1 / 2;    // use from node right middle point
                    } else {
                        fp.X = x1 + w1 / 2; fp.Y = y1 + h1; // use from node bottom middle point
                    }
                    tp.X = x2 + w2 / 2; tp.Y = y2; // use to node top middle point
                } else {
                    if (x2 + toNode.Size.Width <= x1) { // to node is on the west
                        fp.X = x1; fp.Y = y1 + h1 / 2;
                        tp.X = x2 + w2; tp.Y = y2 + h2 / 2;
                    } else if (x2 >= x1 + w1) { // to node is on the east
                        fp.X = x1 + w1; fp.Y = y1 + h1 / 2;
                        tp.X = x2; tp.Y = y2 + h2 / 2;
                    } else { // for this condition, just use the center point as default
                        fp.X = x1 + w1 / 2; fp.Y = y1 + h1 / 2;
                        tp.X = x2 + w2 / 2; tp.Y = y2 + h2 / 2;
                    }
                }
            } else {
                int delta = 30 ;
                fp.X = x1 + w1; fp.Y = y1 + h1 / 2;
                tp.X = x1 + w1 / 2; tp.Y = y1;
                int d = w1 / 2;
                delta = delta > d ? d : delta;
                int my = y1>delta ? y1-delta : 0 ;
                Point mp0 = new Point();
                mp0.X = x1 + w1 + delta; mp0.Y = y1 + h1 / 2;
                mp0.X -= scrollOffset.X;
                mp0.Y -= scrollOffset.Y;
                Point mp1 = new Point();
                mp1.X = x1 + w1 + delta; mp1.Y = my;
                mp1.X -= scrollOffset.X;
                mp1.Y -= scrollOffset.Y;
                Point mp2 = new Point();
                mp2.X = x1 + w1/2; mp2.Y = my;
                mp2.X -= scrollOffset.X;
                mp2.Y -= scrollOffset.Y;
                ps.Add(mp0);
                ps.Add(mp1);
                ps.Add(mp2);
            }

            fp.X -= scrollOffset.X;
            fp.Y -= scrollOffset.Y;
            tp.X -= scrollOffset.X;
            tp.Y -= scrollOffset.Y;

            ps.Insert(0,fp);
            ps.Add(tp);
            
            return ps;
        }
        /// <summary>
        /// check whether the connection is under the mouse click point
        /// return the point to be inserted index, the click point should be at the index of the points list
        /// return -1 if not find 
        /// 
        /// the index value will be [1, count-1], or -1 if not found
        /// </summary>
        /// <param name="conn">Connection to be checked</param>
        /// <param name="np">test point</param>
        /// <returns></returns>
        public static int hitConnection(Connection conn, Point np) {
            for (int i = 0; i < conn.Points.Count - 1; i++) {
                Point sp = conn.Points[i];
                Point ep = conn.Points[i + 1];

                if (hit(sp, ep, np)) {
                    return i + 1;
                }
            }
            return -1;
        }
        /// <summary>
        /// whether the point hit the connector, return Connector or null if no connector hit
        /// </summary>
        /// <param name="con">Connection which connectors to be checkd</param>
        /// <param name="np">test point</param>
        /// <returns></returns>
        public static Connector hitConnector(Connection con, Point np) {
            // check whether the hit point is the From Connector
            Rectangle fr = new Rectangle(con.From.Location.X - 1 - DiagramUtil.ANCHOR_SIZE / 2, con.From.Location.Y - 1 - DiagramUtil.ANCHOR_SIZE / 2, DiagramUtil.ANCHOR_SIZE + 2, DiagramUtil.ANCHOR_SIZE + 2);
            if (fr.Contains(np)) {
                return con.From;
            }
            // check whether the hit point is the To Connector                    
            Rectangle tr = new Rectangle(con.To.Location.X - 1 - DiagramUtil.ANCHOR_SIZE / 2, con.To.Location.Y - 1 - DiagramUtil.ANCHOR_SIZE / 2, DiagramUtil.ANCHOR_SIZE + 2, DiagramUtil.ANCHOR_SIZE + 2);
            if (tr.Contains(np)) {
                return con.To;
            }
            return null;
        }
        /// <summary>
        /// update the connection points to the tplist.
        /// if point is new point and add the points into tplist with proper position 
        /// 
        /// the index value will be index from [0 .. count-1] or -1 if not found
        /// if 0 means that the hit point is the start anchor, or count-1 means the end anchor
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tpList"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static int updateTmpList(Connection con, List<Point> tpList, Point point) {
            int index = hitConnection(con, point);
            bool flag = true;
            if (index != -1) {
                int delta = ANCHOR_SIZE / 2;
                tpList.Clear();
                tpList.AddRange(con.Points);
                // verify the start anchor 
                if (index == 1) {
                    Rectangle r = new Rectangle(con.Points[0].X - delta, con.Points[0].Y - delta, ANCHOR_SIZE, ANCHOR_SIZE);
                    if (r.Contains(point)) {
                        index = 0;
                        flag &= false;
                    }
                } else if (index == con.Points.Count - 1) {
                    // verify the end anchor 
                    Rectangle r = new Rectangle(con.Points[con.Points.Count - 1].X - delta, con.Points[con.Points.Count - 1].Y - delta, ANCHOR_SIZE, ANCHOR_SIZE);
                    if (r.Contains(point)) {
                        flag &= false;
                    }
                }
                if (flag) {
                    Rectangle tr = new Rectangle(tpList[index].X - delta, tpList[index].Y - delta, ANCHOR_SIZE, ANCHOR_SIZE);
                    if (!tr.Contains(point)) {
                        tpList.Insert(index, new Point(point.X, point.Y));
                    }
                }
            }

            return index;
        }
        /// <summary>
        /// get proper anchor point on the node view, based on the target point 
        /// and current cursor point, return PointNull if errors. 
        /// 
        /// The proper point should be the latest cross point of the line and the node. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="evtp">event point </param>
        /// <param name="tp">target point </param>
        /// <param name="scrollOffset">scrollbar offset </param>
        /// <returns></returns>
        internal static Point getProperAnchorPoint(Node node, Point evtp, Point tp, Point scrollOffset) {
            //TODO this method can be refined. 
            if (node == null) {
                return DiagramUtil.PointNull;
            }
            Point ep = new Point(evtp.X, evtp.Y);
            Point np = new Point(node.Location.X - scrollOffset.X, node.Location.Y - scrollOffset.Y);
            int delta = ANCHOR_SIZE / 2;
            Point p = new Point(0, 0);
            List<Point> ps = new List<Point>(4);
            // top line point, ps[0]
            Point p0 = getLinesCrossPoint(np, new Point(np.X + node.Size.Width, np.Y), ep, tp);
            p0.Y = p0.Y != DiagramUtil.PointNull.Y ? np.Y : p0.Y;
            p0.X = p0.X != DiagramUtil.PointNull.X && p0.X <= np.X ? np.X : p0.X;
            p0.X = p0.X != DiagramUtil.PointNull.X && p0.X >= np.X + node.Size.Width ? np.X + node.Size.Width : p0.X;
            // left line point ps[1]
            Point p1 = getLinesCrossPoint(np, new Point(np.X, np.Y + node.Size.Height), ep, tp);
            p1.X = p1.X != DiagramUtil.PointNull.X ? np.X : p1.X;
            p1.Y = p1.Y != DiagramUtil.PointNull.Y && p1.Y <= np.Y ? np.Y : p1.Y;
            p1.Y = p1.Y != DiagramUtil.PointNull.Y && p1.Y >= np.Y + node.Size.Height ? np.Y + node.Size.Height : p1.Y;
            // bottom line point, ps[2]
            Point p2 = getLinesCrossPoint(new Point(np.X, np.Y + node.Size.Height), new Point(np.X + node.Size.Width, np.Y + node.Size.Height), ep, tp);
            p2.Y = p2.Y != DiagramUtil.PointNull.Y ? np.Y + node.Size.Height : p2.Y;
            p2.X = p2.X != DiagramUtil.PointNull.X && p2.X < np.X ? np.X : p2.X;
            p2.X = p2.X != DiagramUtil.PointNull.X && p2.X > np.X + node.Size.Width ? np.X + node.Size.Width : p2.X;
            // right line point, ps[3]
            Point p3 = getLinesCrossPoint(new Point(np.X + node.Size.Width, np.Y), new Point(np.X + node.Size.Width, np.Y + node.Size.Height), ep, tp);
            p3.X = p3.X != DiagramUtil.PointNull.X ? np.X + node.Size.Width : p3.X;
            p3.Y = p3.Y != DiagramUtil.PointNull.Y && p3.Y <= np.Y ? np.Y : p3.Y;
            p3.Y = p3.Y != DiagramUtil.PointNull.Y && p3.Y >= np.Y + node.Size.Height ? np.Y + node.Size.Height : p3.Y;
            ps.Add(p0);
            ps.Add(p1);
            ps.Add(p2);
            ps.Add(p3);
            Rectangle r = new Rectangle(np.X - delta, np.Y - delta, node.Size.Width + ANCHOR_SIZE, node.Size.Height + ANCHOR_SIZE);
            int distance = int.MaxValue;
            for (int i = 0; i < 4; i++) {
                if (ps[i] != PointNull) {
                    if (r.Contains(ps[i])) {
                        int d = (tp.X - ps[i].X) * (tp.X - ps[i].X) + (tp.Y - ps[i].Y) * (tp.Y - ps[i].Y);
                        if (d < distance) {
                            distance = d;
                            p = ps[i];
                        }
                    }
                }
            }
            if (distance == int.MaxValue) {
                return DiagramUtil.PointNull;
            } else {
                return p;
            }
        }
        /// <summary>
        /// get the proper point or PointNull if errors, check the 4 middle point of the node, and select
        /// a shortest distance point
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ep">event point</param>
        /// <param name="scrollOffset">scroll bar position</param>
        /// <returns></returns>
        internal static Point getProperAnchorPoint(Node node, Point ep, Point scrollOffset) {
            Point np = new Point(node.Location.X - scrollOffset.X, node.Location.Y - scrollOffset.Y);
            List<Point> plist = new List<Point>(4);
            plist.Add(new Point(np.X + node.Size.Width / 2, np.Y)); // top middle point
            plist.Add(new Point(np.X, np.Y + node.Size.Height / 2)); // left middle point
            plist.Add(new Point(np.X + node.Size.Width / 2, np.Y + node.Size.Height)); // bottom middle point
            plist.Add(new Point(np.X + node.Size.Width, np.Y + node.Size.Height / 2)); // right middle point 
            int index = 0;
            int dis = int.MaxValue;
            for (int i = 0; i < plist.Count; i++) {
                int d = (ep.X - plist[i].X) * (ep.X - plist[i].X) + (ep.Y - plist[i].Y) * (ep.Y - plist[i].Y);
                if (d < dis) {
                    index = i;
                    dis = d;
                }
            }

            Point p = plist[index];
            plist.Clear();
            return p;
        }
        /// <summary>
        /// clean all duplicated points caused by the connection point move 
        /// </summary>
        /// <param name="tpList"></param>
        /// <param name="ddpIndex">moved point index </param>
        internal static void cleanDuplicatePoints(List<Point> tpList, int ddpIndex) {
            if (ddpIndex < 0 || ddpIndex>tpList.Count-1) {
                return;
            }
            List<Point> ps = new List<Point>();
            if (ddpIndex - 2 >= 0) {
                if (DiagramUtil.isDuplicatedPoint(tpList[ddpIndex - 2], tpList[ddpIndex], tpList[ddpIndex - 1])) {
                    ps.Add(tpList[ddpIndex - 1]);
                }
            }
            if (ddpIndex - 1 >= 0 && ddpIndex + 1 < tpList.Count) {
                if (DiagramUtil.isDuplicatedPoint(tpList[ddpIndex - 1], tpList[ddpIndex + 1], tpList[ddpIndex])) {
                    ps.Add(tpList[ddpIndex]);
                }
            }
            if (ddpIndex + 2 < tpList.Count) {
                if (DiagramUtil.isDuplicatedPoint(tpList[ddpIndex], tpList[ddpIndex + 2], tpList[ddpIndex + 1])) {
                    ps.Add(tpList[ddpIndex + 1]);
                }
            }

            foreach (Point p in ps) {
                tpList.Remove(p);
            }
            ps.Clear();
        }
        /// <summary>
        /// re-layout the connectors position when node moved or resized
        /// </summary>
        public static void updateConnectors(NodeView node, Point offset) {
            foreach (Connector ctr in node.Node.Ctrs) {
                updateConnector(offset, ctr);
            }
        }
        /// <summary>
        /// update the connector's location
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="ctr"></param>
        public static void updateConnector(Point offset, Connector ctr) {
            if (ctr.Connection.Points.Count == 2) {
                List<Point> pl = DiagramUtil.getFromToPoints(ctr.Connection.From.RefNode, ctr.Connection.To.RefNode, offset);
                ctr.Connection.From.Location = pl[0];
                ctr.Connection.To.Location = pl[1];
                ctr.Connection.updateEnds();
            } else {
                //handle if there are more than 2 points on a connection
                if (ctr.Location.Equals(ctr.Connection.From.Location)) {
                    Point refp = ctr.Connection.Points[1];
                    Point p = DiagramUtil.getProperAnchorPoint(ctr.RefNode, refp, offset);
                    if (p != DiagramUtil.PointNull) {
                        ctr.Location = p;
                    }
                } else if (ctr.Location.Equals(ctr.Connection.To.Location)) {
                    Point refp = ctr.Connection.Points[ctr.Connection.Points.Count - 2];
                    Point p = DiagramUtil.getProperAnchorPoint(ctr.RefNode, refp, offset);
                    if (p != DiagramUtil.PointNull) {
                        ctr.Location = p;
                    }
                }
                ctr.Connection.updateEnds();
            }
        }
        /// <summary>
        /// If the tplist points is the same with connection points, return true, else return false 
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tpList"></param>
        /// <returns></returns>
        internal static bool isConnPointsChanged(Connection con, List<Point> tpList) {
            if (con != null && con.Points != null && tpList != null && tpList.Count>0) {
                if( tpList.Count != con.Points.Count){
                    return true;
                }
                foreach (Point p in tpList) {
                    if (!con.Points.Contains(p)) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get the connection Label position point or return Point.Empty if errors. 
        /// </summary>
        /// <param name="plist">connection points.plist[0] is start point</param>
        /// <returns></returns>
        internal static Point getConnectionLabelPosition(List<Point> plist) {
            if (plist == null || plist.Count < 2) {
                return Point.Empty;
            }
            int ds = UIConstants.CONN_LABEL_DISTANCE * UIConstants.CONN_LABEL_DISTANCE;
            Point tp = new Point(0,0);
            if (plist.Count == 2) {
                int dx = plist[0].X - plist[1].X;
                int dy = plist[0].Y - plist[1].Y;
                int d1 = dx * dx + dy * dy;
                if (d1 > ds) {
                    tp = getPointByDis(plist[0], plist[1], UIConstants.CONN_LABEL_DISTANCE);                    
                } else {
                    tp = getPointByDis(plist[0], plist[1], (int)(Math.Sqrt(d1) / 2));
                }
            } else {
                int dx = plist[0].X - plist[1].X;
                int dy = plist[0].Y - plist[1].Y;
                int d1 = dx * dx + dy * dy;
                if (d1 > ds) {
                    tp = getPointByDis(plist[0], plist[1], UIConstants.CONN_LABEL_DISTANCE);
                } else {
                    tp = plist[1];
                }
            }
            int x = tp.X - UIConstants.CONN_LABEL_OFFSET ;
            if(x<0){
                x = tp.X +UIConstants.CONN_LABEL_OFFSET ;
            }
            int y = tp.Y - UIConstants.CONN_LABEL_OFFSET;
            if(y<0){
                y = tp.Y+UIConstants.CONN_LABEL_OFFSET ;
            }
            return new Point(x,y);

        }
        /// <summary>
        /// Get the point from pstart to pend direction, the distance from pstart is dis
        /// </summary>
        /// <param name="pstart"></param>
        /// <param name="pend"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        private static Point getPointByDis(Point pstart, Point pend, int dis) {
            if (pstart.X == pend.X) {
                int y = pstart.Y > pend.Y ? pstart.Y - dis : pstart.Y + dis;
                return new Point(pstart.X, y);
            }
            if (pstart.Y == pend.Y) {
                int x = pstart.X > pend.X ? pstart.X - dis : pstart.X + dis;
                return new Point(x, pstart.Y);
            }
            double tan = (double)(pend.Y - pstart.Y) / (pend.X - pstart.X);
            double sin = tan / Math.Sqrt((1 + tan * tan));
            double cos = sin / tan;
            sin = Math.Abs(sin);
            cos = Math.Abs(cos);
            if (pend.Y > pstart.Y) {
                if (pend.X < pstart.X) {
                    cos = -cos;
                }
            } else {
                sin = -sin;
                if (pend.X < pstart.X) {
                    cos = -cos;
                }
            }
            int px = (int)(pstart.X + dis * cos);
            int py = (int)(pstart.Y + dis * sin);
            return new Point(px, py);
        }
        #endregion connection/connector utils methods
        #region points/line utils method
        /// <summary>
        /// Tests if the mouse hits this connection from from-point to to-point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool hit(Point from, Point to, Point p) {
            Point p1, p2, s;
            RectangleF r1, r2;
            float o, u;
            p1 = from; p2 = to;
            int delta = ANCHOR_SIZE / 2;
            // p1 must be the leftmost point.
            if (p1.X > p2.X) { s = p2; p2 = p1; p1 = s; }

            r1 = new RectangleF(p1.X, p1.Y, 0, 0);
            r2 = new RectangleF(p2.X, p2.Y, 0, 0);
            r1.Inflate(delta, delta);
            r2.Inflate(delta, delta);
            //this is like a topological neighborhood
            //the connection is shifted left and right
            //and the point under consideration has to be in between.						
            if (RectangleF.Union(r1, r2).Contains(p)) {
                if (p1.Y < p2.Y) //SWNE
				{
                    o = r1.Left + (((r2.Left - r1.Left) * (p.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
                    u = r1.Right + (((r2.Right - r1.Right) * (p.Y - r1.Top)) / (r2.Top - r1.Top));
                    return ((p.X > o) && (p.X < u));
                } else if (p1.Y == p2.Y) {
                    return p.Y < p1.Y + delta && p.Y > p1.Y - delta;
                } else {//NWSE				
                    o = r1.Left + (((r2.Left - r1.Left) * (p.Y - r1.Top)) / (r2.Top - r1.Top));
                    u = r1.Right + (((r2.Right - r1.Right) * (p.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
                    return ((p.X > o) && (p.X < u));
                }
            }
            return false;
        }
        /// <summary>
        /// check whether the np is on the line of the from -> to point
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="np"></param>
        /// <returns></returns>
        internal static bool isDuplicatedPoint(Point from, Point to, Point np) {
            return hit(from, to, np);
        }
        /// <summary>
        /// get cross point of the two lines or PointNull
        /// </summary>
        /// <param name="p1">first line start point</param>
        /// <param name="p2">first line end point </param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        private static Point getLinesCrossPoint(Point p1, Point p2, Point p3, Point p4) {
            double x = double.NaN;
            double y = double.NaN;
            double k1 = double.NaN;
            double b1 = double.NaN;
            double k2 = double.NaN;
            double b2 = double.NaN;
            if (p1.X == p2.X) {
                x = p1.X;
                if (p1.Y != p2.Y) {
                    if (p3.X != p4.X) {
                        k2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                        b2 = p3.Y - k2 * p3.X;
                        y = k2 * x + b2;
                    }
                }
            } else {
                k1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                b1 = p1.Y - k1 * p1.X;
                if (p3.X == p4.X) {
                    x = p3.X;
                    y = k1 * x + b1;
                } else {
                    k2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                    b2 = p3.Y - k2 * p3.X;
                    if (k1 != k2) {
                        x = (b2 - b1) / (k1 - k2);
                        y = k1 * x + b1;
                    }
                }
            }
            if (double.IsNaN(x) || double.IsNaN(y)) {
                return PointNull;
            } else {
                return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
            }
        }
        #endregion points/ line utils method                
        #region common method 
        /// <summary>
        /// get the target node location for the current moving node, or return PointNull if invalid 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Point getMoveNodePoint(Canvas canvas, NodeView node) {
            if (canvas.Move_startPoint != DiagramUtil.PointNull && canvas.Move_endPoint != DiagramUtil.PointNull) {
                int x = canvas.Move_endPoint.X - canvas.Move_startPoint.X + node.Location.X;
                int y = canvas.Move_endPoint.Y - canvas.Move_startPoint.Y + node.Location.Y;
                x = x < 0 ? 0 : x;
                y = y < 0 ? 0 : y;
                return new Point(x, y);
            } else {
                return DiagramUtil.PointNull;
            }
        }
        /// <summary>
        /// get the selected object in the diagram with evt point, the proper object will be 
        /// NodeView, Connector, Connection or Diagram in order, NodeView has the 1st priority. 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="ep">cursor position</param>
        /// <param name="scrollOffset">scroll bar offset </param>
        /// <returns></returns>
        internal static object getSelectedObject(Canvas canvas, Point ep, Point scrollOffset) {
            // check NodeView
            NodeView node = canvas.SelectedObj as NodeView;
            if (node != null) {
                ACTION at = getProperAnchorType(node, new Point(ep.X + scrollOffset.X, ep.Y + scrollOffset.Y));
                if (at == ACTION.RESIZE_EAST || at == ACTION.RESIZE_SOUTH || at == ACTION.RESIZE_SOUTH_EAST) {
                    return node;
                }
            }
            // check Connections 
            foreach (Connection con in canvas.Diagram.Connections) {
                int index = DiagramUtil.hitConnection(con, ep);
                if (index != -1) {
                    Connector ctr = hitConnector(con, ep);
                    if (ctr != null) {
                        return ctr;
                    } else {
                        return con;
                    }
                }
            }
            return canvas.Diagram;
        }
        /// <summary>
        /// whether the point is above the node anchor, and return the anchor type 
        /// or return AnchorType.NONE if not valid
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ep">event point</param>
        internal static ACTION getProperAnchorType(NodeView node, Point ep) {
            //Point p = new Point(ep.X-offset.X,ep.Y-offset.Y);
            int delta = DiagramUtil.ANCHOR_SIZE / 2;
            // east anchor
            Rectangle r = new Rectangle(node.Location.X + node.Width - 1, node.Location.Y + node.Height / 2 - delta, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE);
            if (r.Contains(ep)) {
                return ACTION.RESIZE_EAST;
            }
            // south east anchor
            r = new Rectangle(node.Location.X + node.Width - 1, node.Location.Y + node.Height - 1, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE);
            if (r.Contains(ep)) {
                return ACTION.RESIZE_SOUTH_EAST;
            }
            // south anchor
            r = new Rectangle(node.Location.X + node.Width / 2 - delta, node.Location.Y + node.Height - 1, DiagramUtil.ANCHOR_SIZE, DiagramUtil.ANCHOR_SIZE);
            if (r.Contains(ep)) {
                return ACTION.RESIZE_SOUTH;
            }
            return ACTION.NONE;
        }
        /// <summary>
        /// how many distance the X/Y-o will be moved, less 0 means that the X-o will be moved left
        /// and the size of the perferred canvas client size. 
        /// 
        /// x' = x - dx 
        /// x' means the new location based on canvas, 
        /// x menas the existed location based on canvas, 
        /// dx means movment, dx > 0 means the canva(0,0) point move right. 
        /// 
        /// return a 4 int values [topLeftPoint,bottomRightPoint, delaXYPoint, SizePoint ], the topLeft and bottomRight
        /// point is the real position of the canvas, I mean the position include the scroll values. 
        /// 
        /// dx less than 0 means that the X-o will be moved left
        /// 
        /// </summary>
        /// <param name="node">ajust node</param>
        /// <param name="canvas"></param>
        /// <param name="np"> to be node position with canvas client location value, e.g Node Alocaltion is (10,10), scroll(-10,10)
        /// so that the np value should be (20,20), this is the real position in the canvas. 
        /// </param>
        /// <returns></returns>
        internal static Point[] getXYOAjustment(NodeView node, Canvas canvas, Point np) {
            Point plt = new Point(int.MaxValue, int.MaxValue); // top left point 
            Point pbr = new Point(int.MinValue, int.MinValue); // bottom right point 
            
            foreach (Control nv in canvas.Controls) {
                if (nv != node && nv is NodeView) {
                    Point p = new Point(nv.Location.X - canvas.AutoScrollPosition.X, nv.Location.Y - canvas.AutoScrollPosition.Y);
                    plt.X = plt.X > p.X ? p.X : plt.X;
                    plt.Y = plt.Y > p.Y ? p.Y : plt.Y;
                    // ignore the two marker's bottom right point
                    pbr.X = pbr.X < p.X + nv.Size.Width ? p.X + nv.Size.Width : pbr.X;
                    pbr.Y = pbr.Y < p.Y + nv.Size.Height ? p.Y + nv.Size.Height : pbr.Y;
                }                
            }
            bool ignore = false;
            foreach (Connection con in canvas.Diagram.Connections) {                
                foreach (Point p in con.Points) {
                    ignore = false;
                    // filter the node connector points, they will be ignored 
                    if (node != null && node.Node != null) {
                        foreach (Connector ctr in node.Node.Ctrs) {
                            if (ctr.Connection == con && ctr.Location == p) {
                                ignore = true;
                                break;
                            }
                        }
                    }
                    if (!ignore) {
                        plt.X = p.X < plt.X ? p.X : plt.X;
                        plt.Y = p.Y < plt.Y ? p.Y : plt.Y;
                        pbr.X = p.X > pbr.X ? p.X : pbr.X;
                        pbr.Y = p.Y > pbr.Y ? p.Y : pbr.Y;
                    }
                }
            }
            // update the new topleft point and bottome right point with the new node location. 
            int xflag = 0;
            int yflag = 0;
            if (node != null) {                
                // node move cause there is a new leftest point 
                if (plt.X > np.X) {
                    plt.X = np.X;
                    xflag = -1; // new leftest point 
                }
                if (plt.Y > np.Y) {
                    plt.Y = np.Y;
                    yflag = -1; // new topest point 
                }
                int npx = np.X + node.Size.Width;
                int npy = np.Y + node.Size.Height;
                if (pbr.X < npx) {
                    pbr.X = npx;
                    xflag = 1; // new rightest point 
                }
                if (pbr.Y < npy) {
                    pbr.Y = npy;
                    yflag = 1; // new bottomest point 
                }
            }
            int dx = plt.X - DiagramUtil.CANVAS_PADDING;
            int dy = plt.Y - DiagramUtil.CANVAS_PADDING;
            // justify dx 
            if(plt.X >= CANVAS_PADDING){
                int vw = 0;
                if (canvas.HasHScroll()) {
                    vw = SystemInformation.VerticalScrollBarWidth;
                }
                if (canvas.ClientSize.Width + vw >= pbr.X) {
                    dx = 0;
                }
            }
            // justify dy 
            if (plt.Y >= CANVAS_PADDING) { 
                int hw = 0 ;
                if(canvas.HashVScroll()){
                    hw = SystemInformation.HorizontalScrollBarHeight;
                }
                if (canvas.ClientSize.Height + hw >= pbr.Y) {
                    dy = 0;
                }
            }
            
            int w = pbr.X - plt.X;
            int h = pbr.Y - plt.Y;
            w = xflag == 1 ? w - dx : w;
            h = yflag == 1 ? h - dy : h;
                        
            return new Point[4]{plt,pbr,new Point(dx,dy),new Point(w,h)};
        }
        #endregion common method        
        #region model utils
        /// <summary>
        /// create a new node with specified operation type. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="p"></param>
        /// <param name="ctrl"></param>
        /// <param name="diagramManager"></param>
        /// <returns></returns>
        public static NodeView createNode(OPERATION type, Point p, Canvas ctrl, DiagramManager diagramManager) {
            NodeView nv = new NodeView(ctrl);

            nv.Location = new Point(p.X, p.Y);
            Operation refOp = null;
            switch (type) {
                case OPERATION.START:
                    nv.Image = global::ide.Properties.Resources.op_start16;
                    nv.Size = new Size(30, 30);
                    refOp = createUniqueOperation(ctrl.Diagram.Proc.Ops, OPERATION.START);
                    break;
                case OPERATION.END:
                    nv.Image = global::ide.Properties.Resources.op_stop16;
                    nv.Size = new Size(30, 30);
                    refOp = createUniqueOperation(ctrl.Diagram.Proc.Ops, OPERATION.END);
                    break;
                case OPERATION.NOP:
                    nv.Image = global::ide.Properties.Resources.op_nop16;
                    nv.Size = new Size(30, 30);
                    refOp = createUniqueOperation(ctrl.Diagram.Proc.Ops, OPERATION.NOP);
                    break;
                case OPERATION.CLICK:
                    nv.Image = global::ide.Properties.Resources.op_click16;
                    nv.Size = new Size(100, 60);
                    refOp = createUniqueOperation(ctrl.Diagram.Proc.Ops, OPERATION.CLICK);
                    break;
                case OPERATION.INPUT:
                    nv.Image = global::ide.Properties.Resources.op_input16;
                    nv.Size = new Size(100, 60);
                    refOp = createUniqueOperation(ctrl.Diagram.Proc.Ops, OPERATION.INPUT);
                    break;
                case OPERATION.PROCESS:
                    nv.Image = global::ide.Properties.Resources.op_process16;
                    nv.Size = new Size(100, 60);
                    refOp = createUniqueProcess(ctrl.Diagram.Proc.Procs);
                    break;
                case OPERATION.OPEN_URL_T:
                    nv.Image = global::ide.Properties.Resources.op_url16;
                    nv.Size = new Size(100, 60);
                    refOp = createUniqueOperation(ctrl.Diagram.Proc.Ops, OPERATION.OPEN_URL_T);
                    break;

            }
            nv.Label = refOp.Name;
            // update node model info
            Node nd = ModelFactory.createNode();
            nd.Location = new Point(nv.Location.X, nv.Location.Y);
            nd.Size = new Size(nv.Size.Width, nv.Size.Height);
            nd.RefOp = refOp;

            nv.Node = nd;

            Process proc = ctrl.Diagram.Proc;
            if (refOp is Process) {
                proc.Procs.AddUnique(refOp as Process);
            } else {
                // add the new operaiton to ScriptRoot 
                proc.Ops.AddUnique(refOp);
            }
            diagramManager.ActiveCanvas.Diagram.Nodes.Add(nd);

            return nv;
        }
        /// <summary>
        /// create a name unique OpCondition for the opclist, so that this OpCondition can be added into 
        /// the list. 
        /// </summary>
        /// <param name="opclist"></param>
        /// <returns></returns>
        internal static OpCondition createUniqueOpCondition(Operation op) {
            BEList<OpCondition> conlist = op.OpConditions;
            OpCondition opc = ModelFactory.createOpCondition();
            string name = op.Name+" - Link0";
            opc.Name = name;
            if (!ModelManager.Instance.isUniqueToBeElement(opc, conlist)) {
                for (int i = 1; i < 500; i++) {
                    opc.Name = name + i;
                    if (ModelManager.Instance.isUniqueToBeElement(opc, conlist)) {
                        break;
                    }
                }
            }
            return opc;
        }
        internal static Process createUniqueProcess(BEList<Process> proclist) {
            Process proc = ModelFactory.createProcess();
            proc.Name = ModelManager.Instance.getUniqueElementName(proclist, proc);

            return proc;
        }
        internal static Operation createUniqueOperation(BEList<Operation> oplist, OPERATION type) {
            Operation op = ModelFactory.createOperation(type);
            op.Name = ModelManager.Instance.getUniqueElementName(oplist, op);

            return op;
        }
        /// <summary>
        /// remove the connection from diagram, 
        /// 1. remove the connectors from both nodes, 
        /// 2. remove connection view model from diagram 
        /// 3. remove connection referrenced model
        /// </summary>
        /// <param name="con"></param>
        /// <param name="diagram"></param>
        internal static void removeConnection(Connection con, Diagram diagram) {
            if (con == null || diagram == null) {
                return;
            }
            Operation op = con.From.RefNode.RefOp;
            // clean connectors 
            con.From.RefNode.Ctrs.Remove(con.From);
            con.To.RefNode.Ctrs.Remove(con.To);
            // remove con from diagram 
            diagram.Connections.Remove(con);
            // remove from model
            OpCondition opcon = con.RefCon;
            ModelManager.Instance.removeFromModel(opcon);
        }
        /// <summary>
        /// 1. remove all coming and outing connections 
        /// 2. remove node view from canvas 
        /// 3. remove node view model from diagram 
        /// 4. remove node referrenced model from parent 
        /// </summary>
        /// <param name="nv"></param>
        /// <param name="diagram"></param>
        /// <param name="diagramManager"></param>
        internal static void removeNodeView(NodeView nv, Diagram diagram, DiagramManager diagramManager) {
            // remove all comming/outing connections
            List<Connector> ctrs = new List<Connector>();
            foreach (Connector ctr in nv.Node.Ctrs) {
                ctrs.Add(ctr);
            }
            foreach (Connector ctr in ctrs) {
                DiagramUtil.removeConnection(ctr.Connection, diagram);
            }
            // remove node view from canvas 
            nv.Parent.Controls.Remove(nv);
            // remove node model from diagram 
            diagram.Nodes.Remove(nv.Node);
            // remvoe node ref model 
            Operation op = nv.Node.RefOp;
            // remove the diagram info.            
            if (op is Process) {
                Diagram tdiagram = null;
                Process proc = op as Process;
                foreach (Diagram dgm in diagramManager.BigModel.VRoot.Diagrams) {
                    if (proc == dgm.Proc) {
                        tdiagram = dgm;
                        break;
                    }
                }
                if (tdiagram != null) {
                    // remove the diagram from view model
                    diagramManager.BigModel.VRoot.Diagrams.Remove(tdiagram);
                    // remove the process from data model
                    ModelManager.Instance.removeFromModel(proc);                    
                    // close the diagram tab if opened. 
                    diagramManager.closeActiveDiagram();
                }
            } else {
                ModelManager.Instance.removeFromModel(op);
            }
        }
        #endregion model utils
        #region delete/restore connection/node
        /// <summary>
        /// delete an Connection or NodeView form the canvas 
        /// </summary>
        /// <param name="canvas"></param>
        internal static void delete(Canvas canvas,DiagramManager diagramManager) {
            if (canvas == null) {
                return;
            }
            // delete selected object 
            if (canvas.SelectedObj is NodeView) {
                DeleteNodeViewCommand cmd = new DeleteNodeViewCommand(canvas.SelectedObj as NodeView, canvas, diagramManager);
                cmd.execute();
                diagramManager.markModelDirty();
                canvas.Refresh();
            }
            if (canvas.SelectedObj is Connection) {
                DeleteConnectionCommand cmd = new DeleteConnectionCommand(canvas.SelectedObj as Connection, canvas);
                cmd.execute();
                diagramManager.markModelDirty();
                canvas.Refresh();
            }
        }
        #endregion delete/restore connection/node

    }
}
