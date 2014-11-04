using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor.mapping
{
    public partial class MSParamPanel : UserControl,IMappingSrc
    {
        #region variables 
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int cb_procs_flag = 0;
        /// <summary>
        /// This is the stub operation/Process of the mapping source .
        /// a stub operation is the operation who triggerred the update parameter mapping 
        /// or OpCondition's owner that OpCondition triggerred the parameters mapping 
        /// </summary>
        private Operation stubOp = null;
        /// <summary>
        /// Script Root 
        /// </summary>
        private ScriptRoot sroot = null;
        /// <summary>
        /// Mapping source type, this is the final type of the mapping source 
        /// </summary>
        private ParamType srcType = ParamType.STRING;
        /// <summary>
        /// mapping source - parameter, tobe returned parameter for the invoker. 
        /// </summary>
        private Parameter outputParam = null;
        /// <summary>
        /// Candidate process that can be allowed to show parameters as the mapping source 
        /// </summary>
        private List<Process> candidateProcs = null;
        #endregion variables 
        #region events
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping Parameter
        /// </summary>
        public event EventHandler<CommonEventArgs> MappingSrcChangedEvt;
        protected virtual void OnMappingSrcChangedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> mappingSrcChangedEvt = MappingSrcChangedEvt;
            if (mappingSrcChangedEvt != null) {
                mappingSrcChangedEvt(this, e);
            }
        }
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping Parameter
        /// </summary>
        /// <param name="sender">IMappingSrc panel</param>
        /// <param name="obj">Parameter</param>
        public void raiseMappingSrcChangedEvt(Object sender, object obj) {
            if (obj != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnMappingSrcChangedEvt(evt);
            }
        }
        #endregion events
        public MSParamPanel() {
            InitializeComponent();
            this.rtb_exp.ForeColor = Color.FromName(UIConstants.COLOR_MAPPING_EXP_TEXT);
        }
        #region mandatory methods
        public string getExpression() {
            if (isValid()) {
                return ModelManager.Instance.getMappingSrcText(outputParam);
            } else {
                return string.Empty;
            }
        }

        public string getValidMsg() {
            string msg = null;
            if (this.stubOp == null || sroot == null) {
                msg = UILangUtil.getMsg("mapping.src.param.err.msg0");
            } else if (outputParam == null) {
                msg = UILangUtil.getMsg("mapping.src.param.err.msg1");
            } else {
                if (srcType == ParamType.STRING) {
                    if (ModelManager.Instance.isMaybeStringValue(outputParam)) {
                        msg = null;
                    }
                } else if (srcType == ParamType.NUMBER) {
                    if (ModelManager.Instance.isMaybeNumberValue(outputParam)) {
                        msg = null;
                    } else {
                        msg = UILangUtil.getMsg("mapping.src.param.err.msg2");
                    }                    
                } else {
                    msg = UILangUtil.getMsg("mapping.src.const.err.msg3");
                }
            }
            return msg;
        }

        public bool isValid() {
            return getValidMsg() == null; 
        }
        /// <summary>
        /// Get mapping source or null if errors 
        /// </summary>
        /// <returns></returns>
        public object getMappingSrc() {
            if (isValid()) {
                return outputParam;
            }
            return null;
        }
        
        public void show(object src, ParamType srcType) {
            // update mapping source type info 
            updateSrcType(srcType);
            // update view with input 
            setInput(src);
            this.Visible = true;
        }

        public void hidden() {
            this.Visible = false;
        }
        #endregion mandatory methods 
        #region common methods
        /// <summary>
        /// set the stub operation, a stub operation is the operation who triggerred the update parameter mapping 
        /// or OpCondition's owner that OpCondition triggerred the parameters mapping 
        /// </summary>
        /// <param name="op">stub operation</param>
        /// <param name="sroot"></param>
        public void setStubOp(Operation op, ScriptRoot sroot) {
            this.stubOp = op;
            this.sroot = sroot;
        }
        private void setInput(object src) {
            // initialized and update outputParam
            Parameter inputParam = null;
            if (src is Parameter) {
                inputParam = src as Parameter;
            }
            this.outputParam = createAndInitOutput(inputParam);
            // update cb_process box 
            initCB_ProcBox();         
            // build param tree 
            buildParamTree(this.cb_procs.Tag as Process);            
            // update validation message 
            this.lb_msg.Text = getValidMsg();
            // update expression msg 
            this.rtb_exp.Text = getExpression();            
            // update the WEA selected status 
            if (this.outputParam != null) {
                setParamSelected(outputParam);
            } else {
                tv_params.ExpandAll();
            }
        }
        /// <summary>
        /// Create an Parameter and initialize it the same as the inputParam. 
        /// </summary>
        private Parameter createAndInitOutput(Parameter inputParam) {
            //Parameter param = null;
            //if (inputParam != null) {
            //    param = inputParam.Clone();
            //} else {
            //    param = ModelFactory.createParameter();
            //}
            //return param;
            return inputParam;
        }
        /// <summary>
        /// Initial the cb_proc box and set the proper process as parameter owner.
        /// </summary>
        private void initCB_ProcBox() {            
            this.cb_procs.Items.Clear();
            // build up candidate process list 
            this.candidateProcs = new List<Process>();
            if (this.stubOp is Process) {
                candidateProcs.Add(this.stubOp as Process);
            }
            Process proc1 = ModelManager.Instance.getOwnerProc(this.stubOp);
            if (proc1 != null) {
                candidateProcs.Add(proc1);
            }
            if (candidateProcs.Count > 0) {
                Process pp = candidateProcs[candidateProcs.Count - 1];
                if (pp != this.sroot.ProcRoot) {
                    this.candidateProcs.Add(this.sroot.ProcRoot);
                }
            }
            // reorder the candidateProces, make sure the Root Proc is the first one. 
            if (candidateProcs.Count > 0) {
                candidateProcs.Reverse();
            }
            // build up items for cb_proc box 
            string[] pps = new string[candidateProcs.Count];
            for (int i = 0; i < candidateProcs.Count; i++) {
                pps[i] = candidateProcs[i].Name;
            }
            this.cb_procs.Items.AddRange(pps);
            // set default selected process 
            this.cb_procs_flag = 2;
            this.cb_procs.SelectedIndex = getSelectedIndex();
            this.doCB_procs_SelectedIndexChanged();
        }
        /// <summary>
        /// Get proper index based on the paramSrc or -1 if errors 
        /// </summary>
        /// <returns></returns>
        private int getSelectedIndex() {            
            int index = -1 ;
            if (this.candidateProcs != null && this.candidateProcs.Count > 0) {
                if (outputParam != null) {
                    for (int i = 0; i < this.candidateProcs.Count; i++) {
                        Process proc = this.candidateProcs[i];
                        ParamGroup pgrp = ModelManager.Instance.getRootParamGroup(outputParam);
                        if (proc.ParamPublic == pgrp || proc.ParamPrivate == pgrp) {
                            index = i;
                        }
                    }
                } else {
                    index = 0;
                }
            }

            return index;
        }
        private void setParamSelected(Parameter paramSrc) {
            if (this.outputParam != null) {
                TreeNode node = UIUtils.getTreeNodeByTag(this.tv_params,paramSrc);
                if (node != null) {
                    this.tv_params.SelectedNode = node;
                }
            }
        }

        private void buildParamTree(Process proc) {
            this.tv_params.Nodes.Clear();
            if (proc != null) {
                this.tv_params.BeginUpdate();
                string type = "string";
                if (this.srcType == ParamType.NUMBER) {
                    type = "number";
                } else if (this.srcType == ParamType.DATETIME) {
                    type = "datetime";
                }
                
                TreeNode pubNode = UIUtils.createSingleParamGrpNode(proc.ParamPublic, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP);
                this.tv_params.Nodes.Add(pubNode);
                List<TreeNode> pnodes = UIUtils.createParamGrpSubNodes(proc.ParamPublic, type, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARAM, UIConstants.IMG_PARAM);
                pubNode.Nodes.AddRange(pnodes.ToArray());

                TreeNode priNode = UIUtils.createSingleParamGrpNode(proc.ParamPrivate, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP);
                this.tv_params.Nodes.Add(priNode);
                List<TreeNode> pinodes = UIUtils.createParamGrpSubNodes(proc.ParamPrivate, type, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARMGRP, UIConstants.IMG_PARAM, UIConstants.IMG_PARAM);
                priNode.Nodes.AddRange(pinodes.ToArray());

                // remove sensitive parameter node if have 
                UIUtils.removeSensitiveParam(tv_params);

                this.tv_params.EndUpdate();
            }
        }
                
        private void updateSrcType(ParamType srcType) {
            if (srcType == ParamType.STRING || srcType == ParamType.NUMBER) {
                this.srcType = srcType;
            }
        }
       
        #endregion common methods   
        #region ui methods 
        private void cb_procs_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_procs_flag = 1;
        }
        private void cb_procs_SelectedIndexChanged(object sender, EventArgs e) {
            this.doCB_procs_SelectedIndexChanged();
        }

        private void doCB_procs_SelectedIndexChanged() {
            if (this.cb_procs_flag == 0) {
                return;
            }
            // update process to tag 
            if (this.cb_procs.SelectedIndex == -1) {
                this.cb_procs.Tag = null;
            } else { 
                this.cb_procs.Tag = this.candidateProcs[this.cb_procs.SelectedIndex];
            }
            // update text 
            UIUtils.updateComboBoxText(this.cb_procs);
            buildParamTree(this.cb_procs.Tag as Process);
            this.tv_params.ExpandAll();
            this.cb_procs_flag = 0;
        }

        private void tv_params_MouseDown(object sender, MouseEventArgs e) {
            TreeNode tn = this.tv_params.GetNodeAt(new Point(e.X, e.Y));
            if (tn != null) {
                bool changed = false;
                // update parameter 
                if (tn.Tag is Parameter) {
                    if (this.outputParam != tn.Tag) {
                        this.outputParam = tn.Tag as Parameter;
                        changed = true;
                    }
                } else {
                    if (this.outputParam != null) {                     
                        this.outputParam = null;
                        changed = true;
                    }
                }
                if (changed) {
                    this.handleOutputChanged();
                }
            }
        }
        /// <summary>
        /// Update message text, expression text and raiseMappingSrcChangedEvt. 
        /// </summary>
        private void handleOutputChanged() {
            // update validation msg 
            this.lb_msg.Text = getValidMsg();
            // update expression 
            this.rtb_exp.Text = getExpression();

            this.raiseMappingSrcChangedEvt(this, this.getMappingSrc());
        }      
        #endregion ui methods 

        private void panel3_Paint(object sender, PaintEventArgs e) {
            ControlPaint.DrawBorder(e.Graphics, this.panel3.ClientRectangle, Color.FromName(UIConstants.COLOR_MAPPING_EXP_BORDER), ButtonBorderStyle.Solid);
        }
        
    }
}
