using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using mshtml;
using WebMaster.lib.hooks;

namespace WebMaster.lib.engine
{
    public class CaptureUtil
    {
        /// <summary>
        /// get captured element info with cp if find or rerturn an Empty object. 
        /// Make sure that the doc is top level document
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="topDoc2"> it is the top level dom document, it should be the webbrowser
        /// top levle document</param>
        /// <returns></returns>
        public static CapturedElement getIHTMLElement2ByPos(Point cp, IHTMLDocument2 topDoc2) {
            CapturedElement capturedE = new CapturedElement();
            Log.println_hook("get IHTMLElement2ByPos ............ cp = " + cp);
            if (topDoc2 != null) {
                try {
                    updateIHTMLElement2ByPos(cp, capturedE, topDoc2, topDoc2, 0);
                } catch (UnauthorizedAccessException e) {
                    Log.println_hook(".............. catch exception ........ e = " + e.Data);
                }
            }
            return capturedE;
        }
        /// <summary>
        /// udpate the capturedElement with captured element info and cover box info.
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="capturedElement"></param>
        /// <param name="elemDoc">element document, that is used to locate html element</param>
        /// <param name="cbxdoc2">this is used to find the cover box</param>
        /// <returns></returns>
        private static void updateIHTMLElement2ByPos(Point cp, CapturedElement capturedElement, IHTMLDocument2 elemDoc, IHTMLDocument2 cbxdoc2, int level) {
            IHTMLElement elem = elemDoc.elementFromPoint(cp.X, cp.Y);
            Log.println_hook("level = " + level + " : start updateIHTMLElement2ByPos, elem tag = " + elem.tagName);
            if (elem != null) {
                string tag = elem.tagName.ToLower();
                IHTMLElement2 elem2 = elem as IHTMLElement2;
                // handle iframe contained elements 
                if (tag.Equals("iframe")) {
                    HTMLIFrame iframe = elem2 as HTMLIFrame;
                    IHTMLWindow2 fwin2 = iframe.contentWindow;
                    IHTMLDocument2 d2 = CrossFrameIE.GetDocumentFromWindow(fwin2);
                    IHTMLRect rect = elem2.getBoundingClientRect();
                    Point fp = new Point(cp.X - rect.left, cp.Y - rect.top);
                                       
                    updateIHTMLElement2ByPos(fp, capturedElement, d2, cbxdoc2, level + 1);

                    Point offset = getIFrameOffset(elem);
                    capturedElement.Left += offset.X;
                    capturedElement.Top += offset.Y;
                    capturedElement.frames.Insert(0,elem);

                    String str = "level = " + level + " : iframe : doc2, iframe pos = (" + rect.left + "," + rect.top + "), iframe size = (" + (rect.right - rect.left) + "," + (rect.bottom - rect.top) + "), iframe cursor pos = (" + fp.X + "," + fp.Y + "), captured element = " + capturedElement;
                    Log.println_hook(str);
                } else if (tag.Equals("frameset")) {
                    // handle frameset, in fact, we need do nothing here, frame set is just a frame
                    Log.println_hook("level = " + level + " : frame set element, just return ");
                } else if (tag.Equals("frame")) {
                    // although frame is seldom used, but it is better to support this. 
                    HTMLFrameElement frame = elem as HTMLFrameElement;
                    IHTMLWindow2 fwin2 = frame.contentWindow;

                    IHTMLDocument2 d2 = CrossFrameIE.GetDocumentFromWindow(fwin2);
                    IHTMLRect rect = elem2.getBoundingClientRect();
                    Point fp = new Point(cp.X - rect.left, cp.Y - rect.top);

                    //elem2 = d2.elementFromPoint(fp.X, fp.Y) as IHTMLElement2;
                    //updateCapturedElem(capturedElement, elem2);
                    updateIHTMLElement2ByPos(fp, capturedElement, d2, d2, level + 1);
                    //capturedElement.Left += rect.left;
                    //capturedElement.Top += rect.top;

                    String str = "level = " + level + " : frame : doc2, frame pos = (" + rect.left + "," + rect.top + "), frame size = (" + (rect.right - rect.left) + "," + (rect.bottom - rect.top) + "), frame cursor pos = (" + fp.X + "," + fp.Y + "), caputred element = " + capturedElement;
                    Log.println_hook(str);
                } else if (!tag.Equals("html")) {
                    // fileter the frame condition, sometimes it will capture all html object
                    // here is handle the top level document contained elements 
                    CaptureUtil.updateCapturedElem(capturedElement, elem2, cbxdoc2);
                    if (capturedElement.Flashed == true) {
                        capturedElement.Hotpoint = cp;
                    }
                    
                    Log.println_hook("level = " + level + " : normal html element, cp = " + cp + ", capture elem = " + capturedElement);
                }
            }
        }
        /// <summary>
        /// get the iframe element's offset in its container, return Point.EMPTY if errors 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static Point getIFrameOffset(IHTMLElement elem) {
            if (elem == null || !elem.tagName.Equals("iframe", StringComparison.CurrentCultureIgnoreCase)) {
                return Point.Empty;
            }

