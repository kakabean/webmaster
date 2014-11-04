using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.Net;

namespace WebMaster.update
{
    /// <summary>
    /// Note this class is only used for update project internal
    /// </summary>
    internal class UpdateUtil
    {
        #region constants Notes: it should be the same with WebMaster.lib.Constants
        /// <summary>
        /// Update exe file name 
        /// </summary>
        public static readonly string UPDATER_NAME = "update.exe";
        public static readonly string IDE_NAME = "ide.exe";
        /// <summary>
        /// Local update xml file for update.exe
        /// </summary>
        public static readonly string LOCAL_UPDATE_XML = "update.xml";
        /// <summary>
        /// Server update xml file for update.exe
        /// </summary>
        public static readonly string SERVER_UPDATE_XML = "update.xml";
        /// <summary>
        /// Application Id, it will be used for create tmp folder when do update. 
        /// </summary>
        public static readonly string UPDATE_TMP_APPID = "MayiBrowser";
        #endregion constants 
        /// <summary>
        /// Return the list of updated file name, or empty list if no update. 
        /// </summary>
        public static List<string> getUpdateFiles(string serverURLBase) {
            List<string> list = new List<string>();
            if (serverURLBase == null || serverURLBase.Length == 0) {
                return list;
            }
            try {
                // remote update.xml url
                string serverXMLURLFullPath = serverURLBase + "/" + SERVER_UPDATE_XML;
                // local file update.xml full path
                string localXmlFullPath = Application.StartupPath + "\\" + LOCAL_UPDATE_XML;
                List<FileInfoEx> newFiles = getFileInfoList(serverXMLURLFullPath);
                List<FileInfoEx> currFiles = getFileInfoList(localXmlFullPath);
                foreach (FileInfoEx fInfo in newFiles) {
                    string newVer = fInfo.Version;
                    string localVer = getVersion(fInfo.Filename, currFiles);
                    if (IsNeedUpdate(localVer, newVer)) {
                        list.Add(fInfo.Filename);
                    }
                }
            } catch (Exception) {
                return list;
            }

            return list;
        }
        /// <summary>
        /// Return the file version from the list or string.Empty if errors 
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
                string msg = UILangUtil.getMsg("update.util.err1"); 
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
        /// Get server update base url from local updater.xml file, or return null if errors 
        /// </summary>
        /// <returns></returns>
        public static string getServerUpdateBase() {
            string localUpdaterXMLFullPath = Application.StartupPath + "\\" + LOCAL_UPDATE_XML;            
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
        /// Verify the local save folder, if not existed, create a new one. 
        /// true : if the folder existed, false : failed. 
        /// </summary>
        public static bool verifyLocalSaveFolder(string folderPath){
            if (folderPath == null || folderPath.Length ==0) {
                return false;
            }
            if (!System.IO.Directory.Exists(folderPath)) {
                try {
                    System.IO.Directory.CreateDirectory(folderPath);
                } catch (Exception) {
                    //string title = "Error";
                    //string msg = "Can not create update folder on path=" + localSaveFolder;
                    //MessageBox.Show(msg, title);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Local update files tmp save folder, NOT end with \, like : xxx\xx\
        /// </summary>
        /// <returns></returns>
        public static string getLocalTmpUpdateFolder() {
            string appId = UPDATE_TMP_APPID;
            string s1 = Environment.GetEnvironmentVariable("Temp");
            if (s1 == null || s1.Length == 0) {
                s1 = Application.StartupPath;
            }
            return s1 + "\\" + "_" + appId + "_update";
        }                
        /// <summary>
        ///	true : need update to the new version, false : no update needed. 
        /// </summary>
        /// <param name="localVersion"></param>
        /// <param name="remoteVersion"></param>
        /// <returns></returns>
        private static bool IsNeedUpdate(string localVersion, string remoteVersion) {
            try {
                long lcVersion = Convert.ToInt64(localVersion.Replace(".", ""));
                long rmVersion = Convert.ToInt64(remoteVersion.Replace(".", ""));
                return lcVersion < rmVersion;
            } catch (Exception) {
                //System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return false;
        }
    }
    public class FileInfoEx
    {
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
