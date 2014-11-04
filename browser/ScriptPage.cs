using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.com.script;
using WebMaster.lib.ui.browser;
using WebMaster.lib;
using WebMaster.com;

namespace WebMaster.browser
{
    public partial class ScriptPage : UserControl
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
                doSRootChanged();
            }
        }

        private void doSRootChanged() {
            this.scriptCommonInfoView1.setScriptRoot(_sroot);
            ParamGroup paramGrp = null;
            if (_sroot != null) {
                paramGrp = _sroot.ProcRoot.ParamPublic;
            }
            this.globalParamConfigView1.setInput(paramGrp);
            this.btn_save.Enabled = false;
        }    
        /// <summary>
        /// Engine for release
        /// </summary>
        private WebEngine releaseEngine = null;

        private WebBrowserEx webBrowserEx1 = null;
        /// <summary>
        /// Remove previous webBrowser if have, and then Add the new WebBroswe into browser tab. 
        /// </summary>
        public WebBrowserEx WebBrowser {
            get { return webBrowserEx1; }
            set {
                if (this.webBrowserEx1 != null) {
                    if (!webBrowserEx1.Equals(value)) {
                        this.tab_browser.Controls.Remove(this.webBrowserEx1);
                        this.webBrowserEx1.Dispose();
                    } else {
                        return;
                    }
                }
                this.webBrowserEx1 = value;
                this.tab_browser.Controls.Add(this.webBrowserEx1);

                this.releaseEngine = new WebEngine(this.backgroundWorker1, this.webBrowserEx1, WebEngine.MODE_RELEASE);
                this.releaseEngine.LogReleaseMsgUpdated += new EventHandler<CommonEventArgs>(releaseEngine_LogReleaseMsgUpdated);
                this.releaseEngine.ReqURLViolationEvt += new EventHandler<CommonEventArgs>(releaseEngine_ReqURLViolationEvt);
                this.releaseEngine.StatusChanged += new EventHandler<CommonCodeArgs>(releaseEngine_StatusChanged);
                this.tab_browser.Invalidate();
            }
        }    
        /// <summary>
        /// this is a runtime user info, including the user ticket when login sucessfully. 
        /// </summary>
        private UserProfile user = null;

        #endregion variables
        #region events
        /// <summary>
        /// Sender is the ScriptPage, data is status code 
        /// </summary>
        public event EventHandler<CommonCodeArgs> EngineStatusChanged;

        /// <summary>
        /// Raises the EngineStatusChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEngineStatusChanged(CommonCodeArgs e) {
            if (EngineStatusChanged != null) {
                EngineStatusChanged(this, e);
            }
        }
        #endregion events 
        public ScriptPage() {
            InitializeComponent();
            initUI();
        }
        #region event handling 
        
        void scriptCommonInfoView1_UpdateModelEvt(object sender, CommonEventArgs e) {
            this.btn_save.Enabled = true;
        }

        void globalParamConfigView1_ConfigChanged(object sender, CommonEventArgs e) {
            this.btn_save.Enabled = true;
        }

        void releaseEngine_StatusChanged(object sender, CommonCodeArgs e) {
            if ((ENGINE_STATUS)(e.Code) == ENGINE_STATUS.STOPED) {
                OnEngineStatusChanged(new CommonCodeArgs(this, e.Code));
            }
        }
        /// <summary>
        /// log user log message 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void releaseEngine_LogReleaseMsgUpdated(object sender, CommonEventArgs e) {
            String msg = e.Data as String;
            //msgs in log tab page
            UIUtils.logMsg(msg, this.rtb_Log);
        }
        void releaseEngine_ReqURLViolationEvt(object sender, CommonEventArgs e) {
            string msg = e.Data as string;
            MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), msg, "", MessageBoxButtons.OK, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
        }

        private void btn_save_Click(object sender, EventArgs e) {
            //check if there is an active script working           
            if (this.SRoot != null) {
                //do save the current script to server.
                saveScriptWithConfig();

                string msg = UILangUtil.getMsg("browser.script.dlg.save.msg1", this.SRoot.Name); // Script { {0} } Configuration saved sucessfully. 
                string title = UILangUtil.getMsg("dlg.info.title");
                MessageBoxEx.showMsgDialog(this, msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information, FormStartPosition.CenterParent);
            }
        }

        private void btn_cleanLog_Click(object sender, EventArgs e) {
            this.rtb_Log.Clear();
        }

        private void tab_log_SizeChanged(object sender, EventArgs e) {
            int x = this.tab_log.ClientSize.Width - this.btn_cleanLog.Size.Width - 3;
            this.btn_cleanLog.Location = new Point(x, 3);
        }

        #endregion event handling 
        #region UI common functions
        /// <summary>
        /// init all views: WebElement properties view, flow editor and common view
        /// clean and set all views disabled 
        /// </summary>
        private void initUI() {
            InitCommonView();
            this.globalParamConfigView1.ConfigChanged += new EventHandler<CommonEventArgs>(globalParamConfigView1_ConfigChanged);
            this.scriptCommonInfoView1.UpdateModelEvt += new EventHandler<CommonEventArgs>(scriptCommonInfoView1_UpdateModelEvt);
        }
        private void InitCommonView() {
            this.scriptCommonInfoView1.setDevMode(false);
            this.scriptCommonInfoView1.setScriptRoot(null);
            this.globalParamConfigView1.setInput(null);
        }
        internal void runScript() {
            if ((releaseEngine == null || this.SRoot == null || this.SRoot.ProcRoot == null || this.SRoot.ProcRoot.StartOp == null)
                || isScriptRuning()) {
                return;
            }
            this.releaseEngine.LogLevel = Logger.LOG_USER_MSG;
            releaseEngine.Mode = WebEngine.MODE_RELEASE;
            releaseEngine.DebugCmd = DEBUG_CMD.NONE;

            Log.println_eng("T = Release Run script , start ================== ");
            if (releaseEngine.Status == ENGINE_STATUS.STOPED) {
                releaseEngine.ReqStartTime = DateTime.Now;
                releaseEngine.SRoot = this.SRoot;
                releaseEngine.RunWorkerAsync(this.SRoot.ProcRoot.StartOp);
            }
        }
        /// <summary>
        /// Whether the browser page is active page 
        /// </summary>
        /// <returns></returns>
        internal bool isBrowserPageActive() {
            return this.tabControl1.SelectedTab == this.tab_browser;
        }
        /// <summary>
        /// Active Browser page if deactive 
        /// </summary>
        internal void activeBrowserPage() {
            if (!isBrowserPageActive()) {
                this.tabControl1.SelectedTab = this.tab_browser;
            }
        }
        /// <summary>
        /// whether the script is running, true : running, false : stoped. 
        /// </summary>
        /// <returns></returns>
        public bool isScriptRuning() {
            return releaseEngine != null && (releaseEngine.Status == ENGINE_STATUS.RUNING || releaseEngine.Status == ENGINE_STATUS.INITIALIZING);
        }

        internal void stopScript() {
            if (isScriptRuning()) {
                releaseEngine.stopScript();
            }
        }

        /// <summary>
        /// save the script with config info to remote server
        /// </summary>
        /// <param name="bgmodel"></param>
        private void saveScriptWithConfig() {
            if (this.SRoot != null) {
                //string temppath = UIConstants.SCRIPT_PATH + "\\" + this.Bigmodel.SRoot.Name.Trim()+".wd";
                string folder = Application.StartupPath + "\\script\\config\\";
                if (!System.IO.Directory.Exists(folder)) {
                    System.IO.Directory.CreateDirectory(folder);
                }
                string filepath = folder + this.SRoot.Name.Trim() + ".ws";
                Log.println_eng("submit big model, path = " + filepath);
                ModelManager.Instance.saveScript(this.SRoot, filepath);
            }
            this.btn_save.Enabled = false;
        }        
        #endregion UI common functions              

    }
}
