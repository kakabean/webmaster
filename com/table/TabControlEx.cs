using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.ui.browser;
using WebMaster.lib;

namespace WebMaster.com.table
{
    public partial class TabControlEx : UserControl
    {
        private string Tab_Header_New = null;
        public TabControlEx() {
            InitializeComponent();
            this.createAddButton();            
//            this.addNewPage(true,Tab_Header_New);
        }

        #region event area
        /// <summary>
        /// raise when a TabPageEx actived
        /// </summary>
        public event System.EventHandler<TabPageActiveEventArgs> ActiveTabPage;

        protected virtual void OnActiveTabPage(TabPageActiveEventArgs e) {
            EventHandler<TabPageActiveEventArgs> activeTabPage = ActiveTabPage;
            if (activeTabPage != null) {
                activeTabPage(this, e);
            }
        }
        /// <summary>
        /// raise just after a new TabPageEx created and actived 
        /// </summary>
        public event EventHandler<TabPageActiveEventArgs> NewTabPage;

        protected virtual void OnNewTabPage(TabPageActiveEventArgs e) {
            EventHandler<TabPageActiveEventArgs> newTabPage = NewTabPage;
            if (newTabPage != null) {
                newTabPage(this, e);
            }
        }
        public event EventHandler<TabPageActiveEventArgs> CloseTabPage;

        protected virtual void OnCloseTabPage(TabPageActiveEventArgs e) {
            EventHandler<TabPageActiveEventArgs> closeTabPage = CloseTabPage;
            if (closeTabPage != null) {
                closeTabPage(this, e);
            }
        }

        public void raiseTabPageActiveEvt(TabPageEx preActpage, TabPageEx activePage) {
            if (activePage != null) {
                TabPageActiveEventArgs evt = new TabPageActiveEventArgs(preActpage, activePage);
                OnActiveTabPage(evt);
            }
        }
        public void raiseTabPageNewEvt(TabPageEx preActivePage, TabPageEx newPage) {
            if (newPage != null) {
                TabPageActiveEventArgs evt = new TabPageActiveEventArgs(preActivePage, newPage);
                OnNewTabPage(evt);
            }
        }
        public void raiseTabPageCloseEvt(TabPageEx prePage, TabPageEx closePage) {
            if (closePage != null) {
                TabPageActiveEventArgs evt = new TabPageActiveEventArgs(prePage, closePage);
                OnCloseTabPage(evt);
            }
        }
        #endregion
        #region data fields
        private Stack<TabPageEx> _pageStack = new Stack<TabPageEx>();
        /// <summary>
        /// user page collections, the sequence is the active order 
        /// the stack top is the current active page 
        /// </summary>
        public Stack<TabPageEx> PageStack {
            get { return _pageStack; }
        }
        private TabHeader addBtn = null;

