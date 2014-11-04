using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebMaster.lib.engine;
using WebMaster.ide.editor.model;
using WebMaster.lib.ui.browser;
using System.Threading;
using WebMaster.lib;
using WebMaster.com.script;

namespace WebMaster.ide.ui
{
    public partial class DebugPanel : UserControl
    {
        /// <summary>
        /// model of script model and view model 
        /// </summary>
        private BigModel bigmodel = null;

        public BigModel Bigmodel {
            get { return bigmodel; }
            set {
                if (bigmodel == null || bigmodel != value) {
                    bigmodel = value;
                    bigModelChanged();
                }
            }
        }
        private WebEngine engine = null;
        
        private WebBrowserEx webBrowserEx1;

        public WebBrowserEx WebBrowser {
            get { return webBrowserEx1; }
            set {
                if (webBrowserEx1 == null || webBrowserEx1 != value) {
                    webBrowserEx1 = value;                    
                }
            }
        }
        /// <summary>
        /// current debug status 
        /// </summary>
        private ENGINE_STATUS dbg_status = ENGINE_STATUS.STOPED;
        /// <summary>
        /// current debug status, it is used to control the debug bar buttons status  
        /// </summary>
        internal ENGINE_STATUS DebugStatus {
            get { return dbg_status; }
            set {
                if (value != dbg_status) {
                    dbg_status = value;
                    updateDebugToolbarByStatus();
                }
            }
        }

        private WindowStatus panelStatus = WindowStatus.RESTORED;
        private int restoredHeight = 100;
        /// <summary>
        /// it is used to restore the original panel height 
        /// </summary>
        public int RestoredHeight {
            get { return restoredHeight; }
            set { restoredHeight = value; }
        }

        public DebugPanel() {
            InitializeComponent();
        }

        internal void init(WebBrowserEx browser,WebEngine dbgEngine) {
            this.webBrowserEx1 = browser;

            engine = dbgEngine;
            engine.StatusChanged += new EventHandler<CommonCodeArgs>(engine_DebugStatusChanged);
            engine.LogDbgMsgUpdated += new EventHandler<CommonEventArgs>(engine_LogDbgMsgUpdated);
        }

