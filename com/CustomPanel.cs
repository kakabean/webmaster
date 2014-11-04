using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WebMaster.com
{
    [System.Drawing.ToolboxBitmapAttribute(typeof(System.Windows.Forms.Panel))]
    public partial class CustomPanel : System.Windows.Forms.Panel
    {
        // Fields
        private System.Drawing.Color _BackColour1 = System.Drawing.SystemColors.Window;
        private System.Drawing.Color _BackColour2 = System.Drawing.SystemColors.Window;
        private LinearGradientMode _GradientMode = LinearGradientMode.None;
        private System.Windows.Forms.BorderStyle _BorderStyle = System.Windows.Forms.BorderStyle.None;
        private System.Drawing.Color _BorderColour = System.Drawing.SystemColors.WindowFrame;
        private int _BorderWidth = 1;
        private int _Curvature = 0;
        // Properties
        //   Shadow the Backcolor property so that the base class will still render with a transparent backcolor
        private CornerCurveMode _CurveMode = CornerCurveMode.All;
        private bool _isCloseBorder = true;
        /// <summary>
        /// whether close the border of the panel
        /// </summary>
        public bool CloseBorder {
            get { return _isCloseBorder; }
            set { _isCloseBorder = value; }
        }
        private bool _extendHeight = false;
        /// <summary>
        /// whether to extend the border height to +2 pixels when draw the border 
        /// </summary>
        public bool ExtendHeight {
            get { return _extendHeight; }
            set { _extendHeight = value; }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(System.Drawing.Color), "Window"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The primary background color used to display text and graphics in the control.")]
        public new System.Drawing.Color BackColor {
            get {
                return this._BackColour1;
            }
            set {
                this._BackColour1 = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(System.Drawing.Color), "Window"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The secondary background color used to paint the control.")]
        public System.Drawing.Color BackColor2 {
            get {
                return this._BackColour2;
            }
            set {
                this._BackColour2 = value;
                this.Invalidate();                
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(LinearGradientMode), "None"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The gradient direction used to paint the control.")]
        public LinearGradientMode GradientMode {
            get {
                return this._GradientMode;
            }
            set {
                this._GradientMode = value;                
                this.Invalidate();                
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(System.Windows.Forms.BorderStyle), "None"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The border style used to paint the control.")]
        public new System.Windows.Forms.BorderStyle BorderStyle {
            get {
                return this._BorderStyle;
            }
            set {
                this._BorderStyle = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(System.Drawing.Color), "WindowFrame"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The border color used to paint the control.")]
        public System.Drawing.Color BorderColor {
            get {
                return this._BorderColour;
            }
            set {
                this._BorderColour = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(int), "1"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The width of the border used to paint the control.")]
        public int BorderWidth {
            get {
                return this._BorderWidth;
            }
            set {
                this._BorderWidth = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(int), "1"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The radius of the curve used to paint the corners of the control.")]
        public int Curvature {
            get {
                return this._Curvature;
            }
            set {
                this._Curvature = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.DefaultValueAttribute(typeof(CornerCurveMode), "All"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("The style of the curves to be drawn on the control.")]
        public CornerCurveMode CurveMode {
            get {
                return this._CurveMode;
            }
            set {
                this._CurveMode = value;
                this.Invalidate();
            }
        }

        private int adjustedCurve {
            get {
                int curve = 0;
                if (!(this._CurveMode == CornerCurveMode.None)) {
                    if (this._Curvature > (this.ClientRectangle.Width / 2)) {
                        curve = DoubleToInt(this.ClientRectangle.Width / 2);
                    } else {
                        curve = this._Curvature;
                    }
                    if (curve > (this.ClientRectangle.Height / 2)) {
                        curve = DoubleToInt(this.ClientRectangle.Height / 2);
                    }
                }
                return curve;
            }
        }

        public CustomPanel()
            : base() {
            this.SetDefaultControlStyles();
            this.customInitialisation();
        }

        private void SetDefaultControlStyles() {
            this.SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, false);
            this.SetStyle(System.Windows.Forms.ControlStyles.ResizeRedraw, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
        }

        private void customInitialisation() {
            this.SuspendLayout();
            base.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ResumeLayout(false);
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent) {
            base.OnPaintBackground(pevent);
            System.Drawing.Drawing2D.LinearGradientBrush filler = null;
            System.Drawing.Drawing2D.GraphicsPath graphPath = null;
            try {
                pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;                
                graphPath = this.GetPath();
                //	Create Gradient Brush (Cannot be width or height 0)                
                System.Drawing.Rectangle rect = this.ClientRectangle;
                if (this.ClientRectangle.Width == 0) {
                    rect.Width += 1;
                }
                if (this.ClientRectangle.Height == 0) {
                    rect.Height += 1;
                }
                if (this._GradientMode == LinearGradientMode.None) {
                    filler = new System.Drawing.Drawing2D.LinearGradientBrush(rect, this._BackColour1, this._BackColour1, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                } else {
                    filler = new System.Drawing.Drawing2D.LinearGradientBrush(rect, this._BackColour1, this._BackColour2, ((System.Drawing.Drawing2D.LinearGradientMode)this._GradientMode));
                }
                pevent.Graphics.FillPath(filler, graphPath);
                filler.Dispose();
                if (this._BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle) {
                    System.Drawing.Pen borderPen = new System.Drawing.Pen(this._BorderColour, this._BorderWidth);                    
                    pevent.Graphics.DrawPath(borderPen, graphPath);
                    borderPen.Dispose();
                } else if (this._BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D) {
                    DrawBorder3D(pevent.Graphics, this.ClientRectangle);
                } else if (this._BorderStyle == System.Windows.Forms.BorderStyle.None) {
                }
                graphPath.Dispose();
            } finally {
                if (filler != null) {
                    filler.Dispose();
                }
                if (graphPath != null) {
                    graphPath.Dispose();
                }
            }
        }

        protected System.Drawing.Drawing2D.GraphicsPath GetPath() {
            System.Drawing.Drawing2D.GraphicsPath graphPath = new System.Drawing.Drawing2D.GraphicsPath();
            if (this._BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D) {
                graphPath.AddRectangle(this.ClientRectangle);
            } else {
                try {
                    int curve = 0;
                    System.Drawing.Rectangle rect = this.ClientRectangle;
                    int offset = 0;
                    if (this._BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle) {
                        if (this._BorderWidth > 1) {
                            offset = DoubleToInt(this.BorderWidth / 2);
                        }
                        curve = this.adjustedCurve;
                    } else if (this._BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D) {
                    } else if (this._BorderStyle == System.Windows.Forms.BorderStyle.None) {
                        curve = this.adjustedCurve;
                    }
                    if (curve == 0) {
                        graphPath.AddRectangle(System.Drawing.Rectangle.Inflate(rect, -offset, -offset));
                    } else {
                        int rectWidth = rect.Width - 1 - offset;
                        int rectHeight = rect.Height - 1 - offset;
                        rectHeight += ExtendHeight ? 2 : 0;
                        int curveWidth = 1;

                        if ((this._CurveMode & CornerCurveMode.BottomLeft) != 0) {
                            curveWidth = (curve * 2);
                        } else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(offset, rectHeight - curveWidth, curveWidth, curveWidth, 90, 90);
                        if ((this._CurveMode & CornerCurveMode.TopLeft) != 0) {
                            curveWidth = (curve * 2);
                        } else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(offset, offset, curveWidth, curveWidth, 180, 90);
                        if ((this._CurveMode & CornerCurveMode.TopRight) != 0) {
                            curveWidth = (curve * 2);
                        } else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(rectWidth - curveWidth, offset, curveWidth, curveWidth, 270, 90);
                        if ((this._CurveMode & CornerCurveMode.BottomRight) != 0) {
                            curveWidth = (curve * 2);
                        } else {
                            curveWidth = 1;
                        }
                        graphPath.AddArc(rectWidth - curveWidth, rectHeight - curveWidth, curveWidth, curveWidth, 0, 90);
                        
                        if (CloseBorder) {
                            graphPath.CloseFigure();
                        }
                    }
                } catch (System.Exception) {
                    graphPath.AddRectangle(this.ClientRectangle);
                }
            }
            return graphPath;
        }

        public static void DrawBorder3D(System.Drawing.Graphics graphics, System.Drawing.Rectangle rectangle) {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            graphics.DrawLine(System.Drawing.SystemPens.ControlDark, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Y);
            graphics.DrawLine(System.Drawing.SystemPens.ControlDark, rectangle.X, rectangle.Y, rectangle.X, rectangle.Height - 1);
            graphics.DrawLine(System.Drawing.SystemPens.ControlDarkDark, rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 1, rectangle.Y + 1);
            graphics.DrawLine(System.Drawing.SystemPens.ControlDarkDark, rectangle.X + 1, rectangle.Y + 1, rectangle.X + 1, rectangle.Height - 1);
            graphics.DrawLine(System.Drawing.SystemPens.ControlLight, rectangle.X + 1, rectangle.Height - 2, rectangle.Width - 2, rectangle.Height - 2);
            graphics.DrawLine(System.Drawing.SystemPens.ControlLight, rectangle.Width - 2, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2);
            graphics.DrawLine(System.Drawing.SystemPens.ControlLightLight, rectangle.X, rectangle.Height - 1, rectangle.Width - 1, rectangle.Height - 1);
            graphics.DrawLine(System.Drawing.SystemPens.ControlLightLight, rectangle.Width - 1, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
        }

        public static int DoubleToInt(double value) {
            return System.Decimal.ToInt32(System.Decimal.Floor(System.Decimal.Parse((value).ToString())));
        }
    }

    [FlagsAttribute()]
    public enum CornerCurveMode
    {

        None = 0,
        TopLeft = 1,
        TopRight = 2,
        TopLeft_TopRight = 3,
        BottomLeft = 4,
        TopLeft_BottomLeft = 5,
        TopRight_BottomLeft = 6,
        TopLeft_TopRight_BottomLeft = 7,
        BottomRight = 8,
        BottomRight_TopLeft = 9,
        BottomRight_TopRight = 10,
        BottomRight_TopLeft_TopRight = 11,
        BottomRight_BottomLeft = 12,
        BottomRight_TopLeft_BottomLeft = 13,
        BottomRight_TopRight_BottomLeft = 14,
        All = 15

    }

    public enum LinearGradientMode
    {
        Horizontal = 0,
        Vertical = 1,
        ForwardDiagonal = 2,
        BackwardDiagonal = 3,
        None = 4
    }
}
