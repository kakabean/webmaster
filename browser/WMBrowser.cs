using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.com.script;
using WebMaster.com;
using WebMaster.lib.ui.browser;
using WebMaster.com.table;
using WebMaster.lib;

namespace WebMaster.browser
{
    public partial class WMBrowser : Form
    {
        #region constants 
        private static readonly string RUN_SCRIPT = "run";
        private static readonly string STOP_SCRIPT = "stop";
        #endregion constants 
        #region variables
        /// <summary>
        /// this is a runtime user info, including the user ticket when login sucessfully. 
        /// </summary>
        private UserProfile user = null;
        private ScriptPageManager spManager = new ScriptPageManager();
        private string app_name = "Mayi Browser";
        private string no_login = "Not Login";
        #endregion variables 
        public WMBrowser() {
            InitializeComponent();
            initData();            
        }
        #region override 
        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            HotkeyUtil.ProcessHotKey(m);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch(keyData){
                case Keys.F5:
                    ShortCut_F5();
                    break;
                case Keys.Control | Keys.Tab:
                    ShortCut_CTRL_Tab();
                    break ;
                case Keys.Control | Keys.T:
                    ShortCut_CTRL_T();
                    break ;
                case Keys.F9:
                    runScript();
                    break;
                case Keys.F10:
                    stopScript();
                    break;
                case Keys.Alt|Keys.F4:
                    ShortCut_ALT_F4();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion override methos 
        #region common methods 
        private void initData() {
            app_name = UILangUtil.getMsg("app.name.text");
            no_login = UILangUtil.getMsg("app.user.no.login");
            // update title area 
            this.doTitleUpdate(null);
            this.ts_url.Text = Constants.URL_BLANK;
            tsb_run.Tag = RUN_SCRIPT;            
            spManager.setTabContrlRoot(this.tabControlEx1);
            spManager.StatusTextChanged += new EventHandler<CommonEventArgs>(spManager_StatusTextChanged);
            spManager.TitleTextChanged += new EventHandler<CommonEventArgs>(spManager_TitleTextChanged);
            spManager.ToolbarStatusChanged += new EventHandler<CommonEventArgs>(spManager_ToolbarStatusChanged);
            spManager.URLTextChanged += new EventHandler<CommonEventArgs>(spManager_URLTextChanged);
            spManager.EngineStatusChanged += new EventHandler<CommonCodeArgs>(spManager_EngineStatusChanged);
            TabPageEx tpage = this.tabControlEx1.addNewPage(true, null);            
            if (tpage != null) {
                spManager.ActiveBrowser = tpage.WebBrowser;
            }

            this.tsb_login.Text = UILangUtil.getMsg("user.login.text1"); // login
            this.tsb_login.Tag = Constants.USER_STATUS_LOGOUT;
            this.tssl_user.ToolTipText = UILangUtil.getMsg("user.login.status.msg1"); // No user not login
        }
        #endregion common methods
        #region events handler
        private void tabControlEx1_NewTabPage(object sender, TabPageActiveEventArgs e) {
            if (e.ActivePage != null) {
                spManager.initBrowserEvtHanders(e.ActivePage.WebBrowser);
            }
        }

        void tabControlEx1_ActiveTabPage(object sender, TabPageActiveEventArgs e) {
            TabPageEx tbpage = e.ActivePage;//this.tabControlEx1.getActivePage();
            if (tbpage != null && tbpage.WebBrowser != null) {
                // update spManager 
                if (e.ActivePage != null) {
                    spManager.ActiveBrowser = e.ActivePage.WebBrowser;
                    if (e.ActivePage.Page.Controls[0] is ScriptPage) {
                        spManager.ActiveScriptPage = e.ActivePage.Page.Controls[0] as ScriptPage;
                    } else {
                        spManager.ActiveScriptPage = null;
                    }
                }
                // update title                 
                if (tbpage.WebBrowser.Document != null) {
                    doTitleUpdate(tbpage.WebBrowser.DocumentTitle);
                } else {
                    this.doTitleUpdate(null);
                }
                // update url text box 
                string url = getBrowserURL(tbpage);
                this.ts_url.Text = url;
                if (url == Constants.URL_BLANK) {
                    this.ts_url.Focus();
                } else {
                    tbpage.WebBrowser.Focus();
                }               
                
                // update toolbar buttons status 
                this.doUpdateBrowserButonsStatus();
            }
        }
        /// <summary>
        /// Returun the page browser url if have or about:blank as default 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private string getBrowserURL(TabPageEx page) {
            if (page != null && page.WebBrowser != null && page.WebBrowser.Url != null) {
                return page.WebBrowser.Url.ToString();
            } else {
                return Constants.URL_BLANK;
            }            
        }

