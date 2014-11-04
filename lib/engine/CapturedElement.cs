using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;
using System.Drawing;
using System.Windows.Forms;

namespace WebMaster.lib.engine
{
    public class CapturedElement
    {
        /// <summary>
        /// make sure that when you use this methods, pls add the current element's 
        /// container frame in order if have. it means that, frames[0] is the direct container frame
        /// of core element, while, frames[1] is frames[0]'s direct container, and frames[last]'s
        /// container is the top document window.
        /// </summary>
        public List<IHTMLElement> frames = new List<IHTMLElement>();
        private int left = 0;
        /// <summary>
        /// core element's location based on the visible view. it will be used for cover box
        /// </summary>
        public int Left {
            get { return left; }
            set { left = value; }
        }
        private int top = 0;
        /// <summary>
        /// core element's location based on the visible view. it will be used for cover box
        /// </summary>
        public int Top {
            get { return top; }
            set { top = value; }
        }
        private int width = 0;
        /// <summary>
        /// core element's location based on the visible view. it will be used for cover box
        /// </summary>
        public int Width {
            get { return width; }
            set { width = value; }
        }
        private int height = 0;
        /// <summary>
        /// core element's location based on the visible view. it will be used for cover box
        /// </summary>
        public int Height {
            get { return height; }
            set { height = value; }
        }
        /// <summary>
        /// previous html element that is showed by coverbox.
        /// lastShowElem and lastShowColor is used to improve the performance. 
        /// I help to do that when capture mouse moving only the same IHTMLElement
        /// was shown the coverbox once if it is not changed. 
        /// </summary>
        private IHTMLElement2 lastShowElem = null;
        /// <summary>
        /// previous cover box color that is shown. 
        /// </summary>
        private string lastShowColor = string.Empty;
        /// <summary>
        /// core selected HtmlELement IHTMLElement2
        /// </summary>
        private IHTMLElement2 elem2 = null;
        /// <summary>
        /// core selected IHTMLElement2
        /// </summary>
        public IHTMLElement2 Elem2 {
            get { return elem2; }
            set { elem2 = value; }
        }
        private CoverBox coverBox = null;
        /// <summary>
        /// selected element referenced cover box 
        /// </summary>
        internal CoverBox CoverBox {
            get { return coverBox; }
            set { coverBox = value; }
        }
        private bool flashed = false;
        /// <summary>
        /// whethere flash the cover box
        /// </summary>
        public bool Flashed {
            get { return flashed; }
            set { flashed = value; }
        }
        private Point hotpoint = Point.Empty;
        /// <summary>
        /// it is a record of the current cursor position, it is used work with flashed flag.
        /// if flashed is true, the hotpoint will take effect, then when show cover box, it will
        /// show a flashed small cover box on the cursor position. 
        /// </summary>
        public Point Hotpoint {
            get { return hotpoint; }
            set { hotpoint = value; }
        }
        /// <summary>
        /// whether the captured element is empty, it means that it is cleaned
        /// </summary>
        /// <returns></returns>
        public bool isEmpty() {
            return elem2 == null;
        }
        /// <summary>
        /// show cover box referenced with the captured element 
        /// </summary>
        /// <param name="borderColor">cover box border color</param>
        internal void showCoverBox(string borderColor) {
            Log.println_hook("CapturedElement show cover box : " + this.ToString());
            if (CoverBox != null) {
                if (needShow(borderColor)) {
                    //Log.println_hook("Show cover box, outHtml = \n"+Elem.OuterHtml);
                    if (!Flashed) {
                        CoverBox.show(new Rectangle(Left, Top, Width, Height), borderColor);
                    } else {
                        Rectangle rect = new Rectangle(Hotpoint.X - 5, hotpoint.Y - 5, 10, 10);
                        CoverBox.show(rect, borderColor, true);
                        Flashed = false;
                    }

                    lastShowElem = this.Elem2;
                    lastShowColor = borderColor;
                    if (lastShowColor == null) {
                        lastShowColor = string.Empty;
                    }
                }
            } else {
                Log.println_hook("CapturedElement Cover box is null : " + this.ToString());
            }
        }
        /// <summary>
        /// whether need to show the cover box , true: means need show. false it not. 
        /// </summary>
        /// <param name="tobeColor">to be showned coverbox color</param>
        /// <returns></returns>
        private bool needShow(string tobeColor) {
            if (false == coverBox.isVisible()) {
                Log.println_hook("  need show : true");
                return true;
            }

            if ((Elem2 != null && Elem2.Equals(lastShowElem) && this.lastShowColor.Equals(tobeColor)) || CoverBox.isCoverbox(Elem2 as IHTMLElement)) {
                Log.println_hook("  need show : false .      con1 = " + CoverBox.isCoverbox(Elem2 as IHTMLElement));

                return false;
            } else {
                Log.println_hook("  need show 1 : true");
                return true;
            }
        }

        public override string ToString() {
            String tag = elem2 != null ? ((IHTMLElement)elem2).tagName : "null";
            return "captured rect = (" + Left + "," + Top + "," + Width + "," + Height + "), elem tag = " + tag + ", cover box = " + CoverBox;
        }
    }
    /// <summary>
    /// this is used to fix the outer project access embedding types from managed assemblies. 
    /// http://msdn.microsoft.com/en-us/library/dd409610.aspx
    /// 
    /// </summary>
    public class IHTMLElementWrap{
        IHTMLElement ihtmlElem = null;

        public IHTMLElement IHTMLElem {
            get { return ihtmlElem; }
            set { ihtmlElem = value; }
        }

    }
}
