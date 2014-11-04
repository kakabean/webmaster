using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using WebMaster.lib.ui.browser;
using System.Threading;
using System.Windows.Forms;
using mshtml;
using System.Drawing;
using WebMaster.lib.rule;
using System.Diagnostics;

namespace WebMaster.lib.engine
{
    /// <summary>
    /// this engine will run the script to do real operations. 
    /// one WebEgine object bridge one worker and browser to run the script 
    /// </summary>
    public class WebEngine
    {
        #region constants
        public static readonly int EXE_CODE_OK = 0;
        public static readonly int EXE_CODE_URL_VIOLATION = 1;

        /// <summary>
        /// Engine thread is run in debug mode
        /// </summary>
        public static readonly int MODE_DEBUG = 0x10;
        /// <summary>
        /// Engine thread is run in release mode
        /// </summary>
        public static readonly int MODE_RELEASE = 0x11;
        /// <summary>
        /// tell the UI thread to execute the operaiton
        /// </summary>
        private static readonly int DO_OPERATION = 0x20;
        /// <summary>
        /// tell the UI thread to find condition input WebElement and update it as refershed realtime values
        /// </summary>
        private static readonly int CON_INPUT_WEBELEMENT = 0x21;
        /// <summary>
        /// tell the UI thread to find condition input WebElementAttribute and update it as a refreshed realtime value
        /// </summary>
        private static readonly int CON_INPUT_ATTRIBUTE = 0x22;
        /// <summary>
        /// tell UI thread to check wether the WebElement existed, if existed update the WebElement with real values 
        /// </summary>
        private static readonly int WE_EXISTED_TEST = 0x23;
        ///// <summary>
        ///// TODO .....
        ///// </summary>
        //public static readonly int REQ_TIMEOUT = 0x50;
        /// <summary>
        /// UI thread and Engine thread default sync time out value, default value is 5 minutes,
        /// value is in milliseconds 
        /// It means that Engine thread will wait TIMEOUT_THREAD_LOCK time for the UI thread.
        /// </summary>
        public static readonly int TIMEOUT_THREAD_LOCK = 300 * 1000;
        /// <summary>
        /// default request timeout value, 1 minute. 
        /// </summary>
        public static readonly TimeSpan TIMEOUT_WEB_REQUEST = new TimeSpan(0, 0, 1, 0, 0);
        public static readonly int SYNC_ENGINE_PRE_LOCK = 0;
        public static readonly int SYNC_ENGINE_LOCK = 1;
        public static readonly int SYNC_ENGINE_TIMEOUT = -1;

        public static readonly int RELEASE_CMD_STOP = 0 ;
        public static readonly int RELEASE_CMD_START = 1 ;
        #endregion constants
        #region variables
        /// <summary>
        /// handle the real script operation in back ground thread
        /// </summary>
        private BackgroundWorker worker = null;
        /// <summary>
        /// This is the current OpWrapper that engine executing. 
        /// </summary>
        private OpWrapper currentOpw = new OpWrapper();

        internal OpWrapper CurrentOpw {
            get { return currentOpw; }
            //set { currentOpw = value; }
        }
        /// <summary>
        /// script working webBrowser 
        /// </summary>
        private WebBrowserEx browser = null;
        private ScriptRoot sroot = null;
        /// <summary>
        /// script that is executing for by engine, make sure before start to run a script, please set the script root first
        /// </summary>
        public ScriptRoot SRoot {
            get { return sroot; }
            set { sroot = value; }
        }
        /// <summary>
        /// stack to maintain the process invoke stack, the top process is currently working 
        /// process, the next process(if have) is the top one's invoker process. 
        /// when a process finished, it will remove the top proc and route the logic to proper 
        /// operation
        /// </summary>
        private Stack<Process> procStack = new Stack<Process>();
        /// <summary>
        /// It is only take effect "if the OpCondition target op is process and there are parameters 
        /// mapping defined", and when a process is first initiated, it will be used to update the mapping 
        /// process public parameter. 
        /// 1. When a OpCondition's next target is Process, it will check whehter there are some 
        ///    parameter mappings to the target process public parameters, if have, the src process
        ///    parameter value will first mapped to helper's public parameter with 
        ///    "name=target parameter name, value=new value".
        /// 2. In the next step, when the process first initial, it will be used to update the existed public
        ///    parameters. and then clean helper. 
        /// </summary>
        private Process paramMappingProc = null;
        /// <summary>
        /// a lock to control the UI thread and back engine thread synchronized, it used to control whether lock 
        /// the engine thread or not 
        /// </summary>
        private AutoResetEvent threadLock = new AutoResetEvent(false);
        /// <summary>
        /// a lock to control the UI debug status with back engine thread
        /// only effective when debug mode
        /// </summary>
        private AutoResetEvent debugLock = new AutoResetEvent(false);
        /// <summary>
        /// this variable is used to check if the engine thread is wakeup by timeout event
        /// </summary>
        private volatile bool isLockerTimeout = false;
        /// <summary>
        /// a lock to control the UI debug status with back engine thread
        /// only effective when debug mode
        /// </summary>
        public AutoResetEvent DebugLock {
            get { return debugLock; }
            //set { debugLock = value; }
        }        
        /// <summary>
        /// Engine execute mode, release or debug 
        /// </summary>
        private int mode = MODE_RELEASE;
        /// <summary>
        /// Engine execute mode, release or debug 
        /// </summary>
        public int Mode {
            get { return mode; }
            set { mode = value; }
        }
        private ENGINE_STATUS status = ENGINE_STATUS.STOPED;
        /// <summary>
        /// This is used to help to control the engine execute status. 
        /// 0: means ok. 
        /// 1: means url security locked. 
        /// </summary>
        private int exe_code = EXE_CODE_OK; 
        /// <summary>
        /// engine thread status, run, stop, initializing, invalid, can step over/into
        /// </summary>
        public ENGINE_STATUS Status {
            get { return status; }
            set { status = value; }
        }
        private int RELEASE_CMD = RELEASE_CMD_STOP;

        /// <summary>
        /// this flag is used to help to make sure the engine thread and UI thread worked synchronized. because if no such flag, 
        /// when call the code like this 
        ///        worker.ReportProgress(LOG_APP_MSG);
        ///        this.lockEngine();
        /// The UI thread code will executed directly when the first cause called, but the expected is that the engine locked first
        /// and then the UI thread run, some times it is worked ok, sometimes error, not stable, so just add a flag to help to correct
        /// 
        /// How to use: 
        /// 1. before call worker.ReportProgress() method, set the flag as SYNC_ENGINE_PRE_LOCK,
        /// 2. in the UI thread, first it will check the flag, if it is SYNC_ENGINE_PRE_LOCK, it will sleep 200 ms and check again, until
        ///    the flag is SYNC_ENGINE_LOCK(This means that the engine thread was in wait status for UI/Outer thread wakeup)
        ///    or timeout(set the flag SYNC_ENGINE_TIMEOUT), default time out is TIMEOUT_THREAD_LOCK.  
        /// 3. in the lockEngine() method, set flag as SYNC_ENGINE_LOCK
        /// 
        /// Comments standards use to lock the thread is that 
        /// <code>
        ///    this.syncFlag = SYNC_ENGINE_PRE_LOCK;  // this maybe SYNC_ENGINE_TIMEOUT for difference purpose. check code for details 
        ///    worker.ReportProgress(DO_OPERATION, opw);
        ///    this.lockEngine();
        /// </code>
        /// In the UI thread, it is working something like this: 
        /// <example>
        /// <code>
        ///    int time = 0;
        ///    while (time "less" TIMEOUT_THREAD_LOCK && this.syncFlag == SYNC_ENGINE_PRE_LOCK) {
        ///        Thread.Sleep(200);
        ///        time += 200;
        ///    }
        ///    if (syncFlag == SYNC_ENGINE_LOCK) {
        ///        // when UI operation finished, just release the engine thread lock
        ///        this.unlockEngine();
        ///    } else {
        ///        syncFlag = SYNC_ENGINE_TIMEOUT;
        ///    }
        ///    </code>
        ///    </example>
        /// </summary>
        private volatile int syncFlag = SYNC_ENGINE_TIMEOUT;
        /// <summary>
        /// debug command from the UI thread, engine will follow the command to execute
        /// script
        /// </summary>
        private DEBUG_CMD debugCmd = DEBUG_CMD.NONE;
        /// <summary>
        /// debug command from the UI thread, engine will follow the command to execute
        /// script.
        /// UI thread should set debug command before invoke the engine with debug mode
        /// </summary>
        public DEBUG_CMD DebugCmd {
            get { return debugCmd; }
            set { debugCmd = value; }
        }
        /// <summary>
        /// Help to control the step over command when in debug mode
        /// </summary>
        private Operation stepOverOp = null;
        /// <summary>
        /// it is used to control the step over break points
        /// </summary>
        private bool isStepOverBreak = false;
        /// <summary>
        /// this list is valid if it is in debug mode, it will save the break point Operation
        /// or process
        /// </summary>
        private List<Operation> breakpoints = new List<Operation>();
        // temp code for check thread sync 
        private int tempcode = -1;
        /// <summary>
        /// Used to log user msg or application msg
        /// </summary>
        private Logger logger = null;

        internal Logger Logger {
            get { return logger; }
            //set { logger = value; }
        }
        ///// <summary>
        ///// msg to record the debug logs for each operation and OpConditions, Rules and what else.
        ///// when each msg showed in the UI thread, msg should be cleaned for next use
        ///// </summary>
        //private StringBuilder dbgMsg = new StringBuilder();

        ///// <summary>
        ///// msg to record the release logs for each operation, 
        ///// when each msg showed in the UI thread, msg should be cleaned for next use
        ///// </summary>
        //private StringBuilder releaseMsg = new StringBuilder();        
        
        /// <summary>
        /// log level, default is just log the script developer's log info.
        /// </summary>
        public int LogLevel {
            get { return logger.LogLevel; }
            set { logger.LogLevel = value; }
        }
        #endregion variables

        #region variables time relative
        private TimeSpan reqTimeOut = TIMEOUT_WEB_REQUEST;
        /// <summary>
        /// how long the request will refresh if the page is not download compelted. 
        /// default value is 60 seconds, value is in milliseconds 
        /// </summary>
        public TimeSpan ReqTimeOut {
            get { return reqTimeOut; }
            set { reqTimeOut = value; }
        }
        /// <summary>
        /// used in debug or run mode
        /// this time is used to mark when the current request is started, it is used to check the 
        /// request timeout event. click a link or button or navigate method will reset the value. 
        /// </summary>
        private DateTime reqStartTime = new DateTime();

        public DateTime ReqStartTime {
            get { return reqStartTime; }
            set { reqStartTime = value; }
        }

        private bool timeSensitive = false;
        /// <summary>
        /// whether the script is time sensitive, default is false ;
        /// if true, when a frame document is load completed, the op will check whether the relative WebElement 
        /// is available, if yes, the op will executed, if no, it will wait the next document completed event to check
        /// until the relative WebElement is avaible.
        /// 
        /// if false, the op will wait until all the document is loaded completed. and check necessary WebElement used. 
        /// </summary>
        public bool TimeSensitive {
            get { return timeSensitive; }
            set { timeSensitive = value; }
        }
        private String requestURL = null;
        /// <summary>
        /// current requested url if it is a click links or buttons operation. if others non-request url operation, 
        /// RequestURL will be set null 
        /// </summary>
        public String RequestURL {
            get { return requestURL; }
            set { requestURL = value; }
        }
        ///// <summary>
        ///// This one is used to maintain the browser ReadyState and anytime. 
        ///// ??? //TODO this one should be removed. I think it is useless, I just remember that this is used 
        ///// to avoid some UI thread blocking problem, it seems that it is useless ..
        ///// //// >>>>>>> 
        ///// </summary>
        //private WebBrowserReadyState bwState = WebBrowserReadyState.Uninitialized;
        
