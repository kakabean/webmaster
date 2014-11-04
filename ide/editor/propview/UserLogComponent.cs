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
using WebMaster.lib;
using WebMaster.lib.ui;
using WebMaster.com;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    /// <summary>
    /// This properties view is used to show log info about Process and Operation.
    /// 
    /// </summary>
    public partial class UserLogComponent : UserControl
    {
        /// <summary>
        /// acceptable input can be Process and Operation
        /// </summary>
        private BaseElement input = null;
        /// <summary>
        /// only take effect if the input is Process, 0 : log start, 1 : log end 
        /// </summary>
        private int proc_logFlag = -1;
        /// <summary>
        /// this one is used to fixed a defect, when selected an Operation, the default properties view is not user log, 
        /// then turn to UserLog view, the log string richTextBox will show all strings with the same color, the color the 
        /// same as the first item's color. 
        /// This is used to only fix this problem. 
        /// </summary>
        private bool firstShow = false;

        private FlowPropViewManager flowPVManager = null;

        public FlowPropViewManager FlowPVManager {
            get { return flowPVManager; }
            set { flowPVManager = value; }
        }
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int tb_item_flag = 1;

        public UserLogComponent() {
            InitializeComponent();
        }

        #region UI methods 
        /// <summary>
        /// clean the view field as initialized 
        /// </summary>
        internal void cleanView() {
            this.rb_ignore.Checked = true;
            this.rb_log.Checked = false;
            this.treeView1.Nodes.Clear();
            this.tb_item_flag = 2;
            this.tb_item.Text = string.Empty;
            this.richTextBox1.ResetText();
        }

        internal void enableView() {
            this.rb_ignore.Enabled = true;
            this.rb_log.Enabled = true;
            this.treeView1.Enabled = true;            
            this.btn_time.Enabled = true;
            this.btn_Obj.Enabled = true;
            this.btn_const.Enabled = true;
            this.btn_color.Enabled = false;
            this.tb_item.Enabled = true;
            this.button1.Enabled = true;
            this.richTextBox1.Enabled = true;
        }

        internal void disableView() {
            this.rb_ignore.Enabled = false;
            this.rb_log.Enabled = false;
            this.treeView1.Enabled = false;
            this.btn_time.Enabled = false;
            this.btn_Obj.Enabled = false;
            this.btn_const.Enabled = false;
            this.tb_item.Enabled = false;
            this.button1.Enabled = false;
            this.richTextBox1.Enabled = false;
        }

        private void btn_time_Click(object sender, EventArgs e) {
            TreeNode tn = this.treeView1.SelectedNode;
            TreeNode node = new TreeNode(UILangUtil.getMsg("view.log.time.text1"));
            node.ImageKey = UIConstants.IMG_TIME;
            node.SelectedImageKey = UIConstants.IMG_TIME;
            DateTime time = new DateTime();
            UserLogItem item = ModelFactory.createUserLogItem(time, UserLogItem.DEFAULT_COLOR);
            node.Tag = item;

            int index = -1 ;
            if (tn != null) {
                index = this.treeView1.Nodes.IndexOf(this.treeView1.SelectedNode);
                if (index >= 0 && index < this.treeView1.Nodes.Count) {
                    index += 1;
                }
            }
            if (index == -1 || index >= this.treeView1.Nodes.Count || this.treeView1.Nodes.Count == 0) {
                this.treeView1.Nodes.Add(node);
            } else {
                this.treeView1.Nodes.Insert(index, node);
            }

            // set the new node selected 
            this.treeView1.SelectedNode = node;
            // update model
            UserLog log = this.getUserLog();
            if (tn != null) {
                index = this.treeView1.Nodes.IndexOf(this.treeView1.SelectedNode);
                if (index >= 0 && index < this.treeView1.Nodes.Count) {
                    index += 1;
                }
            }
            
            if (index == -1 || index >= this.treeView1.Nodes.Count || this.treeView1.Nodes.Count == 0) {                
                log.LogItems.Add(item);
            } else {
                log.LogItems.Insert(index, item);
            }

            this.raiseModelUpdated();
            // update the rich text string
            this.updateLogString(log);
            // update text area 
            this.tb_item_flag = 2;
            this.tb_item.Text = UILangUtil.getMsg("view.log.time.text1");// Constants.LOG_TIME;
            this.tb_item.Enabled = false;
        }

        private void btn_Obj_Click(object sender, EventArgs e) {
            BaseElement be = getSelectedElement();
            if (be is WebElementAttribute || be is Parameter) {
                TreeNode tn = this.treeView1.SelectedNode;
                TreeNode node = new TreeNode(be.Name);
                UserLogItem item = ModelFactory.createUserLogItem(be, UserLogItem.DEFAULT_COLOR);
                node.Tag = item;
                if (be is WebElementAttribute) {
                    node.ImageKey = UIConstants.IMG_WEA;
                    node.SelectedImageKey = UIConstants.IMG_WEA;
                } else if (be is Parameter) {
                    node.ImageKey = UIConstants.IMG_PARAM;
                    node.SelectedImageKey = UIConstants.IMG_PARAM;
                }
                int index = -1;
                if (tn != null) {
                    index = this.treeView1.Nodes.IndexOf(this.treeView1.SelectedNode);
                    if (index >= 0 && index < this.treeView1.Nodes.Count) {
                        index += 1;
                    }
                }
                if (index == -1 || index >= this.treeView1.Nodes.Count || this.treeView1.Nodes.Count == 0) {
                    this.treeView1.Nodes.Add(node);
                } else {
                    this.treeView1.Nodes.Insert(index, node);
                }

                // set the new node selected 
                this.treeView1.SelectedNode = node;
                // set the text area 
                this.tb_item_flag = 2;
                this.tb_item.Text = be.Name;
                // update model
                UserLog log = this.getUserLog();
                if (index == -1 || index >= this.treeView1.Nodes.Count || this.treeView1.Nodes.Count == 0) {
                    log.LogItems.Add(item);
                } else {
                    log.LogItems.Insert(index, item);
                }

                this.raiseModelUpdated();
                // update the rich text string
                this.updateLogString(log);
            }
        }
        
        private void btn_const_Click(object sender, EventArgs e) {
            TreeNode tn = this.treeView1.SelectedNode;
            string text = UILangUtil.getMsg("view.log.item.newstr");
            TreeNode node = new TreeNode(text);
            node.ImageKey = UIConstants.IMG_STR;
            node.SelectedImageKey = UIConstants.IMG_STR;
            UserLogItem item = ModelFactory.createUserLogItem(text, UserLogItem.DEFAULT_COLOR);
            node.Tag = item;
            int index = -1;
            if (tn != null) {
                index = this.treeView1.Nodes.IndexOf(this.treeView1.SelectedNode);
                if (index >= 0 && index < this.treeView1.Nodes.Count) {
                    index += 1;
                }                     
            }
            if (index == -1 || index >= this.treeView1.Nodes.Count || this.treeView1.Nodes.Count == 0 ) {
                this.treeView1.Nodes.Add(node);
            } else {
                this.treeView1.Nodes.Insert(index, node);
            }
            // set the new node selected 
            this.treeView1.SelectedNode = node;
            // update model
            UserLog log = this.getUserLog();            
            if (index == -1 || index >= this.treeView1.Nodes.Count || this.treeView1.Nodes.Count == 0) {
                log.LogItems.Add(item);
            } else {
                log.LogItems.Insert(index, item);
            }
            
            this.raiseModelUpdated();
            // update the rich text string
            this.updateLogString(log);
            // set the focus to text edit area
            this.tb_item_flag = 2;
            this.tb_item.Text = item.Item.ToString();
            this.tb_item.Enabled = true;
            this.button1.Enabled = false;
            this.tb_item.Focus();
        }

        private void btn_color_Click(object sender, EventArgs e) {
            if (this.treeView1.SelectedNode.Tag is UserLogItem) {
                UserLogItem item = this.treeView1.SelectedNode.Tag as UserLogItem;
                DialogResult dr = colorDialog1.ShowDialog(UIUtils.getTopControl(this));
                if (dr == DialogResult.OK) {
                    Color clr = colorDialog1.Color;
                    item.Color = clr.ToArgb();
                    UserLog log = getUserLog();
                    this.updateLogString(log);
                    this.raiseModelUpdated();
                }
            }
        }
        private void rb_ignore_Click(object sender, EventArgs e) {
            UserLog log = getUserLog();

            if (log != null) {
                string text = UILangUtil.getMsg("view.log.warn.text");
                string title = UILangUtil.getMsg("view.log.warn.label");
                DialogResult dr = MessageBoxEx.showMsgDialog(UIUtils.getTopControl(this), text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, FormStartPosition.CenterParent);
                if (dr == DialogResult.Yes) {
                    cleanUserLog();
                    // notify that the model is updated
                    raiseModelUpdated();
                    disableView();
                    cleanView();
                    this.rb_ignore.Enabled = true;
                    this.rb_log.Enabled = true;
                } else {
                    rb_ignore.Checked = false;
                }
            }
        }

        private void rb_log_Click(object sender, EventArgs e) {
            this.rb_ignore.Checked = false;
            enableView();
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e) {
            TreeNode snode = this.treeView1.GetNodeAt(e.Location);
            if (snode!= null && snode != this.treeView1.SelectedNode) {
                this.treeView1.SelectedNode = snode;
                updateItemDetails(snode.Tag as UserLogItem);
            }
            // update color button 
            if (this.treeView1.SelectedNode != null && this.treeView1.SelectedNode.Tag is UserLogItem) {
                this.btn_color.Enabled = true;
            } else {
                this.btn_color.Enabled = false;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right && this.treeView1.Enabled == true) {                
                if (snode != null) {                   
                    int index = this.treeView1.Nodes.IndexOf(snode);
                    this.tsmi_up.Enabled = index == 0 ? false : true;
                    this.tsmi_down.Enabled = index == this.treeView1.Nodes.Count - 1 ? false : true;
                    this.cms_tree.Show(this.treeView1, e.X, e.Y);
                }                
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            Log.println_prop("tree view selected node = " + e.Node.Text);
            if (e.Node.Tag != null && e.Node.Tag is UserLogItem) {
                this.btn_color.Enabled = true;
            } else {
                this.btn_color.Enabled = false;
            }
            object o = e.Node.Tag;
            updateItemDetails(o as UserLogItem);
        }
        /// <summary>
        /// get the selecetec WebElementAttribute or Parameter as the Log item or return null if not selected 
        /// </summary>
        /// <returns></returns>
        private BaseElement getSelectedElement() {
            UserLogItemDialog dlg = new UserLogItemDialog();
            string title = UILangUtil.getMsg("view.log.selectobj.title");
            DialogResult dr = dlg.showLogItemDialog(UIUtils.getTopControl(this), title, this.input as Operation, this.FlowPVManager.Bigmodel.SRoot);
            if (dr == DialogResult.OK) {
                return dlg.SelectedObj as BaseElement;
            } else {
                return null;
            }
        }
        /// <summary>
        /// update item details area 
        /// </summary>
        /// <param name="o"></param>
        private void updateItemDetails(UserLogItem item) {            
            this.tb_item.Enabled = false;
            this.button1.Enabled = false;
            if (item == null || item.Item == null) {
                return;
            }
            object o = item.Item;
            this.tb_item_flag = 2;
            if (o is DateTime) {
                this.tb_item.Text = UILangUtil.getMsg("view.log.time.text1");                
            } else if (o is WebElementAttribute) {
                WebElementAttribute wea = o as WebElementAttribute;
                this.tb_item.Text = wea.Name;
                this.button1.Enabled = true;
            } else if (o is Parameter) {
                Parameter p = o as Parameter;
                this.tb_item.Text = p.Name;
                this.button1.Enabled = true;
            } else if (o is string) {
                this.tb_item.Text = o.ToString();                
                this.tb_item.Enabled = true;
            }
        }
        private void tsmi_delete_Click(object sender, EventArgs e) {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null) {
                // update tree 
                TreeNode selectedNode = node.NextNode;
                if (selectedNode == null) {
                    selectedNode = node.PrevNode;
                }
                this.treeView1.Nodes.Remove(node);
                if (selectedNode != null) {
                    this.treeView1.SelectedNode = selectedNode;
                }
                // update log.
                UserLog log = getUserLog();
                if (log != null) {
                    ModelManager.Instance.removeFromModel(node.Tag as UserLogItem);
                    if (log.LogItems.Count == 0) {
                        disableView();
                        this.rb_ignore.Checked = true;
                        this.rb_ignore.Enabled = true;
                        this.rb_log.Checked = false;
                        this.rb_log.Enabled = true;
                        cleanUserLog();
                    }
                    // update ui info
                    if (this.treeView1.SelectedNode != null) {
                        updateItemDetails(this.treeView1.SelectedNode.Tag as UserLogItem);
                    } else {
                        this.tb_item_flag = 2;
                        this.tb_item.Text = string.Empty;
                    }
                }
                raiseModelUpdated();
                // update log string 
                updateLogString(log);
            }
        }

        private void tsmi_down_Click(object sender, EventArgs e) {
            this.treeView1.BeginUpdate();
            TreeNode node = this.treeView1.SelectedNode;
            int index = this.treeView1.Nodes.IndexOf(node);
            if (index >= 0 && index < this.treeView1.Nodes.Count - 1) {
                // update tree node 
                TreeNode nn = node.NextNode;
                this.treeView1.Nodes.Remove(nn);
                this.treeView1.Nodes.Insert(index, nn);
                this.treeView1.SelectedNode = node;
                // update model 
                UserLog log = getUserLog();
                if (log != null) {
                    log.LogItems.Remove(nn.Tag as UserLogItem);
                    log.LogItems.Insert(index, nn.Tag as UserLogItem);
                    // update editor status.
                    this.raiseModelUpdated();
                }
                // update log string 
                this.updateLogString(log);
            }
            this.treeView1.EndUpdate();
        }

        private void tsmi_up_Click(object sender, EventArgs e) {
            this.treeView1.BeginUpdate();
            TreeNode node = this.treeView1.SelectedNode;
            int index = this.treeView1.Nodes.IndexOf(node);
            if (index > 0 && index < this.treeView1.Nodes.Count) {
                // update tree node 
                TreeNode nn = node.PrevNode;
                this.treeView1.Nodes.Remove(nn);
                this.treeView1.Nodes.Insert(index, nn);
                this.treeView1.SelectedNode = node;
                // update model 
                UserLog log = getUserLog();
                if (log != null) {
                    log.LogItems.Remove(nn.Tag as UserLogItem);
                    log.LogItems.Insert(index, nn.Tag as UserLogItem);                    
                    // update editor status.
                    this.raiseModelUpdated();
                }
                // update log string 
                this.updateLogString(log);
            }
            this.treeView1.EndUpdate();
        }

        private void tb_item_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.treeView1.Focus();
                return;
            }
            this.tb_item_flag = 1;
        }

        private void tb_item_TextChanged(object sender, EventArgs e) {
            if (tb_item_flag == 2) {
                tb_item_flag = 1;
                return;
            }
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null || node.Tag == null || !((node.Tag as UserLogItem).Item is string)) {
                return;
            }

            node.Text = this.tb_item.Text;
            UserLogItem item = node.Tag as UserLogItem;
            item.Item = this.tb_item.Text;
            // update log string 
            UserLog log = this.getUserLog();
            this.updateLogString(log);
            // update model 
            this.raiseModelUpdated();
        }

        private void button1_Click(object sender, EventArgs e) {
            BaseElement be = getSelectedElement();
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null && (be is WebElementAttribute || be is Parameter)) {
                // set the new node selected 
                node.Text = be.Name;
                UserLogItem item = ModelFactory.createUserLogItem(be, UserLogItem.DEFAULT_COLOR);
                node.Tag = item;
                // set the text area 
                this.tb_item_flag = 2;
                this.tb_item.Text = be.Name;
                // update model
                UserLog log = this.getUserLog();
                int index = this.treeView1.Nodes.IndexOf(node);
                // make sure the color is the same with updated item.
                item.Color = log.LogItems.get(index).Color;
                
                log.LogItems.Insert(index, item);
                log.LogItems.RemoveAt(index + 1);
                // update editor dirty 
                this.raiseModelUpdated();
                // update the rich text string
                this.updateLogString(log);
            }
        }
        /// <summary>
        /// it is used to fix a defect, ref firstShow variables description. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupBox3_Paint(object sender, PaintEventArgs e) {
            if (firstShow == false) {
                UserLog log = getUserLog();
                this.updateLogString(log);
                firstShow = true;
            }
        }       
        #endregion UI methods 
        #region common methods 
        /// <summary>
        /// if the input is Process the procLog parameter will take effect. 0 : process start log, 1 : process end log 
        /// </summary>
        /// <param name="input">Input must be Process or Operation</param>
        /// <param name="procLog">0 : process start log, 1 : process end log </param>
        internal void setInput(BaseElement input, int procLog) {            
            this.input = input;
            
            this.cleanView();
            if (input is Process) {
                this.proc_logFlag = procLog;
                updateInput();
            } else if (input is Operation) {
                updateInput();
            }
        }
        private void updateInput() {
            UserLog log = null;
            if (input is Process) {
                Process proc = input as Process;
                if (proc_logFlag == 0) {
                    this.groupBox1.Text = UILangUtil.getMsg("view.log.proc.start.text");// UIConstants.PROP_LOG_PROC_START;                    
                    log = proc.LogStart;
                } else if (proc_logFlag == 1) {
                    this.groupBox1.Text = UILangUtil.getMsg("view.log.proc.end.text");// UIConstants.PROP_LOG_PROC_END;
                    log = proc.LogEnd;
                }
            } else if (input is Operation) {
                Operation op = input as Operation;
                this.groupBox1.Text = UILangUtil.getMsg("view.log.op.end.text");// UIConstants.PROP_LOG_OP;
                log = op.LogEnd;
            }
            // update the radion buttons 
            rb_ignore.Checked = false;
            rb_log.Checked = false;
            if (log != null) {
                rb_log.Checked = true;
                this.treeView1.Enabled = true;
            } else {
                rb_ignore.Checked = true;
                disableView();
                rb_ignore.Enabled = true;
                rb_log.Enabled = true;
            }
            // update the Log tree view 
            this.updateLogTree(log);
            // update the details area 
            this.tb_item_flag = 2;
            this.tb_item.Text = string.Empty;
            this.tb_item.Enabled = false;
            this.button1.Enabled = false;
            // update log string 
            this.updateLogString(log);
        }
        private void updateLogTree(UserLog log) {
            if (log == null) {
                this.treeView1.Nodes.Clear();
            } else {
                buildLogTree(log);
            }
        }
        /// <summary>
        /// build the log tree
        /// </summary>
        /// <param name="log"></param>
        private void buildLogTree(UserLog log) {            
            // clear all nodes 
            this.treeView1.Nodes.Clear();

            this.treeView1.BeginUpdate();
            foreach (UserLogItem item in log.LogItems) {                
                if (item.Item is DateTime) {
                    buildTimeNode(item);
                } else if (item.Item is WebElementAttribute) {
                    buildWEANode(item);
                } else if (item.Item is Parameter) {
                    buildParamNode(item);
                } else if (item.Item is string) {
                    buildStringNode(item);
                }
            }
            this.treeView1.EndUpdate();
        }
        private void buildStringNode(UserLogItem item) {
            TreeNode tn = new TreeNode();
            tn.Text = item.Item.ToString();
            tn.ImageKey = UIConstants.IMG_STR;
            tn.SelectedImageKey = UIConstants.IMG_STR;
            tn.Tag = item;
            this.treeView1.Nodes.Add(tn);
        }
        private void buildParamNode(UserLogItem item) {
            TreeNode tn = new TreeNode();
            tn.Text = (item.Item as Parameter).Name;
            tn.ImageKey = UIConstants.IMG_PARAM;
            tn.SelectedImageKey = UIConstants.IMG_PARAM;
            tn.Tag = item;
            this.treeView1.Nodes.Add(tn);
        }
        private void buildWEANode(UserLogItem item ){
            TreeNode tn = new TreeNode();
            WebElementAttribute wea = item.Item as WebElementAttribute;
            tn.Text = wea.Name;
            tn.ImageKey = UIConstants.IMG_WEA;
            tn.SelectedImageKey = UIConstants.IMG_WEA;
            tn.Tag = item;
            this.treeView1.Nodes.Add(tn);
        }
        private void buildTimeNode(UserLogItem item) {
            TreeNode tn = new TreeNode();
            tn.Text = UILangUtil.getMsg("view.log.time.text1");// Constants.LOG_TIME;
            tn.ImageKey = UIConstants.IMG_TIME;
            tn.SelectedImageKey = UIConstants.IMG_TIME;
            tn.Tag = item;
            this.treeView1.Nodes.Add(tn);
        }
        private void updateLogString(UserLog log) {
            this.richTextBox1.ResetText();
            if (log != null) {
                foreach (UserLogItem item in log.LogItems) {
                    object o = item.Item;
                    if (o is DateTime) {
                        buildTimeText(item.Color);
                    } else if (o is WebElementAttribute) {
                        buildWEAText(o as WebElementAttribute,item.Color);
                    } else if (o is Parameter) {
                        buildParamText(o as Parameter, item.Color);
                    } else if (o is string) {
                        buildStringText(o.ToString(), item.Color);
                    } 
                }
            }
        }
        private void buildStringText(string p, int color) {
            this.richTextBox1.SelectionColor = ModelManager.Instance.getLogColor(color,Constants.LOG_USER_KEY_STR, this.FlowPVManager.Bigmodel.SRoot);
            this.richTextBox1.AppendText(p);
        }
        private void buildParamText(Parameter parameter, int color) {
            this.richTextBox1.SelectionColor = ModelManager.Instance.getLogColor(color, Constants.LOG_USER_KEY_PARAM, this.FlowPVManager.Bigmodel.SRoot);
            this.richTextBox1.AppendText(parameter.Name);
        }
        private void buildWEAText(WebElementAttribute wea, int color) {
            this.richTextBox1.SelectionColor = ModelManager.Instance.getLogColor(color, Constants.LOG_USER_KEY_WEA, this.FlowPVManager.Bigmodel.SRoot);
            this.richTextBox1.AppendText(wea.Name);
        }
        private void buildTimeText(int color) {
            this.richTextBox1.SelectionColor = ModelManager.Instance.getLogColor(color, Constants.LOG_USER_KEY_TIME, this.FlowPVManager.Bigmodel.SRoot);
            this.richTextBox1.AppendText(UILangUtil.getMsg("view.log.time.text1"));
        }
        /// <summary>
        /// get current input user log that is shown now
        /// </summary>
        /// <returns></returns>
        private UserLog getUserLog() {
            UserLog log = null;
            if (input is Process) {
                Process proc = input as Process;
                if (proc_logFlag == 0) {
                    log = proc.LogStart;
                    if (log == null && this.rb_log.Checked) {
                        log = ModelFactory.createUserLog();
                        proc.LogStart = log;
                    }
                } else if (proc_logFlag == 1) {
                    log = proc.LogEnd;
                    if (log == null && this.rb_log.Checked) {
                        log = ModelFactory.createUserLog();
                        proc.LogEnd = log;
                    }
                }
            } else if (input is Operation) {
                Operation op = input as Operation;
                log = op.LogEnd;
                if (log == null && this.rb_log.Checked) {
                    log = ModelFactory.createUserLog();
                    op.LogEnd = log;
                }
            }

            return log;
        }
        /// <summary>
        /// clean the model updated user log info. 
        /// </summary>
        private void cleanUserLog() {
            if (input is Process) {
                Process proc = input as Process;
                if (proc_logFlag == 0) {
                    proc.LogStart = null;                    
                } else if (proc_logFlag == 1) {
                    proc.LogEnd = null;                    
                }
            } else if (input is Operation) {
                Operation op = input as Operation;
                op.LogEnd = null;                
            }
        }
        private void raiseModelUpdated() {
            this.FlowPVManager.raiseInputUpdatedEvt(this.Parent.Parent, input);
        }
        #endregion common methods 
                
    }
}
