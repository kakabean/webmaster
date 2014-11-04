using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;

namespace WebMaster.lib.rule
{
    /// <summary>
    /// This is used to restart the script and engine status and run the script again as a new one
    /// </summary>
    class RestartScriptRule
    {
        public static bool execute(ListRef ruleParams, OpWrapper opw, WebEngine eng) {
            opw.Status = OpStatus.RESTART_SCRIPT;
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
                text = LangUtil.getMsg("Rule.RestartScriptRule");
            }
            return text;
        }
    }
}