        private volatile bool isDocumentCompleted = false;
        /// <summary>
        /// True : if the engine browser document downloaded completed. False: document downloading is not completed.
        /// It is used to show whether the browser document downloaded completed. so that under this scenario if an WebElement was 
        /// not found, it will be treated can not reached WE. so that it can save some loop-check time and improve the engine performance. 
        /// 
        /// It will be updated anytime browser status changed. 
        /// </summary>
        public bool IsDocumentCompleted {
            get { return isDocumentCompleted; }
            //set { isDocumentCompleted = value; }
        }
        /// This is used to maintain an internal clock to sync time with the server time. 
        private Stopwatch sw = new Stopwatch();
        /// <summary>
        /// The date this script started to run, in case that the Operation doesn't set the date info for the exeTime.
        /// this will be initialized whenever the server time updated. 
        /// </summary>
        private DateTime startDate = DateTime.Now;
        private DateTime serverTime = DateTime.Now;
        /// <summary>
        /// Current server time, when sever time is set, the sw will be reset.
        /// </summary>
        public DateTime ServerTime {
            get { return serverTime.AddMilliseconds(sw.ElapsedMilliseconds); }
            set {
                sw.Reset();
                serverTime = value; }
        }

        #endregion variables time relative
        /// <summary>
        /// make sure the parameters are not null 
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="browser"></param>
        public WebEngine(BackgroundWorker worker, WebBrowserEx browser, int mode) {
            this.worker = worker;
            this.browser = browser;
            this.browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            this.browser.Navigating += new WebBrowserNavigatingEventHandler(browser_Navigating);
            //this.browser.StatusTextChanged += new EventHandler(browser_StatusTextChanged);
            this.mode = mode;
            logger = new Logger(this, this.mode);
            Log.println_eng("T = ENGINE, create a new WebEngine instance ...");

            // initial worker             
            this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            this.worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            // initData
            initData();
        }

        private void initData() { }
        /// <summary>
        /// Update the bwState if browser status changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void browser_StatusTextChanged(object sender, EventArgs e) {
            //if (browser.IsBusy || browser.ReadyState != WebBrowserReadyState.Complete) {
            //    this.isDocumentCompleted = false;
            //}
        }
        /// <summary>
        /// In this method it will used to control the url security check. 
        /// This is the core place to check, another pre-check place is before the UI operation run. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            // make sure that at that point to navigating, isDocumentCompleted changed
            this.isDocumentCompleted = false;
            
