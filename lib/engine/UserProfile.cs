using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.lib.engine
{
    /// <summary>
    /// this profile is used in runtime, it is used to maintain the runtime user status. 
    /// </summary>
    public class UserProfile
    {
        #region base info
        private string uid = null;
        /// <summary>
        /// unique id of the customer 
        /// </summary>
        public string UID {
            get { return uid; }
            set { uid = value; }
        }

        private string pwd = null;

        public string Password {
            get { return pwd; }
            set { pwd = value;  }
        }
        private string verifyCode = null;
        /// <summary>
        /// verification code, maybe a picture or a question answer, it is used to enhance the
        /// security access 
        /// </summary>
        public string VerifyCode {
            get { return verifyCode; }
            set { verifyCode = value; }
        }

        private string ticket = null;
        /// <summary>
        /// When login successfully, server will sendback an unique to certify the user
        /// </summary>
        public string Ticket {
            get { return ticket; }
            set { ticket = value; }
        }
        private RESPONSE response = RESPONSE.INIT;
        /// <summary>
        /// server response
        /// </summary>
        public RESPONSE Response {
            get { return response; }
            set { response = value; }
        }
        /// <summary>
        /// Key is internal used string that is used retrieve the real scripts from server side. 
        /// Value is script name text that will show in UI. 
        /// </summary>
        private Dictionary<string, string> ownedScripts = new Dictionary<string, string>();
        /// <summary>
        /// Key is internal used string that is used retrieve the real scripts from server side. 
        /// Value is script name text that will show in UI. 
        /// </summary>
        private Dictionary<string, string> bookedScripts = new Dictionary<string, string>();
        #endregion 
        #region register info 
        // register info 
        private string name = null;

        public string Name {
            get { return name; }
            set { name = value; }
        }
        private DateTime _lastLoginTime = DateTime.MinValue;
        /// <summary>
        /// Time the user last logon 
        /// </summary>
        public DateTime LastLogonTime
        {
            get { return _lastLoginTime; }
            set { _lastLoginTime = value; }
        }
        #endregion               
    }
    /// <summary>
    /// server response types
    /// </summary>
    public enum RESPONSE { 
        /// <summary>
        /// this is a default value, internal program use
        /// </summary>
        INIT,
        /// <summary>
        /// network error, can not connect to the server 
        /// </summary>
        CONNECT_ERROR, 
        /// <summary>
        /// user name or password incorrect
        /// </summary>
        PWD_ERROR, 
        /// <summary>
        /// user login sucessfully
        /// </summary>
        LOGIN_SUCCESS, 
        /// <summary>
        /// server is busy, please try later.
        /// </summary>
        SERV_BUSY
    }
}
