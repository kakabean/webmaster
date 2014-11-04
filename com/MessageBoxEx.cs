using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebMaster.com
{
    public partial class MessageBoxEx : Form
    {
        private static MessageBoxEx instance = new MessageBoxEx();
        private string txt_OK = "OK";
        private string txt_Cancel = "Cancel";
        private string txt_Yes = "Yes";
        private string txt_No = "No";
        private MessageBoxEx() {
            InitializeComponent();
            txt_OK = UILangUtil.getMsg("dlg.btn.ok.txt");
            txt_Cancel = UILangUtil.getMsg("dlg.btn.cancel.txt");
            txt_Yes = UILangUtil.getMsg("dlg.btn.yes.txt");
            txt_No = UILangUtil.getMsg("dlg.btn.no.txt");
            //initUI(text,title,btns,icon);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler">Parent control that is used to adjust the dialog position</param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="btns"></param>
        /// <param name="icon"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static DialogResult showMsgDialog(IWin32Window handler,string text, string title, MessageBoxButtons btns, MessageBoxIcon icon,FormStartPosition pos) {
            instance.initUI(text,title,btns,icon);
            instance.StartPosition = pos;
            return instance.ShowDialog(handler);
        }
        private void initUI(string text,string title,MessageBoxButtons btns, MessageBoxIcon icon) {
            this.button1.Visible = false;
            this.button2.Visible = false;
            this.button3.Visible = false;

            if (btns == MessageBoxButtons.OK) {
                this.button1.Text = txt_OK;
                this.button1.Visible = true;
                this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;                
            }else if (btns == MessageBoxButtons.OKCancel) {
                this.button1.Text = txt_OK;
                this.button1.Visible = true;
                this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.button2.Text = txt_Cancel;                
                this.button2.Visible = true;
                this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            } else if(btns == MessageBoxButtons.YesNoCancel){                
                this.button1.Text = txt_Yes;
                this.button1.Visible = true;
                this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.button2.Text = txt_No;
                this.button2.Visible = true;
                this.button2.DialogResult = System.Windows.Forms.DialogResult.No;
                this.button3.Text = txt_Cancel;
                this.button3.Visible = true;
                this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            } else if (btns == MessageBoxButtons.YesNo) {
                this.button1.Text = txt_Yes;
                this.button1.Visible = true;
                this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.button2.Text = txt_No;
                this.button2.Visible = true;
                this.button2.DialogResult = System.Windows.Forms.DialogResult.No;                
            }
            if (icon == MessageBoxIcon.Warning) {
                this.panel1.BackgroundImage = SystemIcons.Warning.ToBitmap();
            }
            this.label1.Text = text;
            this.Text = title;
        }
                
    }
}
