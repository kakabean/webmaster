using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.com.table;
using WebMaster.lib.ui.browser;
using WebMaster.lib.engine;
using System.Windows.Forms;
using WebMaster.lib;

namespace WebMaster.browser
{
    /// <summary>
    /// Use this class to manage the all opened TabPageEx in the root tab control. 
    /// </summary>
    internal class ScriptPageManager
    {
        #region variables 
        /// <summary>
        /// Currently active script page 
        /// </summary>
        private ScriptPage activeScriptPage = null;

        public ScriptPage ActiveScriptPage {
            get { return activeScriptPage; }
            set { activeScriptPage = value; }
        }
        private WebBrowserEx activeBrowser = null;
        /// <summary>
        /// Current active browser 
        /// </summary>
        public WebBrowserEx ActiveBrowser {
            get { return activeBrowser; }
            set { activeBrowser = value; }
        }
        /// <summary>
        /// Root container of all tab pages in the browser. 
        /// </summary>
        private TabControlEx tabCtrlRoot = null;
        
        #endregion variables
        #region events 
        /// <summary>
        /// Sender is the status changed WebBrowserEx, data is status txt. 
        /// </summary>
        public event EventHandler<CommonEventArgs> StatusTextChanged;

        /// <summary>
        /// Raises the StatusTextChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStatusTextChanged(CommonEventArgs e) {
            if (StatusTextChanged != null) {
                StatusTextChanged(this, e);
            }
        }
        /// <summary>
        /// Sender is the status changed WebBrowserEx, data is url txt. 
        /// </summary>
        public event EventHandler<CommonEventArgs> URLTextChanged;

