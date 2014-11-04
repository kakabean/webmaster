using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib.ui;
using WebMaster.ide.editor.propview;
using WebMaster.com;
using WebMaster.ide.ui;
using WebMaster.lib.rule;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    public partial class RulePropView : UserControl,IPropView
    {
        /// <summary>
        /// input can be Operation or Process
        /// </summary>
        private Operation input = null;
        private FlowPropViewManager flowPVManager = null;
        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { flowPVManager = value; }
        }

        public RulePropView() {
            InitializeComponent();
        }
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
        public void updatedInput() {
            cleanView();
            //TODO
        }

        public void setInput(object input) {
            cleanView();
            this.input = input as Operation;
            if (this.input == null) {
                return;
            }
            this.enableView();
            updateRuleList();
        }

        public object getInput() {
            return input;
        }

        public void cleanView() {
            this.listView1.Items.Clear();
            this.btn_add.Enabled = false;
            this.btn_edit.Enabled = false;
            this.btn_del.Enabled = false;
        }

        public void showView() {
            this.Visible = true;
        }

        public void hideView() {
            this.Visible = false;
        }

        public void enableView() {
            this.listView1.Enabled = true;
            this.btn_add.Enabled = true;
            this.btn_edit.Enabled = true;
            this.btn_del.Enabled = true;
        }

        public void disableView() {
            this.listView1.Enabled = false;
            this.btn_add.Enabled = false;
            this.btn_edit.Enabled = false;
            this.btn_del.Enabled = false;
        }
        public void updateValidationMsg(string msg) { 
        
        }
        #endregion mandatory methods 
        #region common method 
        
        private void addRule(OperationRule rule) {
            if(rule == null){
                return ;
            }
            ListViewItem lvi = new ListViewItem();
            lvi.Text = RuleUtil.getTriggerText(rule);
            string action = RuleUtil.getActionText(rule);
            lvi.SubItems.Add(action);
            lvi.Tag = rule;
            this.listView1.Items.Add(lvi);
        }
        /// <summary>
        /// return true if the rule is valid one for rules list. or return false. 
        /// Valid means that the there is no rule in the list has the same trigger and action with target rule
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool isValidRuleForList(BEList<OperationRule> rules, OperationRule rule) {
            if (rules == null) {
                return false;
            }
            List<OperationRule> list = new List<OperationRule>();
            list.AddRange(rules.ToArray());
            return ModelManager.Instance.isValidRuleForList(list,rule);
        }
        #endregion common method 

        private void btn_add_Click(object sender, EventArgs e) {
            RuleEditDialog dlg = new RuleEditDialog();
            DialogResult dr = dlg.showRuleDialog(UIUtils.getTopControl(this), this.input, null);
            if (dr == DialogResult.OK) {
                OperationRule rule = dlg.Rule;               
                if (rule != null) {
                    if (isValidRuleForList(input.Rules, rule)) {
                        this.input.Rules.Add(rule);
                        updateRuleList();
                        this.FlowPVManager.raiseInputUpdatedEvt(this, this.input);
                    } else {
                        showDuplicatedDialog();
                    }
                }
            }
        }
        
        private void btn_edit_Click(object sender, EventArgs e) {
            OperationRule selectedRule = getSelectedRule();
            if (selectedRule == null) {
                return;
            }
            RuleEditDialog dlg = new RuleEditDialog();
            DialogResult dr = dlg.showRuleDialog(UIUtils.getTopControl(this), this.input, selectedRule);
            if (dr == DialogResult.OK) {                
                OperationRule rule = dlg.Rule;                
                if (rule != null) {
                    int index = this.input.Rules.IndexOf(selectedRule);
                    this.input.Rules.Remove(selectedRule);
                    //if (ModelManager.Instance.isValidRuleForList(input.Rules, rule)) {
                    if(isValidRuleForList(input.Rules,rule)){
                        this.input.Rules.Insert(index,rule);
                        updateRuleList();
                    } else {
                        this.input.Rules.Insert(index, selectedRule);
                        showDuplicatedDialog();
                    }
                    this.FlowPVManager.raiseInputUpdatedEvt(this, this.input);
                }
            }
        }

        private void btn_del_Click(object sender, EventArgs e) {
            OperationRule rule = getSelectedRule();
            if (rule != null) {
                ModelManager.Instance.removeFromModel(rule);
                
                updateRuleList();
                this.FlowPVManager.raiseInputUpdatedEvt(this, this.input);
            }
        }
        
        private OperationRule getSelectedRule() {
            OperationRule rule = null;
            if (this.listView1.SelectedItems.Count > 0) {
                rule = this.listView1.SelectedItems[0].Tag as OperationRule;   
            }
            return rule;
        }

        private void showDuplicatedDialog() {
            string text = UILangUtil.getMsg("view.rule.dup.text");
            string title = UILangUtil.getMsg("view.rule.dup.title");
            MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
        }

        private void updateRuleList() {
            listView1.Items.Clear();
            listView1.BeginUpdate();
            if (this.input != null && this.input.Rules != null) {
                foreach (OperationRule rule in this.input.Rules) {
                    addRule(rule);
                }
            }
            listView1.EndUpdate();
        }
        
    }
}
