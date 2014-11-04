using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib;
using WebMaster.lib.engine;
using WebMaster.lib.ui;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor.mapping
{
    public partial class ParamMappingDlg : Form
    {
        #region variables 
        /// <summary>
        /// allowed input will be
        /// 1. Operation/Process, used for parameters update 
        /// 2. OpCondition, which target op must be process. 
        /// </summary>
        private object input = null;
        /// <summary>
        /// script root process 
        /// </summary>
        //private Process procRoot = null;
        /// <summary>
        /// Script root 
        /// </summary>
        private ScriptRoot sroot = null;
        /// <summary>
        /// current target process, it is used to provide the target parameter 
        /// </summary>
        private Process targetProc = null;
        /// <summary>
        /// result of all command list for the input
        /// </summary>
        private List<ParamCmd> cachedCmdList = new List<ParamCmd>();
        /// <summary>
        /// All parameters command mappping list for the input. 
        /// </summary>
        public List<ParamCmd> AllMappings {
            get { return cachedCmdList; }
            set { cachedCmdList = value; }
        }
        /// <summary>
        /// This is an internal used object, it is used to record the new created command src.         
        /// If the mapping source changed by hand, it will NOT null. If the SelectedCmd src was not
        /// changed in src area. it will be null. 
        /// </summary>
        private object newCmdSrc = null;
        /// <summary>
        /// This is an internal used object, it is used to record the new created command target 
        /// Parameter. If the target parameter was changed by hand, it will be valued. else it will be null.  
        /// </summary>
        private Parameter newCmdTgt = null;
        /// <summary>
        /// It is an internal used object, it is used to record the CMD command changed by hand. 
        /// If the ParamCmd command is changed by hand, it will turn true. 
        /// When dialog initial and SelectedCmd changed, it will turn false. 
        /// </summary>
        private bool isCMDChanged = false;

        private ParamCmd selectedCmd = null;
        /// <summary>
        /// Currently selected param command from the paramCmd list view 
        /// </summary>
        public ParamCmd SelectedCmd {
            get { return selectedCmd; }
            set {
                if (value != selectedCmd) {
                    selectedCmd = value;
                    resetCBIndexFlags();
                    this.doParamCmdSelected();
                }
            }
        }
        /// <summary>
        /// It is used to record the currently active mapping src. 
        /// </summary>
        private IMappingSrc activeMappingSrc = null;
        /// <summary>
        /// maintain current combo box selectedIndex, to improve a bit performance. 
        /// The default value is -1
        /// </summary>
        private int cb_mappingtype_Index = -1;
        private int cb_elemtype_Index = -1;
        private int cb_cmdtype_Index = -1;
        private int cb_targetProc_Index = -1;
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int cb_mappingtype_flag = 0;
        private int cb_elemtype_flag = 0;
        private int cb_cmdtype_flag = 0;
        private int cb_targetProc_flag = 0;
        private MSConstantPanel constPanel = null;
        private MSWEAPanel weapanel = null;
        private MSParamPanel parampanel = null;
        private MSExpressionPanel exppanel = null;
        private MSGFPanel gfpanel = null;

        // UI editor for ParamCmd list comment cell 
        private TextBox ceTextBox = null;        
        private Control[] lv_commentCE = null;
        #endregion variables 
        public ParamMappingDlg() {
            InitializeComponent();
            initData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="input">Input must be Operation/Process, or OpCondition(next op is a Process)</param>
        /// <param name="rootProc">script root process</param>
        /// <returns></returns>
        public DialogResult showMappingDlg(IWin32Window handler, object input, ScriptRoot sroot) {
            if (isValidInput(input) && sroot!=null) {
                this.input = input;
                this.sroot = sroot ;
                // cache all paramCmd items, and update the targetProc 
                cacheParamCmdList();
                buildMappingSrcPanels();
                resetUI();
            } else {
                Log.println_map("Mapping, ERROR, showMappingDlg, input is invalid, input = "+input);
                this.input = null;
                return DialogResult.Cancel;
            }
            this.StartPosition = FormStartPosition.CenterParent;
            return this.ShowDialog();            
        }

        private void buildMappingSrcPanels() {
            // build constant panel 
            constPanel = new MSConstantPanel();
            constPanel.MinimumSize = new System.Drawing.Size(200, 240);
            constPanel.Name = "msConstantPanel";
            constPanel.Dock = DockStyle.Fill;
            this.panel_src.Controls.Add(constPanel);
            constPanel.MappingSrcChangedEvt += new EventHandler<CommonEventArgs>(MSPanelMappingSrcChangedEvt);
            constPanel.hidden();
            // build WebElementAttribute panel
            weapanel = new MSWEAPanel();
            weapanel.MinimumSize = new System.Drawing.Size(200, 240);
            weapanel.Name = "msWEAPanel";
            weapanel.Dock = DockStyle.Fill;
            this.panel_src.Controls.Add(weapanel);
            weapanel.MappingSrcChangedEvt += new EventHandler<CommonEventArgs>(MSPanelMappingSrcChangedEvt);
            weapanel.hidden();
            // build Parameter panel
            parampanel = new MSParamPanel();
            parampanel.MinimumSize = new System.Drawing.Size(200, 240);
            parampanel.Name = "msParamPanel";
            parampanel.Dock = DockStyle.Fill;
            this.panel_src.Controls.Add(parampanel);
            parampanel.MappingSrcChangedEvt += new EventHandler<CommonEventArgs>(MSPanelMappingSrcChangedEvt);
            parampanel.hidden();
            // build Expression panel             
            exppanel= new MSExpressionPanel();
            exppanel.MinimumSize = new System.Drawing.Size(200, 240);
            exppanel.Name = "msExpressionPanel";
            exppanel.Dock = DockStyle.Fill;
            this.panel_src.Controls.Add(exppanel);
            exppanel.MappingSrcChangedEvt += new EventHandler<CommonEventArgs>(MSPanelMappingSrcChangedEvt);
            exppanel.hidden();
            // build GlobalFunction panel            
            gfpanel = new MSGFPanel();
            gfpanel.MinimumSize = new System.Drawing.Size(200, 240);
            gfpanel.Name = "msGFPanel";
            gfpanel.Dock = DockStyle.Fill;
            this.panel_src.Controls.Add(gfpanel);
            gfpanel.MappingSrcChangedEvt += new EventHandler<CommonEventArgs>(MSPanelMappingSrcChangedEvt);
            gfpanel.hidden();
            
            // update active mapping panel as constant panel 
            this.activeMappingSrc = constPanel;
        }

        void MSPanelMappingSrcChangedEvt(object sender, CommonEventArgs e) {
            this.newCmdSrc = e.Data;
            this.isCMDChanged = true;
            this.newCmdTgt = getTargetParameter();

            this.updateCmdButtonsArea();            
            showValidationMsg();
        }

        #region common function
        /// <summary>
        /// Cache all ParamCmd items and update the targetProc
        /// </summary>
        private void cacheParamCmdList() {
            if (this.input is Operation) {
                Operation op = this.input as Operation;

                this.cachedCmdList.AddRange(op.Commands.ToArray());
    
                // update target proc 
                this.targetProc = ModelManager.Instance.getOwnerProc(op);
            } else if (this.input is OpCondition) {
                OpCondition opc = this.input as OpCondition;

                this.cachedCmdList.AddRange(opc.Mappings.ToArray());

                // update target proc 
                Operation op = ModelManager.Instance.getOwnerOp(opc);
                if (op is Process) {
                    this.targetProc = op as Process;
                } else {
                    this.targetProc = null;
                }
            }
        }

        private void initData() {
            List<string> types = ModelManager.Instance.getMappingSrcTypeTexts();
            this.cb_mappingType.Items.AddRange(types.ToArray());

            List<string> elemTypes = ModelManager.Instance.getMappingSrcElemTypes();
            this.cb_elemType.Items.AddRange(elemTypes.ToArray());

            List<string> cmds = ModelManager.Instance.getParamCmdTypesText();
            this.cb_cmd.Items.AddRange(cmds.ToArray());

            // text box cell editor 
            this.ceTextBox = new TextBox();
            this.ceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ceTextBox.Location = new System.Drawing.Point(32, 104);
            this.ceTextBox.Multiline = true;
            this.ceTextBox.Name = "ceTextBox";
            this.ceTextBox.Size = new System.Drawing.Size(80, 16);            
            this.ceTextBox.Visible = false;
            this.ceTextBox.TextChanged += new EventHandler(ceTextBox_TextChanged);
            this.panel4.Controls.Add(ceTextBox);

            this.lv_commentCE = new Control[] { null, null, ceTextBox};
            this.lvex_mappings.setCellEditors(lv_commentCE);
        }

        void ceTextBox_TextChanged(object sender, EventArgs e) {
            if (this.SelectedCmd != null) {
                this.SelectedCmd.Description = this.ceTextBox.Text;
            }
        }
        /// <summary>
        /// Input must be Operation/Process or OpCondition with next Op is a process. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool isValidInput(object input) {
            if (input is Operation || input is Process) {
                return true;
            } else if (input is OpCondition) {
                OpCondition opc = input as OpCondition;
                if (opc.Op is Process) {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Reset UI based on the input type 
        /// </summary>
        private void resetUI() {
            resetCBIndexFlags();
            resetInternalData();
            this.selectedCmd = null;
            // reset dialog title text / header text, target parameter radio buttons text and visibility status
            updateTitleAndHeader();
            // only update target process combox, not trigger the parameters treeview update. 
            initTargetProcComboBox();
            // update mapping list area 
            updateMappingListArea();
            // update mapping type area 
            updateSrcArea();            
            // update target proc combobox area
            updateTargetProcComboBox();
            // update target tv and type area 
            updateTargetTVAndTypeArea();            
            // update command buttons 
            updateCmdButtonsArea();
        }

        private void resetCBIndexFlags() {
            this.cb_mappingtype_Index = -1;
            this.cb_elemtype_Index = -1;
            this.cb_cmdtype_Index = -1;
            this.cb_targetProc_Index = -1;
        }
        /// <summary>
        /// Reset the Internal data used to cache the temp new Cmd. 
        /// </summary>
        private void resetInternalData() {
            this.newCmdSrc = null;
            this.newCmdTgt = null;
            this.isCMDChanged = false;
        }        
        /// <summary>
        /// update src area/target area and buttons area, if cmd == null, it means that 
        /// initialize the src area butons area and target areas as default. 
        /// </summary>
        private void doParamCmdSelected() {                        
            // update mapping type area 
            updateSrcArea();
            // update target proc combobox area
            updateTargetProcComboBox();
            // update target tv and type area 
            updateTargetTVAndTypeArea();            
            // update command buttons 
            updateCmdButtonsArea();
            // show validation msg 
            this.showValidationMsg();
        }
               
        /// <summary>
        /// update title / header area
        /// </summary>
        private void updateTitleAndHeader() {
            this.rtb_header.Text = string.Empty;
            if (this.input is Operation) {
                Operation op = this.input as Operation;
                this.Text = UILangUtil.getMsg("dlg.mapping.title.op");
                string t1 = UILangUtil.getMsg("view.op.grp2.text2");
                if (op is Process) {
                    t1 = UILangUtil.getMsg("view.op.grp1.text1");
                }
                string text1 = UILangUtil.getMsg("dlg.mapping.header.text1");
                this.rtb_header.SelectionColor = Color.Black;
                this.rtb_header.AppendText(text1+t1+" ");
                this.rtb_header.SelectionColor = Color.DarkBlue;
                this.rtb_header.AppendText(op.Name);                
            } else if (this.input is OpCondition) {
                OpCondition opc = this.input as OpCondition;
                this.Text = UILangUtil.getMsg("dlg.mapping.title.opc");
                Operation src = opc.Collection.Owner as Operation;
                string t1 = UILangUtil.getMsg("view.op.grp2.text2"); ;
                if (src is Process) {
                    t1 = UILangUtil.getMsg("view.op.grp1.text1");
                }
                string text1 = UILangUtil.getMsg("dlg.mapping.header.text2");
                this.rtb_header.SelectionColor = Color.Black;
                this.rtb_header.AppendText(text1+t1+" ");
                this.rtb_header.SelectionColor = Color.DarkBlue;
                this.rtb_header.AppendText(src.Name+" ");
                string text2 = UILangUtil.getMsg("dlg.mapping.header.text3");
                string text3 = UILangUtil.getMsg("view.op.grp1.text1");
                this.rtb_header.SelectionColor = Color.Black;
                this.rtb_header.AppendText(text2+text3+" ");
                this.rtb_header.SelectionColor = Color.DarkBlue;
                this.rtb_header.AppendText(this.targetProc.Name);                
            }
        }
        /// <summary>
        /// Show validation msg for the newCmd or SelectedCmd if newCmd is null. 
        /// </summary>
        private void showValidationMsg() {
            ValidationMsg msg = null;
            ParamCmd cmd = getParamCmd();
            if (cmd == null) {
                lb_msg.Text = UILangUtil.getMsg("dlg.mapping.valid.msg1");
            } else {
                msg = ModelManager.Instance.getValidMsg(cmd);
                if (msg.Type == MsgType.VALID) {
                    lb_msg.Text = string.Empty;
                } else {
                    lb_msg.Text = msg.Msg;
                }
            }
        }
        /// <summary>
        /// Get newCreated ParamCmd from UI info, if no newCmd, return SelectedCmd, if no ,return null if No new command created.
        /// </summary>
        /// <returns></returns>
        private ParamCmd getParamCmd() {
            ParamCmd cmd = null;
            if (isNewCmdCreated()) {
                // NOTE that : at a time not all the newCmdSrc/newCmdTarget are syncronized with the UI info
                // so just get the value from UI, it is the real value. 
                cmd = ModelFactory.createParamCmd();
                if (this.activeMappingSrc != null) {
                    cmd.Src = this.activeMappingSrc.getMappingSrc();
                }
                cmd.Cmd = ModelManager.Instance.getParamCmdByText(this.cb_elemType.Text);
                cmd.Target = this.newCmdTgt;// getTargetParameter();                
            } else {
                cmd = this.SelectedCmd;
            }
            return cmd;
        }
        /// <summary>
        /// Whether a new command is create by hand. 
        /// </summary>
        /// <returns></returns>
        private bool isNewCmdCreated() {
            bool isOld = this.newCmdSrc == null && this.newCmdTgt == null && this.isCMDChanged == false;
            return !isOld;
        }
        #endregion common function
        #region source area

        /// <summary>
        /// If no new command create && SelectedCmd is null, initialized the src areas as default. 
        /// else it will update the src areas with newCmd(SelectedCmd if newCmd is null)
        /// </summary>
        private void updateSrcArea() {
            if (!isNewCmdCreated() && this.SelectedCmd == null) {
                initializeSrcArea();
            } else {
                doUpdateSrcArea();
            }
        }

        /// <summary>
        /// update src area with new create src or SelectedCmd src. 
        /// </summary>
        private void doUpdateSrcArea() {
            object src = newCmdSrc;
            if (!isNewCmdCreated() && this.SelectedCmd!=null) {
                src = this.SelectedCmd.Src;
            }
            // update mapping src type combo box 
            this.cb_mappingtype_flag = 0;
            this.cb_mappingType.SelectedIndex = getSrcElemMappingTypeIndex(src);
            UIUtils.updateComboBoxText(this.cb_mappingType);
            //this.doCB_mappingType_SelectedIndexChanged();
            // update mapping element type combo box
            ParamType srcType = getMappingSrcElemType();
            int index = ModelManager.Instance.getMappingSrcElemTypeIndex(srcType);
            this.cb_elemtype_flag = 0;
            this.cb_elemType.SelectedIndex = index;
            UIUtils.updateComboBoxText(this.cb_elemType);
            //this.doCB_elemType_SelectedIndexChanged();

            // show the mapping src panel 
            IMappingSrc panel = getSrcMappingPanel(src);
            this.showActiveSrcPanel(panel);
        }
        /// <summary>
        /// initialized the source mapping panel as constant panel, update 
        /// element type combo box.
        /// </summary>
        private void initializeSrcArea() {
            // default selected mapping src type is Constant
            this.cb_mappingtype_flag = 0;
            this.cb_mappingType.SelectedIndex = 0;
            UIUtils.updateComboBoxText(this.cb_mappingType);
            //this.doCB_mappingType_SelectedIndexChanged();
            // default selected mapping src element type is String 
            this.cb_elemtype_flag = 0;
            this.cb_elemType.SelectedIndex = 0;
            UIUtils.updateComboBoxText(this.cb_elemType);
            //this.doCB_elemType_SelectedIndexChanged();

            // show the mapping src panel 
            showActiveSrcPanel(constPanel);
        }
        /// <summary>
        /// hidden the current panel, set the actPanel as new active panel and show it with 
        /// currCmd. 
        /// </summary>
        /// <param name="actPanel"></param>
        private void showActiveSrcPanel(IMappingSrc actPanel) {
            // hidden current panel 
            if (this.activeMappingSrc != null && this.activeMappingSrc!=actPanel) {
                this.activeMappingSrc.hidden();
            }
            // set the new panel as active panel. 
            this.activeMappingSrc = actPanel;
            object src = newCmdSrc;
            if (src == null && SelectedCmd != null && this.SelectedCmd.Src != null) {
                src = this.SelectedCmd.Src;
            }
            if (this.activeMappingSrc == this.weapanel) {
                weapanel.setSRoot(this.sroot);
            } else if (this.activeMappingSrc == this.parampanel) { 
                Operation op = getStubOp();
                parampanel.setStubOp(op, this.sroot);
            } else if (this.activeMappingSrc == this.exppanel) {
                Operation op = getStubOp();
                exppanel.setStubOp(op, this.sroot);
            } else if (this.activeMappingSrc == this.gfpanel) {
                Operation op = getStubOp();
                gfpanel.setStubOp(op, this.sroot);
            }
            this.activeMappingSrc.show(src, getMappingSrcElemType());
        }
        /// <summary>
        /// Get the mapping source stub operation, or null if errors 
        /// </summary>
        /// <returns></returns>
        private Operation getStubOp() {
            if (input is Operation) {
                return input as Operation;
            } else if (input is OpCondition) {
                return ModelManager.Instance.getOwnerOp(input as OpCondition);
            }
            return null;
        }

        private void cb_mappingType_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_mappingtype_flag = 1;
        }
        private void cb_mappingType_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_mappingType_SelectedIndexChanged();
        }

        private void doCB_mappingType_SelectedIndexChanged() {
            if (this.cb_mappingtype_flag == 0) {
                return;
            }            
            // check whether the index value changed 
            if (this.cb_mappingtype_Index == this.cb_mappingType.SelectedIndex) {
                return;
            } else {
                this.cb_mappingtype_Index = this.cb_mappingType.SelectedIndex;
            }

            UIUtils.updateComboBoxText(this.cb_mappingType);

            // handled by hand
            if (this.cb_mappingtype_flag == 1) {
                this.newCmdSrc = null;
                this.newCmdTgt = getTargetParameter();
                this.isCMDChanged = true;
            }
            
            updateMSPanel();
            updateCmdButtonsArea();
            showValidationMsg();

            this.cb_mappingtype_flag = 0;
        }

        private void cb_elemType_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_elemtype_flag = 1;
        }
        private void cb_elemType_SelectedIndexChanged(object sender, EventArgs e) {
            doCB_elemType_SelectedIndexChanged();
        }

        private void doCB_elemType_SelectedIndexChanged() {
            if (this.cb_elemtype_flag == 0) {
                return;
            }
           
            // check whether the selected index changed 
            if (this.cb_elemtype_Index == this.cb_elemType.SelectedIndex) {
                return;
            } else {
                this.cb_elemtype_Index = this.cb_elemType.SelectedIndex;
            }
            // update text 
            UIUtils.updateComboBoxText(this.cb_elemType);

            if (this.cb_elemtype_flag == 1) {
                //this.newCmdSrc = null;
                //this.newCmdTgt = null;
                this.isCMDChanged = true;
            }
            
            // update mapping source panel 
            ParamType srcType = ModelManager.Instance.getParamTypeByText(this.cb_elemType.Text);
            if (this.activeMappingSrc != null) {                
                this.activeMappingSrc.show(null, srcType);
            }
            // update cmd buttons area. 
            updateCmdButtonsArea();
            // update target parameters 
            this.updateTargetTVAndTypeArea();
            // show validation msg 
            this.showValidationMsg();

            this.cb_elemtype_flag = 0;
        }
        private void updateMSPanel() {
            ParamType type = getMappingSrcElemType();
            IMappingSrc panel = null;
            if (this.cb_mappingType.SelectedIndex == 0) {
                panel = constPanel;
            } else if (this.cb_mappingType.SelectedIndex == 1) {
                panel = weapanel;
            } else if (this.cb_mappingType.SelectedIndex == 2) {
                panel = parampanel;
            } else if (this.cb_mappingType.SelectedIndex == 3) {
                panel = exppanel;
            } else if (this.cb_mappingType.SelectedIndex == 4) {
                panel = gfpanel;
            }

            this.showActiveSrcPanel(panel);
        }
        /// <summary>
        /// get the mapping source element type from current cmd, or String as default. 
        /// </summary>
        /// <returns></returns>
        private ParamType getMappingSrcElemType() {
            ParamType type = ParamType.STRING;
            Parameter param = getTargetParameter();
            if (param == null && this.SelectedCmd!=null && this.SelectedCmd.Target!=null) {
                param = this.SelectedCmd.Target;
            }
            if(param!=null){
                if (param.Type == ParamType.NUMBER) {
                    type = ParamType.NUMBER;
                } else if (param.Type == ParamType.SET) {
                    type = param.SetType;
                }
            } else { 
                type = ModelManager.Instance.getParamTypeByText(this.cb_elemType.Text);
            }
            
            return type;
        }
        /// <summary>
        /// get the mapping src element type index, default value is 0 . 
        /// </summary>
        /// <returns></returns>
        private int getSrcElemMappingTypeIndex(object src) {
            int index = 0;
            if (src != null) {
                index = ModelManager.Instance.getMappingSrcTypeIndex(src);
            }
            return index ;
        }
        /// <summary>
        /// Get the proper Mapping source panel or return constPanel as default.
        /// </summary>
        /// <returns></returns>
        private IMappingSrc getSrcMappingPanel(object src) {
            IMappingSrc panel = null;
            if (src == null) {
                panel = this.constPanel;
            }else{                
                int index = ModelManager.Instance.getMappingSrcTypeIndex(src);
                if (index == 0) {
                    panel = this.constPanel;
                } else if (index == 1) {
                    panel = this.weapanel;
                } else if (index == 2) {
                    panel = this.parampanel;
                } else if (index == 3) {
                    panel = this.exppanel;
                } else if (index == 4) {
                    panel = this.gfpanel;
                }
            }
            return panel;
        }
        

        #endregion source area
        #region target area
        /// <summary>
        /// Get the target parameter selected from UI, in the parameter tree, or return null if 
        /// no target parameter selected. 
        /// </summary>
        /// <returns></returns>
        private Parameter getTargetParameter() {
            if (this.tv_tgtParam.SelectedNode != null) {
                object tag = this.tv_tgtParam.SelectedNode.Tag;
                if (tag is Parameter) {
                    return tag as Parameter;
                }
            }
            return null;
        }
        /// <summary>
        /// Initialize the Target Process combo box area, only update target process combox, 
        /// not trigger the parameters treeview update. 
        /// </summary>
        private void initTargetProcComboBox() {
            List<string> items = new List<string>();
            // only show root process 
            if (this.targetProc == null || this.targetProc == this.sroot.ProcRoot) {
                items.Add(this.sroot.ProcRoot.Name);
            } else {
                items.Add(this.sroot.ProcRoot.Name);
                items.Add(this.targetProc.Name);
            }

            if (items.Count > 0) {
                this.cb_targetProc.Items.AddRange(items.ToArray());
                // initialize and make global process was selected. 
                // but not trigger the event handling method here. 
                this.cb_targetProc_flag = 0;
                this.cb_targetProc.SelectedIndex = 0;
                UIUtils.updateComboBoxText(this.cb_targetProc);
                //this.doCB_targetProc_SelectedIndexChanged();
            }
        }
        /// <summary>
        /// Only update the combox selection and not trigger the evt handling method
        /// </summary>
        private void updateTargetProcComboBox() {
            if (this.cb_targetProc.Items.Count > 0) {
                // Global parameter
                int index = 0;
                if (!isNewCmdCreated() && this.SelectedCmd != null && this.cb_targetProc.Items.Count == 2) {
                    // process parameter 
                    index = 1;
                }
                this.cb_targetProc_flag = 0;
                this.cb_targetProc.SelectedIndex = index;
                UIUtils.updateComboBoxText(this.cb_targetProc);
                //this.doCB_targetProc_SelectedIndexChanged();
            }
        }

        /// <summary>
        /// Update the parameter tree view and items info area. update target area based on the command, only show the parameters node 
        /// that matched the element type. 
        /// </summary>
        private void updateTargetTVAndTypeArea() {
            Parameter target = getTargetParameter();
            if (!isNewCmdCreated() && this.SelectedCmd!=null) {
                target = this.SelectedCmd.Target;
            }
            // NO command 
            if (!isNewCmdCreated() && this.SelectedCmd == null) {
                // update parameters tree view. 
                Process tproc = this.sroot.ProcRoot;
                if (this.cb_targetProc.Items.Count == 2 && this.cb_targetProc.SelectedIndex == 1) {
                    tproc = this.targetProc;
                }
                buildParamTV(tproc);
                tv_tgtParam.ExpandAll();
                updateParamTypes(null);
            } else { // there is a command existed, maybe newCmd or SelectedCmd                
                // get proper process 
                Process tproc = getProperTargetProcess(target);
                if(tproc == null) {
                    Log.println_map("ParamMappingDlg - ERROR, updateTargetArea(), getProc errors. param = "+target);
                    return;
                }
                // update param tree view, if input is OpCondition, just show the public parameters 
                buildParamTV(tproc);
                // highlight the command target parameters 
                highlightParameter(target);
                // update type area
                updateParamTypes(target);
            }
        }
        /// <summary>
        /// Return the target process of parameter, candidate maybe ScriptRoot process or target process. 
        /// If not, it will return null. 
        /// If param is null, it will return the current target combobox marked process.
        /// Notes that the target process must be one of them. 
        /// </summary>
        /// <param name="param"></param>
        private Process getProperTargetProcess(Parameter param) {
            Process proc = null;
            if (param == null) {
                if (this.cb_targetProc.SelectedIndex == 0) {
                    proc = this.sroot.ProcRoot;
                } else if (this.cb_targetProc.Items.Count == 2 && this.cb_targetProc.SelectedIndex == 1) {
                    proc = this.targetProc;
                } 
            } else {                
                proc = ModelManager.Instance.getOwnerProcess(this.sroot.ProcRoot, param);
            }
            
            return proc;
        }
        private void updateParamTypes(Parameter param) {           
            this.label3.Visible = false;
            label_p_type.Text = string.Empty;
            label_set_type.Visible = false;
            label5.Visible = false;
            if (param == null) {
                return;
            } else {
                this.label3.Visible = true;
            }
            if (param.Type == ParamType.STRING) {
                label_p_type.Text = ModelManager.Instance.getParamTypeText(ParamType.STRING);// "String";
            } else if (param.Type == ParamType.NUMBER) {
                label_p_type.Text = ModelManager.Instance.getParamTypeText(ParamType.NUMBER);// "Number";
            } else if (param.Type == ParamType.DATETIME) {
                label_p_type.Text = ModelManager.Instance.getParamTypeText(ParamType.DATETIME);// "DateTime";
            } else if (param.Type == ParamType.SET) {
                label_p_type.Text = ModelManager.Instance.getParamTypeText(ParamType.SET);// "Set";
                label5.Visible = true;
                label_set_type.Text = ModelManager.Instance.getParamTypeText(ParamType.STRING);// "String";
                if (param.SetType == ParamType.NUMBER) {
                    label_set_type.Text = ModelManager.Instance.getParamTypeText(ParamType.NUMBER);// "Number";
                }
            }
        }
        
        private void highlightParameter(Parameter param) {
            if (param == null) {
                return;
            }
            foreach (TreeNode tn in this.tv_tgtParam.Nodes) {
                object tag = tn.Tag;
                if (tag == param) {
                    this.tv_tgtParam.SelectedNode = tn;
                    return;
                }
                if (tn.Nodes.Count > 0) {
                    bool isOK = highlightParameter(tn, param);
                    if (isOK) {
                        return;
                    }
                }
            }
        }

        private bool highlightParameter(TreeNode pn, Parameter param) {
            if (pn == null || pn.Tag == null || param == null) {
                return false;
            }
            foreach (TreeNode node in pn.Nodes) {
                object tag = node.Tag;
                if (tag == param) {
                    this.tv_tgtParam.SelectedNode = node;
                    return true;
                }
                if (node.Nodes.Count > 0) {
                    bool isOK = highlightParameter(node, param);
                    if (isOK) {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// build the parameter treeview based on the process, if input is OpCondition, just show
        /// the public parameters. 
        /// It will only show the parameters that matched the SrcElementType
        /// </summary>
        /// <param name="proc"></param>
        private void buildParamTV(Process proc) {
            this.tv_tgtParam.Nodes.Clear();
            if (proc == null) {
                return;
            }            
            this.tv_tgtParam.BeginUpdate();
            // add public parameters 
            TreeNode pubNode = createParamGrpNode(proc.ParamPublic);
            this.tv_tgtParam.Nodes.Add(pubNode);
            // if it is update parameters, show the private parameters 
            if (this.input is Operation) {
                TreeNode priNode = createParamGrpNode(proc.ParamPrivate);
                this.tv_tgtParam.Nodes.Add(priNode);    
            }
            this.tv_tgtParam.EndUpdate();

        }
        /// <summary>
        /// Build parameter group treeNode and children nodes that align with the 
        /// ParamCmd element type. 
        /// </summary>
        /// <param name="tgtGrp"></param>
        /// <returns></returns>
        private TreeNode createParamGrpNode(ParamGroup tgtGrp) {
            TreeNode gnode = new TreeNode();
            gnode.Text = tgtGrp.Name;
            gnode.ImageKey = UIConstants.IMG_PARMGRP;
            gnode.SelectedImageKey = UIConstants.IMG_PARMGRP;
            gnode.ToolTipText = tgtGrp.Description;
            gnode.Tag = tgtGrp;

            foreach (ParamGroup grp in tgtGrp.SubGroups) {
                TreeNode pnode = createParamGrpNode(grp);
                gnode.Nodes.Add(pnode);
            }

            foreach (Parameter param in tgtGrp.Params) {
                if (isValidTargetParam(param)) {
                    TreeNode tnode = createParamNode(param);
                    gnode.Nodes.Add(tnode);
                }
            }

            return gnode;
        }
        /// <summary>
        /// Whether the target parameter is valid to be added into the parameter tree. 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool isValidTargetParam(Parameter param) {
            bool result = false;
            if (param != null) {
                // Check current CMD 
                PARAM_CMD CMD = ModelManager.Instance.getParamCmdByText(this.cb_cmd.Text);                
                // TYPE 
                ParamType type = ModelManager.Instance.getParamTypeByText(this.cb_elemType.Text);
                if (CMD == PARAM_CMD.ASSIGN) {
                    if (param.Type == type) {
                        result = true;
                    }
                } else if (CMD == PARAM_CMD.UPDATE_SET_DEL || CMD == PARAM_CMD.UPDATE_SET_ADD) {
                    if (param.Type == ParamType.SET && param.SetType == type) {
                        result = true;
                    }
                }
            }
            return result;
        }

        private TreeNode createParamNode(Parameter param) {
            TreeNode node = new TreeNode();
            node.Text = param.Name;
            node.ImageKey = "param16.gif";
            node.SelectedImageKey = "param16.gif";
            node.ToolTipText = param.Description;
            node.Tag = param;

            return node;
        }

        private void tv_tgtParam_MouseDown(object sender, MouseEventArgs e) {
            TreeNode node = this.tv_tgtParam.GetNodeAt(e.X, e.Y);
            if (node != null) {
                if (node.Tag is Parameter) {
                    Parameter p = node.Tag as Parameter;
                    this.newCmdTgt = p;
                    this.isCMDChanged = true;
                    this.updateParamTypes(p);
                    updateCmdButtonsArea();
                    showValidationMsg();
                    return;
                } else {
                    this.newCmdTgt = null;
                    // update parameter types 
                    updateParamTypes(null);
                    updateCmdButtonsArea();
                }
            }
            this.lb_msg.Text = string.Empty;
        }

        #endregion target area
        #region command area

        private void cb_cmd_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_cmdtype_flag = 1;
        }

        private void cb_cmd_SelectedIndexChanged(object sender, EventArgs e) {
            this.doCB_cmd_SelectedIndexChanged();
        }

        private void doCB_cmd_SelectedIndexChanged() {
            if (this.cb_cmdtype_flag == 0) {
                return;
            }            
            // check whether the selected index changed 
            if (this.cb_cmdtype_Index == this.cb_cmd.SelectedIndex) {
                return;
            } else {
                this.cb_cmdtype_Index = this.cb_cmd.SelectedIndex;
            }
            // update combo box text 
            UIUtils.updateComboBoxText(this.cb_cmd); 

            // updated by hand
            if (this.cb_cmdtype_flag == 1) {
                this.isCMDChanged = true;
                // clean src and target area.
                if (this.newCmdSrc == null && this.activeMappingSrc != null) {
                    this.newCmdSrc = this.activeMappingSrc.getMappingSrc();
                }
                this.tv_tgtParam.SelectedNode = null;
                updateTargetTVAndTypeArea();
                this.newCmdTgt = null;
            }
                       
            // this condition is that an existed Command selected, the validation
            // will be handled in the paramCmd list selection event.             
            showValidationMsg();
            
            this.cb_cmdtype_flag = 0;
        }
        /// <summary>
        /// update the Command buttons area when source or target areas updated. 
        /// </summary>
        private void updateCmdButtonsArea() {
            ParamCmd cmd = getParamCmd();
            if (cmd == null) {
                this.cb_cmdtype_flag = 2;
                this.cb_cmd.SelectedIndex = ModelManager.Instance.getParamCmdIndex(PARAM_CMD.ASSIGN);
                this.doCB_cmd_SelectedIndexChanged();
                
                this.btn_bind.Enabled = false;                
                this.btn_update.Enabled = false;
                this.btn_unbind.Enabled = false;                
            } else {
                this.cb_cmdtype_flag = 2;
                this.cb_cmd.SelectedIndex = ModelManager.Instance.getParamCmdIndex(cmd.Cmd);
                this.doCB_cmd_SelectedIndexChanged();

                if (isValidCmd(cmd)) {
                    this.btn_bind.Enabled = true;
                    this.btn_update.Enabled = true;
                } else {
                    this.btn_bind.Enabled = false;
                    this.btn_update.Enabled = false;
                }

                this.btn_unbind.Enabled = true;
            }
            // unbind All buttons 
            if (this.cachedCmdList != null && this.cachedCmdList.Count > 0) {
                this.btn_unbindAll.Enabled = true;
            } else {
                this.btn_unbindAll.Enabled = false;
            }
        }
        /// <summary>
        /// Check whether the ParamCmd is a valid commnad. 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool isValidCmd(ParamCmd cmd) {
            if (cmd == null) {
                return false; 
            }
            ValidationMsg msg = ModelManager.Instance.getValidMsg(cmd);
            return msg.Type == MsgType.VALID;
        }
        /// <summary>
        /// Add a new CMD into the command list. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_bind_Click(object sender, EventArgs e) {
            ParamCmd cmd = getParamCmd();
            if (ModelManager.Instance.isValidParamCmd(this.cachedCmdList, cmd)) {
                this.cachedCmdList.Add(cmd);
                // update list view. 
                ListViewItem lvi = createMappingItem(cmd);
                this.lvex_mappings.Items.Add(lvi);
                // Reset the Internal data used to cache the temp new Cmd
                resetInternalData();
                resetCBIndexFlags();
                // update selected cmd, but not need to updat the UI, so 
                // use the selectedCmd itself, not SelectedCmd. 
                this.selectedCmd = lvi.Tag as ParamCmd;
                // set the new item selected.
                UIUtils.selectListViewItem(this.lvex_mappings, lvi);
            } else {
                lb_msg.Text = UILangUtil.getMsg("dlg.mapping.msg.dup");
            }
        }

        private void btn_update_Click(object sender, EventArgs e) {
            if (this.SelectedCmd == null) {
                return;
            }

            ListViewItem lvi = UIUtils.getListViewItemByTag(this.lvex_mappings, this.SelectedCmd);
            int index = -1;
            if (lvi != null) {
                ParamCmd cmd = getParamCmd();
                // update the previous comments if need. 
                if (this.SelectedCmd != null) {
                    cmd.Description = this.SelectedCmd.Description;
                }
                index = this.lvex_mappings.Items.IndexOf(lvi);
                // add newCmd into previous selected cmd position. 
                this.cachedCmdList.Insert(index, cmd);
                ListViewItem newItem = createMappingItem(cmd);
                this.lvex_mappings.Items.Insert(index, newItem);

                // remove current cmd from cachelist             
                this.cachedCmdList.Remove(this.SelectedCmd);
                // remove item from list view 
                this.lvex_mappings.Items.Remove(lvi);

                // set the new item selected 
                UIUtils.selectListViewItem(this.lvex_mappings, newItem);
                // update selected cmd, but not need to updat the UI, so 
                // use the selectedCmd itself, not SelectedCmd. 
                this.selectedCmd = cmd;
            }
            // clean internal flags 
            this.resetInternalData();
            resetCBIndexFlags();
        }

        private void btn_unbind_Click(object sender, EventArgs e) {
            if (this.SelectedCmd != null) {
                ListViewItem lvi = UIUtils.getListViewItemByTag(this.lvex_mappings, this.SelectedCmd);
                if (lvi != null) {
                    this.lvex_mappings.Items.Remove(lvi);
                    this.cachedCmdList.Remove(this.SelectedCmd);

                    this.SelectedCmd = null;
                    this.resetInternalData();
                    resetCBIndexFlags();
                }
            }
        }

        private void btn_unbindAll_Click(object sender, EventArgs e) {
            this.cachedCmdList.Clear();
            this.lvex_mappings.Items.Clear();
            this.SelectedCmd = null;
            this.resetInternalData();
            resetCBIndexFlags();
        } 
        #endregion command area
        #region mapping list area
        private void updateMappingListArea() {
            this.lb_msg.Text = string.Empty;
            this.lvex_mappings.BeginUpdate();
            foreach (ParamCmd cmd in cachedCmdList) {
                ListViewItem lvi = createMappingItem(cmd);
                this.lvex_mappings.Items.Add(lvi);
            }
            this.lvex_mappings.EndUpdate();
        }

        private ListViewItem createMappingItem(ParamCmd cmd) {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = ModelManager.Instance.getParamCmdSrcText(cmd);
            string to = ModelManager.Instance.getParamCmdToText(cmd);
            lvi.SubItems.Add(to);
            string cmt = "";
            if (cmd.Description != null) {
                cmt = cmd.Description;
            }
            lvi.SubItems.Add(cmt);
            // update tag with model command 
            lvi.Tag = cmd;

            return lvi;
        }

        private void lvex_mappings_MouseDown(object sender, MouseEventArgs e) {            
            ListViewItem lvi = this.lvex_mappings.GetItemAt(e.X, e.Y);
            if (lvi != null) {
                resetInternalData();
                this.tv_tgtParam.SelectedNode = null;
                this.SelectedCmd = lvi.Tag as ParamCmd;
            }             
        }
        
        private void lvex_mappings_SubItemClicked(object sender, WebMaster.com.SubItemEventArgs e) {
            Rectangle r = this.lvex_mappings.GetSubItemBounds(e.Item, 2);
            Size size = new Size(-1, -1);
            this.lvex_mappings.setAdjustCellCtrl(2, size);

            this.lvex_mappings.StartEditing(e.SubItem, e.Item);
        }

        private void lvex_mappings_SizeChanged(object sender, EventArgs e) {
            int w_from_min = 200;
            int w_to_min = 100;
            int w_comm_min = 200;
            int w_lv = this.lvex_mappings.ClientSize.Width;
            int w_from = w_from_min;
            int w_to = w_to_min;
            int w_comment = w_comm_min;
            if (w_lv > w_from_min + w_to_min + w_comm_min) {
                int delta = w_lv / 5;
                w_from = delta * 2;
                w_to = delta;
                w_comment = delta * 2;
            }            
            this.colFrom.Width = w_from;
            this.colTo.Width = w_to;
            this.colCmt.Width = w_comment;
        }        
        #endregion mapping list area
        #region UI methods         
        private void ParamMappingDlg_SizeChanged(object sender, EventArgs e) {
            if (this.Size.Height > 600) {
                this.panel4.Size = new Size(this.panel4.Width, (int)(this.Size.Height * 0.25));
            }
        }

        private void panel2_SizeChanged(object sender, EventArgs e) {
            if (this.panel2.Size.Width > 940) {
                this.grp_target.Size = new Size((int)(this.panel2.Size.Width * 0.4), this.grp_target.Height);
            }
        }
        #endregion UI methods 

        private void cb_targetProc_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_targetProc_flag = 1;
        }

        private void cb_targetProc_SelectedIndexChanged(object sender, EventArgs e) {
            this.doCB_targetProc_SelectedIndexChanged();
        }

        private void doCB_targetProc_SelectedIndexChanged() {
            if (this.cb_targetProc_flag == 0) {
                return;
            }            
            
            // check whether the selected index changed 
            if (this.cb_targetProc_Index == this.cb_targetProc.SelectedIndex) {
                return;
            } else {
                this.cb_targetProc_Index = this.cb_targetProc.SelectedIndex;
            }
            // update text 
            UIUtils.updateComboBoxText(this.cb_targetProc);
            // handled by hand
            if (this.cb_targetProc_flag == 1) {                
                //this.newCmdSrc = null;
                //if (this.activeMappingSrc != null) {
                //    this.newCmdSrc = this.activeMappingSrc.getMappingSrc();
                //}
                this.isCMDChanged = true;
                // update newCmdTarget  
                this.newCmdTgt = null;
            }            
            // update mapping source panel 
            ParamType srcType = ModelManager.Instance.getParamTypeByText(this.cb_elemType.Text);                
            // update target parameters 
            this.updateTargetTVAndTypeArea();
            // show validation msg 
            this.showValidationMsg();

            this.cb_targetProc_flag = 0;
        }
               
    }
}