        /// <summary>
        /// Raises the StatusTextChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnURLTextChanged(CommonEventArgs e) {
            if (URLTextChanged != null) {
                URLTextChanged(this, e);
            }
        }
        /// <summary>
        /// Sender is the status changed WebBrowserEx, data is title string txt. 
        /// </summary>
        public event EventHandler<CommonEventArgs> TitleTextChanged;

        /// <summary>
        /// Raises the TitleTextChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTitleTextChanged(CommonEventArgs e) {
            if (TitleTextChanged != null) {
                TitleTextChanged(this, e);
            }
        }
        /// <summary>
        /// Sender is the null, data is current take effect WebBrowserEx, that will cause outer 
        /// status bar changed. 
        /// </summary>
        public event EventHandler<CommonEventArgs> ToolbarStatusChanged;

        /// <summary>
        /// Raises the ToolbarStatusChanged event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnToolbarStatusChanged(CommonEventArgs e) {
            if (ToolbarStatusChanged != null) {
                ToolbarStatusChanged(this, e);
            }
        }
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
        public ScriptPageManager() {
        }
        #region common methods 
        /// <summary>
        /// Set the TabControlEx root of all script pages 
        /// </summary>
        /// <param name="ctrlRoot"></param>
        public void setTabContrlRoot(TabControlEx ctrlRoot) {
            this.tabCtrlRoot = ctrlRoot;
        }
        /// <summary>
        /// Get the TabPageEx of the current browser or null if errors 
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        private TabPageEx getTabPageByBrowser(WebBrowserEx browser) {
            if (browser != null) {
                foreach (TabPageEx page in tabCtrlRoot.PageStack) {
                    if (page.WebBrowser == browser) {
                        return page;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Create a TabPage with the script Root. 
        /// If sroot != null, it will create a new tab page with ScriptPage in side.
        /// If sroot == null, it will create a new tab page with WebBrowserEx in side. 
        /// </summary>
        /// <param name="sroot"></param>
        /// <returns></returns>
        public TabPageEx createNewPage(ScriptRoot sroot) {
            TabPageEx pageEx = this.tabCtrlRoot.addNewPage(true, null);
            if (sroot != null) {
                ScriptPage spage = new ScriptPage();
                spage.SRoot = sroot;
                spage.EngineStatusChanged += new EventHandler<CommonCodeArgs>(spage_EngineStatusChanged);
                spage.Dock = System.Windows.Forms.DockStyle.Fill;
                // remove web browser from tab page 
                pageEx.Page.Controls.Remove(pageEx.WebBrowser);
                // add the web browser into script tab page
                spage.WebBrowser = pageEx.WebBrowser;
                // add script page into root tab page.
                pageEx.Page.Controls.Add(spage);

                this.activeScriptPage = spage;
            }

            this.activeBrowser = pageEx.WebBrowser;

            return pageEx;
        }

        void spage_EngineStatusChanged(object sender, CommonCodeArgs e) {
            OnEngineStatusChanged(e);
        }
        /// <summary>
        /// Create a TabPage only with the default browser control. 
        /// </summary>
        /// <returns></returns>
        public TabPageEx createNewPage() {
            TabPageEx pageEx = this.tabCtrlRoot.addNewPage(true, null);
            this.activeBrowser = pageEx.WebBrowser;
            this.activeScriptPage = null;

            return pageEx;
        }

        internal void initBrowserEvtHanders(WebBrowserEx browser) {
            if (browser != null) {
                browser.ScriptErrorsSuppressed = true;
                // initial event for browser 
                browser.Downloading += new System.EventHandler(this.webBrowserEx1_Downloading);
                browser.DownloadComplete += new System.EventHandler(this.webBrowserEx1_DownloadComplete);
                browser.CanGoBackChanged += new System.EventHandler(this.webBrowserEx1_CanGoBackChanged);
                browser.CanGoForwardChanged += new System.EventHandler(this.webBrowserEx1_CanGoForwardChanged);
                browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserEx1_DocumentCompleted);
                browser.DocumentTitleChanged += new System.EventHandler(this.webBrowserEx1_DocumentTitleChanged);
                browser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserEx1_Navigated);
                browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserEx1_Navigating);
                browser.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowserEx1_NewWindow);
                browser.StartNewWindow += new EventHandler<BrowserExtendedNavigatingEventArgs>(browser_StartNewWindow);
                browser.StatusTextChanged += new System.EventHandler(this.webBrowserEx1_StatusTextChanged);
            }
        }

        #endregion common methods 
        #region browser event handler
        void webBrowserEx1_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e) {
            if (e.Url == null || Constants.URL_BLANK.Equals(e.Url.OriginalString)) {
                // filter blank frame
                return;
            }
            if (sender is WebBrowserEx) {
                TabPageEx page = getTabPageByBrowser(sender as WebBrowserEx);
                if (page != null && !page.Header.isLoadingImg()) {
                    page.Header.setLoadingImg();
                }
                Log.println_brw("Navigating.             url = " + e.Url + ", browser url=" + (sender as WebBrowserEx).Url + ", frame = " + e.TargetFrameName + ", page = " + page + ", loadingImg = " + page.Header.isLoadingImg());
            }            
        }       
        private void webBrowserEx1_Navigated(object sender, WebBrowserNavigatedEventArgs e) {
            if (sender is WebBrowserEx) {
                WebBrowserEx browser = sender as WebBrowserEx;
                string url = browser.Url.ToString();
                OnURLTextChanged(new CommonEventArgs(browser,url));                
            }            
        }

        void browser_StartNewWindow(object sender, BrowserExtendedNavigatingEventArgs e) {
            WebBrowserEx browser = sender as WebBrowserEx;
            // Allow a popup when there is no information available or when the Ctrl key is pressed
            bool allowPopup = (e.NavigationContext == UrlContext.None) || ((e.NavigationContext & UrlContext.OverrideKey) == UrlContext.OverrideKey);

            if (!allowPopup) {
                // Give None, Low & Medium still a chance.
                switch (PopupBlockerFilterLevel.Medium/*SettingsHelper.Current.FilterLevel*/) {
                    //case PopupBlockerFilterLevel.None:
                    //    allowPopup = true;
                    //    break;
                    //case PopupBlockerFilterLevel.Low:
                    //    // See if this is a secure site
                    //    if (browser.EncryptionLevel != WebBrowserEncryptionLevel.Insecure) {
                    //        allowPopup = true;
                    //    } else {
                    //        // Not a secure site, handle this like the medium filter
                    //        goto case PopupBlockerFilterLevel.Medium;
                    //    }
                    //    break;
                    case PopupBlockerFilterLevel.Medium:
                        // This is the most difficult one.
                        // Only when the user first inited and the new window is user inited
                        if ((e.NavigationContext & UrlContext.UserFirstInited) == UrlContext.UserFirstInited && (e.NavigationContext & UrlContext.UserInited) == UrlContext.UserInited) {
                            allowPopup = true;
                        }
                        break;
                }
            }
            if (allowPopup) {
                if (this.ActiveScriptPage != null) {
                    // When do script logging, make sure all the actions are occurred in the same browser 
                    if (browser != null && browser.Document != null && browser.Document.ActiveElement != null) {
                        e.Cancel = true;
                        HtmlElement elem = browser.Document.ActiveElement;
                        string url = elem.GetAttribute("href");
                        if (url != null && url.Length > 0) {
                            elem.SetAttribute("target", "_self");
                            browser.Navigate(url);
                        }
                    }
                } else { // for normal page, it can open links in new window. 
                    // Check wheter it's a HTML dialog box. If so, allow the popup but do not open a new tab
                    if (!((e.NavigationContext & UrlContext.HtmlDialog) == UrlContext.HtmlDialog)) {
                        this.createNewPage();
                        WebBrowserEx wbe = this.ActiveBrowser;
                        // The (in)famous application object
                        e.AutomationObject = wbe.Application;
                    } else {
                        // Here you could notify the user that the pop-up was blocked
                        e.Cancel = true;
                    }
                }
            }
        }
        
        void webBrowserEx1_NewWindow(object sender, System.ComponentModel.CancelEventArgs e) {
            if (sender is WebBrowserEx) {
                WebBrowserEx browser = sender as WebBrowserEx;
                Log.println_brw("new window.             e.cancel = " + e.Cancel);
                e.Cancel = true;
            }
        }
        private void webBrowserEx1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            WebBrowserEx browser = sender as WebBrowserEx;
            if (browser.IsBusy || browser.ReadyState != WebBrowserReadyState.Complete) {
                Log.println_brw("document completed in process. url = " + e.Url);
                return;
            }
            TabPageEx page = getTabPageByBrowser(browser);
            if (page != null && page.Header.isLoadingImg()) {
                page.Header.setBrowserImg();
            }
            Log.println_brw("document completed ................................ e. url = " + e.Url);
        }
        private void webBrowserEx1_DownloadComplete(object sender, EventArgs e) {
            WebBrowserEx browser = sender as WebBrowserEx;
            Log.println_brw("download complete,      url= " + browser.Url);
        }
        private void webBrowserEx1_Downloading(object sender, EventArgs e) {
            WebBrowserEx browser = sender as WebBrowserEx;
            Log.println_brw("downloading...,         url=" + browser.Url);
        }
        void webBrowserEx1_StatusTextChanged(object sender, System.EventArgs e) {
            WebBrowserEx browser = sender as WebBrowserEx;
            if (this.ActiveBrowser == browser) {
                string status = string.Empty;
                // Add the try catch is used to handle the exceptions like this: 
                // {"Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))"}
                // at System.Windows.Forms.UnsafeNativeMethods.IHTMLLocation.GetHref()
                // at System.Windows.Forms.WebBrowser.get_Document()
                // at WebMaster.browser.ScriptPageManager.webBrowserEx1_DocumentTitleChanged(Object sender, EventArgs e) in D:\mywork\WebMasterAll\solution\WebMaster\browser\ScriptPageManager.cs:line 219
                // at System.Windows.Forms.WebBrowser.OnDocumentTitleChanged(EventArgs e)
                // at System.Windows.Forms.WebBrowser.WebBrowserEvent.TitleChange(String text)
                try {
                    status = browser.StatusText;
                } catch (UnauthorizedAccessException) {
                    return;
                }
                // send the event that updates the status bar text
                OnStatusTextChanged(new CommonEventArgs(browser,status));
            }
            // Log.println_brw("Status Text Changed, text= " + browser.StatusText);
        }
        void webBrowserEx1_CanGoForwardChanged(object sender, System.EventArgs e) {
            Log.println_brw("Can go forward changed ");
            WebBrowserEx browser = sender as WebBrowserEx;
            updateBrowserButonsStatus(browser);
        }
        void webBrowserEx1_CanGoBackChanged(object sender, System.EventArgs e) {
            Log.println_brw("Can go forward changed ");
            WebBrowserEx browser = sender as WebBrowserEx;
            updateBrowserButonsStatus(browser);
        }
        void webBrowserEx1_DocumentTitleChanged(object sender, System.EventArgs e) {
            WebBrowserEx browser = sender as WebBrowserEx;
            string title = string.Empty;
            if (this.ActiveBrowser == browser) {                
                try {
                    title = browser.DocumentTitle;
                } catch (UnauthorizedAccessException) {
                    return;
                }
                OnTitleTextChanged(new CommonEventArgs(browser,title));           
            }
            TabPageEx page = this.getTabPageByBrowser(browser);
            if (page != null) {
                page.Header.Title = browser.DocumentTitle;
            }
            // Log.println_brw("Document title changed, title = " + browser.Document.Title + ", url = " + browser.Url);
        }        
        internal void updateBrowserButonsStatus(WebBrowserEx browser) {
            if (browser != null) {
                OnToolbarStatusChanged(new CommonEventArgs(null, browser));
            }
        }
        #endregion browser event handler
        
    }
}
