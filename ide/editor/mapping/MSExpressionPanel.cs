using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor.mapping
{
    public partial class MSExpressionPanel : UserControl,IMappingSrc
    {
        #region variables 
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int cb_op_flag = 0;
        /// <summary>
        /// Mapping source type, this is the final type of the mapping source 
        /// </summary>
        private ParamType srcType = ParamType.STRING;
        /// <summary>
        /// This is the stub operation/Process of the mapping source .
        /// a stub operation is the operation who triggerred the update parameter mapping 
        /// or OpCondition's owner that OpCondition triggerred the parameters mapping 
        /// </summary>
        private Operation stubOp = null;
        /// <summary>
        /// Script Root 
        /// </summary>
        private ScriptRoot sroot = null;       
        /// <summary>
        /// Mapping source object, to be returned expresion object for invoker. 
        /// </summary>
        private Expression outputExp = null;
        #endregion variables 
        #region events
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping Expression
        /// </summary>
        public event EventHandler<CommonEventArgs> MappingSrcChangedEvt;
        protected virtual void OnMappingSrcChangedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> mappingSrcChangedEvt = MappingSrcChangedEvt;
            if (mappingSrcChangedEvt != null) {
                mappingSrcChangedEvt(this, e);
            }
        }
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping Expression
        /// </summary>
        /// <param name="sender">IMappingSrc panel</param>
        /// <param name="obj">Expression</param>
        public void raiseMappingSrcChangedEvt(Object sender, object obj) {
            if (obj != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnMappingSrcChangedEvt(evt);
            }
        }
        #endregion events
        public MSExpressionPanel() {
            InitializeComponent();
            this.rtb_exp.ForeColor = Color.FromName(UIConstants.COLOR_MAPPING_EXP_TEXT);
        }
        #region mandatory methods
        public string getExpression() {
            if (isValid()) {
                return ModelManager.Instance.getMappingSrcText(this.outputExp);
            } else {
                return string.Empty;
            }
        }

        public string getValidMsg() {
            string msg = null;            
            if (this.stubOp == null || sroot == null) {
                msg = UILangUtil.getMsg("mapping.src.exp.err.msg0");
            } else if (this.outputExp == null) {
                msg = UILangUtil.getMsg("mapping.src.exp.err.msg1");
            } else {
                if (srcType == ParamType.STRING) {
                    if (ModelManager.Instance.isMaybeStringValue(this.outputExp.Input1)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.exp.err.msg2");
                    }
                    if (ModelManager.Instance.isMaybeStringValue(this.outputExp.Input2)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.exp.err.msg3");
                    }
                } else if (srcType == ParamType.NUMBER) {
                    if (ModelManager.Instance.isMaybeNumberValue(this.outputExp.Input1)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.exp.err.msg4");
                    }
                    if (ModelManager.Instance.isMaybeNumberValue(this.outputExp.Input2)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.exp.err.msg5");
                    }
                } else if (srcType == ParamType.DATETIME) {
                    if (ModelManager.Instance.isMaybeTimeValue(this.outputExp.Input1)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.exp.err.msg6");
                    }
                    if (ModelManager.Instance.isMaybeTimeValue(this.outputExp.Input2)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.exp.err.msg7");
                    }
                } else {
                    msg = UILangUtil.getMsg("mapping.src.const.err.msg3");
                }
            }
            return msg;
        }

        public bool isValid() {
            return getValidMsg() == null;
        }
        /// <summary>
        /// Return the mapping expression source or null if errors 
        /// </summary>
        /// <returns></returns>
        public object getMappingSrc() {
            if (isValid()) {
                return this.outputExp;
            }
            return null;
        }

        public void show(object src, ParamType srcType) {
            // update mapping source type info 
            updateSrcType(srcType);
            // update view with input 
            setInput(src);
            this.Visible = true;
        }

        public void hidden() {
            this.Visible = false;
        }
        #endregion mandatory methods   
        #region common methods 
        /// <summary>
        /// set the stub operation, a stub operation is the operation who triggerred the update parameter mapping 
        /// or OpCondition's owner that OpCondition triggerred the parameters mapping 
        /// </summary>
        /// <param name="op">stub operation</param>
        /// <param name="sroot"></param>
        public void setStubOp(Operation op, ScriptRoot sroot) {
            this.stubOp = op;
            this.sroot = sroot;
        }
        private void setInput(object src) {            
            // initialize update outputExp
            Expression inputExp = null;
            if (src is Expression) {
                inputExp = src as Expression;   
            }
            this.outputExp = createAndInitOutput(inputExp);
            this.outputExp.Type = this.srcType;

            // update cb_op box 
            initCB_OpBox();
            // update inputs area 
            updateInputArea();
            // update validation message 
            this.lb_msg.Text = getValidMsg();
            // update expression msg            
            this.rtb_exp.Text = getExpression();            
        }
        /// <summary>
        /// Create an Expression and initialize it the same as the inputExp. 
        /// </summary>
        private Expression createAndInitOutput(Expression inputExp) {
            Expression exp = null;
            if (inputExp != null) {
                exp = inputExp.Clone();
            } else {
                exp = ModelFactory.createExpression();
            }
            return exp;
        }

        private void updateInputArea() {
            this.tb_left.Text = string.Empty;
            this.tb_right.Text = string.Empty;
            
            if (this.outputExp != null) {
                this.tb_left.Text = ModelManager.Instance.getMappingSrcText(this.outputExp.Input1);
                this.tb_right.Text = ModelManager.Instance.getMappingSrcText(this.outputExp.Input2);
            }
        }

        private void initCB_OpBox() {
            // build up the cb_op box items 
            buildCB_OPBoxItems();
            // set the selected item 
            setSelectedCB_OPOBX();
        }

        private void setSelectedCB_OPOBX() {
            if (cb_op.Items.Count > 0) {                
                int index = 0;
                if (this.outputExp != null && 
                    (this.outputExp.Type == ParamType.NUMBER ||this.outputExp.Type == ParamType.DATETIME)) {
                    if ("+"==this.outputExp.Operator) {
                        index = 0;
                    } else if ("-" == this.outputExp.Operator) {
                        index = 1;
                    } else if ("*" == this.outputExp.Operator) {
                        index = 2;
                    } else if ("/" == this.outputExp.Operator) {
                        index = 3;
                    }
                }
                this.cb_op_flag = 0;
                this.cb_op.SelectedIndex = index;
                UIUtils.updateComboBoxText(this.cb_op);
            }
        }

        private void buildCB_OPBoxItems() {
            this.cb_op.Items.Clear();
            
            if (this.outputExp.Type == ParamType.NUMBER) {
                this.cb_op.Items.Add("+");
                this.cb_op.Items.Add("-");
                this.cb_op.Items.Add("*");
                this.cb_op.Items.Add("/");
            } else if (this.outputExp.Type == ParamType.STRING) {
                this.cb_op.Items.Add("+");
            } else if (this.outputExp.Type == ParamType.DATETIME) {
                this.cb_op.Items.Add("+");
                this.cb_op.Items.Add("-");
            }
        }

        private void updateSrcType(ParamType srcType) {
            if (srcType == ParamType.STRING || srcType == ParamType.NUMBER) {
                this.srcType = srcType;
            }
        }
        private void doCB_op_SelectedIndexChanged() {
            if (this.cb_op_flag == 0) {
                return;
            }
            // update combo box text 
            UIUtils.updateComboBoxText(this.cb_op);
            // update Operator
            if (this.cb_op.Text != this.outputExp.Operator) {
                outputExp.Operator = this.cb_op.Text;                
            }
            handleOutputChanged();
            this.cb_op_flag = 0;
        }
        /// <summary>
        /// Update message text, expression text and raiseMappingSrcChangedEvt. 
        /// </summary>
        private void handleOutputChanged() {
            // update validation msg 
            this.lb_msg.Text = getValidMsg();
            // update expression 
            this.rtb_exp.Text = getExpression();

            this.raiseMappingSrcChangedEvt(this, this.getMappingSrc());
        }       
        #endregion common methods 
        #region UI events 
        private void cb_op_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_op_flag = 1;
        }
        private void cb_op_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_op_SelectedIndexChanged();
        }

        private void bt_left_Click(object sender, EventArgs e) {
            MappingParamSelectionDlg dlg = new MappingParamSelectionDlg();
            DialogResult dr = dlg.showSelectionDlg(UIUtils.getTopControl(this), this.stubOp, this.sroot, this.outputExp.Input1, this.srcType);
            if (dr == DialogResult.OK) {
                object newinput = dlg.Output;
                if (newinput != this.outputExp.Input1) {
                    this.outputExp.Input1 = newinput;
                    this.tb_left.Text = ModelManager.Instance.getMappingSrcText(this.outputExp.Input1);
                    handleOutputChanged();
                }                
            }            
        }

        private void bt_right_Click(object sender, EventArgs e) {
            MappingParamSelectionDlg dlg = new MappingParamSelectionDlg();
            DialogResult dr = dlg.showSelectionDlg(UIUtils.getTopControl(this), this.stubOp, this.sroot, this.outputExp.Input2, this.srcType);
            if (dr == DialogResult.OK) {
                object newinput = dlg.Output;
                if (newinput != this.outputExp.Input2) {
                    this.outputExp.Input2 = newinput;
                    this.tb_right.Text = ModelManager.Instance.getMappingSrcText(this.outputExp.Input2);
                    handleOutputChanged();
                }
            }
        }
        private void panel3_Paint(object sender, PaintEventArgs e) {
            ControlPaint.DrawBorder(e.Graphics, this.panel3.ClientRectangle, Color.FromName(UIConstants.COLOR_MAPPING_EXP_BORDER), ButtonBorderStyle.Solid);
        }
        #endregion UI events         
    }
}
