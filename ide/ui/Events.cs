using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.ide.editor;
using WebMaster.lib.ui;

namespace WebMaster.ide.util
{
    public class CommonEventArgs : EventArgs {
        private Object data = null;

        public Object Data {
            get { return data; }
            set { data = value; }
        }
        private Object sender = null;

        public Object Sender {
            get { return sender; }
            set { sender = value; }
        }

        public CommonEventArgs(Object sender, Object data) {
            this.sender = sender;
            this.data = data;
        }
    }

    public class CommonCodeArgs : EventArgs
    {
        public static readonly int INVALID = Int32.MinValue;
        private int code = INVALID;

        public int Code {
            get { return code; }
        }
        private Object sender = null;

        public Object Sender {
            get { return sender; }
            set { sender = value; }
        }

        public CommonCodeArgs(Object sender, int code) {
            this.sender = sender;
            this.code = code;
        }
    }
    /// <summary>
    /// it is used for model element validation, data is validated element, msg is validaton message
    /// </summary>
    public class ValidationArgs : CommonEventArgs
    {
        public ValidationArgs(object sender, object data, MsgType msgType, string msg):base(sender,data){            
            this.Msg = msg;
            this.type = msgType;
        }
        private MsgType type = MsgType.VALID;
        /// <summary>
        /// msg type 
        /// </summary>
        internal MsgType Type {
            get { return type; }
            set { type = value; }
        }

        private string msg = null;
        /// <summary>
        /// validation msg
        /// </summary>
        public string Msg {
            get { return msg; }
            set { msg = value; }
        }
    }
}
