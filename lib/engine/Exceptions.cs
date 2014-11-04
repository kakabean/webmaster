using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.lib.engine
{
    class RuleException : Exception
    {
        private RuleTrigger _ruleTrigger = RuleTrigger.INVALID;
        /// <summary>
        /// rule trigger type 
        /// </summary>
        public RuleTrigger RuleTrigger {
            get { return _ruleTrigger; }
        }

        public RuleException(RuleTrigger ruleTrigger, string msg)
            : base(msg) {
            _ruleTrigger = ruleTrigger;
        }

        public RuleException(string msg)
            : base(msg) {
        }
    }

    class ConditionException : Exception {

        public ConditionException(String msg) : base(msg) {             
        }
        public ConditionException(String msg, Exception e) : base(msg, e) { }
    }

    class ScriptDesignException : Exception {
        public ScriptDesignException(String msg) : base(msg) { }
        public ScriptDesignException(String msg, Exception e) : base(msg, e) { }
    }

    class ScriptRuntimeException : Exception
    {
        public ScriptRuntimeException(String msg) : base(msg) { }
        public ScriptRuntimeException(String msg, Exception e) : base(msg, e) { }
    }
}
