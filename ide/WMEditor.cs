using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.editor.model;
using WebMaster.ide.ui;
using mshtml;
using WebMaster.lib;
using System.Threading;
using WebMaster.lib.ui;
using WebMaster.com;
using WebMaster.com.script;
using WebMaster.lib.ui.browser;

namespace WebMaster.ide
{
    public partial class WMEditor : Form
    {
        #region variables        
        /// <summary>
        /// scripte root for current application 
        /// </summary>
        private ScriptRoot _sroot = null;

        public ScriptRoot SRoot {
            get { return _sroot; }
            set { 
                _sroot = value;
                wePanel.setScriptRoot(_sroot);
            }
        }
        /// <summary>
        /// model of script model and view model 
        /// </summary>
        private BigModel bigmodel = null;

        public BigModel Bigmodel {
            get { return bigmodel; }
            set {
                bigmodel = value;
                debugPanel.Bigmodel = bigmodel;
                SRoot = bigmodel != null ? bigmodel.SRoot : null;
                if (this.bigmodel != null) {
                    debugPanel.enableDebug();
                } else {
                    debugPanel.disableDebug();
                }
            }
        }
        /// <summary>
        /// debug panel 
        /// </summary>
        private DebugPanel debugPanel;
        /// <summary>
        /// Engine for debug, one application just has one debug
        /// </summary>
        private WebEngine dbgEngine = null;
        /// <summary>
        /// this is a runtime user info, including the user ticket when login sucessfully. 
        /// </summary>
        private UserProfile user = null;

        private string app_name = "Mayi Browser";
        private string no_login = "Not Login";
        #endregion variables
        public WMEditor() {
            InitializeComponent();
            dbgEngine = new WebEngine(this.backgroundWorker1, this.webBrowserEx1, WebEngine.MODE_DEBUG);
            app_name = UILangUtil.getMsg("app.name.text");
            no_login = UILangUtil.getMsg("app.user.no.login");
            initUI();            
        }
        #region overrides methods 
        protected override void WndProc(ref Message m) {            
            base.WndProc(ref m);
            HotkeyUtil.ProcessHotKey(m);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.F5:
                    ShortCut_F5();
                    break;
                case Keys.Control | Keys.S:
                    ShortCut_CTRL_S();
                    break;
                case Keys.Control | Keys.P:
                    ShortCut_CTRL_P();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion overrides methos 
        #region UI common functions
        /// <summary>
        /// init all views: WebElement properties view, flow editor and common view
        /// clean and set all views disabled 
        /// </summary>
        private void initUI() {
            wePanel.setWebBrowser(this.webBrowserEx1);
            addDebugPanel();
            wePanel.initWEPropertiesView();
            wePanel.UpdateWEPropViewEvt += new EventHandler<CommonEventArgs>(wePanel_UpdateWEPropViewEvt);
            initFlowEditor();
            InitCommonView();
            this.dbgEngine.LogReleaseMsgUpdated += new EventHandler<CommonEventArgs>(dbgEngine_LogReleaseMsgUpdated);
            this.dbgEngine.ReqURLViolationEvt += new EventHandler<CommonEventArgs>(dbgEngine_ReqURLViolationEvt);
            //this.openFileDialog1.InitialDirectory = UIConstants.SCRIPT_PATH;
            this.doTitleUpdate(null);

            this.tsb_login.Text = UILangUtil.getMsg("user.login.text1"); // login
            this.tsb_login.Tag = Constants.USER_STATUS_LOGOUT;
            this.tssl_user.ToolTipText = UILangUtil.getMsg("user.login.status.msg1"); // No user not login
        }

        void dbgEngine_ReqURLViolationEvt(object sender, CommonEventArgs e) {
            string msg = e.Data as string;
            MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg, "", MessageBoxButtons.OK, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
        }
        /// <summary>
        /// log user log message 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgEngine_LogReleaseMsgUpdated(object sender, CommonEventArgs e) {
            String msg = e.Data as String;
            //msgs in log tab page
            UIUtils.logMsg(msg, this.rtb_Log);
        }
        private void addDebugPanel() {
            debugPanel = new DebugPanel();
            // 
            // debugPanel
            // 
            this.debugPanel.Bigmodel = null;
            this.debugPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugPanel.Location = new System.Drawing.Point(0, 0);
            this.debugPanel.Name = "debugPanel";
            this.debugPanel.Size = new System.Drawing.Size(1044, 245);
            this.debugPanel.WebBrowser = this.webBrowserEx1;
            this.debugPanel.RestoredHeight = this.splitContainer1.SplitterDistance;
            debugPanel.PanelStatusChangedEvt += new EventHandler<CommonEventArgs>(debugPanel_PanelStatusChangedEvt);

            debugPanel.init(this.webBrowserEx1,dbgEngine);

            this.splitContainer1.Panel2.Controls.Add(this.debugPanel);
        }