            Point p = new Point(0, 0);
            IHTMLElement2 elem2 = elem as IHTMLElement2;
            IHTMLRect rect = elem2.getBoundingClientRect();
            IHTMLDocument3 doc3 = elem.document as IHTMLDocument3;
            IHTMLElement2 he2 = doc3.getElementsByTagName("HTML").item(0) as IHTMLElement2;

            p.X = rect.left + he2.scrollLeft;
            p.Y = rect.top + he2.scrollTop;
            return p;
        }
        /// <summary>
        /// get cover box obj or return null if errors 
        /// </summary>
        /// <param name="doc2"></param>
        /// <returns></returns>
        private static CoverBox getCoverBox(IHTMLDocument2 doc2) {
            if (doc2 != null) {
                String id = CoverBox.getId();
                IHTMLDocument3 doc3 = doc2 as IHTMLDocument3;
                IHTMLElement left = doc3.getElementById(CoverBox.CB_KEY_LEFT + id);
                if (left == null) {
                    CoverBox.create(doc2);
                    left = doc3.getElementById(CoverBox.CB_KEY_LEFT + id);
                    if (left == null) {
                        //TODO log errors                         
                        return null;
                    }
                }
                IHTMLElement right = doc3.getElementById(CoverBox.CB_KEY_RIGHT + id);
                IHTMLElement top = doc3.getElementById(CoverBox.CB_KEY_TOP + id);
                IHTMLElement bottom = doc3.getElementById(CoverBox.CB_KEY_BOTTOM + id);
                CoverBox cb = new CoverBox();
                cb.build(left, right, top, bottom);
                return cb;
            }

            return null;
        }
        /// <summary>
        /// update the captured Element location info, and cover box info 
        /// </summary>
        /// <param name="capturedElement">a object to take the result, it should not be null</param>
        /// <param name="he2"></param>
        /// <param name="cbxdoc2">it is used to find the cover box </param>
        public static void updateCapturedElem(CapturedElement capturedElement, IHTMLElement2 he2, IHTMLDocument2 cbxdoc2) {
            Log.println_hook("update captured Elem : elem = " + ((IHTMLElement)he2).innerText);
            capturedElement.Elem2 = he2;
            // update size info 
            Rectangle rect = getDocPosition(he2);
            // this is used to handle the area tag, if the shape is circle or polygon, it will
            // show a flashed cover box to mention user that this is not a real shape
            if (isNoneRectArea(he2)) {
                capturedElement.Flashed = true;
            }
            capturedElement.Left += rect.X >= 0 ? rect.X : 0;
            capturedElement.Top += rect.Y >= 0 ? rect.Y : 0;
            capturedElement.Width = rect.Width - 2;
            capturedElement.Height = rect.Height - 2;
                        
            // update cover box 
            IHTMLDocument2 doc2 = cbxdoc2;//((IHTMLElement)he2).document as IHTMLDocument2;
            CoverBox cb = getCoverBox(doc2);
            if (cb != null) {
                capturedElement.CoverBox = cb;
            }
        }
        /// <summary>
        /// whether the element is area tag, but the shape is not rect
        /// </summary>
        /// <param name="elem2"></param>
        /// <returns></returns>
        private static bool isNoneRectArea(IHTMLElement2 elem2) {
            IHTMLElement elem = elem2 as IHTMLElement;
            if (elem2 != null && elem.tagName.ToLower().Equals("area")) {
                string shape = WebUtil.getAttributeValue(elem, "shape");
                if (shape!=null && !shape.ToLower().Equals("rect")) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// get the elem's location and size info base on its parent document's top left corner
        /// or return Rectangle.Empty if errors 
        /// </summary>
        /// <param name="elem2"></param>
        /// <returns></returns>
        private static Rectangle getDocPosition(IHTMLElement2 elem2) {
            if (((IHTMLElement)elem2).tagName.ToLower().Equals("area")) {
                return getAreaDocPosition(elem2);
            } else {
                // This method retrieves an object that exposes the left, top, right, and bottom coordinates of the 
                // union of rectangles relative to the client's upper-left corner. In Microsoft Internet Explorer 5,
                // the window's upper-left is at 2,2 (pixels) with respect to the true client.
                IHTMLRect rect = elem2.getBoundingClientRect();
                Rectangle result = Rectangle.Empty;

                // update the position with scroll
                IHTMLElement elem = elem2 as IHTMLElement;
                IHTMLDocument3 doc3 = elem.document as IHTMLDocument3;
                IHTMLElement2 he2 = doc3.getElementsByTagName("HTML").item(0) as IHTMLElement2;
                IHTMLElement2 body2 = he2.getElementsByTagName("BODY").item(0) as IHTMLElement2;
                result.X = rect.left + he2.scrollLeft+body2.scrollLeft;
                result.Y = rect.top + he2.scrollTop+body2.scrollTop;
                result.Width = rect.right - rect.left;
                result.Height = rect.bottom - rect.top;

                return result;
            }
        }
        /// <summary>
        /// get the area location and position of area tag, if the shape is not rect, 
        /// or Rectangle.Empty if errors 
        /// </summary>
        /// <param name="elem2"></param>
        /// <returns></returns>
        private static Rectangle getAreaDocPosition(IHTMLElement2 elem2) {
            // find the real image for the area 
            IHTMLElement elem = elem2 as IHTMLElement;
            string mapName = WebUtil.getAttributeValue(elem.parentElement, "name");
            IHTMLElement2 img = null;
            IHTMLDocument3 doc3 = elem.document as IHTMLDocument3;
            IHTMLElementCollection ec = doc3.getElementsByTagName("img");
            foreach (IHTMLElement imgelem in ec) {
                string map = WebUtil.getAttributeValue(imgelem, "usemap");
                if (map != string.Empty && map.Equals("#" + mapName)) {
                    img = imgelem as IHTMLElement2;
                    break;
                }
            }
            Rectangle result = Rectangle.Empty;
            // get image size info 
            IHTMLRect rect = img.getBoundingClientRect();

            // update the position with scroll            
            IHTMLElement2 he2 = doc3.getElementsByTagName("HTML").item(0) as IHTMLElement2;
            result.X = rect.left + he2.scrollLeft;
            result.Y = rect.top + he2.scrollTop;
            result.Width = rect.right - rect.left;
            result.Height = rect.bottom - rect.top;

            // update area size
            string coords = WebUtil.getAttributeValue(elem, "coords");
            string shape = WebUtil.getAttributeValue(elem, "shape");
            if (shape.ToLower().Equals("rect")) {
                string[] cos = coords.Split(',');
                if (cos.Length == 4) {
                    int x1 = int.Parse(cos[0]);
                    int y1 = int.Parse(cos[1]);
                    int x2 = int.Parse(cos[2]);
                    int y2 = int.Parse(cos[3]);
                    result.X += x1;
                    result.Y += y1;
                    result.Width = x2 - x1;
                    result.Height = y2 - y1;
                }
            }

            return result;

        }
        /// <summary>
        /// show cover box if the element is selectable based on the CapturedElement, 
        /// if the captured element's cover box is differenct with current coverbox, 
        /// it will hidden the current cover box first. for there are maybe more than one 
        /// cover box in a page for there are maybe some frames. 
        /// </summary>
        /// <param name="currentBox">current displaying cover box</param>
        /// <param name="capturedE">capturedElement</param>
        /// <param name="needScrollTo">whether need to scroll the cover box into client area</param>
        public static void showCoverBox(CoverBox currentBox, CapturedElement capturedE, string color, bool needScrollTo) {
            if (capturedE == null || capturedE.isEmpty()) {
                //TODO log errors 
                Log.println_hook("show cover box, ignored, rect is empty ");
                return;
            }
            if (currentBox == null) {
                currentBox = capturedE.CoverBox;
                if (currentBox == null) {
                    Log.println_hook("show cover box, current box is null ");
                    return;
                }
            }
            if (!currentBox.isTheSame(capturedE.CoverBox)) {
                Log.println_hook("cover box is not the previous one, just hidden the previous cover box , id = " + currentBox);
                currentBox.hidden();
                currentBox = capturedE.CoverBox;
            }
            capturedE.showCoverBox(color);

            if(needScrollTo){
                capturedE.CoverBox.scrollIntoView();
            }
        }
        /// <summary>
        /// whether the IHTMLElement is proper for capture. 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static bool isProperCapturabledElem(IHTMLElement elem) {
            if (elem == null) {
                return false;
            }
            return true;
        }
    }
}
