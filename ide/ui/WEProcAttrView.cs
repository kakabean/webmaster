using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using mshtml;
using System.Collections;
using WebMaster.lib;
using WebMaster.ide.editor;
using WebMaster.lib.ui;
using WebMaster.com;
using ide;

namespace WebMaster.ide.ui
{
    public partial class WEProcAttrView : UserControl, IWEPropView
    {
        #region constants 
        public static readonly int COL_MIN_WIDTH = 20 ;

        #endregion 
        #region variable definition
        // current editable WebElement 
        private WebElement _we = null;
        // record the error message if the validation error
        private string _errmsg = String.Empty;
        // script root 
        private ScriptRoot sroot = null;
        private TextBox ceTextBox = null;
        private ComboBoxEx ceComboBox = null;
        /// <summary>
        /// it is used to help to control the attribute table check/unchecked status 
        /// </summary>
        private int ATT_CK_FLAG = -2;
        /// <summary>
        /// use to check whether the WebElement is an existed updated one or a new to be created one 
        /// </summary>
        private bool _isNewWE = true;
        /// <summary>
        /// maintain whether the WE is a password HtmlElement 
        /// </summary>
        private bool isPassword = false;
        #endregion variable definition
        #region events
        /// <summary>
        /// raise the event for outer properties control to check the current view modeled
        /// WebElement. event data is useless 
        /// </summary>
        public event EventHandler<CommonEventArgs> CheckWebElementEvt;
        protected virtual void OnCheckWebElementEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> checkWebElementEvt = CheckWebElementEvt;
            if (checkWebElementEvt != null) {
                checkWebElementEvt(this, e);
            }
        }
        /// <summary>
        /// raise the event for outer properties control to check the current view modeled
        /// WebElement. event data is useless 
        /// </summary>
        /// <param name="sender"></param>
        public void raiseCheckWebElementEvt(Object sender) {
            if (sroot != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, null);
                OnCheckWebElementEvt(evt);
            }
        }
        #endregion events 
        #region ui functions
        public WEProcAttrView() {
            InitializeComponent();
            initUIData();
        }

        private void initUIData() {
            string[] ps = ModelManager.Instance.getStringPatterns();
            // text box cell editor 
            this.ceTextBox = new TextBox();
            this.ceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ceTextBox.Location = new System.Drawing.Point(32, 104);
            this.ceTextBox.Multiline = true;
            this.ceTextBox.Name = "ceTextBox";
            this.ceTextBox.Size = new System.Drawing.Size(80, 16);
            this.ceTextBox.TabIndex = 3;
            this.ceTextBox.Visible = false;
            this.ceTextBox.VisibleChanged += new EventHandler(ceTextBox_VisibleChanged);

            // combo box cell editor
            this.ceComboBox = new ComboBoxEx();
            this.ceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ceComboBox.IntegralHeight = false;
            this.ceComboBox.ItemHeight = 13;
            this.ceComboBox.Location = new System.Drawing.Point(32, 80);
            this.ceComboBox.Name = "ceComboBox";
            this.ceComboBox.Size = new System.Drawing.Size(80, 21);
            this.ceComboBox.TabIndex = 1;
            this.ceComboBox.Visible = false;
            this.ceComboBox.Items.AddRange(ps);
            this.ceComboBox.SelectedValueChanged += new EventHandler(ceComboBox_SelectedValueChanged);

            this.groupBox1.Controls.Add(ceTextBox);
            this.groupBox1.Controls.Add(ceComboBox);

            Control[] ceeditors = new Control[] { null,ceTextBox,ceComboBox};
            this.listViewEx1.setCellEditors(ceeditors);

            this.listViewEx1.SubItemClicked += new SubItemEventHandler(listViewEx1_SubItemClicked);
            this.listViewEx1.SubItemEndEditing += new SubItemEndEditingEventHandler(listViewEx1_SubItemEndEditing);
        }

        void ceTextBox_VisibleChanged(object sender, EventArgs e) {
            string text = UILangUtil.getMsg("view.wea.ce.multiline");
            if(text.Equals(errmsg.Text)){
                errmsg.ForeColor = Color.Black ;
                errmsg.Text = string.Empty ;
            }
        }

        void ceComboBox_SelectedValueChanged(object sender, EventArgs e) {
            this.listViewEx1.EndEditing(true);
        }

        void listViewEx1_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e) {
            // do nothing
        }

        void listViewEx1_SubItemClicked(object sender, SubItemEventArgs e) {
            listViewEx1.StartEditing(e.SubItem, e.Item);
            if (ceTextBox.Lines.Length > 1) {
                errmsg.ForeColor = Color.DarkOrange;
                errmsg.Text = UILangUtil.getMsg("view.wea.ce.multiline");
            } else {
                errmsg.ForeColor = Color.Black;
                errmsg.Text = string.Empty;
            }
        }
        /// <summary>
        /// this method is used to set the checked status if the first column clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewEx1_SubItemClicked_1(object sender, SubItemEventArgs e) {
            int index = e.SubItem;
            if (index < 0 || index > this.listViewEx1.Columns.Count) {
                return;
            }
            if (index == 0) {
                ATT_CK_FLAG = -1;
                ListViewItem lvi = e.Item;
                lvi.Checked = !lvi.Checked;
            }
        }

        private void listViewEx1_ItemChecked(object sender, ItemCheckedEventArgs e) {
            if (ATT_CK_FLAG == -1) {
                ATT_CK_FLAG = 0;
            } else if (ATT_CK_FLAG == 0) {
                ATT_CK_FLAG = 1;
                ListViewItem lvi = e.Item;
                lvi.Checked = !lvi.Checked;
            }
        }

        private void listViewEx1_ItemCheck(object sender, ItemCheckEventArgs e) {
            //e.CurrentValue = CheckState.Checked;

        }
        #endregion 
        #region mandatory methods
        /// <summary>
        /// update the property view based on the element. e.g by specific string, by attributes, 
        /// by color, by location or by image, it depends on the WEType.
        /// </summary>
        /// <param name="elem">HtmlElement as a input of the property view</param>
        /// <param name="isNew">if the elem is HTMLElement but isNew is false, restore the text and description info</param>
        public void updateView(Object elem, bool isNew) {
            ATT_CK_FLAG = -2;
            this.resetView(isNew);
            if (elem is IHTMLElement) {
                if (isNew) {
                    this._we = null;
                }
                IHTMLElement he = elem as IHTMLElement;
                isPassword = WebUtil.isPassword(he);
                buildAttributeTable(he);
                if (this.listViewEx1.Enabled == false) {
                    this.listViewEx1.Enabled = true;
                    this.tb_name.Enabled = false;
                    this.tb_des.Enabled = false;
                }
                if (this.errmsg.Tag != null && this.errmsg.Tag.ToString() == "we") {
                    this.errmsg.ForeColor = Color.Black;
                    this.errmsg.Text = "";
                }
            } else if(elem is WebElement){ // this means that it is a updated WebElement operation
                this._we = elem as WebElement;
                this._isNewWE = false;
                this.isPassword = this._we.isPassword;
                buildAttributeTable(this._we);
                this.tb_name.Text = this._we.Name;
                this.tb_des.Text = this._we.Description;

                this.listViewEx1.Enabled = false;
                this.tb_name.Enabled = false;
                this.tb_des.Enabled = false;
                // if it is an parametered WE, the WEA values can not be updated in this page.
                if (this._we.hasParameter) {
                    this.listViewEx1.Enabled = false;
                    this.tb_name.Enabled = false;
                    this.tb_des.Enabled = false;
                    this.errmsg.ForeColor = Color.DarkOrange;
                    this.errmsg.Text = UILangUtil.getMsg("view.we.att.param.msg1");// This Element is combined with Parameter, It can not be edited here.
                    this.errmsg.Tag = "we";
                }
            }else{
                //TODO LOG
            }
        }
        /// <summary>
        /// get the WebElement specified by the property view. Note that whenever the updateView(xx) method 
        /// called, the this._we will be set to null and create a new one. 
        /// while if thie this._we is not null, it means the the _we is the current expected one, so in order
        /// to get the latest _we info, it will update the _we value with current UI info each time. 
        /// </summary>
        /// <returns>get the WebElement from the property view</returns>
        public WebElement getWebElement() {
            if (this._we == null) {
                this._we = buildupWebElement();
            } else {
                // If it is a parametered WebElement, it can not be updated in prop view.
                if (!this._we.hasParameter) {
                    updateWebElement(this._we);
                }
            }
            this._we.isPassword = this.isPassword;
            return this._we;
        }
        private void resetView(bool isNew) {
            this.listViewEx1.Enabled = true;
            this.listViewEx1.Items.Clear();

            //this.listViewEx1.Clear();
            this.errmsg.Text = "";

            this.tb_name.Enabled = true;
            this.tb_des.Enabled = true;
            this.isPassword = false;
            if (isNew) {            
                // clean UI info 
                this.tb_name.Text = Constants.BLANK_TEXT;
                this.tb_des.Text = Constants.BLANK_TEXT;
            }
        }
        public void resetView() {
            resetView(true);
        }
        public void showView() {
            this.Visible = true;
        }
        public void hideView() {
            this.Visible = false;
        }
        /// <summary>
        /// set the view editable areas enabled 
        /// </summary>
        public void enableView() {
            //this.listViewEx1.Enabled = true;
            this.tb_name.Enabled = true;
            this.tb_des.Enabled = true;
        }
        /// <summary>
        /// set the view editable areas disabled
        /// </summary>
        public void disableView() {
            //this.listViewEx1.Enabled = false;
            this.tb_name.Enabled = false;
            this.tb_des.Enabled = false;            
        }
        public bool isValid() {
            this._errmsg = String.Empty;
            // check name area 
            if (this.tb_name.Text == null || this.tb_name.Text.Trim().Length < 1) {
                this._errmsg = UILangUtil.getMsg("view.we.attr.valid.text1");
                return false;
            }
            // check WebElement attributes area
            if (this.listViewEx1.Items.Count < 1) {
                this._errmsg = UILangUtil.getMsg("view.we.attr.valid.text2");
                return false;
            }
            // check selected WEA
            if (this._we.Tag == null || this._we.Tag.Length < 1) {
                this._errmsg = UILangUtil.getMsg("view.we.attr.valid.text3");
                return false;
            }
            if (this._isNewWE) {
                // check whether the WebElement name is unique
                ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(this._we, "WebElement - ", sroot.RawElemsGrp.Elems);
                if (msg.Type != MsgType.VALID) {
                    this._errmsg = msg.Msg;
                    return false;
                }
            }
            return true;
        }
        
        public string getInvalidMsg() {
            return this._errmsg;
        }
        public void setScriptRoot(ScriptRoot sroot) {
            this.sroot = sroot;
            this.resetView();
        }
        /// <summary>
        /// this method can be invoked when control size changed 
        /// </summary>
        public void handleSizeChangedEvt() {
            int w_colKey = 102;
            int w_colValue = 318;
            int w_colPattern = 80;
            int w_group = 510;
            int h_group = 255;
            // update the attribute table's column width           
            if (this.colKey.Width < w_colKey) {
                this.colKey.Width = w_colKey;
            }
            if (this.colValue.Width < w_colValue) {
                this.colValue.Width = w_colValue;
            }
            if (this.colPattern.Width < w_colPattern) {
                this.colPattern.Width = w_colPattern;
            }
            int dw = this.ClientSize.Width - this.AutoScrollMinSize.Width;
            // group1 size 
            int gw = w_group; // min size
            int gh = h_group; // min size            
            if (dw > 0) {
                int delta = dw / 5;
                gw = w_group + delta * 3;
                this.colValue.Width = w_colValue + delta * 3 - 5;                
            }            
            this.groupBox1.Size = new Size(gw, gh);
            
            int w_tb1 = 362;
            int w_tb2 = 375;
            int w_panel1 = 440;
            int d1 = this.panel1.Width - w_panel1;
            if (d1 > 0) {
                this.tb_name.Size = new Size(w_tb1 + d1+5, this.tb_name.Height);
                this.tb_des.Size = new Size(w_tb2 + d1+5, this.tb_des.Height);
            } else {
                this.tb_name.Size = new Size(w_tb1, this.tb_name.Height);
                this.tb_des.Size = new Size(w_tb2, this.tb_des.Height);
            }
        }

        public void resetUISize() {
            this.Size = new Size(this.Size.Width, 270);
            handleSizeChangedEvt();
        }
        #endregion mandatory methods 
        #region functions 
        /// <summary>
        /// build up WebElement from the UI properties view 
        /// </summary>
        /// <returns></returns>
        private WebElement buildupWebElement() {
            WebElement we = ModelFactory.createWebElement();
            updateWebElement(we);
            return we;
        }
        /// <summary>
        /// update the _we data with UI info
        /// <param name="we"> to be updated WebElement</param>
        /// </summary>
        private void updateWebElement(WebElement we) {
            if (we == null) {
                return;
            }
            we.Attributes.Clear();
            //we.ID = null;
            //we.Tag = string.Empty;

            we.Name = this.tb_name.Text.Trim();
            we.Description = this.tb_des.Text.Trim();
            
            foreach (ListViewItem lvi in listViewEx1.CheckedItems) {
                WebElementAttribute wea = WebUtil.getWebElementAttribute(we, lvi.Text.Trim());
                if (wea == null) {
                    wea = ModelFactory.createWebElementAttribute();
                }
                wea.Key = lvi.SubItems[0].Text;
                wea.PValues.Clear();
                wea.PValues.Add(lvi.SubItems[1].Text);
                wea.PATTERN = ModelManager.Instance.getPattern(lvi.SubItems[2].Text);
                we.Attributes.AddUnique(wea);
                //// handle id 
                //if(Constants.HE_ID.Equals(wea.Key.ToLower())){
                //    we.ID = ModelManager.Instance.getWEAText4Design(wea);
                //}
                //// handle tag 
                //if (Constants.HE_TAG.Equals(wea.Key.ToLower())) {
                //    we.Tag = ModelManager.Instance.getWEAText4Design(wea);
                //}
            }            
        }        
        private void buildAttributeTable(IHTMLElement elem) {
            HashtableEx attributeTable = WebUtil.getValuedAttriubtes(elem);
            this.listViewEx1.BeginUpdate();
            // clean view
            this.listViewEx1.Items.Clear();
            if (attributeTable != HashtableEx.EMPTY) { //build table 
                foreach (object o in attributeTable) {
                    string key = Convert.ToString(o);
                    string value = Convert.ToString(attributeTable.Get(o));
                    string pn = ModelManager.Instance.getPatternText(CONDITION.STR_FULLMATCH);
                    addAttTableRow(this.listViewEx1, key, value, pn);
                }
            }
            this.listViewEx1.EndUpdate();
        }
        /// <summary>
        /// this functions is used to rebuild the WebElement table if user want to
        /// modify the attribute value. 
        /// </summary>
        /// <param name="we"></param>
        private void buildAttributeTable(WebElement we){
            if (we != null && we.Attributes.Count > 0) {
                this.listViewEx1.BeginUpdate();
                // clean table 
                this.listViewEx1.Items.Clear();
                // create table                 
                foreach (WebElementAttribute wea in we.Attributes) {
                    string pn = ModelManager.Instance.getPatternText(wea.PATTERN);
                    addAttTableRow(this.listViewEx1, wea.Key, ModelManager.Instance.getWEAText4Design(wea), pn);
                }
                // set all items in checked state
                foreach (ListViewItem lvi in this.listViewEx1.Items) {
                    lvi.Checked = true;
                }
                this.listViewEx1.EndUpdate();
            }
        }
        /// <summary>
        /// add an attribute row in the Attribute View 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="pn"></param>
        private void addAttTableRow(ListView lv, string key, string value, string pn)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = key;
            //lvi.SubItems.Add(key);
            lvi.SubItems.Add(value);
            lvi.SubItems.Add(pn);
            lv.Items.Add(lvi);
        }
        #endregion        

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            //if (e.KeyCode == Keys.Enter) {
            //    tb_des.Focus();
            //}
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e) {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Enter) {
                raiseCheckWebElementEvt(this);
            }
        }
    }
}
