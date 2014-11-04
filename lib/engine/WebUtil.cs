using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using mshtml;
using System.Drawing;
using WebMaster.lib.hooks;
using WebMaster.lib.engine;
using System.Diagnostics;
using System.Net;

namespace WebMaster.lib.engine
{
    /// <summary>
    /// this class is used to handle the Model and Browser object tracking
    /// and browser operation. 
    /// 
    /// e.g find a WebElement in the browser or click/input on the WebBrowser
    /// </summary>
    public class WebUtil
    {
        #region domain info 
        /// <summary>
        /// Top level international domain surfix. 
        /// </summary>
        private static string[] topDomain = new string[] { ".com",".net",".org",".tel",".biz",".info",".me",".name",".edu",".mobi",".firm",".store",".web",".arts",".rec",".nom"};
        private static string[] countryDomain = new string[] { ".cn", ".co", ".us", ".ca", ".mx", ".ws", ".ag", ".am", ".asia", ".at", ".be", ".bz",".de",".es",".eu",".fm",".fr","gs",".in",".it",".jobs",".jp",".ms",".nl",".nu",".se",".tk",".tw",".vg" };
        private static string[] countryDomain2nd = new string[]{".gov.cn", ".com.cn", ".net.cn", ".org.cn", ".com.au", ".net.au", ".org.au", ".com.ag", ".net.ag", ".org.ag", ".com.br", ".net.br", ".net.br", ".com.bz", ".net.bz", ".com.co", ".net.co",  ".nom.co", ".com.es", ".nom.es", ".org.es", ".co.in", ".firm.in", ".gen.in", ".ind.in", ".net.in", ".org.in", ".com.mx", ".co.nz", ".net.nz", ".org.nz", ".com.tw",  ".idv.tw", ".org.tw", ".co.uk", ".me.uk", ".org.uk", ".vg"};
        #endregion domain info 
        #region contants
        /// <summary>
        /// interval at which to raise the timer event, default is 1s  
        /// </summary>
        public static readonly int TIMER_INTERVAL = 1000;
        /// <summary>
        /// filtered attribute names when do validation. 
        /// </summary>
        private static string[] _attfilters = new string[] { "hidefocus", "contenteditable", "readonly", "start", "indeterminate", "tabindex" };
        /// <summary>
        /// tags that will be ignored when captured
        /// </summary>
        private static readonly string[] ignoreTags = new string[] { "html", "head", "meta", "noframe", "script", "style", "title", "base", "frameset" };
        #endregion
        #region variables 
        /// <summary>
        /// app message default prefix value
        /// </summary>
        private static string appMsgPrefix = null;

