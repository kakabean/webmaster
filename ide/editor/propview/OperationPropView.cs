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
using WebMaster.ide.editor.mapping;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    public partial class OperationPropView : UserControl, IPropView
    {
        private Operation input = null;
        private ScriptRoot sroot = null;
        private FlowPropViewManager flowPVManager = null;

        private bool tb_name_updated = false;
        private bool tb_des_updated = false;
        private bool rtb_input_updated = false;
        /// <summary>
        /// operation input types text with locale info 
        /// </summary>
        private List<string> opInputTypes = null;
        private List<string> openURLTypes = null;
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int cb_OpenURLTypeFlag = 0;
        private int cb_waittimeFlag = 0;
        private int cb_inputTypeFlag = 0;
        /// <summary>
        /// If the flag is false, value changed event will ignore raiseModelChanged evt. 
        /// </summary>
        private bool tb_waittime_flag = false;
        private bool tb_waittime1_flag = false;
        /// <summary>
        /// this is trick variable to make sure the i18n worked at design time
        /// </summary>
        private bool firstInitData = true;

        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { flowPVManager = value; }
        }

        public OperationPropView() {
            InitializeComponent();
            initData();
        }

        private void initData() {
            this.cb_OpenURLType.Visible = false;
            this.cb_OpenURLType.Dock = DockStyle.Fill;
            this.tb_opInputWE.Visible = true;
            this.tb_opInputWE.Dock = DockStyle.Fill;
            
            //// update editParameter button location, it is strange that its location 
            //// is not same like it is set in the UI Designer at runtime, while others buttons are ok
            //int x = this.groupBox2.ClientRectangle.Width - 6-this.btn_updateParam.Size.Width;
            //this.btn_updateParam.Location = new Point(x, this.btn_updateParam.Location.Y);
        }
        /// <summary>
        /// Try to make sure the UI data was only initialized once. it is added to fix the 
        /// i18n design problem. If below code added into initData() method, it will
        /// some errors in design time when the propview created.
        /// </summary>
        public void tryInitDataOnlyOnce() {
            if (this.firstInitData) {
                this.firstInitData = false;

                this.opInputTypes = ModelManager.Instance.getOperationInputTypes();
                this.cb_inputType.Items.AddRange(opInputTypes.ToArray());

                this.openURLTypes = ModelManager.Instance.getOpenURLTypes();
                this.cb_OpenURLType.Items.AddRange(this.openURLTypes.ToArray());
                
                this.cb_OpenURLTypeFlag = 2;
                this.cb_OpenURLType.SelectedIndex = 0;
                this.doCB_OpenURLTypeSelectedChangedEvt();
            }
        }

        public void setScriptRoot(ScriptRoot sroot) {
            this.sroot = sroot;
        }
        #region events
        /// <summary>
        /// raise the event for outer properties control that an Element validation error
        /// the sender is property view, data is Validation element, msg is validatoin info
        /// 
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
            if (data != null && string.Empty != msg) {
                ValidationArgs evt = new ValidationArgs(sender, data, type, msg);
                OnElementValidationEvt(evt);
            }
        }
        #endregion events 
        #region madatory methods 
        public void updatedInput() {
            if (input is Operation || input is Process) {
                Operation op = (Operation)input;
                if (op != null) {
                    if (tb_name.Text != null && tb_name.Text.Trim().Length > 0) {
                        // here is a maybe valid name
                        if (label_msg.Text != null && label_msg.Text.Length < 1) {
                            op.Name = this.tb_name.Text;
                        }
                    }
                    op.Description = this.tb_des.Text;
                    op.WaitTime = getWaitTimeValue();                    
                    op.Input = this.rtb_input.Text;
                }
                if (op is Process) {
                    groupBox1.Text = "Process";
                } else {
                    groupBox1.Text = "Operation";
                }
            } else {
                cleanView();
            }
        }
        public void setInput(object input) {
            cleanView();
            if (input is Operation || input is Process) {
                Operation op = (Operation)input;
                this.input = op;
                
                // handle start Node and End node
                if (op.OpType == OPERATION.START || op.OpType == OPERATION.END) {
                    disableView();
                    this.tb_name_updated = false;
                    this.tb_name.Text = op.Name;
                    this.tb_waittime_flag = false;
                    this.tb_waittime.Text = op.WaitTime;
                } else {
                    // handle normal operation node 
                    enableView();
                    if (/*op.OpType == OPERATION.NOP || */op.OpType == OPERATION.PROCESS) {
                        this.bt_searchWE.Enabled = false;
                    }
                    this.tb_name_updated = false;
                    if (this.input.Name != null && this.input.Name.Trim().Length > 0) {
                        this.tb_name.ForeColor = Color.Black;
                        this.tb_name.Text = op.Name;
                    } else {
                        this.tb_name.ForeColor = Color.Red;
                        this.tb_name.Text = UILangUtil.getMsg("valid.be.name.msg1");// Name is mandatory
                    }
                    this.tb_des_updated = false;
                    this.tb_des.Text = op.Description;
                    updateWaitTimeArea();
                    string inputText = op.Input != null ? op.Input.ToString() : "";
                    this.rtb_input_updated = false;
                    this.rtb_input.Text = inputText;
                    
                    this.cb_inputTypeFlag = 2;
                    this.cb_inputType.SelectedIndex = getInputTypeIndex(op);
                    this.doCB_InputTypeSelectedIndexChangedEvt();

                    string wename = op.Element != null ? op.Element.Name : "";
                    this.tb_opInputWE.Text = wename;

                    // update error msg 
                    ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(op, "Operation - ");
                    if (msg.Type != MsgType.VALID) {
                        this.label_msg.ForeColor = Color.Red;
                        this.label_msg.Text = msg.Msg;
                    }
                }
                if (op is Process) {
                    groupBox1.Text = UILangUtil.getMsg("view.op.grp1.text1");
                } else {
                    groupBox1.Text = UILangUtil.getMsg("view.op.grp2.text2");
                }
                // update update param button
                if (this.input == this.FlowPVManager.Bigmodel.SRoot.ProcRoot) {
                    this.btn_updateParam.Enabled = false;
                } else {
                    this.btn_updateParam.Enabled = true;
                }
                updateExeTimeArea();
                adjustUIByOpType();
                showView();
            } else {
                this.input = null;
                this.disableView();
            }
        }

        private void adjustUIByOpType() {
            if (input == null) {
                return;
            }
            this.rtb_input.Enabled = false;
            this.cb_inputType.Enabled = false;
            this.bt_input.Enabled = false;
            
            if (input.OpType == OPERATION.OPEN_URL_N_T || input.OpType == OPERATION.OPEN_URL_T) {
                this.rtb_input.Enabled = true;
                this.cb_inputType.Enabled = true;
                this.bt_input.Enabled = true;

                this.label3.Text = UILangUtil.getMsg("view.op.label3.text2");
                this.tb_opInputWE.Visible = false;
                this.bt_searchWE.Visible = false;
                this.cb_OpenURLType.Visible = true;
            } else {
                this.label3.Text = UILangUtil.getMsg("view.op.label3.text1");
                this.cb_OpenURLType.Visible = false;
                this.bt_searchWE.Visible = true;
                this.tb_opInputWE.Visible = true;    
            }

            if (input.OpType == OPERATION.INPUT) {
                this.rtb_input.Enabled = true;
                this.cb_inputType.Enabled = true;
                this.bt_input.Enabled = true;
            } else if (input.OpType == OPERATION.NOP) {                
                this.cb_inputTypeFlag = 2;
                this.cb_inputType.SelectedIndex = 1;
                this.doCB_InputTypeSelectedIndexChangedEvt();
                
                this.rtb_input.Enabled = true;
                this.bt_input.Enabled = true;
            }
        }
        
        public object getInput() {
            return input;
        }

        public void cleanView() {
            this.tb_name_updated = false;
            this.tb_name.Text = Constants.BLANK_TEXT;
            this.tb_des_updated = false;
            this.tb_des.Text = Constants.BLANK_TEXT;
            this.tb_opInputWE.Text = Constants.BLANK_TEXT;
            this.tb_waittime_flag = false;
            this.tb_waittime.Text = Constants.BLANK_TEXT;
            this.tb_waittime1_flag = false;
            this.tb_waittime1.Text = Constants.BLANK_TEXT;
            this.rtb_input_updated = false;
            this.rtb_input.Text = Constants.BLANK_TEXT;
            this.cb_inputType.Text = opInputTypes[0];
            this.label_msg.Text = "";
            this.btn_updateParam.Enabled = false;
        }

        public void showView() {
            this.Visible = true;
        }

        public void hideView() {
            this.Visible = false;
        }

        public void enableView() {
            this.tb_name.Enabled = true ;
            this.tb_des.Enabled = true;
            this.tb_opInputWE.Enabled = true ;
            this.tb_waittime.Enabled = true;
            this.tb_waittime1.Enabled = true;
            this.cb_waittime.Enabled = true;
            this.rtb_input.Enabled = true;
            this.cb_inputType.Enabled = true;
            this.bt_searchWE.Enabled = true;            
            this.btn_updateParam.Enabled = true;
            // filter that if the object is Root Process, disable update Paramter function. 
            if (this.input is Process) {
                Process proc = ModelManager.Instance.getOwnerProc(this.input as Process);
                if (proc == null) {
                    this.btn_updateParam.Enabled = false;
                }
            }
            this.ckb_time.Enabled = true;
            this.nud_hour.Enabled = true;
            this.nud_min.Enabled = true;
            this.nud_sec.Enabled = true;
        }

        public void disableView() {
            this.tb_name.Enabled = false;
            this.tb_name_updated = false;
            this.tb_name.Text = Constants.BLANK_TEXT;
            this.tb_des.Enabled = false;
            this.tb_des_updated = false;
            this.tb_des.Text = Constants.BLANK_TEXT;
            this.bt_searchWE.Enabled = false;
            this.btn_updateParam.Enabled = false;
            this.tb_opInputWE.Enabled = false;
            this.tb_opInputWE.Text = "";
            this.tb_waittime.Enabled = false;
            this.tb_waittime_flag = false;
            this.tb_waittime.Text = Constants.BLANK_TEXT;
            this.tb_waittime1_flag = false;
            this.tb_waittime1.Text = Constants.BLANK_TEXT;
            this.tb_waittime1.Enabled = false;
            this.cb_waittime.Enabled = false;
            this.rtb_input.Enabled = false;
            this.rtb_input_updated = false;
            this.rtb_input.Text = Constants.BLANK_TEXT;
            this.cb_inputType.Enabled = false;
            if (this.opInputTypes != null && this.opInputTypes.Count > 0) {
                this.cb_inputType.Text = this.opInputTypes[0];
            }
            this.ckb_time.Enabled = false;
            this.nud_hour.Enabled = false;
            this.nud_hour.Value = 0;
            this.nud_min.Enabled = false;
            this.nud_min.Value = 0;
            this.nud_sec.Enabled = false;
            this.nud_sec.Value = 0;
        }
        
        #endregion mandatory methods 
        #region ui methods 
        private string getWaitTimeValue() {
            if (cb_waittime.SelectedIndex == 0) {
                return tb_waittime.Text;
            } else if (cb_waittime.SelectedIndex == 1) {
                return tb_waittime.Text.Trim() + " .. " + tb_waittime1.Text.Trim();
            }
            return "1";
        }

        private int getInputTypeIndex(Operation op) {
            if (op.Input is Parameter) {
                return 1;
            }else{
                return 0;
            }
        }

        private void updateExeTimeArea() {
            this.ckb_time.Checked = false;            
            if (ModelManager.Instance.isValidTime(this.input.ExeuteTime)) { 
                string[] ss = this.input.ExeuteTime.Split(':');
                this.nud_hour.Value = int.Parse(ss[0]);
                this.nud_min.Value = int.Parse(ss[1]);
                this.nud_sec.Value = int.Parse(ss[2]);
                this.ckb_time.Checked = true;
            }
        }

        /// <summary>
        /// update the wait time area based on the value 
        /// </summary>
        private void updateWaitTimeArea() {
            int[] ns = this.input.getWaitTimes();

            this.cb_waittimeFlag = 2;
            this.cb_waittime.SelectedIndex = ns.Length - 1;
            this.doCB_WaittimeSelectionIndexChangedEvt();

            if (ns.Length == 1) {
                this.tb_waittime1.Visible = false;
                this.label7.Visible = false;
                this.tb_waittime_flag = false;
                this.tb_waittime.Text = ns[0].ToString();
                int w = this.rtb_input.Width;
                this.tb_waittime.Size = new Size(w, tb_waittime.Height);
            } else if (ns.Length == 2) {
                int w = (this.rtb_input.Width - label7.Width)/2;
                tb_waittime.Size = new Size(w, tb_waittime.Height);
                label7.Visible = true;
                label7.Location = new Point(this.tb_waittime.Location.X+w, label7.Location.Y);
                tb_waittime1.Visible = true;
                this.tb_waittime1.Location = new Point(label7.Location.X + label7.Width, this.tb_waittime1.Location.Y);
                this.tb_waittime1.Size = new Size(w, this.tb_waittime1.Size.Height);
                this.tb_waittime_flag = false;
                this.tb_waittime.Text = ns[0].ToString();
                this.tb_waittime1_flag = false;
                this.tb_waittime1.Text = ns[1].ToString();
            }
        }

        private int getWaitTimeTypeIndex(string time) {
            if (time != null && time.Contains("..")) {
                return 1;
            } else {
                return 0;
            }
        }
        private void cb_inputType_SelectedIndexChanged(object sender, EventArgs e) {
            this.doCB_InputTypeSelectedIndexChangedEvt();
        }
        private void cb_inputType_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_inputTypeFlag = 1;
        }
        private void doCB_InputTypeSelectedIndexChangedEvt() {
            if (this.cb_inputTypeFlag == 0) {
                return;
            }
            // update text, some times text is not updated
            int sIndex = this.cb_inputType.SelectedIndex ;
            if (sIndex == -1) {
                this.cb_inputType.Text = Constants.BLANK_TEXT;
            } else {
                this.cb_inputType.Text = this.cb_inputType.Items[sIndex].ToString();
            }
            
            if (cb_inputType.SelectedIndex == 1) {
                // Parameter type selected 
                bt_input.Visible = true;
                rtb_input.ReadOnly = true;
                if (this.cb_inputTypeFlag == 1) {
                    rtb_input.Text = Constants.BLANK_TEXT;
                }
            } else {
                bt_input.Visible = false;
                rtb_input.ReadOnly = false;
                if (this.cb_inputTypeFlag == 1) {
                    rtb_input.Text = Constants.BLANK_TEXT;
                }
            }

            this.cb_inputTypeFlag = 0;
        }        

        private void cb_waittime_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_waittimeFlag = 1;
        }

        private void cb_waittime_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_WaittimeSelectionIndexChangedEvt();
        }
        private void doCB_WaittimeSelectionIndexChangedEvt() {
            if (cb_waittimeFlag == 0) {
                return;
            }
            int sIndex = cb_waittime.SelectedIndex;
            // update combo box text 
            if (sIndex == -1) {
                this.cb_waittime.Text = Constants.BLANK_TEXT;
            } else {
                this.cb_waittime.Text = this.cb_waittime.Items[sIndex].ToString();
            }

            if (cb_waittime.SelectedIndex == 0) {
                label7.Visible = false;
                tb_waittime1.Visible = false;
                // wait 100 milli-seconds 
                tb_waittime.Value = 100;                
                int w = this.rtb_input.Width;
                tb_waittime.Size = new Size(w, tb_waittime.Height);
            } else if (cb_waittime.SelectedIndex == 1) {
                int w = (this.rtb_input.Width - label7.Width) / 2;
                tb_waittime.Size = new Size(w, tb_waittime.Height);
                label7.Visible = true;
                label7.Location = new Point(this.tb_waittime.Location.X + w, label7.Location.Y);
                tb_waittime1.Visible = true;
                this.tb_waittime1.Location = new Point(label7.Location.X + label7.Width, this.tb_waittime1.Location.Y);
                this.tb_waittime1.Size = new Size(w, this.tb_waittime1.Size.Height);
                
                this.tb_waittime.Value = 100;
                this.tb_waittime1.Value = 500;
            }
            
            cb_waittimeFlag = 0;            
        }

        private void tb_waittime_KeyDown(object sender, KeyEventArgs e) {
            this.tb_waittime_flag = true;
        }

        private void tb_waittime_ValueChanged(object sender, EventArgs e) {
            string text = string.Empty ;
            if (this.cb_waittime.SelectedIndex == 0) {
                text = this.tb_waittime.Value.ToString();                
            } else if (this.cb_waittime.SelectedIndex == 1) {
                text = this.tb_waittime.Value + ".." + this.tb_waittime1.Value;
            }
            if (this.tb_waittime_flag && isWaitTimeChanged(this.input, text)) {
                this.input.WaitTime = text;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
            this.tb_waittime_flag = true;
        }

        private void tb_waittime1_KeyDown(object sender, KeyEventArgs e) {
            this.tb_waittime1_flag = true;
        }
        private void tb_waittime1_ValueChanged(object sender, EventArgs e) {
            string text = string.Empty;
            if (this.cb_waittime.SelectedIndex == 1) {
                text = this.tb_waittime.Value + ".." + this.tb_waittime1.Value;
            }
            if (this.tb_waittime1_flag && isWaitTimeChanged(this.input, text)) {
                this.input.WaitTime = text;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
            this.tb_waittime1_flag = true;
        }
        /// <summary>
        /// Check whether the new waittime text is changed 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool isWaitTimeChanged(Operation operation, string text) {
            bool result = false;
            if (operation != null && text != null && text.Trim().Length > 0) {
                if (!text.Equals(operation.WaitTime)) {
                    result = true;
                }
            }
            return result;
        }

        private void bt_searchWE_Click(object sender, EventArgs e) {
            if (sroot == null) {
                label_msg.ForeColor = SystemColors.HotTrack;
                label_msg.Text = UILangUtil.getMsg("view.op.we.err.msg1");// "Error when show the WebElement, sroot is null";
            } else {
                WEListDialog dlg = new WEListDialog();
                string msg = UILangUtil.getMsg("view.op.we.dlg.msg"); // Please choose a WebElement as Operation input
                DialogResult dr = dlg.showWEDialog(UIUtils.getTopControl(this), msg, this.sroot);
                if (dr == DialogResult.OK) {
                    WebElement we = dlg.SelectedWE;
                    String name = we == null ? "" : we.Name;
                    tb_opInputWE.Text = name;
                    
                    // update model 
                    input.Element = we;                    
                    updateValidationMsg(input);
                    // fire model updated event
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                }
            }
        }

        private void updateValidationMsg(Operation input) {
            ValidationMsg vmsg = ModelManager.Instance.getValidMsg(input);
            if (vmsg.Type == MsgType.ERROR) {
                if (vmsg.Msg != null && string.Empty != vmsg.Msg) {
                    this.label_msg.ForeColor = Color.Red;
                    this.label_msg.Text = vmsg.Msg;
                } else {
                    this.label_msg.ForeColor = Color.Black;
                    this.label_msg.Text = "";
                }
            }
        }
        private void tb_name_KeyUp(object sender, KeyEventArgs e) {
            //if (e.KeyCode == Keys.Enter) {
            //    this.tb_des.Focus();
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
        private void tb_name_Leave(object sender, EventArgs e) {
            handleNameTextChanged();
        }
        private void handleNameTextChanged() {
            if (tb_name.Text.Trim().Length < 1 || tb_name.Equals(Constants.BLANK_TEXT)) {
                label_msg.ForeColor = Color.Red;
                label_msg.Text = UILangUtil.getMsg("valid.be.name.msg1");// Name is mandatory
                return;
            }
            string newn = tb_name.Text.Trim();
            if (newn.Equals(this.input.Name)) {
                return;
            }

            string tn = this.input.Name;
            this.input.Name = newn;
            ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(input, "");

            if (msg.Type == MsgType.VALID) {
                this.label_msg.Text = "";
                FlowPVManager.raiseInputUpdatedEvt(this, input);
            } else {
                this.input.Name = tn;
                label_msg.ForeColor = Color.Red;
                this.label_msg.Text = msg.Msg;
            }
        }
        private void tb_des_KeyUp(object sender, KeyEventArgs e) {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Enter) {                
                this.bt_searchWE.Focus();
            }
        }
        private void tb_des_KeyDown(object sender, KeyEventArgs e) {
            this.tb_des_updated = true;
        }
        private void tb_des_TextChanged(object sender, EventArgs e) {
            if (tb_des_updated) {
                handleDesTextChanged();
            }
        }
        private void tb_des_Leave(object sender, EventArgs e) {
            handleDesTextChanged();
        }
        private void handleDesTextChanged() {
            Operation op = this.input as Operation;
            if (op != null) {
                if (this.tb_des.Text.Length>0 && !this.tb_des.Text.Equals(Constants.BLANK_TEXT)) {
                    string ndes = this.tb_des.Text;
                    if (!ndes.Equals(op.Description)) {
                        this.input.Description = ndes;
                        FlowPVManager.raiseInputUpdatedEvt(this, op);
                    }
                }                
            }
        }
        private void rtb_input_KeyDown(object sender, KeyEventArgs e) {
            this.rtb_input_updated = true;
        }
        private void rtb_input_TextChanged(object sender, EventArgs e) {
            if (rtb_input_updated) {
                this.handleInputTextChanged();
            }
        }
        private void rtb_input_Leave(object sender, EventArgs e) {
            handleInputTextChanged();
        }        
        private void handleInputTextChanged(){
            Operation op = this.input as Operation;
            if (op != null && !this.rtb_input.Text.Equals(Constants.BLANK_TEXT) && cb_inputType.Text.Equals(this.opInputTypes[0])) {
                if (!this.rtb_input.Text.Equals(op.Input)) {
                    op.Input = this.rtb_input.Text;
                    FlowPVManager.raiseInputUpdatedEvt(this, input);
                }
            }
        }

        private void bt_input_Click(object sender, EventArgs e) {
            ConditionInputDialog dlg = new ConditionInputDialog();
            DialogResult dr = dlg.showOpInputDialog(this.Parent.Parent.Parent.Parent.Parent.Parent, "Operation input dialog", this.FlowPVManager.Bigmodel.SRoot, this.input);
            if (dr == DialogResult.OK) {                
                this.input.Input = dlg.SelectedObj;

                FlowPVManager.raiseInputUpdatedEvt(this, input);
                showParamterText(this.input.Input);
                
                updateValidationMsg(input);
            }            
        }

        private void showParamterText(object p) {
            if (p is Parameter) {
                Parameter param = p as Parameter;
                //this.rtb_input.SelectionColor = Color.Blue;
                //this.rtb_input.AppendText("Parameter - " + param.Type + " : "+Environment.NewLine);
                this.rtb_input.Text = string.Empty;
                this.rtb_input.SelectionColor = Color.Black;
                this.rtb_input.AppendText(param.ToString());
            }
        }

        private void cb_OpenURLType_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_OpenURLTypeFlag = 1;
        }
        /// <summary>
        /// take effect if the operation type is OPEN_URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_OpenURLType_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_OpenURLTypeSelectedChangedEvt();
        }

        private void doCB_OpenURLTypeSelectedChangedEvt() {
            if (cb_OpenURLTypeFlag == 0) {
                return;
            }
            int sIndex = cb_OpenURLType.SelectedIndex;
            // make sure text updated 
            if (sIndex == -1) {
                this.cb_OpenURLType.Text = Constants.BLANK_TEXT;
            } else { 
                this.cb_OpenURLType.Text = this.cb_OpenURLType.Items[sIndex].ToString();
            }

            Operation op = this.input as Operation;
            if (op != null && (op.OpType == OPERATION.OPEN_URL_T || op.OpType == OPERATION.OPEN_URL_N_T)) {
                OPERATION type = getOpenURLType() ;
                if (op.OpType != type) {
                    op.OpType = type;
                    FlowPVManager.raiseInputUpdatedEvt(this, input);
                }
            }

            cb_OpenURLTypeFlag = 0;
        }
        /// <summary>
        /// default return the open url in current tab. 
        /// </summary>
        /// <returns></returns>
        private OPERATION getOpenURLType() {
            if (cb_OpenURLType.Text.Equals(this.openURLTypes[1])) {
                return OPERATION.OPEN_URL_N_T;
            } else {
                return OPERATION.OPEN_URL_T;
            }
        }
        #endregion ui methods        

        private void btn_updateParam_Click(object sender, EventArgs e) {
            ParamMappingDlg dlg = new ParamMappingDlg();
            DialogResult dr = dlg.showMappingDlg(UIUtils.getTopControl(this), this.input, this.FlowPVManager.Bigmodel.SRoot);
            if (dr == DialogResult.OK) {                
                this.input.Commands.Clear();
                this.input.Commands.AddRange(dlg.AllMappings);
                this.FlowPVManager.raiseInputUpdatedEvt(this, this.input);
            }
            //Console.WriteLine("group size = " + this.groupBox2.ClientRectangle);
            //Console.WriteLine("Up.loc = "+this.btn_updateParam.Location+"up.size = "+this.btn_updateParam.Size);
            //Console.WriteLine("inputType.loc = " + this.cb_inputType.Location + "inputType.size = " + this.cb_inputType.Size);
            //Console.WriteLine("wt.loc = " + this.cb_waittime.Location + "wt.size = " + this.cb_waittime.Size);
            //Console.WriteLine("edit.loc = " + this.bt_searchWE.Location + "edit.size = " + this.bt_searchWE.Size);
        }

        private void ckb_time_CheckedChanged(object sender, EventArgs e) {
            if (this.ckb_time.Checked) {
                this.nud_hour.Enabled = true;                
                this.nud_min.Enabled = true;                
                this.nud_sec.Enabled = true;                
            } else {
                this.nud_hour.Enabled = false;
                this.nud_min.Enabled = false;
                this.nud_sec.Enabled = false;
            }
            this.nud_sec.Value = 0;
            this.nud_min.Value = 0;
            this.nud_hour.Value = 0;
        }

        private void nud_hour_ValueChanged(object sender, EventArgs e) {
            doExeTimeUpdated();            
        }
        
        private void nud_min_ValueChanged(object sender, EventArgs e) {
            doExeTimeUpdated();
        }

        private void nud_sec_ValueChanged(object sender, EventArgs e) {
            doExeTimeUpdated();
        }

        private void doExeTimeUpdated() {
            if (ckb_time.Checked == false) {
                return;
            }
            string str = nud_hour.Value + ":" + nud_min.Value + ":" + nud_sec.Value;
            if (str != this.input.ExeuteTime) {
                this.input.ExeuteTime = str;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
        }
    }
}
