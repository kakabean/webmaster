using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor.propview
{
    public partial class WEListDialog : Form
    {
        private WebElement selectedWE = null;
        /// <summary>
        /// Selected WebElement or null if no one selected 
        /// </summary>
        public WebElement SelectedWE {
            get { return selectedWE; }
            //set { selectedWE = value; }
        }
 
        public WEListDialog() {
            InitializeComponent();
        }

        public DialogResult showWEDialog(IWin32Window handler,String text,ScriptRoot sroot) {
            this.Text = text;
            this.initUI(sroot);
            this.StartPosition = FormStartPosition.CenterParent;
            return this.ShowDialog(handler);
        }
        /// <summary>
        /// 1. Clean the WebElement TreeView list 
        /// 2. Reset buttons status 
        /// 3. Clean details area 
        /// </summary>
        /// <param name="sroot"></param>
        private void initUI(ScriptRoot sroot) {
            if (sroot == null) {
                return;
            }
            // 1. Clean the WebElement TreeView list 
            this.treeView1.BeginUpdate();
            // clean existed nodes 
            if (this.treeView1.Nodes.Count > 0) {
                this.treeView1.Nodes.Clear();
            }
            // build new tree 
            List<TreeNode> nodes = UIUtils.createWEGSubNodes(sroot.WERoot, false, "all", UIConstants.IMG_WEG, UIConstants.IMG_WEG, UIConstants.IMG_WE, UIConstants.IMG_WE, UIConstants.IMG_WEA, UIConstants.IMG_WEA);
            this.treeView1.Nodes.AddRange(nodes.ToArray());
            this.treeView1.EndUpdate();

            this.expendWERootTree();
            // 2. Reset buttons status     
            this.btn_OK.Enabled = false;
            this.btn_Cancel.Enabled = true;
            // 3. Clean details area 
            this.tb_name.Text = string.Empty;
            this.tb_des.Text = string.Empty;
        }        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            if (treeView1.SelectedNode.Tag is WebElement) {
                this.btn_OK.Enabled = true;
                selectedWE = this.treeView1.SelectedNode.Tag as WebElement;
            } else {
                this.btn_OK.Enabled = false;
            }
            BaseElement be = treeView1.SelectedNode.Tag as BaseElement;
            this.tb_name.Text = be.Name;
            this.tb_des.Text = be.Description;
        }

        /// <summary>
        /// expend the WebElement tree root 
        /// </summary>
        internal void expendWERootTree() {
            if (this.treeView1.Nodes.Count > 0) {
                this.treeView1.Nodes[0].Expand();
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e) {
            // Only if the selected node is a WebElement, DClick will close the dialog 
            if (this.treeView1.SelectedNode != null && this.treeView1.SelectedNode.Tag is WebElement) {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;            
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e) {
            TreeNode node = this.treeView1.GetNodeAt(e.X, e.Y);
            if (this.treeView1.SelectedNode!=node) {
                this.treeView1.SelectedNode = node;
            }
            if (this.treeView1.SelectedNode == null || !(this.treeView1.SelectedNode.Tag is WebElement)) {
                this.btn_OK.Enabled = false;
            } else {
                WebElement twe = this.treeView1.SelectedNode.Tag as WebElement;
                if (checkPasswordPassed(twe)) {
                    selectedWE = twe;
                    this.btn_OK.Enabled = true;
                } else {
                    this.btn_OK.Enabled = false;
                }
            }
        }
        /// <summary>
        /// True: if WE is a password and it is not used by any operation as element.
        /// else return false.
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        private bool checkPasswordPassed(WebElement we) {
            if (we != null && we.isPassword) {
                foreach (object obj in we.WeakRef) {
                    if (obj is Operation) {
                        Operation op = obj as Operation;
                        if (op.Element == we) {
                            return false;
                        }
                    } else if (obj is ListRef) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
