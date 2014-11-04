using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WebMaster.lib;

namespace WebMaster.com.table
{
    [System.Drawing.ToolboxBitmapAttribute(typeof(System.Windows.Forms.Panel))]
    public partial class TabHeader : CustomPanel
    {
        public TabHeader() {
            InitializeComponent();
            this.Curvature = 1;
        }

        public TabHeader(IContainer container) {
            container.Add(this);

            InitializeComponent();
            this.Curvature = 1;
        }
        #region properties
        private static readonly int iconWidth = 16;
        private static readonly int closeIconWidth = 12;
        private int iconTop = 5;

        public int IconTop {
            get { return iconTop; }
            set {
                if (value != iconTop) {
                    iconTop = value;
                    this.imgLabel.Location = new Point(paddingLeft, IconTop);
                }
            }
        }

        private Label imgLabel = null;

        //private Image icon = global::com.Properties.Resources.op_url16;
        private bool isLoadingIcon = false;
        private Image closeIconEnable = null;
        private Image closeIconDisable = null;
        /// <summary>
        /// icon of the header 
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(typeof(Image), null), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("the image of the tab header.")]
        public Image Icon{
            get { return this.imgLabel.Image; }
            set { 
                this.imgLabel.Image = value;
                this.Invalidate();
            }
        }
        private string title;
        /// <summary>
        /// tab header text 
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(typeof(string), ""), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("the text of the tab header.")]
        public string Title {
            get { return title; }
            set { 
                title = value;
                this.Invalidate();
            }
        }
        private bool closable = false;
        /// <summary>
        /// whether to show the closable icon 
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(typeof(bool), "false"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("whether to display the close button")]
        public bool Closable {
            get { return closable; }
            set {
                if (value != closable) {
                    closable = value;                    
                    this.Invalidate();
                }
            }
        }
        
        private int paddingLeft = 5;
        /// <summary>
        /// left padding of the image icon 
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "5"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("left padding of the icon image")]
        public int PaddingLeft {
            get { return paddingLeft; }
            set {
                if (value != paddingLeft) {
                    paddingLeft = value;
                    this.imgLabel.Location = new Point(paddingLeft, IconTop);
                    this.Invalidate();
                }
            }
        }
        private int paddingRight = 5;
        /// <summary>
        /// right padding of the closable icon 
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "5"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("right padding of the close icon")]
        public int PaddingRight {
            get { return paddingRight; }
            set {
                if (value != paddingRight) {
                    paddingRight = value;
                    this.Invalidate();
                }
            }
        }
        private bool isActive = false;
        #endregion 
        #region event area
        public event EventHandler<TabHeaderEventArgs> CMouseOver;

        protected virtual void OnCMouseOver(TabHeaderEventArgs e) {
            EventHandler<TabHeaderEventArgs> cMouseOver = CMouseOver;
            if (cMouseOver != null) {
                cMouseOver(this, e);
            }
        }
        public event EventHandler<TabHeaderEventArgs> DeActive;

        protected virtual void OnDeActive(TabHeaderEventArgs e) {
            EventHandler<TabHeaderEventArgs> deActive = DeActive;
            if (deActive != null) {
                deActive(this, e);
            }
        }
        public event EventHandler<TabHeaderEventArgs> Active;

        protected virtual void OnActive(TabHeaderEventArgs e) {
            EventHandler<TabHeaderEventArgs> active = Active;
            if (active != null) {
                active(this, e);
            }
        }
        public event EventHandler<TabHeaderEventArgs> Close;

        protected virtual void OnClose(TabHeaderEventArgs e) {
            EventHandler<TabHeaderEventArgs> close = Close;
            if (close != null) {
                close(this, e);
            }
        }
        public void raiseMouseOverEvt(EventArgs e) {
            TabHeaderEventArgs ce = new TabHeaderEventArgs(this);
            OnCMouseOver(ce);
        }
        public void raiseDeactiveEvt(EventArgs e) {
            TabHeaderEventArgs ce = new TabHeaderEventArgs(this);
            OnDeActive(ce);
        }
        public void raiseActiveEvt(EventArgs e) {
            TabHeaderEventArgs ce = new TabHeaderEventArgs(this);
            OnActive(ce);
        }
        public void raiseCloseEvt(EventArgs e) {
            TabHeaderEventArgs ce = new TabHeaderEventArgs(this);
            OnClose(ce);
        }
        #endregion 
        #region override method
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent) {
            base.OnPaintBackground(pevent);
            Graphics g = pevent.Graphics;            
            //if (this.imgLabel.Image == null) {
            //    this.imgLabel.Image = global::com.Properties.Resources.att16;
            //}
            //if (title == null || title.Length < 1) {
            //    title = "New Tab";
            //}
            // draw icon image 
            int ix = this.PaddingLeft;
            int iy = 1 + this.BorderWidth;            
            //if (this.Height > iconWidth) {
            //    iy = (this.Height - iconWidth) / 2 + this.BorderWidth;
            //}
            //g.DrawImage(icon, new Rectangle(ix, iy, iconWidth, iconWidth));
            // draw text
            int fx = iconWidth + ix+3;
            int fy = 0+this.BorderWidth;
            if (Font.Height < this.Height) {
                fy = (this.Height - Font.Height) / 2+this.BorderWidth;
            }            
            int w = this.Width - fx - closeIconWidth-PaddingRight-3;
            w = w > 0 ? w : 0;
            int h = this.Height - fy * 2;
            h = h > 0 ? h : 0;
            Rectangle rect = new Rectangle(fx,fy,w,h);
            TextRenderer.DrawText(pevent.Graphics, this.title, this.Font, rect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            // draw close icon 
            if (this.Closable) {
                if (closeIconDisable == null) {
                    closeIconDisable = global::com.Properties.Resources.close12dis;
                }
                if (closeIconEnable == null) {
                    closeIconEnable = global::com.Properties.Resources.close12en;
                }
                int cx = this.Width - this.BorderWidth - closeIconWidth-paddingRight;
                int cy = 0+this.BorderWidth;            
                if (this.Height > closeIconWidth) {
                    cy = (this.Height - closeIconWidth) / 2 + this.BorderWidth;
                }
                if (isActive) {
                    g.DrawImage(closeIconEnable,new Rectangle(cx,cy,closeIconWidth,closeIconWidth));
                } else { 
                    g.DrawImage(closeIconDisable,new Rectangle(cx,cy,closeIconWidth,closeIconWidth));
                }
            }
        }
        protected override void OnResize(EventArgs eventargs) {
            base.OnResize(eventargs);
        }
        //protected override void OnMouseHover(EventArgs e) {
        //    base.OnMouseHover(e);
        //    this.raiseMouseOverEvt(e);
        //}
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            this.raiseMouseOverEvt(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (isActive) {
                isActive = false;
                this.Refresh();
            }
            this.raiseDeactiveEvt(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (Closable) {
                int cx = this.Width - this.BorderWidth - closeIconWidth - 3;
                int cy = 0 + this.BorderWidth;
                if (this.Height > closeIconWidth) {
                    cy = (this.Height - closeIconWidth) / 2 + this.BorderWidth;
                }
                Rectangle rect = new Rectangle(cx - 1, cy - 1, closeIconWidth + 2, closeIconWidth + 2);
                if (rect.Contains(e.Location)) {                    
                    this.raiseCloseEvt(e);                    
                    return;
                }
            }
            this.raiseActiveEvt(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (Closable) {
                int cx = this.Width - this.BorderWidth - closeIconWidth - 3;
                int cy = 0 + this.BorderWidth;
                if (this.Height > closeIconWidth) {
                    cy = (this.Height - closeIconWidth) / 2 + this.BorderWidth;
                }
                Rectangle rect = new Rectangle(cx - 1, cy - 1, closeIconWidth + 2, closeIconWidth + 2);
                if (rect.Contains(e.Location)) {
                    if (!isActive) {
                        isActive = true;
                        this.Refresh();                        
                    }
                } else {
                    if (isActive) {
                        isActive = false;
                        this.Refresh();
                    }
                }
            }
        }

        void imgLabel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            this.OnMouseMove(e);
        }
        void imgLabel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
            this.OnMouseDown(e);
        }
        void imgLabel_Click(object sender, System.EventArgs e) {
            this.OnClick(e);
        }
        
        #endregion override method
   
        public void activeBorder() {
            if (this.ExtendHeight == false) {
                this.ExtendHeight = true;
                this.CloseBorder = false;
                this.Size = new Size(this.Width, this.Height + 2);
                this.Location = new Point(this.Location.X, this.Location.Y - 2);
            }
        }

        public void deActiveBorder() {
            if (this.ExtendHeight == true) {
                this.ExtendHeight = false;
                this.CloseBorder = true;
                this.Size = new Size(this.Width, this.Height - 2);
                this.Location = new Point(this.Location.X, this.Location.Y + 2);
            }
        }
        /// <summary>
        /// True, the header is show loading icon, false : it normal icon. 
        /// </summary>
        public bool isLoadingImg() {
            return isLoadingIcon;
        }
        /// <summary>
        /// Set the header area with a loading img. 
        /// </summary>
        public void setLoadingImg() {
            this.Icon = global::com.Properties.Resources.loading16;
            this.isLoadingIcon = true;
        }
        /// <summary>
        /// Set the header area with a browser image
        /// </summary>
        public void setBrowserImg() { 
            this.Icon = global::com.Properties.Resources.browser16;
            this.isLoadingIcon = false;
        }
    }
}
