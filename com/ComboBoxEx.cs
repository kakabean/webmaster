using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WebMaster.com
{
    public partial class ComboBoxEx : ComboBox
    {
        private bool isReadonly = false;
        /// <summary>
        /// It will only take effect when the DropDownStyle=DropDown.
        /// The background color will be Color.Control, and the text area will non-editable
        /// </summary>
        [System.ComponentModel.DefaultValueAttribute(typeof(bool), "false"), System.ComponentModel.CategoryAttribute("Appearance"), System.ComponentModel.DescriptionAttribute("Only take effect when DropDownStyle is DropDown, whether the text is readonly")]
        public bool Readonly {
            get { return isReadonly; }
            set {
                if (DropDownStyle == ComboBoxStyle.DropDown) {
                    this.BackColor = SystemColors.Control;
                    isReadonly = value;         
                }               
            }
        }        
        /// <summary>
        /// cache the display text, only take effect when ReadOnly==true
        /// </summary>
        private string displayText = string.Empty;

        public ComboBoxEx() {
            InitializeComponent();
        }

        public ComboBoxEx(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }
        #region override methods 
        protected override void OnSelectedIndexChanged(EventArgs e) {
            if (this.Readonly) {
                displayText = this.Text;
            }
            base.OnSelectedIndexChanged(e);
        }
        protected override void OnTextUpdate(EventArgs e) {
            if (this.Readonly) {
                if (this.Text != this.displayText) {
                    this.Text = this.displayText;
                }
            }
            base.OnTextUpdate(e);
        }     
        //protected override void OnTextChanged(EventArgs e) {
            
        //    base.OnTextChanged(e);
        //    Console.WriteLine("text changed. text = "+this.Text);
        //}
        //protected override void OnPaint(PaintEventArgs e) {
        //    base.OnPaint(e);
        //    Console.WriteLine("onPaint, text = "+this.Text);
        //}
        //protected override void OnValidated(EventArgs e) {
        //    Console.WriteLine("onValidated, text = " + this.Text);
        //    base.OnValidated(e);
        //}
        //protected override void OnValidating(CancelEventArgs e) {
        //    Console.WriteLine("onValidating, text = " + this.Text);
        //    base.OnValidating(e);
        //}
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
        //    Console.WriteLine("CmdKey , text = " + this.Text+", msg = "+msg.Msg+", lp = "+msg.LParam+", wp = "+msg.WParam);
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}
        #endregion override methods 
    }
}
