using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;
using System.Drawing;

namespace WebMaster.lib.engine
{
    public class CoverBox
    {
        /// <summary>
        /// try to used to unique define a cover box 
        /// </summary>
        private static string key = null;

        private static readonly int LINE_WIDTH = 2;
        //private static readonly string COLOR_SHOW = "#ececec";
        public static readonly string COLOR_CAPTURE = "blue";
        public static readonly string COLOR_CHECK = "red";
        public static readonly string CB_KEY_LEFT = "wm_cb_left_";
        public static readonly string CB_KEY_RIGHT = "wm_cb_right";
        public static readonly string CB_KEY_TOP = "wm_cb_top";
        public static readonly string CB_KEY_BOTTOM = "wm_cb_bottom";

        IHTMLElement left = null;
        IHTMLElement top = null;
        IHTMLElement right = null;
        IHTMLElement bottom = null;
        
        /// <summary>
        /// build a cover box with four div 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        public void build(IHTMLElement left, IHTMLElement right, IHTMLElement top, IHTMLElement bottom) {
            Log.println_hook("build cover box ...................");
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
        /// <summary>
        /// whether the elem is the cover box 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool isCoverbox(IHTMLElement elem) {
            if (elem != null && elem.id != null) {
                return elem.id.StartsWith("wm_cb_") && elem.id.EndsWith(CoverBox.key);
            }
            return false;
        }
        /// <summary>
        /// show cover box at the rect location and size, with color 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public void show(Rectangle rect, string color) {
            show(rect, color, false);
        }
        /// <summary>
        /// show cover box at the rect location and size, with color 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="flashed">whether flash the cover box </param>
        public void show(Rectangle rect, string color, bool flashed) {
            // check size first, to avoid some errors, just the element size was 
            // updated by something before the showing.
            int min = LINE_WIDTH+LINE_WIDTH+LINE_WIDTH ;
            if (rect != null && (rect.Width < min || rect.Height < min)) {
                return;
            }

            left.style.visibility = "visible";
            int lx = rect.X >= LINE_WIDTH ? rect.X - LINE_WIDTH : 0;
            int ly = rect.Y >= LINE_WIDTH ? rect.Y - LINE_WIDTH : 0;
            left.style.left = lx + "px";
            left.style.top = ly + "px";
            left.style.height = (rect.Height + LINE_WIDTH + LINE_WIDTH) + "px";
            if (flashed) {
                left.style.borderStyle = "dashed";
                left.style.borderWidth = "1px";
                left.style.borderColor = color;
                Log.println_hook("show , flashed . ");
            } else {
                left.style.backgroundColor = color;
                Log.println_hook("show, cb.style visibility = "+left.style.visibility+", left = "+left.style.left+", top = "+left.style.top+", width = "+left.style.width+", height = "+left.style.height);
            }
            right.style.visibility = "visible";
            right.style.left = (rect.X + rect.Width) + "px";
            right.style.top = ly + "px";
            right.style.height = (rect.Height + LINE_WIDTH + LINE_WIDTH) + "px";
            if (flashed) {
                right.style.borderStyle = "dashed";
                right.style.borderWidth = "1px";
                right.style.borderColor = color;
            } else {
                right.style.backgroundColor = color;
            }
            top.style.visibility = "visible";
            top.style.left = rect.X + "px";
            top.style.top = ly + "px";
            top.style.width = rect.Width + "px";
            if (flashed) {
                top.style.borderStyle = "dashed";
                top.style.borderWidth = "1px";
                top.style.borderColor = color;
            } else {
                top.style.backgroundColor = color;
                Log.println_hook("show top, cb.style visibility = " + top.style.visibility + ", left = " + top.style.left + ", top = " + top.style.top + ", width = " + top.style.width + ", height = " + top.style.height + ", border color = " + top.style.borderColor);
            }
            bottom.style.visibility = "visible";
            bottom.style.left = rect.X + "px";
            bottom.style.top = (rect.Y + rect.Height) + "px";
            bottom.style.width = rect.Width + "px";
            if (flashed) {
                bottom.style.borderStyle = "dashed";
                bottom.style.borderWidth = "1px";
                bottom.style.borderColor = color;
            } else {
                bottom.style.backgroundColor = color;
            }
        }
        /// <summary>
        /// hidden cover box 
        /// </summary>
        public void hidden() {
            left.style.visibility = "hidden";
            top.style.visibility = "hidden";
            right.style.visibility = "hidden";
            bottom.style.visibility = "hidden";
        }
        internal bool isVisible() {
            return left.style.visibility == "visible";
        }
        /// <summary>
        /// whether the cover box is the same one 
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        internal bool isTheSame(CoverBox cb) {
            return this.left != null && this.left.Equals(cb.left);
        }
        public override string ToString() {
            return "Id = " + CoverBox.key;
        }
        /// <summary>
        /// build cover box for document with specified code as id
        /// </summary>
        /// <param name="doc2"></param>
        /// <param name="id"></param>
        public static void create(IHTMLDocument2 doc2) {
            if (CoverBox.key == null) {
                CoverBox.key = DateTime.Now.Millisecond + "";
            }
            if (doc2 != null && doc2.body != null) {
                Log.println_hook("create cover box , id = " + CoverBox.key);
                createLeft(doc2, CoverBox.key);
                createRight(doc2, CoverBox.key);
                createTop(doc2, CoverBox.key);
                createBottom(doc2, CoverBox.key);
            }
        }
        #region static method for create cover box 
        private static IHTMLElement createLeft(IHTMLDocument2 doc2, string code) {
            IHTMLElement cb = createCoverBox(doc2, CB_KEY_LEFT + code);
            IHTMLStyle style = cb.style;
            style.setAttribute("position", "absolute");
            style.zIndex = "2147483647";
            style.borderStyle = "none";
            style.filter = "alpha(opacity=50)";
            style.width = LINE_WIDTH + "px";
            style.overflow = "hidden";
            return cb;
        }
        private static IHTMLElement createRight(IHTMLDocument2 doc2, string code) {
            IHTMLElement cb = createCoverBox(doc2, CB_KEY_RIGHT + code);
            IHTMLStyle style = cb.style;
            style.setAttribute("position", "absolute");
            style.zIndex = "2147483647";
            style.borderStyle = "none";
            style.filter = "alpha(opacity=50)";
            style.width = LINE_WIDTH + "px";
            style.overflow = "hidden";
            return cb;
        }
        private static IHTMLElement createTop(IHTMLDocument2 doc2, string code) {
            IHTMLElement cb = createCoverBox(doc2, CB_KEY_TOP + code);
            IHTMLStyle style = cb.style;
            style.setAttribute("position", "absolute");
            style.zIndex = "2147483647";
            style.borderStyle = "none";
            style.filter = "alpha(opacity=50)";
            style.height = LINE_WIDTH + "px";
            style.overflow = "hidden";
            return cb;
        }
        private static IHTMLElement createBottom(IHTMLDocument2 doc2, string code) {
            IHTMLElement cb = createCoverBox(doc2, CB_KEY_BOTTOM + code);
            IHTMLStyle style = cb.style;
            style.setAttribute("position", "absolute");
            style.zIndex = "2147483647";
            style.borderStyle = "none";
            style.filter = "alpha(opacity=50)";
            style.height = LINE_WIDTH + "px";
            style.overflow = "hidden";
            return cb;
        }
        private static IHTMLElement createCoverBox(IHTMLDocument2 doc2, string id) {
            IHTMLElement2 cb2 = doc2.createElement("div") as IHTMLElement2;
            IHTMLElement cb = cb2 as IHTMLElement;
            cb.id = id;
            HTMLBody body = doc2.body as HTMLBody;
            body.appendChild(cb2 as IHTMLDOMNode);
            return cb;
        }
        #endregion 
        public static string getId() {
            if (CoverBox.key == null) {
                CoverBox.key = DateTime.Now.Millisecond + "";
            }
            return CoverBox.key;
        }
        /// <summary>
        /// scroll the coverbox into view 
        /// </summary>
        internal void scrollIntoView() {
            //if (this.left != null) {
            //    this.left.scrollIntoView();
            //}
            if (this.top != null) {
                this.top.scrollIntoView();
            }
            // adjust scroll bar             
            IHTMLDocument3 doc3 = left.document as IHTMLDocument3;
            IHTMLElement2 he2 = doc3.getElementsByTagName("HTML").item(0) as IHTMLElement2;
            // IHTMLElement2 body2 = doc3.getElementsByTagName("BODY").item(0) as IHTMLElement2;

            if (he2.scrollTop >= 6) {
                he2.scrollTop -= 6;
            }
            if (he2.scrollLeft >= 6) {
                he2.scrollLeft -= 6;
            }
        }
    }
}
