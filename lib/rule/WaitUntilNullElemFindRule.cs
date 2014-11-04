using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;
using WebMaster.lib;
using System.Threading;

namespace WebMaster.lib.rule
{
    /// <summary>
    /// This rule is used to handle the Null WebElement scenario, it maybe Operaiton operated element, or Parameter value
    /// is a WebElementAttribute. 
    /// 
    /// Scenario: are seperated by the parameters 
    /// 1. Wait until Element find.    
    /// 2. Wait until Element find or timeout. If time out, stop engine.
    ///    [Time]
    /// 3. Wait until Element find or timeout. if time out, goto another pair operation. 
    ///    [Time][rule next operation]
    /// 
    /// </summary>
    class WaitUntilNullElemFindRule
    {
        private static int timeout = Constants.WE_CHECK_TIMEOUT;
        private static OpWrapper opWrapper = null;
        private static Operation ruleNextOp = null;
        private static WebEngine engine = null;

        private static void setInput(ListRef ruleParams, OpWrapper opw, WebEngine eng) {
            if (ruleParams == null || opw == null || eng == null
                || !((opw.NullWE != null || (opw.OpcNullWEList!=null && opw.OpcNullWEList.Count>0)) ) ) {
                Log.println_eng("T = ENGINE, ERROR, Rule WaitUntilNullElemFindRule, set input invalid parameters . ");
                return; 
            }
            opWrapper = opw;
            engine = eng;
            ruleNextOp = null;
            timeout = Constants.WE_CHECK_TIMEOUT;
            if(ruleParams.Count == 0) {
                //timeout = int.MaxValue;
            } else if (ruleParams.Count == 1) {
                timeout = getTimeout(ruleParams.get(0));
            } else if (ruleParams.Count == 2) {
                timeout = getTimeout(ruleParams.get(0));
                ruleNextOp = ruleParams.get(1) as Operation;
                // validation ruleNextOp and opw.Op is the same level
                Process pproc1 = ModelManager.Instance.getOwnerProc(ruleNextOp);
                Process pproc2 = ModelManager.Instance.getOwnerProc(opw.Op);
                if (pproc1 == null || pproc1!=pproc2) {
                    Log.println_eng("T = ENGINE, ERROR, rule next Op = "+ruleNextOp.Name+" should has the same value with current Op = "+opw.Op.Name);
                    ruleNextOp = null;
                }
            }
        }

        private static int getTimeout(object timeobj) {
            int time = Constants.WE_CHECK_TIMEOUT;
            if (timeobj != null) {
                try {
                    time = Convert.ToInt32(timeobj);
                } catch (Exception) {
                    Log.println_eng("ERROR - RULE : WaitUntilNullElemFindRule, first parameter should be a number. Timeout = "+timeobj);
                }
            }
            return time;
        }