        public TabHeader AddBtn {
            get { return addBtn; }
        }
        /// <summary>
        /// default tab header width 
        /// </summary>
        public static readonly int DEFAULT_HEADER_WIDTH = 240;
        /// <summary>
        /// default tab header height
        /// </summary>
        public static readonly int DEFAULT_HEADER_HEIGHT = 24;
        /// <summary>
        /// min width of the tab button 
        /// </summary>
        public static readonly int MIN_WIDTH = 20;
        /// <summary>
        /// default add button width 
        /// </summary>
        public static readonly int ADD_BUTTON_WIDTH = 22;
        public static readonly int ADD_BTN_ICON_TOP = 3;
        #endregion 
        #region properties        
        private int hHeight = 28;
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "28"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("height of the tab header, it will take effect when the style is horizontal")]
        public int HeaderHeight {
            get { return hHeight; }
            set {
                if (value != hHeight) {
                    this.headerBar.Height = value;
                    hHeight = value;
                    this.Invalidate();
                }
            }
        }        
        [System.ComponentModel.DefaultValueAttribute(typeof(System.Drawing.Color), "Window"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The primary background color used to display text and graphics in the header bar.")]
        public Color barBGColor1 {
            get { return this.headerBar.BackColor; }
            set {
                if (!value.Equals(this.headerBar.BackColor)) {
                    this.headerBar.BackColor = value;
                    this.Invalidate();
                }
            }
        }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.Drawing.Color), "Window"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The secondary background color for the header bar")]
        public Color barBGColor2 {
            get { return this.headerBar.BackColor2; }
            set {
                if (!value.Equals(this.headerBar.BackColor2)) {
                    this.headerBar.BackColor2 = value;
                    this.Invalidate();
                }
            }
        }
        [System.ComponentModel.DefaultValueAttribute(typeof(LinearGradientMode), "None"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The gradient direction used to paint the header bar.")]
        public LinearGradientMode GradientMode {
            get { return this.headerBar.GradientMode; }
            set {
                if(value!=this.headerBar.GradientMode){
                    this.headerBar.GradientMode = value;
                    this.Invalidate();
                }
            }
        }        
        #endregion
        #region handle tab header
        /// <summary>
        /// add TabHeader on the header bar
        /// </summary>
        /// <param name="header"></param>
        internal void addTabHeader(TabHeader header) {
            if (header != null) {
                if (AddBtn == null) {
                    addBtn = createAddButton();
                    this.headerBar.Controls.Add(AddBtn);
                }
                this.headerBar.Controls.Remove(AddBtn);
                this.headerBar.Controls.Add(header);
                this.headerBar.Controls.Add(AddBtn);

                updateTabSize();
            }
        }
        /// <summary>
        /// remove TabHeader on the header bar
        /// </summary>
        /// <param name="header"></param>
        public void removeTabHeader(TabHeader header) {
            if (header != null) {
                this.headerBar.Controls.Remove(header);
                updateTabSize();
            }
        }
        private void updateTabSize() {
            int cnt = this.headerBar.Controls.Count - 1;
            int delta = DEFAULT_HEADER_WIDTH;
            int tw = this.headerBar.Width - ADD_BUTTON_WIDTH -this.headerBar.BorderWidth*2-2;

            if (cnt * DEFAULT_HEADER_WIDTH > tw) {
                delta = tw / cnt;
            }

            int td = tw - delta * cnt;
            int dd = 0;
            if (td > 0 && td<delta) {
                dd = td / cnt + 1;
            }
            if (delta + dd > DEFAULT_HEADER_WIDTH) {
                dd = 0;
            }

            //Console.WriteLine("dd=" + dd+", bar width="+this.headerBar.Width);
            int i = 0;
            int tmpw = 2;
            foreach (TabHeader th in this.headerBar.Controls) {
                if(!th.Equals(AddBtn)){                                        
                    th.Width = delta;
                }
                if (dd * i < td) {                    
                    th.Size = new Size(th.Width + dd, th.Height);                    
                }
                th.Location = new Point(tmpw, th.Location.Y);
                tmpw += th.Width;

                //Console.WriteLine("point=" + th.Location + " , size=" + th.Size);

                i++;
            }
        }
        /// <summary>
        /// active header, and deactive pre-active header if not null 
        /// </summary>
        /// <param name="preHeader"></param>
        /// <param name="header"></param>
        private void activeTabHeader(TabHeader preHeader, TabHeader header) {
            if (preHeader != null) {
                preHeader.deActiveBorder();
                preHeader.BackColor = SystemColors.Control;
                preHeader.BackColor2 = SystemColors.ButtonFace;
            } else {
                header.Closable = false;
            }
            if (header != null) {
                header.activeBorder();
                header.BackColor = SystemColors.ControlLight;
                header.BackColor2 = SystemColors.Window;
            }
        }
        private void updateEvtHandler(TabHeader header) {
            if (header != null) {
                header.Active += new EventHandler<TabHeaderEventArgs>(tabHeader_Active);
                header.DeActive += new EventHandler<TabHeaderEventArgs>(tabHeader_DeActive);
                header.Close += new EventHandler<TabHeaderEventArgs>(tabHeader_Close);
                header.CMouseOver += new EventHandler<TabHeaderEventArgs>(tabHeader_MouseOver);
            }
        }
        void tabHeader_MouseOver(object sender, TabHeaderEventArgs e) {
            TabPageEx page = getActivePage();
            TabHeader header = sender as TabHeader;
            // hidden close icon when just one page left 
            if (this.PageStack.Count == 1) {
                if (page != null && page.Header != null && page.Header.Closable == true) {
                    page.Header.Closable = false;
                }
            } else {
                if (page != null && page.Header != null && page.Header.Closable == false) {
                    page.Header.Closable = true;
                }
                if (header.Closable == false) {
                    header.Closable = true;
                }
            }
            if (!header.Equals(page.Header)) {
                header.deActiveBorder();
                header.BackColor = SystemColors.Highlight;//SystemColors.ControlLight;
            }
        }
        void tabHeader_Close(object sender, TabHeaderEventArgs e) {
            TabHeader header = sender as TabHeader;
            TabPageEx page = getTabPage(header);
            closeTab(page);
        }

        /// <summary>
        /// Close the tab page 
        /// </summary>
        /// <param name="page"></param>
        public void closeTab(TabPageEx page) {
            if (page == null) {
                return;
            }
            bool isActive = isActivePage(page.Header);
            if (page != null) {
                this.updateStackPageClose(page);
                if (page != null) {
                    closePage(page);
                }

                if (PageStack.Count == 1) {
                    TabPageEx tp = this.PageStack.Peek();
                    tp.Header.Closable = false;
                }

                if (isActive) {
                    TabPageEx prePage = this.PageStack.Count > 0 ? this.PageStack.Peek() : null;
                    activePageUI(null, prePage);
                    this.raiseTabPageActiveEvt(null, prePage);
                }

                this.raiseTabPageCloseEvt(null, page);
            }
        }

        void tabHeader_DeActive(object sender, TabHeaderEventArgs e) {
            TabPageEx page = getActivePage();
            TabHeader header = sender as TabHeader;
            if (!header.Equals(page.Header)) {
                header.deActiveBorder();
                header.BackColor = SystemColors.Control;
            }
        }
        /// <summary>
        /// active page and deactive previous page if have 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabHeader_Active(object sender, TabHeaderEventArgs e) {
            TabPageEx apage = getTabPage(e.Header);
            setPageActive(apage);
        }
        /// <summary>
        /// make sure aPage is the new Active page. 
        /// </summary>
        /// <param name="apage"></param>
        public void setPageActive(TabPageEx apage) {
            if (apage != null) {
                updateStackPageActive(apage);
            }
            TabPageEx page = getActivePage();
            TabPageEx prePage = getPreviousPage();
            activePageUI(prePage, page);

            this.raiseTabPageActiveEvt(prePage, page);
        }
        #endregion 
        #region methods
        /// <summary>
        /// create the default add button if have 
        /// </summary>
        /// <returns></returns>Brow
        private TabHeader createAddButton() {
            TabHeader th = new TabHeader();
            th.Size = new Size(ADD_BUTTON_WIDTH, ADD_BUTTON_WIDTH);
            th.Icon = global::com.Properties.Resources.add16;
            th.Closable = false;
            th.Title = "";
            th.IconTop = ADD_BTN_ICON_TOP;
            th.Click += new System.EventHandler(this.addButton_Click);
            th.Location = new Point(0,7);
            th.BackColor = SystemColors.Control;
            th.BorderColor = SystemColors.ControlDark;
            th.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            th.PaddingLeft = ADD_BTN_ICON_TOP;
            th.PaddingRight = ADD_BTN_ICON_TOP;

            return th;
        }
        private void addButton_Click(object sender, EventArgs e) {
            addNewPage(true, Tab_Header_New);
        }
        /// <summary>
        /// add a new page for the tabControl and active it if necessary 
        /// <param name="needActive"></param>
        /// <param name="title"></param>
        /// </summary>
        public TabPageEx addNewPage(bool needActive, string title) {
            if (Tab_Header_New == null) {
                Tab_Header_New = UILangUtil.getMsg("tabpage.header.new");
            }
            if (title == null || title.Trim().Length == 0) {
                title = Tab_Header_New;
            }
            TabPageEx page = createPage(title);
            updateEvtHandler(page.Header);
            TabPageEx prePage = null;
            if (this.PageStack.Count > 0) {
                prePage = this.PageStack.Peek();
            }
            //push page into stack 
            this.PageStack.Push(page);
            addNewPage(prePage, page, needActive);

            return page;
        }
        /// <summary>
        /// handle NewTabPage event, hidden pre active page if have, 
        /// set page visible 
        /// </summary>
        /// <param name="prePage"></param>
        /// <param name="page"></param>
        /// <param name="needActive"></param>
        internal void addNewPage(TabPageEx prePage, TabPageEx page, bool needActive) {
            if (page == null) {
                return;
            }
            // add new TabButton
            this.addTabHeader(page.Header);
            // add new PageArea
            this.pageContainer.Controls.Add(page.Page);
            if (prePage == null) {
                page.Header.Closable = false;
            }
            if (this.PageStack.Count == 2) {
                prePage.Header.Closable = true;
            }

            this.raiseTabPageNewEvt(prePage, page);

            if (needActive || this.PageStack.Count == 1) {
                this.tabHeader_Active(page.Header, new TabHeaderEventArgs(page.Header));
            }
        }
        /// <summary>
        /// Update the page ui status to reflect the active status 
        /// </summary>
        /// <param name="prePage"></param>
        /// <param name="page"></param>
        internal void activePageUI(TabPageEx prePage, TabPageEx page) {
            if (page != null) {
                TabHeader tb = prePage == null ? null : prePage.Header;
                // active tab header. 
                this.activeTabHeader(tb, page.Header);
                // active page area 
                activePageArea(prePage, page);
            }
        }
        /// <summary>
        /// active page area 
        /// </summary>
        /// <param name="prePage"></param>
        /// <param name="page"></param>
        private void activePageArea(TabPageEx prePage, TabPageEx page) {
            if (prePage != null) {
                prePage.Page.Visible = false;
            }
            page.Page.Visible = true;
        }
        /// <summary>
        /// close the page form the TabControl, and dispose it
        /// </summary>
        /// <param name="tabPage"></param>
        private void closePage(TabPageEx tabPage) {
            TabHeader header = tabPage.Header;
            Panel pageArea = tabPage.Page;
            // remove tabButton from tab bar
            this.headerBar.Controls.Remove(header);
            this.updateTabSize();

            // remove page area from pageContainer
            this.pageContainer.Controls.Remove(pageArea);

            if (header != null && !header.IsDisposed) {
                header.Dispose();
            }
            if (pageArea != null && !pageArea.IsDisposed) {
                pageArea.Dispose();
            }
        }
        #endregion 
        #region util functions
        /// <summary>
        /// get TabPage by tabButton, return null if errors 
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        internal TabPageEx getTabPage(TabHeader header) {
            Stack<TabPageEx> ts = new Stack<TabPageEx>();
            TabPageEx page = null;
            bool find = false;
            while (this.PageStack.Count > 0) {
                page = this.PageStack.Pop();
                ts.Push(page);
                if (page.Header.Equals(header)) {
                    find = true;
                    break;
                }
            }
            while (ts.Count > 0) {
                this.PageStack.Push(ts.Pop());
            }

            return find ? page : null;
        }
        /// <summary>
        /// set the page into the stack top 
        /// </summary>
        /// <param name="page"></param>
        private void updateStackPageActive(TabPageEx page) {
            if (page != null && PageStack.Contains(page)) {
                if (!page.Equals(PageStack.Peek())) {
                    Stack<TabPageEx> tstack = new Stack<TabPageEx>();
                    while (PageStack.Count > 0) {
                        TabPageEx tp = PageStack.Pop();
                        if (tp.Equals(page)) {
                            break;
                        } else {
                            tstack.Push(tp);
                        }
                    }
                    while (tstack.Count > 0) {
                        this.PageStack.Push(tstack.Pop());
                    }

                    this.PageStack.Push(page);
                }
            }
        }
        /// <summary>
        /// remove the page in the page stack 
        /// </summary>
        /// <param name="page"></param>
        private void updateStackPageClose(TabPageEx page) {
            if (page != null && PageStack.Contains(page)) {
                this.updateStackPageActive(page);
                // remove the target pagpe
                this.PageStack.Pop();
            }
        }
        /// <summary>
        /// get the previous TabPage or null if just one page in the stack 
        /// </summary>
        /// <returns></returns>
        public TabPageEx getPreviousPage() {
            TabPageEx page = null;
            if (this.PageStack.Count > 1) {
                TabPageEx tp = this.PageStack.Pop();
                page = this.PageStack.Peek();
                this.PageStack.Push(tp);
            }

            return page;
        }
        /// <summary>
        /// get active page of null if errors 
        /// </summary>
        /// <returns></returns>
        public TabPageEx getActivePage() {
            TabPageEx page = null;
            if (this.PageStack.Count > 0) {
                page = this.PageStack.Peek();
            }

            return page;
        }
        public bool isActivePage(TabHeader tb) {
            if (this.PageStack.Count > 0 && this.PageStack.Peek().Header != null) {
                if (this.PageStack.Peek().Header.Equals(tb)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// create a new TabPage 
        /// </summary>
        /// <param name="text">tab text</param>
        /// <returns></returns>
        public TabPageEx createPage(string text) {
            TabPageEx page = new TabPageEx();
            page.Header = createTabHeader(text);
            page.Page = createPageArea();
            
            WebBrowserEx browser = new WebBrowserEx();
            browser.ScriptErrorsSuppressed = true;
            browser.Dock = DockStyle.Fill;
            page.WebBrowser = browser;

            return page;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TabHeader createTabHeader(string text) {
            TabHeader header = new TabHeader();

            header.Location = new System.Drawing.Point(2, 4);
            header.Margin = new System.Windows.Forms.Padding(2);
            header.Size = new System.Drawing.Size(TabControlEx.DEFAULT_HEADER_WIDTH, TabControlEx.DEFAULT_HEADER_HEIGHT);
            header.Title = text;

            header.BackColor = System.Drawing.SystemColors.Control;
            header.BackColor2 = System.Drawing.SystemColors.ButtonFace;
            header.GradientMode = LinearGradientMode.Vertical;
            header.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            header.BorderColor = System.Drawing.SystemColors.ControlDark;

            return header;
        }
        private Panel createPageArea() {
            Panel page = new Panel();
            page.Dock = System.Windows.Forms.DockStyle.Fill;
            page.Location = new System.Drawing.Point(0, 0);
            page.Padding = new Padding(0);
            
            return page;
        }
        #endregion        
    }

    public enum TabStyle { 
        HORIZONTAL,
        VERTICAL
    }
}
