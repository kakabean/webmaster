using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebMaster.update;

namespace update
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 1) {
                Application.Run(new Update(args[0]));
            } 
            // used for test
            //Application.Run(new Update("MayiBrowser.exe"));
        }
    }
}
