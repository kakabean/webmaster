using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Net;

namespace WebMaster.update
{
    public partial class Update : Form
    {
        /// <summary>
        /// ToBe started application name when the update finished. 
        /// </summary>
        private string targetAppName = "MayiIDE.exe";
        /// <summary>
        /// Server update base url, all the updated files will be put under this url. 
        /// </summary>
        private string serverURLBase = "";
        /// <summary>
        /// This folder is used to cache the tmp downloaded update files 
        /// </summary>
        private string localTmpSaveFolder = "";
        /// <summary>
        /// All to be updated file name list. 
        /// </summary>
        private List<string> newFileList = new List<string>();
        /// <summary>
        /// current updated file index in the newFileList
        /// </summary>
        private int currentUpdateFileIndex = -1;
        
        private bool downloadWithError = false;
        
        public Update() {
            InitializeComponent();
        }
        public Update(string targetAppName) {
            this.targetAppName = targetAppName;
            InitializeComponent();
        }

        private void Update_Load(object sender, EventArgs e) {
             int r = doUpdate();
             if (r == 0) {
                 startOriginalApp();
             }
        }
        /// <summary>
        /// -1: connect to server error. 
        /// 0 : No need to do update 
        /// 1 : start to update 
        /// 2 : create tmp folder error. 
        /// </summary>
        /// <returns></returns>
        private int doUpdate() {
            // get the server side update url base
            this.serverURLBase = UpdateUtil.getServerUpdateBase();
            if (serverURLBase == null || serverURLBase.Length == 0) {
                lb_info.Text = UILangUtil.getMsg("update.util.err1");
                return -1;
            }
            // get the to be updated file list. 
            List<string> ll = UpdateUtil.getUpdateFiles(serverURLBase);
            if (ll.Count == 0) {
                return 0;
            }
            newFileList.Clear();
            newFileList.AddRange(ll);
            // verify the local file save folder 
            this.localTmpSaveFolder = UpdateUtil.getLocalTmpUpdateFolder();
            bool verify = UpdateUtil.verifyLocalSaveFolder(localTmpSaveFolder);
            if (verify == false) {
                lb_info.Text = UILangUtil.getMsg("update.msg.text3");
                return 2;
            }
            // start to update files one by one. 
            this.currentUpdateFileIndex = 0;
            downloadFileAsync();
            return 1;
        }
        /// <summary>
        /// Download the file marked index with currentUpdateFileIndex from remote file path to local file path.
        /// </summary>
        /// <returns></returns>
        internal void downloadFileAsync() {
            if (this.currentUpdateFileIndex < this.newFileList.Count && this.currentUpdateFileIndex >= 0) {
                string fname = this.newFileList[this.currentUpdateFileIndex];
                string localFileFullPath = localTmpSaveFolder + "\\" + fname;
                verifyFilePathFolder(localFileFullPath);
                string removetFileFullPath = serverURLBase + "/" + fname.Replace("\\", "/");
                // update the file name label. 
                string str1 = UILangUtil.getMsg("update.msg.text1");
                string str2 = UILangUtil.getMsg("update.msg.text2");
                this.lb_info.Text = str1+"("+this.currentUpdateFileIndex+"/"+this.newFileList.Count+")   |   "+str2+fname;
                using (WebClient wc = new WebClient()) {
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    try {
                        wc.DownloadFileAsync(new Uri(removetFileFullPath), localFileFullPath);                        
                    } catch (Exception) {
                        this.downloadWithError = true;
                        string msg = UILangUtil.getMsg("update.msg.err1");
                        lb_info.Text = msg;
                    }
                }
            }            
        }
        /// <summary>
        /// For the server file name, if it is under a folder, make sure the folder existed. 
        /// </summary>
        /// <param name="localFileFullPath"></param>
        private void verifyFilePathFolder(string localFileFullPath) {
            if (localFileFullPath != null) {
                int lastIndex = localFileFullPath.LastIndexOf("\\");
                string folder = localFileFullPath.Substring(0, lastIndex);
                UpdateUtil.verifyLocalSaveFolder(folder);
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            int value = e.ProgressPercentage;
            progressBar1.Value = value;            
        }

        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            if (downloadWithError == false) {
                if (this.currentUpdateFileIndex < this.newFileList.Count - 1 && this.currentUpdateFileIndex >= 0) {
                    this.currentUpdateFileIndex += 1; // download the next file 
                    downloadFileAsync();
                } else if (this.currentUpdateFileIndex == this.newFileList.Count - 1) { // the last file download finished
                    lb_info.Text = UILangUtil.getMsg("update.msg.text4");// "Update completed ! ";
                    updateExistedFiles();
                    startOriginalApp();
                }
            } else {
                string msg = UILangUtil.getMsg("update.msg.err1");
                lb_info.Text = msg;
            }
        }

        private void startOriginalApp() {
            string appPath = Application.StartupPath + "\\" + targetAppName;
            // start updater, and updater will start app when udpate done. 
            System.Diagnostics.Process launch = new System.Diagnostics.Process();
            //string path = "D:\\ZhangHui\\mywork\\WebMasterAll\\solution\\WebMaster\\ide\\bin\\Debug\\ide.exe";
            launch.StartInfo = new ProcessStartInfo(appPath);
            launch.StartInfo.Arguments = "true";
            launch.Start();
            // close current application
            Application.Exit();
        }
        /// <summary>
        /// copy the cached files to replace current existed ones
        /// </summary>
        private void updateExistedFiles() {
            foreach (string fname in this.newFileList) {
                string newFileFullPath = localTmpSaveFolder + "\\" + fname;
                string existedFileFullPath = Application.StartupPath + "\\" + fname;
                try {
                    if (File.Exists(existedFileFullPath)) {
                        File.Delete(existedFileFullPath);
                    }
                    verifyFilePathFolder(existedFileFullPath);
                    if (File.Exists(newFileFullPath)) {
                        File.Move(newFileFullPath, existedFileFullPath);
                    }
                }catch(Exception e){
                    string msg = UILangUtil.getMsg("update.msg.err1")+e.StackTrace;
                    MessageBox.Show(this, msg, "Error");
                    break;
                }
            }
        }
    }
}