        void spManager_EngineStatusChanged(object sender, CommonCodeArgs e) {
            updateTsbRunStatus((ENGINE_STATUS)e.Code);
        }
        void spManager_StatusTextChanged(object sender, CommonEventArgs e) {
            this.tssl_status.Text = e.Data as string;
        }

        void spManager_TitleTextChanged(object sender, CommonEventArgs e) {
            doTitleUpdate(e.Data as string);
        }

        void spManager_ToolbarStatusChanged(object sender, CommonEventArgs e) {
            this.doUpdateBrowserButonsStatus();
        }

        void spManager_URLTextChanged(object sender, CommonEventArgs e) {
            WebBrowserEx browser = e.Sender as WebBrowserEx;
            if (this.spManager.ActiveBrowser == browser) {
                string url = e.Data as string;
                if (!this.ts_url.Text.Equals(url)) {
                    this.ts_url.Text = url;
                }
            }
        }
        
        /// <summary>
        /// Update the title text info based on current active browser status and login info
        /// </summary>
        /// <param name="txt"></param>
        internal void doTitleUpdate(string txt) {
            string scriptName = "";
            if (spManager != null && spManager.ActiveScriptPage != null) {
                ScriptPage sp = this.spManager.ActiveScriptPage;
                if (sp.SRoot != null) {
                    scriptName = sp.SRoot.Name;
                }
            }
            // user info 
            string ustr = UIConstants.USER_GUEST;
            if (user != null) {
                ustr = user.Name;
            }
            // title info 
            if (txt != null && txt.Length > 0) {
                this.Text = txt + " | "+scriptName+" - "+ustr;
            } else {                 
                this.Text = app_name + " | "+scriptName+" - " + ustr;
            }
        }

        private void WMBrowser_Move(object sender, EventArgs e) {
            // This is used to handle a defect that when Maxmized the window, and then
            // restore the original size, the toolbar url part will be hidden. 
            if (this.ts_url.Tag != null) {
                this.ts_url.Tag = null;
            } else {
                this.ts_url.Size = new Size(750, this.ts_url.Size.Height);
            }
        }
        
