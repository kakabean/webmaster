using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.lib.ui
{
    /* this file will used to define some data model that will be used by flow editor 
     in design time
    */

    /// <summary>
    /// validation msg info
    /// </summary>
    public class ValidationMsg {
        private MsgType type = MsgType.VALID;

        public MsgType Type {
            get { return type; }
            set { type = value; }
        }
        private string msg=string.Empty;

        public string Msg {
            get { return msg; }
            set { msg = value; }
        }
    }

    public enum MsgType{
        VALID,
        INFO, 
        ERROR,
        WARNING
    }
}
