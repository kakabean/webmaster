using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.com.script
{
    public partial class AddURLDialog : Form
    {
        private List<string> trustedURLs = null;

        private string url = "";
        /// <summary>
        /// returned URL 
        /// </summary>
        public string URL {
            get { return url; }
            //set { url = value; }
        }

        public AddURLDialog() {
            InitializeComponent();
            tb_url.Focus();
        }

        public DialogResult showAddURLDlg(IWin32Window handler, List<string> trustedURLs) {
            this.trustedURLs = trustedURLs;
            return ShowDialog(handler);
        }

        private void tb_url_TextChanged(object sender, EventArgs e) {
            string txt = tb_url.Text.Trim();
            lb_msg.Text = string.Empty;
            if (!(txt.StartsWith("http://") || txt.StartsWith("https://"))) {
                lb_msg.Text = UILangUtil.getMsg("dlg.addurl.err.text1");
            } else if (ModelManager.Instance.isTrustedURL(txt,trustedURLs)) {
                lb_msg.Text = UILangUtil.getMsg("dlg.addurl.err.text2");
            }
            if (lb_msg.Text.Length > 0) {
                url = string.Empty;
                btn_OK.Enabled = false;
            } else {
                url = txt;
                btn_OK.Enabled = true;
            }
        }
    }
}
