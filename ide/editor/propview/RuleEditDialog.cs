using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.com;
using WebMaster.lib.engine;
using WebMaster.ide.ui;
using WebMaster.ide;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    public partial class RuleEditDialog : Form
    {
        #region variables         
        /// <summary>
        /// Rule's stub operation 
        /// </summary>
        private Operation inputOp = null;
        private OperationRule inputRule = null;
        OperationRule returnRule = null;
        /// <summary>
        /// returned rule or null if errors 
        /// </summary>
        public OperationRule Rule {
            get { return returnRule; }
            set { returnRule = value; }
        }
        
        // UI editor for parameters list 
        private TextBox ceTextBox = null;
        private Button ceButton = null;
        private Control[] lv_paramCE1 = null;
        private Control[] lv_paramCE2 = null;
        // it is used to mantain the latest trigger text, so that 
        // when it is modified by keyboard, the right value can be restored
        private string triggerText = "";

        // this is only used to handle the operation parameter 
        private Operation paramOp = null;
        private bool isOpEditor = false ;
        // whether it is first load an existed rule to edit 
        private bool editParmFirstLoad = false;
        // temp maintain the rule paramters 
        private object[] ruleParams = null;
        #endregion variables 
        public RuleEditDialog() {
            InitializeComponent();
            initUIData();
        }

        private void initUIData() {
            // text box cell editor 
            this.ceTextBox = new TextBox();
            this.ceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ceTextBox.Location = new System.Drawing.Point(32, 104);
            this.ceTextBox.Multiline = true;
            this.ceTextBox.Name = "ceTextBox";
            this.ceTextBox.Size = new System.Drawing.Size(80, 16);            
            this.ceTextBox.Visible = false;

            // button cell editor
            this.ceButton = new Button();            
            this.ceButton.Location = new System.Drawing.Point(32, 80);
            this.ceButton.Name = "ceButton";
            this.ceButton.Text = "...";
            this.ceButton.Size = new System.Drawing.Size(80, 21);            
            this.ceButton.Visible = false;
            this.ceButton.Click += new EventHandler(ceButton_Click);

            this.grp_params.Controls.Add(ceTextBox);
            this.grp_params.Controls.Add(ceButton);

            lv_paramCE1 = new Control[] { null, ceTextBox};
            lv_paramCE2 = new Control[] { null, ceButton };            

            // build up triggers 
            List<string> triggerTextList = getTriggerTextList();
            this.cb_trigger.Items.AddRange(triggerTextList.ToArray());            
        }
        public DialogResult showRuleDialog(IWin32Window handler, Operation op, OperationRule rule) {
            this.inputOp = op;
            this.inputRule = rule;
            if (this.inputRule == null) {
                resetUI();
            } else {
                updateUI();
            }
            return ShowDialog(handler);
        }

        private void updateUI() {
            this.Rule = this.inputRule.Clone();
            this.editParmFirstLoad = true;
            // update trigger list 
            this.cb_trigger.SelectedIndex = -1;
            this.cb_trigger.SelectedIndex = getTriggerIndex(this.inputRule);
            // udpate action list 
            ListViewItem lvi = getActionListItem(this.inputRule);
            lvi.Selected = true;            
        }

        private void resetUI() {
            this.cb_trigger.SelectedIndex = -1;
            this.cb_trigger.Text = string.Empty;
            this.lv_action.Items.Clear();
            this.lv_params.Items.Clear();
            this.tb_msg.Text = string.Empty;
            this.Rule = ModelFactory.createOperationRule();
            this.rtb_des.Text = string.Empty;
        }
        #region trigger area
        
        #endregion trigger area 
        #region parameters 
        void ceButton_Click(object sender, EventArgs e) {
            if (isOpEditor) {
                isOpEditor = false;
                // show operation dialog 
                OpParamDialog dlg = new OpParamDialog();
                DialogResult dr = dlg.showOpDialog(UIUtils.getTopControl(this), this.inputOp);
                if (dr == System.Windows.Forms.DialogResult.OK) {
                    this.paramOp = dlg.Output;
                }
                this.lv_params.EndEditing(true);
            }
        }

        private void lv_params_SubItemClicked(object sender, SubItemEventArgs e) {
            if (isOperationEditor(e.Item)) {
                Rectangle r = lv_params.GetSubItemBounds(e.Item, 1);
                Size size = new Size(24, -1);
                this.lv_params.setAdjustCellCtrl(1, size);
                List<int> list = new List<int>();
                list.Add(1);
                this.lv_params.setCellEditors(lv_paramCE2,list);

                this.paramOp = null;
                this.isOpEditor = true;
            } else {
                this.lv_params.setAdjustCellCtrl(-1, new Size(-1, -1));
                this.lv_params.setCellEditors(lv_paramCE1);
                this.paramOp = null;
                this.isOpEditor = false;
            }
            lv_params.StartEditing(e.SubItem, e.Item);
        }

        private void lv_params_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e) {
            if (Rule.Action == RuleAction.WaitUntilElemFind) {
                if (lv_params.Items.Count == 1) { // paramter is timeout
                    handleWaitUntilParam_Timeout(e.Item, e.SubItem);
                    e.DisplayText = e.Item.SubItems[e.SubItem].Text;
                    if (this.ruleParams[0] != null) {
                        this.Rule.Params.Clear();
                        this.Rule.Params.AddRange(this.ruleParams);
                        this.btn_OK.Enabled = true;
                    } else {
                        this.btn_OK.Enabled = false;
                    }
                } else if (lv_params.Items.Count == 2) { // parameter is timeout, operation
                    string tag = e.Item.Tag.ToString();
                    if (tag == "Rule.WaitUntilNullElemFindRule.P2.1.Name") {
                        handleWaitUntilParam_Timeout(e.Item, e.SubItem);                    
                    } else if (tag == "Rule.WaitUntilNullElemFindRule.P2.2.Name") {
                        handleWaitUntilParam_Op(e.Item, e.SubItem);
                    }
                    e.DisplayText = e.Item.SubItems[e.SubItem].Text;

                    if (this.ruleParams[0]!=null && this.ruleParams[1] is Operation) {
                        this.Rule.Params.Clear();
                        this.Rule.Params.AddRange(this.ruleParams);
                        this.btn_OK.Enabled = true;
                    } else {
                        this.btn_OK.Enabled = false;
                    }
                }
            } else if (Rule.Action == RuleAction.Goto_Operation) {
                if (lv_params.Items.Count == 1) { // paramter is operation
                    string tag = e.Item.Tag.ToString();
                    if (tag == "Rule.GotoOperation.P1.0.Name") {
                        handleGotoOp(e.Item, e.SubItem);
                    }
                    e.DisplayText = e.Item.SubItems[e.SubItem].Text;

                    if (this.ruleParams[0] is Operation) {
                        this.Rule.Params.Clear();
                        this.Rule.Params.AddRange(this.ruleParams);
                        this.btn_OK.Enabled = true;
                    } else {
                        this.btn_OK.Enabled = false;
                    }
                }
            }
        }

        private void handleGotoOp(ListViewItem lvi, int index) {
            string text = string.Empty;
            if (this.paramOp != null) {
                text = this.paramOp.Name;
                this.ruleParams[0] = this.paramOp;
            }

            lvi.SubItems[index].Text = text;
        }

        private void handleWaitUntilParam_Op(ListViewItem lvi, int index) {
            string text = string.Empty;
            if (this.paramOp != null) {
                text = this.paramOp.Name;
                this.ruleParams[1] = this.paramOp;
            } 
            
            lvi.SubItems[index].Text = text ;
        }

        private void handleWaitUntilParam_Timeout(ListViewItem lvi, int index) {
            string value = this.ceTextBox.Text.Trim();
            decimal timeout = decimal.MinValue;
            bool error = false;
            try {
                timeout = Convert.ToDecimal(value);
            } catch (Exception) {
                error = true;
            }
            if (error) {
                lvi.SubItems[index].Text = string.Empty;
                this.ruleParams[0] = null;
                this.btn_OK.Enabled = false;                
            } else {
                lvi.SubItems[index].Text = timeout.ToString();
                this.ruleParams[0] = timeout;
                this.btn_OK.Enabled = true;
            }
        }

        private void lv_params_MouseDown(object sender, MouseEventArgs e) {
            ListViewItem lvi = lv_params.GetItemAt(e.X, e.Y);
            if (lvi == null) {
                return;
            }
            updateParamMsg(lvi.Tag.ToString());
        }
        /// <summary>
        /// Update parameter list view 
        /// </summary>
        /// <param name="lvi"></param>
        private void updateParamListView(ListViewItem lvi) {
            if (lvi == null || lvi.Text == null) {
                return;
            }
            lv_params.Items.Clear();
            if ("Rule.WaitUntilNullElemFindRule.P1.Name".Equals(lvi.Tag)) {
                updateParams_WaitUntilNullElemFindRule_P1();
            } else if ("Rule.WaitUntilNullElemFindRule.P2.Name".Equals(lvi.Tag)) {
                updateParams_WaitUntilNullElemFindRule_P2();
            } else if ("Rule.RestartScript.P0.Name".Equals(lvi.Tag)) {
                updateParams_RestartScript_P0();
            } else if("Rule.GotoOperation.P1.Name".Equals(lvi.Tag)){
                updateparams_GotoOp_P0();
            }
        }
        
        private void updateParams_WaitUntilNullElemFindRule_P1() {
            lv_params.BeginUpdate();
            this.ruleParams = new object[1];
            string timeout = "";
            if (this.inputRule != null && this.inputRule.Params.Count == 1) {
                timeout = this.inputRule.Params.get(0).ToString();
                this.ruleParams[0] = timeout;
            }
            addListItem(lv_params, "Rule.WaitUntilNullElemFindRule.P1.1.Name", timeout);
            lv_params.EndUpdate();
        }

        private void updateParams_WaitUntilNullElemFindRule_P2() {
            lv_params.BeginUpdate();
            this.ruleParams = new object[2];
            string timeout = "";
            string opname = "";
            if (this.inputRule != null && this.inputRule.Params.Count == 2) {
                timeout = this.inputRule.Params.get(0).ToString();
                this.ruleParams[0] = timeout;
                this.ruleParams[1] = this.inputRule.Params.get(1);
                opname = (this.inputRule.Params.get(1) as Operation).Name;
            }
            addListItem(lv_params, "Rule.WaitUntilNullElemFindRule.P2.1.Name", timeout);
            addListItem(lv_params, "Rule.WaitUntilNullElemFindRule.P2.2.Name", opname);
            lv_params.EndUpdate();
        }

        private void updateparams_GotoOp_P0() {
            lv_params.BeginUpdate();
            this.ruleParams = new object[1];
            string opname = "";
            if (this.inputRule != null && this.inputRule.Params.Count == 1) {
                Operation op = this.inputRule.Params.get(0) as Operation ;
                opname = op.Name;
            }
            addListItem(lv_params, "Rule.GotoOperation.P1.0.Name", opname);
            lv_params.EndUpdate();
        }

        private void updateParams_RestartScript_P0() {
            lv_params.Items.Clear();
        }
        private void updateParamMsg(string tag) {
            if (tag == null) {
                return;
            }
            if ("Rule.WaitUntilNullElemFindRule.P1.1.Name" == tag) {
                tb_msg.Text = UILangUtil.getMsg("Rule.WaitUntilNullElemFindRule.P1.1.Des");
            } else if ("Rule.WaitUntilNullElemFindRule.P2.1.Name" == tag) {
                tb_msg.Text = UILangUtil.getMsg("Rule.WaitUntilNullElemFindRule.P2.1.Des");
            } else if ("Rule.WaitUntilNullElemFindRule.P2.2.Name" == tag) {
                tb_msg.Text = UILangUtil.getMsg("Rule.WaitUntilNullElemFindRule.P2.2.Des");
            } else if ("Rule.GotoOperation.P1.0.Name" == tag) {
                tb_msg.Text = UILangUtil.getMsg("Rule.GotoOperation.P1.0.Des");
            }
        }
        private bool isOperationEditor(ListViewItem lvi) {
            if (lvi == null || lvi.Text == null) {
                return false;
            }
            if (lvi.Text.Equals(UILangUtil.getMsg("Rule.WaitUntilNullElemFindRule.P2.2.Name"))
                || lvi.Text.Equals(UILangUtil.getMsg("Rule.GotoOperation.P1.0.Name"))) {
                return true;
            }
            return false;
        }
        #endregion parameters 
        #region trigger area 
        private void cb_trigger_SelectedIndexChanged(object sender, EventArgs e) {
            this.triggerText = this.cb_trigger.Text;
            this.btn_OK.Enabled = false;
            this.tb_msg.Text = string.Empty;
            int index = this.cb_trigger.SelectedIndex ;
            if (index == -1) {
                return;
            }
            // update data and ui status 
            this.lv_action.Items.Clear();
            this.lv_params.Items.Clear();
            this.tb_msg.Text = string.Empty;
            this.Rule.Action = RuleAction.None;
            this.Rule.Params.Clear();
            // update action list 
            lv_action.BeginUpdate();
            if (0 == index) { // NUll_ELEMENT
                this.Rule.Trigger = RuleTrigger.NULL_ELEMENT;
                buildNullELementRuleList();
            } else if (1 == index) { // OP_EXE_ERROR
                this.Rule.Trigger = RuleTrigger.OP_EXE_ERROR;
                buildExeOpErrorRuleList();
            } else if (2 == index) { // REQ_TIMEOUT
                this.Rule.Trigger = RuleTrigger.REQ_TIMEOUT;
                buildReqTimeoutRuleList();
            } else if (3 == index) { // NO_NEXT_OP_FOUND
                this.Rule.Trigger = RuleTrigger.NO_NEXT_OP_FOUND;
                buildNoNextOpFoundRuleList();
            }
            lv_action.EndUpdate();
        }

        private List<string> getTriggerTextList() {
            List<string> list = new List<string>();
            list.Add(ModelManager.Instance.getRuleTriggerText(RuleTrigger.NULL_ELEMENT));
            list.Add(ModelManager.Instance.getRuleTriggerText(RuleTrigger.OP_EXE_ERROR));
            list.Add(ModelManager.Instance.getRuleTriggerText(RuleTrigger.REQ_TIMEOUT));
            list.Add(ModelManager.Instance.getRuleTriggerText(RuleTrigger.NO_NEXT_OP_FOUND));
            return list;
        }

        /// <summary>
        /// return the selected trigger index by the rule or -1 if errrors 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private int getTriggerIndex(OperationRule rule) {
            int index = -1;
            if (rule != null) {
                if (rule.Trigger == RuleTrigger.NULL_ELEMENT) {
                    index = 0;
                } else if (rule.Trigger == RuleTrigger.OP_EXE_ERROR) {
                    index = 1;
                } else if (rule.Trigger == RuleTrigger.REQ_TIMEOUT) {
                    index = 2;
                } else if (rule.Trigger == RuleTrigger.NO_NEXT_OP_FOUND) {
                    index = 3;
                }
            }
            return index;
        }

        private void buildNoNextOpFoundRuleList() {
            this.updateActionList_RestartScript();
            this.updateActionList_GotoOperation();
        }

        private void buildReqTimeoutRuleList() {
            this.updateActionList_WaitUntilElementNotFoundRule();
            this.updateActionList_RestartScript();        
        }

        private void buildExeOpErrorRuleList() {        
            this.updateActionList_RestartScript();                       
        }

        private void buildNullELementRuleList() {
            updateActionList_WaitUntilElementNotFoundRule();
        }

        private void cb_trigger_TextUpdate(object sender, EventArgs e) {
            string text = this.cb_trigger.Text;
            if (text != null && text != this.triggerText) {
                this.cb_trigger.Text = this.triggerText;
            }
        }
        
        #endregion trigger area 
        #region RuleActions
        private void lv_action_MouseDown(object sender, MouseEventArgs e) {
            ListViewItem lvi = lv_action.GetItemAt(e.X, e.Y);
            if (lvi == null) {
                return;
            }
            this.editParmFirstLoad = true;
            doActionItemSelected(lvi);
        }
        private void lv_action_SelectedIndexChanged(object sender, EventArgs e) {
            if (lv_action.SelectedItems.Count > 0) {
                ListViewItem lvi = lv_action.SelectedItems[0];
                if (this.editParmFirstLoad) {
                    doActionItemSelected(lvi);
                    this.editParmFirstLoad = false;
                }
            }
        }
        private void doActionItemSelected(ListViewItem lvi) {
            // update data and ui status             
            this.lv_params.Items.Clear();
            this.tb_msg.Text = string.Empty;
            this.Rule.Action = getAction(lvi.Tag.ToString());
            this.Rule.Params.Clear();

            // update parameters list 
            this.tb_msg.Text = string.Empty;
            // update description
            this.rtb_des.Text = this.Rule.Description;
            if (!hasParameter(lvi)) {
                this.btn_OK.Enabled = true;                
            } else {
                this.btn_OK.Enabled = false;
                updateParamListView(lvi);                
            }
        }
        private RuleAction getAction(string tag) {
            RuleAction action = RuleAction.None;
            if (tag == "Rule.WaitUntilNullElemFindRule.P1.Name"
                || tag == "Rule.WaitUntilNullElemFindRule.P2.Name") {
                action = RuleAction.WaitUntilElemFind;
            } else if (tag == "Rule.RestartScript.P0.Name") {
                action = RuleAction.RestartScript;
            } else if (tag == "Rule.GotoOperation.P1.Name") {
                action = RuleAction.Goto_Operation;
            }
            return action;
        }
        /// <summary>
        /// Get the ListVIewItem of the lv_action list or null if not found 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private ListViewItem getActionListItem(OperationRule rule) {
            if(rule == null){
                return null ;
            }
            ListViewItem item = null;
            string tag = null;
            if (rule.Action == RuleAction.WaitUntilElemFind) { 
                if(rule.Params.Count == 1){
                    tag = "Rule.WaitUntilNullElemFindRule.P1.Name";
                }else if(rule.Params.Count == 2){
                    tag = "Rule.WaitUntilNullElemFindRule.P2.Name";
                }
            }else if(rule.Action == RuleAction.RestartScript){
                tag = "Rule.RestartScript.P0.Name";
            } else if (rule.Action == RuleAction.Goto_Operation) {
                tag = "Rule.GotoOperation.P1.Name";
            }
            if (tag != null) {
                foreach (ListViewItem lvi in lv_action.Items) {
                    string nt = lvi.Tag.ToString();
                    if (tag == nt) {
                        item = lvi;
                        break;
                    }
                }
            }
            return item;
        }
        /// <summary>
        /// Add WaitUntilElementNotFound rule's action in action list 
        /// </summary>
        private void updateActionList_WaitUntilElementNotFoundRule() {
            // WaitUntilNullElement with 1 parameter 
            addListItem(lv_action,"Rule.WaitUntilNullElemFindRule.P1.Name");
            // WaitUntilNullElement with 2 parameter 
            addListItem(lv_action, "Rule.WaitUntilNullElemFindRule.P2.Name");
        }
        private void updateActionList_RestartScript() {
            // Restart script  
            addListItem(lv_action, "Rule.RestartScript.P0.Name");
        }

        private void updateActionList_GotoOperation() {
            // Goto operaiton 
            addListItem(lv_action, "Rule.GotoOperation.P1.Name");
        }
        /// <summary>
        /// whether the selected action has parameter 
        /// </summary>
        /// <param name="lvi"></param>
        /// <returns></returns>
        private bool hasParameter(ListViewItem lvi) {
            bool result = true;
            if (lvi != null) {
                if ("Rule.WaitUntilNullElemFindRule.P1.Name".Equals(lvi.Tag)) {
                    result = true;
                }else if("Rule.WaitUntilNullElemFindRule.P2.Name".Equals(lvi.Tag)) {
                    result = true;
                } else if ("Rule.RestartScript.P0.Name".Equals(lvi.Tag)) {
                    result = false; // restart script action has no parameters
                } else if ("Rule.GotoOperation.P1.Name".Equals(lvi.Tag)) {
                    result = true ;
                }
            }

            return result;
        }
        /// <summary>
        /// add a list view items with specified text 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="tag"></param>
        private void addListItem(ListView lv, string tag) {
            if (lv == null || tag == null) {
                return; 
            }
            string text = UILangUtil.getMsg(tag);
            ListViewItem lvi = new ListViewItem(text);
            lvi.Tag = tag;
            lv.Items.Add(lvi);
        }
        /// <summary>
        /// add a list view items with specified text 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="tag"></param>
        /// <param name="subItem"></param>
        private void addListItem(ListView lv, string tag, string subItem) {
            if (lv == null || tag == null) {
                return;
            }
            string text = UILangUtil.getMsg(tag);
            ListViewItem lvi = new ListViewItem(text);
            lvi.Tag = tag;
            lvi.SubItems.Add(subItem);

            lv.Items.Add(lvi);
        }
        #endregion RuleActions 

        private void btn_OK_Click(object sender, EventArgs e) {
            if (cb_trigger.SelectedIndex == -1 || lv_action.SelectedItems.Count == 0) {
                Rule = null;
            }
        }

        private void rtb_des_TextChanged(object sender, EventArgs e) {
            this.Rule.Description = this.rtb_des.Text;
        }
    }
}
