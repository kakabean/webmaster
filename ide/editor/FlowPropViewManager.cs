using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.ide.editor.model;
using System.Windows.Forms;
using System.Drawing;
using WebMaster.lib.engine;
using WebMaster.ide.editor.propview;
using WebMaster.lib.ui;

namespace WebMaster.ide.editor
{
    /// <summary>
    /// this class manage the flow editor properties view
    /// </summary>
    public class FlowPropViewManager
    {
        #region fields
        private BigModel bigmodel = null;
        public BigModel Bigmodel {
            get { return bigmodel; }
        }
        /// <summary>
        /// general tab page for the properties view, it's real view depends on
        /// the input element. e.g. BaseElement view(WebElement/WebElementGroup), Operation/ProcessView, 
        /// ParamterView 
        /// </summary>
        private TabPage genPage = null;
        public TabPage GenPage {
            get { return genPage; }
            set { genPage = value; }
        }
        /// <summary>
        /// Condition tab page for the properties view 
        /// </summary>
        private TabPage conPage = null;
        public TabPage ConPage {
            get { return conPage; }
            set { conPage = value; }
        }
        /// <summary>
        /// rule tab page for the properties view 
        /// </summary>
        private TabPage rulePage = null;
        public TabPage RulePage {
            get { return rulePage; }
            set { rulePage = value; }
        }
        private TabPage errorPage = null;
        /// <summary>
        /// error tab page for properties view 
        /// </summary>
        public TabPage ErrorPage {
            get { return errorPage; }
            set { errorPage = value; }
        }
        private TabPage logPage = null;
        /// <summary>
        /// user log design tab page 
        /// </summary>
        public TabPage LogPage {
            get { return logPage; }
            set { logPage = value; }
        }

        /// <summary>
        /// BaseElement properties view, it is used for general page, 
        /// it can be adapterred for WebElement/WebElementGroup/OpCondition
        /// </summary>
        BaseElemPropView bePV = null;
        /// <summary>
        /// Operation and Process properties view, it is used for the general page. 
        /// it can be used for Operation and Process 
        /// </summary>
        OperationPropView opPV = null;
        /// <summary>
        /// Parameter properties view, it is used for the general page. 
        /// Only worked for Parameter object. 
        /// </summary>
        ParamPropView paramPV = null;
        /// <summary>
        /// condition properties view. it is used for the condition page. 
        /// it will be used for the OpCondition
        /// </summary>
        ConPropView conPV = null;
        /// <summary>
        /// rule properties view, it is used for rule page. 
        /// it will be used for ScriptRoot, Operation, and Process
        /// </summary>
        RulePropView rulePV = null;
        /// <summary>
        /// Error log properties view, it is used for error page, 
        /// it will be used floweditor globle, but it will just handle the editor
        /// visible element, include WebElement, Process, Operaiton, and OpCondition
        /// </summary>
        private ErrorLogPropView errPV = null;
        /// <summary>
        /// User Log properties view, it is used for developer to define logs that can be shown to end user
        /// It is only effective for Operation and Process. 
        /// </summary>
        private UserLogPropView logPV = null;
        /// <summary>
        /// hash table main the error properties view info, the key is error element
        /// the value is error msg. 
        /// </summary>
        private HashtableEx errTable = new HashtableEx();
        /// <summary>
        /// hash table main the error properties view info, the key is error element, e.g 
        /// ScriptRoot, WebElement, WebElementGroup, Process, Operation, OpCondition, Parameter
        /// the value is ValidationMsg obj msg.
        /// </summary>
        internal HashtableEx ErrLogTable {
            get { return errTable; }
            //set { errTable = value; }
        }
        /// <summary>
        /// active general properties view, e.g BaseElemPropView or OperationPropView. 
        /// </summary>
        private IPropView activeGenPV = null;
        /// <summary>
        /// active condition properties view
        /// </summary>
        private IPropView activeConPV = null;
        /// <summary>
        /// active rule properties view 
        /// </summary>
        private IPropView activeRulePV = null;

        private FlowEditor flowEditor = null;
        
