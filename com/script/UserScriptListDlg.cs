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
    public partial class UserScriptListDlg : Form
    {
        #region variables 
        private BigModel bigmodel = null;
        /// <summary>
        /// only take effect if isScript == false
        /// </summary>
        internal BigModel Bigmodel {
            get { return bigmodel; }
            set { bigmodel = value; }
        }
        private ScriptRoot sroot = null;
        /// <summary>
        /// only take effect if isScript == true 
        /// </summary>
        public ScriptRoot SRoot {
            get { return sroot; }
            set { sroot = value; }
        }
        /// <summary>
        /// whether the loaded model is script model for release mode or BigModel for edit mode. 
        /// </summary>
        private bool isScript = true;
        /// <summary>
        /// current selected ListViewItem 
        /// </summary>
        private ListViewItem selectedLVI = null;

        private UserProfile user = null;
        #endregion variables 
        public UserScriptListDlg() {
            InitializeComponent();
        }
        
        public DialogResult showScriptListDlg(IWin32Window handler, bool isScript, UserProfile user) {
            if (user == null) {
                return DialogResult.Cancel;
            }
            this.isScript = isScript;
            this.user = user;
            
            return ShowDialog(handler);
        }

        private void lv_myscripts_MouseDown(object sender, MouseEventArgs e) {
            selectedLVI = this.lv_myscripts.GetItemAt(e.X, e.Y);
            if (selectedLVI != null) {
                this.btn_OK.Enabled = true;
            } else {
                this.btn_OK.Enabled = false;
            }
        }
        private void lv_myscripts_MouseDoubleClick(object sender, MouseEventArgs e) {
            selectedLVI = this.lv_myscripts.GetItemAt(e.X, e.Y);
            performOKClicked();
        }

        private void lv_bookedscript_MouseDown(object sender, MouseEventArgs e) {
            selectedLVI = this.lv_bookedscript.GetItemAt(e.X, e.Y);
            if (selectedLVI != null) {
                this.btn_OK.Enabled = true;
            } else {
                this.btn_OK.Enabled = false;
            }
        }
        private void lv_bookedscript_MouseDoubleClick(object sender, MouseEventArgs e) {
            selectedLVI = this.lv_bookedscript.GetItemAt(e.X, e.Y);
            performOKClicked();
        }

        private void showErrMsg(int errCode) {
            //throw new NotImplementedException();
        }

        private void btn_OK_Click(object sender, EventArgs e) {
            performOKClicked();
        }

        private void performOKClicked() {
            if (selectedLVI == null) {
                return;
            }
            //if (isScript) {
            //    ModelManager.Instance.loadScript(lvi.Tag.ToString());
            //} else 
            {
                 this.Bigmodel = ModelManager.Instance.loadBigModel(selectedLVI.Tag.ToString());
                 this.SRoot = this.Bigmodel.SRoot;
            }
            int errCode = 0; // error code from the server response 
            if (errCode != 0) {
                showErrMsg(errCode);
                this.btn_OK.Enabled = false;
            } else {
                this.Close();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
        
    }
}
