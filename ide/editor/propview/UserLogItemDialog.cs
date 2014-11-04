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
    public partial class UserLogItemDialog : Form
    {
        private Operation stubOp = null;
        private ScriptRoot sroot = null;
        private RadioButton checkedBtn = null;

        private object selectedObj = null;
        /// <summary>
        /// this is the selected LogItem input object, it can be WebElementAttribute/Parameter
        /// </summary>
        public object SelectedObj {
            get { return this.selectedObj; }
            set { selectedObj = value; }
        }

        public UserLogItemDialog() {
            InitializeComponent();
        }
        /// <summary>
        /// show the available WebElementAttributes or Parameters for the operation/process log
        /// the candidate WebElementAttribute will be the operation's WebElement's attribute
        /// the parmameter will be the process parameter 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public DialogResult showLogItemDialog(IWin32Window handler, String text, Operation op, ScriptRoot sroot) {            
            this.checkedBtn = null;
            if (text != null && text.Length > 0) {
                this.Text = text;
            }
            if (op == null || sroot == null) {
                return DialogResult.Cancel;
            } else {
                this.stubOp = op;
                this.sroot = sroot;
            }

            this.initTree();
            this.StartPosition = FormStartPosition.CenterParent;
            
            return this.ShowDialog(handler);
        }
        
        private void cleanDetailsArea() {
            this.tb_name.Text = string.Empty;
            this.tb_des.Text = string.Empty;
            if (this.checkedBtn == rbtn_param) {
                this.label3.Text = UILangUtil.getMsg("dlg.conInput.value.text2");
            } else {
                this.label3.Text = UILangUtil.getMsg("dlg.conInput.value.text1");
            }
            this.tb_value.Text = string.Empty;
        }

        /// <summary>
        /// show the WebElementAttribute tree based on the operaiton's input element 
        /// 
        /// </summary>
        private void initTree() {
            if(stubOp == null){
                return;
            }
            this.rbtn_att.Checked = false;
            this.rbtn_param.Checked = false;
            this.treeView1.BeginUpdate();
            // update WebElementAttriubte tree 
            if (stubOp.Element is WebElement) {
                updateWEAttributeTree();                
                this.rbtn_att.Checked = true;
            } else {
                this.rbtn_param.Checked = true;
                Process proc = ModelManager.Instance.getOwnerProc(stubOp);                
                this.treeView1.Nodes.Clear();
                TreeNode pnode = this.buildProcNode(proc);
                this.treeView1.Nodes.Add(pnode);
                if (proc != this.sroot.ProcRoot) {
                    TreeNode snode = this.buildProcNode(this.sroot.ProcRoot);
                    this.treeView1.Nodes.Insert(0, snode);
                }                
            }
            
            this.treeView1.EndUpdate();
            
            if (this.treeView1.Nodes.Count > 0) {
                this.treeView1.Nodes[0].Expand();
            }
        }
        /// <summary>
        /// Build the process node and all its private and public parameters 
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        private TreeNode buildProcNode(Process proc) {
            TreeNode node = new TreeNode();
            if (proc != null) {
                node.Text = proc.Name;
                node.ToolTipText = proc.Description;
                node.Tag = proc;
                node.ImageKey = UIConstants.IMG_OP_PROC;
                node.SelectedImageKey = UIConstants.IMG_OP_PROC;
                
                string type = "all";                
                // build public parameters 
                TreeNode pubNode = UIUtils.createSingleParamGrpNode(proc.ParamPublic, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP);
                node.Nodes.Add(pubNode);
                List<TreeNode> pnodes = UIUtils.createParamGrpSubNodes(proc.ParamPublic, type, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARAM, UIConstants.IMG_PARAM);
                pubNode.Nodes.AddRange(pnodes.ToArray());
                // build private parameters 
                TreeNode priNode = UIUtils.createSingleParamGrpNode(proc.ParamPrivate, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP);
                node.Nodes.Add(priNode);
                List<TreeNode> pinodes = UIUtils.createParamGrpSubNodes(proc.ParamPrivate, type, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARAM, UIConstants.IMG_PARAM);
                priNode.Nodes.AddRange(pinodes.ToArray());
            } else {
                node.Text = "SysError!";
            }

            // remove sensitive parameter if have 
            UIUtils.removeSensitiveParam(node);

            return node;
        }
        private void updateWEAttributeTree() {
            this.treeView1.Nodes.Clear();
            WebElement we = this.stubOp.Element as WebElement;
            if (we == null || we.isPassword) {
                return;
            }                                
            if (we.TYPE == WEType.CODE) {
                foreach (WebElementAttribute wea in we.Attributes) {
                    if ("text".Equals(wea.Name)) {
                        TreeNode caNode = new TreeNode();
                        caNode.Text = "text";
                        caNode.ToolTipText = we.Description;
                        caNode.Tag = we.FeatureString;
                        caNode.ImageKey = UIConstants.IMG_WEA;
                        caNode.SelectedImageKey = UIConstants.IMG_WEA;
                        this.treeView1.Nodes.Add(caNode);
                        break;
                    }
                }
            } else if (we.TYPE == WEType.ATTRIBUTE) {
                foreach (WebElementAttribute wea in we.Attributes) {
                    TreeNode attNode = new TreeNode();
                    attNode.Text = wea.Name;
                    attNode.ToolTipText = wea.Description;
                    attNode.Tag = wea;
                    attNode.ImageKey = UIConstants.IMG_WEA;
                    attNode.SelectedImageKey = UIConstants.IMG_WEA;

                    this.treeView1.Nodes.Add(attNode);
                }
            }                  
        }
        
        private void rbtn_att_Click(object sender, EventArgs e) {
            if (rbtn_att != this.checkedBtn && rbtn_att.Checked) {
                this.updateWEAttributeTree();
                this.checkedBtn = rbtn_att;
                cleanDetailsArea();
                this.btn_OK.Enabled = false;
            }
        }
        /// <summary>
        /// for a Process, it only can reach itself defined parameters, 
        /// for an operaton, it only can reach its container process's parameters 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtn_param_Click(object sender, EventArgs e) {
            if (rbtn_param != checkedBtn && rbtn_param.Checked) {
                Process proc = ModelManager.Instance.getOwnerProc(stubOp);
                this.treeView1.Nodes.Clear();
                TreeNode node = this.buildProcNode(proc);
                this.treeView1.Nodes.Add(node);
                if (proc != this.sroot.ProcRoot) {
                    TreeNode snode = this.buildProcNode(this.sroot.ProcRoot);
                    this.treeView1.Nodes.Insert(0, snode);
                }    
                this.checkedBtn = rbtn_param;
                cleanDetailsArea();
                this.btn_OK.Enabled = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            Object tag = this.treeView1.SelectedNode.Tag;            
            if (tag is Parameter || tag is WebElementAttribute) {
                this.btn_OK.Enabled = true;
            } else {
                this.btn_OK.Enabled = false;
            }

            // update UI info 
            BaseElement be = tag as BaseElement;            
            tb_name.Text = be.Name;
            tb_des.Text = be.Description;
            if (be is WebElementAttribute) {
                WebElementAttribute wea = be as WebElementAttribute;
                label3.Text = UILangUtil.getMsg("dlg.conInput.value.text1");
                tb_value.Text = ModelManager.Instance.getWEAText4Design(wea);
            } else {
                label3.Text = UILangUtil.getMsg("dlg.conInput.value.text2");
            }
            if (be is Parameter) {
                Parameter p = be as Parameter;
                if (p.Type == ParamType.SET) {
                    tb_value.Text = p.ToString();
                } else {
                    tb_value.Text = p.DesignValue + "";
                }
            }
        }
        
        private void treeView1_DoubleClick(object sender, EventArgs e) {
            MouseEventArgs me = e as MouseEventArgs;
            if (me != null) {
                TreeNode snode = this.treeView1.GetNodeAt(me.X, me.Y);
                if (snode != null) {
                    Object tag = snode.Tag;
                    if (tag is Parameter || tag is WebElementAttribute) {
                        this.btn_OK.PerformClick();
                        return;
                    }
                }
            } 

            this.btn_Cancel.PerformClick();            
        }

        private void btn_OK_Click(object sender, EventArgs e) {
            this.SelectedObj = this.treeView1.SelectedNode.Tag;
        }
    }
}