        #endregion end fields
        public FlowPropViewManager(FlowEditor flowEditor) {
            this.flowEditor = flowEditor;
        }
        /// <summary>
        /// when a new script set, the view will be rebuilded 
        /// </summary>
        /// <param name="sroot"></param>
        public void setBigModel(BigModel bmodel) {
            if (bigmodel != bmodel || bmodel == null) {
                bigmodel = bmodel;
                resetPropertiesView();
                if (bigmodel != null) {
                    opPV.setScriptRoot(this.bigmodel.SRoot);
                }
            }
        }
        #region events
        /// <summary>
        /// the sender is the FlowEditor properties view, the data is the input object of 
        /// the properties views, maybe WebElementGroup, WebElement, Parameter, ParamGroup, Operation, Process
        /// OpCondition, this evt will cause others views to updated necessary info if need when input updated.
        /// </summary>
        public event EventHandler<CommonEventArgs> InputUpdatedEvt;
        protected virtual void OnInputUpdatedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> inputUpdatedEvt = InputUpdatedEvt;
            if (inputUpdatedEvt != null) {
                inputUpdatedEvt(this, e);
            }
        }
        /// <summary>
        /// the sender is the FlowEditor properties view, the data is the input object of 
        /// the properties views, maybe WebElementGroup, WebElement, Parameter, ParamGroup, Operation, Process
        /// OpCondition, this evt will cause others views to updated necessary info if need when input updated.
        /// </summary>
        /// <param name="sender">is FlowEditor properties view</param>
        /// <param name="be">ScriptRoot, WebElementGroup, WebElement, Parameter, Operation, Process
        /// OpCondition</param>
        public void raiseInputUpdatedEvt(Object sender, BaseElement be) {
            if (be != null) {
                CommonEventArgs evt = new CommonEventArgs(sender, be);
                OnInputUpdatedEvt(evt);
            }
        }
        #endregion events
        #region properties view area
        /// <summary>
        /// clean all properties view UI info
        /// 
        /// if the script root existed, update view based on the script root
        /// if script root is null, clean and disable view 
        /// </summary>
        public void resetPropertiesView() {
            bePV.disableView();
            opPV.disableView();
            paramPV.disableView();
            conPV.disableView();
            rulePV.disableView();
            logPV.disableView();
        }
        