        #endregion variables 
        #region html web element fucntions
        /// <summary>
        /// NOTE this method should matched the getValuedAttriubtes() method.
        /// 
        /// get the html element attribute value with the att name. return string value or string.EMPTY string
        /// if not find or errors
        /// </summary>
        /// <param name="elem">IHTMLElement </param>
        /// <param name="attName">attribute name</param>
        /// <returns>attribute value string or empty string if not find</returns>
        public static string getAttributeValue(IHTMLElement elem, string attName) {
            if (elem == null || attName == null) {
                return string.Empty;
            }
            attName = attName.ToLower();
            // check specific attributes 
            if (attName.Equals(Constants.HE_TAG)) {
                return elem.tagName.ToLower();
            }
            if (attName.Equals(Constants.HE_ID)) {
                return elem.id;
            }
            if (attName.Equals(Constants.HE_INNER_TEXT)) {
                return elem.innerText;
            }
            if (attName.Equals(Constants.HE_STYLE)) {
                IHTMLStyle style = elem.style;
                return style == null ? string.Empty : style.cssText;
            }
            // check common attributes 
            IHTMLDOMNode domNode = elem as IHTMLDOMNode;
            IHTMLAttributeCollection acoll = (IHTMLAttributeCollection)domNode.attributes;
            foreach (IHTMLDOMAttribute att in acoll) {
                if (att.nodeName.ToLower().Equals(attName)) {
                    return att.nodeValue+"";
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// return a hashtable that contains all valued attributes for the html element, or an empty
        /// hashtable if not found or errors.
        /// any attributes if the value is empty string will be ignored, and all attributes
        /// value will be translated as string type. 
        /// 
        /// NOTE : this method should match the getAttributeValue() method 
        /// 
        /// attribute name is changed to lower cases if necessary. 
        /// </summary>
        /// <param name="elem">IHTMLElement object</param>
        /// <returns></returns>
        public static HashtableEx getValuedAttriubtes(IHTMLElement elem){                        
            if(elem==null && ignoreTags.Contains(elem.tagName.ToLower())){
                return HashtableEx.EMPTY ;
            }

            HashtableEx table = new HashtableEx();
            table.Add(Constants.HE_TAG,elem.tagName.ToLower());
            if (elem.id != null && elem.id.Length > 0) {
                table.Add(Constants.HE_ID, elem.id);
            }
            string itext = elem.innerText;
            if (itext != null && itext.Length > 0) {
                table.Add(Constants.HE_INNER_TEXT, elem.innerText);
            }
            IHTMLStyle style = elem.style;
            if (style != null && style.cssText != null) {
                table.Add(Constants.HE_STYLE, style.cssText);
            }
            IHTMLDOMNode domNode = elem as IHTMLDOMNode;
            IHTMLAttributeCollection acoll = (IHTMLAttributeCollection)domNode.attributes;
            foreach (IHTMLDOMAttribute att in acoll) {
                string value = att.nodeValue != null ? att.nodeValue.ToString() : "";
                string key = att.nodeName.ToLower();
                if (key.Equals(Constants.HE_TAG) || key.Equals(Constants.HE_ID) || key.Equals(Constants.HE_INNER_TEXT) || key.Equals(Constants.HE_STYLE)) {
                    continue;
                }
                if (isValidAttribute(att)) {                    
                    table.Add(key, value);
                }
            }
            return table;
        }
        /// <summary>
        /// filter the some useless dom attributes, such as aris-*, and others useless attributes 
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        /// <summary>
        private static bool isValidAttribute(IHTMLDOMAttribute att) {
            // validate parameters 
            if (att == null || att.nodeValue == null) {
                return false;
            }
            string nv = att.nodeValue + "";
            nv = nv.ToLower();
            if (nv.Trim().Length < 1) {
                return false;
            }
            string key = att.nodeName.ToLower().Trim();

            // filter useless aria-* items 
            if (key.StartsWith("aria", StringComparison.CurrentCultureIgnoreCase) && nv.Trim().Equals("0")) {
                return false;
            }            
            // filter ignored attributes 
            if (_attfilters.Contains(key)) {
                return false;
            }
            // fitler specific attributes with values 
            if (("width".Equals(key) || "height".Equals(key) || "size".Equals(key) || "hspace".Equals(key) || "vspace".Equals(key)) && nv.Equals("0")) {
                return false;
            }
            if ("maxlength".Equals(key) && nv.Equals("2147483647")) {
                return false;
            }
            if (("rowspan".Equals(key) || "colspan".Equals(key) || "loop".Equals(key)) && nv.Equals("1")) {
                return false;
            }
            if (("disabled".Equals(key) || "nohref".Equals(key) || "nowrap".Equals(key) || "checked".Equals(key)) && nv.Equals("false", StringComparison.CurrentCultureIgnoreCase)) {
                return false;
            }
            // filter by value 
            string value = att.nodeValue.ToString().Trim();
            if (value.Equals("System.__ComObject")) {
                return false;
            }

            return true;
        }
        /// <summary>
        /// get the WebElementAttribute object by the key, return null if not found 
        /// </summary>
        /// <param name="we">container WebElement</param>
        /// <param name="key">WebElementAttribute key</param>
        /// <returns></returns>
        public static WebElementAttribute getWebElementAttribute(WebElement we, string key) {
            if (we == null || key == null || key.Length<1) {
                return null;
            }
            foreach (WebElementAttribute wea in we.Attributes) {
                if (key.Equals(wea.Key)) {
                    return wea;
                }
            }

            return null;
        }
        /// <summary>
        /// find and update the real HtmlElement element attributes into WebElement if find. 
        /// if find the we.IsRealElement = true
        /// </summary>
        /// <param name="we"></param>
        /// <param name="browser"></param>
        public static void updateWebElement(WebElement we, WebBrowser browser) {
            IHTMLElement ihtml = getFirstIHTMLElement(we, browser);
            if (ihtml != null) {
                updateWebElement(we, ihtml);
            }
        }
        /// <summary>
        /// update the real IHTMLElement attributes into WebElement if not null.
        /// if ok, the we.IsRealElement = true
        /// </summary>
        /// <param name="we"></param>
        /// <param name="iHtmlElem"></param>
        public static void updateWebElement(WebElement we, IHTMLElement iHtmlElem) {            
            if (iHtmlElem != null && we!=null) {
                we.IsRealElement = true;
                foreach (WebElementAttribute wea in we.Attributes) {
                    string rvalue = getAttributeValue(iHtmlElem, wea.Key);
                    if (rvalue != string.Empty) {
                        wea.RValue = rvalue;
                    }
                }
            }
        }
        /// <summary>
        /// update the frames info as refWebElement for element if needed. 
        /// </summary>
        /// <param name="capturedE">that contains the frame info </param>
        /// <param name="we"></param>
        /// <param name="sroot">it is used to check whether the frame WebElement existed in the script</param>
        public static void updateRefFrames(CapturedElement capturedE, WebElement we, ScriptRoot sroot) {
            if (capturedE == null || capturedE.frames.Count < 1 || we == null) {
                return;
            }
            WebElement cwe = we;
            for (int i = 0; i < capturedE.frames.Count; i++) {
                WebElement fwe = ModelFactory.createWebElement();
                updateFrameWE(fwe,capturedE.frames[i]);
                bool find = false;
                foreach (WebElement tfwe in sroot.IFrames) {
                    if (tfwe.Name.Equals(fwe.Name)) {
                        find = true;
                        cwe.refWebElement = tfwe ;
                        break;
                    }    
                }
                if (!find) {
                    sroot.IFrames.AddUnique(fwe);
                    cwe.refWebElement = fwe;
                    cwe = fwe;
                } else {
                    // if find, it means that the iframe web element hirachy should be build up 
                    // before, just return 
                    break;
                }                
            }
        }
        /// <summary>
        /// update the html frame info into WebElement, the name is frame id+src value
        /// </summary>
        /// <param name="fwe"></param>
        /// <param name="frame"></param>
        private static void updateFrameWE(WebElement fwe, IHTMLElement frame) {
            //fwe.Tag = frame.tagName.ToLower();
            //fwe.ID = frame.id == null ? null : frame.id;
            HashtableEx atts = getValuedAttriubtes(frame);

            if (atts != HashtableEx.EMPTY) { //build table 
                foreach (object o in atts) {
                    string key = Convert.ToString(o);
                    string value = Convert.ToString(atts.Get(o));

                    WebElementAttribute wea = ModelFactory.createWebElementAttribute();
                    wea.Key = key;
                    wea.PValues.Add(value);
                    wea.PATTERN = CONDITION.STR_FULLMATCH;
                    fwe.Attributes.AddUnique(wea);
                    // here for a iframe, use the id+src value as name 
                    if("src".Equals(key.ToLower())){
                        fwe.Name = fwe.ID + value;
                    }
                }
            }
        }
        /// <summary>
        /// get the first matched IHTMLElement defined by we, or null if not find 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="browser"></param>
        /// <returns>the first IHTMLElement find or null if not find </returns>
        public static IHTMLElement getFirstIHTMLElement(WebElement we, WebBrowser browser) {
            List<IHTMLElementWrap> list = getIHTMLElements(we, browser, true);
            if (list.Count > 0) {
                return list[0].IHTMLElem;
            }
            return null;
        }
        /// <summary>
        /// return all IHTMLElement arrays that matched WebElement definition in current browser document, or empty list, 
        /// It will retrieve all the element in the browser document, for frame/iframe, if it is defined in we.refElement
        /// if will be retrieved, else it will be ignored. 
        /// 
        /// For refWebElement, it is just check the first one as expected. 
        /// 
        /// if the onlyFirst parameter is true, just return the first one that matched the condition. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="browser"></param>
        /// <param name="onlyFirst">whether to just return the first IHTMLElement founded</param>
        /// <returns>IHTMLElement list or empty list if not find </returns>
        public static List<IHTMLElementWrap> getIHTMLElements(WebElement we, WebBrowser browser, bool onlyFirst) {
            if (we == null || browser == null || browser.Document == null) {
                return new List<IHTMLElementWrap>();
            }
            Point p = new Point(0,0);
            IHTMLDocument2 idoc2 = null;
            return getIHTMLElements(we, browser, onlyFirst,ref p, ref idoc2);
        }
        /// <summary>
        /// return all IHTMLElement arrays that matched WebElement definition in current browser document, or empty list, 
        /// It will retrieve all the element in the browser document, for frame/iframe, if it is defined in we.refElement
        /// if will be retrieved, else it will be ignored. 
        /// 
        /// update the cbxidoc if the element is under a frame element, or cbxdoc2 will be null.
        /// 
        /// if the onlyFirst parameter is true, just return the first one that matched the condition. 
        /// 
        /// For refWebElement, it is just check the first one as expected. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="browser"></param>
        /// <param name="onlyFirst">whether to just return the first IHTMLElement founded</param>
        /// <param name="offset">it is used to record the selected HTMLElement's offset, it is take effective if its refWebElement
        /// is an iframe</param>
        /// <param name="cbxdoc2">which IHTMLDocument2 is the cover box container</param>
        /// <returns>IHTMLElement list or empty list if not find </returns>
        public static List<IHTMLElementWrap> getIHTMLElements(WebElement we, WebBrowser browser, bool onlyFirst, ref Point offset, ref IHTMLDocument2 cbxdoc2) {
            List<IHTMLElementWrap> list = new List<IHTMLElementWrap>();
            
            if ((we!=null && (we.TYPE == WEType.CODE || we.TYPE == WEType.ATTRIBUTE)) || (browser!=null && browser.Document!=null)) {                
                IHTMLDocument idoc = browser.Document.DomDocument as IHTMLDocument;
                
                // just check the browser document level 
                if (we.refWebElement == null) {                    
                    // get the only one HtmlElement by id if find 
                    if (we.ID != null && we.ID.Length > 0) {
                        HtmlElement he = browser.Document.GetElementById(we.ID);
                        if (he!=null && isValidHtmlElement(he.DomElement as IHTMLElement,we)) {
                            IHTMLElementWrap wrap = createIHTMLElementWraper(he.DomElement as IHTMLElement);
                            list.Add(wrap);
                        }
                    } else {
                        list.AddRange(doGetIHTMLElements(we,idoc,onlyFirst));
                    }
                } else { // there is a container element for the WebElement 
                    List<IHTMLElementWrap> clist = getIHTMLElements(we.refWebElement, browser,true, ref offset, ref cbxdoc2);
                    IHTMLElement refHE = null;
                    if(clist.Count>0){
                        refHE = clist[0].IHTMLElem ;
                    }
                    if(refHE!=null){
                        Point fp = CaptureUtil.getIFrameOffset(refHE);
                        offset.X += fp.X;
                        offset.Y += fp.Y;
                        // just check the innerest frame is the cbx idocument2. 
                        if (we.refWebElement.Tag.Equals("frame") && cbxdoc2 != null) {
                            HTMLFrameElement frame = refHE as HTMLFrameElement;
                            IHTMLWindow2 fwin2 = frame.contentWindow;
                            IHTMLDocument2 d2 = CrossFrameIE.GetDocumentFromWindow(fwin2);
                            cbxdoc2 = d2;
                        }
                        list.AddRange(doGetIHTMLElements(we,refHE,onlyFirst));
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// return all IHTMLElement arrays that matched WebElement definition in current document, or empty list, 
        /// it will retrieve all the element in the document, for frame/iframe, if it is defined in we.refElement
        /// if will be retrieved, else it will be ignored. 
        /// if the onlyFirst parameter is true, just return the first one that matched the condition. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="idoc"></param>
        /// <param name="onlyFirst">whether to just return the first IHTMLElement founded</param>
        /// <param name="offset">it is used to record the selected HTMLElement's offset, it is take effective if its refWebElement
        /// is an iframe</param>
        /// <param name="cbxdoc2">which IHTMLDocument2 is the cover box container</param>
        /// <returns></returns>
        public static List<IHTMLElementWrap> getIHTMLElements(WebElement we, IHTMLDocument idoc, bool onlyFirst, ref Point offset, ref IHTMLDocument2 cbxdoc2) {
            List<IHTMLElementWrap> list = new List<IHTMLElementWrap>();
            if ((we != null && (we.TYPE == WEType.CODE || we.TYPE == WEType.ATTRIBUTE)) || idoc != null) {
                // just check the browser document level 
                if (we.refWebElement == null) {
                    // get the only one HtmlElement by id if find 
                    if (we.ID != null && we.ID.Length > 0) {
                        IHTMLDocument3 idoc3 = idoc as IHTMLDocument3;
                        IHTMLElement he = idoc3.getElementById(we.ID);
                        if (he != null && isSizedHtmlElement(he)) {
                            IHTMLElementWrap wrap = createIHTMLElementWraper(he);
                            list.Add(wrap);
                        }
                    } else {
                        list.AddRange(doGetIHTMLElements(we,idoc,onlyFirst));
                    }
                } else { // there is a container element for the WebElement 
                    List<IHTMLElementWrap> clist = getIHTMLElements(we.refWebElement, idoc, true, ref offset, ref cbxdoc2);
                    IHTMLElement refHE = null;
                    if (clist.Count > 0) {
                        refHE = clist[0].IHTMLElem;
                    }
                    if (refHE != null) {
                        Point fp = CaptureUtil.getIFrameOffset(refHE);
                        offset.X += fp.X;
                        offset.Y += fp.Y;
                        
                        // just check the innerest frame is the cbx idocument2. 
                        if (we.refWebElement.Tag.Equals("frame") && cbxdoc2 != null) {
                            HTMLFrameElement frame = refHE as HTMLFrameElement;
                            IHTMLWindow2 fwin2 = frame.contentWindow;
                            IHTMLDocument2 d2 = CrossFrameIE.GetDocumentFromWindow(fwin2);
                            cbxdoc2 = d2;
                        }
                        list.AddRange(doGetIHTMLElements(we,refHE,onlyFirst));
                    }
                }
            }
            
            return list;
        }

        private static IHTMLElementWrap createIHTMLElementWraper(IHTMLElement he) {
            if (he != null) {
                IHTMLElementWrap wrap = ModelFactory.createIHTMLElementWrap();
                wrap.IHTMLElem = he;
                return wrap;
            }
            return null;
        }
        ///// <summary>
        ///// return all IHTMLElements matched we definition under container.         
        ///// if the onlyFirst parameter is true, just return the first one that matched the condition. 
        ///// </summary>
        ///// <param name="we"></param>
        ///// <param name="container"></param>
        ///// <param name="onlyFirst">whether to just return the first IHTMLElement founded</param>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //internal static List<IHTMLElement> getIHTMLElements(WebElement we, IHTMLElement container, bool onlyFirst, ref Point offset) {
        //    List<IHTMLElement> list = new List<IHTMLElement>();
        //    if ((we != null && (we.TYPE == WEType.CODE || we.TYPE == WEType.ATTRIBUTE)) || container != null) {
        //        IHTMLElement2 ce2 = container as IHTMLElement2;
        //        // just check the browser document level 
        //        if (we.refWebElement == null) {
        //            list.AddRange(doGetIHTMLElements(we, container, onlyFirst));
        //        } else { // there is a container element for the WebElement 
        //            List<IHTMLElement> clist = getIHTMLElements(we.refWebElement, container, true, ref offset);
        //            IHTMLElement refHE = null;
        //            if (clist.Count > 0) {
        //                refHE = clist[0];
        //            }
        //            if (refHE != null) {
        //                Point fp = CaptureUtil.getIFrameOffset(refHE);
        //                offset.X += fp.X;
        //                offset.Y += fp.Y;
        //                list.AddRange(getIHTMLElements(we, refHE, onlyFirst, ref offset));
        //            }
        //        }
        //    }

        //    return list;
        //}
        /// <summary>
        /// get all IHTMLElements under the document, or empty list if not find. It will ignore the
        /// refWebElement if have, just check the webElement itself. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="idoc"></param>
        /// <param name="onlyFirst"></param>
        /// <returns></returns>
        private static List<IHTMLElementWrap> doGetIHTMLElements(WebElement we, IHTMLDocument idoc, bool onlyFirst) {
            List<IHTMLElementWrap> list = new List<IHTMLElementWrap>();
            if (we != null && idoc != null) {
                IHTMLDocument3 idoc3 = idoc as IHTMLDocument3;
                // get the only one HtmlElement by id if find 
                if (we.ID != null && we.ID.Length > 0) {                    
                    IHTMLElement he = idoc3.getElementById(we.ID);
                    if (isValidHtmlElement(he,we)) {
                        IHTMLElementWrap wrap = createIHTMLElementWraper(he);
                        list.Add(wrap);
                    }
                    return list;
                } else {
                    IHTMLElementCollection ihc = idoc3.getElementsByTagName(we.Tag);
                    foreach (IHTMLElement he in ihc) {
                        if (isValidHtmlElement(he, we)) {
                            IHTMLElementWrap wrap = createIHTMLElementWraper(he);
                            list.Add(wrap);
                            if (onlyFirst) {
                                return list;
                            }
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// get all IHTMLElements under the container element, or empty list if not find. It will ignore the
        /// refWebElement if have, just check the webElement itself. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="container"></param>
        /// <param name="onlyFirst"></param>
        /// <returns></returns>
        private static List<IHTMLElementWrap> doGetIHTMLElements(WebElement we, IHTMLElement container, bool onlyFirst) {
            List<IHTMLElementWrap> list = new List<IHTMLElementWrap>();
            if (we != null && container != null) {
                if (container.tagName.Equals("iframe", StringComparison.CurrentCultureIgnoreCase)) {
                    HTMLIFrame iframe = container as HTMLIFrame;
                    IHTMLWindow2 fwin2 = iframe.contentWindow;
                    IHTMLDocument2 d2 = CrossFrameIE.GetDocumentFromWindow(fwin2);
             
                    return doGetIHTMLElements(we, d2 as IHTMLDocument, onlyFirst);
                }
                // get the only one HtmlElement by id if find 
                if (we.ID != null && we.ID.Length > 0) {
                    IHTMLDocument3 idoc3 = container.document as IHTMLDocument3;
                    IHTMLElement he = idoc3.getElementById(we.ID);
                    if (isValidHtmlElement(he,we)) {
                        IHTMLElementWrap wrap = createIHTMLElementWraper(he);
                        list.Add(wrap);
                    }
                    return list;
                } else {
                    IHTMLElement2 ce2 = container as IHTMLElement2;
                    IHTMLElementCollection ihc = ce2.getElementsByTagName(we.Tag);
                    foreach (IHTMLElement he in ihc) {
                        if (isValidHtmlElement(he, we)) {
                            IHTMLElementWrap wrap = ModelFactory.createIHTMLElementWrap();
                            wrap.IHTMLElem = he;
                            list.Add(wrap);
                            if (onlyFirst) {
                                return list;
                            }
                        }
                    }
                }

            }
            return list;
        }
        /// <summary>
        /// return true if the htmlElem is visiable/has size, or false if not. 
        /// </summary>
        /// <param name="htmlElem"></param>
        /// <returns></returns>
        private static bool isCandidateHtmlElement(HtmlElement htmlElem) {
            if (htmlElem != null && htmlElem.DomElement != null && isCandidateIHtmlElement(htmlElem.DomElement as IHTMLElement)) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// return true if the ihtmlElem is visiable/has size, or false if not. 
        /// </summary>
        /// <param name="ihtmlElem"></param>
        /// <returns></returns>
        private static bool isCandidateIHtmlElement(IHTMLElement ihtmlElem) {
            if (ihtmlElem != null && isVisibleHtmlElement(ihtmlElem as IHTMLElement2) && isSizedHtmlElement(ihtmlElem)) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// return true if htmlElem2 is visible or false if not
        /// </summary>
        /// <param name="htmlElem2"></param>
        /// <returns></returns>
        private static bool isVisibleHtmlElement(IHTMLElement2 htmlElem2) {
            if (htmlElem2 == null) {
                return false;
            }
            if (htmlElem2 != null) {
                if (htmlElem2.currentStyle.display != null ){
                    if (htmlElem2.currentStyle.display.Equals("none", StringComparison.CurrentCultureIgnoreCase)) {
                        return false;
                    }
                }
                if(htmlElem2.currentStyle.visibility != null){
                    if (htmlElem2.currentStyle.visibility.Equals("hidden", StringComparison.CurrentCultureIgnoreCase)) {
                        return false;
                    }
                    //if (htmlElem2.currentStyle.visibility.Equals("inherit", StringComparison.CurrentCultureIgnoreCase)) { 
                    //    IHTMLElement elem = htmlElem2 as IHTMLElement ;
                    //    return isVisibleHtmlElement(elem.parentElement as IHTMLElement2);
                    //}
                }
            }
            return true;
        }
        /// <summary>
        /// whether the elemenet has size, if its width and height both bigger than zero, return true, else return false 
        /// In fact, I'm not exactly why the element size is zero. maybe I can discover it later. 
        /// </summary>
        /// <param name="ielem"></param>
        /// <returns></returns>
        private static bool isSizedHtmlElement(IHTMLElement ielem) {
            if (ielem != null) {
                IHTMLElement2 elem2 = ielem as IHTMLElement2;
                IHTMLRect rect = elem2.getBoundingClientRect();
                if (rect != null && rect.bottom - rect.top > 0 && rect.right - rect.left > 0) {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// whether the html element is width and height both are bigger than zero and adjust 
        /// if the elem is the element modeled by the WebElement 
        /// </summary>
        /// <param name="he">real html element</param>
        /// <param name="we">WebElement</param>
        /// <returns>true if the elem is the element modeled by webelement or return false </returns>
        internal static bool isValidHtmlElement(IHTMLElement he, WebElement we) {
            if (he == null || we == null || !isCandidateIHtmlElement(he)) {
                return false;
            }
            foreach (WebElementAttribute wea in we.Attributes) {
                string rvalue = WebUtil.getAttributeValue(he, wea.Key);
                if (!ModelManager.Instance.isValidWEAValue(wea, rvalue)) {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// WebElelement type is a WEType.CODE type, it will be updated as a Attribute type. 
        /// It means that the ATTR of the webElement will be created
        /// </summary>
        /// <param name="we"></param>
        public static void convertCodeToAttribute(WebElement we) {
            if (we.TYPE != WEType.CODE) {
                //TODO LOG
                return;
            }

            string str = we.FeatureString;
            int blockStart = 0;
            int blockEnd = str.Length - 1;
            int sepIndex = 0;
            // the first char must be { and the last char must be }
            if (str[blockStart] != '{' || str[blockEnd] != '}' || str.Length < 5) {
                // LOG
                return;
            }
            Stack<bool> stack = new Stack<bool>();

            for (int i = 0; i < str.Length; i++) {
                if (str[i] == '{') {
                    if (stack.Count == 0) {
                        blockStart = i;
                    }
                    stack.Push(true);
                }
                if (str[i] == '}') {
                    stack.Pop();
                    if (stack.Count == 0) {
                        blockEnd = i;
                        sepIndex = str.IndexOf(':', blockStart);
                        if (sepIndex > 0 && sepIndex < blockEnd) {
                            string key = str.Substring(blockStart + 1, sepIndex - blockStart - 1);
                            if (String.Compare(key, Constants.HE_TAG, true) == 0 || String.Compare(key, Constants.HE_ID, true) == 0) {
                                continue;
                            }
                            string value = str.Substring(sepIndex + 1, blockEnd - sepIndex - 1);
                            WebElementAttribute wea = ModelFactory.createWebElementAttribute();
                            wea.Key = key;
                            wea.PValues.Add(value);
                            wea.PATTERN = CONDITION.STR_FULLMATCH;

                            we.Attributes.AddUnique(wea);
                        } else {
                            //LOG
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Return true if the he element is an input password, or return false 
        /// </summary>
        /// <param name="he"></param>
        /// <returns></returns>
        public static bool isPassword(IHTMLElement he) {
            if (he != null && he.tagName.ToLower().Equals("input")) {
                HTMLInputElement input = he as HTMLInputElement;
                if (input.type != null && input.type.ToLower() == "password") {
                    return true;
                }
            }
            return false;
        }
        #endregion 
        #region web operations
        /// <summary>
        /// invoke click method on the html element 
        /// </summary>
        /// <param name="htmlElem">to be clicked element</param>
        /// <param name="click">it contains the click details info</param>
        public static void mouseClick(IHTMLElement htmlElem, Click click) {
            if (htmlElem == null) {
                Log.println_eng("T = UI,     !!! Error, to be clicked element is null ");
                return;
            }
            IHTMLElement3 elem3 = htmlElem as IHTMLElement3 ;
            //if (isButton(htmlElem) && click.Key == Keys.None && click.Dbclick == false && click.Type== CLICK_TYPE.CLICK) { 
                
            //}
            // build up event object 
            IHTMLDocument4 doc4 = htmlElem.document as IHTMLDocument4;
            object refObj = null;
            IHTMLEventObj2 evtObj2 = doc4.CreateEventObject(ref refObj) as IHTMLEventObj2;
            evtObj2.button = (int)click.Button;
            if (click.Key == Keys.Alt) {
                evtObj2.altKey = true;
            }
            if (click.Key == Keys.Control) {
                evtObj2.ctrlKey = true;
            }
            if (click.Key == Keys.Shift) {
                evtObj2.shiftKey = true;
            }
            // fire click event
            object eventRef = evtObj2;
            // here align with the mouse execute order, mouse down, mouse up, click. 
            if (click.Type == CLICK_TYPE.MOUSE_DOWN) {
                Log.println("T = UI,     *** mouse down clicked, elem.tag=" + htmlElem.tagName);
                elem3.FireEvent("onmousedown", ref eventRef);
            } else if (click.Type == CLICK_TYPE.MOUSE_UP) {
                Log.println("T = UI,     *** mouse up clicked, elem.tag=" + htmlElem.tagName);
                elem3.FireEvent("onmouseup", ref eventRef);
            } else if (click.Type == CLICK_TYPE.DBCLICK) {
                Log.println("T = UI,     *** mouse double clicked, elem.tag=" + htmlElem.tagName);
                elem3.FireEvent("ondblclick", ref eventRef);
            } else if (click.Type == CLICK_TYPE.CLICK) {
                Log.println("T = UI,     *** mouse clicked, elem.tag=" + htmlElem.tagName);
                htmlElem.click();
                //elem3.FireEvent("onclick", ref eventRef);
            }
        }
        /// <summary>
        /// check whether the html element is a input button
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private static bool isButton(IHTMLElement elem) {
            if (elem != null) {
                string tagname = elem.tagName.ToLower();
                if (tagname.Equals("input")) {
                    String type = elem.getAttribute("type") as string;
                    type = type != null ? type.ToLower() : null;
                    if ("button".Equals(type) || "submit".Equals(type)) {
                        return true;
                    }
                }
                if (tagname.Equals("button")) {
                    return true;
                }
            }
            
            return false;
        }
        public static void mouseOver(HtmlElement element) {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlElem"></param>
        /// <param name="input">input must be string or Parameter</param>
        public static void textInput(IHTMLElement htmlElem,object input) {
            if (htmlElem != null && input != null) {
                string text = getInputText(input);
                if(htmlElem.tagName.Equals("textarea", StringComparison.CurrentCultureIgnoreCase)) {
                    htmlElem.innerText = text;                    
                } else if (htmlElem.tagName.Equals("input", StringComparison.CurrentCultureIgnoreCase)) {
                    htmlElem.setAttribute("value", text);
                }
            }
        }
        /// <summary>
        /// get the input object's text value, 
        /// </summary>
        /// <param name="input">input should be a string or Parameter</param>
        /// <returns></returns>
        private static string getInputText(object input) {
            string text = "";
            if (input is string) {
                text = input as string;
            } else if (input is Parameter) { 
                //TODO 
            }

            return text;
        }

        public static void openURLInCurrentTab(WebBrowser browser, string url) {
            if (browser != null && url != null && url.Length > 0) {
                browser.Navigate(url);
            }
        }

        public static void openURLInNewTab(WebBrowser browser, string url) {
            throw new NotImplementedException();
        }
        
        #endregion web oprations
        #region WebElement with Location and Image

        private static bool isValidLocationElement(WebElement we, WebBrowser webBrowser1) {
            throw new NotImplementedException();
        }

        private static bool isValidImgElement(WebElement we, WebBrowser webBrowser1) {
            throw new NotImplementedException();
        }
        #endregion WebElement with Location and Image
        #region logs
        internal static string getAppMsgPrefix() {
            if (appMsgPrefix == null) {
                string blankText = LangUtil.getMsg("log.blankspace.text");
                // initialize appMsgPrefix value 
                string time = DateTime.Now.ToString();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < time.Length; i++) {
                    sb.Append(blankText);
                }
                appMsgPrefix = sb.Append(" ").ToString();
            }
            return appMsgPrefix;
        }
        /// <summary>
        /// Get the blanck space text with locale info 
        /// </summary>
        /// <returns></returns>
        public static string getBlankSpaceText() {
            //TODO 这里有些问题，中文的空格字符和英文的空格没法对齐的问题，以后再说了
            return LangUtil.getMsg("log.blankspace.text");
        }
        /// <summary>
        /// get the bool text with locale info
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string getBoolText(bool result) {
            if (result) {
                return LangUtil.getMsg("log.bool.true.text");
            }
            return LangUtil.getMsg("log.bool.false.text");
        }
        /// <summary>
        /// Get OpWrapper status text with locale 
        /// </summary>
        /// <param name="opStatus"></param>
        /// <returns></returns>
        internal static string getStatusText(OpStatus opStatus) {
            string text = LangUtil.getMsg("log.opw.status.invalid");
            if (opStatus == OpStatus.CON_WE_FOUND) {
                text = LangUtil.getMsg("log.opw.status.cwf");
            } else if (opStatus == OpStatus.CON_WE_NOT_FOUND) {
                text = LangUtil.getMsg("log.opw.status.cwnf");
            } else if (opStatus == OpStatus.EXE_ERROR) {
                text = LangUtil.getMsg("log.opw.status.exerr");
            } else if (opStatus == OpStatus.OP_WE_NOT_FOUND) {
                text = LangUtil.getMsg("log.opw.status.openf");
            } else if (opStatus == OpStatus.OPC_PARAM_MAPPING_WE_FOUND) {
                text = LangUtil.getMsg("log.opw.status.opmwf");
            } else if (opStatus == OpStatus.OPC_PARAM_MAPPING_WE_NOT_FOUND) {
                text = LangUtil.getMsg("log.opw.status.opmwnf");
            } else if (opStatus == OpStatus.PROC_END) {
                text = LangUtil.getMsg("log.opw.status.procend");
            } else if (opStatus == OpStatus.READY) {
                text = LangUtil.getMsg("log.opw.status.ready");
            } else if (opStatus == OpStatus.REQ_TIME_OUT) {
                text = LangUtil.getMsg("log.opw.status.timeout");
            } else if (opStatus == OpStatus.RESTART_SCRIPT) {
                text = LangUtil.getMsg("log.opw.status.restart");
            } else if (opStatus == OpStatus.STOP) {
                text = LangUtil.getMsg("log.opw.status.stop");
            } else if (opStatus == OpStatus.UPDATE_PARAM_WE_FOUND) {
                text = LangUtil.getMsg("log.opw.status.upwf");
            } else if (opStatus == OpStatus.UPDATE_PARAM_WE_NOT_FOUND) {
                text = LangUtil.getMsg("log.opw.status.upwnf");
            }

            return text;
        }
        /// <summary>
        /// return the app message prefix or error with string.Empty. 
        /// </summary>
        /// <param name="logType">LOG_APP_MSG_OP,LOG_APP_MSG_OPC,LOG_APP_MSG_CON</param>
        /// <returns></returns>
        public static string getAppLogPrefix(int logType) {
            string prefix = getAppMsgPrefix();
            if (logType == Logger.LOG_APP_MSG_OP) {
                string time = DateTime.Now.ToString();
                string str = LangUtil.getMsg("log.op.prefix");
                return time + "   " + str + " = \"";
            } else if (logType == Logger.LOG_APP_MSG_PROC) {
                string time = DateTime.Now.ToString();
                string str = LangUtil.getMsg("log.proc.prefix");
                return time + "   " + str + " = \"";
            } else if (logType == Logger.LOG_APP_MSG_OPC) {
                string str = LangUtil.getMsg("log.opc.prefix");
                return prefix + "    | " + str + " = \"";
            } else if (logType == Logger.LOG_APP_MSG_CON) {
                //TODO
                return prefix + "?????";
            } else if (logType == Logger.LOG_APP_MSG_CONGRP) {
                //TODO 
                return prefix + "??????";
            } else if (logType == Logger.LOG_APP_MSG_OP_PARAM_UPD) {
                string str = LangUtil.getMsg("log.op.update.prefix"); // Update Parameters
                return prefix + "    ***>> " + str;
            } else if (logType == Logger.LOG_APP_MSG_OPC_PARAM_UPD) {
                string str = LangUtil.getMsg("log.opc.update.prefix"); // Update Parameters
                return prefix + "    ***>> " + str;
            } else if (logType == Logger.LOG_APP_MSG_PARAM_CMD) {
                string str = LangUtil.getMsg("log.cmd.prefix"); // Command - 
                return prefix + "       >>> " + str;
            }
            return string.Empty;
        }
        
        #endregion logs 
        #region utils
        /// <summary>
        /// Get the server time with the specified url or return DateTime.MinValue if errors. 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static DateTime getServerTime(string url) {
            if (url != null && url.Length > 0) {
                Stopwatch sw = new Stopwatch();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                sw.Start();
                HttpWebResponse res;
                try {
                    res = (HttpWebResponse)req.GetResponse();
                } catch (WebException) {
                    return DateTime.MinValue;
                }
                DateTime st = DateTime.MinValue;
                DateTime.TryParse(res.Headers["Date"], out st);
                res.Close();
                if (st != DateTime.MinValue) {
                    long delay = (long)sw.ElapsedMilliseconds / 2;
                    st.AddMilliseconds(delay);
                    return st;
                }
            }
            return DateTime.MinValue;
        }
        /// <summary>
        /// Return true if date2 is after date1 by only Date(yyyy-mm-dd) false if not. 
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        internal static bool isDateAfter(DateTime date1, DateTime date2) {
            int y1 = date1.Year;
            int y2 = date2.Year;
            if (y2 > y1) {
                return true;
            } else {
                int m1 = date1.Month;
                int m2 = date2.Month;
                if (m2 > m1) {
                    return true;
                } else {
                    int d1 = date1.Day;
                    int d2 = date2.Day;
                    return d2 > d1;
                }
            }
        }

        /// <summary>
        /// check whether the url1 and url2 the same one, e.g http://xxx.com and xxx.com is the same one
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static bool isTheSameURL(string url1, string url2){
            string u1 = getPureAddress(url1);
            string u2 = getPureAddress(url2);
            return u1.Equals(u2);
        }
        /// <summary>
        /// filter the http://, https:// and www. from the begining of url, return pure address or string.Empty if errors. 
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string getPureAddress(string url) {
            if(url == null /*|| !(url.StartsWith("http://") || url.StartsWith("https://"))*/){
                return "" ;
            }
            string address = string.Empty;
            // filter http:// or https:// 
            if (url.StartsWith("http://")) {
                url = url.Substring(7);
            } else if (url.StartsWith("https://")) {
                url = url.Substring(8);
            }
            // filter www. if have 
            address = url;
            if(url.StartsWith("www.")){
                address = url.Substring(4);
            }

            return address ;
        }
        /// <summary>
        /// Get the domain name of the url. e.g http://www.bbs.autowebkit.com/index.php, domain name is bbs.autowebkit.com
        /// or return string.Empty if errors 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static string getDomainName(string url) {
            if (url != null) {
                string u1 = getPureAddress(url);
                // find the first '/'
                if (u1.Length > 0) {
                    int index = u1.IndexOf('/');
                    int length = u1.Length;
                    if (index != -1) {
                        length = index;                        
                    }
                    return u1.Substring(0, length);
                }
            }
            return string.Empty;
        }
        #endregion utils
        #region security check
        /// <summary>
        /// Whether the url is a trusted url for the script trustedURLs, 
        /// return true: trusted url. false: un-trusted url. 
        /// NOTES: here just check the url's domain name is valid, it will be valid. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="trustedURLs"></param>
        /// <returns></returns>
        public static bool isTrustedURL(string url, List<string> trustedURLs) {
            if (url == Constants.URL_BLANK) {
                return true;
            }
            if (url != null && trustedURLs != null) {
                string u1 = getDomainName(url);
                string[] targetFields = u1.Split('.');
                string domain = "."+targetFields[targetFields.Length - 1];
                // check top domain                     
                bool istopDomain = isInDomain(domain, topDomain);
                if (istopDomain && targetFields.Length >= 2) {
                    string testStr = targetFields[targetFields.Length - 2] + domain;
                    foreach (string turl in trustedURLs) {
                        string ut = getDomainName(turl);
                        if (ut == testStr || ut.EndsWith("."+testStr)) {
                            return true;
                        }
                    }
                }
                // check countryDomain 
                bool iscnyDomain = isInDomain(domain, countryDomain);
                if (iscnyDomain) {
                    string testStr = targetFields[targetFields.Length - 2] + domain;
                    if (targetFields.Length > 2) {
                        // check whether it is end with countryDomain2nd surfix 
                        string d2nd = "." + testStr;
                        bool iscnyDomain2nd = isInDomain(d2nd, countryDomain2nd);
                        // check the scenario like abc.org.cn
                        if (iscnyDomain2nd) {
                            // find the domain name 
                            string dname = targetFields[targetFields.Length - 3] + d2nd;
                            foreach (string turl in trustedURLs) {
                                string ut = getDomainName(turl);
                                if (ut == dname || ut.EndsWith("."+dname)) {
                                    return true;
                                }
                            }
                        }
                        // check the scenario like abc.td.cn
                        foreach (string turl in trustedURLs) {
                            string ut = getDomainName(turl);
                            if (ut == testStr || ut.EndsWith(d2nd)) {
                                return true;
                            }
                        }
                    } else if (targetFields.Length == 2) {
                        // check case e.g target=net.cn trustedURL include net.cn domain .
                        foreach (string turl in trustedURLs) {
                            string ut = getDomainName(turl);
                            if (ut == testStr) {
                                return true;
                            }
                        }
                    }
                }                
            }

            return false;
        }

        /// <summary>
        /// whether the domain name is in the trustedDomains 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="trustedDomains"></param>
        /// <returns></returns>
        private static bool isInDomain(string domainName, string[] trustedDomains) {
            if (domainName != null && trustedDomains!=null) {
                foreach (string d in trustedDomains) {
                    if (domainName == d) {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Check whether the elem is a security url violation. If passed, return string.Empty, 
        /// if it is a url request violation, return the violation url address. 
        /// </summary>
        /// <param name="sroot"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        internal static string doPreURLSecurityCheck(ScriptRoot sroot, IHTMLElement elem) {
            if (sroot != null && elem != null && sroot.TrustedUrls != null && sroot.UrlsLocked == true) {
                // check a tag 
                string tag = elem.tagName.ToLower();
                if (tag.Equals("a") || tag.Equals("area")) {
                    string href = elem.getAttribute("href");
                    if (href.StartsWith("#")) { // filter anchor
                        return string.Empty;
                    } else if (!(href.StartsWith("http://") || href.StartsWith("https://"))) { // this is relative path
                        return string.Empty;
                    }
                    bool isTrusted = isTrustedURL(href, sroot.TrustedUrls);
                    if (!isTrusted) {
                        return href;
                    }
                }

            }

            return string.Empty;
        }
        #endregion security check         
        #region login handling 
        /*I prefer this part is a temp way to handling the login in the first version         */
        /// <summary>
        /// Check whether the user has loginned in the browser. return user or null if not logined.
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        public static UserProfile getUserInfo(ui.browser.WebBrowserEx browser) {
            UserProfile user = null;
            if (browser != null && browser.Document != null) {
                HtmlElement elemName = browser.Document.GetElementById("ls_username");
                if (elemName == null) {
                    HtmlElementCollection ec = browser.Document.GetElementsByTagName("a");
                    foreach (HtmlElement elemA in ec) {
                        string outer = elemA.OuterHtml;
                        
                        if (outer.Contains("title=访问我的空间") && outer.Contains("href=\"home.php?mod=space&amp;uid=")) {
                            string name = elemA.InnerText;
                            user = new UserProfile();
                            user.Name = name;
                            user.Response = RESPONSE.LOGIN_SUCCESS;

                            break;
                        }
                    }
                }
            }
            return user;
        }
        
        #endregion login handling

    }
    #region util classes
    /// <summary>
    /// This class is mainly added the enumerator method, GetEnumerator() is easy 
    /// for check all items. 
    /// </summary>
    public class HashtableEx {        
        public static readonly HashtableEx EMPTY = new HashtableEx();

        ArrayList keys = new ArrayList();
        ArrayList values = new ArrayList();
        /// <summary>
        /// remove all elements in the hashtable 
        /// </summary>
        public void clear() {
            keys.Clear();
            values.Clear();
        }
        /// <summary>
        /// add key value object, if key,values existed, the new value will
        /// replace the existed one 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(object key, object value) {
            if (key == null) {
                return;
            }
            if (keys.Contains(key)) {
                int ik = keys.IndexOf(key);
                // existed key/value
                if (ik >= 0 && ik < keys.Count) {
                    values[ik] = value;
                }
            } else { // new key/value
                keys.Add(key);
                values.Add(value);
            }
        }
        /// <summary>
        /// remove the key/value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(object key) {
            if (key == null) {
                return;
            }
            if (keys.Contains(key)) {
                int ik = keys.IndexOf(key);
                if (ik >= 0 && ik < keys.Count) {
                    keys.RemoveAt(ik);
                    values.RemoveAt(ik);
                }
            }
        }
        /// <summary>
        /// get object of the key or null if not find 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(object key) {
            if (key == null) {
                return null;
            }
            if (keys.Contains(key)) {
                int ik = keys.IndexOf(key);
                if (ik >= 0 && ik < keys.Count) {
                    return values[ik];
                }
            }

            return null;
        }
        /// <summary>
        /// get keys enumerator, then you should call 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() {
            return keys.GetEnumerator();
        }

        public int Count {
            get {return keys.Count; }
        }
    }
    #endregion util classes

}