        private void WMBrowser_Load(object sender, EventArgs e) {
            // register hot keys
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.CTRL, Keys.T, HotKey_CTRL_T);
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.CTRL, Keys.Tab, HotKey_CTRL_Tab);
            //HotkeyUtil.Regist(this.Handle, HotkeyModifiers.NONE, Keys.F5, HotKey_F5);
        }

        private void WMBrowser_FormClosed(object sender, FormClosedEventArgs e) {
            // remove hot keys
            //HotkeyUtil.UnRegist(this.Handle, HotKey_CTRL_T);
            //HotkeyUtil.UnRegist(this.Handle, HotKey_CTRL_Tab);
            //HotkeyUtil.UnRegist(this.Handle, HotKey_F5);
        }

        private void WMBrowser_FormClosing(object sender, FormClosingEventArgs e) {
            int count = this.tabControlEx1.PageStack.Count;
            if (count > 1) {
                string msg = UILangUtil.getMsg("browser.close.msg1", count); // There are {0} pages opened, do you want to close all ? 
                string title = "";//UILangUtil.getMsg("");
                DialogResult dr = MessageBoxEx.showMsgDialog(this, msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, FormStartPosition.CenterParent);
                if (dr != System.Windows.Forms.DialogResult.Yes) {
                    e.Cancel = true;
                }
            }
        }
        /// <summary>
        /// Use F5 to refresh the web browser 
        /// </summary>
        private void ShortCut_F5() {
            WebBrowserEx browser = this.spManager.ActiveBrowser ;            
            if (browser != null) {
                browser.Refresh();
            }
        }
        /// <summary>
        /// Use control T to open a new Tab
        /// </summary>
        private void ShortCut_CTRL_T() {
            this.spManager.createNewPage();
        }

        private void ShortCut_ALT_F4() {
            if (this.tabControlEx1.PageStack.Count == 1) {
                this.Close();
            } else { 
                this.tabControlEx1.closeTab(this.tabControlEx1.getActivePage());
            }
        }
        /// <summary>
        /// Use CTRL+Tab switch to previous tab
        /// </summary>
        private void ShortCut_CTRL_Tab() {
            TabPageEx prePage = this.tabControlEx1.getPreviousPage();
            if (prePage != null) {
                this.tabControlEx1.setPageActive(prePage);
            }
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutDlg dlg = new AboutDlg();
            dlg.ShowDialog(this);
        }
        #endregion events handler 
        #region toolbar functions
        /// <summary>
        /// load script from server 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Click_loadScriptBAK(object sender, EventArgs e) {
            UserScriptListDlg dlg = new UserScriptListDlg();
            DialogResult dr = dlg.showScriptListDlg(UIUtils.getTopControl(this), true, user);
            if (dr == System.Windows.Forms.DialogResult.OK) {
                this.spManager.createNewPage(dlg.SRoot);
                if (dlg.SRoot != null) {                    
                    this.tsb_run.Enabled = true;
                } else {
                    this.tsb_run.Enabled = false;
                }
                this.doTitleUpdate(null);
            }
        }
        /// <summary>
        /// This is a temp version to just load script from file system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Click_loadScript(object sender, EventArgs e) {
            this.openFileDialog1.InitialDirectory = Application.StartupPath + "\\script\\config";
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                string temppath = openFileDialog1.FileName;
                Log.println_eng("load file path = " + temppath);
                ScriptRoot sroot = ModelManager.Instance.loadScript(temppath);
                if (sroot != null) {
                    this.spManager.createNewPage(sroot);
                    this.tsb_run.Enabled = true;
                    this.spManager.ActiveBrowser.Navigate(sroot.TargetWebURL);
                } else {
                    this.tsb_run.Enabled = false;
                }
                this.doTitleUpdate(null);
            }            
        }
        private void toolStrip1_SizeChanged(object sender, EventArgs e) {
            int w = this.toolStrip1.Size.Width;
            int h = this.toolStrip1.Size.Height;
            if (w < 800) {
                return;
            }
            int tw = this.tsb_fwd.Width + this.tsb_back.Width + this.tsb_refresh.Width + tss1.Width * 6 + tsb_Load.Width + tsb_run.Width + tsl_space.Width + tsb_login.Width + tsddb_settings.Width;
            int uw = w - tw - 5;
            uw = uw < 200 ? 200 : uw;
            this.ts_url.Size = new Size(uw, this.ts_url.Size.Height);
        }
        /// <summary>
        /// This is release run 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_run_Click(object sender, EventArgs e) {
            ScriptPage spage = this.spManager.ActiveScriptPage;
            if (spage != null) {
                if (RUN_SCRIPT == tsb_run.Tag.ToString()) {
                    runScript();
                } else {
                    stopScript();
                }
            }
        }
        /// <summary>
        /// Run script and update the run buttons status 
        /// </summary>
        private void runScript() { 
            ScriptPage spage = this.spManager.ActiveScriptPage;
            if (spage != null && !spage.isScriptRuning()) {
                if (RUN_SCRIPT == tsb_run.Tag.ToString()) {
                    spage.runScript();
                    this.tsl_space.Image = global::browser.Properties.Resources.running_script16;
                    tsb_run.Image = global::browser.Properties.Resources.dbg_stop16;
                    string text = UILangUtil.getMsg("toolbar.tooltip.stop");
                    tsb_run.ToolTipText = text;
                    tsb_run.Tag = STOP_SCRIPT;
                }
            }
        }
        /// <summary>
        /// /// Stop script and update the run buttons status 
        /// </summary>
        private void stopScript() {
            ScriptPage spage = this.spManager.ActiveScriptPage;
            if (spage != null && spage.isScriptRuning()) {
                if (RUN_SCRIPT != tsb_run.Tag.ToString()) {
                    spage.stopScript();
                    this.tsl_space.Image = null;
                    tsb_run.Image = global::browser.Properties.Resources.run_script16;
                    string text = UILangUtil.getMsg("toolbar.tooltip.run");
                    tsb_run.ToolTipText = text;
                    tsb_run.Tag = RUN_SCRIPT;
                }
            }
        }
        private void updateTsbRunStatus(ENGINE_STATUS status) {
            if (status == ENGINE_STATUS.STOPED) {
                this.tsl_space.Image = null;
                tsb_run.Image = global::browser.Properties.Resources.run_script16;
                string text = UILangUtil.getMsg("toolbar.tooltip.run");
                tsb_run.ToolTipText = text;
                tsb_run.Tag = RUN_SCRIPT;
            } else {
                this.tsl_space.Image = global::browser.Properties.Resources.running_script16;
                tsb_run.Image = global::browser.Properties.Resources.dbg_stop16;
                string text = UILangUtil.getMsg("toolbar.tooltip.stop");
                tsb_run.ToolTipText = text;
                tsb_run.Tag = STOP_SCRIPT;
            }
        }
        private void updateToolStripMenuItem_Click(object sender, EventArgs e) {
            //UpdateUtil updater = new UpdateUtil(this, Constants.UPDATE_REMOTE_OBJ_URI);
            //updater.update();
        }
        
        private void tsb_back_Click(object sender, EventArgs e) {
            WebBrowserEx browser = this.spManager.ActiveBrowser ;
            if (browser != null && browser.CanGoBack) {
                browser.GoBack();
            }
        }

        private void tsb_fwd_Click(object sender, EventArgs e) {
            WebBrowserEx browser = this.spManager.ActiveBrowser ;
            if(browser!=null && browser.CanGoForward){
                browser.GoForward();
            }
        }
        private void ts_url_KeyDown(object sender, KeyEventArgs e) {
            WebBrowserEx browser = this.spManager.ActiveBrowser;
            if (e.KeyData == Keys.Enter) {
                ScriptPage spage = this.spManager.ActiveScriptPage;
                if (spage != null && spage.isBrowserPageActive()) {
                    spage.activeBrowserPage();
                }                
                
                browser.Navigate(this.ts_url.Text);
            }
        }
        private void tsb_refresh_Click(object sender, EventArgs e) {
            WebBrowserEx browser = this.spManager.ActiveBrowser;
            if (browser != null) {
                browser.Refresh();
            }
        }
        
        internal void doUpdateBrowserButonsStatus() {
            WebBrowserEx browser = this.spManager.ActiveBrowser;
            if (browser != null) {
                if (browser.CanGoBack) {
                    this.tsb_back.Enabled = true;
                } else {
                    this.tsb_back.Enabled = false;
                }
                if (browser.CanGoForward) {
                    this.tsb_fwd.Enabled = true;
                } else {
                    this.tsb_fwd.Enabled = false;
                }
            }
            if (user!=null && user.Response == RESPONSE.LOGIN_SUCCESS) {
                this.tsb_Load.Enabled = true;
            } else {
                this.tsb_Load.Enabled = false;
            }
            if (spManager.ActiveScriptPage != null && spManager.ActiveScriptPage.SRoot != null) {
                if (spManager.ActiveScriptPage.isScriptRuning()) {
                    updateTsbRunStatus(ENGINE_STATUS.RUNING);
                } else {
                    updateTsbRunStatus(ENGINE_STATUS.STOPED);
                }
                
                this.tsb_run.Enabled = true;
            } else {
                tsl_space.Image = null;
                this.tsb_run.Enabled = false;
            }
        }
        #endregion toolbar functions
        #region status bar functions         
        internal void doStatusTextUpdate(WebBrowserEx browser) {
            if (browser != null && browser == this.spManager.ActiveBrowser) {
                string url = browser.StatusText;
                this.tssl_status.Text = url;
            } else {
                this.tssl_status.Text = string.Empty;
            }            
        }
        #endregion status bar functions 
        #region user and settings
        private void tsbLogin_Click(object sender, EventArgs e) {
            tsb_login.Enabled = false;
            if (Constants.USER_STATUS_LOGOUT == tsb_login.Tag.ToString()) {   
                // check if the user has logined 
                // Try to check whether there is a user has logined.
                UserProfile tuser = WebUtil.getUserInfo(this.spManager.ActiveBrowser);
                //////////////////////////////////////////////////////////////////////
                //// fake code for easy development 
                //tuser = new UserProfile();
                //tuser.Name = "GAGA";
                //tuser.Response = RESPONSE.LOGIN_SUCCESS;
                ///////////////////////////////////////////////////////////////////////
                if (tuser != null) {
                    this.user = tuser;
                    this.user.Response = RESPONSE.LOGIN_SUCCESS;
                    updateLoginPassed(user);
                    return;
                } else {
                    // do login 
                    LoginDialog dlg = new LoginDialog();
                    dlg.tempSetMethod(this.spManager.ActiveBrowser);
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
            doTitleUpdate(null);
            
            // update login buttons status 
            this.tsb_login.Text = UILangUtil.getMsg("user.login.text1"); // login
            this.tsb_login.ToolTipText = UILangUtil.getMsg("user.login.text1");
            this.tsb_login.Tag = Constants.USER_STATUS_LOGOUT;
            this.tssl_user.ToolTipText = UILangUtil.getMsg("user.login.status.msg1"); // No user not login
            this.tssl_user.Image = global::browser.Properties.Resources.userOffline16;
        }

        private void updateLoginPassed(UserProfile user) {
            this.tsb_Load.Enabled = true;
            if (this.spManager.ActiveScriptPage != null) {
                ScriptPage page = this.spManager.ActiveScriptPage;
                if (page.SRoot != null) {
                    this.tsb_run.Enabled = true;
                } else {
                    this.tsb_run.Enabled = false;
                }
            }
            this.doTitleUpdate(null);
            // update login buttons status 
            this.tsb_login.Text = UILangUtil.getMsg("user.logout.text1"); // logout 
            this.tsb_login.ToolTipText = UILangUtil.getMsg("user.logout.text1");
            this.tsb_login.Tag = Constants.USER_STATUS_LOGIN;
            this.tssl_user.ToolTipText = UILangUtil.getMsg("user.login.status.msg2", user.Name); // Welcome {0}
            this.tssl_user.Image = global::browser.Properties.Resources.userOnline16;
        }
        private void tsbSettings_Click(object sender, EventArgs e) {

        }
        private string getAuthorName() {
            if (user != null) {
                return user.Name;
            }
            return UIConstants.USER_GUEST;
        }


        #endregion user and settings       
                               
    }
}
