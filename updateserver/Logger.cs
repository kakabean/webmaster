using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebMaster.updateserver
{
    class Logger
    {
        public static ListView listLog;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip">client ip address</param>
        /// <param name="message"></param>
        public static void logAccess(string ip, string message) {
            if (listLog == null) return;
            WriteLog(ip, message);
        }

        /// <summary>
        ///		Handle multi thread scenario
        /// </summary>
        private static void WriteLog(string ip, string message) {
            if (listLog.InvokeRequired) {
                WriteLogDlg writeLogDlg = new WriteLogDlg(WriteLog);
                listLog.Invoke(writeLogDlg, new object[] { message });
            } else {
                ListViewItem item = listLog.Items.Add(DateTime.Now.ToShortTimeString());                
                item.SubItems.Add(ip);
                item.SubItems.Add(message);
                listLog.EnsureVisible(item.Index);
            }
        }
        delegate void WriteLogDlg(string ip, string message);
    }
}
