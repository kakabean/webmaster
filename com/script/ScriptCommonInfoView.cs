using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.com.script
{
    public partial class ScriptCommonInfoView : UserControl
    {
        private ScriptRoot _sroot = null;
        /// <summary>
        /// This is used to maintain the existed valid timeout value
        /// </summary>
        private int timeout = 0;

        private bool is_tb_timeout_updatedByHand = false;

        private bool isDevMode = false;
        /// <summary>
        /// whether need to validate the version when version text changed. 
        /// </summary>
        private bool needValidVersion = false;
        private bool needValidURL = false;

        public ScriptCommonInfoView() {
            InitializeComponent();
            disableView();
        }
        /// <summary>
        /// only in develop mode, the fields can be editable. 
        /// </summary>
        /// <param name="devMode"></param>
        public void setDevMode(bool devMode) {
            this.isDevMode = devMode;
        }
        public void setScriptRoot(ScriptRoot sr) {
            if (sr != _sroot || sr == null) {
                this._sroot = sr;
                updateView();
            }
        }
        #region events area
        /// <summary>
        /// data is the url TextBox object 
        /// </summary>
        public event EventHandler<CommonEventArgs> UpdateUrlTextBoxEvt;
        protected virtual void OnUpdateUrlTextBoxEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> updateUrlTextBoxEvt = UpdateUrlTextBoxEvt;
            if (updateUrlTextBoxEvt != null) {
                updateUrlTextBoxEvt(this, e);
            }
        }
        public void raiseUpdateUrlEvt(Object sender, TextBox urlTextbox) {
            if (urlTextbox != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, urlTextbox);
                OnUpdateUrlTextBoxEvt(evt);
            }
        }
        /// <summary>
        /// when the script common info updated, raise this event. sender is view, data is an object. 
        /// maybe script name
        /// </summary>
        public event EventHandler<CommonEventArgs> UpdateModelEvt;
        protected virtual void OnUpdateModelEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> updateModelEvt = UpdateModelEvt;
            if (updateModelEvt != null) {
                updateModelEvt(this, e);
            }
        }
        public void raiseUpdateModelEvt(Object sender, object data) {
            if (data != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, data);
                OnUpdateModelEvt(evt);
            }
        }
        #endregion events area 
        /// <summary>
        /// clean the view values 
        /// </summary>
        public void cleanView() {
            this.tb_name.Text = Constants.BLANK_TEXT;
            this.tb_des.Text = Constants.BLANK_TEXT;
            this.tb_contributors.Text = Constants.BLANK_TEXT;
            this.lb_author.Text = Constants.BLANK_TEXT;
            this.tb_version.Text = Constants.BLANK_TEXT;
            needValidURL = false;
            this.tb_url.Text = Constants.BLANK_TEXT;
            this.btn_Icon.Image = null;//should be a default image
            this.lv_url.Items.Clear();
            this.ckb_enableSec.Checked = false;
        }
        /// <summary>
        /// update the view with script root info, 
        /// if script root is null, disable view 
        /// </summary>
        public void updateView() {
            if (_sroot != null) {
                this.tb_name.Text = _sroot.Name;
                this.tb_des.Text = _sroot.Description;
                needValidURL = false;
                this.tb_url.Text = _sroot.TargetWebURL;
                this.btn_Icon.Image = ModelManager.Instance.getImage(_sroot.IconPath);
                this.tb_timeout.Text = _sroot.Timeout.ToString();
                this.needValidVersion = false;
                this.tb_version.Text = _sroot.Version;
                this.lb_author.Text = _sroot.Author;
                this.tb_contributors.Text = getContributorDisplayText();
                this.ckb_enableSec.Checked = _sroot.UrlsLocked;
                this.updateTrustedURLListView();
                enableView();
            } else {
                cleanView();
                disableView();
            }
        }

        private string getContributorDisplayText() {
            if (_sroot != null) {
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (string str in _sroot.Contributors) {
                    sb.Append(str);
                    i++;
                    if (i < _sroot.Contributors.Count) {
                        sb.Append(" , ");
                    }                    
                }
                return sb.ToString();
            }
            return string.Empty;
        }
        private void disableView() {
            this.tb_name.Enabled = false;
            this.tb_des.Enabled = false;
            this.tb_url.Enabled = false;
            this.btn_Icon.Enabled = false;
            this.btn_getUrl.Enabled = false;
            this.btn_Icon.Enabled = false;
            this.tb_version.Enabled = false;
            this.btn_genVer.Enabled = false;
            this.tb_timeout.Enabled = false;
            if (isDevMode) {
                this.btn_genVer.Visible = true;
                this.btn_addURL.Visible = true;
                this.btn_rmURL.Visible = true ;
            } else {
                this.btn_genVer.Visible = false;
                this.btn_addURL.Visible = false;
                this.btn_rmURL.Visible = false;
            }
            this.ckb_enableSec.Enabled = false;
            this.btn_addURL.Enabled = false;
            this.btn_rmURL.Enabled = false;
        }
        private void enableView() {
            this.ckb_enableSec.Enabled = true;
            this.tb_timeout.Enabled = true;
            if (isDevMode) {
                this.tb_name.Enabled = true;
                this.tb_des.Enabled = true;
                this.tb_url.Enabled = true;
                this.btn_Icon.Enabled = true;
                this.btn_getUrl.Enabled = true;
                this.btn_Icon.Enabled = true;
                this.tb_version.Enabled = true;
                this.btn_genVer.Enabled = true;                
                this.btn_addURL.Enabled = true;
                this.btn_rmURL.Enabled = true;

                this.btn_genVer.Visible = true;
                this.btn_addURL.Visible = true;
                this.btn_rmURL.Visible = true;
            } else {
                this.tb_name.Enabled = false;
                this.tb_des.Enabled = false;
                this.tb_url.Enabled = false;
                this.btn_Icon.Enabled = false;
                this.btn_getUrl.Enabled = false;
                this.btn_Icon.Enabled = false;
                this.tb_version.Enabled = false;
                this.btn_genVer.Enabled = false;
                this.btn_addURL.Enabled = false;
                this.btn_rmURL.Enabled = false;

                this.btn_genVer.Visible = false;
                this.btn_addURL.Visible = false;
                this.btn_rmURL.Visible = false;
            }            
        }
        private string getIconPath() {
            //TODO
            return "";
            //throw new NotImplementedException();
        }
        /// <summary>
        /// set the name area focused when initial show the view 
        /// </summary>
        public void setInitFocus() {
            this.tb_name.Focus();
        }

        private void btn_getUrl_Click(object sender, EventArgs e) {
            this.raiseUpdateUrlEvt(sender,this.tb_url);
        }
        
        internal void updateScriptRoot() {
            this.tb_name.Text = _sroot.Name;
            this.tb_des.Text = _sroot.Description;
        }

        private void textBoxName_Leave(object sender, EventArgs e) {
            handleSRNameTextChanged();
        }

        void tb_name_TextChanged(object sender, System.EventArgs e) {
            this.handleSRNameTextChanged();
        }

        private void tb_des_Leave(object sender, EventArgs e) {
            this.handleSRDesTextChanged();
        }

        private void tb_des_TextChanged(object sender, EventArgs e) {
            handleSRDesTextChanged();
        }

        void textBoxUrl_TextChanged(object sender, System.EventArgs e) {
            this.handleURLTextChanged();
        }

        private void handleURLTextChanged() {
            if (needValidURL == false) {
                needValidURL = true;
                return;
            }            
            string txt = tb_url.Text.Trim();
            lb_msg_gen.Text = string.Empty;
            if (!(txt.StartsWith("http://") || txt.StartsWith("https://"))) {
                lb_msg_gen.Text = UILangUtil.getMsg("dlg.addurl.err.text1");
            } else {
                if (!ModelManager.Instance.isTrustedURL(txt, _sroot.TrustedUrls)) {
                    this._sroot.TargetWebURL = txt;
                    updateTrustedURLListView();
                    this.raiseUpdateModelEvt(this, this._sroot.TargetWebURL);
                }
                if (lb_msg_gen.Text == UILangUtil.getMsg("dlg.addurl.err.text1")) {
                    lb_msg_gen.Text = string.Empty;
                }
            }
        }
        
        private void updateTrustedURLListView() {
            lv_url.Items.Clear();
            lv_url.BeginUpdate();
            foreach (string url in this._sroot.TrustedUrls) { 
                ListViewItem lvi = new ListViewItem();
                lvi.Text = url;
                lvi.Tag = url;
                lv_url.Items.Add(lvi);
            }
            
            lv_url.EndUpdate();
        }

        private void handleSRNameTextChanged() {
            if (this.tb_name.Text.Equals(Constants.BLANK_TEXT)) {
                return;
            }
            if (this._sroot != null) {
                if (tb_name.Text == null || tb_name.Text.Trim().Length < 1) {
                    this._sroot.Name = "Root";
                    this.tb_name.Text = this._sroot.Name;
                    this.raiseUpdateModelEvt(this, this._sroot.Name);
                    return;
                }
                if (this._sroot.Name != null && !this._sroot.Name.Equals(this.tb_name.Text.Trim())) {
                    this._sroot.Name = this.tb_name.Text.Trim();
                    this.raiseUpdateModelEvt(this, this._sroot.Name);
                    return;
                }
            }
        }

        private void handleSRDesTextChanged() {
            if (this.tb_des.Text.Equals(Constants.BLANK_TEXT)) {
                return;
            }
            if (this._sroot.Description == null) {
                this._sroot.Description = "";
            }
            if (!this._sroot.Description.Equals(this.tb_des.Text.Trim())) {
                this._sroot.Description = this.tb_des.Text.Trim();
                this.raiseUpdateModelEvt(this, this._sroot.Description);
                return;
            }
        }
        
        private void tb_timeout_TextChanged(object sender, EventArgs e) {
            int t = 0;
            try {
                t = Convert.ToInt32(this.tb_timeout.Text);
                if (t > 0) {
                    this.timeout = t;
                }                
            } catch (Exception) {}
            _sroot.Timeout = this.timeout;
            //this.tb_timeout.Text = this.timeout.ToString();
            if (this.is_tb_timeout_updatedByHand) {
                this.raiseUpdateModelEvt(this, this._sroot.Timeout);
            }
            this.is_tb_timeout_updatedByHand = false;
        }

        private void tb_timeout_KeyDown(object sender, KeyEventArgs e) {
            this.timeout = Convert.ToInt32(this.tb_timeout.Text);
            if (e.KeyValue >= 48 && e.KeyValue <= 57) {
                this.is_tb_timeout_updatedByHand = true;
            }
        }

        private void ckb_enableSec_CheckedChanged(object sender, EventArgs e) {
            if( ckb_enableSec.Checked != this._sroot.UrlsLocked){
                this._sroot.UrlsLocked = ckb_enableSec.Checked;
                this.raiseUpdateModelEvt(this, this._sroot.UrlsLocked);
            }            
        }

        private void btn_addURL_Click(object sender, EventArgs e) {
            AddURLDialog dlg = new AddURLDialog();
            DialogResult dr = dlg.showAddURLDlg(UIUtils.getTopControl(this), this._sroot.TrustedUrls);
            if (dr == DialogResult.OK) {
                string url = dlg.URL;
                if (!ModelManager.Instance.isTrustedURL(url, this._sroot.TrustedUrls)) {
                    this._sroot.TrustedUrls.Add(url);
                    this.updateTrustedURLListView();
                    this.raiseUpdateModelEvt(this, this._sroot.TrustedUrls);
                }
            }
        }

        private void btn_rmURL_Click(object sender, EventArgs e) {
            if (lv_url.SelectedItems.Count == 1) {
                ListViewItem lvi = lv_url.SelectedItems[0];
                string url = lvi.Tag.ToString();
                this._sroot.TrustedUrls.Remove(url);
                this.updateTrustedURLListView();
                this.raiseUpdateModelEvt(this, this._sroot.TrustedUrls);
            }
        }

        private void btn_genVer_Click(object sender, EventArgs e) {
            if (this._sroot != null) {
                string ver = this._sroot.Version;
                string nver = ModelManager.Instance.getNextVersion(ver);
                this._sroot.Version = nver;
                this.needValidVersion = false;
                this.tb_version.Text = nver;
                this.lb_msg_gen.Text = string.Empty;
                this.raiseUpdateModelEvt(this, nver);
            }
        }

        private void tb_version_TextChanged(object sender, EventArgs e) {
            if (needValidVersion == false) {
                needValidVersion = true;
                return;
            }
            if (this._sroot != null) {
                string ver = this.tb_version.Text.Trim();
                string msg = ModelManager.Instance.checkVersion(ver, this._sroot.Version);
                if (msg == string.Empty) {
                    this._sroot.Version = ver;
                    lb_msg_gen.Text = string.Empty;
                    this.raiseUpdateModelEvt(this, ver);
                } else {
                    lb_msg_gen.Text = msg;
                }
            }
        }                
    }
}
