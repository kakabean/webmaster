using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebMaster.ide;
using System.Diagnostics;
using WebMaster.com.update;
using WebMaster.lib.engine;

namespace ide
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
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;            
            if (Constants.IS_DEV_MODE == true || (args.Length == 1 && args[0] == "true")) {
                Application.Run(new WMEditor());
            } else {
                // check whether need to update the update.exe
                UpdateUtil.downloadUpdater();
                if (UpdateUtil.needUpdateFiles()) {
                    // start updater, and updater will start app when udpate done. 
                    System.Diagnostics.Process launch = new System.Diagnostics.Process();
                    string path = Application.StartupPath + "\\" + Constants.UPDATER_NAME;
                    //string path = "D:\\ZhangHui\\mywork\\WebMasterAll\\solution\\WebMaster\\update\\bin\\Debug\\update.exe";
                    launch.StartInfo = new ProcessStartInfo(path);
                    launch.StartInfo.Arguments = Constants.IDE_NAME;
                    launch.Start();
                    // close current application
                    Application.Exit();
                } else {
                    Application.Run(new WMEditor());
                }
            }
        }        
    }
}
