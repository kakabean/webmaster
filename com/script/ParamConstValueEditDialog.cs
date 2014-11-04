using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib.ui;

namespace WebMaster.com.script
{
    /// <summary>
    /// This dialog is used to edit a Constant Parameter value
    /// </summary>
    public partial class ParamConstValueEditDialog : Form
    {
        #region variables         
        private object pvalue = null;
        /// <summary>
        /// Constant parameter new value
        /// </summary>
        public object Value {
            get { return pvalue; }
            //set { pvalue = value; }
        }
        private ParamType type = ParamType.STRING;
        #endregion variables 
        public ParamConstValueEditDialog() {
            InitializeComponent();
        }
        
        private void setInput(object input) {
            Parameter param = input as Parameter;
            string type = getTypeText();
            this.labelType.Text = type;
            this.tb_des.Text = param.Description;
            string value = string.Empty;
            if (param.DesignValue != null) {
                value = param.DesignValue.ToString();
            }
            this.tb_value.Text = value;
        }

        private string getTypeText() {
            string text = ModelManager.Instance.getParamTypeText(ParamType.STRING);            
            if (type == ParamType.NUMBER) {
                text = ModelManager.Instance.getParamTypeText(ParamType.NUMBER);
            }

            return text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler">parent window</param>
        /// <param name="input">set item object, maybe string, Decimal, Parameter or WebElementAttribute </param>
        /// <param name="type">set item type</param>
        /// <returns></returns>
        public DialogResult showValueDialog(IWin32Window handler, object input, ParamType type) {
            if (input is Parameter) {
                Parameter param = input as Parameter;
                if (param.Type == ParamType.NUMBER || param.Type == ParamType.STRING) {
                    this.type = type;                    
                    this.setInput(input);
                    this.StartPosition = FormStartPosition.CenterParent;
                    this.tb_value.Focus();
                    return this.ShowDialog(handler);
                }
            }
            return System.Windows.Forms.DialogResult.Cancel;
        }
        
        /// <summary>
        /// get the input value, maybe later version, input will support global function wo concat some parameters, maybe 
        /// </summary>
        /// <returns></returns>
        private object getInputValue() {
            return this.tb_value.Text;
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e) {
            doTextBox1_TextChanged();
        }

        private void doTextBox1_TextChanged() {
            object value = getInputValue();
            labelMsg.Text = string.Empty;
            if (type == ParamType.NUMBER) {
                decimal dec = ModelManager.Instance.getDecimal(value);
                if (dec == decimal.MinValue) {
                    labelMsg.Text = UILangUtil.getMsg("dlg.param.const.value.valid.msg1");
                    this.btn_OK.Enabled = false;
                } else {
                    this.pvalue = dec;
                    this.btn_OK.Enabled = true;                    
                }
            } else {
                this.pvalue = value;
                this.btn_OK.Enabled = true;
            }            
        }
    }
}
