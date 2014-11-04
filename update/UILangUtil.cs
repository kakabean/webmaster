using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;

namespace WebMaster.update
{
    class UILangUtil
    {
        private static readonly string resPath = "update.lang.UIResource";

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
    }
}
