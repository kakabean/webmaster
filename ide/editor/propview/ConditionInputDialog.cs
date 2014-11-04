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
    public partial class ConditionInputDialog : Form
    {                
        //private static ConditionInputDialog instance = new ConditionInputDialog();

        private ScriptRoot sroot = null;
        private CONDITION pattern = CONDITION.EMPTY;
        private object selectedObj = null;
        /// <summary>
        /// this is the selected condition input object, it can be WebElement/WebElementAttribute/Parameter
        /// </summary>
        public object SelectedObj {
            get { return this.selectedObj; }
            set { selectedObj = value; }
        }
        /// <summary>
        /// Stub op for Condition or Operation input dialog. 
        /// </summary>
        private Operation op = null;
        /// <summary>
        /// Whether the dialog is a Operaton Input dialog 
        /// </summary>
        private bool isOpInput = false;
        /// <summary>
        /// it take effect when the pattern is SET pattern, candidate value is "set","elem".  
        /// </summary>
        private string setHelp = "set";
        private RadioButton checkedBtn = null;

        public ConditionInputDialog() {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="text"></param>
        /// <param name="sroot"></param>
        /// <param name="op">stub op</param>
        /// <param name="pattern"></param>
        /// <param name="set">It take effect when the pattern is SET pattern, candidate value is "set","elem"</param>
        /// <returns></returns>
        public DialogResult showConInputDialog(IWin32Window handler, String text, ScriptRoot sroot,Operation op, CONDITION pattern, string set) {
            this.isOpInput = false;
            resetConInputUI();
            this.checkedBtn = null;
            if (text != null && text.Length > 0) {
                this.Text = text;
            }
            if (sroot == null || op == null) {
                return DialogResult.Cancel;
            } else {
                this.sroot = sroot;
                this.op = op;
            }
            this.pattern = pattern;
            this.setHelp = set;

            this.initTree(sroot, pattern);
            this.StartPosition = FormStartPosition.CenterParent;

            return this.ShowDialog(handler);
        }

        private void resetConInputUI() {
            this.panel5.Visible = true;
            this.treeView1.Nodes.Clear();
            cleanDetailsArea();
        }

        private void resetOpInputUI() {
            this.panel5.Visible = false;
            this.treeView1.Nodes.Clear();
            cleanDetailsArea();
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
        /// show operation input dialog, the allowed input element is parameter 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="text"></param>
        /// <param name="sroot"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public DialogResult showOpInputDialog(IWin32Window handler, String text, ScriptRoot sroot, Operation op) {
            this.isOpInput = true;
            resetOpInputUI();            
            if (text != null && text.Length > 0) {
                this.Text = text;
            }
            if (op == null || sroot == null) {
                return DialogResult.Cancel;
            } else {
                this.op = op;
                this.sroot = sroot;
            }

            this.updateParameterTree(this.sroot);
            this.StartPosition = FormStartPosition.CenterParent;

            return this.ShowDialog(handler);
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
                string type = "string";
                if(isOpInput){
                    type = "all";
                }else if (ModelManager.Instance.isNumberPattern(pattern)) {
                    type = "number";
                } else if (ModelManager.Instance.isSetPattern(pattern)) {
                    if (setHelp == "set") {
                        type = "set";
                    } else {
                        type = "all";
                    }
                }
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
            // For condition input, make sure the sensitive parameters are filterred
            if (this.isOpInput == false) {
                UIUtils.removeSensitiveParam(node);
            }

            return node;
        }

        /// <summary>
        /// show the condition input tree based on the script root and selected pattern
        /// 
        /// </summary>
        /// <param name="sroot"></param>
        /// <param name="pattern"></param>
        private void initTree(ScriptRoot sroot, CONDITION pattern) {
            if (sroot == null || pattern == CONDITION.EMPTY) {
                //LOG
                return;
            }
            if (ModelManager.Instance.isObjPattern(pattern)) {
                this.rbtn_we.Enabled = true;
                this.rbtn_att.Enabled = false;
                this.rbtn_param.Enabled = false;
                this.rbtn_we.Checked = true;
                updateWETree(sroot,false,"all");
            } else if (ModelManager.Instance.isNumberPattern(pattern)) {
                this.rbtn_we.Enabled = false;
                this.rbtn_att.Enabled = true;
                this.rbtn_param.Enabled = true;
                this.rbtn_param.Checked = true;

                updateNumberTree(sroot);
            } else if (ModelManager.Instance.isStringPattern(pattern)) {
                this.rbtn_we.Enabled = false;
                this.rbtn_att.Enabled = true;
                this.rbtn_param.Enabled = true;
                this.rbtn_att.Checked = true;

                updateStrTree(sroot);
            } else if (ModelManager.Instance.isSetPattern(pattern)) {
                this.rbtn_we.Enabled = false;
                this.rbtn_att.Enabled = true;
                this.rbtn_param.Enabled = true;
                this.rbtn_att.Checked = true;

                updateParameterTree(sroot);
            }
        }
        /// <summary>
        /// show the WebElement as the leaf node in the tree, if showAttribute == true, it will show the WebElementAttribute node 
        /// by the type filter. 
        /// </summary>
        /// <param name="sroot"></param>
        /// <param name="showAttribute"></param>
        /// <param name="type"></param>
        private void updateWETree(ScriptRoot sroot, bool showAttribute, string type) {
            this.treeView1.Nodes.Clear();

            this.treeView1.BeginUpdate();
            // clean existed nodes 
            if (this.treeView1.Nodes.Count > 0) {
                this.treeView1.Nodes.Clear();
            }
            // build new tree             
            List<TreeNode> nodes = UIUtils.createWEGSubNodes(sroot.WERoot, showAttribute, type, UIConstants.IMG_WEG, UIConstants.IMG_WEG, UIConstants.IMG_WE, UIConstants.IMG_WE, UIConstants.IMG_WEA, UIConstants.IMG_WEA);
            this.treeView1.Nodes.AddRange(nodes.ToArray());
            // remove the password WebElement if it is used for Condition
            if (isOpInput == false) {                
                UIUtils.removeWEPassword(this.treeView1);
            }
            //this.expendWERootTree();

            this.treeView1.EndUpdate();
        }

        private void updateNumberTree(ScriptRoot sroot) {
            this.treeView1.Nodes.Clear();
            if (rbtn_att.Checked) {
                updateWETree(sroot, true, "number");
            } else if (rbtn_param.Checked) {
                updateParameterTree(sroot);
            }
        }

        private void updateStrTree(ScriptRoot sroot) {
            this.treeView1.Nodes.Clear();
            if (rbtn_att.Checked) {
                updateWETree(sroot, true, "string");
            } else if (rbtn_param.Checked) {
                updateParameterTree(sroot);
            }
        }
        private void updateParameterTree(ScriptRoot sroot) {
            this.treeView1.Nodes.Clear();
            List<Process> candidateProcs = getCandidateProcs4Param(sroot);
            this.treeView1.BeginUpdate();
            foreach (Process proc in candidateProcs) {
                TreeNode node = buildProcNode(proc);
                this.treeView1.Nodes.Add(node);
            }
            this.treeView1.EndUpdate();

            this.treeView1.Nodes[0].ExpandAll();
        }
        /// <summary>
        /// Get candidate process list for current condition.
        /// </summary>
        /// <param name="sroot"></param>
        /// <returns></returns>
        private List<Process> getCandidateProcs4Param(ScriptRoot sroot) {
            List<Process> procs = new List<Process>();
            // check Operation itself
            if (isOpInput == false) {
                if (op is Process) {
                    Process sproc = op as Process;
                    procs.Insert(0,sproc);
                }
            }
            // check container process
            if (this.op != sroot.ProcRoot) {
                Process cproc = ModelManager.Instance.getOwnerProc(this.op);
                procs.Insert(0, cproc);
            }
            // add script root process
            if (procs.Count == 0 || sroot.ProcRoot != procs[0]) {
                procs.Insert(0, sroot.ProcRoot);
            }
            return procs;
        }

        /// <summary>
        /// expend the WebElement tree root 
        /// </summary>
        internal void expendWERootTree() {
            if (this.treeView1.Nodes.Count > 0) {
                this.treeView1.Nodes[0].Expand();
            }
        }
        private void rbtn_we_CheckedChanged(object sender, EventArgs e) {
            if (rbtn_we != this.checkedBtn && rbtn_we.Checked) {                
                this.updateWETree(sroot,false,"all");
                this.checkedBtn = rbtn_we;
                cleanDetailsArea();
                this.btn_OK.Enabled = false;
            }
        }

        private void rbtn_att_CheckedChanged(object sender, EventArgs e) {
            if (rbtn_att!=this.checkedBtn && rbtn_att.Checked) {
                this.treeView1.Nodes.Clear();
                string type = "string";
                if (ModelManager.Instance.isNumberPattern(this.pattern)) {
                    type = "number";
                }
                this.updateWETree(sroot, true, type);
                this.checkedBtn = rbtn_att;
                cleanDetailsArea();
                this.btn_OK.Enabled = false;
            }
        }

        private void rbtn_param_CheckedChanged(object sender, EventArgs e) {
            if (rbtn_param != checkedBtn && rbtn_param.Checked) {
                if (ModelManager.Instance.isNumberPattern(pattern)) {
                    updateNumberTree(sroot);
                } else if (ModelManager.Instance.isStringPattern(pattern)) {
                    updateStrTree(sroot);
                } else if (ModelManager.Instance.isSetPattern(pattern)) {
                    updateParameterTree(sroot);
                }
                this.checkedBtn = rbtn_param;
                
                cleanDetailsArea();

                this.btn_OK.Enabled = false;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            Object tag = this.treeView1.SelectedNode.Tag;
            // condition input 
            if (isOpInput == false) {
                if (ModelManager.Instance.isObjPattern(pattern)) {
                    if (tag is WebElement) {
                        btn_OK.Enabled = true;
                    } else {
                        btn_OK.Enabled = false;
                    }
                } else if (ModelManager.Instance.isNumberPattern(pattern)) {
                    if (tag is WebElementAttribute) {
                        btn_OK.Enabled = true;
                    } else if (tag is Parameter) {
                        Parameter p = tag as Parameter;
                        if (p.Type == ParamType.NUMBER) {
                            this.btn_OK.Enabled = true;
                        } else {
                            this.btn_OK.Enabled = false;
                        }
                    } else {
                        this.btn_OK.Enabled = false;
                    }

                } else if (ModelManager.Instance.isStringPattern(pattern)) {
                    if (tag is WebElementAttribute) {
                        btn_OK.Enabled = true;
                    } else if (tag is Parameter) {
                        Parameter p = tag as Parameter;
                        if (p.Type == ParamType.STRING) {
                            this.btn_OK.Enabled = true;
                        } else {
                            this.btn_OK.Enabled = false;
                        }
                    } else {
                        this.btn_OK.Enabled = false;
                    }
                }
            } else { 
                // operation input 
                if (tag is Parameter) {
                    this.btn_OK.Enabled = true;
                } else {
                    this.btn_OK.Enabled = false;
                }
            }

            // update UI info 
            BaseElement be = tag as BaseElement;
            //if (be == null && tag is String){
            //    be = this.treeView1.SelectedNode.Parent.Tag as BaseElement;
            //}
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
                    tb_value.Text = p.DesignValue+"";
                }
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e) {
            this.btn_OK.PerformClick();
        }

        private void btn_OK_Click(object sender, EventArgs e) {
            SelectedObj = this.treeView1.SelectedNode.Tag;
        }
    }
}
