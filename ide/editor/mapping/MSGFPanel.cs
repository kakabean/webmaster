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
using WebMaster.ide.ui;
using WebMaster.lib.gf;
using WebMaster.com.script;

namespace WebMaster.ide.editor.mapping
{
    public partial class MSGFPanel : UserControl,IMappingSrc
    {
        /// <summary>
        /// Used for GF method which parameters are not fixed. it is used to identify the next ToBe added
        /// parameters. 
        /// </summary>
        private static string GF_COMMON_PARAM_FLAG = Constants.STR_START_FLAG+"ToBe Parameter"+Constants.STR_END_FLAG;
        /// <summary>
        /// It is used to in a new created GF when all its parameters is empty. If the relative parameters updated, 
        /// the placeholder itme will be replaced. It means that if there is placeholder parameter, the gf parameters are 
        /// not fully updated.
        /// </summary>
        private static string GF_PARAM_PLACE_HOLDER = Constants.STR_START_FLAG + "PlaceHolder" + Constants.STR_END_FLAG;
        #region variables         
        /// <summary>
        /// Mapping source type, this is the final type of the mapping source 
        /// </summary>
        private ParamType srcType = ParamType.STRING;
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
        /// Mapping source GlobalFunction, to be returned GlobalFunction object for invoker. 
        /// </summary>
        private GlobalFunction outputGF = null;

        // UI editor for parameters list 
        private Button ceButton = null;
        private Control[] lv_paramCE = null;
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int tv_gf_cmd_flag = 0 ;

