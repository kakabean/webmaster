using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.lib;
using WebMaster.ide.ui;
using WebMaster.com.script;

namespace WebMaster.ide.editor.mapping
{
    public partial class MappingParamSelectionDlg : Form
    {
        #region variables 
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
        /// Tobe returned parameter type
        /// </summary>
        private ParamType srcType = ParamType.STRING;
        /// <summary>
        /// Dialog input object, that will be shown as the initial value of the dialog. 
        /// It will be updated when each dialog show method. It can be constant string|Number, 
        /// WEA, Parameter, Expression, GlobalFunction. 
        /// </summary>
        private object input = null;
        /// <summary>
        /// output parameter object, tobe returned parameter for the invoker, 
        /// It can be constant string, decimal, WebElementAttribute, Parameter, 
        /// Expression or GlobalFunction. 
        /// </summary>
        private object output = null;
        /// <summary>
        /// output parameter object, tobe returned parameter for the invoker, 
        /// It can be constant string, decimal, WebElementAttribute, Parameter, 
        /// Expression or GlobalFunction.
        /// </summary>
        public object Output {
            get { return output; }
            set { output = value; }
        }
        /// <summary>
        /// this is an internal use to control a small defect 
        /// 0 : Do nothing, just ignore request. 
        /// 1 : changed by hand
        /// 2 : changed by program
        private int cb_mappingtype_flag = 0;
        private int cb_elemtype_flag = 0;
        private MSConstantPanel constPanel = null;
        private MSWEAPanel weapanel = null;
        private MSParamPanel parampanel = null;
        private MSExpressionPanel exppanel = null;
        private MSGFPanel gfpanel = null;
        /// <summary>
        /// It is used to record the currently active mapping src. 
        /// </summary>
        private IMappingSrc activeMappingSrc = null;
        #endregion variables 
        public MappingParamSelectionDlg() {
            InitializeComponent();
            initData();
        }
        /// <summary>
        /// show the mapping parameter selection dialog, it will initialize the dialog with parameters. 
        /// Input is the tobe shown object in the dialog.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="stubOp"></param>
        /// <param name="sroot"></param>
        /// <param name="input">To be shown in the dialog, Allowed type : constant string/decimal, WebElementAttribute, Parameter, 
        /// Expression and GlobalFunction</param>
        /// <param name="paramType"></param>
        /// <returns></returns>
        public DialogResult showSelectionDlg(IWin32Window handler, Operation stubOp, ScriptRoot sroot, object input, ParamType paramType) {
            if (isValidInput(stubOp, sroot, input, paramType)) {
                this.stubOp = stubOp;
                this.sroot = sroot;
                this.input = input;
                this.srcType = paramType;
                buildMappingSrcPanels();
                resetUI(input);
            } else {
                Log.println_map("Mapping, ERROR, showSelectionDlg, input is invalid, stubOp = " + stubOp+", sroot = "+sroot+", paramType = "+paramType);
                this.stubOp = null;
                this.sroot = null;
                return DialogResult.Cancel;
            }

            return this.ShowDialog();
        }

        #region common methods
        private void initData() {
            List<string> types = ModelManager.Instance.getMappingSrcTypeTexts();
            this.cb_mappingType.Items.AddRange(types.ToArray());
            // default type is string 
            this.labelSrcType.Text = ModelManager.Instance.getParamTypeText(ParamType.STRING);
        }
        private bool isValidInput(Operation stubOp, ScriptRoot sroot, object input, ParamType paramType) {
            bool r = false;
            if(stubOp!=null && sroot!=null && 
                ( input == null || input is string || input is decimal || input is WebElementAttribute || input is Parameter 
                || input is Expression || input is GlobalFunction)
                && (paramType == ParamType.STRING || paramType == ParamType.NUMBER)){
                    r = true;
            }
            return r;
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
            exppanel = new MSExpressionPanel();
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
        /// <summary>
        /// reset UI based on the input
        /// <param name="input">Constant string/decimal, WEA, Parameter, Expression, GlobalFunction</param>
        /// </summary>
        private void resetUI(object input) {
            // update the mapping source type combo box selection, update element type Combox selection. 
            updateHeaderArea(input);
        }     
        /// <summary>
        /// update the mapping source type combo box selection, update element type Combox selection. 
        /// </summary>
        private void updateHeaderArea(object input) {
            // update mapping source type combox selectionIndex and text 
            int index = ModelManager.Instance.getMappingSrcTypeIndex(input);
            this.cb_mappingtype_flag = 0;
            this.cb_mappingType.SelectedIndex = index;
            UIUtils.updateComboBoxText(this.cb_mappingType);
            // update element type label text             
            this.labelSrcType.Text = ModelManager.Instance.getParamTypeText(srcType);

            // update the maping source area 
            IMappingSrc panel = getSrcMappingPanel(index);
            this.showActiveSrcPanel(panel);
        }

        void MSPanelMappingSrcChangedEvt(object sender, CommonEventArgs e) {
            this.btn_OK.Enabled = false;
            if (this.activeMappingSrc != null) {
                object obj = this.activeMappingSrc.getMappingSrc();
                if (obj != null) {
                    this.output = obj;
                    this.btn_OK.Enabled = true;
                }
            }
        }
        /// <summary>
        /// Get the proper Mapping source panel or null if errors 
        /// </summary>
        /// <returns></returns>
        private IMappingSrc getSrcMappingPanel(int index) {
            IMappingSrc panel = null;
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

            return panel;
        }
        /// <summary>
        /// hidden the current panel, set the actPanel as new active panel and show it with 
        /// currCmd. 
        /// </summary>
        /// <param name="actPanel"></param>
        private void showActiveSrcPanel(IMappingSrc actPanel) {
            // hidden current panel 
            if (this.activeMappingSrc != null && this.activeMappingSrc != actPanel) {
                this.activeMappingSrc.hidden();
            }
            // set the new panel as active panel. 
            this.activeMappingSrc = actPanel;
            object src = this.input;
            
            if (this.activeMappingSrc == this.weapanel) {
                weapanel.setSRoot(this.sroot);
            } else if (this.activeMappingSrc == this.parampanel) {
                parampanel.setStubOp(this.stubOp, this.sroot);
            } else if (this.activeMappingSrc == this.exppanel) {
                 exppanel.setStubOp(this.stubOp, this.sroot);
            } else if (this.activeMappingSrc == this.gfpanel) {                
                gfpanel.setStubOp(this.stubOp, this.sroot);
            }

            this.activeMappingSrc.show(src, this.srcType);
        }
        #endregion common methods 
        #region ui methods 
        private void cb_mappingType_SelectedIndexChanged(object sender, EventArgs e) {
            this.doCB_mappingType_SelectedIndexChanged();
        }

        private void doCB_mappingType_SelectedIndexChanged() {
            if (cb_mappingtype_flag == 0) {
                return;
            }
            // update text 
            UIUtils.updateComboBoxText(this.cb_mappingType);
            // update src panel area 
            IMappingSrc panel = getSrcMappingPanel(this.cb_mappingType.SelectedIndex);
            this.showActiveSrcPanel(panel);
            this.cb_mappingtype_flag = 0;
        }

        private void cb_mappingType_SelectionChangeCommitted(object sender, EventArgs e) {
            this.cb_mappingtype_flag = 1;
        }
        #endregion ui methods 
    }
}
