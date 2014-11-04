using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;

namespace WebMaster.com
{
    class UILangUtil
    {
        private static readonly string resPath = "com.lang.UIResource";

        static ResourceManager rm = new ResourceManager(resPath, Assembly.GetExecutingAssembly());
        /// <summary>
        /// Get the msg with default system locale info 
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public static string getMsg(string msgId) {
            return getMsg(msgId, Thread.CurrentThread.CurrentCulture);
        }
        /// <summary>
        /// get msg with specified locale info 
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string getMsg(string msgId, CultureInfo culture) {
            return rm.GetString(msgId, culture);
        }

        /// <summary>
        /// This is used for that the NLS string has some variables defined as {0},{1} 
        /// and it will be replaced with values item's string inorder if have. 
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string getMsg(string msgId, params object[] values) {
            string msg = getMsg(msgId);
            string pattern = @"\{\d+\}";
            Match match = Regex.Match(msg, pattern);
            if (match.Success == false) {
                return msg;
            }
            string[] ss = Regex.Split(msg, pattern);

            if (values != null && values.Length > 0) {
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (string s in ss) {
                    string param = string.Empty;
                    if (i >= 0 && i < values.Length) {
                        param = values[i].ToString();
                    }
                    sb.Append(s).Append(param);
                    i++;
                }
                msg = sb.ToString();
            }

            return msg;
        }
    }
}
