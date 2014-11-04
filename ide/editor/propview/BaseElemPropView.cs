using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.ide.editor.model;
using WebMaster.ide.ui;
using WebMaster.lib.engine;
using WebMaster.lib.ui;
using ide;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    /// <summary>
    /// this properties view is used to show info about 
    /// WebElement, WebElementGroup, ScriptRoot, OpCondition
    /// </summary>
    public partial class BaseElemPropView : UserControl, IPropView
    {
        /// <summary>
        /// acceptable input can be ScriptRoot, OpCondition, WebElementGroup, WebElement
        /// </summary>
        private BaseElement input = null;
        private FlowPropViewManager flowPVManager = null;
        /// <summary>
        /// It is used to record whether the text is updated by keyboard input 
        /// </summary>
        private bool tb_name_updated = false;
        private bool tb_des_updated = false;
        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { flowPVManager = value; }
        }
        public BaseElemPropView() {
            InitializeComponent();
        }
        #region events
        /// <summary>
        /// raise the event for outer properties control that an Element validation error
        /// the sender is property view, data is Validation element, msg is validatoin info
        /// if the msg is string.EMPTY, it means that this is a fixed error msg
        /// </summary>
        public event EventHandler<ValidationArgs> ElementValidationEvt;
        protected virtual void OnElementValidationEvt(ValidationArgs e) {
            EventHandler<ValidationArgs> elementValidationEvt = ElementValidationEvt;
            if (elementValidationEvt != null) {
                elementValidationEvt(this, e);
            }
        }
        /// <summary>
        /// raise the event for outer properties control that an Element validation error
        /// the sender is property view, data is Validation element, msg is validatoin info        
        /// 
        /// If the msg is string.EMPTY, it means that this is a fixed error msg
        /// </summary>
        /// <param name="sender"></param>
        public void raiseElementValidationEvt(Object sender, object data, MsgType type, string msg) {
            if (data != null) {
                ValidationArgs evt = new ValidationArgs(sender, data, type, msg);
                OnElementValidationEvt(evt);
            }
        }
        #endregion events 
        #region madatory methods 
        /// <summary>
        /// input must be ScriptRoot, WebElementGroup, WebElement, OpCondition
        /// </summary>
        /// <param name="input"></param>
        public void setInput(Object input) {
            cleanView();
            if (input is ScriptRoot || input is OpCondition || input is WebElementGroup || input is WebElement || input is ParamGroup) {
                BaseElement be = (BaseElement)input;
                this.input = be;
                this.updateElemTypeText(be);
                if (be != null) {
                    enableView();
                    this.tb_name_updated = false;
                    if (this.input.Name != null && this.input.Name.Trim().Length > 0) {
                        tb_name.ForeColor = Color.Black;
                        tb_name.Text = this.input.Name;
                    } else {
                        tb_name.ForeColor = Color.Red;
                        tb_name.Text = UILangUtil.getMsg("valid.be.name.msg1");// Name is mandatory
                    }
                    this.tb_des_updated = false;
                    tb_Description.Text = this.input.Description;
                }
                // special handle RawDataGroup, it is not a good design here
                if(be.Equals(FlowPVManager.Bigmodel.SRoot.RawElemsGrp)){
                //if (be.Name.Equals(Constants.RAW_DATA_NAME)) {
                    tb_name.Enabled = false;
                    tb_Description.Enabled = false;
                } else {
                    tb_name.Enabled = true;
                    tb_Description.Enabled = true;
                }

                // update validation msg 
                updateValidationMsg();
                showView();
            } else {
                this.input = null;
                this.disableView();
            }
        }

        public void cleanView() {
            this.tb_des_updated = false;
            tb_Description.Text = Constants.BLANK_TEXT;
            this.tb_name_updated = false;            
            tb_name.Text = Constants.BLANK_TEXT;
        }

        public void showView() {
            this.Visible = true;
        }

        public void hideView() {
            this.Visible = false;
        }

        public void enableView() {
            tb_name.Enabled = true;
            tb_Description.Enabled = true;
        }

        public void disableView() {
            this.tb_name_updated = false;
            tb_name.Text = Constants.BLANK_TEXT;
            this.tb_des_updated = false;
            tb_Description.Text = Constants.BLANK_TEXT;
            tb_name.Enabled = false;
            tb_Description.Enabled = false;
        }

        private void updateValidationMsg() {
            ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(this.input, "");
            if (msg.Type != MsgType.VALID) {
                this.label_msg.ForeColor = Color.Red;
                this.label_msg.Text = msg.Msg;
            } else {
                this.label_msg.ForeColor = Color.Red;
                this.label_msg.Text = "";
            }
        }
        #endregion mandatory method 
        #region UI methods 
        private void tb_name_KeyUp(object sender, KeyEventArgs e) {            
            // enter to update the name if need 
            //if (e.KeyCode == Keys.Enter) {                
            //    this.tb_Description.Focus();
            //}
        }
        private void tb_name_KeyDown(object sender, KeyEventArgs e) {
            this.tb_name_updated = true;
        }
        private void tb_name_TextChanged(object sender, EventArgs e) {
            if (this.tb_name_updated) {
                handleNameTextChanged();
            }
        }
        private void tb_Name_Leave(object sender, EventArgs e) {
            handleNameTextChanged();
        }
        private void tb_Description_KeyDown(object sender, KeyEventArgs e) {
            this.tb_des_updated = true;
        }
        private void tb_Description_TextChanged(object sender, EventArgs e) {
            if (this.tb_des_updated) {
                this.handleDesTextChanged();
            }
        }
        private void tb_Description_Leave(object sender, EventArgs e) {
            if (input != null && input.Description != null && !input.Description.Equals(tb_Description.Text)) {
                handleDesTextChanged();
            }
        }

        public void updatedInput() {
            if (input != null) {
                handleNameTextChanged();
            } else {
                cleanView();
            }
        }

        public object getInput() {
            return this.input;
        }

        private void updateElemTypeText(BaseElement be) {
            if (be is ScriptRoot) {
                labelType.Text = UILangUtil.getMsg("view.common.type.Script");
            } else if (be is WebElement) {
                labelType.Text = UILangUtil.getMsg("view.common.type.WE"); 
            } else if (be is WebElementGroup) {
                labelType.Text = UILangUtil.getMsg("view.common.type.WEG"); 
            } else if (be is OpCondition) {
                labelType.Text = UILangUtil.getMsg("view.common.type.Opc"); 
            } else if (be is ParamGroup) {
                labelType.Text = UILangUtil.getMsg("view.common.type.ParamGrp");
            } else {
                labelType.Text = UILangUtil.getMsg("view.common.type.gen");
            }
        }        
        /// <summary>
        /// update input if need 
        /// </summary>
        private void handleNameTextChanged() {            
            if (tb_name.Text == null || tb_name.Text.Trim().Length < 1) {
                label_msg.ForeColor = Color.Red;
                label_msg.Text = UILangUtil.getMsg("valid.be.name.msg1");// Name is mandatory
                return ;
            }
            string newn = tb_name.Text.Trim();
            if(newn.Equals(this.input.Name)){
                return;
            }

            string tn = this.input.Name;
            this.input.Name = newn;
            ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(input, "");
            
            if(msg.Type == MsgType.VALID) {                
                this.label_msg.Text = "";
                FlowPVManager.raiseInputUpdatedEvt(this, input);
            } else {
                this.input.Name = tn;
                label_msg.ForeColor = Color.Red;
                this.label_msg.Text = msg.Msg;
            }
        }
        /// <summary>
        /// update input if need 
        /// </summary>
        private void handleDesTextChanged() {
            if (this.input == null) {
                return;
            }
            if (this.tb_Description.Text != null && this.tb_Description.Text != Constants.BLANK_TEXT) {
                string ndes = this.tb_Description.Text;
                if (!ndes.Equals(this.input.Description)) {
                    this.input.Description = ndes;
                    FlowPVManager.raiseInputUpdatedEvt(this, input);
                }
            }            
        }
        #endregion UI methods        
    }
}
