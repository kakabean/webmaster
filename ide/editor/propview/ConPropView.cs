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
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    public partial class ConPropView : UserControl, IPropView
    {
        #region constants 
        private static readonly int ICON_CON = 0;
        private static readonly int ICON_CON_ERR = 1;
        private static readonly int ICON_CON_NOT = 2;
        private static readonly int ICON_CON_NOT_ERR = 3;
        private static readonly int ICON_CONGRP_AND = 4;
        private static readonly int ICON_CONGRP_AND_ERR = 5;
        private static readonly int ICON_CONGRP_AND_NOT = 6;
        private static readonly int ICON_CONGRP_AND_NOT_ERR = 7;
        private static readonly int ICON_CONGRP_OR = 8;
        private static readonly int ICON_CONGRP_OR_ERR = 9;
        private static readonly int ICON_CONGRP_OR_NOT = 10;
        private static readonly int ICON_CONGRP_OR_NOT_ERR = 11;
        #endregion constants 
        #region variables 
        private OpCondition input = null;
        // used to maintain whether the text is changed by keyboard input 
        private bool tb_name_updated = false;
        private bool tb_des_updated = false;
        private bool tb_input1_updated = false;
        private bool tb_input2_updated = false;
        /// <summary>
        /// string pattern text
        /// </summary>
        private List<string> categoryStr = new List<string>();
        private List<string> categoryNum = new List<string>();
        private List<string> categoryObj = new List<string>();
        private List<string> categorySet = new List<string>();

        private FlowPropViewManager flowPVManager = null;

        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { flowPVManager = value; }
        }
        /// <summary>
        /// this is trick variable to make sure the i18n worked at design time
        /// </summary>
        private bool firstInitData = true;

        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        /// </summary>
        private int pattenChangedByHandFlag = 0;
        private int catagoryChangedByHandFlag = 0; 
        #endregion variables 
        #region constructor
        public ConPropView() {
            InitializeComponent();
            initUI();
        }

        private void initUI() {        
            // build pattern comboBox
            this.cb_pattern.Items.Clear();
        }
        /// <summary>
        /// Try to make sure the UI data was only initialized once. it is added to fix the 
        /// i18n design problem. If below code added into initData() method, it will
        /// some errors in design time when the propview created.
        /// </summary>
        public void tryInitDataOnlyOnce() {
            if (this.firstInitData) {
                this.firstInitData = false;
                // initial category box   
                cb_catagory.Items.AddRange(ModelManager.Instance.getConditonCatagory().ToArray());
                // build string category 
                categoryStr.AddRange(ModelManager.Instance.getStringPatterns());
                // build number category
                categoryNum.AddRange(ModelManager.Instance.getNumberPatterns());
                // build object category
                categoryObj.AddRange(ModelManager.Instance.getObjPatterns());
                // build set category 
                categorySet.AddRange(ModelManager.Instance.getSetPatterns());
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
            if (input is OpCondition) {
                this.input = input as OpCondition;
                this.tv_con.Enabled = true;
                initConditionTree();
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
            this.tv_con.Nodes.Clear();
            this.cleanDetailsArea();
        }
        public void showView() {
            this.Visible = true;
        }
        public void hideView() {
            this.Visible = false;
        }
        public void enableView() {
            this.tv_con.Enabled = true;
            this.tb_name.Enabled = true;
            this.tb_des.Enabled = true;
            this.tb_input1.Enabled = true;
            this.tb_input2.Enabled = true;
            this.label_msg.Enabled = true;
            this.cb_catagory.Enabled = true;
            this.cb_pattern.Enabled = true;            
            this.btn_Input1.Enabled = true;
            this.btn_Input2.Enabled = true;
            this.rbtn_AND.Enabled = true;
            this.rbtn_AND.Checked = false;
            this.rbtn_OR.Enabled = true;
            this.rbtn_OR.Checked = false;
            this.ckb_Not.Enabled = true;
            this.ckb_Not.Checked = false;
        }
        public void disableView() {
            this.tv_con.Enabled = false;
            this.tb_name.Enabled = false;
            this.tb_des.Enabled = false;
            this.tb_input1.Enabled = false;
            this.tb_input2.Enabled = false;
            this.label_msg.Enabled = false;
            this.cb_pattern.Enabled = false;
            this.cb_pattern.Enabled = false;            
            this.btn_Input1.Enabled = false;
            this.btn_Input2.Enabled = false;
            this.rbtn_AND.Enabled = false;
            this.rbtn_AND.Checked = false;
            this.rbtn_OR.Enabled = false;
            this.rbtn_OR.Checked = false;
            this.ckb_Not.Enabled = false;
            this.ckb_Not.Checked = false;
        }
        #endregion madatory methods 
        #region ui methods         
        /// <summary>
        /// update the OpCondition's validation messages 
        /// </summary>
        private void updateValidationMsg() {            
            ValidationMsg msg = getValidationMsg(this.input.ConditionGroup);
            if (msg.Type != MsgType.VALID) {
                this.label_msg.ForeColor = Color.Red;
                this.label_msg.Text = msg.Msg;
            } else {
                this.label_msg.ForeColor = Color.Black;
                this.label_msg.Text = "";
            }
        }
        private ValidationMsg getValidationMsg(BaseElement be) {
            ValidationMsg msg = ModelManager.Instance.getValidMsg(be);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            if (be is ConditionGroup) {
                ConditionGroup grp = be as ConditionGroup;
                foreach (BaseElement b in grp.Conditions) {
                    msg = getValidationMsg(b);
                    if (msg.Type != MsgType.VALID) {
                        return msg;
                    }
                }
            }
            return msg;
        }
        private void initDetailsArea() {
            this.tb_name.Enabled = false;
            this.tb_des.Enabled = false;
            this.tb_input1.Enabled = false;
            this.tb_input2.Enabled = false;
            this.cb_catagory.Enabled = false;
            this.cb_pattern.Enabled = false;
            this.btn_Input1.Enabled = false;
            this.btn_Input2.Enabled = false;
            this.label_msg.Text = "";            
        }
        
        private void initConditionTree() {
            if (this.input == null) {
                return;
            }
            this.tv_con.BeginUpdate();
            // create root node 
            TreeNode rn = this.createRootNode(this.input.ConditionGroup);
            this.tv_con.Nodes.Add(rn);
            // create children node 
            foreach (BaseElement be in this.input.ConditionGroup.Conditions) {
                if (be is ConditionGroup) {
                    ConditionGroup grp = be as ConditionGroup;
                    TreeNode gn = createGrpNode(grp);
                    rn.Nodes.Add(gn);
                } else if (be is Condition) {
                    Condition con = be as Condition;
                    TreeNode cn = createConNode(con);
                    rn.Nodes.Add(cn);
                }
            }
            tv_con.Nodes[0].Expand();
            this.tv_con.EndUpdate();
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e) {
            TreeNode node = tv_con.GetNodeAt(e.X, e.Y);
            if (node != tv_con.SelectedNode) {
                tv_con.SelectedNode = node;
            }
            if (node.Tag is ConditionGroup) {
                if(node.Tag.Equals(this.input.ConditionGroup)){
                // if it is the root node 
                    this.tsmi_remove.Visible = false ;
                    this.tsmi_newCon.Visible = true;
                    this.tsmi_newGrp.Visible = true;
                    this.tv_con.ContextMenuStrip = this.cms_conTree ;
                }else{
                    this.tsmi_newCon.Visible = true;
                    this.tsmi_newGrp.Visible = true;
                    this.tsmi_remove.Visible = true;
                    this.tv_con.ContextMenuStrip = this.cms_conTree;
                }
            } else if (node.Tag is Condition) {
                this.tsmi_newCon.Visible = false;
                this.tsmi_newGrp.Visible = false;
                this.tsmi_remove.Visible = true;
                this.tv_con.ContextMenuStrip = this.cms_conTree;
            }
        }

        private void tv_con_AfterSelect(object sender, TreeViewEventArgs e) {
            TreeNode node = tv_con.SelectedNode;
            updateDetailsArea(node.Tag);
            updateAndOrNotButtons(node.Tag);
        }
        /// <summary>
        /// when selected a Condition/ConditionGroup, make sure the AND, OR, NOT buttons enabled if needed
        /// </summary>
        /// <param name="obj"></param>
        private void updateAndOrNotButtons(object obj) {
            this.rbtn_AND.Enabled = false;
            this.rbtn_AND.Checked = false;
            this.rbtn_OR.Enabled = false;
            this.rbtn_OR.Checked = false;
            this.ckb_Not.Enabled = false;
            this.ckb_Not.Checked = false;
            if (obj is Condition) {
                Condition con = obj as Condition;
                this.ckb_Not.Enabled = true;
                if (con.IsNot) {
                    this.ckb_Not.Checked = true;
                }
            } else if (obj is ConditionGroup) {
                this.rbtn_AND.Enabled = true;
                this.rbtn_OR.Enabled = true;
                this.ckb_Not.Enabled = true;
                ConditionGroup cgrp = obj as ConditionGroup;
                if (cgrp.IsNot) {
                    this.ckb_Not.Checked = true;
                } 
                if (cgrp.Relation == CONDITION.AND) {
                    this.rbtn_AND.Checked = true;
                } 
                if (cgrp.Relation == CONDITION.OR) {
                    this.rbtn_OR.Checked = true;
                } 
            }
        }
        /// <summary>
        /// update the details UI area with selected Condtion or CondtionGroup 
        /// </summary>
        /// <param name="tag"></param>
        private void updateDetailsArea(object tag) {
            this.cleanDetailsArea();
            
            BaseElement be = tag as BaseElement;
            tb_name_updated = false;
            this.tb_name.Text = be.Name;
            tb_des_updated = false;
            this.tb_des.Text = be.Description;            

            if(tag is Condition){
                Condition con = tag as Condition ;
                this.tb_name.Enabled = true;
                this.tb_des.Enabled = true;
                this.tb_input1.Enabled = false;
                this.tb_input2.Enabled = false;
                this.cb_catagory.Enabled = true;
                this.cb_pattern.Enabled = true;
                this.btn_Input1.Enabled = false;
                this.btn_Input2.Enabled = false;

                this.catagoryChangedByHandFlag = 2;
                this.cb_catagory.SelectedIndex = this.getConditionCategoryIndex(con.COMPARE);
                this.doCatagorySelectionIndexChangedEvt();

            } else if (tag is ConditionGroup) {
                this.tb_name.Enabled = true;
                this.tb_des.Enabled = true;
                this.tb_input1.Enabled = false;
                this.tb_input1.Text = Constants.BLANK_TEXT;
                this.tb_input2.Enabled = false;
                tb_input2.Text = Constants.BLANK_TEXT;
                this.cb_catagory.Enabled = false;
                this.cb_pattern.Enabled = false;
                this.btn_Input1.Enabled = false;
                this.btn_Input2.Enabled = false;
            }
        }

        /// <summary>
        /// update the label type icon based on the condtion input 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="label"></param>
        /// <param name="pattern"></param>
        private void updateInputType(Object input,Label label, CONDITION pattern) {
            if (input is WebElement) {
                label.Text = "E";
            } else if (input is WebElementAttribute){
                label.Text = "A";
            } else if(input is Parameter){
                label.Text = "P";
            }else if(input is String) {
                if (ModelManager.Instance.isNumberPattern(pattern)) {
                    label.Text = "N";
                } else {
                    label.Text = "s";
                }
            }
        }
        /// <summary>
        /// show the condition display text 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string getConditionInputDisplayText(Object input) {
            if (input is WebElement) {
                WebElement we = input as WebElement;
                return we.Name;
            } else if (input is WebElementAttribute) {
                WebElementAttribute wea = input as WebElementAttribute;
                return wea.Name;
            } else if (input is Parameter) {
                Parameter param = input as Parameter;
                return param.Name;
            } else if (input is string) {
                return input as string;
            } else if (input == null) {
                return "";
            } else{
                return "ErrorInput";
            }
        }
        /// <summary>
        /// clean details area UI info 
        /// </summary>
        private void cleanDetailsArea() {
            tb_name_updated = false;
            this.tb_name.Text = Constants.BLANK_TEXT;
            tb_des_updated = false;
            this.tb_des.Text = Constants.BLANK_TEXT;
            //this.tb_input1_updated = false;
            this.tb_input1.Text = Constants.BLANK_TEXT;
            //this.tb_input2_updated = false;
            this.tb_input2.Text = Constants.BLANK_TEXT;
            
            this.catagoryChangedByHandFlag = 2;
            this.cb_catagory.SelectedIndex = -1;
            this.doCatagorySelectionIndexChangedEvt();

            this.label_msg.Text = "";
        }
        private void tsmi_newCon_Click(object sender, EventArgs e) {
            if (tv_con.SelectedNode.Tag is ConditionGroup) {
                ConditionGroup parent = tv_con.SelectedNode.Tag as ConditionGroup;
                Condition con = ModelFactory.createCondtion();
                con.Name = ModelManager.Instance.getUniqueElementName(parent.Conditions, con);
                TreeNode cnode = createConNode(con);
                cnode.Tag = con;

                TreeNode pnode = tv_con.SelectedNode;
                
                // updated model 
                parent.Conditions.AddUnique(con);
                // updated view 
                pnode.Nodes.Add(cnode);
                // update selection 
                tv_con.SelectedNode = cnode;

                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
        }

        private void tsmi_newGrp_Click(object sender, EventArgs e) {
            if (tv_con.SelectedNode.Tag is ConditionGroup) {
                ConditionGroup pGrp = tv_con.SelectedNode.Tag as ConditionGroup;
                ConditionGroup grp = ModelFactory.createConditionGroup();
                grp.Name = ModelManager.Instance.getUniqueElementName(pGrp.Conditions, grp);
                TreeNode gnode = createGrpNode(grp);
                gnode.Tag = grp;

                TreeNode pnode = tv_con.SelectedNode;
                // updated model 
                pGrp.Conditions.AddUnique(grp);
                // updated view 
                pnode.Nodes.Add(gnode);
                // update selection 
                tv_con.SelectedNode = gnode;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
        }

        private void tsmi_remove_Click(object sender, EventArgs e) {
            TreeNode node = tv_con.SelectedNode;
            if (node.Parent == null) {
                return;
            }
            // update view selection status 
            TreeNode snode = node.NextNode;
            if (snode == null) {
                snode = node.PrevNode;
            }
            if (snode == null) {
                snode = node.Parent;
            }
            tv_con.SelectedNode = snode;
            // updated model             
            if (node.Tag is ConditionGroup) {
                ModelManager.Instance.removeFromModel(node.Tag as ConditionGroup, this.input);
            } else if (node.Tag is Condition) {
                ModelManager.Instance.removeFromModel(node.Tag as Condition);
            }
            // updated view 
            node.Remove();

            // fire model updated event
            this.flowPVManager.raiseInputUpdatedEvt(this, input);
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
            if (tb_name_updated) {
                handleNameTextChanged();
            }
        }
        private void tb_name_Leave(object sender, EventArgs e) {
            handleNameTextChanged();
        }
        private void handleNameTextChanged() {
            if (this.tv_con.SelectedNode == null || this.tv_con.SelectedNode.Tag == null || this.tb_name.Text.Equals(Constants.BLANK_TEXT)) {
                return;
            }
            BaseElement be = this.tv_con.SelectedNode.Tag as BaseElement;
            String name = tb_name.Text.Trim();
            if (name.Length > 0 && !name.Equals(be.Name)) {
                be.Name = name;
                this.tv_con.SelectedNode.Text = name;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
            if (name.Length < 1) {
                label_msg.ForeColor = Color.Red;
                label_msg.Text = UILangUtil.getMsg("view.con.valid.msg1");// "Condition name is mandatory ! ";
            } else {
                ValidationMsg msg = ModelManager.Instance.getValidMsg(be);
                if (msg.Type != MsgType.VALID) {
                    label_msg.ForeColor = Color.Red;
                    label_msg.Text = msg.Msg;
                } else {
                    label_msg.ForeColor = Color.Black;
                    label_msg.Text = "";
                }
            }            
        }
        private void tb_des_KeyDown(object sender, KeyEventArgs e) {
            this.tb_des_updated = true;
            if (e.KeyCode == Keys.Enter) {
                this.cb_catagory.Focus();
            }
        }

        private void tb_des_TextChanged(object sender, EventArgs e) {
            if (tb_des_updated) {
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
            if (this.tv_con.SelectedNode != null && this.tv_con.SelectedNode.Tag != null) {
                BaseElement be = this.tv_con.SelectedNode.Tag as BaseElement;
                String desc = tb_des.Text.Trim();
                if (desc.Length > 0 && !desc.Equals(be.Description)) {
                    be.Description = desc;
                    // fire model updated event
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                }
            }
        }

        private void tb_input1_MouseDown(object sender, MouseEventArgs e) {
            if (tv_con.SelectedNode != null && tv_con.SelectedNode.Tag is Condition) {
                Condition con = this.tv_con.SelectedNode.Tag as Condition;
                if (!(con.Input1 is string || con.Input1 is decimal) && this.tb_input1.Text.Length > 0) {
                    this.tb_input1.SelectAll();
                }
            }
        }

        private void tb_input1_KeyDown(object sender, KeyEventArgs e) {
            this.tb_input1_updated = true;
        }

        private void tb_input1_TextChanged(object sender, EventArgs e) {
            if (tb_input1_updated) {
                if (this.tv_con.SelectedNode != null && this.tv_con.SelectedNode.Tag is Condition) {
                    Condition con = this.tv_con.SelectedNode.Tag as Condition;
                    con.Input1 = this.tb_input1.Text;
                    updateInputType(con.Input1, this.labelIType1, con.COMPARE);
                    // fire model updated event
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                }
                tb_input1_updated = false;
            }
        }
        private void tb_input2_MouseDown(object sender, MouseEventArgs e) {
            if (tv_con.SelectedNode != null && tv_con.SelectedNode.Tag is Condition) {
                Condition con = this.tv_con.SelectedNode.Tag as Condition;
                if (!(con.Input2 is string || con.Input2 is decimal) && this.tb_input2.Text.Length > 0) {
                    this.tb_input1.SelectAll();
                }
            }
        }

        private void tb_input2_KeyDown(object sender, KeyEventArgs e) {
            this.tb_input2_updated = true;
        }

        private void tb_input2_TextChanged(object sender, EventArgs e) {
            if (tb_input2_updated) {
                if (this.tv_con.SelectedNode != null && this.tv_con.SelectedNode.Tag is Condition) {
                    Condition con = this.tv_con.SelectedNode.Tag as Condition;
                    con.Input2 = this.tb_input2.Text;
                    updateInputType(con.Input2, this.labelIType2,con.COMPARE);
                    // fire model updated event
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                }
            }
            tb_input2_updated = false;
        }

        private void cb_pattern_SelectionChangeCommitted(object sender, EventArgs e) {
            this.pattenChangedByHandFlag = 1;
        }
        private void cb_pattern_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_PatternIndexChangedEvent();
        }
        /// <summary>
        /// It is stange that sometimes the although the SelectedIndex is set, but the cb_pattern_SelectedIndexChanged method
        /// was not triggered, so just handle this event by handle, not by the event. 
        /// </summary>
        private void doCB_PatternIndexChangedEvent() {
            if (pattenChangedByHandFlag == 0) {
                // it is used to reduce the duplicated second run.
                return;
            }
            // update comboBox text 
            if (this.cb_pattern.SelectedIndex == -1) {
                this.cb_pattern.Text = Constants.BLANK_TEXT;
            } else {
                this.cb_pattern.Text = this.cb_pattern.Items[this.cb_pattern.SelectedIndex].ToString();
            }

            Condition con = null;
            if (tv_con.SelectedNode != null && tv_con.SelectedNode.Tag is Condition) {
                con = tv_con.SelectedNode.Tag as Condition;
            }
            if (con == null) {
                disableInputArea();
                return;
            } else {
                enableInputArea();
            }
            // update model if changed by hand
            if (pattenChangedByHandFlag == 1) {
                con.Input1 = null;
                con.Input2 = null;
                //this.tb_input1_updated = false;
                tb_input1.Text = Constants.BLANK_TEXT;
                //this.tb_input2_updated = false;
                tb_input2.Text = Constants.BLANK_TEXT;
                btn_Input1.Tag = null;
                btn_Input2.Tag = null;
            }
            
            // update condition Pattern 
            String text = cb_pattern.Text;
            CONDITION compare = CONDITION.EMPTY;
            if (ModelManager.Instance.isStringPattern(text, ref compare)) {
                con.COMPARE = compare;
                handleStringPattern(con);
            } else if (ModelManager.Instance.isNumberPattern(text, ref compare)) {
                con.COMPARE = compare;
                handleNumberPattern(con);
            } else if (ModelManager.Instance.isObjPattern(text, ref compare)) {
                con.COMPARE = compare;
                handleObjPattern(con);
            } else if (ModelManager.Instance.isSetPattern(text, ref compare)) {
                con.COMPARE = compare;
                handleSETPattern(con);
            }
            // update input type labels 
            this.updateInputType(con.Input1, labelIType1,con.COMPARE);
            this.updateInputType(con.Input2, labelIType2,con.COMPARE);

            // fire model updated event
            if (pattenChangedByHandFlag == 1) {                
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
            pattenChangedByHandFlag = 0;
        }

        private void cb_catagory_SelectionChangeCommitted(object sender, EventArgs e) {
            this.catagoryChangedByHandFlag = 1;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_catagory_SelectedIndexChanged(object sender, EventArgs e) {
            doCatagorySelectionIndexChangedEvt();
        }

        private void doCatagorySelectionIndexChangedEvt() {
            if (catagoryChangedByHandFlag == 0) {
                // it is used to reduce the duplicated second run.
                return;
            }
            if (cb_catagory.SelectedIndex == -1) {
                
                this.pattenChangedByHandFlag = 2;
                this.cb_pattern.SelectedIndex = -1;                
                this.doCB_PatternIndexChangedEvent();

                this.cb_catagory.Text = Constants.BLANK_TEXT;
                return;
            } else {
                this.cb_catagory.Text = this.cb_catagory.Items[this.cb_catagory.SelectedIndex].ToString();
                //string text = this.cb_catagory.Items[this.cb_catagory.SelectedIndex] as string;
                int index = this.cb_catagory.SelectedIndex;
                this.cb_pattern.Items.Clear();
                if (0 == index) {
                    this.cb_pattern.Items.AddRange(categoryStr.ToArray());
                } else if (1 == index) {
                    this.cb_pattern.Items.AddRange(categoryNum.ToArray());
                } else if (2 == index) {
                    this.cb_pattern.Items.AddRange(categoryObj.ToArray());
                } else if (3 == index) {
                    this.cb_pattern.Items.AddRange(categorySet.ToArray());
                }
                // update patten comboBox index 
                int pIndex = -1;
                if (catagoryChangedByHandFlag == 2 && this.tv_con.SelectedNode != null && this.tv_con.SelectedNode.Tag is Condition) {
                    Condition con = this.tv_con.SelectedNode.Tag as Condition;
                    pIndex = getConditionPatternIndex(con.COMPARE);
                }
                // It is strange that sometimes if the pIndex=-1, it will never tigger the cb_pattern SelectedIndexChanged event
                // ft to death. 
                this.pattenChangedByHandFlag = 2;
                this.cb_pattern.SelectedIndex = pIndex;                
                this.doCB_PatternIndexChangedEvent();
            }
            catagoryChangedByHandFlag = 0;
        }
     
        private void cb_catagory_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.cb_pattern.Focus();
            }
        }
        /// <summary>
        /// update input enable status and icons 
        /// </summary>
        /// <param name="con"></param>
        private void handleSETPattern(Condition con) {
            if (con == null) {
                return;
            }
            btn_Input1.Tag = "SET";
            labelIType1.Text = "S";
            labelIType2.Text = "s";
            tb_input1.Enabled = false;
            tb_input1.Text = getConditionInputDisplayText(con.Input1);
            btn_Input1.Enabled = true;
            tb_input2.Enabled = true;
            tb_input2.Text = getConditionInputDisplayText(con.Input2);
            btn_Input2.Enabled = true;
        }
        /// <summary>
        /// update input enable status and icons 
        /// </summary>
        /// <param name="con"></param>
        private void handleStringPattern(Condition con) {
            if (con == null) {
                return;
            }
            labelIType1.Text = "s";
            labelIType2.Text = "s";
            tb_input1.Enabled = true;
            tb_input1.Text = getConditionInputDisplayText(con.Input1);            
            btn_Input1.Enabled = true;
            tb_input2.Enabled = true;
            tb_input2.Text = getConditionInputDisplayText(con.Input2);
            btn_Input2.Enabled = true;            
        }

        private void handleNumberPattern(Condition con) {
            if (con == null) {
                return;
            }
            labelIType1.Text = "N";
            labelIType2.Text = "N";
            tb_input1.Enabled = true;
            tb_input1.Text = getConditionInputDisplayText(con.Input1);
            btn_Input1.Enabled = true;
            tb_input2.Enabled = true;
            tb_input2.Text = getConditionInputDisplayText(con.Input2);
            btn_Input2.Enabled = true;
        }

        private void handleObjPattern(Condition con) {
            if (con == null) {
                return;
            }
            labelIType1.Text = "O";
            labelIType2.Text = "";
            tb_input1.Enabled = false;
            btn_Input1.Enabled = true;
            //tb_input1_updated = false;
            tb_input1.Text = getConditionInputDisplayText(con.Input1);             
            tb_input2.Enabled = false;
            //tb_input2_updated = false;
            tb_input2.Text = Constants.BLANK_TEXT;
            btn_Input2.Enabled = false;
        }
        private void disableInputArea() {
            labelIType1.Text = "";
            labelIType2.Text = "";
            tb_input1.Enabled = false;
            tb_input1.Text = string.Empty;
            btn_Input1.Enabled = false;
            tb_input2.Enabled = false;
            tb_input2.Text = string.Empty;
            btn_Input2.Enabled = false;
        }
        private void enableInputArea() { 
            labelIType1.Text = "";
            labelIType2.Text = "";
            tb_input1.Enabled = true;
            tb_input1.Text = string.Empty;
            btn_Input1.Enabled = true;
            tb_input2.Enabled = true;
            tb_input2.Text = string.Empty;
            btn_Input2.Enabled = true;
        }
        private void validateConditionInput(Condition con) {
            if (con == null) {
                return;
            }
            bool isError = false;
            String text = "";
            if (ModelManager.Instance.isStringPattern(con.COMPARE)){
                if(con.Input1 is Parameter){
                    Parameter p = con.Input1 as Parameter ;
                    if(p.Type == ParamType.FILE || p.Type == ParamType.SET){
                        isError = true;
                        text = "Condition input 1 should be a string ! ";
                    }
                }else if (con.Input2 is Parameter) {
                    Parameter p = con.Input2 as Parameter;
                    if (p.Type == ParamType.FILE || p.Type == ParamType.SET) {
                        isError = true;
                        text = "Condition input 2 should be a string ! ";
                    }
                }
            } else if (ModelManager.Instance.isNumberPattern(con.COMPARE)) {
                if (con.Input1 is Parameter) {
                    Parameter p = con.Input1 as Parameter;
                    if (p.Type == ParamType.FILE || p.Type == ParamType.SET) {
                        isError = true;
                        text = "Condition input 1 should be a number ! ";
                    }
                }
            } else if (ModelManager.Instance.isObjPattern(con.COMPARE)) {
                if (!(con.Input1 is WebElement)) {
                    isError = true;
                    text = "Condition input 1 should be a WebElement";
                } else if (con.Input2 != null) {
                    isError = true;
                    text = "Condition input 2 sould be empty ! ";
                }
            } else if (ModelManager.Instance.isSetPattern(con.COMPARE)) {
                if (con.Input1 is WebElement) {
                    isError = true;
                    text = "Condition input 1 should be a Set parameter. ";
                } else if (con.Input1 is Parameter) {
                    Parameter p = con.Input1 as Parameter;
                    if (p.Type != ParamType.SET) {
                        isError = true;
                        text = "Condition input 1 should be a Set parameter. ";
                    }
                } else if (con.Input2 is WebElement) {
                    isError = true;
                    text = "Condition input 2 should be string or nummer. ";
                } else if (con.Input2 is Parameter) {
                    Parameter p = con.Input2 as Parameter;
                    Parameter ps = con.Input1 as Parameter;
                    if (p.Type != ps.SetType) {
                        isError = true;
                        text = "Condition input 2 should be the same type with input1 items. ";
                    }
                }
            }
            if (isError) {
                label_msg.ForeColor = Color.Red;
            } else {
                label_msg.ForeColor = Color.Black;
            }
            label_msg.Text = text;
        } 
        private void btn_Input1_Click(object sender, EventArgs e) {
            if (tv_con.SelectedNode == null || tv_con.SelectedNode.Tag == null) {
                return;
            }
            Condition con = tv_con.SelectedNode.Tag as Condition;
            if (btn_Input1.Tag == null) {
                string flag = null;
                if (ModelManager.Instance.isSetPattern(con.COMPARE)) {
                    flag = "set";
                }
                Operation stubOp = getStubOp();
                ConditionInputDialog dlg = new ConditionInputDialog();
                DialogResult r = dlg.showConInputDialog(UIUtils.getTopControl(this), "", this.FlowPVManager.Bigmodel.SRoot,stubOp, con.COMPARE, flag);
                if (r == DialogResult.OK) {
                    con.Input1 = dlg.SelectedObj;
                    //tb_input1_updated = false;
                    tb_input1.Text = getConditionInputDisplayText(con.Input1);
                    updateInputType(con.Input1, labelIType1,con.COMPARE);

                    // fire model updated event
                    this.flowPVManager.raiseInputUpdatedEvt(this, input);
                    // validatoin the condition 
                    this.validateConditionInput(con);
                }
            } else if (btn_Input1.Tag.ToString().Equals("SET")) { 
                // handle SET. open a set edit dialog.
            }
        }
        
        private void btn_Input2_Click(object sender, EventArgs e) {
            Condition con = tv_con.SelectedNode.Tag as Condition;
            string flag = null;
            if (ModelManager.Instance.isSetPattern(con.COMPARE)) {
                flag = "elem";
            }
            ConditionInputDialog dlg = new ConditionInputDialog();
            Operation stubOp = getStubOp();
            DialogResult r = dlg.showConInputDialog(UIUtils.getTopControl(this), "", this.FlowPVManager.Bigmodel.SRoot, stubOp, con.COMPARE, flag);
            if (r == DialogResult.OK) {
                con.Input2 = dlg.SelectedObj;
                //tb_input2_updated = false;
                tb_input2.Text = getConditionInputDisplayText(con.Input2);
                updateInputType(con.Input2, labelIType2,con.COMPARE);

                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
                // do validation
                this.validateConditionInput(con);
            }
        }
        /// <summary>
        /// Get the OpCondition's owner operation/process
        /// </summary>
        /// <returns></returns>
        private Operation getStubOp() {
            Operation stubOp = null;
            if (this.input.Collection != null) {
                stubOp = this.input.Collection.Owner as Operation;
            }
            return stubOp;
        }

        private void ckbNot_Click(object sender, EventArgs e) {
            object obj = tv_con.SelectedNode.Tag ;            
            if (obj is Condition) {
                Condition con = obj as Condition;
                con.IsNot = ckb_Not.Checked;
                int index = getConImgIndex(con);
                tv_con.SelectedNode.ImageIndex = index;
                tv_con.SelectedNode.SelectedImageIndex = index;
            }
            if (obj is ConditionGroup) {
                ConditionGroup grp = obj as ConditionGroup;
                grp.IsNot = ckb_Not.Checked;
                int index = getConGrpImgIndex(grp);
                tv_con.SelectedNode.ImageIndex = index ;
                tv_con.SelectedNode.SelectedImageIndex = index;
            }
            // fire model updated event
            this.flowPVManager.raiseInputUpdatedEvt(this, input);
        }

        private void rbtn_AND_Click(object sender, EventArgs e) {
            object obj = tv_con.SelectedNode.Tag;
            if (obj is ConditionGroup) {
                ConditionGroup grp = obj as ConditionGroup;
                // update model 
                grp.Relation = CONDITION.AND;                
                // update UI 
                this.rbtn_OR.Checked = false;
                int index = getConGrpImgIndex(grp);
                this.tv_con.SelectedNode.ImageIndex = index;
                this.tv_con.SelectedNode.SelectedImageIndex = index;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }
        }

        private void rbtn_OR_Click(object sender, EventArgs e) {
            object obj = tv_con.SelectedNode.Tag;
            if (obj is ConditionGroup) {
                ConditionGroup grp = obj as ConditionGroup;
                // update model 
                grp.Relation = CONDITION.OR;
                // update UI 
                this.rbtn_AND.Checked = false;
                int index = getConGrpImgIndex(grp);
                this.tv_con.SelectedNode.ImageIndex = index;
                this.tv_con.SelectedNode.SelectedImageIndex = index;
                // fire model updated event
                this.flowPVManager.raiseInputUpdatedEvt(this, input);
            }            
        }
        #endregion ui methods 
        #region util methods
        /// <summary>
        /// Get the Condition image index for the condition list view 
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private static int getConImgIndex(Condition con) {
            //TODO here not touched the invalid status now. 
            int index = ICON_CON;
            if (con.IsNot) {
                index = ICON_CON_NOT;
            }
            return index;
        }
        /// <summary>
        /// /// Get the conditionGroup image index for the condition list view 
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        private static int getConGrpImgIndex(ConditionGroup grp) {
            int index = ICON_CONGRP_AND;
            if (grp.Relation == CONDITION.AND) {
                if (grp.IsNot) {
                    index = ICON_CONGRP_AND_NOT;
                }
            } else {
                index = ICON_CONGRP_OR;
                if (grp.IsNot) {
                    index = ICON_CONGRP_OR_NOT;
                }
            }

            return index;
        }
        private TreeNode createConNode(Condition con) {
            TreeNode node = new TreeNode();
            node.Text = con.Name;
            node.ToolTipText = con.Description;
            int index = getConImgIndex(con);
            node.ImageIndex = index;
            node.SelectedImageIndex = index;
            node.Tag = con;

            return node;
        }
        /// <summary>
        /// Create the Non-Root group node. 
        /// </summary>
        /// <param name="conGrp"></param>
        /// <returns></returns>
        private TreeNode createGrpNode(ConditionGroup conGrp) {
            TreeNode gnode = new TreeNode();
            gnode.Tag = conGrp;
            gnode.Text = conGrp.Name;
            gnode.ToolTipText = conGrp.Description;
            int index = getConGrpImgIndex(conGrp);
            gnode.ImageIndex = index;
            gnode.SelectedImageIndex = index;
            // create children node 
            foreach (BaseElement be in conGrp.Conditions) {
                if (be is ConditionGroup) {
                    ConditionGroup grp = be as ConditionGroup;
                    TreeNode gn = createGrpNode(grp);
                    gnode.Nodes.Add(gn);
                } else if (be is Condition) {
                    Condition con = be as Condition;
                    TreeNode cn = createConNode(con);
                    gnode.Nodes.Add(cn);
                }
            }
            return gnode;
        }

        private TreeNode createRootNode(ConditionGroup root) {
            TreeNode node = new TreeNode();
            node.Text = root.Name;
            node.Tag = root;
            node.ToolTipText = root.Description;
            int index = getConGrpImgIndex(root);
            node.ImageIndex = index;
            node.SelectedImageIndex = index;
            return node;
        }
        /// <summary>
        /// this method should exactly align with buildConditionPatterns method 
        /// return index or -1 if errors 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        internal int getConditionPatternIndex(CONDITION pattern) {
            int index = -1;
            string text = ModelManager.Instance.getPatternText(pattern);
            index = this.cb_pattern.Items.IndexOf(text);

            return index;
                        
        }

        private int getConditionCategoryIndex(CONDITION pattern) {
            int index = -1;
            string text = ModelManager.Instance.getPatternText(pattern);
            if (categoryStr.Contains(text)) {
                index = 0;
            } else if (categoryNum.Contains(text)) {
                index = 1;
            } else if (categoryObj.Contains(text)) {
                index = 2;
            } else if (categorySet.Contains(text)) {
                index = 3;
            }           

            return index;
        }
        #endregion util methods        
    }
}
