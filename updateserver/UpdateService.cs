using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib;
using System.Collections;
using System.IO;
using System.Reflection;
using WebMaster.lib.engine;

namespace WebMaster.updateserver 
{
    /// <summary>
    ///		The update service
    /// </summary>
    public class UpdateService : MarshalByRefObject, IUpdateService
    {
        #region IUpdateService Members

        /// <summary>
        /// Get all files
        /// </summary>
        /// <returns></returns>
        public string[] getFiles() {
            //Logger.LogMessage("localhost","Inside UpdateService::GetFiles()");
            return ConfigInfo.Instance.FileNameVersion;
        }

        /// <summary>
        ///	Get the current version of the file
        /// </summary>
        /// <param name="fileName">file name with the extension, not include the file path</param>
        /// <returns></returns>
        public string getCurrentVersion(string fileName) {
            //Logger.LogMessage("localhost", "Inside UpdateService::GetCurrentVersion()");
            
            for(int i=0; i<ConfigInfo.Instance.FileNameVersion.Length ; i+=2) {
                string name = ConfigInfo.Instance.FileNameVersion[i];
                if(name.Equals(fileName,StringComparison.CurrentCultureIgnoreCase)){
                    return ConfigInfo.Instance.FileNameVersion[i + 1];
                }
            }
            return "Invalid file name";
            //throw new ArgumentException("Given file is not found into the server.");
        }

        /// <summary>
        ///	Gets the entire file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] getFile(string fileName) { 
            //Logger.LogMessage("localhost", "Inside UpdateService::GetFile()");
            foreach (FileObject fileObject in ConfigInfo.Instance.FileObjects) {
                if (fileObject.FileInfo.Name.Equals(fileName)) {
                    return getBinaryContents(fileObject);
                }
            }
            return null;
            //throw new ArgumentException("Given file is not found into the server.");
        }

        #endregion

        /// <summary>
        ///		Gets the binary content of the entire file
        /// </summary>
        /// <param name="?"></param>
        private byte[] getBinaryContents(FileObject fileObject) {
            byte[] block;
            using (FileStream fileStream = File.OpenRead(fileObject.FileInfo.FullName)) {
                using (BinaryReader reader = new BinaryReader(fileStream)) {
                    block = reader.ReadBytes((int)fileStream.Length);
                }
            }
            return block;
        }
    }
    
    public class ConfigInfo {
        private static ConfigInfo configInfo;
        private static object syncObject = new object();

        /// <summary>
        /// 
        /// </summary>
        private ConfigInfo() {}
        private string[] fileNameVersion = new string[] { "",""};
        /// <summary>
        /// file name,version arrays, the r[i]=filename, r[i+1]=file version 
        /// </summary>
        public string[] FileNameVersion {
            get { return fileNameVersion; }            
        }
        
        private FileObject[] fileObjects;

        public FileObject[] FileObjects {
            get { return fileObjects; }
            set { 
                fileObjects = value;
                updateFileNameVersion();
            }
        }

        private void updateFileNameVersion() {
            List<string> list = new List<string>();
            foreach (FileObject fo in fileObjects) {
                string name = fo.FileInfo.Name;
                string version = getVersion(fo.FileInfo);
                if (version != Constants.FILE_UNKNOWN && name != null) {
                    list.Add(name);
                    list.Add(version);
                }
            }
            if (list.Count > 1) {
                fileNameVersion = list.ToArray();
            } else {
                fileNameVersion = new string[] { "",""};
            }
        }
        /// <summary>
        ///		Get the version of an assembly
        /// </summary>
        /// <param name="fInfo"></param>
        /// <returns></returns>
        /// <remarks>
        ///	Only the managed assemblies will work here, like .exe or .dll file. For unmanaged it will simply say UNKNOWN        
        /// </remarks>
        public string getVersion(FileInfo fInfo) {
            try {
                Assembly assembly = Assembly.LoadFile(fInfo.FullName);
                if (assembly != null) {
                    string[] parts = assembly.FullName.Split(" ".ToCharArray());
                    foreach (string part in parts) {
                        if (part.ToLower().IndexOf("version") >= 0) {
                            string res = part.Trim();
                            res = res.Substring(res.IndexOf("=") + 1);
                            return res.Trim().Replace(",", "");
                        }
                    }
                }
            } catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return Constants.FILE_UNKNOWN;
        }
        /// <summary>
        ///		Get the single ton instance
        /// </summary>
        public static ConfigInfo Instance {
            get {
                #region Ensuring Singleton
                if (configInfo == null) {
                    lock (syncObject) {
                        if (configInfo == null) {
                            configInfo = new ConfigInfo();
                        }
                    }
                }
                #endregion

                return configInfo;
            }
        }
    }
}
