using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;

namespace WebMaster.com.script
{
    /// <summary>
    /// This dialog is used to edit/maintain a Set Parameter. you can update the Set items 
    /// in this dialog. 
    /// </summary>
    public partial class ParamSetEditDialog : Form
    {
        #region variables 
        private Parameter input = null;
        /// <summary>
        /// back up set items in case to cancel the modification. 
        /// </summary>
        private List<object> backupList = new List<object>();
        /// <summary>
        /// Updated set items result. 
        /// </summary>
        public List<object> SetItems {
            get { return backupList; }
            //set { backupList = value; }
        }
        /// <summary>
        /// Whether to show the description area
        /// </summary>
        private bool isShowDes = false;
        #endregion variables 
        public ParamSetEditDialog() {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler">parent window</param>
        /// <param name="param">Set parameter </param>
        /// <returns></returns>
        public DialogResult showSetInputDialog(IWin32Window handler, Parameter param, bool showDes) {
            if (param == null || param.Type != ParamType.SET) {
                return DialogResult.Cancel;
            }
            this.isShowDes = showDes;
            this.setInput(param);
            //this.StartPosition = FormStartPosition.CenterParent;
            
            return this.ShowDialog(handler);
        }

        private void setInput(Parameter input){
            this.input = input;
            // update des area 
            if (isShowDes) {
                panelDes.Visible = true;
                tb_des.Text = input.Description;
            } else {
                panelDes.Visible = false;
            }
            backupSetItems();
            // update Set tree 
            upadateSetTree();
            updateBtns();
        }

        private void backupSetItems() {
            if (input.DesignSet != null) {
                foreach (object obj in input.DesignSet) {
                    backupList.Add(obj);
                }
            }
        }

        private void updateBtns() {
            if (treeView1.SelectedNode != null) {
                btn_delete.Enabled = true;
                btn_edit.Enabled = true;
            } else {
                btn_delete.Enabled = false;
                btn_edit.Enabled = false;
            }
        }

        private void upadateSetTree() {
            // clean tree 
            this.treeView1.Nodes.Clear();            
            // update tree 
            this.treeView1.BeginUpdate();
            foreach (object obj in backupList) {
                TreeNode node = new TreeNode();
                node.Text = ModelManager.Instance.getSetItemValue(obj);
                node.Tag = obj;
                this.treeView1.Nodes.Add(node);
            }
            this.treeView1.EndUpdate();
        }

        private void btn_add_Click(object sender, EventArgs e) {
            object item = null;
            ParamSetItemDialog dlg = new ParamSetItemDialog();
            DialogResult dr = dlg.showItemDialog(UIUtils.getTopControl(this), item, input.SetType);
            if (dr == System.Windows.Forms.DialogResult.OK) {
                item = dlg.Item;
                if (item != null) {
                    TreeNode tnode = this.treeView1.SelectedNode;
                    TreeNode node = new TreeNode();
                    node.Text = item.ToString();
                    node.Tag = item;
                    if (tnode == null) {
                        this.backupList.Add(item);                        
                        this.treeView1.Nodes.Add(node);
                    } else {
                        int index = this.treeView1.Nodes.IndexOf(tnode);
                        this.treeView1.Nodes.Insert(index, node);
                        this.backupList.Insert(index, item);
                    }
                }
            }
        }

        private void btn_edit_Click(object sender, EventArgs e) {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null) {
                object item = node.Tag;
                ParamSetItemDialog dlg = new ParamSetItemDialog();
                DialogResult dr = dlg.showItemDialog(UIUtils.getTopControl(this), item, input.SetType);
                if (dr == System.Windows.Forms.DialogResult.OK) {
                    item = dlg.Item;
                    if (item != null) {
                        node.Text = item.ToString();
                        node.Tag = item;
                        int index = this.treeView1.Nodes.IndexOf(node);
                        this.backupList.Insert(index, item);
                        this.backupList.RemoveAt(index + 1);
                    }
                }
            }
        }

        private void btn_delete_Click(object sender, EventArgs e) {
            TreeNode node = this.treeView1.SelectedNode;
            if (node != null) {
                int index = this.treeView1.Nodes.IndexOf(node);
                this.backupList.RemoveAt(index);
                this.treeView1.Nodes.Remove(node);
            }
        }

        private void btn_up_Click(object sender, EventArgs e) {
            TreeNode node = this.treeView1.SelectedNode;
            if (node.PrevNode != null) {
                TreeNode pnode = node.PrevNode;
                int index = this.treeView1.Nodes.IndexOf(node);
                this.treeView1.Nodes.Remove(node);
                this.backupList.RemoveAt(index);

                index = this.treeView1.Nodes.IndexOf(pnode);
                this.treeView1.Nodes.Insert(index, node);
                this.backupList.Insert(index, node.Tag);
            }
            this.treeView1.SelectedNode = node;
        }

        private void btn_down_Click(object sender, EventArgs e) {
            TreeNode node = this.treeView1.SelectedNode;
            if (node.NextNode != null) {
                TreeNode nnode = node.NextNode;
                int index = this.treeView1.Nodes.IndexOf(nnode);
                this.treeView1.Nodes.Remove(nnode);
                this.backupList.RemoveAt(index);

                index = this.treeView1.Nodes.IndexOf(node);
                this.treeView1.Nodes.Insert(index, nnode);
                this.backupList.Insert(index, nnode.Tag);
            }
            this.treeView1.SelectedNode = node;
        }

        private void btn_OK_Click(object sender, EventArgs e) {
            
        }

        private void btn_Cancel_Click(object sender, EventArgs e) {
            
        }
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            TreeNode node = this.treeView1.SelectedNode;
            if (node == null) {
                this.btn_edit.Enabled = false;
                this.btn_delete.Enabled = false;
                this.btn_up.Enabled = false;
                this.btn_down.Enabled = false;
            } else {
                this.btn_edit.Enabled = true;
                this.btn_delete.Enabled = true;
                if (node.PrevNode == null) {
                    this.btn_up.Enabled = false;
                } else {
                    this.btn_up.Enabled = true;
                }
                if (node.NextNode == null) {
                    this.btn_down.Enabled = false;
                } else {
                    this.btn_down.Enabled = true;
                }
            }
        }

    }
}
