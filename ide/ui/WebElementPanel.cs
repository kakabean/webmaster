using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using mshtml;
using WebMaster.lib;
using WebMaster.lib.ui.browser;
using System.Threading;
using WebMaster.lib.hooks;
using WebMaster.com;
using WebMaster.com.script;

namespace WebMaster.ide.ui
{
    public partial class WebElementPanel : UserControl
    {
        public static readonly string CMD_MIN = "min";
        public static readonly string CMD_RESET = "reset";
        /// <summary>
        /// it used define whether the updated WebElement is a newly added one, 
        /// and it will be added into scriptroot's rawdata folder
        /// this is the default behaviour
        /// </summary>
        public static readonly string WE_NEW_ADD = "new added";
        /// <summary>
        /// it is used define that the updated WebElement is an existed one, and 
        /// just update it value will be ok. 
        /// </summary>
        public static readonly string WE_UPDATE = "update existed"; 
        #region variables 
        /// <summary>
        /// scripte root for current application 
        /// </summary>
        private ScriptRoot _sroot = null;
        /// <summary>
        /// WebBrowser for the attribute area 
        /// </summary>
        private WebBrowserEx webBrowserEx1;
        /// <summary>
        /// WebElement properties view manager 
        /// </summary>
        private PropViewManager _wePVManager;
        /// <summary>
        /// current candidate IHTMLElement to be shown, the valid value is [0,list size)
        /// if the value is -1, it means that there is no candidate elements should 
        /// be shown
        /// </summary>
        private int _candidateIHTMLElemIndex = -1;
        /// <summary>
        /// candidate IHTMLElement list to be shown for navigate
        /// </summary>
        private List<IHTMLElementWrap> _candidateIHTMLElemList = null;
        /// <summary>
        /// it will be used to record the candidate HTMLElement's offset, if it 
        /// is in a iframe. 
        /// </summary>
        private Point _candidateIFRMHEOffset = Point.Empty;
        /// <summary>
        /// the cover box will be set when check the candidate HtmlElement in browser.
        /// it will be set null before check and update to proper document value if find. 
        /// it will be used to find proper cover box. 
        /// </summary>
        private IHTMLDocument2 _cbxdoc2 = null;
        /// <summary>
        /// type of the WebElement 
        /// </summary>
        private WEType _weType = WEType.ATTRIBUTE;
        ///// <summary>
        ///// currently captured WebElement 
        ///// </summary>
        //public WebElement CapturedWE {
        //    get { return this._wePVManager.ActiveView.getWebElement(); }
        //}
        /// <summary>
        /// whether to record the element 
        /// </summary>
        private bool _isRecording = false;
        /// <summary>
        /// this is a div area used to display the selection bounds，it is the current active cover box.  
        /// </summary>
        private CoverBox currentBox = null;

