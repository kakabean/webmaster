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
    public partial class ParamListDialog : Form
    {
        private ScriptRoot sroot = null;
        private Parameter selectedObj = null;
        /// <summary>
        /// This is the selected Parameter object
        /// </summary>
        public Parameter Output {
            get { return this.selectedObj; }
            set { selectedObj = value; }
        }

        public ParamListDialog() {
            InitializeComponent();
        }
        /// <summary>
        /// show the all Parameters for the in the ScriptRoot. 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sroot"></param>
        /// <returns></returns>
        public DialogResult showParamDialog(IWin32Window handler, String text, ScriptRoot sroot) {                        
            if (text != null && text.Length > 0) {
                this.Text = text;
            }
            if (sroot == null) {
                return DialogResult.Cancel;
            } else {
                this.sroot = sroot;
            }

            this.initTree();
            this.cleanDetailsArea();
            this.btn_OK.Enabled = false;

            this.StartPosition = FormStartPosition.CenterParent;
            
            return this.ShowDialog(handler);
        }
        
        private void cleanDetailsArea() {
            this.tb_name.Text = string.Empty;
            this.tb_des.Text = string.Empty;
            this.label3.Text = UILangUtil.getMsg("dlg.conInput.value.text2");
            this.tb_value.Text = string.Empty;
        }

        /// <summary>
        /// show the parameters in list tree view 
        /// 
        /// </summary>
        private void initTree() {
            this.treeView1.Nodes.Clear();
            this.treeView1.BeginUpdate();
            List<Process> proclist = getProperProcs(sroot);
            foreach(Process proc in proclist){                            
                TreeNode pnode = this.buildProcNode(proc);
                this.treeView1.Nodes.Add(pnode);                             
            }
            
            this.treeView1.EndUpdate();
            
            if (this.treeView1.Nodes.Count > 0) {
                this.treeView1.Nodes[0].Expand();
            }
        }
        /// <summary>
        /// Add all processes under the script that has parameters defined or empty list if none find. 
        /// </summary>
        /// <param name="sroot"></param>
        /// <returns></returns>
        private List<Process> getProperProcs(ScriptRoot sroot) {
            List<Process> procs = new List<Process>();
            buildUpProcs(sroot.ProcRoot,procs);
            
            return procs;
        }

        private void buildUpProcs(Process proc, List<Process> procs) {
            if (ModelManager.Instance.hasParameter(proc)) {
                procs.Add(proc);
            }
            foreach (Process tproc in proc.Procs) {
                buildUpProcs(tproc, procs);
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            Object tag = this.treeView1.SelectedNode.Tag;            
            if (tag is Parameter) {
                this.btn_OK.Enabled = true;
            } else {
                this.btn_OK.Enabled = false;
            }

            // update UI info 
            BaseElement be = tag as BaseElement;            
            tb_name.Text = be.Name;
            tb_des.Text = be.Description;
            
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
                    if (tag is Parameter) {
                        this.btn_OK.PerformClick();
                        return;
                    }
                }
            } 

            this.btn_Cancel.PerformClick();            
        }

        private void btn_OK_Click(object sender, EventArgs e) {
            if (this.treeView1.SelectedNode.Tag is Parameter) {
                this.Output = this.treeView1.SelectedNode.Tag as Parameter;
            }else{
                this.Output = null ;
            }
        }
    }
}