        #region event area
        /// <summary>
        /// panel has 3 status, Max, Min or Resotred. The data is the new status value 
        /// it maybe the string value of WindowStatus.MAX, WindowStatus.MIN, WindowStatus.RESTORED
        /// </summary>
        public event EventHandler<CommonEventArgs> PanelStatusChangedEvt;
        protected virtual void OnPanelStatusChangedEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> panelStatusChangedEvt = PanelStatusChangedEvt;
            if (panelStatusChangedEvt != null) {
                panelStatusChangedEvt(this, e);
            }
        }
        public void raisePanelStatusChangedEvt(Object sender, WindowStatus status) {
            CommonEventArgs evt = new CommonEventArgs(sender, status.ToString());
            
            OnPanelStatusChangedEvt(evt);
        }
        #endregion event area
        #region debug and run area
        /// <summary>
        /// This is debug run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_DbgRun_Click(object sender, EventArgs e) {
            if (this.Bigmodel == null || this.Bigmodel.SRoot == null || this.Bigmodel.SRoot.ProcRoot == null || this.Bigmodel.SRoot.ProcRoot.StartOp == null) {
                return;
            }
            doDebugRun(DEBUG_CMD.RUN, this.bigmodel.SRoot.ProcRoot.StartOp);
        }
        /// <summary>
        /// run the script 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="op">from which operation to run</param>
        private void doDebugRun(DEBUG_CMD cmd,Operation op) {
            if (this.Bigmodel == null || this.Bigmodel.SRoot == null || this.Bigmodel.SRoot.ProcRoot == null) {
                return;
            }
            engine.LogLevel = Logger.LOG_APP_MSG;

            if (engine.Mode != WebEngine.MODE_DEBUG) {
                engine.Mode = WebEngine.MODE_DEBUG;
            }
            Log.println_eng("T = DebugUI, set debug cmd = "+cmd);

            this.tsb_Stop.Enabled = true;

            engine.DebugCmd = cmd;
            if (engine.Status == ENGINE_STATUS.BREAK_POINT) {
                this.DebugStatus = ENGINE_STATUS.RUNING;
                // go on run the script
                engine.DebugLock.Set();
            } else if (engine.Status == ENGINE_STATUS.STOPED) {                
                engine.ReqStartTime = DateTime.Now;

                //if (op != null && op == this.Bigmodel.SRoot.ProcRoot.StartOp) {
                //    this.DebugStatus = ENGINE_STATUS.INITIALIZING;
                //    engine.Status = ENGINE_STATUS.INITIALIZING;
                //    // start to debug script 
                //    checkBrowserReady();
                //}
        
                engine.SRoot = this.Bigmodel.SRoot;
                engine.RunWorkerAsync(op);
            }
            this.DebugStatus = ENGINE_STATUS.RUNING;
        }

        private void tsb_Stop_Click(object sender, EventArgs e) {
            this.DebugStatus = ENGINE_STATUS.STOPED;
            this.tsb_Stop.Enabled = false;
            engine.DebugCmd = DEBUG_CMD.STOP;
            engine.DebugLock.Set();
            if (isScriptRuning()) {
                engine.stopScript();
            }
        }

        private void tsb_StepOver_Click(object sender, EventArgs e) {
            if (this.Bigmodel == null || this.Bigmodel.SRoot == null || this.Bigmodel.SRoot.ProcRoot == null || this.Bigmodel.SRoot.ProcRoot.StartOp == null) {
                return;
            }
            doDebugRun(DEBUG_CMD.STEP_OVER, this.Bigmodel.SRoot.ProcRoot.StartOp);
        }

        private void tsb_StepInto_Click(object sender, EventArgs e) {
            if (this.Bigmodel == null || this.Bigmodel.SRoot == null || this.Bigmodel.SRoot.ProcRoot == null || this.Bigmodel.SRoot.ProcRoot.StartOp == null) {
                return;
            }
            doDebugRun(DEBUG_CMD.STEP_INTO, this.Bigmodel.SRoot.ProcRoot.StartOp);
        }
        void engine_LogDbgMsgUpdated(object sender, CommonEventArgs e) {
            if (engine.LogLevel == Logger.LOG_APP_MSG) {
                String msg = e.Data as String;
                //msgs in log tab page
                UIUtils.logMsg(msg,this.rtb);
            }
        }
        void engine_DebugStatusChanged(object sender, CommonCodeArgs e) {
            DebugStatus = (ENGINE_STATUS)(e.Code);
        }
        public bool isScriptRuning() {
            return engine != null && (engine.Status == ENGINE_STATUS.RUNING || engine.Status == ENGINE_STATUS.INITIALIZING);
        }
        /// <summary>
        /// update the status bar with status 
        /// </summary>
        private void updateDebugToolbarByStatus() {
            tsb_DbgRun.Enabled = false;
            tsb_Stop.Enabled = false;
            tsb_StepOver.Enabled = false;
            tsb_StepInto.Enabled = false;

            if (dbg_status == ENGINE_STATUS.STOPED) {                
                tsb_DbgRun.Enabled = true;
                tsb_StepOver.Enabled = true;
                tsb_StepInto.Enabled = true;
            } else if (dbg_status == ENGINE_STATUS.BREAK_POINT) {
                tsb_DbgRun.Enabled = true;
                tsb_Stop.Enabled = true;
                tsb_StepOver.Enabled = true;
                if (engine.isProcessHandling()) {
                    tsb_StepInto.Enabled = true;
                } else {
                    tsb_StepInto.Enabled = false;
                }
            } else if (dbg_status == ENGINE_STATUS.RUNING) {
                tsb_Stop.Enabled = true;
            } else if (dbg_status == ENGINE_STATUS.INITIALIZING) {                
                tsb_Stop.Enabled = true;
            }
        }
        /// <summary>
        /// it is a button to control whether to max/min size the debug panel. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsb_Max_Click(object sender, EventArgs e) {
            if (panelStatus == WindowStatus.RESTORED || panelStatus == WindowStatus.MIN) {

                panelStatus = WindowStatus.MAX;

                tsb_max.Image = global::ide.Properties.Resources.max_maxd16;
                tsb_max.ToolTipText = "Resotre debug panel";

                tsb_min.Image = global::ide.Properties.Resources.min16;
                tsb_min.ToolTipText = "Minimize debug panel";

                raisePanelStatusChangedEvt(sender, WindowStatus.MAX);
            } else if (panelStatus == WindowStatus.MAX) {

                panelStatus = WindowStatus.RESTORED;

                tsb_max.Image = global::ide.Properties.Resources.max16;
                tsb_max.ToolTipText = "Maximize debug panel";

                tsb_min.Image = global::ide.Properties.Resources.min16;
                tsb_min.ToolTipText = "Minimize debug panel";
                
                raisePanelStatusChangedEvt(sender, WindowStatus.RESTORED);
            }
        }

        private void tsb_min_Click(object sender, EventArgs e) {
            if (panelStatus == WindowStatus.MAX || panelStatus == WindowStatus.RESTORED) {
                
                panelStatus = WindowStatus.MIN;

                tsb_max.Image = global::ide.Properties.Resources.max16;
                tsb_max.ToolTipText = "Maximize debug panel";

                tsb_min.Image = global::ide.Properties.Resources.max_maxd16;
                tsb_min.ToolTipText = "Resotre debug panel";

                raisePanelStatusChangedEvt(sender, WindowStatus.MIN);
            } else {
                panelStatus = WindowStatus.RESTORED;

                tsb_min.Image = global::ide.Properties.Resources.min16;
                tsb_min.ToolTipText = "Minimize debug panel";
                
                raisePanelStatusChangedEvt(sender, WindowStatus.RESTORED);
            }
        }           
        #endregion debug and run area
        #region event handling
        private void bigModelChanged() {
            //TODO throw new NotImplementedException();
        }
        #endregion event handling
        #region public methods 
        /// <summary>
        /// Run button clicked, make sure that this method is exactly the same with the tsb_Run_Click() method 
        /// <param name="op">from which op to run</param>
        /// </summary>
        internal void run(Operation op) {
            //this.tsb_Run_Click(tsb_Run, null);
            if (op == null && engine.Status != ENGINE_STATUS.BREAK_POINT) {
                if (this.Bigmodel != null && this.Bigmodel.SRoot != null && this.Bigmodel.SRoot.ProcRoot != null && this.Bigmodel.SRoot.ProcRoot.StartOp != null) {
                    op = this.Bigmodel.SRoot.ProcRoot.StartOp;
                }
            }
            this.doDebugRun(DEBUG_CMD.RUN,op);
        }
        /// <summary>
        /// step over button clicked 
        /// </summary>
        internal void stepover() {
            this.tsb_StepOver_Click(tsb_StepOver, null);
        }
        /// <summary>
        /// step into button clicked 
        /// </summary>
        internal void stepinto() {
            this.tsb_StepInto_Click(tsb_StepInto, null);
        }
        /// <summary>
        /// stop button clicked 
        /// </summary>
        internal void stop() {
            this.tsb_Stop_Click(tsb_Stop, null);
        }
        /// <summary>
        /// Enable the debug toolbar buttons 
        /// </summary>
        internal void enableDebug() {
            this.tsb_DbgRun.Enabled = true;
            this.tsb_StepInto.Enabled = true;
            this.tsb_StepOver.Enabled = true;
            this.tsb_Stop.Enabled = true;
        }
        /// <summary>
        /// disable the debug toolbar buttons 
        /// </summary>
        internal void disableDebug() {
            this.tsb_DbgRun.Enabled = false;
            this.tsb_StepInto.Enabled = false;
            this.tsb_StepOver.Enabled = false;
            this.tsb_Stop.Enabled = false;
        }
        #endregion public methods 
        
        private void tsb_deleteLog_Click(object sender, EventArgs e) {
            this.rtb.ResetText();
        }
    }
}
