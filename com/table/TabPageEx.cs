using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.ui.browser;

namespace WebMaster.com.table
{
    public class TabPageEx
    {
        private TabHeader _tab = null;
        /// <summary>
        /// tab header 
        /// </summary>
        public TabHeader Header {
            get { return _tab; }
            set { _tab = value; }
        }

        private Panel _page = null;
        /// <summary>
        /// page area 
        /// </summary>
        public Panel Page {
            get { return _page; }
            set { _page = value; }
        }
        private WebBrowserEx webBrowser = null;
        /// <summary>
        /// Remove current WebBrowser and add the new WebBrowser into page if have. 
        /// </summary>
        public WebBrowserEx WebBrowser {
            get { return webBrowser; }
            set {
                if (webBrowser != null) {
                    if (!webBrowser.Equals(value)) {
                        webBrowser.Dispose();
                    } else {
                        return;
                    }
                } 
                webBrowser = value;
                Page.Controls.Add(webBrowser);
                Page.Invalidate();
            }
        } 
    }

    public class TabPageActiveEventArgs : EventArgs
    {
        private TabPageEx _preActPage = null;
        /// <summary>
        /// pre active page 
        /// </summary>
        public TabPageEx PreActPage {
            get { return _preActPage; }
            set { _preActPage = value; }
        }

        private TabPageEx _activePage = null;
        /// <summary>
        /// current active page 
        /// </summary>
        public TabPageEx ActivePage {
            get { return _activePage; }
            set { _activePage = value; }
        }

        public TabPageActiveEventArgs(TabPageEx prePage, TabPageEx activePage) {
            this._preActPage = prePage;
            this._activePage = activePage;
        }
    }

    public class TabHeaderEventArgs : EventArgs
    {
        private TabHeader _header = null;

        public TabHeader Header {
            get { return _header; }
        }
        public TabHeaderEventArgs(TabHeader header) {
            this._header = header;
        }
    }
}
