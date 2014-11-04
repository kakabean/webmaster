using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;

namespace WebMaster.lib.rule
{
    public class RuleUtil
    {
        /// <summary>
        /// return the rule trigger description text. or return "" if errors 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string getTriggerText(OperationRule rule) {
            if (rule != null) {
                return ModelManager.Instance.getRuleTriggerText(rule.Trigger);                
            }
            return string.Empty;
        }
        /// <summary>
        /// return the action description text or return "" if errors 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string getActionText(OperationRule rule) {
            string text = string.Empty;
            if (rule != null) {
                if (rule.Action == RuleAction.WaitUntilElemFind) {
                    text = WaitUntilNullElemFindRule.getDescription(rule);
                } else if (rule.Action == RuleAction.RestartScript) {
                    text = RestartScriptRule.getDescription(rule);
                } else if (rule.Action == RuleAction.Goto_Operation) {
                    text = GotoOperationRule.getDescription(rule);
                }
            }
            return text;
        }
    }
}
