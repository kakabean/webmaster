using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WebMaster.lib.engine;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;

namespace WebMaster.lib
{
    /// <summary>
    /// This class is used to handle the configuration. e.g maybe the the user name and password. or UI settings, or user
    /// settings and so on. 
    /// </summary>
    public class ConfigUtil
    {
        /// <summary>
        /// Get the config file or create a new one if not have, return null if exceptions 
        /// </summary>
        /// <returns></returns>
        public static XmlDocument getConfigFile() {
            string configFullPath = Application.StartupPath+"\\"+Constants.LOCAL_CONFIG_XML;
            if(!File.Exists(configFullPath)){
                XmlDocument cfgXml = buildSampleConfigDoc();                
                cfgXml.Save(configFullPath);
            }
            XmlDocument cfg = loadXml(configFullPath);
            return cfg;
            
        }
        /// <summary>
        /// Create a sample config XmlDocument 
        /// </summary>
        /// <returns></returns>
        private static XmlDocument buildSampleConfigDoc() {
            XmlDocument cfg = new XmlDocument();
            XmlDeclaration declaration = cfg.CreateXmlDeclaration("1.0", "utf-8", null);
            cfg.AppendChild(declaration);
            XmlElement root = cfg.CreateElement("Config");
            cfg.AppendChild(root);

            return cfg;
        }
        /// <summary>
        /// load xml file from the local full path or full server url. 
        /// return a loaded finished xmlDocument or null if errors 
        /// </summary>
        /// <param name="xmlFileFullPath"></param>
        /// <returns></returns>
        public static XmlDocument loadXml(string xmlFileFullPath) {
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
        /// Get the saved user info object or null if not find 
        /// </summary>
        /// <param name="configXml"></param>
        /// <returns></returns>
        public static UserProfile getUserInfo(XmlDocument configXml) {
            if (configXml != null) {
                XmlNode nameNode = configXml.SelectSingleNode("/Config/User/Name");
                string name = null;
                if (nameNode != null) {
                    name = nameNode.InnerText;
                }
                XmlNode pwdNode = configXml.SelectSingleNode("/Config/User/Pwd");
                string pwd = null;
                if (pwdNode != null) {
                    pwd = pwdNode.InnerText;
                }
                if (name != null && pwd != null) {
                    UserProfile user = new UserProfile();
                    user.Name = name;
                    user.Password = pwd;
                    user.Response = RESPONSE.INIT;
                    return user;
                }
            }
            return null;
        }
        // Encrypt DES Keys
        private static byte[] Keys = { 0x1F, 0x11, 0x5C, 0xC8, 0xE0, 0x31, 0x24, 0xD2 };
        /// <summary>
        /// Generate the encryptKey by user name, it will be 32 byte string or null if errors 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static string getEncyptKey(string seed) {
            if (seed != null) {
                return ModelManager.Instance.getMd5Hash(seed);                
            }
            return null;
        }
        /// <summary>
        /// Encrypt string or null if errors 
        /// </summary>
        /// <param name="plainString">To be encrypted string</param>
        /// <param name="encryptKey">DSE encript key, must be 8 byte</param>
        /// <returns>encrypted string or null if errors </returns>
        public static string EncryptDES(string plainString, string encryptKey) {
            try {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(plainString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();                
                return Convert.ToBase64String(mStream.ToArray());
            } catch {
                return plainString;
            }
        }

        /// <summary>
        /// DES to be decrypted string or null if errors 
        /// </summary>
        /// <param name="decryptString">Tobe decrypted string </param>
        /// <param name="decryptKey">DSE key, must be same with encrypt keys, 8 bytes</param>
        /// <returns>decrypted plain text or null string if errors </returns>
        public static string DecryptDES(string decryptString, string decryptKey) {
            try {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            } catch {
                return null;
            }
        }
    }
}
