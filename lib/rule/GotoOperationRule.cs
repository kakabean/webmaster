using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;

namespace WebMaster.lib.rule
{
    /// <summary>
    /// This rule is used to handle the NO_NEXT_OP_FOUND scenario.
    /// 
    /// Scenario: 
    /// 1. go to another pair operation as next op
    ///    [rule next operation]
    /// 
    /// </summary>
    class GotoOperationRule
    {
        public static bool execute(ListRef ruleParams, OpWrapper opw, WebEngine eng) {
            if (ruleParams == null || ruleParams.Count!=1 || opw == null || eng == null) {
                Log.println_eng("T = ENGINE, ERROR, Rule GotoOperationRule, set input invalid parameters . ");
                return false;
            }
            Operation ruleNextOp = ruleParams.get(0) as Operation;
            // validation ruleNextOp and opw.Op is the same level 
            // or if it is not in the same level, it will need to clean the relative process 
            // environment info in the engine. 
            Process nextOpOwnerProc = ModelManager.Instance.getOwnerProc(ruleNextOp);
            Process tmpProc = ModelManager.Instance.getOwnerProc(opw.Op);
     
            bool errFlag = nextOpOwnerProc == null ? true : false ;
            List<Process> cleanProcs = new List<Process>();
            if (errFlag == false) {                
                while (nextOpOwnerProc != tmpProc && tmpProc != null) {
                    cleanProcs.Add(tmpProc);
                    tmpProc = ModelManager.Instance.getOwnerProc(tmpProc);
                }          
            }
            if (errFlag) {
                Log.println_eng("T = ENGINE, ERROR, GotoOperationRule, rule next Op = " + ruleNextOp.Name + " should has the same value with current Op = " + opw.Op.Name);
                ruleNextOp = null;
            }

            opw.Op = ruleNextOp;
            if (cleanProcs.Count > 0 && opw.Op!=null) {
                opw.CleanedProcs = cleanProcs;
                opw.Status = OpStatus.CLEAN_PROC_ENV_AND_GOTO;
            } else {
                opw.Status = OpStatus.READY;
            }
            eng.resetOpw(opw);

            return true;
        }
        /// <summary>
        /// get the action description or string.emtpty if errors 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static string getDescription(OperationRule rule) {
            string text = string.Empty;
            if (rule != null) {
                text = LangUtil.getMsg("Rule.GotoOperationRule");
            }
            return text;
        }
        /// <summary>
        /// Get the Rule action's validation msg, return msg string with local if errros or return string.Empty if valid.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string getValidMsg(OperationRule rule) {
            if (rule != null && rule.Action == RuleAction.Goto_Operation) {
                Operation op = ModelManager.Instance.getOwnerOp(rule);
                string prefix = LangUtil.getMsg("model.Op.Name");
                if (op is Process) {
                    prefix = LangUtil.getMsg("model.Proc.Name");
                }
                string name = op == null ? "" : op.Name;
                prefix += " = " + name + " - ";

                if (rule.Params.Count == 1) {                    
                    object obj = rule.Params.get(0);
                    if (obj == null && !(obj is Operation)) {
                        return prefix + LangUtil.getMsg("valid.rule.goto.p1.err1"); // Rule parameter "NextOp" should be an Operation/Process
                    }
                }
            }
            return string.Empty;
        }
    }
}