        private MouseHook mhook = null;
        /// <summary>
        /// how about the updated WebElement, it is newly added or an existed one. 
        /// </summary>
        private string _updateWEType = WE_NEW_ADD ;
        /// <summary>
        /// captured element structure. 
        /// </summary>
        private CapturedElement capturedE = null;
        /// <summary>
        /// How many times the autoParent will check. default only check 3 times
        /// </summary>
        private int autoParentLevel = 0;
        /// <summary>
        /// this is used to record the current IHTMLElement for the autoParent. 
        /// </summary>
        private IHTMLElement autoParentElem = null;
        private WebElement autoParentWE = null;
        #endregion variables 
        public WebElementPanel() {
            InitializeComponent();
            initData();
        }
        #region common function
        private void initData() {
            this.tsmi_keystring.Tag = WEType.CODE;
            this.tsmi_attribute.Tag = WEType.ATTRIBUTE;
            this.tsmi_location.Tag = WEType.LOCATION;
            this.tsmi_color.Tag = WEType.COLOR;
            this.tsmi_image.Tag = WEType.IMAGE;
        }
        public void setWebBrowser(WebBrowserEx webBrowser) {
            this.webBrowserEx1 = webBrowser;
            this.webBrowserEx1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowserEx1_DocumentCompleted);
        }
        void webBrowserEx1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            if (!this.webBrowserEx1.IsBusy && this.webBrowserEx1.ReadyState == WebBrowserReadyState.Complete) {
                if (this._sroot != null) {
                    this.enableWEPropbar();
                }
            }
        }

        public void setScriptRoot(ScriptRoot sroot) {
            this._sroot = sroot;
            _wePVManager.setScriptRoot(sroot);
            // initialize WebElement properties area 
            this.initialPanel();
        }
        /// <summary>
        /// used when first initial the WebElementPanel. 
        /// </summary>
        public void initWEPropertiesView() {
            if (this._wePVManager == null) {
                this._wePVManager = new PropViewManager(this.panel1);
                this._wePVManager.CheckWebElementEvt += new EventHandler<CommonEventArgs>(_wePVManager_CheckWebElementEvt);
            }
            this.disableWEPropbar();
            this._wePVManager.initViews();
            this._wePVManager.ActiveView.disableView();
        }

        void _wePVManager_CheckWebElementEvt(object sender, CommonEventArgs e) {
            if (this.tsb_PropCheck.Enabled == true) {
                this.tsb_Click_checkWE(sender, e);
            }
        }
        /// <summary>
        /// enable properties bar area, and enable the current active WebElement 
        /// properties area. 
        /// </summary>
        public void initialPanel() {
            this.enableWEPropbar();
            this._wePVManager.ActiveView.enableView();
            
            if (this._updateWEType == WE_UPDATE) {
                this.tsb_msg.ForeColor = Color.Green;
                this.tsb_msg.Text = UILangUtil.getMsg("view.weprop.update.text");
            } else {
                tsb_msg.Text = "";
            }
        }
        /// <summary>
        /// clean active properties view UI info, and binding data info. 
        /// </summary>
        public void cleanActiveView() {
            this._wePVManager.cleanActivePropView();
        }
        /// <summary>
        /// reset properties panel, reset ui and data info
        /// </summary>
        public void resetPropPanel() { 
            // clean panel info
            _candidateIHTMLElemIndex = -1;
            _candidateIHTMLElemList = null;
            _candidateIFRMHEOffset = Point.Empty;
            _cbxdoc2 = null;            
            currentBox = null;
            capturedE = null;
            if (this._isRecording) {
                if (mhook.IsInstalled) {
                    mhook.Uninstall();
                }
                this._isRecording = false;                
            }
            updatePropertyBar();
            // clean properties view info
            this.cleanActiveView();
        }
        /// <summary>
        /// update the current active WebElement Properties view, if the current type is not WE_UPDATE, set up it into 
        /// capture status, else it will just show the properties view by the WebElement
        /// </summary>
        /// <param name="we"></param>
        internal void updatePropertiesView(WebElement we) {
            this._isRecording = true;
            this._updateWEType = WE_UPDATE;
            // set it in a capture status, Notes that for the WebElement update, user must capture a new 
            // WebElement to replace the existed one, or do nothing for the WebElement properties, just the 
            // name and description can be updated. 
            if (mhook == null)
            {
                mhook = new MouseHook();
                mhook.MouseMove += new MouseHook.MouseHookEventHandler(mhook_MouseMove);
                mhook.MouseDown += new MouseHook.MouseHookEventHandler(mhook_MouseDown);
            }
            if (!mhook.IsInstalled)
            {
                mhook.Install();
            }

            this.updatePropertyBar();
            
            this._wePVManager.ActiveView.updateView(we, false);
        }
        #endregion common function
        #region hook handlers
        void mhook_MouseDown(object sender, MouseHookEventArgs e) {
            if (this._weType == WEType.CODE || this._weType == WEType.ATTRIBUTE) {
                doMouseDownAtt(e);
            }    
        }
        /// <summary>
        /// this is used to handle the mouse down with HtmlElement attribute 
        /// </summary>
        /// <param name="e"></param>
        private void doMouseDownAtt(MouseHookEventArgs e) {
            Log.println_hook(" mhook mouse down att .....");
            if (mhook.isHoldingMouseDownMsg) {
                mhook.isHoldingMouseDownMsg = false;
            }
            if (this._isRecording) {
                if (mhook.IsInstalled) {
                    mhook.Uninstall();
                }
                this._isRecording = false;
                this.autoParentLevel = 0;
                this.autoParentElem = null;
                this.autoParentWE = null;
                // just the left button is valid 
                if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                    if (isValidCaptureMove(e.X, e.Y) && isValidCaptureElement(this.capturedE.Elem2 as IHTMLElement)) {
                        updateWEPVbyElem2(this.capturedE.Elem2);
                        updatePropertyBar();
                        this.tsb_PropCheck.Enabled = true;
                        //this.tsb_PropUpdate.Enabled = true;
                    }
                } else {
                    updatePropertyBar();
                    if (this._updateWEType == WE_UPDATE) {
                        this.tsb_PropCheck.Enabled = true;                        
                    }
                }
            }
        }
        /// <summary>
        /// update the properties view by IHTMLElement2, and update some control variables 
        /// <param name="elem2"></param>
        /// </summary>
        private void updateWEPVbyElem2(IHTMLElement2 elem2) {
            if (elem2 == null) {
                return;
            }

            if (_weType == WEType.CODE || _weType == WEType.ATTRIBUTE) {
                if (this._wePVManager.ActiveView != null) {
                    bool isNew = true;
                    if (_updateWEType == WE_UPDATE) {
                        isNew = false;
                    }
                    this._wePVManager.ActiveView.updateView(elem2, isNew);
                }
            }           
        }
        void mhook_MouseMove(object sender, MouseHookEventArgs e) {
            if (this._weType == WEType.CODE || this._weType == WEType.ATTRIBUTE) {
                doMouseMoveAtt(e);
            }
        }
        /// <summary>
        /// This is used to handle Capture by HtmlElement attribute
        /// </summary>
        /// <param name="e"></param>
        private void doMouseMoveAtt(MouseHookEventArgs e) {
            // if it is not in recording state, just ignore
            if (!_isRecording || !isValidCaptureMove(e.X, e.Y)) {
                return;
            }
            // this is used to prevent the browser html element response the mouse move event, 
            // so that to ignore some scripts will update the element attribute when mouse move. 
            if (!mhook.isHoldingMouseMoveMsg) {
                mhook.isHoldingMouseMoveMsg = true;
            }
            // update Holding mouse down flag of hooker, ignore a mouse down once
            if (!mhook.isHoldingMouseDownMsg) {
                mhook.isHoldingMouseDownMsg = true;
            }

            // check and show cover box of the WebElement 
            Point cp = this.webBrowserEx1.PointToClient(new Point(e.X, e.Y));
            // make sure the cursor is in the webbrowser area
            if (cp.X < 0 || cp.Y < 0 || cp.X > this.webBrowserEx1.ClientRectangle.Right || cp.Y > this.webBrowserEx1.ClientRectangle.Bottom) {
                return;
            }
            Log.println_hook(" hook mouse move att ................................................................ start");
            if (this.webBrowserEx1.Document != null) {
                IHTMLDocument2 topDoc2 = this.webBrowserEx1.Document.DomDocument as IHTMLDocument2;
                capturedE = CaptureUtil.getIHTMLElement2ByPos(cp, topDoc2);
                //Here iframe/frame element will be ignored 
                if (isValidCaptureElement(this.capturedE.Elem2 as IHTMLElement)) {
                    CaptureUtil.showCoverBox(this.currentBox, this.capturedE, CoverBox.COLOR_CAPTURE, false);
                }
            }
            Log.println_hook(" hook mouse move att ................................................................ end ");
        }
        /// <summary>
        /// filter the frame, iframe and html element if selected 
        /// </summary>
        /// <returns></returns>
        private bool isValidCaptureElement(IHTMLElement elem) {
            if (elem == null) {
                return false;
            }
            string tag = elem.tagName.ToLower();
            if (tag.Equals("iframe") || tag.Equals("frame") || tag.Equals("html")) {
                return false;
            }
            return true;
        }
        /// <summary>
        /// whether the cursor is moved on the browser area 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        private bool isValidCaptureMove(int X, int Y) {
            Rectangle rect = this.webBrowserEx1.ClientRectangle;
            Point p = this.webBrowserEx1.PointToClient(new Point(X,Y));
            //Log.println_wme("browser rect = "+rect+", point = "+X+", "+Y + ", np = "+p);
            // note that: here doesn't consider the scroll bar. 
            return rect.Contains(p);
        }
        /// <summary>
        /// this method is used to disable the recording process
        /// </summary>
        internal void cancelRecording() {
            if (this._isRecording) {
                if (mhook.IsInstalled) {
                    mhook.Uninstall();
                }
                this._isRecording = false;
                updatePropertyBar();
            }
        }
        #endregion 
        #region event area
        /// <summary>
        /// the sender is the WebElementPanel, the data is the newly added 
        /// WebElement
        /// </summary>
        public event EventHandler<CommonEventArgs> NewWebElementAddedEvt;
        protected virtual void OnNewWebElementAddedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> newWebElementAddedEvt = NewWebElementAddedEvt;
            if (newWebElementAddedEvt != null) {
                newWebElementAddedEvt(this, e);
            }
        }
        public void raiseNewWebElementAddedEvt(Object sender, WebElement we) {
            if (we != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, we);
                OnNewWebElementAddedEvt(evt);
            }
        }
        /// <summary>
        /// update the existed WebElement, the data is to be updated WebElement 
        /// </summary>
        public event EventHandler<CommonEventArgs> UpdateExistedWebElementEvt;
        protected virtual void OnUpdateExistedWebElementEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> updateExistedWebElementEvt = UpdateExistedWebElementEvt;
            if (updateExistedWebElementEvt != null) {
                updateExistedWebElementEvt(this, e);
            }
        }
        public void raiseUpdateExistedWebElementEvt(Object sender, WebElement we) {
            if (we != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, we);
                OnUpdateExistedWebElementEvt(evt);
            }
        }
        /// <summary>
        /// the sender is the WebElementPanel, the data is a interger object 
        /// if data == CMD_MIN , it means minimize the properties view 
        /// if data == CMD_RESET , it means restore the default size 
        /// </summary>
        public event EventHandler<CommonEventArgs> UpdateWEPropViewEvt;
        protected virtual void OnUpdateWEPropViewEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> updateWEPropViewEvt = UpdateWEPropViewEvt;
            if (updateWEPropViewEvt != null) {
                updateWEPropViewEvt(this, e);
            }
        }
        /// <summary>
        /// the sender is the WebElementPanel, the data is a interger object 
        /// if data == CMD_MIN , it means minimize the properties view 
        /// if data == CMD_RESET , it means restore the default size 
        /// <param name="sender"></param>
        /// <param name="command"></param>
        /// </summary>
        public void raiseUpdateWEPropViewEvt(Object sender, string command) {
            if (command == CMD_MIN || command == CMD_RESET) {                
                CommonEventArgs evt = new CommonEventArgs(sender, command);
                OnUpdateWEPropViewEvt(evt);
            }
        }
        #endregion event area 
        #region tsb properties bar functions 
        private void tsddb_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            this.ts_ddb.ToolTipText = e.ClickedItem.ToolTipText;
            this.ts_ddb.Image = e.ClickedItem.Image;
            this._weType = (WEType)e.ClickedItem.Tag;

            this._wePVManager.activeView(this._weType);
        }
        private void tsb_capture_Click(object sender, EventArgs e) {
            if(tsb_capture.CheckState == CheckState.Checked){
                return ;
            }
            this._isRecording = true;
            this._updateWEType = WE_NEW_ADD ;
            if (mhook == null) {
                mhook = new MouseHook();
                mhook.MouseMove += new MouseHook.MouseHookEventHandler(mhook_MouseMove);
                mhook.MouseDown += new MouseHook.MouseHookEventHandler(mhook_MouseDown);
            }
            if (!mhook.IsInstalled) {
                mhook.Install();
            }

            this._wePVManager.ActiveView.resetView();
            
            this.updatePropertyBar();            
        }
        /// <summary>
        /// check WebElement button, update how many candidate IHTMLElement can be found
        /// update the candidate element list for navigation button to use if need 
        /// check whether the element is unique in current web page, and update 
        /// messages to developer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Click_checkWE(object sender, EventArgs e) {           
            IWEPropView av = this._wePVManager.ActiveView;
            WebElement we = av.getWebElement();
            Log.println_cap("Get WebElement from prop view\n"+we);
            if (!this._wePVManager.ActiveView.isValid()) {
                this.tsb_msg.ForeColor = Color.Red;
                this.tsb_msg.Text = this._wePVManager.ActiveView.getInvalidMsg();
                return;
            }
            // When type is CODE or ATTRIBUTE, 
            // check all candicates WebElements 
            if (we.TYPE == WEType.CODE || we.TYPE == WEType.ATTRIBUTE) {
                checkIHTMLElement(we);                
            }
        }
        /// <summary>
        /// 1. check and show the the first find element with coverbox or update the error msg if need
        /// 2. update the toolbar status.
        /// </summary>
        /// <param name="we"></param>
        private void checkIHTMLElement(WebElement we) {
            // update toolbar
            this.updatePropertyBar();
            this.tsb_PropCheck.Enabled = true;

            // clean previous action for candidate element 
            if (this._candidateIHTMLElemList != null) {
                this._candidateIHTMLElemList.Clear();
                this._candidateIHTMLElemIndex = -1;
            }
            // update refWebElement if there are frames 
            WebUtil.updateRefFrames(capturedE, we, this._sroot);
            // update candidate Html elements, all these html element are in a same iframe level 
            this._candidateIFRMHEOffset = new Point(0, 0);
            this._cbxdoc2 = null;
            this._candidateIHTMLElemList = WebUtil.getIHTMLElements(we, this.webBrowserEx1, false, ref this._candidateIFRMHEOffset, ref _cbxdoc2);
            //this._candidateIHTMLElemList = obj as List<IHTMLElementWrap>;
            if (this._cbxdoc2 == null && this.webBrowserEx1 != null && webBrowserEx1.Document != null) {
                this._cbxdoc2 = this.webBrowserEx1.Document.DomDocument as IHTMLDocument2;
            }
            // default check the first one 
            this._candidateIHTMLElemIndex = 0;

            // show checked msg 
            showCheckedMsg();
            // show cover box 
            if (this._candidateIHTMLElemList.Count > 0) {
                // highlight the first element found. 
                this.showCoverBox(true);
            }
            this.tsb_PropCheck.Enabled = true;
            // update toolbar status 
            if (this._candidateIHTMLElemList.Count > 1) {
                this.tsb_PropNav.Visible = true;
                this.tsb_PropNav.Enabled = true;
                this.tsb_weUpdate.Enabled = true;
                this.tsddb_AdCap.Enabled = true;
                this.tsddb_refineWE.Enabled = true;
            } else if (this._candidateIHTMLElemList.Count == 1) {
                if (this.tsb_PropNav.Visible) {
                    this.tsb_PropNav.Visible = false;
                }
                this.tsb_weUpdate.Enabled = true;
                this.tsddb_AdCap.Enabled = true;
            } else {
                if (this.tsb_PropNav.Visible) {
                    this.tsb_PropNav.Visible = false;
                }
            }           
        }
        /// <summary>
        /// update the tsb_msg label when checked button or navigation button clicked 
        /// </summary>
        private void showCheckedMsg() {
            string msg = UILangUtil.getMsg("view.weprop.capmsg.text1") + (this._candidateIHTMLElemIndex + 1) + UILangUtil.getMsg("view.weprop.capmsg.text2");
            if (this._candidateIHTMLElemIndex == 0) {
                msg = UILangUtil.getMsg("view.weprop.capmsg.text3");
            }
            if (this._candidateIHTMLElemIndex == 1) {
                msg = UILangUtil.getMsg("view.weprop.capmsg.text4");
            }
            if (this._candidateIHTMLElemIndex == 2) {
                msg = UILangUtil.getMsg("view.weprop.capmsg.text5");
            }
            this.tsb_msg.Text = msg;
            StringBuilder sb = new StringBuilder();
            if (this._candidateIHTMLElemList.Count > 1) {
                this.tsb_msg.ForeColor = Color.OrangeRed;
                string text6 = UILangUtil.getMsg("view.weprop.capmsg.text6");
                string text7 = UILangUtil.getMsg("view.weprop.capmsg.text7");
                sb.Append(" ").Append(this._candidateIHTMLElemList.Count).Append(text6).Append(msg).Append(text7);
            } else if (this._candidateIHTMLElemList.Count == 1) {
                this.tsb_msg.ForeColor = Color.Green;
                sb.Append(UILangUtil.getMsg("view.weprop.capmsg.text8"));
            } else {
                this.tsb_msg.ForeColor = Color.Red;
                sb.Append(UILangUtil.getMsg("view.weprop.capmsg.text9"));
            }
            
            this.tsb_msg.Text = sb.ToString();
        }
        /// <summary>
        /// click to show the candidate checked WebElement on the current page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnWeNav_Click(object sender, EventArgs e) {
            if (this._candidateIHTMLElemIndex < this._candidateIHTMLElemList.Count-1) {                
                this._candidateIHTMLElemIndex++;
            }
                      
            showCoverBox(true);

            // show text 
            this.showCheckedMsg();
            if (this._candidateIHTMLElemIndex >= this._candidateIHTMLElemList.Count - 1) {
                this._candidateIHTMLElemIndex = -1;
            }
        }        
        /// <summary>
        /// show the cover box 
        /// </summary>
        /// <param name="isScrollTo">whether need to scroll the cover box into client area</param>
        private void showCoverBox(bool isScrollTo) {
            // highlight the first element found. 
            this.capturedE.Left = this._candidateIFRMHEOffset.X;
            this.capturedE.Top = this._candidateIFRMHEOffset.Y;
            IHTMLElementWrap wrap = this._candidateIHTMLElemList[this._candidateIHTMLElemIndex];
            this.capturedE.Elem2 =  wrap.IHTMLElem as IHTMLElement2;
            bool scroll = needScrollTo(capturedE.Elem2);
            CaptureUtil.updateCapturedElem(capturedE, capturedE.Elem2, this._cbxdoc2);
            CaptureUtil.showCoverBox(this.currentBox, this.capturedE, CoverBox.COLOR_CHECK, scroll);            
        }
        /// <summary>
        /// whether the elemement need to scroll into the client area 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private bool needScrollTo(IHTMLElement2 elem) {
            if (elem != null) {
                IHTMLRect rect = elem.getBoundingClientRect();
                IHTMLDocument3 doc3 = this.webBrowserEx1.Document.DomDocument as IHTMLDocument3;
                IHTMLElement2 html = doc3.getElementsByTagName("HTML").item(0) as IHTMLElement2;
                int bx1 = html.scrollLeft ;
                int bx2 = html.scrollLeft+html.clientWidth ;
                int by1 = html.scrollTop ;
                int by2 = html.scrollTop+html.clientHeight ;

                if (bx1 > rect.left || bx2 < rect.left || by1 > rect.top || by2 < rect.top) {
                    return true;
                }

                Log.println_cap("scrollTop = " + html.scrollTop + ", client width = " + html.clientWidth +  ", client height = " + html.clientHeight +", scroll width = " + html.scrollWidth + ", scroll height = " + html.scrollHeight);

            }
            return false;
        }
        /// <summary>
        /// add the current captured WebElement into script root
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_weUpdate_Click(object sender, EventArgs e) {
            WebElement we = this._wePVManager.ActiveView.getWebElement();
            if (_sroot == null) {
                tsb_msg.Text = UIConstants.WE_PANEL_SCRIPT_ROOT_IS_NULL;
            } else {
                string msg = getValidationMsg(we);
                // handle if there is a container WE
                if (this.autoParentWE!=null) {
                    we.refWebElement = this.autoParentWE;                    
                }
                if (msg==string.Empty) {
                    //this.updatePropertyBar();
                    //this.tsb_msg.Text = "";
                    // clean property view 
                    this._wePVManager.ActiveView.resetView();
                    if (_updateWEType == WE_NEW_ADD) {
                        // update editor WebElement tree
                        this.raiseNewWebElementAddedEvt(this, we);
                    } else if (_updateWEType == WE_UPDATE) {
                        this.raiseUpdateExistedWebElementEvt(this, we);
                    }
                } else {
                    this.tsb_msg.ForeColor = Color.Red;
                    this.tsb_msg.Text = msg;
                }
                if (we.refWebElement != null && isAutoRef()) {
                    ModelManager.Instance.updateSRootInternalWEs(we, this._sroot);
                }
            }

            this.updatePropertyBar();
        }
        /// <summary>
        /// If the WE.refWebElement is added auto by app, it will return true, or if the refWE is added
        /// by script designer, it will return false
        /// </summary>
        /// <returns></returns>
        private bool isAutoRef() {
            //TODO 
            return true;
        }
        /// <summary>
        /// get the WebElement validation message or Empty string if passed. 
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        private string getValidationMsg(WebElement we) {
            if (we == null) {
                return "Invalid element, Element can not be null. ";
            }
            if (we.Name == null || we.Name.Length < 1) {
                return "Element name can not be empty. ";
            }
            if ((we.TYPE == WEType.ATTRIBUTE || we.TYPE == WEType.CODE) && we.Attributes.Count < 1) {
                return "Error, No Attributes specified. "; 
            }
            if (!ModelManager.Instance.isUniqueToBeElement(we,this._sroot.RawElemsGrp.Elems)) {                
                if (_updateWEType == WE_NEW_ADD) {
                    this.tsb_msg.Text = UILangUtil.getMsg("view.weprop.submit.text");
                } else if (_updateWEType == WE_UPDATE) {
                    this.tsb_msg.Text = UILangUtil.getMsg("view.weprop.submit.text1");
                }
            }

            return string.Empty;
        }
        /// <summary>
        /// restore the property view size as default, the editor area spliter will be updated 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_resetPVSize_Click(object sender, EventArgs e) {
            this.raiseUpdateWEPropViewEvt(this,CMD_RESET);
        }
        private void tsb_PropMin_Click(object sender, EventArgs e) {
            this.raiseUpdateWEPropViewEvt(this, CMD_MIN);
        }
        /// <summary>
        /// update the property bar's status. e.g the buttons state
        /// </summary>
        private void updatePropertyBar() {
            if (this._sroot == null) {
                return;
            }
            this.tsb_PropCheck.Enabled = false;
            this.tsb_PropNav.Enabled = false;
            this.tsb_weUpdate.Enabled = false;
            this.tsddb_AdCap.Enabled = false;
            this.tsddb_refineWE.Enabled = false;
            this.tsb_msg.Text = string.Empty;

            if (this._isRecording) {                
                this.ts_ddb.Enabled = false;                                     
                this.tsb_capture.CheckState = CheckState.Checked;
                this.tsb_PropNav.Visible = false;
            } else {                
                this.ts_ddb.Enabled = true;
                this.tsb_capture.CheckState = CheckState.Unchecked;
                if (this._updateWEType == WE_UPDATE) {
                    this.tsb_msg.Text = "";
                }
            }            
        }
        /// <summary>
        /// disable all the buttons on the WebElement Properties bar
        /// </summary>
        private void disableWEPropbar() {
            ts_ddb.Enabled = false;
            tsb_capture.Enabled = false;
            tsb_PropCheck.Enabled = false;
            tsb_PropNav.Enabled = false;
            tsb_weUpdate.Enabled = false;
            this.tsddb_AdCap.Enabled = false;
            this.tsddb_refineWE.Enabled = false;
        }
        /// <summary>
        /// enable all the buttons on the WebElement Properties bar
        /// </summary>
        private void enableWEPropbar() {
            if (this.webBrowserEx1 != null && this.webBrowserEx1.Document != null) {
                ts_ddb.Enabled = true;
                tsb_capture.Enabled = true;
            }
        }
        
        #endregion tsb properties bar functions 
        #region browser functions               
        /// <summary>
        /// disable WebElement capture panel area when navigate to a new page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void handleNavigating(object sender, WebBrowserNavigatingEventArgs e) {
            this.disableWEPropbar();
        }        
        #endregion browser functions

        private void tsmi_autoRefWE_Click(object sender, EventArgs e) {
            //Log.println("parent click");
            if (this.capturedE == null || this.capturedE.Elem2 == null) {
                return;
            }
            IHTMLElement elem = this.capturedE.Elem2 as IHTMLElement;
            IHTMLElement2 pElem2 = elem.parentElement as IHTMLElement2;
            //bool isNew = true;
            //if (_updateWEType == WE_UPDATE) {
            //    isNew = false;
            //}
            if (this.autoParentLevel < 5) {
                this.autoParentWE = getAutoParent(elem);
                if (this.autoParentWE != null) {
                    WebElement we = this._wePVManager.ActiveView.getWebElement();
                    if (we != null) {
                        WebElement twe = we.Clone();
                        if (twe.refWebElement!=null && (twe.refWebElement.Tag == "iframe" || twe.refWebElement.Tag == "frame")) {
                            WebElement frameWE = twe.refWebElement;                            
                            this.autoParentWE.refWebElement = frameWE;
                            twe.refWebElement = this.autoParentWE;
                        } else {
                            WebElement refWE = twe.refWebElement;
                            if (refWE != null) {
                                this.autoParentWE.refWebElement = refWE.refWebElement;
                            }
                            twe.refWebElement = this.autoParentWE;
                        }
                        checkIHTMLElement(twe);                       
                    }
                }
            } else {
                this.tsb_msg.ForeColor = Color.Red;
                string msg = UILangUtil.getMsg("view.weprop.autoref.text1");
                this.tsb_msg.Text = msg;
                this.tsb_msg.ForeColor = Color.Black;
                                
                this.tsddb_refineWE.Enabled = false;
            }
            //this._wePVManager.ActiveView.updateView(this.capturedE.Elem2, isNew);
            // update toolbar 
            this.updatePropertyBar();
            this.tsb_PropCheck.Enabled = true;
        }
        /// <summary>
        /// Get proper container WebElement to make sure the target element is unique, return null if errors.
        /// The container element can not exceed the frame/iframe if have.
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private WebElement getAutoParent(IHTMLElement elem) {
            if (elem == null || elem.parentElement == null) {
                return null;
            }
            WebElement pWE = null;
            List<string> tags = new List<string>() { "div", "form", "frame", "iframe", "img", "input", "map", "object", "span", "table", "textarea" };
            
            IHTMLElement pElem = elem.parentElement;            
            string tag = pElem.tagName.ToLower();
            // Here just check the parent elements under a frame/iframe level. 
            while (pElem!=null && tag!="body") {
                if (tags.Contains(tag)) {
                    pWE = createWebElement(pElem);
                    break;
                } else {
                    pElem = pElem.parentElement;
                    tag = pElem.tagName.ToLower();
                }
            }
            if (pWE != null) {
                this.autoParentElem = pElem;
                this.autoParentLevel++;
            }

            return pWE;
        }
        /// <summary>
        /// create a WebElement based on IHTMLElement, or return null if errors. 
        /// </summary>
        /// <param name="ihtmlElem"></param>
        /// <returns></returns>
        private static WebElement createWebElement(IHTMLElement ihtmlElem) {
            if (ihtmlElem == null) {
                return null;
            }
            WebElement we = null;
            we = ModelFactory.createWebElement();
            //if (ihtmlElem.id != null && ihtmlElem.id.Trim().Length > 0) {
            //    we.ID = ihtmlElem.id.Trim();
            //}
            //we.Tag = ihtmlElem.tagName.ToLower();
            we.TYPE = WEType.ATTRIBUTE;
            we.Name = "WE" + we.GetHashCode();
            HashtableEx attributeTable = WebUtil.getValuedAttriubtes(ihtmlElem);
            foreach (object o in attributeTable) {
                WebElementAttribute wea = ModelFactory.createWebElementAttribute();
                wea.Key = Convert.ToString(o);
                string value = Convert.ToString(attributeTable.Get(o));
                wea.PValues.Add(value);
                wea.PATTERN = CONDITION.STR_FULLMATCH;
                we.Attributes.AddUnique(wea);
            }
            return we;
        }

        private void tsmi_customizeRefWE_Click(object sender, EventArgs e) {
            //用户自定义的refWE将完全替换WE现在已有的refWE结构
            Log.println("next sibling click");
        }

        private void tsmi_capParent_Click(object sender, EventArgs e) {
            this.tsb_msg.Text = string.Empty;

            if (this.capturedE == null || this.capturedE.Elem2 == null) {
                string text = UILangUtil.getMsg("view.we.parent.errdlg.text");
                string title = UILangUtil.getMsg("view.we.parent.errdlg.title");
                MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.OK, MessageBoxIcon.Error, FormStartPosition.CenterParent);
                return;
            }
            IHTMLElement2 currElem = this.capturedE.Elem2;
            IHTMLElement2 parent = getParentIHTMLElement4Capture(currElem);
            if (parent != null) {
                if (isValidCaptureElement(parent as IHTMLElement)) {
                    this.capturedE.Elem2 = parent;
                    updateWEPVbyElem2(this.capturedE.Elem2);
                    this.tsb_PropCheck.Enabled = true;
                } else {
                    string text = UILangUtil.getMsg("view.we.parent.errdlg.text1");
                    string title = UILangUtil.getMsg("view.we.parent.errdlg.title1");
                    MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.OK, MessageBoxIcon.Error, FormStartPosition.CenterParent);
                    return;
                }
            } else {
                tsb_msg.ForeColor = Color.OrangeRed;
                tsb_msg.Text = UILangUtil.getMsg("view.we.parent.text");
            }
            // update toolbar 
            this.updatePropertyBar();
            this.tsb_PropCheck.Enabled = true;
        }

        private void tsmi_capSibling_Click(object sender, EventArgs e) {
            this.tsb_msg.Text = string.Empty;
            if (this.capturedE == null || this.capturedE.Elem2 == null) {
                string text = UILangUtil.getMsg("view.we.sibling.errdlg.text");
                string title = UILangUtil.getMsg("view.we.sibling.errdlg.title");
                MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.OK, MessageBoxIcon.Error, FormStartPosition.CenterParent);
                return;
            }
            IHTMLElement2 next = getSiblingIHTMLEElement4Capture(this.capturedE.Elem2);
            if (next != null) {
                if (isValidCaptureElement(next as IHTMLElement)) {
                    this.capturedE.Elem2 = next;
                    updateWEPVbyElem2(this.capturedE.Elem2);
                    this.tsb_PropCheck.Enabled = true;
                } else {
                    string text = UILangUtil.getMsg("view.we.sibling.errdlg.text1");
                    string title = UILangUtil.getMsg("view.we.sibling.errdlg.title1");
                    MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.OK, MessageBoxIcon.Error, FormStartPosition.CenterParent);
                    return;
                }
            } else {
                tsb_msg.ForeColor = Color.OrangeRed;
                tsb_msg.Text = UILangUtil.getMsg("view.we.sibling.text");
            }
            // update toolbar 
            this.updatePropertyBar();
            this.tsb_PropCheck.Enabled = true;
        }
        ///// <summary>
        ///// Get the parent IHTMLElement2 of the currentElement or null if errors 
        ///// </summary>
        ///// <param name="elem2"></param>
        ///// <returns></returns>
        private IHTMLElement2 getParentIHTMLElement4Capture(IHTMLElement2 elem2) {
            if (elem2 != null) {
                IHTMLElement curr = elem2 as IHTMLElement;
                IHTMLElement parent = curr.parentElement;
                while (parent!=null && !CaptureUtil.isProperCapturabledElem(parent)) {
                    parent = parent.parentElement;                    
                }
                
                return parent as IHTMLElement2;
            }
            return null;
        }
        ///// <summary>
        ///// Get next sibling IHTMLElement2 of the currentElement or null if errors 
        ///// </summary>
        ///// <param name="elem2"></param>
        ///// <returns></returns>
        private IHTMLElement2 getSiblingIHTMLEElement4Capture(IHTMLElement2 elem2) {
            if (elem2 == null) {
                return null;
            }
            IHTMLElement elem = elem2 as IHTMLElement;
            IHTMLElement parent = elem.parentElement;
            object obj = parent.children;
            IHTMLElementCollection list = (IHTMLElementCollection)parent.children;
            IHTMLElement result = null;
            if (list.length <= 1) {
                result = null;
            } else {
                bool find = false;               
                foreach (IHTMLElement ielem in list) {
                    if (find) {
                        result = ielem;
                        break;
                    }
                    if (ielem == elem2) {
                        find = true;
                    }
                }

                if (find) {
                    if (result == null) {
                        // just get the first element. 
                        foreach (IHTMLElement r in list) {
                            result = r;
                            break;
                        }
                    }
                }                
            }
            return result as IHTMLElement2;
        }       

    }
}
