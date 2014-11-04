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
    /// This dialog is used to edit a Set item. 
    /// </summary>
    public partial class ParamSetItemDialog : Form
    {
        #region variables 
        
        private object item = null;
        /// <summary>
        /// set item result, it maintains the result item data
        /// </summary>
        public object Item {
            get { return item; }
            set { item = value; }
        }
        private ParamType type = ParamType.STRING;
        #endregion variables 
        public ParamSetItemDialog() {
            InitializeComponent();
        }
        
        private void setInput(object input) {
            string type = getTypeText();
            this.labelType.Text = type;
            string value = ModelManager.Instance.getSetItemValue(input);
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
        public DialogResult showItemDialog(IWin32Window handler, object input, ParamType type) {            
            this.type = type;            
            this.setInput(input);
            this.StartPosition = FormStartPosition.CenterParent;
            this.tb_value.Focus();
            return this.ShowDialog(handler);
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
            ValidationMsg msg = ModelManager.Instance.getSetItemValidMsg(value, type, "Parameter Set item - ");
            if (msg.Type == MsgType.VALID) {
                this.labelMsg.Text = string.Empty;
                this.Item = value;
                this.btn_OK.Enabled = true;
            } else {
                labelMsg.Text = msg.Msg;
                this.btn_OK.Enabled = false;
            }
        }
    }
}
