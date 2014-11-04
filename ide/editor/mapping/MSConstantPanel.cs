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
    /// <summary>
    /// Mapping source constant panel 
    /// </summary>
    public partial class MSConstantPanel : UserControl,IMappingSrc
    {    
        #region variables 
        /// <summary>
        /// Mapping source type, this is the final type of the mapping source 
        /// </summary>
        private ParamType srcType = ParamType.STRING;        
        /// <summary>
        /// Mapping source object, to be returned src constant for invoker. 
        /// </summary>
        private object outputConst = null;
        #endregion variables 
        #region events
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping src data(String or Number).
        /// </summary>
        public event EventHandler<CommonEventArgs> MappingSrcChangedEvt;
        protected virtual void OnMappingSrcChangedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> mappingSrcChangedEvt = MappingSrcChangedEvt;
            if (mappingSrcChangedEvt != null) {
                mappingSrcChangedEvt(this, e);
            }
        }
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping src data(String or Number).
        /// </summary>
        /// <param name="sender">IMappingSrc panel</param>
        /// <param name="obj">string or number</param>
        public void raiseMappingSrcChangedEvt(Object sender, object obj) {
            if (obj != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnMappingSrcChangedEvt(evt);
            }
        }
        #endregion events
        public MSConstantPanel() {
            InitializeComponent();
            this.rtb_exp.ForeColor = Color.FromName(UIConstants.COLOR_MAPPING_EXP_TEXT);
            this.panelDate.Dock = DockStyle.Top;
        }
        #region mandatory methods 
        public string getExpression() {
            if (isValid()) {
                if (srcType == ParamType.DATETIME) {
                    if (outputConst is string) {
                        if (outputConst.ToString() == Constants.DATETIME_NOW) {
                            return "Now";
                        } else {
                            return outputConst.ToString();
                        }
                    }
                } 
                return ModelManager.Instance.getMappingSrcText(outputConst);                
            } else {
                return string.Empty;
            }
        }

        public string getValidMsg() {
            string msg = null ;            
            if (outputConst == null || outputConst.ToString().Trim().Length<1) {
                msg = UILangUtil.getMsg("mapping.src.const.err.msg1");
            } else {
                if (srcType == ParamType.STRING) {
                    msg = null;
                }else if (srcType == ParamType.NUMBER) {
                    decimal dec = ModelManager.Instance.getDecimal(this.outputConst);
                    if (dec == decimal.MinValue) {
                        msg = UILangUtil.getMsg("mapping.src.const.err.msg2");
                    }
                } else if (srcType == ParamType.DATETIME) {
                    if (!ModelManager.Instance.isValidTime(this.outputConst+"")) {
                        msg = UILangUtil.getMsg("mapping.src.const.time.err.msg1");
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
        /// Mapping is should be deciaml(Number),string(String) or null if errors 
        /// </summary>
        /// <returns></returns>
        public object getMappingSrc() {
            if (isValid()) {
                return this.outputConst;
            }
            return null;
        }
        /// <summary>
        /// src should be String or Number align with srcType. 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcType"></param>
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
        
        private void setInput(object src) {
            // update the src object 
            this.outputConst = createAndInitOutput(src);
            if (this.srcType == ParamType.STRING || this.srcType == ParamType.NUMBER) {
                this.panelDate.Visible = false;       
                this.panel2.Visible = true;
                this.tb_const.Text = this.outputConst.ToString();
            } else if (srcType == ParamType.DATETIME) {
                this.panel2.Visible = false;
                this.panelDate.Visible = true;
                updatePanelTime();
            }
            this.lb_msg.Text = getValidMsg();
            this.rtb_exp.Text = getExpression();
        }

        private void updatePanelTime() {
            if (ModelManager.Instance.isValidTime(this.outputConst + "")) {
                string str = this.outputConst.ToString();
                if (str == Constants.DATETIME_NOW) {
                    this.rbt_time.Checked = false;
                    this.rbt_now.Checked = true;                    
                } else {
                    this.rbt_now.Checked = false;
                    this.rbt_time.Checked = true;
                    string[] ss = str.Split(':');
                    nud_hour.Value = int.Parse(ss[0]);
                    nud_min.Value = int.Parse(ss[1]);
                    nud_sec.Value = int.Parse(ss[2]);
                }
            }
        }
        /// <summary>
        /// Create a new output object based on the src input 
        /// </summary>
        /// <param name="src">String or decimal</param>
        /// <returns></returns>
        private object createAndInitOutput(object src) {
            string output = string.Empty;
            if (src is string || src is decimal) {
                output = src.ToString();
            }

            return output;
        }
        private void updateSrcType(ParamType srcType) {
            if (srcType == ParamType.STRING || srcType == ParamType.NUMBER) {
                this.srcType = srcType;                
                this.panelDate.Visible = false;
                this.panel2.Visible = true;
            } else if (srcType == ParamType.DATETIME) {
                this.srcType = ParamType.DATETIME;
                this.panel2.Visible = false;
                this.panelDate.Visible = true;
            }
        }
       
        #endregion common methods         
        private void tb_const_TextChanged(object sender, EventArgs e) {
            string txt = tb_const.Text;
            bool changed = false;            
            if (this.srcType == ParamType.NUMBER) {
                decimal d = ModelManager.Instance.getDecimal(txt);
                decimal d1 = ModelManager.Instance.getDecimal(this.outputConst);
                if (d != decimal.MinValue) {
                    if (d1 != d) {
                        changed = true;
                        this.outputConst = d;
                    }
                }
            } else if (this.srcType == ParamType.STRING) {
                if (outputConst != null && txt != this.outputConst.ToString()) {
                    this.outputConst = txt;
                    changed = true;
                }
            }
            if (changed) {
                this.lb_msg.Text = getValidMsg();
                this.rtb_exp.Text = getExpression();
                // raise event 
                this.raiseMappingSrcChangedEvt(this, getMappingSrc());
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e) {
            ControlPaint.DrawBorder(e.Graphics, this.panel3.ClientRectangle,Color.FromName(UIConstants.COLOR_MAPPING_EXP_BORDER),ButtonBorderStyle.Solid);
        }

        private void rbt_now_Click(object sender, EventArgs e) {
            // update UI 
            this.rbt_time.Checked = false;
            this.nud_hour.Enabled = false;
            this.nud_hour.Value = 0;
            this.nud_min.Enabled = false;
            this.nud_min.Value = 0;
            this.nud_sec.Enabled = false;
            this.nud_sec.Value = 0;
            // update model 
            this.outputConst = Constants.DATETIME_NOW;
            this.rtb_exp.Text = getExpression();
            // raise event 
            this.raiseMappingSrcChangedEvt(this, getMappingSrc());            
        }

        private void rbt_time_Click(object sender, EventArgs e) {            
            // update UI 
            this.rbt_now.Checked = false;
            this.nud_hour.Enabled = true;
            this.nud_hour.Value = 0;
            this.nud_min.Enabled = true;
            this.nud_min.Value = 0;
            this.nud_sec.Enabled = true;
            this.nud_sec.Value = 0;
            
        }

        private void nud_hour_ValueChanged(object sender, EventArgs e) {
            doTimeValueChanged();
        }
        private void nud_min_ValueChanged(object sender, EventArgs e) {
            doTimeValueChanged();
        }
        private void nud_sec_ValueChanged(object sender, EventArgs e) {
            doTimeValueChanged();
        }

        private void doTimeValueChanged() {
            if (rbt_time.Checked == true) {                
                // update model 
                this.outputConst = this.nud_hour.Value + ":" + this.nud_min.Value + ":" + this.nud_sec.Value;
                this.lb_msg.Text = getValidMsg();
                this.rtb_exp.Text = getExpression();
                if (ModelManager.Instance.isValidTime(this.outputConst.ToString())) {
                    // raise event 
                    this.raiseMappingSrcChangedEvt(this, this.getMappingSrc());
                }
            }
        }
    }
}