        public static bool execute(ListRef ruleParams, OpWrapper opw, WebEngine eng) {
            setInput(ruleParams, opw, eng);

            List<WebElement> nullwelist = new List<WebElement>();
            if (opw.Status == OpStatus.OP_WE_NOT_FOUND || opw.Status == OpStatus.UPDATE_PARAM_WE_NOT_FOUND || opw.Status == OpStatus.OPC_PARAM_MAPPING_WE_NOT_FOUND) {
                nullwelist.Add(opw.NullWE);
            } else if (opw.Status == OpStatus.CON_WE_NOT_FOUND) {
                foreach (WebElement twe in opw.OpcNullWEList) {
                    nullwelist.Add(twe);
                }                
            }
            if (nullwelist.Count == 0) {
                throw new RuleException("Fetal Error, WaitUntilNullElemFindRule execution error, WE == null");
            }

            bool result = false;
            foreach (WebElement we in nullwelist) {
                WebElement twe = ModelManager.Instance.tryGetRealWE(we, opw, engine,timeout);
                if (twe != null && twe.IsRealElement) {
                    result = true;
                    break;
                }
            }
            
            // if rule check failed, update the next operation. 
            if (false == result) {
                if (opw.Status == OpStatus.OP_WE_NOT_FOUND || opw.Status == OpStatus.UPDATE_PARAM_WE_NOT_FOUND
                    || opw.Status == OpStatus.OPC_PARAM_MAPPING_WE_NOT_FOUND) {
                    engine.updateStepOverWhenJump();
                    opWrapper.Op = ruleNextOp;
                    opWrapper.Status = OpStatus.READY;
                    engine.resetOpw(opWrapper);
                } else if (opw.Status == OpStatus.CON_WE_NOT_FOUND) {
                    opw.NullWEOpcList.RemoveAt(0);
                    if (opw.NullWEOpcList.Count == 0) {
                        engine.updateStepOverWhenJump();
                        opWrapper.Op = ruleNextOp;
                        opWrapper.Status = OpStatus.READY;
                        engine.resetOpw(opWrapper);
                    }
                }
            } else {
                opWrapper.NullWE = null;
                if (opWrapper.Status == OpStatus.OPC_PARAM_MAPPING_WE_NOT_FOUND) {
                    opWrapper.Status = OpStatus.OPC_PARAM_MAPPING_WE_FOUND;
                }
                if (opWrapper.Status == OpStatus.UPDATE_PARAM_WE_NOT_FOUND) {
                    opWrapper.Status = OpStatus.UPDATE_PARAM_WE_FOUND;
                } else if (opWrapper.Status == OpStatus.CON_WE_NOT_FOUND) {
                    opWrapper.Status = OpStatus.CON_WE_FOUND;
                } else if (opWrapper.Status == OpStatus.OP_WE_NOT_FOUND) {
                    opWrapper.Status = OpStatus.READY;
                }
            }
            Log.println_eng("T = ENGINE, >>> R execute WaitUntilNullElemFindRule, start, timeout = " + timeout + ", next op = " + ruleNextOp+", result = "+result);
            return result;
        }
        /// <summary>
        /// get the action description or string.emtpty if errors 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string getDescription(OperationRule rule) {
            string text = string.Empty;
            if (rule != null) {
                object timeout = Constants.WE_CHECK_TIMEOUT;
                if (rule.Params.Count == 1) {
                    timeout = rule.Params.get(0);
                    string tv = getTimeout(timeout)+"";
                    text = LangUtil.getMsg("Rule.WaitUntilNullElemFindRule.P1",tv); // Wait a timeout = {0} ms, if WebElement is null, stop script
                } else if (rule.Params.Count == 2) {
                    timeout = rule.Params.get(0);
                    string tv = getTimeout(timeout) + "";
                    Operation op = rule.Params.get(1) as Operation;
                    string opn = "Invalid parameter";
                    if (op != null) {
                        opn = op.Name;
                    }
                    text = LangUtil.getMsg("Rule.WaitUntilNullElemFindRule.P2",tv,opn); // Wait a timeout = {0} ms, if WebElement is null, goto operation = {1}                    
                }
            }
            return text;
        }
        /// <summary>
        /// Get the Rule action's validation msg, return msg string with local if errros or return string.Empty if valid.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string getValidMsg(OperationRule rule) {
            if (rule != null && rule.Action == RuleAction.WaitUntilElemFind) {                
                Operation op = ModelManager.Instance.getOwnerOp(rule);
                string prefix = LangUtil.getMsg("model.Op.Name");
                if (op is Process) {
                    prefix = LangUtil.getMsg("model.Proc.Name");
                }
                string name = op == null ? "" : op.Name;                
                prefix += " = "+name+" - ";
                
                if (rule.Params.Count >= 1) {
                    // this parameter must be timeout 
                    decimal dec = ModelManager.Instance.getDecimal(rule.Params.get(0));
                    if (dec == decimal.MinValue) {
                        return prefix + LangUtil.getMsg("valid.rule.WNE.p1.err1"); // Rule parameter "Timeout" should be a nubmer
                    }
                } 
                if (rule.Params.Count == 2) {
                    object obj = rule.Params.get(1);
                    if (obj == null && !(obj is Operation)) {
                        return prefix + LangUtil.getMsg("valid.rule.WNE.p2.err1"); // Rule parameter "NextOp" should be an Operation/Process
                    }
                }
            }
            return string.Empty;
        }
    }
}
