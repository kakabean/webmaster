using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WebMaster.lib
{
    public class Log
    {
        public const string LogZEngine = "WebEngine";

        public const string LogWLScript = "WLScript";

        private static readonly bool logThread = false;

        private static readonly bool logScript = true;

        private static readonly int CACHE_SIZE = 1024 * 64;
        /// <summary>
        /// cache of the log info 
        /// </summary>
        private StringBuilder cache = new StringBuilder(CACHE_SIZE);

        public static void log(string msg, string category, string method) {
            Debug.WriteLine(msg, category + " - " + method);
        }

        public static void logIf(bool condition, string msg, string category, string method) {
            Debug.WriteLineIf(condition, msg, category + " - " + method);
        }

        public static void LogThreadInfo(string msg) {
            if (logThread) {
                System.Console.WriteLine("-----Thread--- " + msg);
            }
        }

        public static void LogScript(string msg) {
            if (logScript) {
                System.Console.WriteLine("[OPERATION]: " + msg);
            }
        }

        public static void println(string msg) {
            string[] strs = msg.Split('\n');
            foreach (string ss in strs) {
                string str = ss;
                if (str != null && str.Length > 200) {
                    str = ss.Substring(0, 180) + " ... cut off others ...";
                }
                str = str == null ? "" : str;
                Console.WriteLine(str);    
            }            
        }
        public static void print(string msg) {
            string[] strs = msg.Split('\n');
            foreach (string ss in strs) {
                string str = ss;
                if (str != null && str.Length > 200) {
                    str = str.Substring(0, 180) + " ... cut off others ...";
                }
                str = str == null ? "" : str;
                Console.Write(str);
            }
        }

        public static void println_hook(string msg) {            
            //println(msg);   
        }
        /// <summary>
        /// print web element/properties info 
        /// </summary>
        /// <param name="msg"></param>
        public static void println_brw(string msg) {            
            //println(msg);
        }
        /// <summary>
        /// print engine info 
        /// </summary>
        /// <param name="msg"></param>
        public static void println_eng(string msg) {
            //println(msg);
        }
        public static void println_cap(string msg) {
            //println(msg);
        }
        public static void println_wetree(string msg) {
            //println(msg);
        }
        public static void println_ptree(string msg) {
            //println(msg);
        }
        public static void println_prop(string msg) {
            //println(msg);
        }
        public static void println_graph(string msg) {
            //println(msg);
        }
        public static void println_login(string msg) {
            //println(msg);
        }

        public static void println_map(string msg) {
            //println(msg);
        }
        public static void println_upd(string msg) {
            //println(msg);
        }
    }
}
