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
    public partial class AboutDlg : Form
    {
        #region override 
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Enter || keyData == Keys.Escape) {
                this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion override 
        public AboutDlg() {
            InitializeComponent();
        }
    }
}
