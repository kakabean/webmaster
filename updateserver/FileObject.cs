using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.updateserver
{
    public class FileObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        public FileObject(System.IO.FileInfo fileInfo, string version) {
            this.fInfo = fileInfo;
            this.version = version;
        }
        /// <summary>
        /// file version 
        /// </summary>
        private string version;
        /// <summary>
        /// file version string, like 1.0.0.2
        /// </summary>
        public string Version {
            get { return version; }
            set { version = value; }
        }

        private System.IO.FileInfo fInfo;

        public System.IO.FileInfo FileInfo { 
            get { return fInfo; }
            set { fInfo = value; }
        }
    }
}