        /// <summary>
        /// initial views when application started
        /// </summary>
        /// <param name="propTabCtrl"></param>
        internal void initViews(TabControl propTabCtrl) {
            if (propTabCtrl != null) {
                genPage = propTabCtrl.TabPages[0];
                conPage = propTabCtrl.TabPages[1];
                rulePage = propTabCtrl.TabPages[2];
                errorPage = propTabCtrl.TabPages[3];
                LogPage = propTabCtrl.TabPages[4];
                buildGenPage();
                buildConPage();
                buildRulePage();
                buildErrorPage();
                buildLogPage();
            }
        }
        /// <summary>
        /// build the general properties page
        /// </summary>
        private void buildGenPage() {
            // add BaseElement property view 
            this.bePV = new BaseElemPropView();
            bePV.FlowPVManager = this;
            bePV.ElementValidationEvt += new EventHandler<ValidationArgs>(handleElementValidationEvt);
            bePV.Dock = DockStyle.Fill;
            GenPage.Controls.Add(bePV);

            // add operation property view
            this.opPV = new OperationPropView();
            opPV.FlowPVManager = this;
            opPV.Dock = DockStyle.Fill;
            opPV.ElementValidationEvt += new EventHandler<ValidationArgs>(handleElementValidationEvt);
            GenPage.Controls.Add(opPV);
            this.opPV.hideView();

            // add parameter property view 
            this.paramPV = new ParamPropView();
            paramPV.FlowPVManager = this;
            paramPV.Dock = DockStyle.Fill;
            paramPV.ElementValidationEvt +=new EventHandler<ValidationArgs>(handleElementValidationEvt);
            GenPage.Controls.Add(paramPV);
            this.paramPV.hideView();

            // set a defaule active general view 
            this.activeGenPV = this.bePV;
            this.activeGenPV.showView();
        }
        /// <summary>
        /// build condition properties page 
        /// </summary>
        private void buildConPage() {
            // add Condition properties view for OpCondition
            this.conPV = new ConPropView();
            this.conPV.Dock = DockStyle.Fill;
            this.conPV.FlowPVManager = this;
            this.conPV.ElementValidationEvt +=new EventHandler<ValidationArgs>(handleElementValidationEvt);
            ConPage.Controls.Add(this.conPV);

            // set up default active condition properties view 
            this.activeConPV = this.conPV;
            //this.conPV.tryInitDataOnlyOnce();
            this.activeConPV.showView();
        }
        /// <summary>
        /// build rule properties page 
        /// </summary>
        private void buildRulePage() {
            // build a new rule properties view 
            this.rulePV = new RulePropView();
            this.rulePV.Dock = DockStyle.Fill;
            this.rulePV.FlowPVManager = this;
            this.rulePV.ElementValidationEvt +=new EventHandler<ValidationArgs>(handleElementValidationEvt);
            RulePage.Controls.Add(this.rulePV);

            // set up default active rule properties view
            this.activeRulePV = this.rulePV;
            this.activeRulePV.showView();
        }
        private void buildErrorPage() {
            this.errPV = new ErrorLogPropView();
            this.errPV.Dock = DockStyle.Fill;
            this.errPV.ValidationMsgClickedEvt += new EventHandler<CommonEventArgs>(errPV_ValidationMsgClickedEvt);
            ErrorPage.Controls.Add(this.errPV);
            this.errPV.setInput(ErrLogTable);
            this.errPV.showView();
        }
        private void buildLogPage() {
            this.logPV = new UserLogPropView();
            this.logPV.Dock = DockStyle.Fill;
            LogPage.Controls.Add(this.logPV);
            this.logPV.FlowPVManager = this;

            this.logPV.showView();
        }
        /// <summary>
        /// 1. update current active properties view's value into existed input 
        /// 2. use the new input value to initialize necessary properties views 
        /// </summary>
        /// <param name="input"></param>
        public void inputChanged(BaseElement input) {
            updateExistedPropData();
            updatePropViews(input);
        }
        /// <summary>
        /// active proper properties view based on the input 
        /// </summary>
        /// <param name="input"></param>
        internal void updatePropViews(BaseElement input) {
            updateGeneralPage(input);
            updateConditionPage(input);
            updateRulePage(input);
            updateLogPage(input);
            //TODO just ignore input valid in this version 
            //updateErrorLogPage(input);
            // show proper view based on the input 
            activeProperView(input);
        }
        /// <summary>
        /// Active the proper view based on the input 
        /// </summary>
        /// <param name="input"></param>
        private void activeProperView(BaseElement input) {
            TabControl tabCtrl = this.GenPage.Parent as TabControl;
            if (input is WebElement || input is Parameter || input is WebElementGroup || input is ParamGroup) {
                //if (tabCtrl.SelectedTab != this.ErrorPage) {
                    tabCtrl.SelectedTab = GenPage;
                //}
            } else if (input is OpCondition) {
                if (!(tabCtrl.SelectedTab == GenPage || tabCtrl.SelectedTab == ConPage)) {
                    tabCtrl.SelectedTab = ConPage;
                }
            } else if (input is Process || input is Operation) {
                if (tabCtrl.SelectedTab == ConPage) {
                    tabCtrl.SelectedTab = GenPage;
                }
            }
        }
        /// <summary>
        /// update error log view info based on input, it is used whenever the 
        /// Properties view input changed, it will validate input and check, 
        /// if input error info is in error view, just update if need, else 
        /// if input error info was not in error view, just add
        /// </summary>
        /// <param name="input"></param>
        private void updateErrorLogPage(BaseElement input) {
            if (input != null) { 
                //1. validate input 
                ValidationMsg msg = ModelManager.Instance.getValidMsg(input);
                this.errPV.updateValidationMsg(input, msg);
            }
        }
        /// <summary>
        /// update rule page based on the input value, 
        /// rule can be applied for ScriptRoot, Operation and Process
        /// </summary>
        /// <param name="input"></param>
        private void updateRulePage(BaseElement input) {
            if (input is Operation || input is Process) {
                if (this.rulePV != this.activeRulePV) {
                    this.activeRulePV.hideView();
                    this.activeRulePV = this.rulePV;
                    this.activeRulePV.showView();
                }
            }
            this.activeRulePV.setInput(input);
        }
        /// <summary>
        /// update log page based on the input value, 
        /// input can be Operation or Process
        /// </summary>
        /// <param name="input"></param>
        private void updateLogPage(BaseElement input) {
            this.logPV.setInput(input);
        }
        /// <summary>
        /// update condition page based on the input value 
        /// </summary>
        /// <param name="input"></param>
        private void updateConditionPage(BaseElement input) {
            if (input is OpCondition) {
                // update condition page 
                if (this.conPV != this.activeConPV) {
                    this.activeConPV.hideView();
                    //this.conPV.tryInitDataOnlyOnce();
                    this.activeConPV = this.conPV;                  
                    this.activeConPV.showView();
                }
            }
            if (activeConPV == this.conPV) {
                this.conPV.tryInitDataOnlyOnce();
            }
            this.activeConPV.setInput(input);
        }
        /// <summary>
        /// update general page based on the input
        /// </summary>
        /// <param name="input"></param>
        private void updateGeneralPage(BaseElement input) {
            if (input is OpCondition || input is WebElement || input is WebElementGroup || input is ParamGroup) {
                if (this.bePV != this.activeGenPV ) {
                    this.activeGenPV.hideView();
                    this.activeGenPV = this.bePV;                    
                    this.activeGenPV.showView(); 
                }
            } else if (input is Operation || input is Process) {
                // handle general page - OperationPropView
                if (this.opPV!=this.activeGenPV) {
                    this.activeGenPV.hideView();
                    this.opPV.tryInitDataOnlyOnce();
                    this.activeGenPV = this.opPV;                    
                    this.activeGenPV.showView();
                }
            } else if (input is Parameter) {
                if (this.paramPV != this.activeGenPV) {
                    this.activeGenPV.hideView();
                    this.paramPV.tryInitDataOnlyOnce();
                    this.activeGenPV = this.paramPV;
                    this.activeGenPV.showView();
                }
            }
            
            this.activeGenPV.setInput(input);            
        }
        /// <summary>
        /// update all active propties views info into its input. current is 
        /// general page, condition page, and rule page. 
        /// </summary>
        internal void updateExistedPropData() {
            // this method is useless, I think it should be removed later, 
            // foreach prop view, the data update will take effect imediately. 
            //this.activeGenPV.updatedInput();
            //this.activeConPV.updatedInput();
            //this.activeRulePV.updatedInput();
        }
        #endregion properties area
        #region handle error msg properties view
        /// <summary>
        /// It is used to handle validatoin message, the only supported data is 
        /// WebElement, WebElementGroup, Process, Operation, OpCondition, others info e.g parameter, 
        /// Condition are bind on parent element. 
        /// include
        /// 1. update element icon/lable with/without error tag in ui
        /// 2. update error validation info into error log view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void handleElementValidationEvt(object sender, ValidationArgs e) {
            // TODO this method will be updated for next version
            //object data = e.Data;
            //string msg = e.Msg;
            //if (data == null || msg == null) {
            //    return;
            //}
            //// 1. remove error flag icon from WebElement tree view or Diagram 
            //if (e.Type == MsgType.VALID) {
            //    flowEditor.markValidationStatus(data,e.Type);
            //} else { // 1. update element icon with error flag on WebElement tree or Diagram
            //    if (data is WebElement) { 
            //        //TODO update webElement with error icon in WebElement tree
            //    } else if (data is Process) { 
            //        //TODO update Process node with error icon marked in diagram.
            //    } else if (data is Operation) {
            //        //TODO update Operation node with error icon marked in diagram.
            //    } else if (data is OpCondition) {
            //        //TODO update Operation node with error icon marked in diagram.
            //    }
            //}
            //// 2. update error log 
            //updateErrorLogView(data,e.Type,msg);
        }

        /// <summary>
        /// update ErrorLog view info, 
        /// 1. if the data error msg is contained in view, just udpate the msg text info
        /// 2. if there is no error msg about data, just add a new one, 
        /// 3. if there is a valid element, and contained in the error log view, just remove the record. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="msg">error msg info </param>
        private void updateErrorLogView(object data, MsgType type, string msg) {
            //TODO this method will be updated next version
            if (data == null || msg == null) {
                return;
            }

        }
        /// <summary>
        /// update the Error Log properties view
        /// </summary>
        public void updateErrorLogView() {
            this.errPV.setInput(this.ErrLogTable);
        }        
        /// <summary>
        /// clean error hashtable and clean error log properties view 
        /// </summary>
        internal void cleanLogView() {            
            this.errPV.cleanView();
        }
        /// <summary>
        /// select and focus the relative data 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void errPV_ValidationMsgClickedEvt(object sender, CommonEventArgs e) {
            if (e.Data is WebElement) {
                WebElement we = e.Data as WebElement;
                this.flowEditor.markSelectedTreeNode(we);
            } else if (e.Data is WebElementGroup) {
                WebElementGroup weg = e.Data as WebElementGroup;
                this.flowEditor.markSelectedTreeNode(weg);
            } else if (e.Data is Process || e.Data is Operation || e.Data is OpCondition || e.Data is OperationRule || e.Data is ParamCmd) {              
                this.flowEditor.markSelectedDiagramElement(e.Data as BaseElement);
            } else if (e.Data is Parameter) {
                Parameter param = e.Data as Parameter;
                Process proc = ModelManager.Instance.getOwnerProcess(this.Bigmodel.SRoot.ProcRoot, param);
                if (proc != null) {
                    this.flowEditor.markSelectedDiagramElement(proc);
                    this.flowEditor.markSelectedTreeNode(param);
                }
            }
        }
        #endregion handle error msg properties view

    }
}
