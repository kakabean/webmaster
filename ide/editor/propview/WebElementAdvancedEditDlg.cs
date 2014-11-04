using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.ide.editor.propview
{
    public partial class WebElementAdvancedEditDlg : Form
    {
        #region variables 
        private WebElement input = null;
        private ScriptRoot sroot = null;
        private List<WebElementAttribute> output = new List<WebElementAttribute>();
        /// <summary>
        /// All WEAs that maybe or may not updated with the dialog. 
        /// </summary>
        public List<WebElementAttribute> Output {
            get { return output; }
            set { output = value; }
        }

        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        /// </summary>
        private int pattenChangedByHandFlag = 0; 
        /// <summary>
        /// Pattern text
        /// </summary>
        private string[] ps = null;
        #endregion variables 
        public WebElementAdvancedEditDlg() {
            InitializeComponent(); 
        }

        public DialogResult showEditDlg(IWin32Window handler, WebElement we, ScriptRoot sroot) {
            ps = ModelManager.Instance.getStringPatterns();
            setInput(we,sroot);
            return this.ShowDialog(handler);
        }

        public void setInput(WebElement we,ScriptRoot sroot) {
            this.input = we;
            this.sroot = sroot;
            cleanView();
            disableDetailsArea();
            cloneWEAs();
            updateView();
        }

        private void cloneWEAs() {
            if (input != null) {
                foreach (WebElementAttribute wea in this.input.Attributes) {
                    output.Add(wea.Clone());
                }
            }
        }

        private void disableDetailsArea() {
            this.cb_pattern.Enabled = false;
            this.tb_value.Enabled = false;
            this.btn_edit.Enabled = false;
        }

        private void enableDetailsArea() {
            this.cb_pattern.Enabled = true;
            this.tb_value.Enabled = true;
            this.btn_edit.Enabled = true;
        }

        private void cleanView() {
            this.weaGrp.Text = "";
            this.lv_wea.Items.Clear();
            this.cb_pattern.Items.Clear();
            this.cb_pattern.Text = null;
            this.tb_value.Text = string.Empty;
            this.label_msg.Text = string.Empty;
        }
        /// <summary>
        /// Update view with the input 
        /// </summary>
        private void updateView() {
            if (this.input != null) {
                this.weaGrp.Text = this.input.Name;
                updateWEALV();
                this.cb_pattern.Items.AddRange(ps);
            } else {
                cleanView();    
            }
        }

        private void updateWEALV() {
            if (this.input != null) {
                foreach (WebElementAttribute wea in Output) {
                    ListViewItem lvi = new ListViewItem(wea.Name);
                    //lvi.Text = wea.Name;
                    lvi.Tag = wea;
                    this.lv_wea.Items.Add(lvi);
                }
            }
        }

        private void lv_wea_MouseDown(object sender, MouseEventArgs e) {
            ListViewItem lvi = lv_wea.GetItemAt(e.X, e.Y);
            if (lvi != null && lvi.Tag is WebElementAttribute) {
                this.enableDetailsArea();
                updateDetailsArea(lvi.Tag as WebElementAttribute);
            } else {
                disableDetailsArea();
                updateDetailsArea(null);
            }
        }

        private void updateDetailsArea(WebElementAttribute wea) {
            if (wea == null) {
                this.cb_pattern.Text = string.Empty;

                this.pattenChangedByHandFlag = 2;
                this.cb_pattern.SelectedIndex = -1;
                doCB_PatternIndexChangedEvent();

                this.tb_value.Text = string.Empty;
                this.btn_edit.Enabled = false;
                this.label_msg.Text = string.Empty;
            } else {
                string pn = ModelManager.Instance.getPatternText(wea.PATTERN);
                int index = getPatternCBIndex(pn);

                this.pattenChangedByHandFlag = 2;
                this.cb_pattern.SelectedIndex = index;
                this.doCB_PatternIndexChangedEvent();

                string value = ModelManager.Instance.getWEAText4Design(wea);
                this.tb_value.Text = value;
                this.btn_edit.Enabled = true;
                this.label_msg.Text = string.Empty;
            }
        }
        /// <summary>
        /// Get the combo box index align with the text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int getPatternCBIndex(string text) {
            for(int i=0; i<ps.Length; i++){
                if (text.Equals(ps[i])) {
                    return i;
                }
            }
            return -1 ;
        }

        private void cb_pattern_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_PatternIndexChangedEvent();
        }

        private void cb_pattern_SelectionChangeCommitted(object sender, EventArgs e) {
            this.pattenChangedByHandFlag = 1;
        }
        /// <summary>
        /// It is stange that sometimes the although the SelectedIndex is set, but the cb_pattern_SelectedIndexChanged method
        /// was not triggered, so just handle this event by handle, not by the event. 
        /// </summary>
        private void doCB_PatternIndexChangedEvent() {
            if (pattenChangedByHandFlag == 0) {
                // it is used to reduce the duplicated second run.
                return;
            }
            // update comboBox text 
            if (this.cb_pattern.SelectedIndex == -1) {
                this.cb_pattern.Text = Constants.BLANK_TEXT;
            } else {
                this.cb_pattern.Text = this.cb_pattern.Items[this.cb_pattern.SelectedIndex].ToString();
            }
            // clean details area 
            if (pattenChangedByHandFlag == 1) {
                this.tb_value.Text = string.Empty;
                // update model 
                if (this.lv_wea.SelectedItems.Count == 1 && this.lv_wea.SelectedItems[0].Tag is WebElementAttribute) {
                    WebElementAttribute wea = this.lv_wea.SelectedItems[0].Tag as WebElementAttribute;
                    CONDITION ptn = ModelManager.Instance.getPattern(this.cb_pattern.Text);
                    if (ptn != CONDITION.EMPTY) {
                        wea.PATTERN = ptn;
                    }
                    wea.PValues.Clear();
                    wea.RealPValue = null;
                }
            }
            pattenChangedByHandFlag = 0;
        }

        private void btn_edit_Click(object sender, EventArgs e) {
            if (lv_wea.SelectedItems.Count > 0 && lv_wea.SelectedItems[0].Tag is WebElementAttribute) {
                WebElementAttribute wea = lv_wea.SelectedItems[0].Tag as WebElementAttribute;
                WEAPValuesEditDlg dlg = new WEAPValuesEditDlg();
                DialogResult dr = dlg.showEditDlg(this, wea, this.sroot);
                if (dr == System.Windows.Forms.DialogResult.OK) {
                    if (dlg.Output.Count == 0) {
                        return;
                    } else {
                        wea.PValues.Clear();
                        wea.PValues.AddRange(dlg.Output);
                        
                        string value = ModelManager.Instance.getWEAText4Design(wea);
                        this.tb_value.Text = value;
                    }
                }
            }            
        }
    }
}
