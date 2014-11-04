using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.ide.ui;
using WebMaster.lib.engine;
using WebMaster.lib.ui;
using WebMaster.ide.editor.propview;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    public partial class ParamPropView : UserControl, IPropView
    {
        #region constants 
        
        #endregion constants 
        #region variables 
        private Parameter input = null;
        
        // whether the value is updated by hand - tb_name
        private bool tb_name_updated = false;
        // whether the value is updated by hand - rtb_value
        private bool rtb_value_updated = false;        
        // whether the value is updated by hand - tb_des
        private bool tb_des_updated = false;
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int cb_TypeFlag = 0;
        private int cb_SetTypeFlag = 0;
        private int cb_SetAccessFlag = 0;
        /// <summary>
        /// string pattern text
        /// </summary>
        private List<string> categoryStr = new List<string>();
        private List<string> categoryNum = new List<string>();
        private List<string> categoryObj = new List<string>();
        private List<string> categorySet = new List<string>();

        /// <summary>
        /// this is trick variable to make sure the i18n worked at design time
        /// </summary>
        private bool firstInitData = true;

        private FlowPropViewManager flowPVManager = null;

        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { flowPVManager = value; }
        }
        /// <summary>
        /// record the group size, so that to adjust the widget size if necessary. 
        /// </summary>
        private Size grpSize = new Size(int.MinValue,int.MinValue);
        #endregion variables 
        #region constructor
        public ParamPropView() {
            InitializeComponent();
            initUI();
        }

        private void initUI() {                    
        }
        /// <summary>
        /// Try to make sure the UI data was only initialized once. it is added to fix the 
        /// i18n design problem. If below code added into initUI() method, it will
        /// some errors in design time when the propview created.
        /// </summary>
        public void tryInitDataOnlyOnce() {
            if (this.firstInitData) {
                this.firstInitData = false;
                // initial parameter type box
                cb_type.Items.Add(ModelManager.Instance.getParamTypeText(ParamType.STRING));
                cb_type.Items.Add(ModelManager.Instance.getParamTypeText(ParamType.NUMBER));
                cb_type.Items.Add(ModelManager.Instance.getParamTypeText(ParamType.DATETIME));
                cb_type.Items.Add(ModelManager.Instance.getParamTypeText(ParamType.SET));

                // build set item type 
                cb_setType.Items.Add(ModelManager.Instance.getParamTypeText(ParamType.STRING));
                cb_setType.Items.Add(ModelManager.Instance.getParamTypeText(ParamType.NUMBER));

                // build set access type 
                cb_setAccess.Items.Add(ModelManager.Instance.getSetAccessText(SET_ACCESS.LOOP));
                cb_setAccess.Items.Add(ModelManager.Instance.getSetAccessText(SET_ACCESS.RANDOM_NO_DUPLICATE));
                cb_setAccess.Items.Add(ModelManager.Instance.getSetAccessText(SET_ACCESS.RANDOM));

                //cb_type.SelectedIndex = 0;
            }
        }
        #endregion constructors 
        #region events
        /// <summary>
        /// raise the event for outer properties control that an Element validation error
        /// the sender is property view, data is Validation element, msg is validatoin info
        /// 
        /// If the msg is string.EMPTY, it means that this is a fixed error msg
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
            if (data != null && string.Empty != msg) {
                ValidationArgs evt = new ValidationArgs(sender, data, type, msg);
                OnElementValidationEvt(evt);
            }
        }
        #endregion events 
        #region mandatory methods 
        /// <summary>
        /// update the properties ui values into input objects. 
        /// do nothing, for that in this view, the info will be updated to model
        /// immediately. 
        /// </summary>
        public void updatedInput() {
            // do nothing, for that in this view, the info will be updated to model
            // immediately. 
        }
        /// <summary>
        /// input must the OpCondition
        /// </summary>
        /// <param name="input"></param>
        public void setInput(object input) {
            cleanView();
            if (input is Parameter) {
                enableView();
                this.input = input as Parameter;
                initCommonArea();
                initDetailsArea();
                updateValidationMsg();
            } else {
                disableView();
            }
        }
        public object getInput() {
            return input;   
        }
        public void cleanView() {
            this.tb_name_updated = false;
            this.tb_name.Text = Constants.BLANK_TEXT;
            this.tb_des_updated = false;
            this.ckb_pwd.Tag = null;
            this.ckb_pwd.Checked = false;
            this.tb_des.Text = Constants.BLANK_TEXT;
            this.cb_type.Text = Constants.BLANK_TEXT;
            this.cb_setType.Text = Constants.BLANK_TEXT;
            this.cb_setAccess.Text = Constants.BLANK_TEXT;
            this.rtb_value_updated = false;
            this.rtb_value.Text = Constants.BLANK_TEXT;
            this.btn_set.Enabled = false;
            this.label_msg.Text = Constants.BLANK_TEXT;            
        }
        public void showView() {
            this.Visible = true;
        }
        public void hideView() {
            this.Visible = false;
        }
        public void enableView() {            
            this.tb_name.Enabled = true;
            this.tb_des.Enabled = true;
            this.cb_type.Enabled = true;
            this.cb_setType.Enabled = true;
            this.cb_setAccess.Enabled = true;
            this.rtb_value.Enabled = true;
            this.btn_set.Enabled = true;
        }
        public void disableView() {
            this.tb_name.Enabled = false;
            this.tb_des.Enabled = false;
            this.cb_type.Enabled = false;
            this.cb_setType.Enabled = false;
            this.cb_setAccess.Enabled = false;
            this.rtb_value.Enabled = false;
            this.btn_set.Enabled = false;
        }
        #endregion madatory methods 
        #region ui methods         
        /// <summary>
        /// update the OpCondition's validation messages 
        /// </summary>
        private void updateValidationMsg() {            
            ValidationMsg msg = getValidationMsg(this.input);
            if (msg.Type != MsgType.VALID) {
                this.label_msg.ForeColor = Color.Red;
                this.label_msg.Text = msg.Msg;
            } else {
                this.label_msg.ForeColor = Color.Black;
                this.label_msg.Text = "";
            }
        }

        private void initCommonArea() {
            if (this.input == null) {
                return;
            }
            this.tb_name_updated = false;
            this.tb_name.Text = this.input.Name;
            ckb_pwd.Tag = this.input;
            if (this.input.Sensitive) {                
                this.ckb_pwd.Checked = true;
            }
            this.tb_des_updated = false;
            this.tb_des.Text = this.input.Description;
                        
            this.cb_TypeFlag = 2;
            this.cb_type.SelectedIndex = getParamTypeIndex(this.input);
            this.doCB_TypeSelectionChangedEvt();        
        }
        private void initDetailsArea() {
            if (input.Type != ParamType.SET) {
                cb_setType.Enabled = false;
                cb_setType.Text = string.Empty;
                cb_setAccess.Enabled = false;
                cb_setAccess.Text = string.Empty;
                btn_set.Enabled = false;
                this.rtb_value_updated = false;
                rtb_value.Text = input.DesignValue.ToString();
            } else {
                cb_setType.Enabled = true;
                
                this.cb_SetTypeFlag = 2;
                cb_setType.SelectedIndex = getSetTypeIndex();
                this.doCB_SetTypeSelectionChangedEvt();

                cb_setAccess.Enabled = true;

                this.cb_SetAccessFlag = 2;
                cb_setAccess.SelectedIndex = getSetAccessIndex();
                this.doCB_SetAccSelectedIndexChangedEvt();

                btn_set.Enabled = true;
                showSetValues();
            }
            
        }
        private void showSetValues() {
            this.rtb_value_updated = false;
            rtb_value.Text = string.Empty ;
            if (input.DesignSet == null) {
                return;
            }
            bool flag = false;
            foreach (object obj in input.DesignSet) {
                if (flag) {
                    rtb_value.SelectionColor = Color.Black;
                } else {
                    rtb_value.SelectionColor = Color.Blue;
                }
                flag = !flag;
                rtb_value.AppendText(obj + Environment.NewLine);                
            }
            rtb_value.SelectionColor = Color.Black;
        }      
        private void tb_name_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {                
                this.tb_des.Focus();
            }
        }
        private void tb_name_KeyDown(object sender, KeyEventArgs e) {
            this.tb_name_updated = true;
        }
        private void tb_name_TextChanged(object sender, EventArgs e) {
            if (tb_name_updated) {
                handleNameTextChanged();
            }
        }
        private void tb_name_Leave(object sender, EventArgs e) {
            handleNameTextChanged();
        }
        private void handleNameTextChanged() {                        
            String name = tb_name.Text.Trim();
            if (name.Length > 0 && !name.Equals(input.Name)) {
                input.Name = name;                
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
            if (name.Length < 1) {
                label_msg.ForeColor = Color.Red;
                label_msg.Text = UILangUtil.getMsg("view.param.valid.msg1");// "Parameter name is mandatory ! ";
            } else {
                ValidationMsg msg = ModelManager.Instance.getValidMsg(input);
                if (msg.Type != MsgType.VALID) {
                    label_msg.ForeColor = Color.Red;
                    label_msg.Text = msg.Msg;
                } else {
                    label_msg.ForeColor = Color.Black;
                    label_msg.Text = "";
                }
            }            
        }

        private void ckb_pwd_CheckedChanged(object sender, EventArgs e) {
            if (ckb_pwd.Tag != this.input) {
                return;
            }
            if (ckb_pwd.Checked != this.input.Sensitive) {             
                this.input.Sensitive = ckb_pwd.Checked;             
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }            
        }        
        private void tb_des_KeyDown(object sender, KeyEventArgs e) {
            this.tb_des_updated = true;
            if (e.KeyCode == Keys.Enter) {
                this.cb_type.Focus();
            }
        }

        private void tb_des_TextChanged(object sender, EventArgs e) {
            if (this.tb_des_updated) {
                this.handleDesTextChanged();
            }
        }
        private void tb_des_Leave(object sender, EventArgs e) {
            handleDesTextChanged();
        }
        private void handleDesTextChanged() {
            if (this.input == null || tb_des.Text.Equals(Constants.BLANK_TEXT)) {
                return;
            }

            String desc = tb_des.Text.Trim();
            if ((desc.Length > 0 && !desc.Equals(input.Description))
                ||(desc.Length==0 && input.Description!=null && input.Description.Length!=0)) {
                input.Description = desc;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
        }
        private void cb_type_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_TypeFlag = 1;
        }
        private void cb_type_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_TypeSelectionChangedEvt();
        }

        private void doCB_TypeSelectionChangedEvt() {
            if (this.cb_TypeFlag == 0) {
                return;
            }
            UIUtils.updateComboBoxText(this.cb_type);
            
            if (cb_type.SelectedIndex == 3) { // Set selected 
                this.cb_setType.Enabled = true;

                this.cb_SetTypeFlag = 2;
                this.cb_setType.SelectedIndex = 0; // string type 
                this.doCB_SetTypeSelectionChangedEvt();

                this.cb_setAccess.Enabled = true;

                this.cb_SetAccessFlag = 2;
                this.cb_setAccess.SelectedIndex = 0; // loop access 
                this.doCB_SetAccSelectedIndexChangedEvt();

                this.btn_set.Enabled = true;
                this.rtb_value.ReadOnly = true;
                // update model 
                this.input.Type = ParamType.SET;
            } else {
                this.cb_setType.Enabled = false;
                
                this.cb_SetTypeFlag = 2;
                this.cb_setType.SelectedIndex = -1; // string type 
                this.doCB_SetTypeSelectionChangedEvt();

                this.cb_setAccess.Enabled = false;
                this.cb_SetAccessFlag = 2;
                this.cb_setAccess.SelectedIndex = -1; // 
                this.doCB_SetAccSelectedIndexChangedEvt();

                this.btn_set.Enabled = false;
                // update model 
                if (cb_type.SelectedIndex == 0) {
                    this.input.Type = ParamType.STRING;
                } else if (cb_type.SelectedIndex == 1) {
                    this.input.Type = ParamType.NUMBER;
                } else if (cb_type.SelectedIndex == 2) {
                    this.input.Type = ParamType.DATETIME;
                }
                this.rtb_value_updated = false;
                this.rtb_value.Text = string.Empty;
                this.rtb_value.ReadOnly = false;
            }
            if (this.cb_TypeFlag == 1) {
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
            // update validation msg
            updateValidationMsg();

            this.cb_TypeFlag = 0;
        }

        private void cb_setType_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_SetTypeFlag = 1;
        }
        private void cb_setType_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_SetTypeSelectionChangedEvt();
        }

        private void doCB_SetTypeSelectionChangedEvt() {
            if (this.cb_SetTypeFlag == 0) {
                return;
            }

            UIUtils.updateComboBoxText(this.cb_setType);

            int index = this.cb_setType.SelectedIndex;
            if (index >= 0 && index <= 1) {
                bool setTypeChanged = false;
                if (cb_setType.SelectedIndex == 0) {
                    if (this.input.SetType != ParamType.STRING) {
                        setTypeChanged = true;
                    }
                    this.input.SetType = ParamType.STRING;
                } else if (cb_setType.SelectedIndex == 1) {
                    if (this.input.SetType != ParamType.NUMBER) {
                        setTypeChanged = true;
                    }
                    this.input.SetType = ParamType.NUMBER;
                }
                if (setTypeChanged) {
                    if (this.input.DesignSet != null) {
                        this.input.DesignSet.Clear();
                    }
                    this.rtb_value.Text = Constants.BLANK_TEXT;

                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                    updateValidationMsg();
                }
            } else {
                this.cb_setType.Text = Constants.BLANK_TEXT;
            }
            
            this.cb_SetTypeFlag = 0;
        }

        private void cb_setAccess_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_SetAccessFlag = 1;
        }        
        private void cb_setAccess_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_SetAccSelectedIndexChangedEvt();
        }

        private void doCB_SetAccSelectedIndexChangedEvt() {
            if (this.cb_SetAccessFlag == 0) {
                return;
            }

            int index = this.cb_setAccess.SelectedIndex;
            if (index >= 0 && index <= 2) {
                bool changed = false;
                if (cb_setAccess.SelectedIndex == 0) {
                    if (this.input.SetCtrl != SET_ACCESS.LOOP) {
                        changed = true;
                    }
                    this.input.SetCtrl = SET_ACCESS.LOOP;
                } else if (cb_setAccess.SelectedIndex == 1) {
                    if (this.input.SetCtrl != SET_ACCESS.RANDOM_NO_DUPLICATE) {
                        changed = true;
                    }
                    this.input.SetCtrl = SET_ACCESS.RANDOM_NO_DUPLICATE;
                } else if (cb_setAccess.SelectedIndex == 2) {
                    if (this.input.SetCtrl != SET_ACCESS.RANDOM) {
                        changed = true;
                    }
                    this.input.SetCtrl = SET_ACCESS.RANDOM;
                }
                if (changed) {
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                }
            } else {
                this.cb_setAccess.Text = Constants.BLANK_TEXT;
            }

            this.cb_SetAccessFlag = 0;
        }

        private void rtb_value_KeyDown(object sender, KeyEventArgs e) {            
            this.rtb_value_updated = true;
        }

        private void rtb_value_TextChanged(object sender, EventArgs e) {
            if (this.rtb_value_updated) {                
                if (input.Type == ParamType.STRING || input.Type == ParamType.NUMBER || input.Type == ParamType.DATETIME) {
                    this.input.DesignValue = this.rtb_value.Text;
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);

                    updateValidationMsg();
                }
            }
        }
        #endregion ui methods 
        #region util methods
        private ValidationMsg getValidationMsg(BaseElement be) {
            ValidationMsg msg = ModelManager.Instance.getValidMsg(be);
            return msg;
        }

        /// <summary>
        /// get the type index of the combo box, or -1 if errors 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private int getParamTypeIndex(Parameter parameter) {
            if (parameter.Type == ParamType.STRING) {
                return 0;
            } else if (parameter.Type == ParamType.NUMBER) {
                return 1;
            } else if (parameter.Type == ParamType.DATETIME) {
                return 2;
            } else if (parameter.Type == ParamType.SET) {
                return 3;
            }
            return -1;
        }

        private int getSetAccessIndex() {
            if (input.Type == ParamType.SET) {
                if (input.SetCtrl == SET_ACCESS.LOOP) {
                    return 0;
                } else if (input.SetCtrl == SET_ACCESS.RANDOM_NO_DUPLICATE) {
                    return 1;
                } else if (input.SetCtrl == SET_ACCESS.RANDOM) {
                    return 2;
                }
            }
            return -1;
        }

        private int getSetTypeIndex() {
            if (input.Type == ParamType.SET) {
                if (input.SetType == ParamType.STRING) {
                    return 0;
                } else if (input.SetType == ParamType.NUMBER) {
                    return 1;
                }
            }
            return -1;
        }

        #endregion util methods                  

        private void btn_set_Click(object sender, EventArgs e) {
            ParamSetEditDialog dlg = new ParamSetEditDialog();
            DialogResult dr = dlg.showSetInputDialog(UIUtils.getTopControl(this), this.input,false);
            if (dr == DialogResult.OK) {
                List<object> items = dlg.SetItems;
                if (input.DesignSet == null) {
                    this.input.DesignSet = new List<object>();
                }
                this.input.DesignSet.Clear();
                this.input.DesignSet.AddRange(items);

                showSetValues();
            }
        }

        private void groupBox1_SizeChanged(object sender, EventArgs e) {
            if (grpSize.Width == int.MinValue || grpSize.Height == int.MinValue) {
                grpSize.Width = this.groupBox1.Width;
                grpSize.Height = this.groupBox1.Height;
                this.rtb_value.Height = this.groupBox2.Height - this.rtb_value.Location.Y - this.groupBox2.Padding.Bottom;
                this.rtb_value.Width = this.groupBox2.Width - this.rtb_value.Location.X - this.groupBox2.Padding.Right;
                return;
            }
            int dw = this.groupBox1.Size.Width - grpSize.Width;
            this.tb_des.Width = this.tb_des.Width + dw;
            this.tb_name.Width = this.tb_name.Width + dw;
            this.cb_type.Width = this.cb_type.Width + dw;
            int dh = this.groupBox1.Size.Height - grpSize.Height;
            this.rtb_value.Height = this.rtb_value.Height + dh;

            this.grpSize.Width = this.groupBox1.Width;
            this.grpSize.Height = this.groupBox1.Height;
        }

    }
}