            if (this.SRoot != null && this.SRoot.UrlsLocked) {
                string url = e.Url != null ? e.Url.ToString() : string.Empty;
                if (url != string.Empty && url!=Constants.URL_BLANK) {
                    bool trusted = WebUtil.isTrustedURL(url, this.SRoot.TrustedUrls);
                    if (!trusted) {
                        Log.println_eng("T = ENGINE, ERROR, Non-trusted URL. url = " + url);
                        e.Cancel = true;
                        // raise the url vialation event 
                        string txt = LangUtil.getMsg("log.req.url.violation.text1");
                        string tmsg = txt + url;
                        string str = "\n" + tmsg + "\n";
                        logger.buildLogMsg(str);
                        this.raiseReqURLViolationEvt(this, tmsg);
                        // stop the script runing, 
                        // ******* Notes:  **********************
                        // here not use the currentOpw to control 
                        // the logic because some times it will cause opw in a inconsistant status
                        // so just use another flag to control un-expected behaviour. 
                        this.exe_code = EXE_CODE_URL_VIOLATION;
                    }
                }
            }
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //bwState = browser.ReadyState;
            if (browser.IsBusy == false && browser.ReadyState == WebBrowserReadyState.Complete) {
                this.isDocumentCompleted = true;
            }
            // if engine status is ENGINE_STATUS.INITIALIZING, just wait until the browser finished. 
            // Log.println_eng("T = ENGINE, document completed event. bwState = "+bwState+", Engine.Status="+this.Status+", isBusy = "+browser.IsBusy);
            if (this.Status == ENGINE_STATUS.INITIALIZING && !browser.IsBusy && browser.ReadyState >= WebBrowserReadyState.Interactive) {
                this.Status = ENGINE_STATUS.RUNING;
            }
        }
        #region events area
        /// <summary>
        /// sender is WebEngine, event data is the status code, outer debug panel can book this event 
        /// to update the debug panel status or other things. 
        /// Status code will be 
        /// code == (int)ENGINE_STATUS.BREAK_POINT
        ///        || code == (int)ENGINE_STATUS.RUNING
        ///        || code == (int)ENGINE_STATUS.STOPED
        ///        || code == (int)ENGINE_STATUS.INITIALIZING;
        /// </summary>
        public event EventHandler<CommonCodeArgs> StatusChanged;
        protected virtual void OnStatusChanged(CommonCodeArgs e) {
            EventHandler<CommonCodeArgs> statusChanged = StatusChanged;
            if (statusChanged != null) {
                statusChanged(this, e);
            }
        }
        public void raiseStatusChanged(Object sender, int statusCode) {
            Log.println_eng("T = UI,     *** raise debug status changed event, code = " + (ENGINE_STATUS)statusCode);
            CommonCodeArgs evt = new CommonCodeArgs(sender, statusCode);
            OnStatusChanged(evt);
        }
        /// <summary>
        /// raise the event for outer to catch the runtime engine's debug log message info. 
        /// data is msg info.
        /// </summary>
        public event EventHandler<CommonEventArgs> LogDbgMsgUpdated;
        protected virtual void OnLogDbgMsgUpdated(CommonEventArgs e) {
            EventHandler<CommonEventArgs> logDbgMsgUpdated = LogDbgMsgUpdated;
            if (logDbgMsgUpdated != null) {
                logDbgMsgUpdated(this, e);
            }
        }
        public void raiseLogDbgMsgUpdated(Object sender, String msg) {
            Log.println_eng("T = UI,     *** raise log dbg msg updated event ");
            CommonEventArgs evt = new CommonEventArgs(sender, msg);
            OnLogDbgMsgUpdated(evt);
        }
        /// <summary>
        /// raise the event for outer to catch the runtime engine's release log message info. 
        /// data is msg info.
        /// </summary>
        public event EventHandler<CommonEventArgs> LogReleaseMsgUpdated;
        protected virtual void OnLogReleaseMsgUpdated(CommonEventArgs e) {
            EventHandler<CommonEventArgs> logReleaseMsgUpdated = LogReleaseMsgUpdated;
            if (logReleaseMsgUpdated != null) {
                logReleaseMsgUpdated(this, e);
            }
        }
        public void raiseLogReleaseMsgUpdated(Object sender, String msg) {
            Log.println_eng("T = UI,     *** raise log Release msg updated event ");
            CommonEventArgs evt = new CommonEventArgs(sender, msg);
            OnLogReleaseMsgUpdated(evt);
        }
        /// <summary>
        /// raise the event for outer to catch the runtime engine's request violation event. 
        /// data is vialation msg info.
        /// </summary>
        public event EventHandler<CommonEventArgs> ReqURLViolationEvt;
        protected virtual void OnReqURLViolationEvt(CommonEventArgs e) {
            EventHandler<CommonEventArgs> reqURLViolationEvt = ReqURLViolationEvt;
            if (reqURLViolationEvt != null) {
                reqURLViolationEvt(this, e);
            }
        }
        /// <summary>
        /// raise the event for outer to catch the runtime engine's request violation event. 
        /// data is vialation msg info.
        /// </summary>
        public void raiseReqURLViolationEvt(Object sender, String msg) {
            Log.println_eng("T = UI,     *** raise url violation msg event ");
            CommonEventArgs evt = new CommonEventArgs(sender, msg);
            OnReqURLViolationEvt(evt);
        }
        ///// <summary>
        ///// sender is the WebEngine, event data is null ??? how to use, it should be refined 
        ///// TODO 
        ///// </summary>
        //public event EventHandler<CommonEventArgs> RequestTimeoutEvt;
        //protected virtual void OnRequestTimeoutEvt(CommonEventArgs e) {
        //    //TODO ???? how to used 
        //    EventHandler<CommonEventArgs> requestTimeoutEvt = RequestTimeoutEvt;
        //    if (requestTimeoutEvt != null) {
        //        requestTimeoutEvt(this, e);
        //    }
        //}
        //public void raiseRequestTimeoutEvt(Object sender) {
        //    Log.println_eng("T = UI,     *** raise request timeout event ");
        //    CommonEventArgs evt = new CommonEventArgs(sender, null);
        //    OnRequestTimeoutEvt(evt);
        //}
        #endregion events area
        #region background worker
        /// <summary>
        /// start the backgroundworker, before run this method, make sure the 
        /// SRoot has been set
        /// </summary>
        /// <param name="op"></param>
        public void RunWorkerAsync(Operation op) {
            bool sync = syncServerTime();
            if (sync) {
                this.worker.RunWorkerAsync(op);
            }
        }
        /// <summary>
        /// Sync server time with engine before the scrip running. 
        /// </summary>
        private bool syncServerTime() {
            DateTime st = WebUtil.getServerTime(this.SRoot.TargetWebURL);            
            if (st == DateTime.MinValue) {
                MessageBox.Show("Can not connect to server, please try later. ");
                return false;
            }            
            this.startDate = new DateTime(st.Ticks);
            this.ServerTime = st;
            return true;
        }
        /// <summary>
        /// Stop the engine runing if have. 
        /// </summary>
        public void stopScript() {
            if (this.Status != ENGINE_STATUS.STOPED && this.currentOpw!=null) {
                if (Mode == MODE_RELEASE) {
                    this.RELEASE_CMD = RELEASE_CMD_STOP;
                } else {
                    this.DebugCmd = DEBUG_CMD.STOP;                    
                }
                this.unlockEngine();
            }
        }
        /// <summary>
        /// control the script execution, it will 
        /// 1. execute operation itself in UI thread, 
        /// 2. wait a time align with operation wait time
        /// 3. log messages : system time + op log info. 
        /// 4. update parameter data mapping 
        /// 5. check OpConditions one by one to route the logic to first op pass the condition if have or just stop here. 
        /// 6. If errors, turn to the exception rule handling.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">WorkerData </param>
        void worker_DoWork(object sender, DoWorkEventArgs e) {
            OpWrapper opw = this.currentOpw;
            Log.println_eng("T = ENGINE, --- engine doWork started ! ");            
            // clean engine environment  
            resetEngine();
            if (Mode == MODE_RELEASE) {
                this.RELEASE_CMD = RELEASE_CMD_START;
            }
            // Log script start info.
            logger.logScriptStart(this.SRoot.Name);
            // update the opw info.
            Operation op = e.Argument as Operation;            
            opw.Op = op;
            opw.Status = OpStatus.READY;            

            // this is used to make the root process was initialized later. 
            if (op == SRoot.ProcRoot.StartOp) {
                opw.Op = SRoot.ProcRoot;                
                opw.Status = OpStatus.RESTART_SCRIPT;
            } else if (Mode == MODE_DEBUG) {
                initProcStackWhenDbg(op);
            }
            int count = 0;
            while (opw.Op != null) {
                if (exe_code == EXE_CODE_URL_VIOLATION) {
                    Log.println_eng("T = ENGINE, URL violation. script stoped. ");
                    setStopStatus(opw);                    
                    break;
                }
                if (opw.Status == OpStatus.RESTART_SCRIPT) {
                    tryRestartScript();
                    if (this.Status != ENGINE_STATUS.RUNING) {
                        break;
                    }                    
                    count = 0;
                }
                
                Log.println_eng("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ count = " + count++ + " ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");                
                doExecuteOp();                
            }

            logger.logScriptEnd(this.SRoot.Name);
            
            // update outer debug panel status
            if (Mode == MODE_DEBUG) {
                DebugCmd = DEBUG_CMD.NONE;            
            }
            // release engine and debug engine both need the stop event. 
            raiseDebugUIEvt(ENGINE_STATUS.STOPED);

            // clean engine data 
            cleanEngineData();
            Log.println_eng("T = ENGINE, --- engine doWork finished ! ");
        }
        /// <summary>
        /// this method is used to do UI action in browser thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            // do UI operaitons 
            if (e.ProgressPercentage == DO_OPERATION && e.UserState is OpWrapper) {
                executeOpInUI(e.UserState as OpWrapper);
            } else if (e.ProgressPercentage == CON_INPUT_WEBELEMENT) {
                // update the WebElement with refershed values
                updateConditionInput(e.UserState as WebElement);
            } else if (e.ProgressPercentage == CON_INPUT_ATTRIBUTE) {
                // update WebElementAttribute with refershed value
                WebElementAttribute wea = e.UserState as WebElementAttribute;
                WebElement we = wea.Collection.Owner as WebElement;
                if (we != null) {
                    updateConditionInput(we);
                }
            } else if (e.ProgressPercentage == WE_EXISTED_TEST) {
                doWEExistedTest(e.UserState as WebElement);
            } else if (isStatusCode(e.ProgressPercentage)) {
                raiseStatusChanged(this, e.ProgressPercentage);
            } else if (e.ProgressPercentage == Logger.LOG_APP_MSG) {
                String message = logger.DbgMsg.ToString();
                logger.DbgMsg.Remove(0, logger.DbgMsg.Length);
                raiseLogDbgMsgUpdated(this, message);
                //} else if (e.ProgressPercentage == REQ_TIMEOUT) {
                //    raiseRequestTimeoutEvt(this);
            } else if (e.ProgressPercentage == Logger.LOG_USER_MSG) {
                string message = logger.ReleaseMsg.ToString();
                logger.ReleaseMsg.Remove(0, logger.ReleaseMsg.Length);
                raiseLogReleaseMsgUpdated(this, message);
            }
            // check to make sure the UI thread and Engine thread sync, because the UI thread executing need 
            // some time, and when the UI thread finished, it will update the syncFlag status, so that the
            // engine will stop waiting and go on. 
            int time = 0;
            while (time < TIMEOUT_THREAD_LOCK && this.syncFlag == SYNC_ENGINE_PRE_LOCK) {
                Thread.Sleep(200);
                time += 200;
            }
            if (syncFlag == SYNC_ENGINE_LOCK) {
                // when UI operation finished, just release the engine thread lock
                this.unlockEngine();
            } else {
                syncFlag = SYNC_ENGINE_TIMEOUT;
            }
        }
        /// <summary>
        /// script run compeleted to a end status or was cancelled by something 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Log.println_eng("T = ENGINE, --- script run completed ==================================");
        }
        #endregion back ground worker
        #region engine thread handling
        /// <summary>
        /// reset the engine as initial status 
        /// </summary>
        private void resetEngine() {
            this.cleanEngineData();
            this.ReqTimeOut = TIMEOUT_WEB_REQUEST;
            this.ReqStartTime = DateTime.Now;
        }
        /// <summary>
        /// Raise the log message event and wait for the UI to finish the log output. 
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="flag"></param>
        internal void logScriptMsg(int msgType, int flag) {
            this.syncFlag = flag;
            worker.ReportProgress(msgType);
            this.lockEngine();
        }
        private void cleanEngineData() {
            this.currentOpw.reset();
            this.procStack.Clear();
            this.threadLock.Reset();
            this.debugLock.Reset();
            this.isLockerTimeout = false;
            //this.debugCmd = DEBUG_CMD.NONE;
            this.stepOverOp = null;
            this.isStepOverBreak = false;
            this.Status = ENGINE_STATUS.STOPED;
            logger.cleanMsgCache();            
            this.syncFlag = SYNC_ENGINE_TIMEOUT;
            this.TimeSensitive = false;
            this.RequestURL = null;
            this.exe_code = EXE_CODE_OK;
            //this.bwState = WebBrowserReadyState.Uninitialized;
        }
        /// <summary>
        /// take effect if the Opw.Status == OpStatus.RESTART_SCRIPT. 
        /// It is used to initial the engine environment and start to navigate to the first page.
        /// If ok, the engine.Status==ENGINE_STATUS.RUNNING else it means failed.
        /// </summary>
        private void tryRestartScript() {
            Log.println_eng("T = ENGINE, tryRestartScript. Start ");
            OpWrapper opw = currentOpw;
            // clean cached engine data if have 
            resetEngine();
            // start to load the first page             
            Operation fakeOp = ModelFactory.createOperation(OPERATION.OPEN_URL_T);
            fakeOp.Name = "FakeOp_LoadFirstPage";
            fakeOp.Input = this.SRoot.TargetWebURL;
            // start to debug script 
            this.Status = ENGINE_STATUS.INITIALIZING;
            opw.Op = fakeOp;
            // navigate the browser to the script target page
            raiseOpUIEvt(opw);

            // Just click the start button, wait for page loaded completed
            if (this.Status == ENGINE_STATUS.INITIALIZING) {
                int time = 0;
                while (time < Constants.PAGE_LOAD_TIME_OUT) {
                    if (this.Status == ENGINE_STATUS.RUNING) {
                        break;
                    } else {
                        Thread.Sleep(1000);
                    }

                    time += 1000;
                }
                if (time >= Constants.PAGE_LOAD_TIME_OUT) {
                    Log.println_eng("T = ENGINE, %%%%%%%%%% start script , load first page timeout %%%%%%%%%%%%%%%");
                }
            }
            if (Status == ENGINE_STATUS.RUNING) {
                opw.Status = OpStatus.READY;
                opw.Op = SRoot.ProcRoot;
            }

            Log.println_eng("T = ENGINE, tryRestartScript. End, Status = "+this.Status+", next op = "+getOpwOpName(opw));
        }
        /// <summary>
        /// This is used to handle that in debug model, if it is start from a Non-script root start operation. 
        /// It will need to initialized necessary proc info into procStack. it is ScriptRootProc, op's container
        /// process, and op(if it is a process).
        /// </summary>
        /// <param name="op"></param>
        private void initProcStackWhenDbg(Operation op) {
            // This means that in debug model, the start op is not the root process start op. 
            // It will initial the script root process and the op container process environment
            // and push them into procStack. 
            createAndInitProc(SRoot.ProcRoot);
            if (op == SRoot.ProcRoot) {
                return;
            }
            // add the parent process to the procStack.
            Process pp = ModelManager.Instance.getOwnerProc(op);
            if (pp != SRoot.ProcRoot) {
                createAndInitProc(pp);
            }
            // add current proc to the procstack if it is 
            if (op is Process) {
                createAndInitProc(op as Process);
            }
        }
        /// <summary>
        /// 1. check the End status, if it is process end, update currentOpw.Op. 
        /// 2. execute operation itself in UI thread, 
        /// 3. wait a time as the operation time field. 
        /// 4. log messages : system time + op log info.
        /// 5. update the operation parameters data update. (Till now an operation execution finished)
        /// 6. check OpConditions one by one to route the logic to first op pass the condition if have or just stop here. 
        /// 7. handle exceptions/errors by rule.
        /// </summary>
        private void doExecuteOp() {
            Log.println_eng("T = ENGINE, doExecuteOp(),======= Status = "+this.currentOpw.Status+", op = "+getOpwOpName(currentOpw)+", isProc = "+ (currentOpw.Op is Process)+" ########################## ");
            OpWrapper opw = this.currentOpw;
            if (opw == null || opw.Op == null) {
                Log.println_eng("T = ENGINE, fetal ERROR, opw is null or opw.op == null, opw = "+opw);
                return;    
            }
            // reset the OpConditions if there are some dirty data in previous execution. 
            // make sure that before executing, each op's condition is non-initialized. 
            ModelManager.Instance.resetOpConditions(opw.Op);
            handleOpExecuteTime();
            handleCleanProcs();
            if (Mode == MODE_RELEASE) {
                if (RELEASE_CMD == RELEASE_CMD_STOP) {
                    Status = ENGINE_STATUS.STOPED;
                    // update user log 
                    string txt = LangUtil.getMsg("log.debug.cmd.stop.text1");
                    logger.ReleaseMsg.Append(txt);
                    this.syncFlag = SYNC_ENGINE_TIMEOUT;
                    worker.ReportProgress(Logger.LOG_USER_MSG);
                    // update engine status 
                    setStopStatus(currentOpw);
                    // raise engine stoped. 
                    raiseDebugUIEvt(ENGINE_STATUS.STOPED);
                    return;
                }
            }
            #region handle debug mode
            Log.println_eng("T = ENGINE, doExecute cmd = " + DebugCmd);
            if (Mode == MODE_DEBUG) {
                // handle debug model
                if (DebugCmd == DEBUG_CMD.STOP) {
                    Status = ENGINE_STATUS.STOPED;
                    DebugCmd = DEBUG_CMD.NONE;
                    string txt = LangUtil.getMsg("log.debug.cmd.stop.text1");
                    logger.DbgMsg.Append(txt);
                    this.syncFlag = SYNC_ENGINE_TIMEOUT;
                    worker.ReportProgress(Logger.LOG_APP_MSG);
                    updateStepOverWhenJump();
                    setStopStatus(opw);                    
                    raiseDebugUIEvt(ENGINE_STATUS.STOPED);
                    return;
                } else if (DebugCmd == DEBUG_CMD.STEP_OVER) {
                    this.isStepOverBreak = false;
                    this.stepOverOp = opw.Op;
                } else if (DebugCmd == DEBUG_CMD.STEP_INTO) {
                    if (opw.Op is Process) {
                        Process proc = opw.Op as Process;
                        if (opw.Status == OpStatus.PROC_END) {
                            stepOverOp = proc;
                        } else {
                            this.stepOverOp = proc.StartOp;
                        }                        
                    } else {
                        this.stepOverOp = opw.Op;
                    }
                    this.isStepOverBreak = false;
                }
                DebugCmd = DEBUG_CMD.NONE;
                checkBreakPoints(opw.Op);
                // return and wait for next debug command, just consume the BREAK_POINT status here. 
                if (Status == ENGINE_STATUS.BREAK_POINT) {
                    Status = ENGINE_STATUS.RUNING;
                    return;
                }
            }
            #endregion handle debug model
            if (Status != ENGINE_STATUS.RUNING) {
                Status = ENGINE_STATUS.RUNING;
            }            
            #region handle End operation / or Non-next operation
            // Reach an process end state, just use the container process as the op 
            if (opw.Op.OpType == OPERATION.END/* || 
                ( !(opw.Op is Process) && opw.Op.OpConditions.Count == 0)*/) {
                if (procStack.Count > 0) {
                    Process proc = procStack.Peek();
                    Process proc1 = ModelManager.Instance.getOwnerProc(opw.Op);
                    if (proc1 == proc) {                        
                        // Make sure that the parameter update can be updated. 
                        // take effect if (opw.Status == OpStatus.READY || opw.Status == OpStatus.UPDATE_PARAM_WE_FOUND)
                        handleOpParamUpdate();
                        // Log the Operation End Node if it has parameters update
                        logger.logOpEndIfNeed(opw.Op);
                        // Root process end
                        if (procStack.Count == 1) {
                            Log.println_eng("T = ENGINE, ---> Root Process ended. ");
                            setStopStatus(opw);
                            return;
                        } else {
                            Log.println_eng("T = ENGINE, ---> Next proc = " + proc.Name);
                            updateStepOverWhenJump();
                            opw.Op = proc;
                            opw.Status = OpStatus.PROC_END;
                        }
                    } else {
                        //TODO log fetal error
                        // because in debug mode, sometimes developer can run script from a operation in a process. 
                        if (Mode == MODE_RELEASE) {
                            Log.println_eng("T = ENGINE, ERROR, fetal error with propstack status Error");
                        }
                    }
                } else {
                    if (Mode == MODE_DEBUG) {
                        Process proc = ModelManager.Instance.getOwnerProc(opw.Op);// opw.Op.Collection.Owner as Process;
                        if (proc != null && proc.Collection != null) { // filter the script root proc
                            updateStepOverWhenJump();
                            opw.Op = proc;
                            opw.Status = OpStatus.PROC_END;

                            Log.println_eng("T = ENGINE, --|> Next Proc = " + proc.Name + ", this is a start inside process run");
                            return;
                        }
                    }

                    // reached to the root process end node
                    Log.println_eng("T = ENGINE, --> Next op = null, The end with proc statck is empty. ScriptStoped ");
                    updateStepOverWhenJump();
                    setStopStatus(opw);
                }
                return;
            }
            #endregion handle End operation / or Non-next operation   
            // check whether the input is WEA or Parameter with WEA, if so, update to make sure the WebElement is up-to-date.
            // take effect only opw.Status == OpStatus.READY
            handleOpInput();
            // If the proc statck is empty, initialize process and push it into stack. 
            // If it is operation, execute UI thread, and waittime, and check the execute result  
            // when UI operation finished.
            // take effect if the opw.Status == OpStatus.READY
            bool startOpHelp = false;
            if (opw.Op is Process) {
                startOpHelp = true;
            }
            handleOp();
            if (startOpHelp == true && opw.Op.OpType == OPERATION.START) {
                return;
            }
            Log.println_eng("T = ENGINE, After handleOp(),            opw.op=" + getOpwOpName(opw) + ", status = " + opw.Status);
            // take effect if (opw.Status == OpStatus.READY || opw.Status == OpStatus.UPDATE_PARAM_WE_FOUND)
            handleOpParamUpdate();
            // log user message, it seperate the User message here, to make sure that when all Operation issues(UI op and update Parameters)
            // finished, it will output the user log, so that make sure all touched parameter and WE has valid values. 
            logger.logOpMsg4USER(opw.Op);
            Log.println_eng("T = ENGINE, After handleOpParamUpdate(), opw.op=" + getOpwOpName(opw) + ", status = " + opw.Status);
            // check conditions one by one and move to next op if have
            // update the procStack status and process environments, Parameters mapping
            if (opw.Status == OpStatus.READY || opw.Status == OpStatus.CON_WE_FOUND || opw.Status == OpStatus.PROC_END) {
                Log.println_eng("T = ENGINE, Before check Conditions, opw.op=" + getOpwOpName(opw) + ", status = " + opw.Status);
                checkConditions();
                Log.println_eng("T = ENGINE, After check Conditions,  opw.op=" + getOpwOpName(opw) + ", status = " + opw.Status);
            }else if(opw.Status == OpStatus.OPC_PARAM_MAPPING_WE_FOUND){
                Log.println_eng("T = ENGINE, Before gotoNextOp, opw.op=" + getOpwOpName(opw) + ", status = " + opw.Status);
                gotoNextOp(opw.Opc);
                Log.println_eng("T = ENGINE, After gotoNextOp,  opw.op=" + getOpwOpName(opw) + ", status = " + opw.Status);
            }

            if (opw.Status != OpStatus.READY && opw.Status!= OpStatus.STOP && opw.Status!= OpStatus.PROC_END) {
                // if the operation execution with errors it will find necessary rules to handle 
                handleExecuteErrors();
            }
        }

        /// <summary>
        /// Take effect only the opw.Status = CLEAN_PROC_ENV_AND_GOTO, clean the cleanProcs if it is exactly 
        /// matched process stack. the first proc in the list should be the top one in the engine stack. 
        /// </summary>
        private void handleCleanProcs() {
            OpWrapper opw = this.currentOpw;
            if (opw.Status == OpStatus.CLEAN_PROC_ENV_AND_GOTO) {
                if (opw.CleanedProcs == null) {
                    Log.println_eng("T = ENGINE, Error, Handle clean process error, candidate process list is null. ");
                    opw.Op = null;
                    opw.Status = OpStatus.EXE_ERROR;
                    return;
                }
                for (int i = 0; i < opw.CleanedProcs.Count; i++) {
                    if (procStack.Peek() == opw.CleanedProcs[i]) {
                        cleanProcEnv();
                    } else {
                        Log.println_eng("T = ENGINE, Error, Handle clean process error, candidate process list doesn't match the engine process stack.");
                        opw.Op = null;
                        opw.Status = OpStatus.EXE_ERROR;
                        return;
                    }
                }
                opw.CleanedProcs = null;
                opw.Status = OpStatus.READY;
            }
        }
        /// <summary>
        /// Check the current server time, if the time is equal or after the execute time, 
        /// engine logic will go on, else it be blocked until the time reached. 
        /// </summary>
        private void handleOpExecuteTime() {
            OpWrapper opw = this.currentOpw;
            // NO execute time defined 
            if(opw.Op.ExeuteTime == null || opw.Op.ExeuteTime.Length==0){
                return ;
            }
            // update base date if need
            if (WebUtil.isDateAfter(ServerTime, this.startDate)) {                
                this.startDate = new DateTime(ServerTime.Ticks);
            }
            DateTime exeTime = ModelManager.Instance.getOpExeDateTime(opw.Op.ExeuteTime, this.startDate);
            if (exeTime == DateTime.MinValue) {
                return;
            }
            DateTime currTime = ServerTime;
            if (currTime > exeTime) {
                int waittime = currTime.Millisecond - exeTime.Millisecond;
                Log.println("T = ENGINE, currentTime = "+currTime+", exeTime = "+exeTime+", waitingtime = "+waittime);
                lockEngine(waittime,false);
            }            
        }
        /// <summary>
        /// check whether the input is WEA or Parameter with WEA, if so, update to make sure the WebElement is up-to-date.
        /// </summary>
        private void handleOpInput() {
            OpWrapper opw = this.currentOpw;
            object obj = opw.Op.Input;
            if (opw.Status == OpStatus.READY) {
                if (opw.Op.Input is Parameter) {
                    Parameter param = opw.Op.Input as Parameter;                    
                    obj = param;
                    // urgly code, just for a set loop
                    if (param.Type == ParamType.SET && opw.Op.OpType == OPERATION.NOP) {
                        param.ConsumeSet = true;
                        object oo = param.RealValue;
                    }
                }      

                WebElement we = ModelManager.Instance.tryGetRealWE(obj, opw, this, SRoot.Timeout);
                if (we != null && we.IsRealElement == false) {
                    opw.Status = OpStatus.OP_WE_NOT_FOUND;
                    opw.NullWE = we;
                }
            }            
        }
        /// <summary>        
        /// If the proc statck is empty, initialize process and push it into stack. 
        /// If it is operation, execute UI thread, and waittime, and check the execute 
        /// result when UI operation finished.
        /// </summary>
        private void handleOp() {
            OpWrapper opw = this.currentOpw;
            if (opw.Status != OpStatus.READY) {
                // e.g. parameters updating. the operation was exected before, in this round just continue to 
                // finish the parameters update. 
                return;
            } 
            Operation op = opw.Op;
            if (op is Process) {
                Process proc = op as Process;
                // flow enter a new process, add the process into stack, root process in release mode
                if (procStack.Count == 0) {
                    createAndInitProc(proc);
                    // log proc start messages : system time + op log info. 
                    logger.logProcMsg(proc, Logger.LOG_PROC_START);
                }
                // if it is an existed running process or new process, and not finished
                // just execute the process again.
                // start the process
                Log.println_eng("T = ENGINE, ====>> Next op is proc start op, op = " + proc.StartOp.Name);
                opw.Op = proc.StartOp;
                opw.Status = OpStatus.READY;
                resetOpw(opw);
                return;
            } else if (op is Operation) {
                // handle common operaton 
                Operation oper = op as Operation;
                opw.NullWE = null;
                if (oper.OpType == OPERATION.START) {
                    logger.logOpMsg4APP(op);
                }else if (oper.OpType != OPERATION.END) {
                    bool logged = false;
                    // Here do a WebElement existed test, it can reduce the NullWebElement rule execute times. 
                    WebElement twe = ModelManager.Instance.tryGetRealWE(oper.Element, opw, this, SRoot.Timeout);
                    if (twe != null && twe.IsRealElement) {
                        // log operation messages before it executing in UI, Here only log the APP level msg it is used 
                        // for script developer to clearly understand when/what happened. 
                        logger.logOpMsg4APP(op);                            
                        logged = true;
                    }
                    if (oper.OpType != OPERATION.NOP && oper.OpType!= OPERATION.START && oper.OpType!= OPERATION.END) {                        
                        // raise the UI event to execte op in UI thread, and wait a operaton wait time
                        raiseOpUIEvt(opw);
                    }
                    if (logged == false) {
                        // log operation messages before it executing in UI.
                        logger.logOpMsg4APP(op);
                    }
                    if (opw.Status == OpStatus.READY) {
                        // do result check when operation execute finished.
                        afterExecuteOpInUI(opw);
                    }
                }
            }
        }
        /// <summary>
        /// Initialized Process parameters real value as designed value. 
        /// and push the helper into procStack. 
        /// </summary>
        /// <param name="proc"></param>
        private void createAndInitProc(Process proc) {            
            initProc(proc);
            procStack.Push(proc);
        }
        /// <summary>
        /// Initialized the process parameters real value as the design value. 
        /// </summary>
        /// <param name="helper"></param>
        private void initProc(Process proc) {
            if (proc == null) {
                return;
            }
            // initial parameter real value with design value. 
            this.initProcParams(proc);
        }        
        /// <summary>
        /// do result check when operation execute finished, it is used to do extension after operaiton
        /// executed in UI, e.g. wait for a period based on the operation waittime. 
        /// </summary>
        /// <param name="opw"></param>
        private void afterExecuteOpInUI(OpWrapper opw) {
            // clean the WebElementAttributes real value 
            // ModelManager.Instance.cleanWebElementAttributes(opw.Op.Element);
            // wait a time 
            int time = opw.Op.getWaitTime();
            if (time > 50) {
                Thread.Sleep(time);
            }
            //here can do some extensions 
        }
        /// <summary>
        /// update the OpWrapper items based on the Status, 
        /// If the status is STOP or READY, clean the opw environment(Except Status and Op) 
        /// </summary>
        /// <param name="opw"></param>
        internal void resetOpw(OpWrapper opw) {
            if (opw == null) {
                Log.println_eng("T = ENGINE, Fetal ERROR, OpWrapper is null !!!!!!!!!!!!!!!!!!!!!!");
                return;
            }
            if (opw.Status == OpStatus.STOP || opw.Status == OpStatus.READY) {
                OpStatus status = opw.Status;
                Operation op = opw.Op;
                opw.reset();
                opw.Op = op;
                opw.Status = status;
            }
        }
        /// <summary>
        /// Marked opw with stop status 
        /// </summary>
        /// <param name="opw"></param>
        internal void setStopStatus(OpWrapper opw) {
            if (opw != null) {
                opw.Op = null;
                opw.Status = OpStatus.STOP;
                this.resetOpw(opw);
            }
        }        
        /// <summary>
        /// update the step over flags when the script next operation will be changed if need, It is used offen follow
        /// a opw.Op = anotherOp. 
        /// </summary>
        internal void updateStepOverWhenJump() {
            //for rule handler , please carefully check and compared each place how this method used. 
            // e.g. for the step over, when process finished, it will reset the stepOverOp as the proc for avoid some 
            // non-stop runt problems. 
            if (Mode == MODE_DEBUG) {
                if (stepOverOp != null && stepOverOp.Equals(this.currentOpw.Op)) {
                    this.isStepOverBreak = true;
                    this.stepOverOp = null;
                }
            }
        }
        /// <summary>
        /// This is used to find proper rule to handle relative execution/mapping errors. It may find more than one 
        /// rules, it will execute each rule in-order, if a rule execution success, it will stop later rule execution.
        /// </summary>
        /// <param name="opw"></param>
        private void handleExecuteErrors() {
            OpWrapper opw = this.currentOpw;
            Log.println_eng("T = ENGINE, handleExecuteErrors ..., op = "+getOpwOpName(opw));
            if (opw == null || opw.Op == null) {
                setStopStatus(opw);
                string txt = LangUtil.getMsg("log.exe.exp.text1");
                logger.buildLogMsg(txt);
                return;
            }
            RuleTrigger rtg = getRuleTrigger(opw.Status);
            if (rtg == RuleTrigger.INVALID) {
                Log.println_eng("T = ENGINE, ERROR, can not find rule trigger for opw.status = "+opw.Status);
                return;
            }
            bool handled = false;
            // For a rule list, it only handles the rules on Operation itself, Container Process, and scriptRoot rule in order. 
            // Note that: it will execute the matched actions list in order, if there is one action success with the Operation jump
            // it will break the later actions execute if have. 
            List<OperationRule> rlist = getRule(rtg, opw.Op);
            foreach (OperationRule r in rlist) {
                Operation opBefore = opw.Op; 
                bool ok = handleRule(r, opw);
                if (ok) {
                    handled = true;
                    // there is an action make the logic changed to another Operation. 
                    if (opBefore != opw.Op) {
                        break;
                    }
                }
            }
            // No rule handled the exceptions 
            if (handled == false) {                
                //////////////// log info //////////////////////////////////////
                string opstr = getOpwOpName(opw);
                Log.println_eng("T = ENGINE, ERROR, No rule handle the exceptions successfully. Script Stoped. op = "+opstr);
                ////////////////////////////////////////////////////////////////
                // stop the script. 
                setStopStatus(currentOpw);
            }
        }
        /// <summary>
        /// NOTES : that here are two places to handle the url security check violation. 
        /// 1. in before UI operation execute, check the url
        /// 2. in the browser navigating event. (this is the core place, double check)
        /// </summary>
        /// <param name="opw"></param>
        /// <param name="url"></param>
        private void handleURLViolation(OpWrapper opw, string url) {
            Log.println_eng("T = ENGINE, ERROR, Non-trusted URL. url = " + url);
            // raise the url vialation event 
            string txt = LangUtil.getMsg("log.req.url.violation.text1");
            string tmsg = txt + url;
            string str = "\n" + tmsg + "\n";
            logger.buildLogMsg(str);
            this.raiseReqURLViolationEvt(this, tmsg);
            // stop the script runing, 
            this.exe_code = EXE_CODE_URL_VIOLATION;
            setStopStatus(opw);
        }

        /// <summary>
        /// get current opw operation name or null or null=='xxx' comments if errors 
        /// </summary>
        /// <returns></returns>
        private string getOpwOpName(OpWrapper opw) {
            string opstr = "null";
            if (opw == null) {
                opstr = "opw == null";
            } else if (opw.Op == null) {
                opstr = "opw.Op = null";
            } else {
                opstr = opw.Op.Name;
            }
            return opstr;
        }        
        /// <summary>
        /// return the Rule trigger part relative the status, if not find return OperationRule.INVALID
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private RuleTrigger getRuleTrigger(OpStatus status) {
            RuleTrigger tgr = RuleTrigger.INVALID;            
            if (status == OpStatus.OP_WE_NOT_FOUND || status == OpStatus.UPDATE_PARAM_WE_NOT_FOUND 
                || status == OpStatus.CON_WE_NOT_FOUND || status == OpStatus.OPC_PARAM_MAPPING_WE_NOT_FOUND) {
                tgr = RuleTrigger.NULL_ELEMENT;
            } else if (status == OpStatus.REQ_TIME_OUT) {
                tgr = RuleTrigger.REQ_TIMEOUT;
            } else if (status == OpStatus.EXE_ERROR) {
                tgr = RuleTrigger.OP_EXE_ERROR;
            } else if (status == OpStatus.NO_NEXT_OP_FOUND) {
                tgr = RuleTrigger.NO_NEXT_OP_FOUND;
            }

            return tgr;
        }
        /// <summary>
        /// Return the proper rule list that matched the trigger, the order is 
        /// Operation rule -> Proces Rule -> script Rule, the operation rule is the first one. 
        /// But if there are duplicated rule action, the nearest will only take effect. 
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private List<OperationRule> getRule(RuleTrigger trigger, Operation op) {
            List<OperationRule> list = new List<OperationRule>();            
            // first check the operation itself
            foreach (OperationRule r in op.Rules) {
                if (r.Trigger == trigger) {
                    if (ModelManager.Instance.isValidRuleForList(list, r)) {
                        list.Add(r);
                    }
                    break;
                }
            }            
            // check the operations's container process if not find 
            Process parentProc = ModelManager.Instance.getOwnerProc(op);
            if (parentProc != null) {
                foreach (OperationRule r in parentProc.Rules) {
                    if (r.Trigger == trigger) {
                        if (ModelManager.Instance.isValidRuleForList(list, r)) {
                            list.Add(r);
                        }
                        break;
                    }
                }
            }
            // check the script rule if not find before             
            foreach (OperationRule r in SRoot.ProcRoot.Rules) {
                if (r.Trigger == trigger) {
                    if (ModelManager.Instance.isValidRuleForList(list, r)) {
                        list.Add(r);
                    }
                    break;
                }
            }
            
            return list;
        }
        /// <summary>
        /// check the operation conditions, find the first passed condition and
        /// turn the logic to relative operation. 
        /// if in debug mode, it will stop at the operation checking, else if it is 
        /// release mode, it will directly route the logic to relative operation
        /// 
        /// In this logic, it is used maintain the procStack status and environment initilization
        /// </summary>
        /// <param name="opw"></param>
        private void checkConditions() {
            OpWrapper opw = this.currentOpw;
            // check the whether the opw.op has OpConditions
            if (opw.Op.OpConditions.Count == 0) {
                opw.Status = OpStatus.NO_NEXT_OP_FOUND;                
                return;
            }

            // if it is proc end, update status
            if (opw.Status == OpStatus.PROC_END) {
                opw.Status = OpStatus.READY;
            }
            
            bool r = false;
            // ToBe checked OpConditions 
            List<OpCondition> opclist = new List<OpCondition>();
            opclist.AddRange(opw.Op.OpConditions.ToArray());
            // initial env
            if (opw.Status == OpStatus.CON_WE_FOUND) {
                opclist = opw.NullWEOpcList;
                opw.Status = OpStatus.READY;
                //if (opw.OpcNullWEList != null) {
                //    opw.OpcNullWEList.Clear();
                    opw.OpcNullWEList = null;
                //}
            }
            opw.Opc = null;
            // record to clean all the OpConditions condition flags. 
            Operation op = opw.Op;

            // contains all the failed OpConditions caused by WebElement not found. 
            List<OpCondition> nullweOpclist = new List<OpCondition>();
            // all founded NullWE list in the processed OpCondition
            List<WebElement> opcNullWEList = new List<WebElement>();

            foreach (OpCondition opc in opclist) {
                opcNullWEList.Clear();                
                // It will update the opcNullWEList in the method if have 
                r = doExecuteOpc(opc, opcNullWEList);
                if (r == true) {
                    // reset all the OpConditions condition IsChecked and Result flag. 
                    //ModelManager.Instance.resetOpConditions(op);
                    return;
                } else {
                    if (opcNullWEList.Count > 0) {
                        if (!nullweOpclist.Contains(opc)) {
                            nullweOpclist.Add(opc);
                        }
                        // Here make sure the first opc in opw.NullWEOpcList matched the nullWEList.
                        if (opw.OpcNullWEList == null) {
                            opw.OpcNullWEList = new List<WebElement>();
                            opw.OpcNullWEList.AddRange(opcNullWEList);
                        }
                    }
                }
            }

            if (opw.NullWEOpcList != null) {
                opw.NullWEOpcList.Clear();
                opw.NullWEOpcList = null;
            }

            if (r == false) {
                if (nullweOpclist.Count > 0) {
                    // this means that maybe the failed OpCondition can be passed if the WebElement found. 
                    opw.Status = OpStatus.CON_WE_NOT_FOUND;                    
                    opw.NullWEOpcList = nullweOpclist;
                    opw.NullWE = null;
                } else {
                    // reached to the root process end node
                    Log.println_eng("T = ENGINE, --> checkConditions, No next op, The end with proc statck is empty. ScriptStoped ");                    
                    updateStepOverWhenJump();
                    // commnets this line to make sure the Rule part handle the trigger correctly. 
                    //opw.Op = null; 
                    opw.Status = OpStatus.NO_NEXT_OP_FOUND; // OpStatus.STOP;
                    //resetOpw(opw);
                }
            }
        }
        /// <summary>
        /// handle the specified rule on the operation, usually in this method, the script logic will be changed to another( or maybe current) operation.
        /// if the operation was set to currentOp, return true, if no update, return false.
        /// </summary>
        /// <param name="rule">rule is applied on op/process itself </param>
        /// <param name="opw"></param>
        /// <returns>true, means that the rule will change the current engine control to execute another operatoin; false means that
        /// the rule do nothing to change engine executed operation.</returns>
        private bool handleRule(OperationRule rule, OpWrapper opw) {
            Operation currOp = opw.Op;
            bool result = false;
            // check and make sure all the WebElement was with real value if have. 
            if (rule.Params != null) {
                foreach (object obj in rule.Params) {
                    WebElement we = ModelManager.Instance.checkWE4ParamCmdIfNeed(obj, opw, this, Constants.CONDITION_INPUT_WE_CHECK_TIMEOUT);
                    // if there are one WebElement was not find when update parameters, just return.
                    if (we != null && we.IsRealElement == false) {                        
                        // failed rule
                        return false;
                    }
                }
            }
            // handle rule
            if (rule.Action == RuleAction.WaitUntilElemFind) {
                result = WaitUntilNullElemFindRule.execute(rule.Params,opw,this);
            } else if (rule.Action == RuleAction.RestartScript) {
                result = RestartScriptRule.execute(rule.Params, opw, this);
            } else if (rule.Action == RuleAction.Goto_Operation) {
                result = GotoOperationRule.execute(rule.Params, opw, this);
            }
            // check whether the flow was changed in the rule, and take some action. 
            Operation nextOp = opw.Op;
            // script logic changed by rule 
            if (currOp != null && currOp != nextOp) {
                //TODO Here is a mention that to handle the step over/ step into status if in debug mode
                // details please ref the how OpConditons changed next operation for stepOver/stepInto.    
                // handle step over/into 
            }
            return result;
        }

        /// <summary>
        /// execute the OpCondition, if the condition is true, it will route to next op. 
        /// It will update all WebElement element into opw.CacheWE  
        /// Notes: Here are some conditions 
        /// 1. if opc container only has one opc, 
        ///    a. if condition existed, check condition.
        ///    b. No condition existed, check the nextOp's Element existance if have
        /// 2. If opc container has more than one opc, for one opc
        ///    a. if condition existed, check condition.
        ///    b. No condition existed, check the nextOp's Element existance if have
        /// </summary>
        /// <param name="opc"></param>        
        /// <param name="opcNullWElist">OpConditions "Null WebElement" list.</param>
        private bool doExecuteOpc(OpCondition opc, List<WebElement> opcNullWElist) {
            Log.println_eng("T = ENGINE,   - <<< before check OPC, Candidate Next = " + opc.Op.Name+", Type = "+opc.Op.GetType().Name+", opType="+opc.Op.OpType.ToString());
            bool result = false;
            StringBuilder sb_opcSysLog = new StringBuilder();
            if (opc.Collection == null) {
                Log.println_eng("T = ENGINE, ERROR, opc container BEList is null !!!");
                return false;
            }
            int count = opc.Collection.Count;
            // scenario 1.a, return true 
            if (count == 1 && (opc.ConditionGroup == null || opc.ConditionGroup.Conditions == null || opc.ConditionGroup.Conditions.Count == 0)) {
                if (opc.Op == null || opc.Op.Element == null) {
                    result = true;
                }
            } 

            if (result == false && count >=1 && (opc.ConditionGroup == null || opc.ConditionGroup.Conditions == null || opc.ConditionGroup.Conditions.Count == 0)) { 
                // scenario 1.b,2.b, check the nextOp's Element existance if have
                result = doOpcHandleNextOpElement(opc);
            }else {
                // check the OpCondition's Condition item one by one, and calculate the result
                BaseElement be = null;
                int conLogLevel = 1;                
                for (int i = 0; i < opc.ConditionGroup.Conditions.Count; i++) {
                    be = opc.ConditionGroup.Conditions.get(i);
                    // calculate the condition or condition group result
                    calculateCondition(be, opcNullWElist, conLogLevel, sb_opcSysLog);
                    // if result is true, just break. for performance 
                    if (true == opc.ConditionGroup.Result) {
                        result = true;
                        break;
                    }
                }                
            }
            Log.println_eng("T = ENGINE,   - >>> after check OPC,  Candidate Next = " + opc.Op.Name +", Type = "+opc.Op.GetType().Name+", opType="+opc.Op.OpType.ToString()+ ", result = " + result);
            // log condition info if debug mode, with format : time : result,input1,input2
            logger.logOpcMsg(opc, result, sb_opcSysLog);

            if (true == result) {
                // route the operaiton flow to next operation 
                if (opc.Op != null) {
                    Log.println_eng("T = ENGINE, Before gotoNextOp, opw.op=" + opc.Op.Name + ", status = " + this.currentOpw.Status);
                    gotoNextOp(opc);
                    Log.println_eng("T = ENGINE, After gotoNextOp,  opw.op=" + opc.Op.Name + ", status = " + this.currentOpw.Status);
                    return true;
                } else { // next operation is null                   
                    throw new ScriptDesignException("ERROR ! There is a OpCondition without operation defined");
                }
            } // -:) end OpCondition result = true            
            
            return false;            
        }
        /// <summary>
        /// This method is use to pre-handle the OpCondition(without condition) next op 's Operated WebElement. 
        /// Handle the scenario: 2.b
        /// 2. If opc container has more than one opc, for one opc
        ///    a. if condition existed, check condition.
        ///    b. No condition existed, check the nextOp's Element existance if have
        /// </summary>
        /// <param name="opc"></param>
        private bool doOpcHandleNextOpElement(OpCondition opc) {            
            if (opc.Op == null) {
                Log.println_eng("T = ENGINE, ERROR, there is an opc without nextOp ! Model error. ");
                return false;
            } else {
                Operation op = opc.Op;
                if (op.Element != null) {
                    WebElement we = ModelManager.Instance.tryGetRealWE(op.Element, this.currentOpw, this, Constants.WE_CHECK_TIMEOUT);
                    // find the WebElement 
                    if (we != null && we.IsRealElement) {
                        return true;
                    } 
                }
            }
            return false;
        }
        /// <summary>
        /// update relative condition input object(WE/WEA/Param with WEA) with real value, and calculate the Condition/Group result
        /// </summary>        
        /// <param name="be">Condition or ConditionGroup</param>
        /// <param name="opcNullWEList"></param>
        /// <param name="conLogLevel">level of to control the log if in debug mode</param>
        /// <param name="sb_sysLog">It is used to format the opc log info</param>
        /// <returns></returns>
        private bool calculateCondition(BaseElement be, List<WebElement> opcNullWEList, int conLogLevel, StringBuilder sb_sysLog) {
            Log.println_eng("T = ENGINE, --- loglevel = " + conLogLevel + ", before handle condition, condition = " + be.Name);

            // this is used to record the first not-find WebElement in the opc
            if (be is Condition) {
                Condition con = be as Condition;
                // if the input is WebElemnet or attribute, it should raise event to 
                // get the real runtime value 
                if (con.Input1 is WebElement || con.Input1 is WebElementAttribute || con.Input1 is Parameter) {
                    // after finished, the con.Input1 will be updated with the runtime value                     
                    handleConditionInput(con.Input1, opcNullWEList);
                }
                if (con.Input2 is WebElement || con.Input2 is WebElementAttribute || con.Input2 is Parameter) {
                    // after finished, the con.Input1 will be updated with the runtime value 
                    handleConditionInput(con.Input2, opcNullWEList);
                }
                Log.println_eng("T = ENGINE, --- loglevel = " + conLogLevel + ", after calculate condition, condition = " + be.Name);
                // update log if in debug mode                 
                logger.logUpdateCondition(con, conLogLevel, sb_sysLog);

                return con.Result;
            } else if (be is ConditionGroup) {
                ConditionGroup cgrp = be as ConditionGroup;
                foreach (BaseElement bb in cgrp.Conditions) {
                    calculateCondition(bb, opcNullWEList, conLogLevel + 1,sb_sysLog);
                    if (cgrp.Result) {
                        //Log.println_eng("T = ENGINE, --- loglevel = " + logLevel + ", after calculate condition, conGrp = " + be.Name + ", result is true");
                        break;
                    }
                }
                Log.println_eng("T = ENGINE, --- loglevel = " + conLogLevel + ", after calculate condition, conGrp = " + be.Name);
                logger.logUpdateConGroup(cgrp, conLogLevel,sb_sysLog);

                return cgrp.Result;
            } else {
                String name = be != null ? be.Name : "null";
                throw new ConditionException("Invalid Condition Argument, it is need a Condition or ConditionGroup object, be=" + name + "is invalid");
            }
        }
        /// <summary>
        /// Update the WebElement or WebElementAttribute with real values
        /// add WE into opcNullWEList if the WebElement can not be found
        /// </summary>
        /// <param name="input">WebElement or WebElementAttribute</param>
        /// <param name="opcNullWEList"></param>
        private void handleConditionInput(object input, List<WebElement> opcNullWEList) {
            OpWrapper opw = this.currentOpw;
            // whether input is a WebElement or WEA 
            if (input is WebElement || input is WebElementAttribute) {
                WebElement we = ModelManager.Instance.tryGetRealWE(input, opw, this, Constants.CONDITION_INPUT_WE_CHECK_TIMEOUT);
                // src obj is a WE but can not be found 
                if (we != null && we.IsRealElement == false) {
                    if (!opcNullWEList.Contains(we)) {
                        opcNullWEList.Add(we);
                    }
                }
            }
        }
        /// <summary>
        /// change the opc.Op as next ToBe executed operation. and update environment if need 
        /// </summary>
        /// <param name="opc"></param>
        /// <returns></returns>
        private void gotoNextOp(OpCondition opc) {
            if (opc == null) {
                Log.println_eng("T = ENGINE, Fetal ERROR, opc is null");
                throw new ScriptRuntimeException("Error, opc is null. ");                
            }
            Process nextProc = opc.Op as Process;
            Process currProc = opc.Collection.Owner as Process;
            
            if(nextProc != null && this.currentOpw.Status == OpStatus.READY){
                // Initial the ToBe-ParamMapping target proc parameters real value. 
				initParamMappingToProc(nextProc);
            }
            if (currProc != null) {
                if (nextProc != null) {
                    // handle outer parameter mapping, only effect when proc->proc mapping 
                    bool ok = handleParamMapping(currProc, nextProc, opc.Mappings);
                    
                    if (!ok && currentOpw.Status == OpStatus.OPC_PARAM_MAPPING_WE_NOT_FOUND) {
                        this.currentOpw.Opc = opc;
                        return;
                    }
                }
                exitProc(currProc);
            }
            if (nextProc != null) {
                enterProc(opc);
            } else {
                enterOperation(opc);
            }

            // route logic to next operation or process
            Log.println_eng("T = ENGINE, --> Next op by Opc, next op = " + getOpwOpName(currentOpw));
        }
        /// <summary>
        /// initial the ToBe-ParamMapping target proc parameters real value. 
        /// </summary>
        /// <param name="nextProc">ToBe next process</param>
        private void initParamMappingToProc(Process nextProc) {
            if(!(nextProc is Process)) {
                return;
            }                                                 
            initProc(nextProc);
            this.paramMappingProc = nextProc;
        }
        /// <summary>
        /// enter an operation, update the parameters mapping and update the currentOpw.Op
        /// </summary>
        /// <param name="opc"></param>
        /// <param name="nullWE"></param>
        private void enterOperation(OpCondition opc) {            
            updateStepOverWhenJump();
            this.currentOpw.Op = opc.Op;
            this.currentOpw.Status = OpStatus.READY;
            resetOpw(this.currentOpw);            
        }
        /// <summary>
        /// When logic jump into a process, it will used to update the process environment. 
        /// initialize process environment,handle parameter mapping and push proc into stack.
        /// </summary>
        /// <param name="opc"></param>
        private void enterProc(OpCondition opc) {
            if (opc == null || !(opc.Op is Process)) {
                return;
            }
            
            Process proc = opc.Op as Process;
            // paramMappingHelper is modeled for the TOBE process, it contains the 
            // updated paramters info when in parameter mapping before. 
            if (this.paramMappingProc == null) {
                Log.println_eng("T = ENGINE, ERROR, parameterMappingProc is null");
                updateStepOverWhenJump();
                setStopStatus(currentOpw);             
                return;
            }else if(proc != this.paramMappingProc){
                Log.println_eng("T = ENGINE, ERROR, next op = "+proc.Name+", paramMappingProc proc = "+paramMappingProc);
                updateStepOverWhenJump();
                setStopStatus(currentOpw);
                return;
            }
            // update the flow next op 
            updateStepOverWhenJump();
            
            this.currentOpw.Op = opc.Op;
            this.currentOpw.Status = OpStatus.READY;
            resetOpw(this.currentOpw);
                                    
            procStack.Push(this.paramMappingProc);
            this.paramMappingProc = null;

            // log proc start messages : system time + op log info. 
            logger.logProcMsg(opc.Op as Process, Logger.LOG_PROC_START);
        }
        /// <summary>
        /// exit an existed process, clean the environment and pop up proc from stack.
        /// </summary>
        /// <param name="currProc"></param>
        private void exitProc(Process currProc) {            
            bool ok = procStatusValidation(currProc);
            if (ok) {
                logger.logProcMsg(currProc, Logger.LOG_PROC_END);
                // clean and pop up current process
                cleanProcEnv();
                
                // do result check when operation execute finished.
                if (this.currentOpw.Status == OpStatus.READY) {
                    afterExecuteOpInUI(this.currentOpw);
                }

            } else {
                throwProcStatusError();
            }
        }
        /// <summary>
        /// check whether the process is current top one in the proc stack. 
        /// return true if valid else return false
        /// </summary>
        /// <param name="currProc"></param>
        private bool procStatusValidation(Process currProc) {
            if (procStack.Count> 0 && currProc == procStack.Peek()) {                
                return true;
            }
            Log.println_eng("Fetal Error, process statck corrupted. !!!!! ");
            return false;
        }

        private void throwProcStatusError() {
            string top = "";
            if (procStack.Count > 0) {
                top = procStack.Peek().Name;
            }
            string log = "ERROR ! proc is not in the stack ! stack size=" + procStack.Count + ", top = " + top;
            Log.println_eng(log);
            //log fetal errors with process stack management
            throw new ScriptRuntimeException(log);
        }
        /// <summary>
        /// Pop up the process from engine status and clean process environment if there is some 
        /// data created in the initial state
        /// </summary>
        /// <param name="proc"></param>
        private void cleanProcEnv() {
            Process proc = procStack.Pop();
            resetProcParams(proc);
        }
        /// <summary>
        /// when an Operation/Process finished, it will update some parameters value by WebElementAttribute 
        /// or parmater with value of WebElementAttribute
        /// 
        /// The target parmeter and the src parameter must have the same type. when finished, target parameter value must be constant. 
        /// if there is set, all set items must be constant after update. 
        /// e.g update some WebElementAttribute's value into a string parameter for later use. 
        /// </summary>
        /// <param name="opw"></param>
        private void handleOpParamUpdate() {
            OpWrapper opw = this.currentOpw;
            if (opw.Op.Commands == null || opw.Op.Commands.Count == 0) {
                return;
            }
            if (opw.Status == OpStatus.READY || opw.Status == OpStatus.UPDATE_PARAM_WE_FOUND) {                
                // used for the Operation parameter update log. 
                StringBuilder sb_sysLog = new StringBuilder();
                // this one is used to better code format
                bool isReturn = false;
                // start update item index
                int start = 0;
                if (opw.Status == OpStatus.UPDATE_PARAM_WE_FOUND) {
                    start = opw.Op.Commands.IndexOf(opw.ParamCmd);
                    if (start == -1) {
                        Log.println_eng("T = ENGINE, ERROR, update operation mapping params error, op = " + this.getOpwOpName(opw) + ", paramItem = " + opw.ParamCmd);
                        string errMsg = LangUtil.getMsg("eng.op.update.err.msg1"); // Update parameters error
                        sb_sysLog.Append(errMsg);
                        isReturn = true;
                        //return;
                    } else {
                        opw.ParamCmd = null;
                        opw.Status = OpStatus.READY;
                    }
                }
                if (isReturn == false) {
                    // Check and execute the ParamCmd inorder. 
                    while (start < opw.Op.Commands.Count) {
                        ParamCmd update = opw.Op.Commands.get(start);
                        bool r = ParamCmdUtil.doUpdateParameter(update, opw, this, sb_sysLog);
                        if (r == false) {
                            isReturn = true;
                            break;
                        }
                        start++;
                    }
                }
                // log parameter update
                logger.logOpMsg4APP_ParamUpdate(sb_sysLog);
                // error occurred when do mapping 
                if (isReturn) {
                    opw.Status = OpStatus.EXE_ERROR;
                    opw.Op = null;
                }
            }
        }
        /// <summary>
        /// mapping WEA/Praramter accessable by current process to target process runtime public parameters.
        /// return true if the mapping success, or false if failed. 
        /// 
        /// src parameter can be currProc accessable parameters.         
        /// </summary>
        /// <param name="currProc"></param>
        /// <param name="nextProc"></param>
        /// <param name="mapping"></param>
        private bool handleParamMapping(Process currProc, Process nextProc, BEList<ParamCmd> mapping) {
            if (mapping == null || mapping.Count == 0) {
                return true;
            }
            OpWrapper opw = this.currentOpw;
            if (opw.Status == OpStatus.READY || opw.Status == OpStatus.OPC_PARAM_MAPPING_WE_FOUND) {
                // used for the Operation parameter update log. 
                StringBuilder sb_sysLog = new StringBuilder();
                // this one is used to better code format
                bool isReturn = false;
                // start update item index
                int start = 0;
                if (opw.Status == OpStatus.OPC_PARAM_MAPPING_WE_FOUND) {
                    start = mapping.IndexOf(opw.ParamCmd);
                    if (start == -1) {
                        Log.println_eng("T = ENGINE, ERROR, opc mapping params error, op = " + opw.Op + ", mapping item = " + opw.ParamCmd);
                        string errMsg = LangUtil.getMsg("eng.opc.update.err.msg1"); // Update link's target process parameters error
                        sb_sysLog.Append(errMsg);
                        isReturn = true;
                        //throw new ScriptRuntimeException("ERROR, opc mapping item can not be found, op = " + opw.Op + ", mapping item = " + opw.ParamCmd);
                    } else {
                        opw.ParamCmd = null;
                        opw.Status = OpStatus.READY;
                    }
                }
                if (isReturn == false) {
                    while (start < mapping.Count) {
                        ParamCmd update = mapping.get(start);
                        bool r = ParamCmdUtil.doUpdateParameter(update, opw, this, sb_sysLog);
                        if (r == false) {
                            isReturn = true;
                            break;
                        }                        

                        start++;
                    }
                }
                // log parameter update
                logger.logOpcMsg4APP_ParamUpdate(sb_sysLog);

                // error occurred when do mapping 
                if (isReturn) {
                    opw.Status = OpStatus.EXE_ERROR;
                    opw.Op = null;
                }
            }

            return true;
        }    
        /// <summary>
        /// just effective if it is in Debug mode
        /// if the operation is in the breakpoints list, the Engine thread will
        /// be blocked, and update the debug command as NONE.
        /// 
        /// until UI thread wakeup engine thread. 
        /// </summary>
        /// <param name="operation"></param>
        private void checkBreakPoints(Operation operation) {
            // check whether need to block the Engine thread on the breakpoints
            if (mode == MODE_DEBUG) {
                bool needbreak = false;
                foreach (Operation op in breakpoints) {
                    if (op == operation) {
                        needbreak = true;
                        break;
                    }
                }
                if (this.isStepOverBreak) {
                    needbreak = true;
                    isStepOverBreak = false;
                }
                if (needbreak) {
                    Status = ENGINE_STATUS.BREAK_POINT;
                    raiseDebugUIEvt(ENGINE_STATUS.BREAK_POINT);
                }
            }
        }        
        /// <summary>
        /// invoke the worker to update the debug status, so that the debug ui can 
        /// catch the status code to do action. 
        /// </summary>
        /// <param name="debugStatus"></param>
        private void raiseDebugUIEvt(ENGINE_STATUS debugStatus) {
            this.syncFlag = SYNC_ENGINE_TIMEOUT;
            worker.ReportProgress((int)debugStatus);
            // this cause it used avoid a backgroundworker finished
            if (debugStatus != ENGINE_STATUS.STOPED) {
                Log.println_eng("T = ENGINE, Debug locker lock the engine thread, waiting for UI thread wake up ...");
                DebugLock.WaitOne();
            }
        }
        /// <summary>
        /// raise event to notify UI thread handle the operation 
        /// </summary>
        /// <param name="opw"></param>
        private void raiseOpUIEvt(OpWrapper opw) {
            String opname = getOpwOpName(opw);
            int wtime = opw.Op.getWaitTime();

            Log.println_eng("T = ENGINE, - B2<<< - op = " + opname + ", before bgworker raise UI operation event, evt = " + this.getEventName(DO_OPERATION) + ",   wtime = " + wtime);
            this.syncFlag = SYNC_ENGINE_PRE_LOCK;
            worker.ReportProgress(DO_OPERATION, opw);
            this.lockEngine();
            Thread.Sleep(wtime);
            Log.println_eng("T = ENGINE, - B2>>> - op = " + opname + ", after bgworker raise UI operation event,  evt = " + this.getEventName(DO_OPERATION) + ",   wtime = " + wtime);
        }        
        /// <summary>
        /// raise event to notify UI thread to check wether the WebElement is existed
        /// </summary>
        /// <param name="we"></param>
        internal void raiseWEUITestEvt(WebElement we) {
            //String opname = we.Op.Name;           
            //Log.println_eng("T = ENGINE, - BWE<<< - op = " + we.Op.Name + ", before test we existed event, evt = " + this.getEventName(WE_EXISTED_TEST));
            if( (this.Mode == MODE_RELEASE && this.RELEASE_CMD == RELEASE_CMD_STOP) 
                || (this.Mode == MODE_DEBUG && this.DebugCmd == DEBUG_CMD.STOP) ){
                return;                
            }
            this.syncFlag = SYNC_ENGINE_PRE_LOCK;
            worker.ReportProgress(WE_EXISTED_TEST, we);
            this.lockEngine();
            //Log.println_eng("T = ENGINE, - BWE>>> - op = " + we.Op.Name + ", after test we existed event,  evt = " + this.getEventName(WE_EXISTED_TEST));
        }
        /// <summary>
        /// obj will be condition input, it maybe WebElement, WebElementAttribute
        /// such two types of input need to get the real value in run time from 
        /// UI thread, after UI thread finished the object will be updated with 
        /// real value. 
        /// 
        /// this is a synchronized method, it will blocked until the UI thread find 
        /// the obj and return, the blocked thread will be wakeup. 
        /// </summary>
        /// <param name="obj"></param>
        private void raiseOpcUIEvt(object obj) {
            BaseElement be = obj as BaseElement;
            int code = CON_INPUT_WEBELEMENT;
            if (obj is WebElementAttribute) {
                code = CON_INPUT_ATTRIBUTE;
            }
            Log.println_eng("T = ENGINE, --- before update condition input value, input = " + be.Name + ", evt = " + this.getEventName(code));
            if (obj is WebElement) {
                WebElement we = obj as WebElement;
                this.syncFlag = SYNC_ENGINE_PRE_LOCK;
                worker.ReportProgress(CON_INPUT_WEBELEMENT, we);
            } else if (obj is WebElementAttribute) {
                WebElementAttribute wea = obj as WebElementAttribute;
                this.syncFlag = SYNC_ENGINE_PRE_LOCK;
                worker.ReportProgress(CON_INPUT_ATTRIBUTE, wea);
            }
            // sync with the UI thread, when UI thread wakeup engine thread
            // the engine thread will on the condition checking 
            this.lockEngine();
            Log.println_eng("T = ENGINE, --- after update condition input value, input = " + be.Name);
        }
        /// <summary>
        /// lock background engine thread, until it is wake up by outer event. 
        /// </summary>
        private void lockEngine() {
            int waittime = TIMEOUT_THREAD_LOCK;
            lockEngine(waittime, true);
        }
        /// <summary>
        /// lock background engine thread, until it is wake up by outer event. 
        /// <param name="waittime"> thread wait time, in milliseconds </param>
        /// <param name="isCheckTimeout">whether it is a check timeout wait</param>
        /// </summary>
        private void lockEngine(int waittime,bool isCheckTimeout) {
            if (waittime < 0) {
                waittime = TIMEOUT_THREAD_LOCK;
            }
            tempcode++;
            Log.println_eng("T = ENGINE, /\\ lock engine thread ........................, sync code = " + tempcode);
            isLockerTimeout = true;
            this.syncFlag = SYNC_ENGINE_LOCK;
            this.threadLock.WaitOne(waittime);
            // this will occurr when background engine lock time out 
            if (isCheckTimeout && isLockerTimeout == true && currentOpw.Op != null) {
                currentOpw.Status = OpStatus.REQ_TIME_OUT;
                // update timeout log info 
                String time = DateTime.Now.ToString();
                logger.DbgMsg.Append(time).Append(" : Request timeout ").Append(", Operation=\"" + getOpwOpName(currentOpw)).Append("\"");
                Log.println_eng("T = ENGINE, --- B3< ########################## Request time out , Operatoin = " + getOpwOpName(currentOpw));
                this.syncFlag = SYNC_ENGINE_PRE_LOCK;
                worker.ReportProgress(Logger.LOG_APP_MSG);
                if (this.syncFlag == SYNC_ENGINE_TIMEOUT) {
                    Log.println_eng("T = Engine, --- B3> ########################## Sync time out, UI Thread wait sync timeout ");
                } else {
                    this.syncFlag = SYNC_ENGINE_LOCK;
                    Log.println_eng("T = ENGINE, --- B3> ########################## Request time out , Operatoin = " + getOpwOpName(currentOpw));
                    Log.println_eng("T = ENGINE, ---     ########################## lock engine thread ..............");
                    // wait for log info update in ui thread 
                    this.threadLock.WaitOne(waittime);
                }
            }
        }
        /// <summary>
        /// unlock the background engine thread, so that the engine can go to run the script
        /// </summary>
        private void unlockEngine() {
            Log.println_eng("T = UI,     \\/ unlock engine thread ......................, sync code = " + tempcode);
            isLockerTimeout = false;
            this.threadLock.Set();
        }
        #endregion engine thread handling
        #region ui thread handling        
        /// <summary>
        /// check wether the WebElement is existed in browser.
        /// </summary>
        /// <param name="opw"></param>
        private void doWEExistedTest(WebElement we) {
            if (we == null) {
                return;
            }
            if (we.TYPE == WEType.CODE || we.TYPE == WEType.ATTRIBUTE) {
                we.IsRealElement = false;
                WebUtil.updateWebElement(we, this.browser);
            }
        }
        /// <summary>
        /// execute the operation in UI thread, e.g, input a text, click a button
        /// </summary>
        /// <param name="opw">op can be Operation or OpCondition</param>
        private void executeOpInUI(OpWrapper opw) {
            Log.println_eng("T = UI,     *** op = " + getOpwOpName(opw) + ", execute Op in UI thread");
            Operation op = opw.Op;
            if (op == null) {
                return;
            }
            if (op.OpType == OPERATION.OPEN_URL_T || op.OpType == OPERATION.OPEN_URL_N_T) {
                string url = getOpInputValue(op,true);
                // security url check 
                if (this.SRoot != null && this.SRoot.UrlsLocked == true && url!=null && url.Length>0 && url!=Constants.URL_BLANK) {
                    bool isTrusted = WebUtil.isTrustedURL(url, this.SRoot.TrustedUrls);
                    if (!isTrusted) {
                        handleURLViolation(opw, url);
                        return;
                    }
                }
                if (op.OpType == OPERATION.OPEN_URL_N_T) {
                    WebUtil.openURLInNewTab(this.browser, url);
                } else {
                    WebUtil.openURLInCurrentTab(this.browser, url);
                }
                return;
            } else if (op.OpType == OPERATION.REFRESH) {
                //TODO
                return;
            } else if(op.OpType == OPERATION.INPUT || op.OpType == OPERATION.CLICK){
                // For Input and click Operation, Element Must be existed. 
                WebElement we = op.Element;
                IHTMLElement elem = null;
                if (we != null) {
                    // No need to set the IsRealElement false, because in handleOp(), that has tested the Input Element existance before 
                    // handled in UI. 
                    // we.IsRealElement = false;
                    elem = WebUtil.getFirstIHTMLElement(we, this.browser);
                } else { 
                    // Fetal error, input element missed
                    Log.println_eng("T = UI,     Script Fetal ERROR, Operation input Element is null. !!!*** ");
                    opw.Status = OpStatus.EXE_ERROR;
                    return;
                }
                if (elem == null) {
                    Log.println_eng("T = UI, ERROR, OP_WE_NOT_FOUND, we = "+we);
                    if (this.IsDocumentCompleted) {
                        opw.Status = OpStatus.EXE_ERROR;
                    } else {
                        opw.Status = OpStatus.OP_WE_NOT_FOUND;
                        opw.NullWE = we;
                    }
                } else {
                    string result = WebUtil.doPreURLSecurityCheck(this.SRoot, elem);
                    if (result != string.Empty) {
                        handleURLViolation(opw, result);
                        return;
                    }
                    // update the WebElement Real values with HTMLElement attributes
                    updateWERealValues(opw, we, elem);
                    try {
                        updateTimeStamp(elem, op);
                        if (op.OpType == OPERATION.INPUT) {
                            string input = getOpInputValue(op,true);
                            if (op.Input is Parameter) {
                                Parameter param = op.Input as Parameter;
                                if (param.Sensitive && !op.Element.isPassword) {
                                    input = string.Empty;
                                }
                            }
                            Log.println_eng("T = UI, input value = "+input);
                            WebUtil.textInput(elem, input);
                        } else if (op.OpType == OPERATION.CLICK) {
                            WebUtil.mouseClick(elem, op.Click);
                        }
                        if (opw.Status == OpStatus.OP_WE_NOT_FOUND) {
                            opw.Status = OpStatus.READY;
                        }
                    } catch (Exception e) {
                        opw.Status = OpStatus.EXE_ERROR;
                        Log.println_eng("T = UI, Execute Op in UI ERROR. e = "+e.StackTrace);
                    }
                }
            }
        }

        internal string getOpInputValue(Operation op, bool consume) {
            if (op==null || op.Input==null) {
                return string.Empty;
            }
            if (op.Input is Parameter) {
                Parameter param = op.Input as Parameter;

                if (param.RealValue != null) {
                    if (consume && param.Type == ParamType.SET) {
                        param.ConsumeSet = true;
                    }
                    return param.RealValue + "";
                } else {
                    return param.DesignValue + "";
                }
            }
            if (op.Input is WebElementAttribute) {
                WebElementAttribute wea = op.Input as WebElementAttribute;
                return wea.RValue;
            }
            if (op.Input is string) {
                return op.Input as string;
            }
            return string.Empty;
        }
        /// <summary>
        /// update WebElement real values with HTMLElement Attritures. 
        /// update the opw WECache. 
        /// </summary>
        /// <param name="opw"></param>
        /// <param name="we"></param>
        /// <param name="elem"></param>
        private void updateWERealValues(OpWrapper opw, WebElement we, IHTMLElement elem) {
            if (opw == null || we == null || elem == null) {
                return;
            }
            if (opw.CachedWE == null) {
                opw.CachedWE = new List<WebElement>();
            }
            if (!opw.CachedWE.Contains(we)) {
                WebUtil.updateWebElement(we, elem);
                if (we.IsRealElement) {
                    if (!opw.CachedWE.Contains(we)) {
                        opw.CachedWE.Add(we);
                    }
                }
            }            
        }
        /// <summary>
        /// update the WebElement's attribute real value with current browser content
        /// </summary>
        /// <param name="we"></param>
        private void updateConditionInput(WebElement we) {
            if (we != null) {
                Log.println_eng("T = UI,     *** update condition input , we = " + we.Name);
                WebUtil.updateWebElement(we, this.browser);
            }
        }

        private bool isStatusCode(int code) {
            return code == (int)ENGINE_STATUS.BREAK_POINT
                || code == (int)ENGINE_STATUS.RUNING
                || code == (int)ENGINE_STATUS.STOPED
                || code == (int)ENGINE_STATUS.INITIALIZING;
        }
        #endregion ui thread handling
        #region utils
        //TODO , temp code 
        internal string getEventName(int code) {
            if (code == DO_OPERATION) {
                return "Operation";
            }
            if (code == CON_INPUT_ATTRIBUTE) {
                return "CON_INPUT_ATTRIBUTE";
            }
            if (code == CON_INPUT_WEBELEMENT) {
                return "CON_INPUT_WEBELEMENT";
            }
            if (code == Logger.LOG_APP_MSG) {
                return "LOG_APP_MSG";
            }
            if (code == WE_EXISTED_TEST) {
                return "Check WE existed";
            }
            //if (code == REQ_TIMEOUT) {
            //    return "REQ_TIMEOUT";
            //}
            return "";
        }
        /// <summary>
        /// whether the currently handling operation is a process 
        /// </summary>
        /// <returns></returns>
        public bool isProcessHandling() {
            if (this.currentOpw != null && this.currentOpw.Op is Process) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// whether the engine thread is busy now 
        /// </summary>
        /// <returns></returns>
        public bool isBusy() {
            return worker.IsBusy;
        }
        /// <summary>
        /// update requestStartTime value, it will be used to in rules to check page loading timeout. 
        /// e.g how many times a page will be refreshed if loading with errors. 
        /// candidate element :
        /// 1. link
        /// 2. input - submit,button. 
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="op"></param>
        private void updateTimeStamp(IHTMLElement elem, Operation op) {
            string tag = elem.tagName.ToLower();
            bool flag = false;
            if ("a".Equals(tag)) {
                flag = true;
            } else if ("input".Equals(tag)) {
                string type = elem.getAttribute("type") + "";
                if (type.Length > 0 && type.ToLower().Equals("submit")) {
                    flag = true;
                }
            }
            if (flag == true) {
                ReqStartTime = DateTime.Now;
                //Status = ENGINE_STATUS.INITIALIZING;
            }
        }
        /// <summary>
        /// Update the process parameters' real value as the design value
        /// </summary>
        private void initProcParams(Process proc) {
            if (proc != null) {                
                doInitProcParams(proc.ParamPublic, true);
                doInitProcParams(proc.ParamPrivate, true);
            }
        }
        /// <summary>
        /// Clean the process parameters' real value, Reset the process parameters real value as null. 
        /// </summary>
        private void resetProcParams(Process proc) {
            if (proc != null) {
                doInitProcParams(proc.ParamPublic, false);
                doInitProcParams(proc.ParamPrivate, false);
            }
        }
        /// <summary>
        /// initialize the parameters real value with design value (if isInit==true), 
        /// or clean the real value as null if isInit==false
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="isInit"></param>
        private void doInitProcParams(ParamGroup grp, bool isInit) {
            if (grp != null) {
                foreach (ParamGroup pgrp in grp.SubGroups) {
                    doInitProcParams(pgrp, isInit);
                }
                foreach (Parameter param in grp.Params) {
                    ModelManager.Instance.initRealValue(param, isInit);
                }
            }
        } 
        
        /// <summary>
        /// Get the script root process helper or null if errros 
        /// </summary>
        /// <returns></returns>
        internal Process getRootProc() {
            if (procStack != null) {
                Process[] array = procStack.ToArray();
                Process proc = array[procStack.Count - 1];                
                if (proc.Collection == null) {
                    return proc;
                }
            }
            return null;
        }
        /// <summary>
        /// Get the current process helper or null if errros 
        /// </summary>
        /// <returns></returns>
        internal Process getCurrentProc() {
            if (procStack != null) {                
                return procStack.Peek();
            }
            return null;
        }
        /// <summary>
        /// Get the second process helper in the stack or null if errors 
        /// </summary>
        /// <returns></returns>
        internal Process getSecondProc() {
            if (procStack != null && procStack.Count>1) {
                Process[] array = procStack.ToArray();
                return array[1];
            }
            return null;
        }
        #endregion utils
    }
    /// <summary>
    /// this wrapper is used to conbine the operation/process with it executed status.
    /// </summary>
    internal class OpWrapper
    {
        private Operation op;
        /// <summary>
        /// real operation/process reference. 
        /// </summary>
        public Operation Op {
            get { return op; }
            set { op = value; }
        }
        private OpStatus status = OpStatus.READY;
        /// <summary>
        /// operation status, it will be updated after the op execution. initial value is READY. 
        /// if the status is READY, just execute the operation directly. 
        /// if status is other values, the operation execution flow will be handled by the applied rules
        /// either re-execute again or just ignore to the next operation. 
        /// </summary>
        public OpStatus Status {
            get { return status; }
            set { status = value; }
        }
        private WebElement nullWE = null;
        /// <summary>
        /// It will take effect if the status is OP_WE_NOT_FOUND||UPDATE_PARAM_WE_NOT_FOUND||OPC_PARAM_MAPPING_WE_NOT_FOUND
        /// </summary>
        public WebElement NullWE {
            get { return nullWE; }
            set { nullWE = value; }
        }
        private ParamCmd parmCmd = null;
        /// <summary>
        /// It will take effect if hte status is UPDATE_PARAM_WE_NOT_FOUND || UPDATE_PARAM_WE_FOUND 
        /// || OPC_PARAM_MAPPING_WE_NOT_FOUND || OPC_PARAM_MAPPING_WE_FOUND
        /// </summary>
        public ParamCmd ParamCmd {
            get { return parmCmd; }
            set { parmCmd = value; }
        }
        private List<OpCondition> nullWEOpcList = null;
        /// <summary>
        /// It will take effect if the status is CON_WE_NOT_FOUND
        /// </summary>
        public List<OpCondition> NullWEOpcList {
            get { return nullWEOpcList; }
            set { nullWEOpcList = value; }
        }
        private List<WebElement> opcNullWEList = null;
        /// <summary>
        /// It will take effect if the nullWEOpcList is not null, it is used to 
        /// record all the null WebElement of the first OpCondition in nullWeOpcList
        /// </summary>
        public List<WebElement> OpcNullWEList {
            get { return opcNullWEList; }
            set { opcNullWEList = value; }
        }
        private OpCondition opc = null;
        /// <summary>
        /// It will take effect if Status == OPC_PARAM_MAPPING_WE_FOUND
        /// </summary>
        public OpCondition Opc {
            get { return opc; }
            set { opc = value; }
        }

        private List<WebElement> cachedWE = null;
        /// <summary>
        /// This is used to cache the have-updated WebElement for the operation. 
        /// For a WebElement request, first it will check the cache web element, if not
        /// found, just check the UI thread to update and then, update the cache. 
        /// 
        /// When script flow logic changed, cache will be cleaned. 
        /// </summary>
        public List<WebElement> CachedWE {
            get { return cachedWE; }
            set { cachedWE = value; }
        }
        private List<Process> cleanedProcs = null;
        /// <summary>
        /// This is used only the Status == CLEAN_PROC_ENV_AND_GOTO, it will clean the 
        /// process from [0-->count-1], the first one should be the top process in the engine
        /// process status, or else it should be an error. 
        /// </summary>
        public List<Process> CleanedProcs {
            get { return cleanedProcs; }
            set { cleanedProcs = value; }
        }

        /// <summary>
        /// clean all cached WebElement Real value and reset other field as initial
        /// </summary>
        internal void reset() {
            if (CachedWE != null) {
                foreach (WebElement we in this.CachedWE) {
                    ModelManager.Instance.cleanWebElementAttributes(we);
                }
            }
            this.CachedWE = null;
            this.cleanedProcs = null;
            this.NullWE = null;
            this.NullWEOpcList = null;
            this.Op = null;
            this.Opc = null;
            this.OpcNullWEList = null;
            this.ParamCmd = null;
            this.Status = OpStatus.READY;
        }
    }
    /// <summary>
    /// operation status
    /// </summary>    
    internal enum OpStatus
    {
        // Operation is ready to execute
        READY,
        // Operaiton is ready to Stop
        STOP,
        // restart script as a new one 
        RESTART_SCRIPT,
        // This is only used to flag that current process is in and end status, the 
        // current op is End process, it will prepare for the parameter update and 
        // parameter mapping later in the OpConditions if have 
        PROC_END,
        // Operation operated WebElement not found 
        OP_WE_NOT_FOUND,
        // After operation finished, When update WebElementAttribute real value to target parameter value, Null WebElement
        // Note that the WebElementAttribute can be a src parmater value or an WebElementAttribute itself. 
        UPDATE_PARAM_WE_NOT_FOUND,
        // refer UPDATE_PARAM_WE_NOT_FOUND
        UPDATE_PARAM_WE_FOUND,
        // Condition input parameter value is an WebElement but can not be found 
        CON_WE_NOT_FOUND,
        // Condition input parameter value is an WebElement but can be found 
        CON_WE_FOUND,
        // It is only take effect if the opc's target op is a process 
        OPC_PARAM_MAPPING_WE_NOT_FOUND,
        OPC_PARAM_MAPPING_WE_FOUND,
        /// <summary>
        /// This is used for the goto action, change the current logic to another same level op/proc
        /// or parents' process level's operation/Process
        /// </summary>
        CLEAN_PROC_ENV_AND_GOTO, 
        // operation execution error
        EXE_ERROR,
        // Request timeout 
        REQ_TIME_OUT,
        /// <summary>
        /// No next operation found for the Non-End Operation or Process. It is because there is no proper OpCondition passed or 
        /// There is no OpCondition for the non-end op/proc.
        /// </summary> 
        NO_NEXT_OP_FOUND
    }
    public enum DEBUG_CMD
    {
        NONE,

        RUN,
        /// <summary>
        /// only available for a Process is current TOBE executed operation        
        /// </summary>
        STEP_INTO,
        /// <summary>
        /// step over current operation or process
        /// </summary>
        STEP_OVER,
        /// <summary>
        /// stop engine thread for debug 
        /// </summary>
        STOP
    }
    /// <summary>
    /// this will be valid in release mode
    /// </summary>
    public enum ENGINE_STATUS
    {
        /// <summary>
        /// engine is stoped, a default status of the engine. if first initial or totally run completed, 
        /// the status will be un-initialized. 
        /// </summary>
        STOPED,
        /// <summary>
        /// the engine is running
        /// </summary>
        RUNING,
        // only effective in debug mode, it means that the engine is paused at a break point
        // It is a control status, in such status, the debug panel can update the ui info. 
        BREAK_POINT,
        /// <summary>
        /// this means that the script has been run, but the browser page was loading. 
        /// INITIALIZING status, means that this operation just have been executed with 
        /// previous step, and current action is just to check whether the page loading 
        /// currently and then decide which rules will be applied. 
        /// </summary>
        INITIALIZING,
        ///// <summary>
        ///// used for debug internal control 
        ///// </summary>
        //INVALID
    }
}
