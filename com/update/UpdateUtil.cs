using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using WebMaster.lib;
using WebMaster.lib.engine;
using System.Xml;
using System.Net;

namespace WebMaster.com.update
{
    public class UpdateUtil
    {
        /// <summary>
        /// Check whether need to update application files, true : need to update application files, false : no updates
        /// </summary>
        public static bool needUpdateFiles(){
            try {                
                // get the server side update url base
                string serverURLBase = getServerUpdateBase();
                if (serverURLBase == null || serverURLBase.Length == 0) {
                    return false;
                }
                // remote updater.xml url
                string serverXMLURLFullPath = serverURLBase + "/" + Constants.SERVER_UPDATE_XML;
                string localXmlFullPath = Application.StartupPath + "\\" + Constants.LOCAL_UPDATE_XML;
                List<FileInfoEx> newFiles = getFileInfoList(serverXMLURLFullPath);
                List<FileInfoEx> currFiles = getFileInfoList(localXmlFullPath);
                foreach (FileInfoEx fInfo in newFiles) {
                    string newVer = fInfo.Version;
                    string localVer = getVersion(fInfo.Filename,currFiles);
                    if(IsNeedUpdate(localVer,newVer)){
                        return true ;
                    }
                }
            } catch (Exception) {
                return false;
            }

            return false;
        }
        /// <summary>
        /// Return the file version or string.Empty if errors 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="currFiles"></param>
        /// <returns></returns>
        private static string getVersion(string filename, List<FileInfoEx> currFiles) {
            foreach (FileInfoEx fInfo in currFiles) {
                if (fInfo.Filename == filename) {
                    return fInfo.Version;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Get a list of the FileInfoEx, or empty list if errors 
        /// </summary>
        /// <param name="xmlFileFullPath"></param>
        /// <returns></returns>
        private static List<FileInfoEx> getFileInfoList(string xmlFileFullPath) {
            List<FileInfoEx> list = new List<FileInfoEx>();
            if (xmlFileFullPath == null || xmlFileFullPath.Length == 0) {
                return list; 
            }
            XmlDocument xml = loadXml(xmlFileFullPath);
            if (xml == null) {
                string title = "Error";
                string msg = UILangUtil.getMsg("com.update.util.err1");
                MessageBox.Show(msg, title);
                return list;
            } else {
                // if there is a file need to be updated 
                XmlNodeList nlist = xml.SelectSingleNode("//Files").ChildNodes;
                foreach (XmlNode node in nlist) { 
                    FileInfoEx finfo = new FileInfoEx();
                    finfo.Filename = node.Attributes["name"].Value;
                    finfo.Version = node.Attributes["version"].Value;
                    list.Add(finfo);
                }
            }
            return list;
        }
        /// <summary>
        /// load xml file from the local full path or full server url. 
        /// return a loaded finished xmlDocument or null if errors 
        /// </summary>
        /// <param name="xmlFileFullPath"></param>
        /// <returns></returns>
        private static XmlDocument loadXml(string xmlFileFullPath) {
            if (xmlFileFullPath != null) {
                XmlDocument xml = new XmlDocument();
                try {
                    xml.Load(xmlFileFullPath);
                    return xml;
                } catch (Exception) {                    
                }
            }
            return null;
        }
        /// <summary>
        /// update the updater application if need. 
        /// 1 : there is a new updater file and updated sucess
        /// 0 : there is no new updater file 
        /// -1: there is a new updater file but updat failed
        /// </summary>
        public static int downloadUpdater() {
            // get the server side update url base
            string serverURLBase = getServerUpdateBase();
            if (serverURLBase == null || serverURLBase.Length == 0) {
                string title = "Error";
                string msg = "check update.xml error. path = " + Application.StartupPath;
                MessageBox.Show(msg, title);
                return -1;
            }
            // remote updater.xml url
            string serverXMLURLPath = serverURLBase + "/" + Constants.SERVER_UPDATE_XML;
            string localSaveFolder = getLocalTmpUpdateFolder();
            //Log.println_upd("serverURLBase = " + serverURLBase + ", serverXMLURLPath = " + serverXMLURLPath + ", localSaveFolder = " + localSaveFolder);
            // make sure the temp save file folder existed 
            if (!System.IO.Directory.Exists(localSaveFolder)) {
                try {
                    System.IO.Directory.CreateDirectory(localSaveFolder);
                } catch (Exception) {
                    //string title = "Error";
                    //string msg = "Can not create update folder on path=" + localSaveFolder;
                    //MessageBox.Show(msg, title);
                    return -1;
                }
            }
            string newXmlFullPath = localSaveFolder + "\\" + Constants.LOCAL_UPDATE_XML;
            //Log.println_upd("newXmlFullPath = " + newXmlFullPath);
            bool isOK = downloadFile(newXmlFullPath, serverXMLURLPath);
            if (isOK) {
                bool flag = downloadUpdater(newXmlFullPath, serverURLBase, localSaveFolder);
                if (flag) { 
                    // start updater.exe and close current application 
                    System.Diagnostics.Process updateLanch = new System.Diagnostics.Process();
                    string targetProcessFullPath = Application.StartupPath + "\\" + Constants.UPDATER_NAME;
                    updateLanch.StartInfo = new ProcessStartInfo(targetProcessFullPath) ;
                    updateLanch.Start();
                    // Application exit
                    Application.Exit();
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// Get server update base url from local update.xml file, or return null if errors 
        /// </summary>
        /// <returns></returns>
        private static string getServerUpdateBase() {
            string localUpdaterXMLFullPath = Application.StartupPath + "\\" + Constants.LOCAL_UPDATE_XML;
            Log.println_upd("localUpdaterXMLFullPath = " + localUpdaterXMLFullPath);
            if (!File.Exists(localUpdaterXMLFullPath)) {                
                return null;
            }
            // load and parse the updater.xml 
            XmlDocument localXml = new XmlDocument();
            try {
                localXml.Load(localUpdaterXMLFullPath);
                return localXml.SelectSingleNode("//URL").InnerText;
            } catch (Exception) {                
                return null;
            }
        }
        /// <summary>
        /// Local update files tmp save folder, NOT end with \, like : xxx\xx\
        /// </summary>
        /// <returns></returns>
        private static string getLocalTmpUpdateFolder() {
            string appId = Constants.UPDATE_TMP_APPID;
            string s1 = Environment.GetEnvironmentVariable("Temp");
            if (s1 == null || s1.Length == 0) {
                s1 = Application.StartupPath;
            }
            return s1+"\\" + "_" + appId + "_update";
        }

        private static bool downloadUpdater(string newXmlFullPath, string serverUpdateBase, string localSaveFolder) {            
            string localXmlFullPath = Application.StartupPath + "\\" + Constants.LOCAL_UPDATE_XML;
            string newVer = getUpdaterVersion(newXmlFullPath);
            string currVer = getUpdaterVersion(localXmlFullPath);

            if (IsNeedUpdate(currVer, newVer)) {
                string localNewUpdaterFullPath = localSaveFolder + "\\" + Constants.UPDATER_NAME;
                string remoteFileURL = serverUpdateBase + "/" + Constants.UPDATER_NAME;
                bool isOK = downloadFile(localNewUpdaterFullPath, remoteFileURL);

                // update existed files and remove the temp downloaded files 
                if (isOK) {
                    string localCurrentUpdateExeFilePath = Application.StartupPath + "\\" + Constants.UPDATER_NAME;
                    try {
                        File.Copy(localNewUpdaterFullPath, localCurrentUpdateExeFilePath, true);
                        File.Copy(newXmlFullPath, localXmlFullPath,true);
                        
                        File.Delete(newXmlFullPath);
                        File.Delete(localNewUpdaterFullPath);
                    } catch (Exception) {                        
                        return false;
                    }
                }
            }

            return false;

        }
        /// <summary>
        /// Get the updater version or empty if errors 
        /// </summary>
        /// <param name="newXmlFullPath"></param>
        /// <returns></returns>
        private static string getUpdaterVersion(string newXmlFullPath) {
            XmlDocument updater = loadXml(newXmlFullPath);
            if (updater == null) {
                return string.Empty ;
            }
            return updater.SelectSingleNode("//version").InnerText;
        }
        
        /// <summary>
        /// Download the file from remote file path to local file path, return true if download sucess,
        /// or false if download failed. 
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="remoteFileURL"></param>
        /// <returns></returns>
        private static bool downloadFile(string localFilePath, string remoteFileURL) {
            bool isOK = false;
            using (WebClient wc = new WebClient()) {
                try {
                    wc.DownloadFile(remoteFileURL, localFilePath);
                    isOK = true;
                } catch (Exception) {             
                }
            }
            return isOK;
        }
        /// <summary>
        ///	true : need update to the new version, false : no update needed. 
        /// </summary>
        /// <param name="localVersion"></param>
        /// <param name="remoteVersion"></param>
        /// <returns></returns>
        private static bool IsNeedUpdate(string localVersion, string remoteVersion) {
            if (localVersion == null || remoteVersion == null || localVersion.Length < 7 || remoteVersion.Length < 7) {
                return false;
            }
            try {
                long lcVersion = Convert.ToInt64(localVersion.Replace(".", ""));
                long rmVersion = Convert.ToInt64(remoteVersion.Replace(".", ""));
                return lcVersion < rmVersion;
            } catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return false;
        }
    }
    public class FileInfoEx {
        private string filename = "";

        public string Filename {
            get { return filename; }
            set { filename = value; }
        }
        private string version = string.Empty;

        public string Version {
            get { return version; }
            set { version = value; }
        }
    }
}