        #endregion variables 
        #region events
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping GlobalFunction
        /// </summary>
        public event EventHandler<CommonEventArgs> MappingSrcChangedEvt;
        protected virtual void OnMappingSrcChangedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> mappingSrcChangedEvt = MappingSrcChangedEvt;
            if (mappingSrcChangedEvt != null) {
                mappingSrcChangedEvt(this, e);
            }
        }
        /// <summary>
        /// The sender is IMappingSrc panel, the data is mapping GlobalFunction
        /// </summary>
        /// <param name="sender">IMappingSrc panel</param>
        /// <param name="obj">GlobalFunction</param>
        public void raiseMappingSrcChangedEvt(Object sender, object obj) {
            if (obj != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, obj);
                OnMappingSrcChangedEvt(evt);
            }
        }
        #endregion events
        public MSGFPanel() {
            InitializeComponent();
            initUIData();
        }
        private void initUIData() {            
            // button cell editor
            this.ceButton = new Button();
            this.ceButton.Location = new System.Drawing.Point(32, 80);
            this.ceButton.Name = "ceButton";
            this.ceButton.Text = "...";
            this.ceButton.Size = new System.Drawing.Size(80, 21);
            this.ceButton.Visible = false;
            this.ceButton.Click +=new EventHandler(ceButton_Click);

            this.grpGF.Controls.Add(ceButton);

            lv_paramCE = new Control[] { null,null, ceButton };
            this.lv_params.setCellEditors(lv_paramCE);

            this.rtb_exp.ForeColor = Color.FromName(UIConstants.COLOR_MAPPING_EXP_TEXT);
        }
        void ceButton_Click(object sender, EventArgs e) {            
            if (lv_params.SelectedItems.Count != 1) {
                return;
            }
            ListViewItem lvi = lv_params.SelectedItems[0];            
            object param = lvi.Tag ;                        
            // show the parameter selection dialog 
            MappingParamSelectionDlg dlg = new MappingParamSelectionDlg();
            object tinput = param;
            if (isParamPlaceHolder(tinput)) {
                tinput = null;
            }
            DialogResult dr = dlg.showSelectionDlg(UIUtils.getTopControl(this), this.stubOp, this.sroot, tinput, this.srcType);
            if (dr == DialogResult.OK) {
                object np = dlg.Output;
                // update UI and raise updated outputGF event. 
                if (isParameterChanged(param,np)) {
                    // Currently edit a ToBe Number field, it means a new parameter will be added into
                    // the gf parameters. 
                    if (param is string && GF_COMMON_PARAM_FLAG == param.ToString()) {
                        // update model
                        if (this.outputGF.Params.Count > 0) {
                            if (this.outputGF.Params.Count == 1) {
                                // for a no fixed parameter number gf, if there is a placeholder, it must 
                                // be the first one. 
                                object placeholder = this.outputGF.Params.get(0);
                                if (isParamPlaceHolder(placeholder)) {
                                    this.outputGF.Params.Insert(0, np);
                                    this.outputGF.Params.RemoveAt(1);
                                    // remove the placeholder lvi
                                    this.lv_params.Items.RemoveAt(0);
                                } else {
                                    this.outputGF.Params.Add(np);
                                }
                            } else {
                                this.outputGF.Params.Add(np);
                            }                            
                        } else {
                            this.outputGF.Params.Add(np);
                        }
                        // update UI, create a LVI item for the new added parameter.  
                        ListViewItem nlvi = createGFParamLVI(this.outputGF, np, 0);
                        int index = lv_params.Items.IndexOf(lvi);
                        this.lv_params.Items.Insert(index, nlvi);
                    } else { // update existed parameter 
                        int index = lv_params.Items.IndexOf(lvi);
                        // update model
                        this.outputGF.Params.Insert(index, np);
                        this.outputGF.Params.RemoveAt(index + 1);
                        // update ListViewItem text 
                        string expression = ModelManager.Instance.getMappingSrcText(np);
                        this.ceButton.Text = expression;
                        //lvi.SubItems[2].Text = expression;
                    }
                    // handle output changed 
                    this.handleOutputChanged();
                }
            }
            this.lv_params.EndEditing(true);            
        }

        private bool isParameterChanged(object curr, object newobj) {
            return curr != newobj;
        }

        #region mandatory methods
        public string getExpression() {
            if (isValid()) {
                return ModelManager.Instance.getMappingSrcText(outputGF);
            } else {
                return string.Empty;
            }
        }

        public string getValidMsg() {
            string msg = null;
            if (this.stubOp == null || sroot == null) {
                msg = UILangUtil.getMsg("mapping.src.gf.err.msg0");
            } else if (outputGF == null) {
                msg = UILangUtil.getMsg("mapping.src.gf.err.msg1");
            } else if (srcType != this.outputGF.Type) {
                msg = UILangUtil.getMsg("mapping.src.gf.err.msg2");
            } else {
                // check place holders 
                foreach (object obj in this.outputGF.Params) {
                    if (isParamPlaceHolder(obj)) {
                        msg = UILangUtil.getMsg("mapping.src.gf.err.msg3");
                        break;
                    }
                }
                if (msg == null) {
                    ValidationMsg vmg = GFManager.getValidMsg(this.outputGF);
                    if (vmg.Type != MsgType.VALID) {
                        msg = vmg.Msg;
                    }
                }
            }
            return msg;
        }

        public bool isValid() {
            return getValidMsg() == null;
        }

        public object getMappingSrc() {
            if (isValid()) {
                return this.outputGF;
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
        private void updateSrcType(ParamType srcType) {
            if (srcType == ParamType.STRING || srcType == ParamType.NUMBER||srcType== ParamType.DATETIME) {
                this.srcType = srcType;
            }
        }
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
            // initialize and update outputGF
            GlobalFunction gf = null;
            if (src is GlobalFunction) {
                gf = src as GlobalFunction;
            }
            this.outputGF = createAndInitOutput(gf);
            // clean filter text box 
            this.tb_filter.Text = string.Empty;
            // clean and rebuild the gf tree based on the filter
            updateGFTree();
            // set the gf cmd selected 
            setGFSelected();                     
            // update validation message 
            this.lb_msg.Text = getValidMsg();
            // update expression msg            
            this.rtb_exp.Text = getExpression();
        }
        /// <summary>
        /// Create an GlobalFunction and initialize it the same as the gf. 
        /// </summary>
        /// <param name="inputGF"></param>
        /// <returns></returns>
        private GlobalFunction createAndInitOutput(GlobalFunction inputGF) {
            GlobalFunction gf = null;
            if (inputGF != null) {
                gf = inputGF.Clone();
            } else {
                gf = ModelFactory.createGlobalFunction();
            }

            return gf;
        }
        /// <summary>
        /// Clean the gf parameters and add full parameters with place holder instead.
        /// </summary>
        /// <param name="gf"></param>
        private void initGFParams(GlobalFunction gf) {
            // this is new GF 
            if (gf.Params.Count == 0) {
                if (gf.Cmd == GF_CMD.NUM_GET_AVG || gf.Cmd == GF_CMD.NUM_GET_MAX || gf.Cmd == GF_CMD.NUM_GET_MIN) {
                    gf.Params.Add(GF_PARAM_PLACE_HOLDER);
                } else {
                    int count = GFManager.getGF_CMDParamsCount(gf);
                    if (gf.Cmd == GF_CMD.STR_SPLIT) {
                        count = 2;
                    }

                    for (int i = 0; i < count; i++) {
                        gf.Params.Add(GF_PARAM_PLACE_HOLDER);
                    }

                }
            }
        }
        /// <summary>
        /// Whether the param obj is a parameter place holder, true: it is a place holder, false it is not. 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool isParamPlaceHolder(object param) {
            if (param is string && (param.ToString() == GF_PARAM_PLACE_HOLDER ||param.ToString() == GF_COMMON_PARAM_FLAG)) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Clean the GlobalFunction tree view and rebuild it based on the filter textbox values. 
        /// Only the filter matched GlobalFunctions can be shown.
        /// </summary>
        private void updateGFTree() {
            this.tv_GF_CMD.Nodes.Clear();
            this.tv_GF_CMD.BeginUpdate();

            if (srcType == ParamType.STRING) {
                // build up string gf cmd
                TreeNode psnode = createTopGFCategoryNode(GFManager.getGF_CategoryNameText(GFManager.GF_CATE_STR));
                this.tv_GF_CMD.Nodes.Add(psnode);
                List<TreeNode> snodes = UIUtils.createGFCategorySubNodes(this.tb_filter.Text, GFManager.getGF_CategoryNameText(GFManager.GF_CATE_STR));
                psnode.Nodes.AddRange(snodes.ToArray());
            } else if (srcType == ParamType.NUMBER) {
                // build up number gf cmd
                TreeNode pnnode = createTopGFCategoryNode(GFManager.getGF_CategoryNameText(GFManager.GF_CATE_NUM));
                this.tv_GF_CMD.Nodes.Add(pnnode);
                List<TreeNode> nnodes = UIUtils.createGFCategorySubNodes(this.tb_filter.Text, GFManager.getGF_CategoryNameText(GFManager.GF_CATE_NUM));
                pnnode.Nodes.AddRange(nnodes.ToArray());
            }
            this.tv_GF_CMD.EndUpdate();
        }
        /// <summary>
        /// Set the gf src matched command selected if found in current tree view. 
        /// </summary>
        private void setGFSelected() {
            if (this.outputGF != null) {
                foreach (TreeNode pn in this.tv_GF_CMD.Nodes) {
                    foreach (TreeNode n in pn.Nodes) {
                        GF_CMD cmd = (GF_CMD)n.Tag ;
                        if (cmd == this.outputGF.Cmd) {
                            this.tv_gf_cmd_flag = 2;
                            this.tv_GF_CMD.SelectedNode = n;
                            this.doTV_GF_CMD_AfterSelect();

                            return;
                        }
                    }
                }
            }
        }

        private TreeNode createTopGFCategoryNode(string text) {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.ImageKey = UIConstants.IMG_GF_CATE;
            node.SelectedImageKey = UIConstants.IMG_GF_CATE;
            node.Tag = text;

            return node;
        }

        private void updateGFParamList() {
            this.lv_params.Items.Clear();
            if (this.outputGF != null) {
                this.lv_params.BeginUpdate();
                if (this.outputGF.Cmd == GF_CMD.NUM_GET_AVG || this.outputGF.Cmd == GF_CMD.NUM_GET_MAX || this.outputGF.Cmd == GF_CMD.NUM_GET_MIN) {
                    initNumberUnknowCountParamList(this.outputGF);
                } else {
                    if (this.outputGF.Params != null) {
                        for (int i = 0; i < this.outputGF.Params.Count; i++) {
                            object param = this.outputGF.Params.get(i);

                            ListViewItem lvi = createGFParamLVI(this.outputGF, param, i);
                            this.lv_params.Items.Add(lvi);
                        }
                    }
                    // handle GF_CMD.STR_SPLIT 
                    if (this.outputGF.Cmd == GF_CMD.STR_SPLIT) { 
                        // add a specific TO-BE parameter
                        ListViewItem tlvi = createGFParamLVI(this.outputGF, GF_COMMON_PARAM_FLAG, 2);
                        tlvi.Text = "New...";
                        tlvi.ToolTipText = UILangUtil.getMsg("mapping.gf.btn.text1");// "Add new...";
                        this.lv_params.Items.Add(tlvi);
                    }
                }
                this.lv_params.EndUpdate();
            }
        }

        private void initNumberUnknowCountParamList(GlobalFunction gf) {
            if (this.outputGF.Cmd == GF_CMD.NUM_GET_AVG || this.outputGF.Cmd == GF_CMD.NUM_GET_MAX || this.outputGF.Cmd == GF_CMD.NUM_GET_MIN) {
                for (int i = 0; i < gf.Params.Count; i++) {
                    object param = null;
                    if (gf.Params != null && i < gf.Params.Count) {
                        param = gf.Params.get(i);
                    }
                    ListViewItem lvi = createGFParamLVI(gf, param, i);
                    this.lv_params.Items.Add(lvi);
                }
            }
            // add a specific TO-BE parameter
            ListViewItem tlvi = createGFParamLVI(gf, GF_COMMON_PARAM_FLAG, 0);
            tlvi.Text = "New...";
            tlvi.ToolTipText = UILangUtil.getMsg("mapping.gf.btn.text1");// "Add new...";
            this.lv_params.Items.Add(tlvi);
        }

        private ListViewItem createGFParamLVI(GlobalFunction gf, object obj,int position) {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = GFManager.getCmdParamNameText(gf.Cmd, position);
            lvi.Tag = obj;
            lvi.ToolTipText = GFManager.getCmdParamDesText(gf.Cmd, position);
            string type = srcType.ToString().ToLower(); //ModelManager.Instance.getParamTypeText(srcType);
            string value = ModelManager.Instance.getMappingSrcText(obj);
            if (obj is string && GF_COMMON_PARAM_FLAG == obj.ToString() || isParamPlaceHolder(obj)) {
                value = string.Empty;    
            }            
            lvi.SubItems.Add(type);
            lvi.SubItems.Add(value);

            return lvi;
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
        #endregion common methods 
        
        private void tv_GF_CMD_MouseDown(object sender, MouseEventArgs e) {
            this.tv_gf_cmd_flag = 1;
        }
        private void tv_GF_CMD_AfterSelect(object sender, TreeViewEventArgs e) {
            doTV_GF_CMD_AfterSelect();            
        }

        private void doTV_GF_CMD_AfterSelect() {
            if (this.tv_gf_cmd_flag == 0) {
                return;
            }
            TreeNode snode = this.tv_GF_CMD.SelectedNode;
            if (snode != null) {
                // update outputGF
                GF_CMD cmd = GFManager.getGF_CMD(snode.Text);
                // update output GF
                if (cmd != this.outputGF.Cmd) {
                    this.outputGF.Cmd = cmd;
                    if (this.tv_gf_cmd_flag == 1 && this.outputGF.Params!=null) {
                        this.outputGF.Params.Clear();
                    }
                }
                initGFParams(this.outputGF);
                // update GF parameters list view 
                this.updateGFParamList();
                if (this.tv_gf_cmd_flag == 1) {
                    handleOutputChanged();
                }
            }
            this.tv_gf_cmd_flag = 0;
        }
        
        private void grpGF_Resize(object sender, EventArgs e) {
            int p2MinW = 100;
            int p2MinH = 100;
            int p2w = (this.grpGF.Size.Width - this.grpGF.Padding.Left-this.grpGF.Padding.Right) / 2;
            int p2h = (this.grpGF.Size.Height - this.grpGF.Padding.Bottom- this.grpGF.Padding.Top);            
            p2w = p2w > p2MinW ? p2w : p2MinW;
            p2h = p2h > p2MinH ? p2h : p2MinH;
            this.panel2.Size = new Size(p2w,p2h);

            int tvH = this.panel2.Height - this.tb_filter.Height - tb_filter.Location.Y - this.panel2.Padding.Top - 3;
            this.tv_GF_CMD.Size = new Size(tv_GF_CMD.Size.Width,tvH);            
        }

        private void lv_params_SizeChanged(object sender, EventArgs e) {
            int w_type = 76;
            this.columnHeader2.Width = w_type;
            int w = this.lv_params.Width - w_type-5;
            if (w < 100) {
                w = 100;
            }
            this.columnHeader1.Width = w / 2;
            this.columnHeader3.Width = w / 2;
        }

        private void lv_params_SubItemClicked(object sender, WebMaster.com.SubItemEventArgs e) {
            Rectangle r = lv_params.GetSubItemBounds(e.Item, 2);
            Size size = new Size(24, -1);
            this.ceButton.Text = "...";
            this.lv_params.setAdjustCellCtrl(2, size);            
            lv_params.StartEditing(e.SubItem, e.Item);
        }

        private void panel4_Paint(object sender, PaintEventArgs e) {
            ControlPaint.DrawBorder(e.Graphics, this.panel4.ClientRectangle, Color.FromName(UIConstants.COLOR_MAPPING_EXP_BORDER), ButtonBorderStyle.Solid);
        }

        
    }
}