        private void initFlowEditor() {
            // clean flow editor
            this.flowEditor1.setBigModel(null);
            this.flowEditor1.App = this;
        }

        private void InitCommonView() {
            this.scriptCommonInfoView1.setDevMode(true);
            this.scriptCommonInfoView1.setScriptRoot(null);
        }
        #endregion UI common function
        #region UI event handler
        /// <summary>
        /// reset the WebElement properties view as default 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wePanel_UpdateWEPropViewEvt(object sender, CommonEventArgs e) {
            int dis = this.splitContainer1.SplitterDistance;
            if(WebElementPanel.CMD_MIN.Equals(e.Data.ToString())){
                dis = this.splitContainer1.Size.Height - 25;                
            } else if (WebElementPanel.CMD_RESET.Equals(e.Data.ToString())) {
                dis = this.splitContainer1.Size.Height - 296;                
            }
            if (this.splitContainer1.SplitterDistance != dis) {
                this.splitContainer1.SplitterDistance = dis;
            }
        }

        void scriptCommonInfoView1_UpdateModelEvt(object sender, CommonEventArgs e) {
            this.markDirty();
        }
        void scriptCommonInfoView1_UpdateUrlTextBoxEvt(object sender, CommonEventArgs e) {
            TextBox tb = e.Data as TextBox;
            if (tb != null) {
                tb.Text = this.ts_url.Text;
                this.markDirty();
            }
        }        
        void wePanel_NewWebElementAddedEvt(object sender, CommonEventArgs e) {
            this.flowEditor1.addNewWENode(e.Data as WebElement);
            this.markDirty();
        }
        void wePanel_UpdateExistedWebElementEvt(object sender, CommonEventArgs e) {
            this.flowEditor1.updateExistedWENodeAndPropViewIfNeed(e.Data as WebElement);
            this.markDirty();
        }
        /// <summary>
        /// update existed WebElement, triggerred from the WebElement tree list view 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void flowEditor1_UpdateWebElement(object sender, CommonEventArgs e) {
            if (e.Data is WebElement) {                
                this.wePanel.updatePropertiesView(e.Data as WebElement);
                showWebElementPanel();
            } 
        }
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e) {
            debugPanel.RestoredHeight = splitContainer1.SplitterDistance;
        }
        /// <summary>
        /// e.data is the new debug panel status, it should be 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void debugPanel_PanelStatusChangedEvt(object sender, CommonEventArgs e) {
            String status = null;
            if (e.Data != null) {
                status = e.Data.ToString();
            }
            if(status.Equals(WindowStatus.MAX.ToString())){
                int oldValue = this.debugPanel.RestoredHeight;
                splitContainer1.SplitterDistance = 0;
                this.debugPanel.RestoredHeight = oldValue;
            } else if (status.Equals(WindowStatus.MIN.ToString())) {
                int oldValue = this.debugPanel.RestoredHeight;                
                splitContainer1.SplitterDistance = splitContainer1.Height-30;
                this.debugPanel.RestoredHeight = oldValue;
            } else if (status.Equals(WindowStatus.RESTORED.ToString())) {
                int oldValue = this.debugPanel.RestoredHeight;
                splitContainer1.SplitterDistance = debugPanel.RestoredHeight;                
                this.debugPanel.RestoredHeight = oldValue;
            }            
        }

        private void tabPage4_SizeChanged(object sender, EventArgs e) {
            int x = this.tabPage4.ClientSize.Width - this.btn_cleanLog.Size.Width - 3;
            this.btn_cleanLog.Location = new Point(x, 3);
        }

        private void btn_cleanLog_Click(object sender, EventArgs e) {
            this.rtb_Log.Clear();
        }
        #endregion UI event handler 
        #region browser event handler
        void webBrowserEx1_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e) {
            if (e.Url == null || Constants.URL_BLANK.Equals(e.Url.OriginalString)) {
                // filter blank frame
                return;
            } else if (e.Url.Equals(this.webBrowserEx1.Url)) {
                wePanel.handleNavigating(sender, e);                
            }
            this.tsb_refresh.Image = global::ide.Properties.Resources.loading16;
            Log.println_brw("Navigating.             url = " + e.Url+", browser url="+this.webBrowserEx1.Url+", frame = "+e.TargetFrameName);                
        }
        private void webBrowserEx1_Navigated(object sender, WebBrowserNavigatedEventArgs e) {
            string url = this.webBrowserEx1.Url.ToString();
            if (!this.ts_url.Text.Equals(url)) {
                this.ts_url.Text = url;
            }
            Log.println_brw("Navigated.              url = " + e.Url+", browser url = "+this.webBrowserEx1.Url);                
        }
        void webBrowserEx1_StartNewWindow(object sender, lib.ui.browser.BrowserExtendedNavigatingEventArgs e) {
            // When do script logging, make sure all the actions are occurred in the same browser 
            e.Cancel = true;
            HtmlElement elem = this.webBrowserEx1.Document.ActiveElement;
            string url = elem.GetAttribute("href");
            if(url!=null && url.Length>0){
                elem.SetAttribute("target", "_self");
                this.webBrowserEx1.Navigate(url);
            }
        }
        void webBrowserEx1_NewWindow(object sender, System.ComponentModel.CancelEventArgs e) {
            Log.println_brw("new window.             e.cancel = " + e.Cancel);
            //e.Cancel = true;
        }
        private void webBrowserEx1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            if (this.webBrowserEx1.IsBusy || this.webBrowserEx1.ReadyState != WebBrowserReadyState.Complete) {
                Log.println_brw("document completed in process. url = " + e.Url);
                return;
            }
            this.tsb_refresh.Image = global::ide.Properties.Resources.refresh16;
            Log.println_brw("document completed ................................ e. url = " + e.Url);
        }
        private void webBrowserEx1_DownloadComplete(object sender, EventArgs e) {
            Log.println_brw("download complete,      url= "+this.webBrowserEx1.Url);
        }
        private void webBrowserEx1_Downloading(object sender, EventArgs e) {
            Log.println_brw("downloading...,         url="+this.webBrowserEx1.Url);
        }
        void webBrowserEx1_StatusTextChanged(object sender, System.EventArgs e) {
            //Log.println_wme("Status Text Changed, text= "+this.webBrowserEx1.StatusText);
            this.toolStripStatusLabel1.Text = this.webBrowserEx1.StatusText;
        }
        void webBrowserEx1_CanGoForwardChanged(object sender, System.EventArgs e) {
            Log.println_brw("Can go forward changed ");
            updateBrowserButtonStatus();
        }
        void webBrowserEx1_CanGoBackChanged(object sender, System.EventArgs e) {
            Log.println_brw("Can go forward changed ");
            updateBrowserButtonStatus();
        }
        void webBrowserEx1_DocumentTitleChanged(object sender, System.EventArgs e) {
            doTitleUpdate(this.webBrowserEx1.Document.Title);
            Log.println_brw("Document title changed, title = "+this.webBrowserEx1.Document.Title+", url = "+this.webBrowserEx1.Url);            
        }
        private void tsb_back_Click(object sender, EventArgs e) {
            if (this.webBrowserEx1.CanGoBack) {
                this.webBrowserEx1.GoBack();
            }
        }
        private void updateBrowserButtonStatus() {
            if (this.webBrowserEx1.CanGoBack) {
                this.tsb_back.Enabled = true;
            } else {
                this.tsb_back.Enabled = false;
            }
            if (this.webBrowserEx1.CanGoForward) {
                this.tsb_fwd.Enabled = true;
            } else {
                this.tsb_fwd.Enabled = false;
            }
        }
        private void tsb_fwd_Click(object sender, EventArgs e) {
            if (this.webBrowserEx1.CanGoForward) {
                this.webBrowserEx1.GoForward();
            }
        }
        private void toolStripUrl_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                if (this.tabControl1.SelectedTab != tabPage1) {
                    this.tabControl1.SelectedTab = tabPage1;
                }
                string url = this.webBrowserEx1.Url != null ? this.webBrowserEx1.Url.ToString() : "";
                if (!url.Contains(this.ts_url.Text)) {
                    this.wePanel.resetPropPanel();
                }
                this.webBrowserEx1.Navigate(this.ts_url.Text);
            }
        }
        private void tsb_refresh_Click(object sender, EventArgs e) {
            this.webBrowserEx1.Refresh();
        }
        #endregion browser event handler
        #region tool bar script function
        /// <summary>
        /// create a new big model  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Click_CreateScript(object sender, EventArgs e) {
            //check if there is an active script working           
            if (this.SRoot != null) {
                string msg = UILangUtil.getMsg("ide.editor.dlg.save.msg1");
                string title = UILangUtil.getMsg("dlg.warn.title");
                DialogResult dr = MessageBoxEx.showMsgDialog(this,msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                
                if (dr == System.Windows.Forms.DialogResult.OK) {
                    //TODO do save the current script to server.
                    submitBigModel();

                    cleanWMUI();

                    // create new big model  
                    this.Bigmodel = ModelFactory.createBigModel();
                    this.SRoot.Author = getUserName();
                    this.scriptCommonInfoView1.setScriptRoot(this.SRoot);
                    // initial flow editor view
                    flowEditor1.setBigModel(this.Bigmodel);                    
                    // show script info page 
                    this.tabControl1.SelectedTab = this.tabPage2;
                    this.scriptCommonInfoView1.setInitFocus();
                    // update toolbar status                     
                    this.tsb_Save.Enabled = true;
                    if (this.getUserName() == UIConstants.USER_GUEST) {
                        this.tsb_Load.Enabled = false;
                        this.tsb_publish.Enabled = false;
                    } else {
                        this.tsb_Load.Enabled = true;
                        this.tsb_publish.Enabled = true;
                    }
                    this.tsb_PropertyPanel.Enabled = true;
                    this.tsb_check.Enabled = true;                    
                }
            } else {
                cleanWMUI();

                // create new big model 
                this.Bigmodel = ModelFactory.createBigModel();
                this.SRoot.Author = getUserName();
                this.scriptCommonInfoView1.setScriptRoot(this.SRoot);
                // initial flow editor view
                flowEditor1.setBigModel(this.Bigmodel);                
                // show script info page 
                this.tabControl1.SelectedTab = this.tabPage2;
                this.scriptCommonInfoView1.setInitFocus();

                // update toolbar status                     
                this.tsb_Save.Enabled = true;
                if (this.getUserName() == UIConstants.USER_GUEST) {
                    this.tsb_Load.Enabled = false;
                    this.tsb_publish.Enabled = false;
                } else {
                    this.tsb_Load.Enabled = true;
                    this.tsb_publish.Enabled = true;
                }
                this.tsb_PropertyPanel.Enabled = true;
                this.tsb_check.Enabled = true;
            }
        }
        /// <summary>
        /// clean WebMaster UI info
        /// </summary>
        private void cleanWMUI() {
            // clean script info page
            this.scriptCommonInfoView1.cleanView();
            // clean WebElement properties area
            this.wePanel.cleanActiveView();
            // clean flow editor
            this.flowEditor1.cleanView();
        }
        /// <summary>
        /// submit the BigModel to remote server
        /// </summary>
        /// <param name="bgmodel"></param>
        private void submitBigModel() {
            if (this.Bigmodel != null && this.tsb_Save.Enabled == true) {
                //string temppath = UIConstants.SCRIPT_PATH + "\\" + this.Bigmodel.SRoot.Name.Trim()+".wd";
                string folder = Application.StartupPath + "\\script\\design\\";
                if (!System.IO.Directory.Exists(folder)) {
                    System.IO.Directory.CreateDirectory(folder);
                }
                string filepath = folder+this.Bigmodel.SRoot.Name.Trim() + ".wd";
                Log.println_eng("submit big model, path = " + filepath);
                ModelManager.Instance.saveBigModel(this.Bigmodel, filepath);
            }

            this.tsb_Save.Enabled = false;
            //this.tsb_publish.Enabled = false;
        }
        /// <summary>
        /// This is used to update the contributor info if need. 
        /// </summary>
        private void updateContributor() {
            if (user != null && this.SRoot != null) { 
                if(this.SRoot.Author ==null || this.SRoot.Author.Length==0){
                    this.SRoot.Author = user.Name ;
                }else if(user.Name != this.SRoot.Author){
                    // check the contributor list 
                    if(!this.SRoot.Contributors.Contains(user.Name)){
                        this.SRoot.Contributors.Add(user.Name);                        
                    }
                }
            }
        }
        /// <summary>
        /// publish the script to remote server
        /// </summary>
        /// <param name="bgmodel"></param>
        private void publishScript() {
            string msg = UILangUtil.getMsg("ide.script.publish.warning");
            string title = UILangUtil.getMsg("dlg.info.title"); // info 
            DialogResult dr = MessageBoxEx.showMsgDialog(this, msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, FormStartPosition.CenterParent);
            if (dr == System.Windows.Forms.DialogResult.Cancel) {
                return;
            }
            if (this.Bigmodel != null && this.tsb_publish.Enabled == true) {
                //string temppath = UIConstants.SCRIPT_PATH + "\\" + this.Bigmodel.SRoot.Name.Trim()+".wd";
                string folder = Application.StartupPath + "\\script\\release\\";
                if (!System.IO.Directory.Exists(folder)) {
                    System.IO.Directory.CreateDirectory(folder);
                }
                string filepath = folder + this.Bigmodel.SRoot.Name.Trim() + ".ws";
                Log.println_eng("submit big model, path = " + filepath);
                ModelManager.Instance.saveScript(this.Bigmodel.SRoot, filepath);
            }
            //this.tsb_Save.Enabled = false;
            this.tsb_publish.Enabled = false;
        }        
        private void tsb_Click_saveScript(object sender, EventArgs e) {
            ShortCut_CTRL_S();
        }

        private void tsb_publish_Click(object sender, EventArgs e) {
            ShortCut_CTRL_P();
        }
        /// <summary>
        /// load script from server 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Click_loadScript(object sender, EventArgs e) {
            bool load = true;
            //check if there is an active script working           
            if (this.SRoot != null && this.tsb_Save.Enabled == true) {
                string msg = UILangUtil.getMsg("ide.editor.dlg.save.msg1");
                string title = UILangUtil.getMsg("dlg.warn.title");
                DialogResult dr = MessageBoxEx.showMsgDialog(this,msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,FormStartPosition.CenterParent);

                if (dr == System.Windows.Forms.DialogResult.Yes) {
                    //TODO do save the current script to server.
                    submitBigModel();
                    load = true;
                }else if (dr == System.Windows.Forms.DialogResult.No) {
                    load = true;
                } else {
                    load = false;
                }
            }
            if (load) {
                this.openFileDialog1.InitialDirectory = Application.StartupPath + "\\script\\design";
                DialogResult result = this.openFileDialog1.ShowDialog();
                if (result == DialogResult.OK) {
                    string temppath = openFileDialog1.FileName;
                    Log.println_eng("load file path = " + temppath);
                    this.Bigmodel = ModelManager.Instance.loadBigModel(temppath);
                    // update contributor if need 
                    updateContributor();
                    
                    this.scriptCommonInfoView1.setScriptRoot(this.SRoot);

                    this.flowEditor1.setBigModel(this.Bigmodel);

                    // open target url in browser area
                    string url = this.Bigmodel.SRoot.TargetWebURL;
                    if (url != null) {
                        this.webBrowserEx1.Navigate(this.Bigmodel.SRoot.TargetWebURL);
                    }
                    // expend WebElement tree view 
                    this.flowEditor1.expendWERootTree();

                    this.tabControl1.SelectedTab = tabPage2;
                    // update the url 
                    this.ts_url.Text = this.SRoot.TargetWebURL;                                        
                    this.doTitleUpdate(null);
                   
                    // update toolbar status                     
                    this.tsb_Save.Enabled = false;
                    if (this.getUserName() == UIConstants.USER_GUEST) {
                        this.tsb_Load.Enabled = false;
                        this.tsb_publish.Enabled = false;
                    } else {
                        this.tsb_Load.Enabled = true;
                        this.tsb_publish.Enabled = true;
                    }
                    this.tsb_PropertyPanel.Enabled = true;
                    this.tsb_check.Enabled = true;
                }
            }
            
        }
        /// <summary>
        /// check the script validation info, and put the verification 
        /// result in error log page, verification point: 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_check_Click(object sender, EventArgs e) {
            // update UI to make sure the editor page shown
            this.tabControl1.SelectedTab = tabPage2;
            this.flowEditor1.validateScriptAll();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_PropertyPanel_Click(object sender, EventArgs e) {            
            if (tsb_PropertyPanel.Tag.Equals("attribute")) {
                showWebElementPanel();
            } else {
                // turn to show debug panel 
                showDebugPanel();
            }            
        }

        private void showWebElementPanel() {
            // turn to show attribute panel 
            this.debugPanel.Visible = false;
            this.wePanel.initialPanel();
            this.wePanel.Visible = true;
            tsb_PropertyPanel.Tag = "debug";
            tsb_PropertyPanel.Image = global::ide.Properties.Resources.debug16;
            tsb_PropertyPanel.ToolTipText = "Show debug panel";

            this.tabControl1.SelectedTab = this.tabPage1;
        }
        /// <summary>
        /// show debug panel and update the debug button status 
        /// </summary>
        public void showDebugPanel() {
            this.wePanel.Visible = false;
            this.debugPanel.Bigmodel = this.Bigmodel;
            this.debugPanel.Visible = true;
            tsb_PropertyPanel.Tag = "attribute";
            tsb_PropertyPanel.Image = global::ide.Properties.Resources.we_attr16;
            tsb_PropertyPanel.ToolTipText = "Show WebElement panel";
            this.tabControl1.SelectedTab = this.tabPage1;
        }
        /// <summary>
        /// mark the save button as enabled 
        /// </summary>
        public void markDirty() {
            this.tsb_Save.Enabled = true;
            this.tsb_publish.Enabled = true;
        }

        private void WMEditor_Move(object sender, EventArgs e) {
            // This is used to handle a defect that when Maxmized the window, and then
            // restore the original size, the toolbar url part will be hidden. 
            if (this.ts_url.Tag != null) {
                this.ts_url.Tag = null;
            } else {
                this.ts_url.Size = new Size(200, this.ts_url.Size.Height);
            }
        }
        private void toolStrip1_SizeChanged(object sender, EventArgs e) {
            int w = this.toolStrip1.Size.Width;
            int h = this.toolStrip1.Size.Height;
            if (w < 800) {
                return;
            }
            int tw = this.tsb_fwd.Width + this.tsb_back.Width + this.tsb_refresh.Width + tss1.Width * 6
                + tsb_New.Width + tsb_Save.Width + tsb_Load.Width + tsb_PropertyPanel.Width + tsb_check.Width + tsb_publish.Width
                + tsl_space.Width + tsb_login.Width + tsddb_settings.Width;
            int uw = w - tw - 5;
            uw = uw < 200 ? 200 : uw;
            this.ts_url.Size = new Size(uw, this.ts_url.Size.Height);            
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e) {
            //UpdateUtil updater = new UpdateUtil(this, Constants.UPDATE_REMOTE_OBJ_URI);
            //updater.update();
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutDlg dlg = new AboutDlg();
            dlg.ShowDialog(this);
        }
        #endregion tool bar script function
        #region user and settings
        private void tsbLogin_Click(object sender, EventArgs e) {
            // disable the button to make sure there is no duplicated click. 
            tsb_login.Enabled = false;
            if (Constants.USER_STATUS_LOGOUT == tsb_login.Tag.ToString()) {   
                // check if the user has logined 
                // Try to check whether there is a user has logined.
                UserProfile tuser = WebUtil.getUserInfo(this.webBrowserEx1);
                //////////////////////////////////////////////////////////////////////
                //// fake code for easy development 
                //tuser = new UserProfile();
                //tuser.Name = "烽火";
                //tuser.Response = RESPONSE.LOGIN_SUCCESS;
                ///////////////////////////////////////////////////////////////////////
                if (tuser != null) {
                    this.user = tuser;
                    this.user.Response = RESPONSE.LOGIN_SUCCESS;
                    updateLoginPassed(user);
                    this.tsb_login.Enabled = true;
                    return;
                } else {
                    // do login 
                    LoginDialog dlg = new LoginDialog();
                    dlg.tempSetMethod(this.webBrowserEx1);
                    DialogResult dr = dlg.ShowDialog(this);
                    if (dr == System.Windows.Forms.DialogResult.OK) {
                        user = dlg.User;
                        updateLoginPassed(user);
                    } else {
                        // guest user ...
                        updateLoginFailed();
                    }
                }
            } else {
                // do logout 
                string text = UILangUtil.getMsg("dlg.user.logout.ask.text1");
                string title = UILangUtil.getMsg("dlg.user.logout.title");
                DialogResult dr = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, FormStartPosition.CenterParent);
                if (dr == System.Windows.Forms.DialogResult.OK) {
                    user = null;
                    updateLoginFailed();
                }
            }
            tsb_login.Enabled = true;
        }

        private void updateLoginFailed() {            
            this.tsb_Load.Enabled = false;
            this.tsb_Save.Enabled = false;
            this.tsb_publish.Enabled = false;
            this.tsb_check.Enabled = false;
            if (this.tsb_PropertyPanel.Tag!=null && this.tsb_PropertyPanel.Tag.ToString() == "attribute") {
                this.tsb_PropertyPanel.PerformClick();
            }
            this.tsb_PropertyPanel.Enabled = false;
            doTitleUpdate(null);

            // update login buttons status 
            this.tsb_login.Text = UILangUtil.getMsg("user.login.text1"); // login
            this.tsb_login.ToolTipText = UILangUtil.getMsg("user.login.text1");
            this.tsb_login.Tag = Constants.USER_STATUS_LOGOUT;
            this.tssl_user.ToolTipText = UILangUtil.getMsg("user.login.status.msg1"); // No user not login
            this.tssl_user.Image = global::ide.Properties.Resources.userOffline16;
        }

        private void updateLoginPassed(UserProfile user) {
            this.tsb_Load.Enabled = true;
            if (this.Bigmodel != null) {
                this.tsb_Save.Enabled = true;
                this.tsb_publish.Enabled = true;
                this.tsb_check.Enabled = true;
            } else {
                this.tsb_Save.Enabled = false;
                this.tsb_publish.Enabled = false;
                this.tsb_check.Enabled = false;
            }            
            doTitleUpdate(null);
            // update login buttons status 
            this.tsb_login.Text = UILangUtil.getMsg("user.logout.text1"); // logout 
            this.tsb_login.ToolTipText = UILangUtil.getMsg("user.logout.text1");
            this.tsb_login.Tag = Constants.USER_STATUS_LOGIN;
            this.tssl_user.ToolTipText = UILangUtil.getMsg("user.login.status.msg2", user.Name); // Welcome {0}
            this.tssl_user.Image = global::ide.Properties.Resources.userOnline16;
        }
        /// <summary>
        /// Update the title text info based on current active browser status and login info
        /// </summary>
        /// <param name="txt"></param>
        internal void doTitleUpdate(string txt) {
            string scriptName = "";
            if (this.Bigmodel != null && this.Bigmodel.SRoot != null) {
                scriptName = this.Bigmodel.SRoot.Name;

            }
            // user info 
            string ustr = UIConstants.USER_GUEST;
            if (user != null) {
                ustr = user.Name;
            }
            // title info 
            if (txt != null && txt.Length > 0) {
                this.Text = txt + " | " + scriptName + " - " + ustr;
            } else {
                this.Text = app_name + " | " + scriptName + " - " + ustr;
            }
        }
        private void tsbSettings_Click(object sender, EventArgs e) {

        }
        /// <summary>
        /// Get current user name if login sucessfully or return UIConstants.USER_GUEST if login failed.
        /// </summary>
        /// <returns></returns>
        private string getUserName() {
            if (user != null) {
                return user.Name;
            }
            return UIConstants.USER_GUEST;
        }

        #endregion user and settings        
        #region form event handler 
        private void WMEditor_Deactivate(object sender, EventArgs e) {
            this.wePanel.cancelRecording();
        }
        /// <summary>
        /// Use F5 to refresh the web browser 
        /// </summary>
        private void ShortCut_F5() {           
            if (this.webBrowserEx1 != null) {
                this.webBrowserEx1.Refresh();
            }
        }
        /// <summary>
        /// Use F5 to refresh the web browser 
        /// </summary>
        private void ShortCut_CTRL_S() {
            // update selectedObject in flow prop views if needed. 
            this.flowEditor1.updateExistedPropData();
            // upload script model and big model to the server. 
            submitBigModel();
        }
        /// <summary>
        /// Use F5 to refresh the web browser 
        /// </summary>
        private void ShortCut_CTRL_P() {
            // update selectedObject in flow prop views if needed
            this.flowEditor1.updateExistedPropData();
            if (this.flowEditor1.isValidScript()) {
                // upload script model to server 
                publishScript();
            } else {
                this.tabControl1.SelectedTab = tabPage2;
                string msg = UILangUtil.getMsg("dlg.validation.error.text1");
                string title = UILangUtil.getMsg("dlg.title.err.text");
                MessageBoxEx.showMsgDialog(this, msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error, FormStartPosition.CenterParent);
            }
        }       
        private void WMEditor_Load(object sender, EventArgs e) {
            // register hot keys
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.CTRL, Keys.S, submitBigModel);
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.NONE, Keys.F5, HotKey_F5);
            //// Comments: for the debug_Run methods was updated to support new function, maybe I will create a new method for register, you can check the 
            //// google calendar for 2012/5/4 update
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.NONE, Keys.F5, debug_Run);
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.NONE, Keys.F6, debug_StepOver);
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.NONE, Keys.F7, debug_StepInto);
        }

        private void WMEditor_FormClosed(object sender, FormClosedEventArgs e) {
            // remove hot keys
            //HotkeyUtil.UnRegist(this.Handle, submitBigModel);
            //HotkeyUtil.UnRegist(this.Handle, HotKey_F5);
            //// Comments: for the debug_Run methods was updated to support new function, maybe I will create a new method for register 
            //HotkeyUtil.UnRegist(this.Handle, debug_Run);
            //HotkeyUtil.UnRegist(this.Handle, debug_StepOver);
            //HotkeyUtil.UnRegist(this.Handle, debug_StepInto);
        }
        /// <summary>
        /// Use F5 to refresh the web browser 
        /// </summary>
        private void HotKey_F5() {            
            if (this.webBrowserEx1 != null) {
                this.webBrowserEx1.Refresh();
            }
        }
        #endregion form event 
        #region debug panel
        /// <summary>
        /// start to run the script from the operation, it is only occurred in the debug mode
        /// Steps: 
        /// 1. make sure the debug panel is visible
        /// 2. make sure the engine is in stop status 
        /// 3. run the script from operation 
        /// </summary>
        /// <param name="op"></param>
        internal void debug_Run(Operation op) {
            // show the debug panel 
            if (debugPanel.Visible == false) {
                showDebugPanel();
            }
            // make sure the engine is in stop status 
            if (dbgEngine.Status != ENGINE_STATUS.STOPED) {
                string msg = UILangUtil.getMsg("ide.editor.dlg.debug.stop.msg1");
                string title = UILangUtil.getMsg("dlg.info.title");
                DialogResult dr = MessageBox.Show(this, msg, title, MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                if (dr == System.Windows.Forms.DialogResult.Yes) {
                    debugPanel.stop();
                    // make sure the engine is stoped 
                    int i=0;
                    while (i++ < 15) {
                        if (dbgEngine.isBusy()) {
                            Thread.Sleep(200);
                        } else {
                            break;
                        }
                    }
                    // stop engine failed or timeout 
                    if (i == 15) {
                        msg = UILangUtil.getMsg("ide.editor.dlg.debug.stop.msg2");
                        title = UILangUtil.getMsg("dlg.error.title");
                        MessageBox.Show(this, title, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                } else {
                    return;
                }
            }
            this.debugPanel.run(op);            
        }
        /// <summary>
        /// start to run the script from the operation, it is only occurred in the debug mode
        /// Steps: 
        /// 1. make sure the debug panel is visible
        /// 2. make sure the engine is in stop status 
        /// 3. run the script from operation 
        /// </summary>
        internal void debug_Run() {                        
            this.debugPanel.run(null);
        }
        internal void debug_StepOver() {
            if (debugPanel.Visible) {
                this.debugPanel.stepover();
            }
        }
        internal void debug_StepInto() {
            if (debugPanel.Visible) {
                this.debugPanel.stepover();
            }
        }
        internal WebEngine getDebugEngine() {
            return dbgEngine;
        }
        #endregion debug panel                        
                               
    }
}
