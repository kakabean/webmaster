using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.lib.engine
{
    public class Logger
    {
        #region constants 
        /// <summary>
        /// log system messages, e.g exceptions 
        /// </summary>
        public static readonly int LOG_SYS_MSG = 0x40;
        /// <summary>
        /// notify the UI thread to log application messages. 
        /// </summary>
        public static readonly int LOG_APP_MSG = 0x41;
        public static readonly int LOG_APP_MSG_OP = 0x4100;
        public static readonly int LOG_APP_MSG_PROC = 0x4101;
        public static readonly int LOG_APP_MSG_OPC = 0x4102;
        public static readonly int LOG_APP_MSG_CON = 0x4103;
        public static readonly int LOG_APP_MSG_CONGRP = 0x4104;
        public static readonly int LOG_APP_MSG_OP_PARAM_UPD = 0x4105;
        public static readonly int LOG_APP_MSG_OPC_PARAM_UPD = 0x4106;
        public static readonly int LOG_APP_MSG_PARAM_CMD = 0x4107 ;
        public static readonly int LOG_APP_MSG_RULE = 0x4108;
        /// <summary>
        /// log user messages, this messages will be defined by the script developer 
        /// </summary>
        public static readonly int LOG_USER_MSG = 0x4200;

        public static readonly int LOG_PROC_START = 0;
        public static readonly int LOG_PROC_END = 1;

        #endregion constants 
        #region variables         
        private WebEngine engine = null;
        /// <summary>
        /// Engine model
        /// </summary>
        int engMode = WebEngine.MODE_RELEASE;
        /// <summary>
        /// log level, default is just log the script developer's log info.
        /// </summary>
        private int logLevel = LOG_USER_MSG;
        /// <summary>
        /// log level, default is just log the script developer's log info.
        /// </summary>
        public int LogLevel {
            get { return logLevel; }
            set { logLevel = value; }
        }
        /// <summary>
        /// msg to record the debug logs for each operation and OpConditions, Rules and what else.
        /// when each msg showed in the UI thread, msg should be cleaned for next use
        /// </summary>
        private StringBuilder dbgMsg = new StringBuilder();

        public StringBuilder DbgMsg {
            get { return dbgMsg; }
            set { dbgMsg = value; }
        }

        /// <summary>
        /// msg to record the release logs for each operation, 
        /// when each msg showed in the UI thread, msg should be cleaned for next use
        /// </summary>
        private StringBuilder releaseMsg = new StringBuilder();

        public StringBuilder ReleaseMsg {
            get { return releaseMsg; }
            set { releaseMsg = value; }
        }
        #endregion variables 
        public Logger(WebEngine engine, int mode) {
            this.engine = engine;
            engMode = mode;
        }
        #region log handling
        /// <summary>
        /// update the log msg for debug or release mode. 
        /// </summary>
        /// <param name="str"></param>
        internal void buildLogMsg(string str) {
            if (engMode == WebEngine.MODE_DEBUG) {
                dbgMsg.Append(str);
                releaseMsg.Append(str);
            } else {
                releaseMsg.Append(str);
            }
        }
        /// <summary>
        /// Log operation executing message 
        /// </summary>
        /// <param name="op">operation, not process</param>
        internal void logOpMsg(Operation op) {
            if (logLevel == LOG_APP_MSG) {
                logOpMsg4APP(op);
                // filter start node 
                if (op.OpType != OPERATION.START) {
                    logOpMsg4USER(op);
                }
            } else if (logLevel == LOG_USER_MSG) {
                // filter start node 
                if (op.OpType != OPERATION.START) {
                    logOpMsg4USER(op);
                }
            } else if (logLevel == LOG_SYS_MSG) {
                //TODO ???
            }
        }
        internal void logOpMsg4APP(Operation op) {
            // process normal operation log 
            string prefix = WebUtil.getAppLogPrefix(LOG_APP_MSG_OP);
            string name = Constants.STR_START_FLAG + Constants.LOG_COLOR_OP + Constants.STR_END_FLAG + op.Name + Constants.STR_SEPERATOR;
            dbgMsg.Append(prefix).Append(name);
            if (op.Element != null) {
                string opText1 = LangUtil.getMsg("log.op.text1");
                string elemName = Constants.STR_START_FLAG + Constants.LOG_COLOR_WE + Constants.STR_END_FLAG + op.Element.Name + Constants.STR_SEPERATOR;
                dbgMsg.Append("\", " + opText1 + "=\"").Append(elemName).Append("\"");
                if (op.OpType == OPERATION.INPUT) {
                    string opText3 = LangUtil.getMsg("log.op.text3");
                    string input = engine.getOpInputValue(op, false);
                    string inputStr = Constants.STR_START_FLAG + Constants.LOG_COLOR_OP_INPUT + Constants.STR_END_FLAG + input + Constants.STR_SEPERATOR;
                    dbgMsg.Append(", ").Append(opText3).Append(" = \"").Append(inputStr).Append("\"");
                }
            } else {
                string opText2 = LangUtil.getMsg("log.op.text2");
                if (op.OpType == OPERATION.OPEN_URL_N_T || op.OpType == OPERATION.OPEN_URL_T) {
                    string opText3 = LangUtil.getMsg("log.op.text3");
                    string input = engine.getOpInputValue(op, false);
                    string inputStr = Constants.STR_START_FLAG + Constants.LOG_COLOR_OP_INPUT + Constants.STR_END_FLAG + input + Constants.STR_SEPERATOR;
                    dbgMsg.Append("\", ").Append(opText3).Append(" = \"").Append(inputStr).Append("\"");
                } else {
                    dbgMsg.Append("\", " + opText2);
                }
            }
            string opw_status = LangUtil.getMsg("log.opw.status.label");
            dbgMsg.Append(", " + opw_status + " = " + WebUtil.getStatusText(engine.CurrentOpw.Status));
            Log.println_eng("T = ENGINE, - B1<<< - op = " + op.Name + ", before log op message,                    evt = " + engine.getEventName(LOG_APP_MSG));
            //Log.println_eng("UI INFO : " + dbgMsg);
            engine.logScriptMsg(LOG_APP_MSG,WebEngine.SYNC_ENGINE_PRE_LOCK);            
            Log.println_eng("T = ENGINE, - B1>>> - op = " + op.Name + ", after log op message,                     evt = " + engine.getEventName(LOG_APP_MSG));
        }

        internal void logOpMsg4USER(Operation op) {
            if (op != null) {
                UserLog log = op.LogEnd;
                if (log == null) {
                    return;
                }

                string sb = getUserLog(log);
                if (sb == string.Empty) {
                    return;
                }

                if (releaseMsg.Length > 0) {
                    releaseMsg.Insert(0, "\n");
                }
                releaseMsg.Insert(0, sb);
                Log.println_eng("T = ENGINE, - Op User Log <<< before, op = " + op.Name);
                //Log.println_eng("UI INFO : " + releaseMsg);
                engine.logScriptMsg(LOG_USER_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
                Log.println_eng("T = ENGINE, - Op User Log >>> after , op = " + op.Name);
            }
        }
        /// <summary>
        /// Log process start and finished message info 
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="mode">LOG_PROC_START, LOG_PROC_END</param>
        internal void logProcMsg(Process proc, int mode) {
            if (logLevel == LOG_APP_MSG) {
                logProcMsg4APP(proc, mode);
                logProcMsg4USER(proc, mode);
            } else if (logLevel == LOG_USER_MSG) {
                logProcMsg4USER(proc, mode);
            } else if (logLevel == LOG_SYS_MSG) {
                //TODO ???
            }
        }
        private void logProcMsg4APP(Process proc, int mode) {
            string txt = mode == LOG_PROC_START ? "log.proc.text1" : "log.proc.text2";
            string prefix = WebUtil.getAppLogPrefix(LOG_APP_MSG_PROC);
            StringBuilder sb = new StringBuilder();
            string flag = LangUtil.getMsg(txt);
            string procName = Constants.STR_START_FLAG + Constants.LOG_COLOR_PROC + Constants.STR_END_FLAG + proc.Name + Constants.STR_SEPERATOR;
            sb.Append(prefix).Append(procName).Append("\", ").Append(flag);

            if (dbgMsg.Length > 0) {
                dbgMsg.Insert(0, "\n");
            }
            dbgMsg.Insert(0, sb);
            sb = null;
            Log.println_eng("T = ENGINE, - B3<<< before log proc msg, proc = " + proc.Name + ", mode = " + flag);
            //Log.println_eng("UI INFO : " + dbgMsg);
            engine.logScriptMsg(LOG_APP_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
            Log.println_eng("T = ENGINE, - B3>>> after Log proc msg, proc = " + proc.Name + ", mode = " + txt);
        }
        private void logProcMsg4USER(Process proc, int mode) {
            if (proc != null) {
                UserLog log = proc.LogEnd;
                string txt = "End";
                if (mode == LOG_PROC_START) {
                    log = proc.LogStart;
                    txt = "Start";
                }

                if (log == null) {
                    return;
                }

                string sb = getUserLog(log);
                if (sb == string.Empty) {
                    return;
                }

                if (releaseMsg.Length > 0) {
                    releaseMsg.Insert(0, "\n");
                }
                releaseMsg.Insert(0, sb);
                Log.println_eng("T = ENGINE, - Proc User " + txt + " Log <<< before, proc = " + proc.Name);
                //Log.println_eng("UI INFO : " + releaseMsg);
                engine.logScriptMsg(LOG_USER_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
                Log.println_eng("T = ENGINE, - Proc User " + txt + " Log >>> after , proc = " + proc.Name);
            }
        }
        /// <summary>
        /// log the OpCondition msg in debug mode 
        /// </summary>
        /// <param name="opc"></param>
        /// <param name="result"></param>
        /// <param name="sb_sysLog">It is used to format the opc log info</param>
        internal void logOpcMsg(OpCondition opc, bool result, StringBuilder sb_sysLog) {
            // If it is the last link is to the process end node, just ignore the opc user message 
            if (opc.Op != null && opc.Op.OpType == OPERATION.END) {
                logOpcMsg4APP(opc, result, sb_sysLog);
                return;
            }
            if (logLevel == LOG_APP_MSG) {
                logOpcMsg4APP(opc, result, sb_sysLog);
                logOpcMsg4USER(opc);
            } else if (logLevel == LOG_USER_MSG) {
                logOpcMsg4USER(opc);
            } else if (logLevel == LOG_SYS_MSG) {
                //TODO ???
            }
        }
        private void logOpcMsg4APP(OpCondition opc, bool result, StringBuilder sb_sysLog) {
            string prefix = WebUtil.getAppLogPrefix(LOG_APP_MSG_OPC);
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            string strr = LangUtil.getMsg("log.opc.text1");
            int resultColor = result ? Constants.LOG_COLOR_RESULT_TRUE : Constants.LOG_COLOR_RESULT_FALSE;
            string opcName = Constants.STR_START_FLAG + Constants.LOG_COLOR_OPC + Constants.STR_END_FLAG + opc.Name + Constants.STR_SEPERATOR;
            string opcResult = Constants.STR_START_FLAG + resultColor + Constants.STR_END_FLAG + WebUtil.getBoolText(result) + Constants.STR_SEPERATOR;
            sb.Append(prefix).Append(opcName).Append("\", " + strr + " = \"").Append(opcResult).Append("\"");
            sb1.Append(prefix).Append(opc.Name).Append("\", " + strr + " = \"").Append(WebUtil.getBoolText(result)).Append("\"");
            dbgMsg.Insert(0, sb);
            sb_sysLog.Insert(0, sb1);
            sb1 = null;
            sb = null;

            Log.println_eng("T = ENGINE,   - <<< before opc log, \n" + sb_sysLog.ToString());
            //Log.println_eng("UI INFO : " + dbgMsg);
            Operation owner = ModelManager.Instance.getOwnerOp(opc);
            // If it is a process, just left the msg in cache, and it will be handled when do process exit.
            if (!(owner is Process)) {
                engine.logScriptMsg(LOG_APP_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
            }            
            Log.println_eng("T = ENGINE,   - >>> after opc log,   |  opc = " + opc.Name + ", Next = " + opc.Op.Name + ", Type=" + opc.Op.GetType().Name + ", opType=" + opc.Op.OpType.ToString() + ", Result = " + result);
        }
        private void logOpcMsg4USER(OpCondition opc) {
            // NO user log for OpCondition
        }
        internal void logUpdateConGroup(ConditionGroup cgrp, int logLevel, StringBuilder sb_sysLog) {
            if (logLevel != LOG_APP_MSG) {
                return;
            }
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            sb.Append(WebUtil.getAppMsgPrefix());
            for (int i = 0; i < logLevel; i++) {
                sb.Append("  ");
            }
            sb1.Append(sb);
            sb.Insert(0, "\n");
            bool gr = cgrp.Result;
            string result = WebUtil.getBoolText(gr);
            if (false == cgrp.IsChecked) {
                result = LangUtil.getMsg("log.congrp.text2");
            }
            string congrpText1 = LangUtil.getMsg("log.congrp.text1");
            string text2 = LangUtil.getMsg("log.opc.text1");
            int resultColor = gr ? Constants.LOG_COLOR_RESULT_TRUE : Constants.LOG_COLOR_RESULT_FALSE;
            string cgrpName = Constants.STR_START_FLAG + Constants.LOG_COLOR_CONGRP + Constants.STR_END_FLAG + cgrp.Name + Constants.STR_SEPERATOR;
            string resultTxt = Constants.STR_START_FLAG + resultColor + Constants.STR_END_FLAG + result + Constants.STR_SEPERATOR;
            sb.Append("+").Append(congrpText1).Append(" = \"").Append(cgrpName).Append("\", ").Append(text2).Append(" = \"").Append(resultTxt).Append("\"");
            sb1.Append("+").Append(congrpText1).Append(" = \"").Append(cgrp.Name).Append("\", ").Append(text2).Append(" = \"").Append(result).Append("\"");
            dbgMsg.Insert(0, sb.ToString());
            // update syslog info             
            sb_sysLog.Insert(0, sb1.ToString());
        }
        /// <summary>
        /// just log when in debug mode, log condition input and result 
        /// </summary>
        /// <param name="con"></param>
        /// <param name="conLogLevel"></param>
        /// <param name="sb_sysLog">It is used to format the opc log info</param>
        internal void logUpdateCondition(Condition con, int conLogLevel, StringBuilder sb_sysLog) {
            if (this.LogLevel != LOG_APP_MSG) {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(WebUtil.getAppMsgPrefix());
            for (int i = 0; i < conLogLevel; i++) {
                sb.Append("  ");
            }
            bool cr = con.Result;
            string result = WebUtil.getBoolText(cr);
            if (false == con.IsChecked) {
                result = LangUtil.getMsg("log.congrp.text2");
            }
            string deck = sb.ToString() + "  ";
            string conText1 = LangUtil.getMsg("log.con.text1");
            string text2 = LangUtil.getMsg("log.opc.text1");
            int resultColor = cr ? Constants.LOG_COLOR_RESULT_TRUE : Constants.LOG_COLOR_RESULT_FALSE;
            string conName = Constants.STR_START_FLAG + Constants.LOG_COLOR_CON + Constants.STR_END_FLAG + con.Name + Constants.STR_SEPERATOR;
            string resultTxt = Constants.STR_START_FLAG + resultColor + Constants.STR_END_FLAG + result + Constants.STR_SEPERATOR;
            dbgMsg.Append("\n  ").Append(deck).Append("# " + conText1).Append(" =\"").Append(conName).Append("\", ").Append(text2).Append(" = \"").Append(resultTxt).Append("\"");
            sb_sysLog.Append("\n  ").Append(deck).Append("# " + conText1).Append(" =\"").Append(con.Name).Append("\", ").Append(text2).Append(" = \"").Append(result).Append("\"");

            logConditionInput(con.Input1, deck + " ", sb_sysLog);
            if (con.Input2 != null) {
                logConditionInput(con.Input2, deck + " ", sb_sysLog);
            }
        }
        /// <summary>
        /// log condition messages, just effective in debug mode 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="deck">how many spaces before line msg</param>
        private void logConditionInput(object input, String deck, StringBuilder sb_sysLog) {
            if (engMode != WebEngine.MODE_DEBUG) {
                return;
            }
            string delta = "      ->";
            if (input is WebElement) {
                WebElement we = input as WebElement;
                string text = LangUtil.getMsg("log.con.we.text");
                string text1 = LangUtil.getMsg("log.con.text3");
                if (we.IsRealElement) {
                    text1 = LangUtil.getMsg("log.con.text2");
                }
                string weName = Constants.STR_START_FLAG + Constants.LOG_COLOR_WE + Constants.STR_END_FLAG + we.Name + Constants.STR_SEPERATOR;
                dbgMsg.Append("\n").Append(deck).Append(delta + text).Append(" = \"").Append(weName).Append("\", ").Append(text1);
                sb_sysLog.Append("\n").Append(deck).Append(delta + text).Append(" = \"").Append(we.Name).Append("\", ").Append(text1);
            } else if (input is WebElementAttribute) {
                WebElementAttribute wea = input as WebElementAttribute;
                string text = LangUtil.getMsg("log.con.wea.text");
                string text1 = LangUtil.getMsg("log.con.wea.vtext");
                string value = wea.RValue;
                if (value != null && value.Length > Constants.LOG_MAX_CON_INPUT_LENGTH) {
                    value = value.Substring(0, Constants.LOG_MAX_CON_INPUT_LENGTH);
                    value = value + " ... ";
                }
                string weaName = Constants.STR_START_FLAG + Constants.LOG_COLOR_WEA + Constants.STR_END_FLAG + wea.Name + Constants.STR_SEPERATOR;
                string valueTxt = Constants.STR_START_FLAG + Constants.LOG_COLOR_VALUE + Constants.STR_END_FLAG + value + Constants.STR_SEPERATOR;
                dbgMsg.Append("\n").Append(deck).Append(delta + text + " = \"").Append(weaName).Append("\", " + text1 + " = \"").Append(valueTxt).Append("\"");
                sb_sysLog.Append("\n").Append(deck).Append(delta + text + " = \"").Append(wea.Name).Append("\", " + text1 + " = \"").Append(value).Append("\"");
            } else if (input is Parameter) {
                Parameter p = input as Parameter;
                string text = LangUtil.getMsg("log.con.param.text");
                string text1 = LangUtil.getMsg("log.con.param.pvtext");
                string value = p.ToString();
                if (value != null && value.Length > Constants.LOG_MAX_CON_INPUT_LENGTH) {
                    value = value.Substring(0, Constants.LOG_MAX_CON_INPUT_LENGTH);
                    value = value + " ... ";
                }
                string pName = Constants.STR_START_FLAG + Constants.LOG_COLOR_PARAM + Constants.STR_END_FLAG + p.Name + Constants.STR_SEPERATOR;
                string valueTxt = Constants.STR_START_FLAG + Constants.LOG_COLOR_VALUE + Constants.STR_END_FLAG + value + Constants.STR_SEPERATOR;
                dbgMsg.Append("\n").Append(deck).Append(delta + text + " = \"").Append(pName).Append("\", " + text1 + " = \"").Append(valueTxt + "\"");
                sb_sysLog.Append("\n").Append(deck).Append(delta + text + " = \"").Append(p.Name).Append("\", " + text1 + " = \"").Append(value + "\"");
            } else if (input is String) {
                string value = input as String;
                if (value != null && value.Length > Constants.LOG_MAX_CON_INPUT_LENGTH) {
                    value = value.Substring(0, Constants.LOG_MAX_CON_INPUT_LENGTH);
                    value = value + " ... ";
                }
                string text = LangUtil.getMsg("log.con.str.text");
                string text1 = LangUtil.getMsg("log.con.str.pvtext");
                string valueTxt = Constants.STR_START_FLAG + Constants.LOG_COLOR_VALUE + Constants.STR_END_FLAG + value + Constants.STR_SEPERATOR;
                dbgMsg.Append("\n").Append(deck).Append(delta + text + ", ").Append(text1 + " = \"").Append(valueTxt + "\"");
                sb_sysLog.Append("\n").Append(deck).Append(delta + text + ", ").Append(text1 + " = \"").Append(value + "\"");
            }
        }
        /// <summary>
        /// get the runtime real log info, that will be show to the end user, or return string.Empty if errors.
        /// in the real user log, it use "Constants.STR_START_FLAG + Constants.LOG_COLOR_VALUE + Constants.STR_END_FLAG + value + Constants.STR_SEPERATOR" format to record one log item.
        /// 
        /// </summary>
        /// <param name="userLog"></param>
        /// <returns></returns>
        private string getUserLog(UserLog userLog) {
            StringBuilder sb = new StringBuilder();
            if (userLog != null) {
                foreach (UserLogItem item in userLog.LogItems) {
                    object o = item.Item;
                    if (o is DateTime) {
                        int argb = ModelManager.Instance.getLogColorArgb(item.Color, Constants.LOG_USER_KEY_TIME, engine.SRoot);
                        buildTimeText(sb, argb);
                    } else if (o is WebElementAttribute) {
                        int argb = ModelManager.Instance.getLogColorArgb(item.Color, Constants.LOG_USER_KEY_WEA, engine.SRoot);
                        buildWEAText(o as WebElementAttribute, sb, argb);
                    } else if (o is Parameter) {
                        int argb = ModelManager.Instance.getLogColorArgb(item.Color, Constants.LOG_USER_KEY_PARAM, engine.SRoot);
                        Parameter param = o as Parameter;
                        if (param.Sensitive) {
                            buildStringText(Constants.PARAM_SENS_ERR_MSG, sb, argb);
                        } else {
                            buildParamText(param, sb, argb);
                        }
                    } else if (o is string) {
                        int argb = ModelManager.Instance.getLogColorArgb(item.Color, Constants.LOG_USER_KEY_STR, engine.SRoot);
                        buildStringText(o.ToString(), sb, argb);
                    }
                }
                return sb.ToString();
            }
            return string.Empty;
        }
        private void buildTimeText(StringBuilder sb, int argb) {
            string time = getUserLogTime();
            sb.Append(Constants.STR_START_FLAG).Append(argb).Append(Constants.STR_END_FLAG).Append(time).Append(Constants.STR_SEPERATOR);
        }
        private void buildStringText(string p, StringBuilder sb, int argb) {
            if (p != null && p.Length > 0) {
                sb.Append(Constants.STR_START_FLAG).Append(argb).Append(Constants.STR_END_FLAG).Append(p).Append(Constants.STR_SEPERATOR);
            }
        }
        private void buildParamText(Parameter parameter, StringBuilder sb, int argb) {
            if (parameter != null) {
                sb.Append(Constants.STR_START_FLAG).Append(argb).Append(Constants.STR_END_FLAG).Append(parameter.ToString()).Append(Constants.STR_SEPERATOR);
            }
        }
        private void buildWEAText(WebElementAttribute wea, StringBuilder sb, int argb) {
            if (wea != null) {
                sb.Append(Constants.STR_START_FLAG).Append(argb).Append(Constants.STR_END_FLAG).Append(wea.RValue).Append(Constants.STR_SEPERATOR);
            }
        }
        
        internal void logOpcMsg4APP_ParamUpdate(StringBuilder sb_sysLog){
            //string prefix = WebUtil.getAppLogPrefix(Logger.LOG_APP_MSG_OPC_PARAM_UPD) + "\n";
            //sb_sysLog.Insert(0, prefix);
            dbgMsg.Append(sb_sysLog);
            sb_sysLog = null;

            Log.println_eng("T = ENGINE,   - Op update param log, \n" + sb_sysLog.ToString());
            //Log.println_eng("UI INFO : " + dbgMsg);
            engine.logScriptMsg(LOG_APP_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
        }
        internal void logOpMsg4APP_ParamUpdate(StringBuilder sb_sysLog) {
            //string prefix = WebUtil.getAppLogPrefix(Logger.LOG_APP_MSG_OP_PARAM_UPD) + "\n";
            //sb_sysLog.Insert(0, prefix);
            dbgMsg.Append(sb_sysLog);
            Log.println_eng("T = ENGINE,   - Op update param log, \n" + sb_sysLog.ToString());
            sb_sysLog = null;
            //Log.println_eng("UI INFO : " + dbgMsg);
            engine.logScriptMsg(LOG_APP_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
        }        
        /// <summary>
        /// get the user log time, it maybe the local machine time or 
        /// server time, it depend on the script root setting.
        /// </summary>
        /// <returns></returns>
        private string getUserLogTime() {
            //TODO currently just use the local full time, it should be enhanced later 
            return DateTime.Now.ToString();
        }
        #endregion log handling
        /// <summary>
        /// Clean msg cache
        /// </summary>
        internal void cleanMsgCache() {
            this.dbgMsg.Remove(0, dbgMsg.Length);
            this.releaseMsg.Remove(0, dbgMsg.Length);
        }

        internal void logParamCmdWENotFound(StringBuilder sb_sysLog, WebElement we) {
            string name = null;
            if (we != null) {
                name = we.Name;
            }
            string prefix = WebUtil.getAppLogPrefix(Logger.LOG_APP_MSG_PARAM_CMD);
            name = Constants.STR_START_FLAG + Constants.LOG_COLOR_WE + Constants.STR_END_FLAG + name + Constants.STR_SEPERATOR;
            string msg1 = LangUtil.getMsg("log.mapping.err.text1",name);// Error ! Web Element {0} can not be found. 
            sb_sysLog.Append(prefix+msg1+"\n");
        }

        internal void logParamCmdExeBefore(ParamCmd update, StringBuilder sb_sysLog) {
            string prefix = WebUtil.getAppLogPrefix(Logger.LOG_APP_MSG_PARAM_CMD);
            string cmdTxt = string.Empty;
            if(update == null || sb_sysLog == null){
                return ;
            }
            string srcTxt = ModelManager.Instance.getMappingSrcText(update.Src);
            string targetTxt = Constants.STR_START_FLAG + Constants.LOG_COLOR_PARAM + Constants.STR_END_FLAG + update.Target.Name + Constants.STR_SEPERATOR
                + " = " + Constants.STR_START_FLAG + Constants.LOG_COLOR_VALUE + Constants.STR_END_FLAG + update.Target.RealValue + Constants.STR_SEPERATOR;
                
            if (update.Cmd == PARAM_CMD.ASSIGN) {                
                cmdTxt = LangUtil.getMsg("log.cmd.assign.text"); // Assign
            }
            cmdTxt += LangUtil.getMsg("log.cmd.before.text", srcTxt, targetTxt); // , before execute, src = {0}, target = {1}, 
            sb_sysLog.Append(prefix + cmdTxt);
        }

        internal void logParamCmdExeEnd(ParamCmd update, StringBuilder sb_sysLog) {
            if (update == null || sb_sysLog == null) {
                return;
            }
            string targetTxt = Constants.STR_START_FLAG + Constants.LOG_COLOR_PARAM + Constants.STR_END_FLAG + update.Target.Name + Constants.STR_SEPERATOR
                + " = " + Constants.STR_START_FLAG + Constants.LOG_COLOR_VALUE + Constants.STR_END_FLAG + update.Target.RealValue + Constants.STR_SEPERATOR;
            string txt = LangUtil.getMsg("log.cmd.after.text", targetTxt);  // after execute { {0} }
            sb_sysLog.Append(txt);
        }
        /// <summary>
        /// It is only take effect if the LogLevel == LOG_APP_MSG, it is log the End node if it has ParamCmd. 
        /// </summary>
        /// <param name="op"></param>
        internal void logOpEndIfNeed(Operation op) {
            if (/*this.LogLevel == LOG_APP_MSG && */op.OpType == OPERATION.END) {
                logOpMsg(op);
            }
        }

        internal void logScriptEnd(string scriptName) {
            // Handle script execution finished 
            string finish = LangUtil.getMsg("eng.run.status.finish.text",scriptName);
            string str = "\n" + finish+"\n";
            buildLogMsg(str);

            if (engMode == WebEngine.MODE_DEBUG) {
                engine.logScriptMsg(Logger.LOG_APP_MSG, WebEngine.SYNC_ENGINE_TIMEOUT);
                engine.logScriptMsg(Logger.LOG_USER_MSG, WebEngine.SYNC_ENGINE_TIMEOUT);
            }else{
                engine.logScriptMsg(Logger.LOG_USER_MSG, WebEngine.SYNC_ENGINE_TIMEOUT);
            }            
        }
        
        internal void logScriptStart(string scriptName) {
            // Handle script execution started 
            string start = LangUtil.getMsg("eng.run.status.start.text",scriptName);
            string str = "\n" + start+"\n";
            buildLogMsg(str);

            if (engMode == WebEngine.MODE_DEBUG) {
                engine.logScriptMsg(Logger.LOG_APP_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
                engine.logScriptMsg(Logger.LOG_USER_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
            } else {
                engine.logScriptMsg(Logger.LOG_USER_MSG, WebEngine.SYNC_ENGINE_PRE_LOCK);
            }            
        }
    }
}
