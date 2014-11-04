using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using WebMaster.lib.engine;
using System.Drawing;
using WebMaster.lib.ui;
using System.Threading;
using WebMaster.lib.gf;
using System.Reflection;
using WebMaster.lib.rule;

namespace WebMaster.lib.engine
{
    /// <summary>
    /// this class handle Model relative operations, e.g how to find a WebElement in the script root
    /// how to get the condition result and so on 
    /// </summary>
    public class ModelManager
    {
        #region constants 
        private static readonly char[] BE_NAME_INVALID_CHARS = new char[] {'~','`','!','@','#','$','%','^','&','*','|','|',':','/','?','<','>','{','}' };
        private static string BE_NAME_INVALID_STR = "";
        private static readonly int BE_NAME_MAX_LENGTH = 255;
        /// <summary>
        /// max description length except script root 
        /// </summary>
        internal static readonly int BE_DESC_MAX_LENGTH = 4096;
        /// <summary>
        /// max script descriptin length 
        /// </summary>
        private static readonly int SROOT_DESC_MAX_LENGTH = 10240;
        private static readonly int SROOT_URL_MAX_LENGTH = 1024;
        #endregion constatns 
        #region variables
        // Create a new instance of the MD5CryptoServiceProvider object.
        public static MD5 md5Hasher = MD5.Create();
        /// <summary>
        /// used to generate random VALUE 
        /// </summary>
        private Random rnd = new Random(); 
        #endregion variables
        private ModelManager(){
            // updated invalid string 
            StringBuilder sb = new StringBuilder();
            foreach (char c in BE_NAME_INVALID_CHARS) {
                sb.Append(c).Append(" ");
            }            
            BE_NAME_INVALID_STR = sb.ToString();
        }
        #region initial function

        private static ModelManager _instance = null;
        public static ModelManager Instance{
            get {
                if (_instance == null) {
                    _instance = new ModelManager();
                }

                return _instance;
            }
        }
        #endregion inital function
        #region file operation area
        /// <summary>
        /// save the script model info to file 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filepath"> path of the script </param>
        public void saveScript(ScriptRoot root, string filepath) {
            FileStream fs = null;
            if (root == null || filepath == null || filepath.Length < 1) {
                return;
            }
            try {
                fs = new FileStream(@filepath, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, root);
            } catch (Exception e) {
                Trace.WriteLine(e.StackTrace);
            } finally {
                if (fs != null) {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        /// <summary>
        /// return ScriptRoot object or null if errors 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public ScriptRoot loadScript(string filepath) {
            if (filepath == null || filepath.Length < 1) {
                return null;
            }
            ScriptRoot root = null;
            FileStream fs = null;
            try {
                fs = new FileStream(@filepath, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                root = bf.Deserialize(fs) as ScriptRoot;
            } catch (SerializationException se) {
                Trace.WriteLine(se.StackTrace);
            } finally {
                if (fs != null) {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return root;
        }
        /// <summary>
        /// save the view model and script model 
        /// </summary>
        /// <param name="bgmodel"></param>
        /// <param name="filepath"></param>
        public void saveBigModel(BigModel bgmodel, string filepath) {
            FileStream fs = null;
            if (bgmodel == null || filepath == null || filepath.Length < 1) {
                return;
            }
            try {
                fs = new FileStream(@filepath, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, bgmodel);
            } catch (Exception e) {
                Trace.WriteLine(e.StackTrace);
            } finally {
                if (fs != null) {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        /// <summary>
        /// return Big model or null if errors 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public BigModel loadBigModel(string filepath)
        {
            Log.println_eng("== load model, file path = "+filepath);
            if (filepath == null || filepath.Length < 1){
                return null;
            }
            BigModel bgmodel = null;
            FileStream fs = null;
            try {
                fs = new FileStream(filepath, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                bgmodel = bf.Deserialize(fs) as BigModel;
            } catch (SerializationException se) {
                Trace.WriteLine(se.StackTrace);
            } finally {
                if (fs != null) {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return bgmodel;
        }
        #endregion
        #region scriptRoot area
        /// <summary>
        /// get a script by a key
        /// 
        /// </summary>
        /// <param name="uid">uid of the script</param>
        /// <returns>return ScriptRoot or null if errors</returns>
        public ScriptRoot getScript(string uid) {
            //string path = getFi 
            //return loadScript(uid);
            return null;
        }        
        /// <summary>
        /// Initial default User log color map for script. 
        /// </summary>
        /// <param name="sroot"></param>
        internal void initDefaultLogColorMap(ScriptRoot sroot) {
            if (sroot != null) {
                sroot.ColorMap.Add(Constants.LOG_USER_KEY_TIME, -16776961); // Color.Blue
                sroot.ColorMap.Add(Constants.LOG_USER_KEY_STR, -16777216); //  Black
                sroot.ColorMap.Add(Constants.LOG_USER_KEY_WEA, -2987746);  // Chocolate
                sroot.ColorMap.Add(Constants.LOG_USER_KEY_PARAM, -5952982); // Brown  
            }
        }
        /// <summary>
        /// Whether the url is in the trustedURLs list. true: it is already in the list. false: it is not a trusted 
        /// URL. 
        /// NOTES: A trusted url just check the domain/subdomain info. NOT touch whole address. e.g http://www.bbs.autowebkit.com/art/test....
        /// It will just touch the domain before the first '/', bbs.autowebkit.com
        /// </summary>
        /// <param name="url"></param>
        /// <param name="trustedURLs"></param>
        /// <returns></returns>
        public bool isTrustedURL(string url, List<string> trustedURLs) {
            if (url != null && trustedURLs != null) {
                string t = WebUtil.getDomainName(url);
                foreach (string tu in trustedURLs) {
                    string t1 = WebUtil.getDomainName(tu);
                    if (t == t1) {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Generate next version number based on the version, the new version is the current version's 
        /// last version number+1. or return default "1.0.0.0" if errors 
        /// 
        /// </summary>
        /// <param name="version">current version</param>
        /// <returns></returns>
        public string getNextVersion(string version) {
            string msg = checkVersion(version, null);
            if (msg == string.Empty) {
                string[] ns = version.Split('.');
                decimal dec = getDecimal(ns[3])+1;
                return ns[0] + "." + ns[1] + "." + ns[2] + "." + dec;
            }
            return "1.0.0.0" ;
        }
        /// <summary>
        /// If the new version is valid, return string.Empty, or return error msg. 
        /// NOTES: new version must be 4 digital number and bigger than the old version. 
        /// </summary>
        /// <param name="newver">new version</param>
        /// <param name="currVer">current version</param>
        /// <returns></returns>
        public string checkVersion(string newver, string currVer) {
            // 1. check whether the new version is valid format
            if (newver == null || newver.Length < 7) {
                return LangUtil.getMsg("valid.sroot.version.msg1"); // "New version is invalid, version format is like 1.0.0.1"
            } else { 
                string[] ns = newver.Split('.');
                if (ns.Length != 4) {
                    return LangUtil.getMsg("valid.sroot.version.msg2"); // "New version is invalid, version format should be 4 number fields."
                }
                foreach (string s in ns) {
                    decimal dec = getDecimal(s);
                    if (dec == decimal.MinValue) {
                        return LangUtil.getMsg("valid.sroot.version.msg3"); // "New version is invalid, version 4 fields must be number."
                    }
                }
                if (currVer == null || currVer.Length == 0) { // this is an valid version 
                    return string.Empty;
                } else {
                    string[] cs = currVer.Split('.');
                    for (int i = 0; i < 4; i++) {
                        decimal dn = getDecimal(ns[i]);
                        decimal dc = getDecimal(cs[i]);
                        if (dn > dc) {
                            return string.Empty;
                        }
                    }

                    return LangUtil.getMsg("valid.sroot.version.msg4");// "New version should be bigger than current version."
                }
            }
        }
        #endregion scriptRoot area
        #region WebElement Area
        /// <summary>
        /// update Script iframeWEList and internal refWElist if the WE has refered another refWE. 
        /// all the refWEs are added internally. 
        /// </summary>
        /// <param name="we"></param>
        public void updateSRootInternalWEs(WebElement we,ScriptRoot sroot) {
            if (we == null || we.refWebElement == null || sroot == null) {
                return;
            }
            WebElement tobeRefWE = we.refWebElement;
            WebElement existedRefWE = null;
            // update iframeWE list if necessary. 
            if (tobeRefWE.Tag == "iframe" || tobeRefWE.Tag == "frame") {
                foreach (WebElement frameWE in sroot.IFrames) {
                    if (frameWE.Name.Equals(tobeRefWE.Name)) {
                        existedRefWE = frameWE;
                        break;
                    }
                }
                if (existedRefWE == null) {
                    sroot.IFrames.AddUnique(tobeRefWE);
                }
            } else {
                foreach (WebElement tempRefWE in sroot.InternalRefWEs) {
                    if (tempRefWE.isEqual(tobeRefWE)) {
                        existedRefWE = tempRefWE;
                        break;
                    }
                }
                if (existedRefWE == null) {
                    sroot.InternalRefWEs.AddUnique(tobeRefWE);
                }
            }
            if (existedRefWE == null) {
                updateSRootInternalWEs(tobeRefWE.refWebElement,sroot);
            } else {
                we.refWebElement = existedRefWE;
            }
        }
        /// <summary>
        /// return the WebElementAttribute of webElement by attribute name, or null if errors
        /// </summary>
        /// <param name="webElement"></param>
        /// <param name="attKeyName">attribute key name </param>
        internal WebElementAttribute getWebElementAttributeByKeyName(WebElement webElement, string attKeyName) {
            if (webElement == null || attKeyName == null) {
                return null;
            }
            foreach (WebElementAttribute wea in webElement.Attributes) {
                if (attKeyName.Equals(wea.Name)) {
                    return wea;
                }
            }

            return null;
        }        
        /// <summary>
        /// whether the rvalue is a valid value for the wea
        /// </summary>
        /// <param name="wea"></param>
        /// <param name="rvalue">checking value for the wea</param>
        /// <returns></returns>
        internal bool isValidWEAValue(WebElementAttribute wea, string rvalue) {
            if (wea == null) {
                return false;
            }
            string rpvalue = wea.RealPValue;
            // this is used to handle the design time scenario, in runtime, the RealPValue will
            // be updated with PValue before usage. 
            if (rpvalue == null) {
                rpvalue = tryGetWEARealPatternValue(wea, null);
            }
            int result = ModelManager.Instance.checkResult(wea.PATTERN, rvalue, rpvalue);
            return result == 1;
        }        
        /// <summary>
        /// get the WebElement that is directly under WebElementGroup with the specified name, or null if not find
        /// </summary>
        /// <param name="weg"></param>
        /// <param name="name">WebElement name</param>
        /// <returns></returns>
        internal WebElement getDirectWebElementByName(WebElementGroup weg, string name) {
            return getDirectWebElementX(weg, name, "name") ;
        }        
        /// <summary>
        /// get first WebElement that is directly under the WebElementGroup with the specified attribute key
        /// or null if not found, it only check the direct WebElements under WebElementGroup
        /// </summary>
        /// <param name="weg"></param>
        /// <param name="targetAttValue">target text to match an attribute of the WebElement</param>
        /// <param name="attkey">value is the attribute key name</param>
        /// <returns></returns>
        private WebElement getDirectWebElementX(WebElementGroup weg, string targetAttValue, string attkey) {
            if (weg == null || targetAttValue == null || attkey == null) {
                return null;
            }
            // check the WebElement directly under the script
            foreach (WebElement we in weg.Elems) {
                WebElementAttribute wea = getWebElementAttributeByKeyName(we, attkey);
                if (wea != null) {
                    if (wea.Key.Equals(attkey) && isValidWEAValue(wea, targetAttValue)) {
                        return we;
                    }
                }                
            }            
            return null;
        }
        /// <summary>
        /// get the WebElement from the script root, or return null if not found, it will
        /// check all the WebElement hirachy tree in script
        /// </summary>
        /// <param name="parent">Parent WebElementGroup </param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal WebElement getWebElementByName(WebElementGroup parent, string name) {
            return getWebElementX(parent, name, "name");
        }        
        /// <summary>
        /// get first WebElement with the specified attribute in the WebElementGroup, or null if not 
        /// found 
        /// </summary>
        /// <param name="weg"></param>
        /// <param name="targetAttValue"></param>
        /// <param name="attkey"></param>
        /// <returns></returns>
        private WebElement getWebElementX(WebElementGroup weg, string targetAttValue, string attkey) {
            if (weg == null || targetAttValue == null || attkey == null) {
                return null;
            }
            // check the WebElement directly under the script
            foreach (WebElement we in weg.Elems) {
                WebElementAttribute wea = getWebElementAttributeByKeyName(we, attkey);
                if (wea != null) {
                    if (wea.Key.Equals(attkey) && isValidWEAValue(wea, targetAttValue)) {
                        return we;
                    }
                }     
            }
            // check the Web Element groups
            foreach (WebElementGroup grp in weg.SubGroups) {
                WebElement elem = getWebElementX(grp, targetAttValue, attkey);
                if (elem != null) {
                    return elem;
                }
            }

            return null;
        }
        /// <summary>
        /// How many times the WebElementGroup be referenced, all WebElements under the group will be referenced by
        /// WebElement, operaiont or condition
        /// 
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        public int getWEGRefTimes(WebElementGroup weg) {
            int count = 0;
            if (weg != null) {
                foreach (WebElementGroup tg in weg.SubGroups) {
                    count += getWEGRefTimes(tg);
                }
                foreach (WebElement we in weg.Elems) {
                    count += getWERefTimes(we);
                }
            }
            return count;
        }
        /// <summary>
        /// How many times the WebElements be referenced, a WebElement will be referenced by
        /// WebElement, operaiont or condition
        /// 
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        public int getWERefTimes(WebElement we) {
            int count = 0;
            if (we != null) {
                // check WebElements
                count += (we.WeakRef.Count - 1); // the -1 is the WE container references. 
                foreach (WebElementAttribute wea in we.Attributes) {
                    count += (wea.WeakRef.Count - 1);
                }
            }
            return count;
        }
        
        /// <summary>
        /// wethere the we is the parent of wea object( not value ), return true if we is the wea parent, or return false
        /// </summary>
        /// <param name="wea"></param>
        /// <param name="we"></param>
        /// <returns></returns>
        public bool isWEAParent(WebElementAttribute wea, WebElement we) {
            if (wea != null && we != null) {
                foreach (WebElementAttribute tea in we.Attributes) {
                    if (tea.Equals(wea)) {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// whether the wea object is an attribute of the WebElement that is under group element
        /// </summary>
        /// <param name="grp"></param>
        /// <param name="wea"></param>
        /// <returns></returns>
        internal WebElement getWebElement(WebElementGroup grp, WebElementAttribute wea) {
            foreach (WebElement we in grp.Elems) {
                if (isWEAParent(wea, we)) {
                    return we;
                }
            }
            foreach (WebElementGroup weg in grp.SubGroups) {
                WebElement we1 = getWebElement(weg,wea);
                if (we1 != null) {
                    return we1;
                }
            }
            return null;
        }
        /// <summary>
        /// clean all the attributes Real value of the WebElement as null 
        /// </summary>
        /// <param name="we"></param>
        internal void cleanWebElementAttributes(WebElement we) {
            if (we == null || !we.IsRealElement) {
                return;
            }
            we.IsRealElement = false;
            foreach (WebElementAttribute wea in we.Attributes) {
                wea.RValue = null;
                wea.RealPValue = null;
            }            
        }
        /// <summary>
        /// clean all referenced WebElementAttribute if have 
        /// </summary>
        /// <param name="opc"></param>
        internal void cleanWebElementAttributes(OpCondition opc) {
            if (opc != null) {
                ConditionGroup cgrp = opc.ConditionGroup;
                foreach(BaseElement be in cgrp.Conditions){
                    if (be is ConditionGroup) {
                        cleanWebElementAttributes(be as ConditionGroup);
                    } else if (be is Condition) {
                        cleanWebElementAttributes(be as Condition);
                    }
                }
            }
        }

        private void cleanWebElementAttributes(Condition con) {
            // handle input 1 
            if (con.Input1 is WebElement) {
                cleanWebElementAttributes(con.Input1 as WebElement);
            } else if (con.Input1 is WebElementAttribute) {
                WebElementAttribute wea = con.Input1 as WebElementAttribute;
                WebElement we = wea.Collection.Owner as WebElement;
                cleanWebElementAttributes(we);
            }
            // handle input 2 
            if (con.Input2 is WebElement) {
                cleanWebElementAttributes(con.Input2 as WebElement);
            } else if (con.Input2 is WebElementAttribute) {
                WebElementAttribute wea = con.Input2 as WebElementAttribute;
                WebElement we = wea.Collection.Owner as WebElement;
                cleanWebElementAttributes(we);
            }
        }

        private void cleanWebElementAttributes(ConditionGroup cgrp) {
            foreach (BaseElement be in cgrp.Conditions) {
                if (be is ConditionGroup) {
                    cleanWebElementAttributes(be as ConditionGroup);
                } else if (be is Condition) {
                    cleanWebElementAttributes(be as Condition);
                }
            }
        }
        /// <summary>
        /// return true if the Two we is ATTRIBUTE or CODE type, and they are modeled the same HtmlElement. or false if errors. 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tgt"></param>
        /// <returns></returns>
        internal bool isSameHTMLElementByATT(WebElement src, WebElement tgt) {
            if (src == null || tgt == null) {
                return false;
            }
            if (!(src.TYPE == WEType.ATTRIBUTE || src.TYPE == WEType.CODE) || !(tgt.TYPE == WEType.ATTRIBUTE || tgt.TYPE == WEType.CODE)) {
                return false;
            }
            if (src.Tag != tgt.Tag) {
                return false;
            }
            if (src.ID == null && tgt.ID != null) {
                return false;
            }
            if (src.ID != null && src.ID != tgt.ID) {
                return false;
            }
            if (src.refWebElement != null) {
                if (src.refWebElement != tgt.refWebElement) {
                    return false;
                }
            } else if (tgt.refWebElement != null) {
                return false;
            }

            foreach (WebElementAttribute wea in src.Attributes) {
                WebElementAttribute twea = this.getWebElementAttributeByKeyName(tgt, wea.Key);
                if (twea != null) {                    
                    if (isSameWEA(twea,wea) == false){//!(twea.PValues == wea.PValues && twea.PATTERN == wea.PATTERN)) {
                        return false;
                    }
                } else {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// check whether the wea1 and wea2 are modeled the same attribute. 
        /// </summary>
        /// <param name="wea1"></param>
        /// <param name="wea2"></param>
        /// <returns></returns>
        public bool isSameWEA(WebElementAttribute wea1, WebElementAttribute wea2) {
            if (wea1 != null && wea2 != null) {
                if (wea1.PATTERN != wea2.PATTERN || wea1.PValues.Count!=wea2.PValues.Count) {
                    return false;
                }
                
                for (int i = 0; i < wea1.PValues.Count; i++) {
                    object obj1 = wea1.PValues.get(i);
                    object obj2 = wea2.PValues.get(i);
                    if (obj1 is string && obj2 is string) { 
                        if( !obj1.ToString().Equals(obj2.ToString())){
                            return false ;
                        }
                    } else if (obj1 is Parameter && obj2 is Parameter) {
                        if (obj1 != obj2) {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Whether the WebElementAttriubte real value maybe a number in runtime. return false if it is not a number never.
        /// </summary>
        /// <param name="wea"></param>
        /// <returns></returns>
        private bool isWEAMaybeNumber(WebElementAttribute wea) {
            bool result = true;
            if (wea != null) {
                if (wea.PATTERN == CONDITION.STR_CONTAIN || wea.PATTERN == CONDITION.STR_ENDWIDTH || wea.PATTERN == CONDITION.STR_FULLMATCH || wea.PATTERN == CONDITION.STR_STARTWITH) {
                    string pv = tryGetWEARealPatternValue(wea, null);
                    decimal dec = getDecimal(pv);
                    if (dec == decimal.MinValue) {
                        result = false;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Get the WebElementAttriubte PValues text for UI desplay,return string.Empty if errors 
        /// </summary>
        /// <param name="wea"></param>
        /// <returns></returns>
        public string getWEAPValuesText(WebElementAttribute wea) {
            if (wea != null) {
                return getWEAPValuesText(wea.PValues.ToArray());
            }
            return string.Empty;
        }
        /// <summary>
        /// Get the WebElementAttriubte PValues text for UI desplay, objects[] is the PValues items. 
        /// return string.Empty if errors 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string getWEAPValuesText(object[] items) {
            if (items != null && items.Length > 0) {
                StringBuilder sb = new StringBuilder();
                foreach (object obj in items) {
                    if (obj is string) {
                        sb.Append(obj.ToString());
                    } else if (obj is Parameter) {
                        Parameter param = obj as Parameter;
                        sb.Append("{").Append(param.Name).Append("}");
                    }
                }
                if (sb.Length > 0) {
                    return sb.ToString();
                }
            }
            return string.Empty;
        }
        #endregion WebElement area 
        #region Operation area       
        /// <summary>
        /// return a random number between start and end number, the range is 
        /// minValue <= value < maxValue
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int randomNumber(int start, int end) {
            start = start > 0 ? start : 0;
            end = end > 0 ? end : 0;
            if (start > end) {
                int tmp = end;
                end = start;
                start = tmp;
            }
            return rnd.Next(start, end);
        }
        /// <summary>
        /// return a long value described in the vstr, for vstr format
        /// pls refer<see cref="OperationElement.WaitTime"/>
        /// accepted value format is 
        ///  a. number string ,e.g 20, 80
        ///  b. a range, format [number start] - [number end]. e.g 10 - 20, 12-50
        /// </summary>
        /// <param name="vstr">for vstr format pls refer<see cref="OperationElement.WaitTime"/></param>
        /// <returns>grt Zero number </returns>
        public int getWaitTimeValue(string vstr) {
            if (vstr == null || vstr.Length < 1) {
                ///LOG
                return 0;
            }
            int value = 0;
            int index = vstr.IndexOf('-');
            try {
                if (index > 0) { // handle range value 
                    string vss = vstr.Substring(0, index).Trim();
                    string vts = vstr.Substring(index + 1, vstr.Length - index - 1).Trim();
                    int vs = Convert.ToInt32(vss);
                    int vt = Convert.ToInt32(vts);
                    value = randomNumber(vs, vt);
                } else {
                    value = Convert.ToInt32(vstr);
                }
            } catch (FormatException) {
                ///LOG
                //Console.WriteLine(fe.Message);
                value = 1;
            }

            return value;
        }
        /// <summary>
        /// get the opration Log text, the text is used to show on the UI. 
        /// </summary>
        /// <param name="op"></param>
        public string getLogText(Operation op) { 
            //TODO
            return "";
        }
        /// <summary>
        /// get the operation log value string based on the logitems, items object can be 
        /// WebElement, WebElementAttribute, Parameters, and constant string 
        /// </summary>
        /// <param name="logItems"></param>
        /// <returns></returns>
        public string getLogValue(List<object> logItems) {
            //char stx = (char)0x03; // mark the field start 
            //char etx = (char)0x04; // mark the filed end 
            //TODO
            return "";
            
        }
        /// <summary>
        /// return the operation type text or OP_NONE msg if errors 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getOperationTypeText(OPERATION type) {
            string text = LangUtil.getMsg("OP_NOP");
            if (type == OPERATION.CLICK) {
                text = LangUtil.getMsg("OP_CLICK");
            } else if (type == OPERATION.END) {
                text = LangUtil.getMsg("OP_END");
            } else if (type == OPERATION.INPUT) {
                text = LangUtil.getMsg("OP_INPUT");
            } else if (type == OPERATION.NOP) {
                text = LangUtil.getMsg("OP_NOP");
            } else if (type == OPERATION.OPEN_URL_N_T) {
                text = LangUtil.getMsg("OP_OPEN_URL_N_T");
            } else if (type == OPERATION.OPEN_URL_T) {
                text = LangUtil.getMsg("OP_OPEN_URL_T");
            } else if (type == OPERATION.PROCESS) {
                text = LangUtil.getMsg("OP_PROCESS");
            } else if (type == OPERATION.REFRESH) {
                text = LangUtil.getMsg("OP_REFRESH");
            } else if (type == OPERATION.START) {
                text = LangUtil.getMsg("OP_START");
            }
            
            return text;
        }

        public List<string> getOperationInputTypes() {
            List<string> list = new List<string>();
            list.Add(LangUtil.getMsg("Op.Input.String"));
            list.Add(LangUtil.getMsg("Op.Input.Parameter"));
            return list;
        }

        public List<string> getOpenURLTypes() {
            List<string> list = new List<string>();
            list.Add(LangUtil.getMsg("OP_OPEN_URL_T"));
            // Comments : not support open in new window in this version 
            //list.Add(LangUtil.getMsg("OP_OPEN_URL_N_T"));
            return list;
        }
        /// <summary>
        /// Get the owner process of op, op can be an Operation/Process.
        /// return null if no owner process. 
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public Process getOwnerProc(Operation op) {
            if (op is Process) {
                Process proc = op as Process;
                if(proc.Collection!=null && proc.Collection.Owner !=null){
                    return proc.Collection.Owner as Process;
                }
            } else if (op is Operation) {
                if (op.Collection != null && op.Collection.Owner != null) {
                    return op.Collection.Owner as Process;
                }
            }

            return null;
        }
        /// <summary>
        /// Get the owner Operation/Process of opc, return null if not found 
        /// </summary>
        /// <param name="opc"></param>
        /// <returns></returns>
        public Operation getOwnerOp(OpCondition opc) {
            if (opc != null && opc.Collection != null && opc.Collection.Owner is Operation) {
                return opc.Collection.Owner as Operation;
            }
            return null;
        }
        /// <summary>
        /// Get the owner Operation/Process of rule, return null if not found 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public Operation getOwnerOp(OperationRule rule) {
            if (rule != null && rule.Collection != null && rule.Collection.Owner is Operation) {
                return rule.Collection.Owner as Operation;
            }
            return null;
        }
        /// <summary>
        /// Get the owner Operation/Process of update commands, return null if not found 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public Operation getOwnerOp(ParamCmd cmd) {
            if (cmd != null && cmd.Collection != null && cmd.Collection.Owner is Operation) {
                return cmd.Collection.Owner as Operation;
            }
            return null;
        }
        #endregion Operation area
        #region CondtionArea   
        /// <summary>
        /// 1: means return the condition is true.
        /// 0: means the condition is false.
        /// -1: means there are some errors that cannot check the result. 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input1">input can be WebElement, WebElementAttribute, Parameter, or String</param>
        /// <param name="input2"></param>
        /// <returns></returns>
        internal int checkResult(CONDITION pattern, Object input1, Object input2) {
            int result = -1;
            if (pattern == CONDITION.EMPTY || input1 == null) {
                result = 0;
            }
            switch (pattern) { 
                case CONDITION.INPUT_EXISTED:
                    if (input1 is WebElement) {
                        WebElement we = input1 as WebElement;
                        result = we.IsRealElement ? 1 : 0;
                    } else {
                        result = 0;
                    }                 
                    break;
                case CONDITION.NUM_BIGGER:
                case CONDITION.NUM_EQ_BIGGER:
                case CONDITION.NUM_EQ_LESS:
                case CONDITION.NUM_EQUAL:
                case CONDITION.NUM_LESS:
                case CONDITION.NUM_NOT_EQUAL:
                    result = checkNumberPattern(pattern, input1, input2);
                    break;                
                case CONDITION.STR_FULLMATCH:
                case CONDITION.STR_NOT_FULLMATCH:
                case CONDITION.STR_CONTAIN:
                case CONDITION.STR_NOT_CONTAIN:                
                case CONDITION.STR_STARTWITH:
                case CONDITION.STR_NOT_STARTWITH:
                case CONDITION.STR_ENDWIDTH:
                case CONDITION.STR_NOT_ENDWIDTH:
                    result = checkStrPattern(pattern, input1, input2);
                    break;
                case CONDITION.SET_CONTAIN:                    
                case CONDITION.SET_EXCLUDE:
                    result = checkSetPattern(pattern, input1, input2);
                    break;
                case CONDITION.DATETIME_AFTER:
                case CONDITION.DATETIME_BEFORE:
                case CONDITION.DATETIME_EQUAL:
                    result = checkDateTimePattern(pattern, input1, input2);
                    break;
            }
            return result;
        }

        private int checkDateTimePattern(CONDITION pattern, object input1, object input2) {
            if (input1 != null || input2 != null) {
                DateTime d1 = getDateTime(input1);
                DateTime d2 = getDateTime(input2);
                if (d1 != DateTime.MinValue && d2 != DateTime.MinValue) {
                    switch (pattern) {
                        case CONDITION.DATETIME_AFTER:
                            return d1 > d2 ? 1 : 0;
                        case CONDITION.DATETIME_BEFORE:
                            return d1 < d2 ? 1 : 0;
                        case CONDITION.DATETIME_EQUAL:
                            return d1 == d2 ? 1 : 0;
                    }
                }
            }
            return -1;
        }
        
        private int checkSetPattern(CONDITION pattern, object input1, object input2) {
            if (input2 == null) {
                return -1;
            }
            if (input1 is Parameter) {
                Parameter param = input1 as Parameter;
                List<object> set = param.DesignSet;
                if (param.RealSet.Count >0 ) {
                    set = param.RealSet;
                }
                bool find = false;
                foreach (object obj in set) {
                    if (obj is string || obj is decimal) {
                        if (input2.ToString().Equals(obj.ToString())) {
                            find = true;
                        }
                    } else if (obj is WebElementAttribute) {
                        WebElementAttribute wea = obj as WebElementAttribute;
                        if (input2.ToString().Equals(wea.RValue)) {
                            find = true;
                        }
                    }
                }
                if (pattern == CONDITION.SET_CONTAIN) {
                    return find ? 1 : 0;
                } else if (pattern == CONDITION.SET_EXCLUDE) {
                    return find ? 0 : 1;
                }
            }
            return -1;
        }

        private int checkStrPattern(CONDITION pattern, object input1, object input2) {
            string str1 = getConditionInputAsStr(input1);
            string str2 = getConditionInputAsStr(input2);
            if (str1 == null || str2 == null) {
                return -1;
            }
            int result = -1;
            switch (pattern) {
                case CONDITION.STR_FULLMATCH:
                    result = str1.Equals(str2) ? 1 : 0;
                    break;
                case CONDITION.STR_NOT_FULLMATCH:
                    result = str1.Equals(str2) ? 0 : 1;
                    break;
                case CONDITION.STR_CONTAIN:
                    result = str1.Contains(str2) ? 1 : 0;
                    break;
                case CONDITION.STR_NOT_CONTAIN:
                    result = str1.Contains(str2) ? 0 : 1;
                    break;
                case CONDITION.STR_STARTWITH:
                    result = str1.StartsWith(str2) ? 1 : 0;
                    break;
                case CONDITION.STR_NOT_STARTWITH:
                    result = str1.StartsWith(str2) ? 0 : 1;
                    break;
                case CONDITION.STR_ENDWIDTH:
                    result = str1.EndsWith(str2) ? 1 : 0;
                    break;
                case CONDITION.STR_NOT_ENDWIDTH:
                    result = str1.EndsWith(str2) ? 0 : 1;
                    break;                
            }
            // if Result is false, check whether it is the http://www.aaa... or http://aaa... pattern
            if (result == 0) {
                bool flag = false;
                string http = "http://www.";
                if ((str1.StartsWith(http) && !str2.StartsWith(http)) 
                    || (str2.StartsWith(http) && !str1.StartsWith(http))) {                    
                    flag = true;
                    str1 = WebUtil.getPureAddress(str1);
                    str2 = WebUtil.getPureAddress(str2);
                }
                // If one of the input matched the pattern, just re-check with the new value. 
                if (flag == true) {
                    result = checkStrPattern(pattern, str1, str2);
                }
            }
            return result;
        }
        /// <summary>
        /// handle the number patten
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns></returns>
        private int checkNumberPattern(CONDITION pattern, object input1, object input2) {
            decimal n1 = getConInputAsNumber(input1);            
            decimal n2 = getConInputAsNumber(input2);
            if (n1 == decimal.MinValue || n2 == decimal.MinValue) {
                return -1;
            }
            switch (pattern) {
                case CONDITION.NUM_BIGGER:
                    return n1 > n2 ? 1 : 0;
                case CONDITION.NUM_EQ_BIGGER:
                    return n1 >= n2 ? 1 : 0;
                case CONDITION.NUM_EQ_LESS:
                    return n1 <= n2 ? 1 : 0;
                case CONDITION.NUM_EQUAL:
                    return n1 == n2 ? 1 : 0;
                case CONDITION.NUM_LESS:
                    return n1 < n2 ? 1 : 0;
                case CONDITION.NUM_NOT_EQUAL:
                    return n1 != n2 ? 1 : 0;
            }
            return -1;
        }
        /// <summary>
        /// Get the condition input value as a decimal if it is a number or decimal.MinValue if errors.
        /// input can be WebElementAttribute, Parameter(with number type) or string constant.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private decimal getConInputAsNumber(object input) {
            object obj = null;
            if (input is WebElementAttribute) {
                WebElementAttribute wea = input as WebElementAttribute;
                obj = wea.RValue;
            } else if (input is Parameter) {
                Parameter param = input as Parameter;
                if (param.Type == ParamType.NUMBER) {                    
                    obj = param.DesignValue;
                    if (param.RealValue != null) {
                        obj = param.RealValue;
                    }
                }
            } else if (input is string) {
                obj = input;
            }
            
            if (obj != null) {
                try {
                    decimal v = Convert.ToDecimal(obj);
                    return v;
                } catch (Exception) { }
            }            
            return decimal.MinValue;
        }
        /// <summary>
        /// Get the condition input value as a string or null if errors.
        /// input can be WebElementAttribute, Parameter(with string type) or string constant.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string getConditionInputAsStr(object input) {
            object obj = null;
            if (input is WebElementAttribute) {
                WebElementAttribute wea = input as WebElementAttribute;
                obj = wea.RValue;
            } else if (input is Parameter) {
                Parameter param = input as Parameter;
                if (param.Type == ParamType.STRING) {
                    obj = param.DesignValue;
                    if (param.RealValue != null) {
                        obj = param.RealValue;
                    }
                }
            } else if (input is string) {
                obj = input;
            }
            if (obj != null) {
                return obj.ToString();
            } else {
                return null;
            }            
        }
        
        /// <summary>
        /// check result for the ConditionGroup, and it will update the grp.IsChecked Flag
        /// </summary>
        /// <param name="grp">ConditionGroup </param>
        /// <returns></returns>        
        public bool checkResult(ConditionGroup grp){
            if (grp == null || grp.Conditions.Count == 0) {
                return false;
            }
            if (grp.IsChecked) {
                return grp.Result;
            }

            CONDITION relation = grp.Relation;
            BEList<BaseElement> conditions = grp.Conditions;
            bool isChecked = true;
            for (int i = 0; i < conditions.Count; i++) {
                bool itemChecked = false;
                bool itemResult = false;
                if (conditions.get(i) is Condition) {
                    Condition con = (Condition)conditions.get(i);
                    itemChecked = con.IsChecked;
                    itemResult = con.Result;
                } else if (conditions.get(i) is ConditionGroup) {
                    ConditionGroup congrp = (ConditionGroup)conditions.get(i);
                    itemChecked = congrp.IsChecked;
                    itemResult = congrp.Result;
                }
                // there at least one item can not be checked. 
                if (isChecked == true && itemChecked == false) {
                    isChecked = false;
                }

                if (relation == CONDITION.OR) {
                    if (itemChecked && itemResult) {
                        grp.IsChecked = true;
                        return true;                        
                    }
                }
                if (relation == CONDITION.AND) {
                    if (itemChecked && itemResult == false) {
                        grp.IsChecked = true;
                        return false;                        
                    }                    
                }
            }
            grp.IsChecked = isChecked;            
            if (isChecked && relation == CONDITION.AND) {
                return true;
            }
            return false;            
        }
        /// <summary>
        /// get the localed condition catagory info, or empty list if not find
        /// </summary>
        /// <returns></returns>
        public List<string> getConditonCatagory() {
            List<string> list = new List<string>();
            list.Add(LangUtil.getMsg("con.category.str"));
            list.Add(LangUtil.getMsg("con.category.num"));
            list.Add(LangUtil.getMsg("con.category.obj"));
            list.Add(LangUtil.getMsg("con.category.set"));
            return list;
        }
        /// <summary>
        /// return pattern by the condition display text(with locale) or CONDITION.EMPTY if errors
        /// </summary>
        /// <param name="text">pattern display text</param>
        /// <returns></returns>
        public CONDITION getPattern(string text) {
            CONDITION pattern = CONDITION.EMPTY;
            if (text != null) {
                if (text.Equals(LangUtil.getMsg("CONDITION.STR_FULLMATCH"))) {
                    pattern = CONDITION.STR_FULLMATCH;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_NOT_FULLMATCH"))) {
                    pattern = CONDITION.STR_NOT_FULLMATCH;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_CONTAIN"))) {
                    pattern = CONDITION.STR_CONTAIN;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_NOT_CONTAIN"))) {
                    pattern = CONDITION.STR_NOT_CONTAIN ;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_STARTWITH"))) {
                    pattern = CONDITION.STR_STARTWITH;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_NOT_STARTWITH"))) {
                    pattern = CONDITION.STR_NOT_STARTWITH;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_ENDWIDTH"))) {
                    pattern = CONDITION.STR_ENDWIDTH;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.STR_NOT_ENDWIDTH"))) {
                    pattern = CONDITION.STR_NOT_ENDWIDTH;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.NUM_EQUAL"))) {
                    pattern = CONDITION.NUM_EQUAL;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.NUM_NOT_EQUAL"))) {
                    pattern = CONDITION.NUM_NOT_EQUAL;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.NUM_BIGGER"))) {
                    pattern = CONDITION.NUM_BIGGER;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.NUM_LESS"))) {
                    pattern = CONDITION.NUM_LESS;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.NUM_EQ_BIGGER"))) {
                    pattern = CONDITION.NUM_EQ_BIGGER;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.NUM_EQ_LESS"))) {
                    pattern = CONDITION.NUM_EQ_LESS;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.INPUT_EXISTED"))) {
                    pattern = CONDITION.INPUT_EXISTED;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.SET_CONTAIN"))) {
                    pattern = CONDITION.SET_CONTAIN;
                } else if (text.Equals(LangUtil.getMsg("CONDITION.SET_EXCLUDE"))) {
                    pattern = CONDITION.SET_EXCLUDE;
                }
            }

            return pattern;
        }
        /// <summary>
        /// Get pattern text with current locale or string.empty if errors 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string getPatternText(CONDITION pattern) {
            string text = string.Empty;
            if (pattern.Equals(CONDITION.STR_FULLMATCH)) {
                text = LangUtil.getMsg("CONDITION.STR_FULLMATCH");
            } if (pattern.Equals(CONDITION.STR_NOT_FULLMATCH)) {
                text = LangUtil.getMsg("CONDITION.STR_NOT_FULLMATCH");
            } else if (pattern.Equals(CONDITION.STR_CONTAIN)) {
                text = LangUtil.getMsg("CONDITION.STR_CONTAIN");
            } else if (pattern.Equals(CONDITION.STR_NOT_CONTAIN)) {
                text = LangUtil.getMsg("CONDITION.STR_NOT_CONTAIN");
            } else if (pattern.Equals(CONDITION.STR_STARTWITH)) {
                text = LangUtil.getMsg("CONDITION.STR_STARTWITH");
            } else if (pattern.Equals(CONDITION.STR_NOT_STARTWITH)) {
                text = LangUtil.getMsg("CONDITION.STR_NOT_STARTWITH");
            } else if (pattern.Equals(CONDITION.STR_ENDWIDTH)) {
                text = LangUtil.getMsg("CONDITION.STR_ENDWIDTH");
            } else if (pattern.Equals(CONDITION.STR_NOT_ENDWIDTH)) {
                text = LangUtil.getMsg("CONDITION.STR_NOT_ENDWIDTH");
            } else if (pattern.Equals(CONDITION.NUM_EQUAL)) {
                text = LangUtil.getMsg("CONDITION.NUM_EQUAL");
            } else if (pattern.Equals(CONDITION.NUM_NOT_EQUAL)) {
                text = LangUtil.getMsg("CONDITION.NUM_NOT_EQUAL");
            } else if (pattern.Equals(CONDITION.NUM_BIGGER)) {
                text = LangUtil.getMsg("CONDITION.NUM_BIGGER");
            } else if (pattern.Equals(CONDITION.NUM_LESS)) {
                text = LangUtil.getMsg("CONDITION.NUM_LESS");
            } else if (pattern.Equals(CONDITION.NUM_EQ_BIGGER)) {
                text = LangUtil.getMsg("CONDITION.NUM_EQ_BIGGER");
            } else if (pattern.Equals(CONDITION.NUM_EQ_LESS)) {
                text = LangUtil.getMsg("CONDITION.NUM_EQ_LESS");
            } else if (pattern.Equals(CONDITION.INPUT_EXISTED)) {
                text = LangUtil.getMsg("CONDITION.INPUT_EXISTED");
            } else if (pattern.Equals(CONDITION.SET_CONTAIN)) {
                text = LangUtil.getMsg("CONDITION.SET_CONTAIN");
            } else if (pattern.Equals(CONDITION.SET_EXCLUDE)) {
                text = LangUtil.getMsg("CONDITION.SET_EXCLUDE");
            }

            return text;
        }
        /// <summary>
        /// return array(length=8). 
        /// </summary>
        /// <returns></returns>
        public string[] getStringPatterns() {
            string[] ss = new string[8];
            ss[0] = getPatternText(CONDITION.STR_FULLMATCH);
            ss[1] = getPatternText(CONDITION.STR_NOT_FULLMATCH);
            ss[2] = getPatternText(CONDITION.STR_CONTAIN);
            ss[3] = getPatternText(CONDITION.STR_NOT_CONTAIN);
            ss[4] = getPatternText(CONDITION.STR_STARTWITH);
            ss[5] = getPatternText(CONDITION.STR_NOT_STARTWITH);
            ss[6] = getPatternText(CONDITION.STR_ENDWIDTH);
            ss[7] = getPatternText(CONDITION.STR_NOT_ENDWIDTH);

            return ss;
        }
        /// <summary>
        /// return true if text means a string pattern, and update the compare with proper value 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool isStringPattern(string text, ref CONDITION compare) {
            bool result = false;
            if (text != null) {
                if (text.Equals(getPatternText(CONDITION.STR_FULLMATCH))) {
                    compare = CONDITION.STR_FULLMATCH ;
                    result = true ;
                } if (text.Equals(getPatternText(CONDITION.STR_NOT_FULLMATCH))) {
                    compare = CONDITION.STR_NOT_FULLMATCH;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.STR_CONTAIN))) {
                    compare = CONDITION.STR_CONTAIN ;
                    result = true ;
                } else if (text.Equals(getPatternText(CONDITION.STR_NOT_CONTAIN))) {
                    compare = CONDITION.STR_NOT_CONTAIN;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.STR_STARTWITH))) {
                    compare = CONDITION.STR_STARTWITH;
                    result = true ;
                } else if (text.Equals(getPatternText(CONDITION.STR_NOT_STARTWITH))) {
                    compare = CONDITION.STR_NOT_STARTWITH;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.STR_ENDWIDTH))) {
                    compare = CONDITION.STR_ENDWIDTH;
                    result = true ;
                } else if (text.Equals(getPatternText(CONDITION.STR_NOT_ENDWIDTH))) {
                    compare = CONDITION.STR_NOT_ENDWIDTH;
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// return true the pattern is a string pattern  
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool isStringPattern(CONDITION pattern) {            
            if(pattern == CONDITION.STR_FULLMATCH || pattern == CONDITION.STR_NOT_FULLMATCH 
                || pattern == CONDITION.STR_CONTAIN || pattern == CONDITION.STR_NOT_CONTAIN
                || pattern == CONDITION.STR_STARTWITH || pattern == CONDITION.STR_NOT_STARTWITH
                || pattern == CONDITION.STR_ENDWIDTH || pattern == CONDITION.STR_NOT_ENDWIDTH){
                return true ;
            }                
            return false;
        }
        /// <summary>
        /// return array(length=6). 
        /// </summary>
        /// <returns></returns>
        public string[] getNumberPatterns() {
            string[] ss = new string[6];
            ss[0] = getPatternText(CONDITION.NUM_EQUAL);
            ss[1] = getPatternText(CONDITION.NUM_NOT_EQUAL);
            ss[2] = getPatternText(CONDITION.NUM_BIGGER);
            ss[3] = getPatternText(CONDITION.NUM_LESS);
            ss[4] = getPatternText(CONDITION.NUM_EQ_BIGGER);
            ss[5] = getPatternText(CONDITION.NUM_EQ_LESS);
            return ss;
        }
        /// <summary>
        /// return true if text means a number pattern, and update the compare with proper value 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool isNumberPattern(string text, ref CONDITION compare) {
            bool result = false;
            if (text != null) {
                if (text.Equals(getPatternText(CONDITION.NUM_EQUAL))) {
                    compare = CONDITION.NUM_EQUAL;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.NUM_NOT_EQUAL))) {
                    compare = CONDITION.NUM_NOT_EQUAL;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.NUM_BIGGER))) {
                    compare = CONDITION.NUM_BIGGER;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.NUM_LESS))) {
                    compare = CONDITION.NUM_LESS;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.NUM_EQ_BIGGER))) {
                    compare = CONDITION.NUM_EQ_BIGGER;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.NUM_EQ_LESS))) {
                    compare = CONDITION.NUM_EQ_LESS;
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// return if the pattern is a number pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool isNumberPattern(CONDITION pattern) {            
            if(pattern == CONDITION.NUM_EQUAL || pattern == CONDITION.NUM_NOT_EQUAL 
                || pattern == CONDITION.NUM_BIGGER || pattern == CONDITION.NUM_LESS 
                || pattern == CONDITION.NUM_EQ_BIGGER || pattern == CONDITION.NUM_EQ_LESS){
                
                return true;                
            }
            return false;
        }

        private bool isDateTimePattern(CONDITION pattern) {
            if (pattern == CONDITION.DATETIME_AFTER || pattern == CONDITION.DATETIME_BEFORE || pattern == CONDITION.DATETIME_EQUAL) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// return array(length=1). 
        /// </summary>
        /// <returns></returns>
        public string[] getObjPatterns() {
            string[] ss = new string[1];
            ss[0] = getPatternText(CONDITION.INPUT_EXISTED);
            return ss;
        }
        /// <summary>
        /// return true if text means a object pattern, and update the compare with proper value 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool isObjPattern(string text, ref CONDITION compare) {
            bool result = false;
            if (text != null) {
                if (text.Equals(getPatternText(CONDITION.INPUT_EXISTED))) {
                    compare = CONDITION.INPUT_EXISTED;
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// return true the pattern is a object pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool isObjPattern(CONDITION pattern) {
            if (pattern == CONDITION.INPUT_EXISTED){
                return true;
            }
            return false;
        }
        /// <summary>
        /// return array(length=2). 
        /// </summary>
        /// <returns></returns>
        public string[] getSetPatterns() { 
            string[] ss = new string[2];
            ss[0] = getPatternText(CONDITION.SET_CONTAIN);
            ss[1] = getPatternText(CONDITION.SET_EXCLUDE);
            return ss;
        }
        /// <summary>
        /// return true if text means a set pattern, and update the compare with proper value 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool isSetPattern(string text, ref CONDITION compare) {
            bool result = false;
            if (text != null) {
                if (text.Equals(getPatternText(CONDITION.SET_CONTAIN))) {
                    compare = CONDITION.SET_CONTAIN;
                    result = true;
                } else if (text.Equals(getPatternText(CONDITION.SET_EXCLUDE))) {
                    compare = CONDITION.SET_EXCLUDE;
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// return true if it is a Set pattern 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool isSetPattern(CONDITION pattern) {
            if ( pattern == CONDITION.SET_CONTAIN  || pattern == CONDITION.SET_EXCLUDE){
                return true;
            }
            return false;
        }
        /// <summary>
        /// Reset all the Condition/ConditionGroup IsChecked/Result properties. 
        /// </summary>
        /// <param name="op"></param>
        internal void resetOpConditions(Operation op) {
            if (op != null) {
                foreach (OpCondition opc in op.OpConditions) {
                    resetConditionGroup(opc.ConditionGroup);
                }
            }
        }
        /// <summary>
        /// Reset all the Condition/ConditionGroup IsChecked/Result properties. 
        /// </summary>
        /// <param name="conGrp"></param>
        private void resetConditionGroup(ConditionGroup conGrp) {
            if (conGrp != null) {
                conGrp.reset();
                foreach (BaseElement be in conGrp.Conditions) {
                    if (be is Condition) {
                        Condition con = be as Condition;
                        con.reset();
                    } else if (be is ConditionGroup) {
                        ConditionGroup grp = be as ConditionGroup;                        
                        resetConditionGroup(grp);
                    }
                }
            }
        }
        #endregion CondtionArea
        #region Rule area        
        /// <summary>
        /// build default rules for script 
        /// </summary>
        /// <param name="procRoot">root process </param>
        internal void buildScriptDefaultRules(Process procRoot) {
            if (procRoot == null) {
                return;
            }            
            List<object> paramlist = new List<object>();
            
            paramlist.Add(Constants.WE_CHECK_TIMEOUT);
            OperationRule r1 = createRule(RuleTrigger.NULL_ELEMENT,RuleAction.WaitUntilElemFind,paramlist);
            r1.Name = "Null Element";
            procRoot.Rules.AddUnique(r1);

            paramlist.Clear();
            paramlist.Add(Constants.WE_CHECK_TIMEOUT);
            OperationRule r2 = createRule(RuleTrigger.OP_EXE_ERROR, RuleAction.RestartScript, paramlist);
            r2.Name = "Operation Error";
            procRoot.Rules.AddUnique(r2);

            paramlist.Clear();
            paramlist.Add(Constants.WE_CHECK_TIMEOUT);
            OperationRule r3 = createRule(RuleTrigger.REQ_TIMEOUT, RuleAction.WaitUntilElemFind, paramlist);
            r3.Name = "Request Timeout";
            procRoot.Rules.AddUnique(r3);

            paramlist.Clear();
            paramlist.Add(procRoot.StartOp);
            OperationRule r4 = createRule(RuleTrigger.NO_NEXT_OP_FOUND, RuleAction.Goto_Operation, paramlist);
            r3.Name = "No Next Op Found";
            procRoot.Rules.AddUnique(r4);
        }

        private static OperationRule createRule(RuleTrigger trigger, RuleAction action, List<object> paramlist) {
            OperationRule rule = ModelFactory.createOperationRule();
            rule.Trigger = trigger;
            rule.Action = action;
            rule.Params.AddRange(paramlist);
            
            return rule;
        }
        /// <summary>
        /// return localed rule trigger text or INVALID trigger text if errors 
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public string getRuleTriggerText(RuleTrigger trigger) {
            string text = LangUtil.getMsg("RuleTrigger.INVALID"); ;
            if (trigger == RuleTrigger.NULL_ELEMENT) {
                text = LangUtil.getMsg("RuleTrigger.NULL_ELEMENT");
            } else if (trigger == RuleTrigger.OP_EXE_ERROR) {
                text = LangUtil.getMsg("RuleTrigger.OP_EXE_ERROR");
            } else if (trigger == RuleTrigger.REQ_TIMEOUT) {
                text = LangUtil.getMsg("RuleTrigger.REQ_TIMEOUT");
            } else if (trigger == RuleTrigger.NO_NEXT_OP_FOUND) {
                text = LangUtil.getMsg("RuleTrigger.NO_NEXT_OP_FOUND");
            }
            return text;
        }        
        /// <summary>
        /// Return true if the rule is valid one for rules list. or return false. 
        /// Valid means that the there is no rule in the list has the same trigger and action with target rule
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool isValidRuleForList(List<OperationRule> rules, OperationRule rule) {
            if (rules != null && rule != null) {
                bool find = false;
                foreach (OperationRule r in rules) {
                    if (rule.isEqual(r)) {
                        find = true;
                        break;
                    }
                }
                return !find;
            }

            return false;
        }
        #endregion Rule area
        #region Parameters
        /// <summary>
        /// get the SetItem value string or return string.Empty if errors 
        /// </summary>
        /// <param name="obj">string, number, Parameter, WebElementAttribute</param>
        /// <returns></returns>
        public string getSetItemValue(object obj) {
            if (obj == null) {
                return string.Empty;
            }
            if (obj is string) {
                return obj as string;
            } else if (obj is decimal) {
                return obj.ToString();
            } else if (obj is Parameter) {
                Parameter param = obj as Parameter;
                return param.ToString();
            } else if (obj is WebElementAttribute) {
                WebElementAttribute wea = obj as WebElementAttribute;
                return wea.Name;
            }

            return obj.ToString();
        }
        /// <summary>
        /// Get parameter type text with current locale or empty string if errors 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getParamTypeText(ParamType type) {
            string text = string.Empty;
            if (type == ParamType.FILE) {
                text = LangUtil.getMsg("ParamType.FILE");
            } else if (type == ParamType.NUMBER) {
                text = LangUtil.getMsg("ParamType.NUMBER");
            } else if (type == ParamType.SET) {
                text = LangUtil.getMsg("ParamType.SET");
            } else if (type == ParamType.STRING) {
                text = LangUtil.getMsg("ParamType.STRING");
            } else if (type == ParamType.DATETIME) {
                text = LangUtil.getMsg("ParamType.DATETIME");
            }
            return text;
        }
        /// <summary>
        /// Get parameter type by locale text or ParamType.STRING as default 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ParamType getParamTypeByText(string text) {
            ParamType type = ParamType.STRING;            
            if (LangUtil.getMsg("ParamType.FILE").Equals(text)){
                type = ParamType.FILE ;
            }else if(LangUtil.getMsg("ParamType.NUMBER").Equals(text)){
                type = ParamType.NUMBER ;
            } else if(LangUtil.getMsg("ParamType.SET").Equals(text)){
                type = ParamType.SET ;
            } else if(LangUtil.getMsg("ParamType.STRING").Equals(text)){
                type = ParamType.STRING;
            } else if (LangUtil.getMsg("ParamType.DATETIME").Equals(text)) {
                type = ParamType.DATETIME;
            }
            return type;
        }
        /// <summary>
        /// Get parameter SetAccess text with current locale or empty string if errors 
        /// </summary>
        /// <param name="acess"></param>
        /// <returns></returns>
        public string getSetAccessText(SET_ACCESS access) {
            string text = string.Empty;
            if (access == SET_ACCESS.LOOP) {
                text = LangUtil.getMsg("SET_ACCESS.LOOP");
            } else if (access == SET_ACCESS.RANDOM) {
                text = LangUtil.getMsg("SET_ACCESS.RANDOM");
            } else if (access == SET_ACCESS.RANDOM_NO_DUPLICATE) {
                text = LangUtil.getMsg("SET_ACCESS.RANDOM_NO_DUPLICATE");
            }
            return text;
        }
        /// <summary>
        /// initialize the parameters real value with design value (if isInit==true), 
        /// or clean the real value as null if isInit==false
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isInit"></param>
        internal void initRealValue(Parameter param, bool isInit) {
            if (param == null) {
                return;
            }
            if (param.RealValue != null) {
                param.RealValue = null;
            }
            if (param.RealSet != null) {
                param.RealSet.Clear();
            }
            if (isInit) {
                // reset internal flags.
                param.reset();
                if (param.Type == ParamType.STRING || param.Type == ParamType.NUMBER || param.Type == ParamType.DATETIME) {
                    param.RealValue = param.DesignValue;
                } else if (param.Type == ParamType.SET) {
                    if (param.DesignSet != null) {                        
                        param.RealSet.Clear();
                        foreach (object obj in param.DesignSet) {
                            param.RealSet.Add(obj);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// True: the process has defined at least one parameter, false : none parameters defined. 
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public bool hasParameter(Process proc) {
            if (proc != null) {
                if (hasParameter(proc.ParamPublic)) {
                    return true;
                }
                if (hasParameter(proc.ParamPrivate)) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// True: whether there is at least one parmaeter defined under the paramGroup or its sub groups. else return false . 
        /// </summary>
        /// <param name="paramGroup"></param>
        /// <returns></returns>
        public bool hasParameter(ParamGroup paramGroup) {
            if (paramGroup != null) {
                if (paramGroup.Params.Count > 0) {
                    return true;
                } else {
                    foreach (ParamGroup grp in paramGroup.SubGroups) {
                        if (hasParameter(grp)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion Parameters
        #region logInfo area
        /// <summary>
        /// Get user log color, if the color is UserLogColor.DEFAULT_COLOR, it will use the script default color. 
        /// else it will use the parameter color. It will return Color.Black as default. 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="key">Log user key defined in Constants.cs</param>
        /// <param name="sroot"></param>
        /// <returns></returns>
        public Color getLogColor(int color, string key, ScriptRoot sroot) {
            int argb = getLogColorArgb(color, key, sroot);
            return Color.FromArgb(argb);
        }
        /// <summary>
        /// Get user log color argb value, if the color is UserLogColor.DEFAULT_COLOR, it will use the script default color. 
        /// else it will use the parameter color. It will return Color.Black argb as default. 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="key">Log user key defined in Constants.cs</param>
        /// <param name="sroot"></param>
        /// <returns></returns>
        public int getLogColorArgb(int color, string key, ScriptRoot sroot) {
            // use the script level default user log color. 
            if (color == UserLogItem.DEFAULT_COLOR) {
                if (sroot != null) {
                    foreach (DictionaryEntry de in sroot.ColorMap) {
                        if (key == de.Key.ToString()) {
                            try {
                                int clr = int.Parse(de.Value.ToString());
                                return clr;
                            } catch (Exception) {
                                return -16777216;//Color.Black;
                            }
                        }
                    }
                }
            } else {
                return color;
            }

            return -16777216;//Color.Black;
        }        
        #endregion log info
        #region util classes
        /// <summary>
        /// Hash an input string and return the hash as a 32 character hexadecimal string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public  string getMd5Hash(string input) {            
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        /// <summary>
        /// get image for the script
        /// </summary>
        /// <param name="p">image path</param>
        /// <returns></returns>
        public System.Drawing.Image getImage(string p) {
            //TODO ...
            return SystemIcons.WinLogo.ToBitmap();
        }
        /// <summary>
        /// Get a unique name for the BaseElement if it is added into the list, based on its current name.
        /// 
        /// </summary>
        /// <param name="tobelist">target to be element list </param>
        /// <param name="tobe"></param>
        /// <returns></returns>
        public string getUniqueElementName(object tobelist, BaseElement tobe) {
            string name = tobe.GetHashCode()+DateTime.Now.Millisecond+"";
            if (tobelist != null && tobe != null) {
                string bat = tobe.Name ;
                if (tobe.Name != null && tobe.Name.Trim().Length > 0) {
                    name = tobe.Name;
                }
                if (isUniqueToBeElement(tobe, tobelist)) {
                    return tobe.Name;
                }
                for (int i = 0; i < 500; i++) {
                    tobe.Name = name + i;
                    if (isUniqueToBeElement(tobe, tobelist)) {
                        tobe.Name = bat;
                        return name + i;
                    }
                }
            }
            return name;
        }
        /// <summary>
        /// return the decimal value or decimal.MinValue if errors. 
        /// Input should be a string or decimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public decimal getDecimal(object input) {
            decimal v = decimal.MinValue;
            if (input != null) {
                try {
                    v = Convert.ToDecimal(input);
                } catch (Exception) { }
            }
            return v;
        }
        /// <summary>
        /// Get a BEList's owner element or null if errors. 
        /// </summary>
        /// <param name="belist"></param>
        public BaseElement getBEListOwner(object belist) {            
            if (belist is BEList<WebElementAttribute>) {
                return (belist as BEList<WebElementAttribute>).Owner;
            } else if (belist is BEList<WebElement>) {
                return (belist as BEList<WebElement>).Owner;
            } else if (belist is BEList<WebElementGroup>) {
                return (belist as BEList<WebElementGroup>).Owner;
            } else if (belist is BEList<Process>) {
                return (belist as BEList<Process>).Owner;
            } else if (belist is BEList<Operation>) {
                return (belist as BEList<Operation>).Owner;
            } else if (belist is BEList<OperationRule>) {
                return (belist as BEList<OperationRule>).Owner;
            } else if (belist is BEList<OpCondition>) {
                return (belist as BEList<OpCondition>).Owner;
            } else if (belist is BEList<ConditionGroup>) {
                return (belist as BEList<ConditionGroup>).Owner;
            } else if (belist is BEList<Condition>) {
                return (belist as BEList<Condition>).Owner;
            } else if (belist is BEList<ParamGroup>) {
                return (belist as BEList<WebElementAttribute>).Owner;
            } else if (belist is BEList<Parameter>) {
                return (belist as BEList<Parameter>).Owner;
            } else if (belist is BEList<Expression>) {
                return (belist as BEList<Expression>).Owner;
            } else if (belist is BEList<GlobalFunction>) {
                return (belist as BEList<GlobalFunction>).Owner;
            } else if (belist is BEList<ParamCmd>) {
                return (belist as BEList<ParamCmd>).Owner;
            } else if (belist is BEList<UserLog>) {
                return (belist as BEList<UserLog>).Owner;
            } else if (belist is BEList<UserLogItem>) {
                return (belist as BEList<UserLogItem>).Owner;
            } else if (belist is BEList<BaseElement>) {
                object obj = (belist as BEList<BaseElement>).Owner;
                if (obj is ConditionGroup) {
                    return obj as ConditionGroup ;
                }
            }
            return null;
        }
        #endregion utils
        #region Model validation area
        /// <summary>
        /// check whether the input name contains invalid chars. return true if contains, if false, it means that 
        /// there is no invalid chars contained
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool isNameInvalidChars(BaseElement input) {
            if (input != null && input.Name != null) {
                foreach (char c in input.Name) {
                    if (BE_NAME_INVALID_CHARS.Contains(c)) {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// check whether the tobeE element is unique if it is added into belist. 
        /// 
        /// handle WebElement,WebElementAttribute, WebElementGroup, Process,Operation,
        /// OpCondition,Condition,ConditionGroup,ParamGroup,Parameter, OperationRule
        /// </summary>
        /// <param name="tobeE"></param>
        /// <param name="belist">must be a BEList object</param>
        public bool isUniqueToBeElement(BaseElement tobeE, object belist){
            if(belist == null || tobeE == null){
                return false ;
            }
            if (tobeE is WebElementAttribute) {
                BEList<WebElementAttribute> tlist = belist as BEList<WebElementAttribute>;
                foreach (WebElementAttribute t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            }else if (tobeE is WebElement) {
                BEList<WebElement> tlist = belist as BEList<WebElement>;
                foreach (WebElement t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is WebElementGroup) {
                BEList<WebElementGroup> tlist = belist as BEList<WebElementGroup>;
                foreach (WebElementGroup t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is Process) {
                BEList<Process> tlist = belist as BEList<Process>;
                foreach (Process t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is Operation) {
                BEList<Operation> tlist = belist as BEList<Operation>;
                foreach (Operation t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is OpCondition) {
                BEList<OpCondition> tlist = belist as BEList<OpCondition>;
                foreach (OpCondition t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is ConditionGroup || tobeE is Condition) {
                BEList<BaseElement> tlist = belist as BEList<BaseElement>;
                foreach (BaseElement be in tlist) {
                    if (be.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is ParamGroup) {
                BEList<ParamGroup> tlist = belist as BEList<ParamGroup>;
                foreach (ParamGroup t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is Parameter) {
                BEList<Parameter> tlist = belist as BEList<Parameter>;
                foreach (Parameter t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            } else if (tobeE is OperationRule) {
                BEList<OperationRule> tlist = belist as BEList<OperationRule>;
                foreach (OperationRule t in tlist) {
                    if (t.Name.Equals(tobeE.Name)) {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// check whether a model element is unqiue in its container
        /// true, if the element is unique in its container, false, there is at least one 
        /// duplicated name with the element 
        /// Just handle WebElement, WebElmentAttribute, WebElementGroup, Operation, Process,
        /// OpCondition, Condition, ConditionGroup, ParamGroup, Parameter, OperationRule
        /// </summary>
        /// <param name="be"></param>
        /// <returns></returns>
        public bool isUniqueElement(BaseElement be) {
            if (be == null) {
                return false;
            }
            if (be is WebElementAttribute) {
                WebElementAttribute wea = be as WebElementAttribute;
                foreach (WebElementAttribute t in wea.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            } else if (be is WebElement) {
                WebElement we = be as WebElement;
                foreach (WebElement t in we.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            } else if (be is WebElementGroup) {
                WebElementGroup weg = be as WebElementGroup;
                if (weg.Collection != null) { // here is used to filter the WERoot 
                    foreach (WebElementGroup t in weg.Collection) {
                        if (t != be && t.Name.Equals(be.Name)) {
                            return false;
                        }
                    }
                }
            } else if (be is Process) {
                Process proc = be as Process;
                if (proc.Collection != null) { // here is used to filter the ProcRoot
                    foreach (Process t in proc.Collection) {
                        if (t != be && t.Name.Equals(be.Name)) {
                            return false;
                        }
                    }
                }
            } else if (be is Operation) {
                Operation op = be as Operation;
                foreach (Operation t in op.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            } else if (be is OpCondition) {
                OpCondition opc = be as OpCondition;
                foreach (OpCondition t in opc.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            } else if (be is Condition) {
                Condition con = be as Condition;
                foreach (BaseElement t in con.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            } else if (be is ConditionGroup) {
                ConditionGroup crg = be as ConditionGroup;
                if (crg.Collection != null) { // here is used to filter the OpCondition's root ConditionGroup. 
                    foreach (BaseElement t in crg.Collection) {
                        if (t != be && t.Name.Equals(be.Name)) {
                            return false;
                        }
                    }
                }
            } else if (be is ParamGroup) {
                ParamGroup pgrp = be as ParamGroup;
                if (pgrp.Collection != null) { // here is used to filter the ParamRoot
                    foreach (ParamGroup t in pgrp.Collection) {
                        if (t != be && t.Name.Equals(be.Name)) {
                            return false;
                        }
                    }
                }
            } else if (be is Parameter) {
                Parameter param = be as Parameter;
                foreach (Parameter t in param.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            } else if (be is OperationRule) {
                OperationRule rule = be as OperationRule;
                foreach (OperationRule t in rule.Collection) {
                    if (t != be && t.Name.Equals(be.Name)) {
                        return false;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// check wether the attribute is valid, and give relative messages, 
        /// if valid return msg with MsgType.VALID, if failed, return error msg, just supported type
        /// 0. ScriptRoot, 1. WebElementAttribute, 2. WebElement, 3. WebElementGroup, 4. Process, 5. Operation. 
        /// 6. OpCondition, 7. Condition, 8. ConditionGroup, 9. Parameter, 10. OperationRule, 11. Expression, 
        /// 12. GlobalFunction, 13. ParamCmd.
        /// Notes that each check just check the 
        /// </summary>
        /// <param name="be"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(BaseElement be) {
            ValidationMsg msg = new ValidationMsg();
            if (be is ScriptRoot) {
                return getValidMsg(be as ScriptRoot);
            } else if (be is WebElementAttribute) {
                return getValidMsg(be as WebElementAttribute);
            } else if (be is WebElement) {
                return getValidMsg(be as WebElement);
            } else if (be is WebElementGroup) {
                return getValidMsg(be as WebElementGroup);
            } else if (be is Process) {
                return getValidMsg(be as Process);
            } else if (be is Operation) {
                return getValidMsg(be as Operation);
            } else if (be is OpCondition) {
                return getValidMsg(be as OpCondition);
            } else if (be is Condition) {
                return getValidMsg(be as Condition);
            } else if (be is ConditionGroup) {
                return getValidMsg(be as ConditionGroup);
            } else if (be is ParamGroup) {
                return getValidMsg(be as ParamGroup);
            } else if (be is Parameter) {
                return getValidMsg(be as Parameter);
            } else if (be is OperationRule) {
                return getValidMsg(be as OperationRule);
            } else if (be is Expression) {
                return getValidMsg(be as Expression);
            } else if (be is GlobalFunction) {
                return GFManager.getValidMsg(be as GlobalFunction);
            } else if (be is ParamCmd) {
                return getValidMsg(be as ParamCmd);
            }

            msg.Type = MsgType.ERROR;
            msg.Msg = LangUtil.getMsg("valid.model.err.msg1");// "Invalid element type. ";
            return msg;
        }
        /// <summary>
        /// check whether the script root is valid, just check the script root element itself. 
        /// 1. check whether the name is valid.
        /// 2. check description
        /// 3. check target url 
        /// 4. check candidate urls 
        /// 5. check applied rules 
        /// </summary>
        /// <param name="sroot"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(ScriptRoot sroot) {
            string prefix = LangUtil.getMsg("model.SRoot.Name"); // Script
            ValidationMsg msg = getInvalidNameMsg(sroot, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (sroot.Description != null  && sroot.Description.Length >= SROOT_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", SROOT_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix+txt1;
                return msg;
            }             
            // 3. check target url 
            if (sroot.TargetWebURL == null) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.sroot.tgt.url.msg1");//"Script target URL is empty. ";
                return msg;
            } else if (sroot.TargetWebURL.Length > SROOT_URL_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.sroot.tgt.url.msg2", SROOT_URL_MAX_LENGTH, sroot.TargetWebURL);//Script target URL length exceed, max ={0}, url ={ {1} }
                return msg;
            }
            // 4. check candidate urls  
            if (sroot.TrustedUrls.Count > 0) {
                foreach (string url in sroot.TrustedUrls) {
                    if (url.Length < 1) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.sroot.urls.msg1");// "There is a empty url in the trust URL list.";
                        return msg;
                    } else if (url.Length >= SROOT_URL_MAX_LENGTH) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.sroot.urls.msg2", url, SROOT_URL_MAX_LENGTH);//There is an trusted URL { {0} } exceed the max length, max = {1} .                         
                        return msg;
                    } else if (!(url.StartsWith("http://") || url.StartsWith("https://"))) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.sroot.urls.msg3",url);// URL must start with http:// or https://, url ={ {0} } 

                        return msg;
                    }
                }
            }
            // 5. check applied rules             
            foreach (OperationRule rule in sroot.ProcRoot.Rules) {
                msg = getValidMsg(rule);
                msg.Msg.Insert(0, prefix +" " + sroot.Name + " : ");
                return msg;
            }
            
            return msg;
        }
        /// <summary>
        /// check wether the attribute is valid, and give relative messages, 
        /// if valid return a msg with MsgType.VALID, if failed, return error msg
        /// 1. it name should be unique in its container and not include invalid chars 
        /// 2. check description length
        /// 3. whether the pattern mathed the input value. 
        /// </summary>
        /// <param name="wea"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(WebElementAttribute wea) {
            // 1. it name should be unique in its container and not include invalid chars 
            string prefix = LangUtil.getMsg("model.WEA.Name"); //WebElementAttribute
            ValidationMsg msg = getInvalidNameMsg(wea, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (wea.Description != null && wea.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }             
            // 3. check pattern and values 
            if (!(isStringPattern(wea.PATTERN))) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.wea.pattern.msg1",wea.Name);//Attribute pattern is invalid, name = {0}
                return msg;
            } else if (wea.PValues.Count == 0 || hasPValueLengh(wea)==false) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.wea.pattern.msg2", wea.Name);// Attribute pattern value should not be empty, name = {0}. 	

                return msg;
            }
            return msg;
        }

        private bool hasPValueLengh(WebElementAttribute wea) {
            if (wea != null) {
                foreach (object obj in wea.PValues) {
                    if (obj is string && obj.ToString().Length>0) {
                        return true;
                    }
                    if (obj is Parameter) {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// check wether the WebElement is valid and give relative message. it will check the WebElement
        /// itself and all Attributes 
        /// if valid, return a msg with MsgType.VALID, else return error msg. 
        /// 1. it will check wether the name is unique in its container, and not include invalid chars 
        /// 2. check description length
        /// 3. it will check whether each WebElementAttribute validation info
        /// 4. validtion check the password WebElement
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(WebElement we) {
            // 1. it will check wether the name is unique in its container, and not include invalid chars 
            string prefix = LangUtil.getMsg("model.WE.Name"); // WebElement 
            ValidationMsg msg = getInvalidNameMsg(we, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (we.Description != null && we.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }   
            // 3. it will check whether each WebElementAttribute validation info
            foreach (WebElementAttribute wea in we.Attributes) {
                ValidationMsg tm = getValidMsg(wea);
                if (tm.Type != MsgType.VALID) {
                    tm.Msg.Insert(0,prefix+" : "+we.Name+" - ");
                    return tm;
                }
            }
            // 4. validtion check the password WebElement
            if (we.isPassword) {
                int opcount = 0;
                foreach (object obj in we.WeakRef) {
                    if (obj is Operation) {
                        Operation op = obj as Operation;
                        if (op.Element == we) {
                            opcount++;
                        }
                    }
                    if (!(obj is BEList<WebElement>) && (opcount > 1 || obj is ListRef)) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.we.pwd.msg1", we.Name); // A password Element { {0} } can only be used for one Operation
                        return msg;
                    }
                }
            }
            return msg;
        }
        /// <summary>
        /// check whether the WebElementGroup is valid and give relative message
        /// if valid, return a msg with MsgType.VALID, else return error msg. 
        /// 1. it will check wether the name is unique in its container, and not include invalid chars 
        /// 2. check description length 
        /// </summary>
        /// <param name="we"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(WebElementGroup weg) {
            // 1. it will check wether the name is unique in its container, and not include invalid chars 
            string prefix = LangUtil.getMsg("model.WEGrp.Name"); // WebElementGroup
            ValidationMsg msg =  getInvalidNameMsg(weg, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (weg.Description != null && weg.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            return msg;
        }
        /// <summary>
        /// check wether the Operation is valid and give relative msg. If valid, return a msg with MsgType.VALID, if failed
        /// return error msg. It just check the Operation itself. 
        /// 1. check operaiton name unique and not include invalid chars and check description
        /// 2. check input existance if needed. 
        /// 3. check input value if needed, input must be string or Parameter
        /// 4. check if there is an OpCondition existed, Warning. 
        /// 5. check the wait time is valid.
        /// 6. check applied rules 
        /// 7. check Exeutetime 
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(Operation op) {
            // 1. check operaiton name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.Op.Name"); // Operation
            ValidationMsg msg = getInvalidNameMsg(op, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // filter start and end node 
            if (op.OpType == OPERATION.START || op.OpType == OPERATION.END) {
                return msg;
            }
            // 2. check description 
            if (op.Description != null && op.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 2.1. check input existance if needed. 
            if ( op.OpType == OPERATION.CLICK || op.OpType == OPERATION.INPUT) {
                if (op.Element == null) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.op.elem.msg", op.Name);//Operated element can not be null, name={ {0} }
                    return msg;
                } else if (op.Element.isPassword) {
                    ValidationMsg tmsg = getValidMsg(op.Element);
                    if (tmsg.Type != MsgType.VALID) {
                        return tmsg;
                    }
                }
            }
            // 3. check input value if needed, input must be string or Parameter
            if (op.OpType == OPERATION.INPUT) {
                if (op.Input == null) {
                    msg.Type = MsgType.WARNING;
                    msg.Msg = LangUtil.getMsg("valid.op.input.msg1",op.Name);// Operation { {0} } input is null. 
                    return msg;
                } else if (!(op.Input is Parameter || op.Input is string)) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.op.input.msg2", op.Name);// Operation { {0} } input should be a parameter, number or string.
                    return msg;
                }
            }
            // 4. check if there is an OpCondition existed, Warning. 
            if (op.OpConditions.Count < 1 && op.OpType != OPERATION.END) {
                msg.Type = MsgType.WARNING;
                msg.Msg = LangUtil.getMsg("valid.op.opc.msg1",op.Name);//Operation { {0} } should has at least one link to another operation or process
                return msg;
            }            
            // 5. check the wait time is valid.
            try {
                op.getWaitTimes();
            } catch (InvalidCastException) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.op.time.msg1",op.Name);// Operation { {0} } wait time format error. 
                return msg;
            }
            // 6. check applied rules             
            foreach (OperationRule rule in op.Rules) {
                msg = getValidMsg(rule);
                msg.Msg.Insert(0, prefix+" " + op.Name + " : ");
                return msg;
            }            
            // 7. check execute time 
            if (op.ExeuteTime!=null && !isValidTime(op.ExeuteTime)) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.op.time.msg2",op.Name);// Operation { {0} } Execute time format error, format should like : HH:mm:ss
                return msg;
            }
            return msg;
        }
        /// <summary>
        /// check wether the Process is valid and give relative msg. If valid, return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. check process name unique and not include invalid chars 
        /// 2. check description
        /// 3. check applied rules 
        /// 4. check the wait time is valid.
        /// 5. check execute time 
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(Process proc) {
            // 1. check process name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.Proc.Name"); // Process
            ValidationMsg msg = getInvalidNameMsg(proc, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (proc.Description != null && proc.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check applied rules             
            foreach (OperationRule rule in proc.Rules) {
                msg = getValidMsg(rule);
                if (msg.Type != MsgType.VALID) {
                    msg.Msg.Insert(0, prefix+" " + proc.Name + " : ");
                    return msg;
                }
            }            
            // 4. check the wait time is valid.
            try {
                proc.getWaitTimes();
            } catch (InvalidCastException) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.proc.time.msg1", proc.Name);//Process { {0} } wait time format error.
                return msg;
            }
            // 5. check execute time 
            if (proc.ExeuteTime != null && !isValidTime(proc.ExeuteTime)) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.proc.time.msg2",proc.Name);//Process { {0} } Execute time format error, format should like : HH:mm:ss
                return msg;
            }
            return msg;
        }
        /// <summary>
        /// check whether the OpCondition is valid and give relative msg, If valid return a msg with MsgType.VALID, if failed,
        /// return error msg. 
        /// 1. check OpCondition name is unique
        /// 2. check description
        /// 3. check there is at least one Condtion.
        /// </summary>
        /// <param name="opc"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(OpCondition opc) {
            // 1. check OpCondition name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.Opc.Name"); // Transition
            ValidationMsg msg = getInvalidNameMsg(opc, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (opc.Description != null && opc.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check the OpCondition has at least one condition. 
            //if (opc.ConditionGroup.Conditions.Count == 0) {                
            //    msg.Type = MsgType.WARNING;
            //    msg.Msg = "The Operation condition " + opc.Name + " has no condition";
            //    return msg;
            //}
            return msg;
        }
        /// <summary>
        /// check whether the Operation Rule is valid and give relative msg, If valid return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. check OperationRule name is unique
        /// 2. check description
        /// 3. check sensitive parameter 
        /// 4. check rule parameters are valid according the rule definition. 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(OperationRule rule) {
            // 1. check Rule name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.Rule.Name"); // Rule
            ValidationMsg msg = getInvalidNameMsg(rule, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (rule.Description != null && rule.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check sensitive parameter 
            foreach (object obj in rule.Params) {
                if (obj is Parameter) {
                    Parameter param = obj as Parameter;
                    if (param.Sensitive) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = prefix+" - "+LangUtil.getMsg("valid.param.sensitive.msg1",param); // Parameter { {0} } can only be used to password WebElement
                        return msg; 
                    }
                }
            }
            // 4. check rule parameters based on the rule definition. 
            if (rule.Action == RuleAction.Goto_Operation) {
                string txt = GotoOperationRule.getValidMsg(rule);
                if (txt.Length > 0) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = txt;
                    return msg;
                }
            } else if (rule.Action == RuleAction.WaitUntilElemFind) {
                string txt = WaitUntilNullElemFindRule.getValidMsg(rule);
                if (txt.Length > 0) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = txt;
                    return msg;
                }
            }
            
            return msg;
        }
        /// <summary>
        /// check whether the parameter is valid and give relative msg, if valid return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. check Parameter name is unique. 
        /// 2. check description
        /// 3. check the parmameter value is align with the type 
        /// 4. check DateTime format 
        /// 5. check Set elements
        /// 6. check sensitive parameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(Parameter param) {
            // 1. check Parameter name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.Param.Name"); // Parameter 
            ValidationMsg msg = getInvalidNameMsg(param, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (param.Description != null && param.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check the parmameter value is align with the type
            object value = param.DesignValue;            
            if (param.Type == ParamType.STRING && value != null && value.ToString().Length < 1) {
                msg.Type = MsgType.WARNING;
                msg.Msg = LangUtil.getMsg("valid.param.value.msg1",param.Name);// Parameter { {0} } value is empty
                return msg;
            }
            if (param.Type == ParamType.NUMBER) {
                decimal v = ModelManager.Instance.getDecimal(value);
                if(v == decimal.MinValue){
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.param.value.msg2",param.Name);//Parameter { {0} } is Number type, value should be a number 
                    return msg;
                }
            }
            // 4. check DataTime format
            if (param.Type == ParamType.DATETIME && value!=null) {
                if (!isValidTime(value.ToString())) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.param.time.msg1", param.Name);// Parameter { {0} } time format error, format should like : HH:mm:ss
                    return msg;
                }
            }
            // 5. check Set elements
            if (param.Type == ParamType.SET) {
                if (param.SetType == ParamType.NUMBER) {
                    foreach (object obj in param.DesignSet) {
                        decimal v = ModelManager.Instance.getDecimal(obj);
                        if (v == decimal.MinValue) {
                            msg.Type = MsgType.ERROR;
                            msg.Msg = LangUtil.getMsg("valid.param.set.msg1", param.Name);//Parameter { {0} } is Set type with Number, all items should be a number 
                            return msg;
                        }
                    }
                }
            }
            // 6. check sensitive parameter
            if (param.Sensitive) {
                int opcount = 0;
                foreach (object obj in param.WeakRef) {
                    if (obj is Operation) {
                        Operation op = obj as Operation;
                        if (op.Input == param) {
                            opcount++;
                        }
                    }
                }
                if (opcount > 1) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.param.pwd.msg2", param.Name); // Parameter { {0} } can only be used for one Operation with Password Element
                    return msg;
                }
            }
            
            
            return msg;
        }
        /// <summary>
        /// check whether the ParameterGroup is valid and give relative msg, if valid return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. check parameter group name is unique. 
        /// 2. check description
        /// </summary>
        /// <param name="pgrp"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(ParamGroup pgrp) {
            // 1. check name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.ParamGrp.Name");
            ValidationMsg msg = getInvalidNameMsg(pgrp, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (pgrp.Description != null && pgrp.Description.Length >= BE_DESC_MAX_LENGTH) {
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            return msg;
        }
        /// <summary>
        /// check whether the ConditionGroup is valid and give relative msg, if valid return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. check condition group name is unique. 
        /// 2. check description
        /// </summary>
        /// <param name="congrp"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(ConditionGroup congrp) {
            // 1. check name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.ConGrp.Name");
            ValidationMsg msg = getInvalidNameMsg(congrp, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (congrp.Description != null && congrp.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }            
            return msg;
        }
        /// <summary>
        /// check whether the condition is valid and give relative message, 
        /// if the condition is valid, return a msg with MsgType.VALID. 
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(Condition con) {
            // 1. check Condition name unique and not include invalid chars 
            string prefix = LangUtil.getMsg("model.Con.Name");
            ValidationMsg msg = getInvalidNameMsg(con, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check description 
            if (con.Description != null && con.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check Sensitive parameter 
            if(con.Input1 is Parameter || con.Input2 is Parameter){
                Parameter p1 = con.Input1 as Parameter ;
                Parameter p2 = con.Input2 as Parameter;
                string str = null;
                if(p1!=null && p1.Sensitive){
                    str = "1";
                }
                if (p2 != null && p2.Sensitive) {
                    str = "2";
                }
                if (str != null) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.input.param.msg1", con.Name, str); // Condition { {0} } Input{1} can not be a sensitive parameter
                    return msg;
                }
            }
            // 4. check existed pattern
            if (isObjPattern(con.COMPARE)) {
                if (!(con.Input1 is WebElement || con.Input1 is WebElementAttribute || con.Input1 is Parameter)) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.msg1",con.Name);//Condition { {0} } Input1 should be a WebElement or WebElementAttribute or a Parameter
                    return msg;
                }
                if (con.Input2 != null) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.msg2",con.Name);//Condition { {0} } Input2 should be empty
                    return msg;
                }
            }
            // 5. check number pattern
            if (isNumberPattern(con.COMPARE)) {
                // check input1 
                if (con.Input1 is WebElementAttribute) {
                } else if (con.Input1 is Parameter && ((Parameter)con.Input1).Type != ParamType.NUMBER) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.msg3",con.Name,1);// Condition { {0} } Input{1} should be a number or number type parameter 
                    return msg;
                } else if (con.Input1 is string) {
                    try {
                        int.Parse(con.Input1.ToString());
                    } catch (Exception) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.con.msg3", con.Name,1);// Condition { {0} } Input{1} should be a number or number type parameter 
                        return msg;
                    }
                }
                // check input2 
                if (con.Input2 is WebElementAttribute) {
                } else if (con.Input2 is Parameter && ((Parameter)con.Input2).Type != ParamType.NUMBER) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.msg3", con.Name, 2);// Condition { {0} } Input{1} should be a number or number type parameter
                    return msg;
                } else if (con.Input2 is string) {
                    try {
                        int.Parse(con.Input2.ToString());
                    } catch (Exception) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.con.msg3", con.Name, 2);// Condition { {0} } Input{1} should be a number or number type parameter
                        return msg;
                    }
                }
            }
            // 6. check set pattern 
            if (isSetPattern(con.COMPARE)) {
                if (con.Input1 is Parameter && ((Parameter)con.Input1).Type != ParamType.SET) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.set.msg1",con.Name);//Condition { {0} } is a Set pattern, Input1 should be a Set Parameter";
                    return msg;
                }
                if (con.Input2 == null || con.Input2.ToString().Trim().Length < 1) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.set.msg2",con.Name);//Condition { {0} } is a Set pattern, Input2 shouldn't be empty
                    return msg;
                }
            }
            // 7. check string pattern
            if (isStringPattern(con.COMPARE)) {
                if (con.Input1 != null && con.Input1.ToString().Trim().Length < 1) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.str.msg1",con.Name,1);// Condition { {0} } Input{1} should not be empty
                    return msg;
                }
                if (con.Input2 != null && con.Input2.ToString().Trim().Length < 1) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.str.msg1", con.Name, 2);// Condition { {0} } Input{1} should not be empty
                    return msg;
                }
            }
            // 8. check DateTime pattern
            if (isDateTimePattern(con.COMPARE)) {
                if (con.Input1!=Constants.DATETIME_NOW && !isMaybeTimeValue(con.Input1)) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.time.msg1", con.Name, 1);// Condition { {0} } Input{1} should be a Time value
                    return msg;
                }
                if (con.Input2 != Constants.DATETIME_NOW && !isMaybeTimeValue(con.Input2)) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.con.time.msg1", con.Name, 2);// Condition { {0} } Input{1} should be a Time value
                    return msg;
                }
            }
            return msg;
        }

        /// <summary>
        /// Check wether the Expression is valid and give relative msg. If valid, return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. ignore the expression name unique checking.
        /// 2. check whether the discription exceed the max length. 
        /// 3. check whether the input1/input2 matched the type==String
        /// 4. check whether the input1/input2 matched the type==Number
        /// 5. if type==Number, Operator=='/', check the input2 == 0
        /// 6. check whether the input1/input2 matched the type = DateTime
        /// 7. check sensitive parameter 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public ValidationMsg getValidMsg(Expression exp) {
            // 1. ignore the expression name unique checking.
            string prefix = LangUtil.getMsg("model.Expression.name");
            ValidationMsg msg = getInvalidNameMsg(exp, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check whether the discription exceed the max length. 
            if (exp.Description != null && exp.Description.Length >= BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check whether the input1/input2 matched the type==String
            if (exp.Type == ParamType.STRING) {
                if (exp.Operator != "+") {
                    msg.Type = MsgType.ERROR;
                    string strTxt = LangUtil.getMsg("ParamType.STRING");
                    msg.Msg = LangUtil.getMsg("valid.exp.operator.msg1",strTxt,"'+'");// Expression Operator error, for {0} type, only {1} allowed
                    return msg;
                } else {
                    if (exp.Input1 == null) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.exp.input.left.msg1");//Expression left input is null
                        return msg;
                    }
                    if (exp.Input2 == null) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.exp.right.left.msg1");//Expression right input is null
                        return msg;
                    }
                }
            }
            // 4. check whether the input1/input2 matched the type==Number
            if (exp.Type == ParamType.NUMBER) {
                if (exp.Operator == "+" || exp.Operator == "-" || exp.Operator == "*" || exp.Operator == "/") {
                    if (!isMaybeNumberValue(exp.Input1)) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.exp.num.msg1");// Expression Left input should be a number
                        return msg;
                    }
                    if (!isMaybeNumberValue(exp.Input2)) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.exp.num.msg2"); //Expression Right input should be a number
                        return msg;
                    }
                    // 5. if type==Number, Operator=='/', check the input2 == 0
                    if (exp.Operator == "/") {
                        if (exp.Input2 is string || exp.Input2 is decimal) {
                            decimal d = getDecimal(exp.Input2);
                            if (d == 0) {
                                msg.Type = MsgType.ERROR;
                                msg.Msg = LangUtil.getMsg("valid.exp.num.div.msg1");// For a / operator, Right input should not be zero
                                return msg;
                            }
                        } else if (exp.Input2 is Parameter) {
                            Parameter p = exp.Input2 as Parameter;
                            if (p.Type == ParamType.NUMBER) {
                                decimal d = getDecimal(exp.Input2);
                                if (d == 0) {
                                    msg.Type = MsgType.ERROR;
                                    msg.Msg = LangUtil.getMsg("valid.exp.num.div.msg1");// For a / operator, Right input should not be zero
                                    return msg;
                                }
                            }
                        }
                    }
                } else {
                    msg.Type = MsgType.ERROR;
                    string numTxt = LangUtil.getMsg("ParamType.NUMBER");
                    msg.Msg = LangUtil.getMsg("valid.exp.operator.msg1", numTxt, "'+','-','*','/'");// Expression Operator error, for {0} type, only {1} allowed
                    return msg;
                }
            }
            // 6. check whether the input1/input2 matched the type = DateTime    
            if (exp.Type == ParamType.DATETIME) {
                if (exp.Operator == "+" || exp.Operator == "-") {
                    if (exp.Input1!= Constants.DATETIME_NOW && !isMaybeTimeValue(exp.Input1)) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.exp.time.msg1");// Expression Left input should be a time value
                        return msg;
                    }                    
                    if (exp.Input2 != Constants.DATETIME_NOW && !isMaybeTimeValue(exp.Input2)) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.exp.time.msg2");// Expression Right input should be a time value
                        return msg;
                    }                    
                } else {
                    msg.Type = MsgType.ERROR;
                    string timeTxt = LangUtil.getMsg("ParamType.DATETIME");
                    msg.Msg = LangUtil.getMsg("valid.exp.operator.msg1", timeTxt, "'+','-'");// Expression Operator error, for {0} type, only {1} allowed
                    return msg;
                }
            }
            // 7. check sensitive parameter.             
            if (exp.Input1 is Parameter || exp.Input2 is Parameter) {
                Parameter p1 = exp.Input1 as Parameter;
                Parameter p2 = exp.Input2 as Parameter;
                string str = null;
                if (p1 != null && p1.Sensitive) {
                    str = "1";
                }
                if (p2 != null && p2.Sensitive) {
                    str = "2";
                }
                if (str != null) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.exp.input.param.msg1", str); // Expression Input{1} can not be a sensitive parameter
                    return msg;
                }
            }
            return msg;
        }
        /// <summary>
        /// Check whether the ParamCmd is valid and give relative msg. If valid, return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. ignore the name validation check. 
        /// 2. check the description max length exceed. 
        /// 3. check the CMD/Src/Target matches
        /// 4. check sensitive parameter 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal static ValidationMsg getValidMsg(ParamCmd cmd) {
            // 1. ignore the name validation check. 
            string prefix = LangUtil.getMsg("model.ParamCmd.name"); // Command
            ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(cmd, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check the description max length exceed.
            if (cmd.Description != null && cmd.Description.Length >= ModelManager.BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;
                return msg;
            }
            // 3. check src and target - null
            if (cmd.Src == null) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.cmd.input.msg1");// Command source input is null
                return msg;
            }
            if (cmd.Target == null) {
                msg.Type = MsgType.ERROR;
                msg.Msg = LangUtil.getMsg("valid.cmd.input.msg2");// Command target input is null
                return msg;
            }
            // 4. check the CMD/Src/Target matches
            if (cmd.Cmd == PARAM_CMD.ASSIGN) {
                if (cmd.Target.Type == ParamType.SET) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.cmd.assign.msg1");// For Assign command, target parameter must be String or Number type
                    return msg;
                }
            } else if (cmd.Cmd == PARAM_CMD.UPDATE_SET_ADD || cmd.Cmd == PARAM_CMD.UPDATE_SET_DEL) {
                if (cmd.Target.Type == ParamType.STRING || cmd.Target.Type == ParamType.NUMBER || cmd.Target.Type == ParamType.DATETIME) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.cmd.set.msg1"); // For Set command, Target parameter must be a Set parameter type
                    return msg;
                }
                if (cmd.Target.Type == ParamType.SET) {
                    if (cmd.Target.SetType == ParamType.NUMBER) {
                        if (!ModelManager.Instance.isMaybeNumberValue(cmd.Src)) {
                            msg.Type = MsgType.ERROR;
                            msg.Msg = LangUtil.getMsg("valid.cmd.set.msg2");// For Set command, mapping source should be a Number value
                            return msg;
                        }
                    }
                    if (cmd.Target.SetType == ParamType.DATETIME) {
                        if (!ModelManager.Instance.isMaybeTimeValue(cmd.Src)) {
                            msg.Type = MsgType.ERROR;
                            msg.Msg = LangUtil.getMsg("valid.cmd.set.msg3");// For Set command, mapping source should be a Time value
                            return msg;
                        }
                    }
                }
            }
            // 5. check sensitive parameter, the src can not be sensitive parameter             
            if (cmd.Src is Parameter) {                
                Parameter p = cmd.Src as Parameter;
                if(p.Sensitive){
                    msg.Type = MsgType.ERROR;
                    msg.Msg = LangUtil.getMsg("valid.cmd.input.param.msg1"); // Command source can not be a sensitive parameter
                    return msg;
                }
            }
            return msg;
        }
        /// <summary>
        /// validate the element name and return relative msg, if valid, it will return 
        /// InvalidMsg with MsgType.VALID. 
        /// </summary>
        /// <param name="be"></param>
        /// <param name="msgPrefix">element name that will be added in msg </param>
        /// <returns></returns>
        public ValidationMsg getInvalidNameMsg(BaseElement be, string msgPrefix){
            ValidationMsg msg = new ValidationMsg();
            if (be == null) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.null.msg"); //  can not be null
                msg.Msg = msgPrefix + txt;
                return msg;
            }
            if (be is Operation) {
                Operation op = be as Operation;
                if (op.OpType == OPERATION.START || op.OpType == OPERATION.END) {
                    return msg;
                }
            }
            // for Expression/GlobalFunction object, ignore the name validation check. 
            if (be is Expression || be is GlobalFunction || be is ParamCmd) {
                return msg;
            }
            // 1. check name 
            if (be.Name == null || be.Name.Trim().Length < 0) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.null.msg1"); //  name is empty	
                msg.Msg = msgPrefix+txt;
                return msg;
            }
            bool b1 = isNameInvalidChars(be);
            if (b1) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.name.msg1"); //  name contains invalid chars, invalid chars : 
                msg.Msg = msgPrefix + txt + BE_NAME_INVALID_STR;
                return msg;
            }
            if (be.Name.Length >= BE_NAME_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.name.exceed.msg", BE_NAME_MAX_LENGTH); // name exceed max length, max = {0}
                msg.Msg = msgPrefix + txt;
                return msg;
            }
            if (!isUniqueElement(be)) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.name.msg2",be.Name); // name = { {0} } already existed
                msg.Msg = msgPrefix + txt;
                return msg;
            }

            return msg;
        }
        /// <summary>
        /// validate the element name and return relative msg, if valid, it will return 
        /// InvalidMsg with MsgType.VALID. tobeE will be a candidate for the belist. 
        /// </summary>
        /// <param name="tobeE"></param>
        /// <param name="msgPrefix">element name that will be added in msg </param>
        /// <param name="belist">must be a BEList object</param>
        /// <returns></returns>
        public ValidationMsg getInvalidNameMsg(BaseElement tobeE, string msgPrefix,object belist) {
            ValidationMsg msg = new ValidationMsg();
            if (tobeE == null) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.null.msg"); //  can not be null
                msg.Msg = msgPrefix + txt;
                return msg;
            }
            // 1. check name 
            if (tobeE.Name == null || tobeE.Name.Trim().Length < 0) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.null.msg1"); //  name is empty	
                msg.Msg = msgPrefix + txt;
                return msg;
            }
            bool b1 = isNameInvalidChars(tobeE);
            if (b1) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.name.msg1"); //  name contains invalid chars, invalid chars : 
                msg.Msg = msgPrefix + txt + BE_NAME_INVALID_STR;                
                return msg;
            }
            if (tobeE.Name.Length >= BE_NAME_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.name.exceed.msg", BE_NAME_MAX_LENGTH); // name exceed max length, max = {0}
                msg.Msg = msgPrefix + txt;
                return msg;
            }
            if (!isUniqueToBeElement(tobeE,belist)) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.be.name.msg2", tobeE.Name); // name = { {0} } already existed
                msg.Msg = msgPrefix + txt;
                return msg;
            }

            return msg;
        }

        /// <summary>
        /// check the parameter set item validation
        /// </summary>
        /// <param name="input">string, decimal, Parameter or WebElementAttribute</param>
        /// <param name="type">parameter type</param>
        /// <returns></returns>
        public ValidationMsg getSetItemValidMsg(object input, ParamType type, string msgPrefix) {
            ValidationMsg msg = new ValidationMsg();
            if (input == null) {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.set.item.msg1"); // Set Item value is empty
                msg.Msg = txt;
                return msg;
            }
            // 1. check number 
            if (type == ParamType.NUMBER) {
                if (input is Parameter) {
                    Parameter param = input as Parameter;
                    if (param.Type != type) {
                        msg.Type = MsgType.ERROR;
                        string txt = LangUtil.getMsg("valid.set.item.msg2",param.Name); // Set item { {0} } should be a number type
                        msg.Msg = msgPrefix + txt;
                        return msg;
                    }
                } else if (!(input is WebElementAttribute)) {
                    try {
                        Convert.ToDecimal(input);
                    } catch (Exception) {
                        msg.Type = MsgType.ERROR;
                        string txt = LangUtil.getMsg("valid.set.item.msg3"); // Set item should be a number type
                        msg.Msg = msgPrefix + txt;
                        return msg;
                    }
                }
            } else if (type == ParamType.STRING) {
                if (input is Parameter) {
                    Parameter param = input as Parameter;
                    if (param.Type != type) {
                        msg.Type = MsgType.ERROR;
                        string txt = LangUtil.getMsg("valid.set.item.msg4",param.Name); // Set item { {0} } should be a string type
                        msg.Msg = msgPrefix + txt;
                        return msg;
                    }
                }
            } else {
                msg.Type = MsgType.ERROR;
                string txt = LangUtil.getMsg("valid.set.item.msg5",type); // Set item type not supported, type = { {0} }
                msg.Msg = msgPrefix + txt;
                return msg;
            }

            return msg;
        }
        #endregion model validation area
        #region parameter mapping/updating/Expression/GlobalFunctions
        /// <summary>
        /// If obj or it's children has WebElement or WEA, update the relative WE with runtime value. 
        /// If sucess, it will return null. 
        /// If there is one WE realize failed, it will return the failed WE. you can use " if( WE!=null && we.isRealElement==false )" to 
        /// double check the WE failed.
        /// 
        /// This method will auto handle all child/contained element in obj. 
        /// </summary>
        /// <param name="obj">It is Expression/GF parameter, it can be Constant(String||Number), WebElementAttribute, Parameter, Expression or 
        /// GlobalFunction</param>
        /// <param name="opw"></param>
        /// <param name="engine"></param>
        /// <param name="timeout">WebElement check timeout</param>
        /// <returns></returns>
        internal WebElement checkWE4ParamCmdIfNeed(object obj, OpWrapper opw, WebEngine engine, int timeout) {
            if (obj == null || opw == null || engine == null) {
                return null;
            }
            WebElement we = null;
            if (obj is WebElement || obj is WebElementAttribute) {
                return tryGetRealWE(obj, opw, engine, timeout);
            } else if (obj is Expression) {
                Expression exp = obj as Expression;
                we = checkWE4ParamCmdIfNeed(exp.Input1, opw, engine, timeout);
                if (we != null && we.IsRealElement == false) {
                    return we;
                }
                we = checkWE4ParamCmdIfNeed(exp.Input2, opw, engine, timeout);
                if (we != null && we.IsRealElement == false) {
                    return we;
                }
            } else if(obj is GlobalFunction){
                GlobalFunction gf = obj as GlobalFunction;
                foreach (object p in gf.Params) {
                    we = checkWE4ParamCmdIfNeed(p, opw, engine, timeout);
                    if (we != null && we.IsRealElement == false) {
                        return we;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// If the obj is described by a WebElement/WebElementAttribte, return update the WebElement with real value, 
        /// or null if it is not WebElement. 
        /// It will update the opw.CacheWE if it is a new WebElement. 
        /// If Not WebElement, return null 
        /// WebElement get failed:    if (we != null AND we.IsRealElement == false) { ... }
        /// WebElement get sucess:    if (we != null AND we.IsRealElement == true ) { ... }
        /// </summary>        
        /// <param name="obj">An object, maybe it is an WebElement/WebElementAttribute</param>        
        /// <param name="opw"></param>
        /// <param name="engine"></param>
        /// <param name="timeout">WebElement check timeout </param>
        /// <returns></returns>
        internal WebElement tryGetRealWE(object obj, OpWrapper opw, WebEngine engine, int timeout) {
            WebElement we = null;
            if (obj == null || opw == null || engine == null) {                
                return null;
            }
            // obj is a WebElementAttribute or WebElement string 
            if (obj is WebElement || obj is WebElementAttribute) {
                WebElement twe = obj as WebElement;
                if (twe == null) {
                    WebElementAttribute wea = obj as WebElementAttribute;
                    twe = wea.Collection.Owner as WebElement;
                }
                we = twe;
            } 

            if (we != null) {
                updateWERealPatternIfNeed(we,opw.Op);
                we = getRealWE(we, opw, engine, timeout);
            }
            return we;
        }
        /// <summary>
        /// Update the We's all attribute with PValue and parameters value if have. 
        /// </summary>
        /// <param name="we"></param>
        /// <param name="op"></param>
        private void updateWERealPatternIfNeed(WebElement we, Operation op) {
            if (we == null || op == null) {
                return;
            }            
            foreach (WebElementAttribute wea in we.Attributes) {
                string rpv = tryGetWEARealPatternValue(wea,op);
                wea.RealPValue = rpv;
            }
        }
        /// <summary>
        /// Get the WEA RealPatten value (for parameter with RealValue or use DesignValue if no realValue), 
        /// or string.Empty if errors. 
        /// </summary>
        /// <param name="wea"></param>
        /// <param name="op"></param>
        public string tryGetWEARealPatternValue(WebElementAttribute wea, Operation op) {
            string rpv = string.Empty;
            if (wea != null) {
                StringBuilder sb = new StringBuilder(); 
                foreach (object obj in wea.PValues) {
                    if (obj is string) {
                        sb.Append(obj.ToString());
                    } else if (obj is Parameter) {
                        Parameter param = obj as Parameter;
                        object prvalue = param.RealValue ;
                        if (prvalue == null || prvalue.ToString().Length==0) {
                            sb.Append(param.DesignValue);
                        } else {
                            sb.Append(param.RealValue);
                        }
                    }
                }
                if (sb.Length > 0) {
                    rpv = sb.ToString();
                }
            }
            return rpv ;
        }
        /// <summary>
        /// Get wea pattern value text in design time. 
        /// </summary>
        /// <param name="wea"></param>
        /// <returns></returns>
        public string getWEAText4Design(WebElementAttribute wea) {
            StringBuilder sb = new StringBuilder();
            foreach(object obj in wea.PValues){
                if(obj is string){
                    sb.Append(obj.ToString());
                }else if(obj is Parameter){
                    Parameter param = obj as Parameter ;
                    sb.Append("{").Append(param.Name).Append("}");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Get the WebElement updated with real value. if errors return the WE itself.
        /// It will update the CacheWE if a new WE found.
        /// </summary>
        /// <param name="we"></param>
        /// <param name="opw"></param>
        /// <param name="engine"></param>
        /// /// <param name="timeout">WebElement check timeout </param>
        /// <returns></returns>
        internal WebElement getRealWE(WebElement we, OpWrapper opw, WebEngine engine, int timeout) {
            if (we == null || opw == null || engine == null) {
                return we;
            }
            // find the WE from cache
            if (opw.CachedWE != null) {
                if (opw.CachedWE.Contains(we)) {
                    return we;
                }
            } else {
                opw.CachedWE = new List<WebElement>();
                if (we.IsRealElement) {
                    if (!opw.CachedWE.Contains(we)) {
                        opw.CachedWE.Add(we);
                    }
                    return we;
                }
            }
            // find we from UI page 
            int time = 0;
            timeout = timeout < Constants.CONDITION_INPUT_WE_CHECK_TIMEOUT ? Constants.CONDITION_INPUT_WE_CHECK_TIMEOUT : timeout ;
            while (time < timeout) {                
                engine.raiseWEUITestEvt(we);
                // If the browser document downloaded completed, make sure the loop will be stop
                // at next check. 
                if (engine.IsDocumentCompleted) {
                    time = timeout;
                }
                if (we.IsRealElement) {
                    if (!opw.CachedWE.Contains(we)) {
                        opw.CachedWE.Add(we);
                    }
                    break;
                } else {
                    Thread.Sleep(200);
                }
                time += 200;
            }

            return we;
        }
        /// <summary>
        /// Get parameter mapping source type texts based on the locale, can be 
        /// Constant/WebElementAttribute/Parameter/Expression/GlobalFunction
        /// </summary>
        /// <returns></returns>
        public List<string> getMappingSrcTypeTexts() {
            List<string> list = new List<string>();
            list.Add(LangUtil.getMsg("mapping.src.type.const.text"));
            list.Add(LangUtil.getMsg("model.WEA.Name"));
            list.Add(LangUtil.getMsg("model.Param.Name"));
            list.Add(LangUtil.getMsg("model.Expression.name"));
            list.Add(LangUtil.getMsg("model.GF.name"));
            return list;
        }
        /// <summary>
        /// Get the mapping source type index in the getMappingSrcTypeTexts() method, default value is 0.         
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public int getMappingSrcTypeIndex(object src) {
            int index = 0;
            if (src is GlobalFunction) {
                index = 4;
            } else if (src is Expression) {
                index = 3;
            } else if (src is Parameter) {
                index = 2;
            } else if (src is WebElementAttribute) {
                index = 1;
            } 

            return index;
        }
        /// <summary>
        /// get Parameter command types text with locale info 
        /// </summary>
        /// <returns></returns>
        public List<string> getParamCmdTypesText() {
            List<string> list = new List<string>();
            string text1 = getParamCmdText(PARAM_CMD.ASSIGN);
            list.Add(text1);
            text1 = getParamCmdText(PARAM_CMD.UPDATE_SET_ADD);
            list.Add(text1);
            text1 = getParamCmdText(PARAM_CMD.UPDATE_SET_DEL);
            list.Add(text1);
            return list;
        }     
        /// <summary>
        /// Get the paramIndex in the getParamCmdTypesText() list 
        /// </summary>
        /// <returns></returns>
        public int getParamCmdIndex(PARAM_CMD cmd){
            int index = 0;
            if (cmd == PARAM_CMD.ASSIGN) {
                index = 0;
            } else if (cmd == PARAM_CMD.UPDATE_SET_ADD) {
                index = 1;
            } else if (cmd == PARAM_CMD.UPDATE_SET_DEL) {
                index = 2;
            }
            return index;
        }
        /// <summary>
        /// Get the command by locale text
        /// </summary>
        /// <returns></returns>
        public PARAM_CMD getParamCmdByText(string text) {
            PARAM_CMD cmd = PARAM_CMD.ASSIGN;
            if (text != null) { 
                if(text.Equals(LangUtil.getMsg("ParamCmd.assign.text"))){
                    cmd = PARAM_CMD.ASSIGN ;
                }else if(text.Equals(LangUtil.getMsg("ParamCmd.setadd.text"))){
                    cmd = PARAM_CMD.UPDATE_SET_ADD ;
                }else if(text.Equals(LangUtil.getMsg("ParamCmd.setdel.text"))){
                    cmd = PARAM_CMD.UPDATE_SET_DEL ;
                }
            }
            return cmd;
        }
        /// <summary>
        /// return the locale based command type text or string.Empty if erros 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public string getParamCmdText(PARAM_CMD cmdType) {
            if (cmdType == PARAM_CMD.ASSIGN){
                return LangUtil.getMsg("ParamCmd.assign.text");                
            } else if (cmdType == PARAM_CMD.UPDATE_SET_ADD){
                return LangUtil.getMsg("ParamCmd.setadd.text");
            } else if (cmdType == PARAM_CMD.UPDATE_SET_DEL){
                return LangUtil.getMsg("ParamCmd.setdel.text");
            }
            return string.Empty;
        }
        /// <summary>
        /// Get the mapping source elment types, allowed is String || Number 
        /// </summary>
        /// <returns></returns>
        public List<string> getMappingSrcElemTypes() {
            List<string> list = new List<string>();
            list.Add(getParamTypeText(ParamType.STRING));
            list.Add(getParamTypeText(ParamType.NUMBER));
            list.Add(getParamTypeText(ParamType.DATETIME));
            return list;
        }
        /// <summary>
        /// Return the inext or 0 as default, this method should exactly reflect the method 
        /// getMappingSrcElemTypes(). 0 = STRING, 1 = NUMBER. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int getMappingSrcElemTypeIndex(ParamType type) {
            int r = 0;
            if (type == ParamType.STRING) {
                r = 0;
            } else if (type == ParamType.NUMBER) {
                r = 1;
            }
            return r;
        }
        /// <summary>
        /// Get the owner process of parameter. or return null if not found 
        /// </summary>
        /// <param name="rootProc"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Process getOwnerProcess(Process rootProc, Parameter param) {
            if (rootProc != null && param != null) {
                ParamGroup rootGrp = getRootParamGroup(param);
                if (rootGrp != null) {
                    if (rootProc.ParamPublic == rootGrp || rootProc.ParamPrivate == rootGrp) {
                        return rootProc;
                    } else {
                        foreach (Process proc in rootProc.Procs) {
                            if (proc.ParamPublic == rootGrp || proc.ParamPrivate == rootGrp) {
                                return proc;
                            }
                        }
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Get the root ParamGroup of parameter or null if errors 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ParamGroup getRootParamGroup(Parameter param) {
            if (param != null && param.Collection != null) {
                ParamGroup grp = param.Collection.Owner as ParamGroup;
                while (grp != null) {
                    if (grp.Collection == null) {
                        return grp;
                    } else {
                        grp = grp.Collection.Owner as ParamGroup;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// if type == String, return a string, if type == number, return a decimal.
        /// return string.Empty if the there are some errors.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        internal object getExpressionResult(Expression exp) {
            if (exp == null) {
                return string.Empty;
            }
            if (exp.Type == ParamType.STRING) {
                string s1 = getRuntimeCommonParamValue(exp.Input1) as string;
                string s2 = getRuntimeCommonParamValue(exp.Input2) as string;
                if (s1 != null && s2 != null) {
                    if (exp.Operator == "+") {
                        return s1 + s2;
                    }
                }
            } else if (exp.Type == ParamType.NUMBER) {
                object v1 = getRuntimeCommonParamValue(exp.Input1);
                object v2 = getRuntimeCommonParamValue(exp.Input2);
                decimal d1 = getDecimal(v1);
                decimal d2 = getDecimal(v2);
                if (d1 != decimal.MinValue && d2 != decimal.MinValue) {
                    if (exp.Operator == "+") {
                        return d1 + d2;
                    } else if (exp.Operator == "-") {
                        return d1 - d2;
                    } else if (exp.Operator == "*") {
                        return d1 * d2;
                    } else if (exp.Operator == "/") {
                        if (d2 != 0) {
                            return d1 / d2;
                        }
                    }
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Get the String or decimal value of Parameters for Expression/GF or string.Empty if errors. 
        /// 
        /// Notes: for Parameter if it is sensitive, it will return Constants.PARAM_SENS_ERR_MSG;
        /// 
        /// Input allowed : Constant, WebElementAttribute, Parameter, Expression, GlobalFunction.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object getRuntimeCommonParamValue(object input) {
            if (input is String || input is decimal || input is int) {
                return input;
            } else if (input is WebElementAttribute) {
                WebElementAttribute wea = input as WebElementAttribute;
                WebElement we = wea.Collection.Owner as WebElement;
                if (we.IsRealElement) {
                    return wea.RValue;
                } else {
                    Log.println_map("getParamInputValue - WEA, Error, WE not found. ");
                    return string.Empty;
                }
            } else if (input is Parameter) {
                Parameter param = input as Parameter;
                if (param.Sensitive) {
                    return Constants.PARAM_SENS_ERR_MSG;
                }
                if (param.RealValue != null) {
                    return param.RealValue;
                } else {
                    return param.DesignValue;
                }                
            } else if (input is Expression) {
                Expression exp = input as Expression;
                return exp.Result;
            } else if (input is GlobalFunction) {
                GlobalFunction gf = input as GlobalFunction;
                return gf.Result;
            }
            return string.Empty;
        }
        /// <summary>
        /// Whether can Get a string value from the parameter. 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool isStringValue(Parameter param) {
            bool r = false;
            if (param != null) {
                if (param.Type == ParamType.STRING || param.Type == ParamType.NUMBER || param.Type == ParamType.SET) {
                    r = true;
                }
            }
            return r;
        }
        /// <summary>
        /// Whether can get a number value from parameter 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool isNumberValue(Parameter param) {
            bool r = false;
            if (param != null) {
                if (param.Type == ParamType.NUMBER
                    || (param.Type == ParamType.SET && param.SetType == ParamType.NUMBER)) {
                    r = true;
                }
            }
            return r;
        }

        /// <summary>
        /// Return whether the obj maybe a string value, obj allowed :  
        /// string/decimal, WebElementAttribute, Parameter, Expression, GlobalFunction
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool isMaybeStringValue(object obj) {
            bool result = false;
            if (obj is string || obj is decimal) {
                result = true;
            } else if (obj is WebElementAttribute) {
                result = true;
            } else if (obj is Parameter) {
                result = isStringValue(obj as Parameter);
            } else if (obj is Expression) {
                Expression exp = obj as Expression;
                if (exp.Type == ParamType.STRING || exp.Type == ParamType.NUMBER) {
                    result = true;
                }
            } else if (obj is GlobalFunction) {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Return whether the obj maybe a number value, obj allowed :  
        /// string/decimal, WebElementAttribute, Parameter, Expression, GlobalFunction.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool isMaybeNumberValue(object obj) {
            bool result = false;
            if (obj is decimal) {
                result = true;
            } else if (obj is string) {
                decimal d = getDecimal(obj);
                if (d != decimal.MinValue) {
                    result = true;
                }
            } else if (obj is WebElementAttribute) {
                result = isWEAMaybeNumber(obj as WebElementAttribute);
            } else if (obj is Parameter) {
                result = isNumberValue(obj as Parameter);
            } else if (obj is Expression) {
                Expression exp = obj as Expression;
                if (exp.Type == ParamType.NUMBER) {
                    result = true;
                }
            } else if (obj is GlobalFunction) {
                GlobalFunction gf = obj as GlobalFunction;
                result = gf.Type == ParamType.NUMBER;
            }
            return result;
        }
        /// <summary>
        /// Get the mapping src object text or return string.Empty if errors . Allowed object is : 
        /// String, Number, WebElementAttribute, Parameter, Expression and GlobalFunction. 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public string getMappingSrcText(object src) {
            string text = string.Empty;
            if (src is string) {
                text = "\'" + src as string + "'";
            } else if (src is decimal) {
                text = src.ToString();
            } else if (src is WebElementAttribute) {
                WebElementAttribute wea = src as WebElementAttribute;
                WebElement we = wea.Collection.Owner as WebElement;
                text = we.Name + "." + wea.Name;
            } else if (src is Parameter) {
                Parameter param = src as Parameter;
                text = param.Name;
            } else if (src is Expression) {
                string left = null;
                string right = null;
                Expression exp = src as Expression;
                // check DateTime constants 
                if (exp.Type == ParamType.DATETIME) {
                    if (exp.Input1 is string) {
                        if (exp.Input1.ToString() == Constants.DATETIME_NOW) {
                            left = "Now";
                        } else{
                            left = exp.Input1.ToString();
                        }
                    }
                    if (exp.Input2 is string) {
                        if (exp.Input2.ToString() == Constants.DATETIME_NOW) {
                            right = "Now";
                        } else {
                            right = exp.Input2.ToString();
                        }
                    }
                }
                if (left == null) {
                    left = getMappingSrcText(exp.Input1);
                }
                if (left == null) {
                    right = getMappingSrcText(exp.Input2);
                }
                
                if (exp.Input1 is Expression) {
                    left = "(" + left + ")";
                }
                if (exp.Input2 is Expression) {
                    right = "(" + right + ")";
                }
                text = left + " " + exp.Operator + " " + right;
            } else if (src is GlobalFunction) {
                GlobalFunction gf = src as GlobalFunction;
                StringBuilder sb = new StringBuilder();
                sb.Append(GFManager.getGF_CMDNameText(gf.Cmd)).Append("(");
                for (int i = 0; i < gf.Params.Count; i++) {
                    object obj = gf.Params.get(i);
                    string s = null;
                    if (gf.Type == ParamType.DATETIME && obj is string) {
                        if (obj.ToString() == Constants.DATETIME_NOW) {
                            s = "Now";
                        } else {
                            s = obj.ToString();
                        }
                    }
                    if (s == null) {
                        s = getMappingSrcText(obj);
                    }
                    string t = ",";
                    if (i == (gf.Params.Count - 1)) {
                        t = "";
                    }
                    sb.Append(" " + s + " " + t);
                }
                sb.Append(")");

                text = sb.ToString();
            }
            return text;
        }
        /// <summary>
        /// Whether the newCmd is a valid command to be added into cmdList. A valid cmd in list, should :
        /// 1. newCmd is valid. 
        /// 2. cmd target parameter is unique in the tobe cmdList
        /// </summary>
        /// <param name="cmdList"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool isValidParamCmd(List<ParamCmd> cmdList, ParamCmd cmd) {
            if (cmdList == null || cmd == null || cmd.Target == null || cmd.Src == null) {
                return false;
            }
            foreach (ParamCmd pcmd in cmdList) {
                if (cmd.Target == pcmd.Target) {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Get the command's Source text, that is a string to identify the source part info. 
        /// e.g. it can be shown in the source expression text area.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string getParamCmdSrcText(ParamCmd cmd) {
            string text = string.Empty;
            if (cmd != null) {
                if (cmd.Src != null) {
                    text = getMappingSrcText(cmd.Src);
                }
            }
            return text;
        }
        /// <summary>
        /// Get the command's target text or string.Empty if errors. 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string getParamCmdToText(ParamCmd cmd) {
            string text = string.Empty;
            if (cmd != null) {
                if (cmd.Target != null) {
                    text = getMappingSrcText(cmd.Target);
                }
            }
            return text;
        }
        #endregion parameter mapping/updating/Expression/GlobalFunctions
        #region TODO 
        /// <summary>
        /// how many times the grp all sub parameters was referenced 
        /// </summary>
        /// <param name="grp"></param>
        /// <returns></returns>
        public int getParamGrpRefTimes(ParamGroup grp) {
            int count = 0;
            if (grp != null) {
                foreach (ParamGroup sub in grp.SubGroups) {
                    count += getParamGrpRefTimes(sub);
                }
                foreach (Parameter param in grp.Params) {
                    count += getParamRefTimes(param);
                }
            }
            return count;
        }
        /// <summary>
        /// how many times the parameter was referenced 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public int getParamRefTimes(Parameter param) {
            int count = 0;
            if (param != null) {
                count += (param.WeakRef.Count - 1);
            }
            return 0;
        }
        #endregion TODO                     
        #region Model Add/Remove fucntions 
        /// <summary>
        /// It is used to handle the assgin value operation in model. like Stub.propA = newObj scenario.
        /// Add stub object into newObj's weakRef, and update the previous object's weakRef
        /// </summary>
        /// <param name="current"></param>
        /// <param name="newObj"></param>
        /// <param name="stub">current and newObj is a property value for stub</param>
        internal void updateWeakRef_AssginValue(object current, object newObj, BaseElement stub) {
            if (stub != null) {
                if (newObj is BaseElement) {
                    BaseElement be = newObj as BaseElement;
                    be.WeakRef.Add(stub);
                }
                if (current is BaseElement) {
                    BaseElement be = current as BaseElement;
                    be.WeakRef.Remove(stub);                   
                }
            }
        }
        /// <summary>
        /// Remove a BaseElemnet from script model totally, and clean the cross references. 
        /// Scenario 1.
        /// 1. If be is a BEList contained element, it will be removed from the list.
        /// 2. For each property or item in children list. 
        ///       if true contained, do removeFromModel().
        ///       else if ref, remove the item--->BE ref. 
        /// 3. clean Be weakRef
        /// 4. be = null ;
        /// </summary>
        /// <param name="be"></param>
        //public void removeFromModel(BaseElement be) {
        //    if (be == null) {
        //        return;
        //    }
        //    if (be is WebElementAttribute) {
        //        removeFromModel(be as WebElementAttribute);
        //    } else if (be is WebElement) {
        //        removeFromModel(be as WebElement);
        //    } else if (be is WebElementGroup) {
        //        removeFromModel(be as WebElementGroup);
        //    } else if (be is Process) {                
        //        removeFromModel(be as Process);
        //    } else if (be is Operation) {
        //        removeFromModel(be as Operation);
        //    } else if (be is OperationRule) {
        //        removeFromModel(be as OperationRule);
        //    } else if (be is OpCondition) {
        //        removeFromModel(be as OpCondition);
        //    } else if (be is ConditionGroup) {
        //        removeFromModel(be as ConditionGroup);
        //    } else if (be is Condition) {
        //        removeFromModel(be as Condition);
        //    } else if (be is ParamGroup) {
        //        removeFromModel(be as ParamGroup);
        //    } else if (be is Parameter) {
        //        removeFromModel(be as Parameter);                
        //    } else if (be is Expression) {
        //        removeFromModel(be as Expression);
        //    } else if (be is GlobalFunction) {               
        //        removeFromModel(be as GlobalFunction);
        //    } else if (be is ParamCmd) {
        //        removeFromModel(be as ParamCmd);
        //    } else if (be is UserLog) {
        //        removeFromModel(be as UserLog);
        //    }
        //}
        /// <summary>
        /// Remove WEA from script model totally.
        /// </summary>
        /// <param name="wea"></param>
        public void removeFromModel(WebElementAttribute wea) {
            if (wea == null) {
                return;
            }
            // remove from the collection 
            if (wea.Collection != null) {
                wea.Collection.Remove(wea);
            }
            // clean the weak referrences 
            cleanWeakRef(wea);
            // clean the wea 
            wea = null;
        }
        /// <summary>
        /// Remove WE from script model totally.
        /// </summary>
        /// <param name="we"></param>
        public void removeFromModel(WebElement we) {
            if (we == null) {
                return;
            }
            // remove all WebElementAttributes 
            List<WebElementAttribute> list = new List<WebElementAttribute>();
            list.AddRange(we.Attributes.ToArray());            
            foreach (WebElementAttribute wea in list) {
                removeFromModel(wea);
            }
            // clean property weak ref
            we.refWebElement = null;

            // remove from the collection 
            if (we.Collection != null) {
                we.Collection.Remove(we);
            }
            // clean the weak referrences 
            cleanWeakRef(we);
            // clean the we
            we = null;
        }
        /// <summary>
        /// Remove weg from script model totally.
        /// </summary>
        /// <param name="weg"></param>
        public void removeFromModel(WebElementGroup weg) {
            if (weg == null) {
                return;
            }
            // remvoe all WebElementGroup sub groups 
            List<WebElementGroup> gs = new List<WebElementGroup>();
            gs.AddRange(weg.SubGroups.ToArray());
            foreach (WebElementGroup grp in gs) {
                removeFromModel(grp);
            }
            // remove all WebElement
            List<WebElement> elems = new List<WebElement>();
            elems.AddRange(weg.Elems.ToArray());
            foreach (WebElement we in elems) {
                removeFromModel(we);
            }
            // remove from group the collection 
            if (weg.Collection != null) {
                weg.Collection.Remove(weg);
            }
            // clean the weak referrences 
            cleanWeakRef(weg);
            // clean the weg
            weg = null;
        }
        /// <summary>
        /// remove operation from model totally.
        /// </summary>
        /// <param name="op"></param>
        public void removeFromModel(Operation op) {
            if (op == null) {
                return;
            }
            doRemoveFromModelOp(op);
            // remove from group the collection 
            if (op.Collection != null) {
                op.Collection.Remove(op);
            }
            // clean the weak referrences 
            cleanWeakRef(op);
            // clean the op
            op = null;
        }
        /// <summary>
        /// common operaiton for remove op from model. 
        /// </summary>
        /// <param name="op"></param>
        private void doRemoveFromModelOp(Operation op) {
            // remvoe all commands 
            List<ParamCmd> cmds = new List<ParamCmd>();
            cmds.AddRange(op.Commands.ToArray());
            foreach (ParamCmd cmd in cmds) {
                removeFromModel(cmd);
            }
            // remove all opcs 
            List<OpCondition> opcs = new List<OpCondition>();
            opcs.AddRange(op.OpConditions.ToArray());
            foreach (OpCondition opc in opcs) {
                removeFromModel(opc);
            }
            // remove all rules 
            List<OperationRule> rules = new List<OperationRule>();
            rules.AddRange(op.Rules.ToArray());
            foreach (OperationRule rule in rules) {
                removeFromModel(rule);
            }
            removeFromModel(op.LogStart, op);
            removeFromModel(op.LogEnd, op);
            op.Input = null;
            op.Element = null;
        }
        /// <summary>
        /// Remove Process from model totally
        /// </summary>
        /// <param name="proc"></param>
        public void removeFromModel(Process proc) {
            if (proc == null) {
                return;
            }            
            // remove all sub process 
            List<Process> procs = new List<Process>();
            procs.AddRange(proc.Procs.ToArray());
            foreach (Process p in procs) {
                removeFromModel(p);
            }
            // remove all operations 
            List<Operation> ops = new List<Operation>();
            ops.AddRange(proc.Ops.ToArray());
            foreach (Operation op in ops) {
                removeFromModel(op);
            }            
            removeFromModel(proc.ParamPublic, proc);
            removeFromModel(proc.ParamPrivate, proc);            
            doRemoveFromModelOp(proc);

            // remove from group the collection 
            if (proc.Collection != null) {
                proc.Collection.Remove(proc);
            }            
            // clean the weak referrences 
            cleanWeakRef(proc);
            // clean the we
            proc = null;
        }
        /// <summary>
        /// Remove ParamGroup from model totally. if the paramGroup is not the Process publis/private group, 
        /// stub must be set. 
        /// </summary>
        /// <param name="paramGroup"></param>
        public void removeFromModel(ParamGroup paramGroup, Process stub) {
            if (paramGroup == null || (paramGroup.Collection == null && stub==null)) {
                return;
            }

            // remove all subgroups 
            List<ParamGroup> grps = new List<ParamGroup>();
            grps.AddRange(paramGroup.SubGroups.ToArray());
            foreach (ParamGroup grp in grps) {
                removeFromModel(grp, null);
            }
            // remove all parameters 
            List<Parameter> ps = new List<Parameter>();
            ps.AddRange(paramGroup.Params.ToArray());
            foreach (Parameter p in ps) {
                removeFromModel(p);
            }
            // remove from collection 
            if (paramGroup.Collection != null) {
                paramGroup.Collection.Remove(paramGroup);
            } else {
                if (paramGroup == stub.ParamPublic) {
                    stub.ParamPublic = null;
                } else if (paramGroup == stub.ParamPrivate) {
                    stub.ParamPrivate = null;
                } else {
                    Log.println_eng("T = ENGINE, ERROR, remove paramGroup error. grp = "+paramGroup.Name+", stub = "+stub);
                }
            }
            // clean the weak referrences 
            cleanWeakRef(paramGroup);
            // clean the paramGrp
            paramGroup = null;
        }
        /// <summary>
        /// remove parameter form model totally
        /// </summary>
        /// <param name="param"></param>
        public void removeFromModel(Parameter param) {
            if (param == null) {
                return;
            }
            if (param.Collection != null) {
                param.Collection.Remove(param);
            }
            // clean the weak referrences 
            cleanWeakRef(param);
            // clean the parameter
            param = null;
        }
        /// <summary>
        /// Remove rule form model totally 
        /// </summary>
        /// <param name="rule"></param>
        public void removeFromModel(OperationRule rule) {
            if (rule == null) {
                return;
            }
            List<object> list = new List<object>();
            list.AddRange(rule.Params.ToArray());
            foreach (object obj in list) {
                rule.Params.Remove(obj);
            }

            if (rule.Collection != null) {
                rule.Collection.Remove(rule);
            }
            // clean the weak referrences 
            cleanWeakRef(rule);
            // clean the rule
            rule = null;
        }
        /// <summary>
        /// remove OpCondition from model totally. 
        /// </summary>
        /// <param name="opc"></param>
        public void removeFromModel(OpCondition opc) {
            if (opc == null) {
                return;
            }
            // update mapping command 
            List<ParamCmd> cmds = new List<ParamCmd>();            
            cmds.AddRange(opc.Mappings.ToArray());
            foreach (ParamCmd cmd in cmds) {
                removeFromModel(cmd);
            }            
            // remove conditionGroup 
            removeFromModel(opc.ConditionGroup, opc) ;      
            opc.Op = null ;
            
            if (opc.Collection != null) {
                opc.Collection.Remove(opc);
            }
            // clean the weak referrences 
            cleanWeakRef(opc);
            // clean the parameter
            opc = null;
        }
        /// <summary>
        /// remove condition group from model totally. 
        /// </summary>
        /// <param name="conGrp"></param>
        public void removeFromModel(ConditionGroup conGrp, OpCondition opc) {
            if (conGrp == null || (conGrp.Collection == null && opc == null)) {
                return;
            }
            // remove conditions / condition groups 
            List<BaseElement> belist = new List<BaseElement>();
            belist.AddRange(conGrp.Conditions.ToArray());
            foreach (BaseElement be in belist) {
                if (be is ConditionGroup) {
                    removeFromModel(be as ConditionGroup, null);
                } else if (be is Condition) {
                    removeFromModel(be as Condition);
                }
            }

            if (conGrp.Collection != null) {
                conGrp.Collection.Remove(conGrp);
            } else if (opc.ConditionGroup == conGrp) {
                opc.ConditionGroup = null;
            }
            // clean the weak referrences 
            cleanWeakRef(conGrp);
            // clean the conditionGroup
            conGrp = null;
        }
        /// <summary>
        /// remove condition from model totally. 
        /// </summary>
        /// <param name="con"></param>
        public void removeFromModel(Condition con) {
            if (con == null) {
                return;
            }
            con.Input1 = null;
            con.Input2 = null;

            if (con.Collection != null) {
                con.Collection.Remove(con);
            }
            // clean the weak referrences 
            cleanWeakRef(con);
            // clean the condition 
            con = null;
        }
        /// <summary>
        /// remove the globalFunction from model totally
        /// </summary>
        /// <param name="gf"></param>
        /// <param name="stub">maybe ParamCmd, Expression, or other GF</param>
        public void removeFromModel(GlobalFunction gf) {
            if (gf == null) {
                return;
            }
            // remove weak ref of parameters 
            List<object> list = new List<object>();
            list.AddRange(gf.Params.ToArray());
            foreach (object obj in list) {
                // This list is true contained element if it is Expression or GlobalFunction. 
                if (obj is GlobalFunction) {
                    removeFromModel(obj as GlobalFunction);
                } else if (obj is Expression) {
                    removeFromModel(obj as Expression);
                }
                gf.Params.Remove(obj);
            }

            if (gf.Collection != null) {
                gf.Collection.Remove(gf);
            }
            // clean the weak referrences 
            cleanWeakRef(gf);
            // clean the gf            
            gf = null;
        }
        public void removeFromModel(Expression exp) {
            if (exp == null) {
                return;
            }
            if (exp.Input1 is Expression) {
                // Expression is true contained by exp.input1
                Expression ep1 = exp.Input1 as Expression;
                removeFromModel(ep1);
            } else if (exp.Input1 is GlobalFunction) {
                // GF is true contained by exp.input1
                GlobalFunction gf1 = exp.Input1 as GlobalFunction;
                removeFromModel(gf1);
            }
            if (exp.Input2 is Expression) {
                // Expression is true contained by exp.input2
                Expression ep2 = exp.Input2 as Expression;
                removeFromModel(ep2);
            } else if (exp.Input2 is GlobalFunction) {
                // GF is true contained by exp.input2
                GlobalFunction gf2 = exp.Input2 as GlobalFunction;
                removeFromModel(gf2);
            }
            exp.Input1 = null;
            exp.Input2 = null;

            if (exp.Collection != null) {
                exp.Collection.Remove(exp);
            }
            // clean the weak referrences 
            cleanWeakRef(exp);
            // clean the expression            
            exp = null;
        }
        /// <summary>
        /// Remove cmd from script model totally. 
        /// </summary>
        /// <param name="cmd"></param>
        public void removeFromModel(ParamCmd cmd) {
            if (cmd == null) {
                return;
            }
            if (cmd.Src is Expression) {
                // Expression is true contained by cmd.Src 
                Expression exp = cmd.Src as Expression;
                removeFromModel(exp);
            } else if (cmd.Src is GlobalFunction) {
                GlobalFunction gf = cmd.Src as GlobalFunction;
                removeFromModel(gf);
            }
            cmd.Src = null;
            cmd.Target = null;

            if (cmd.Collection != null) {
                cmd.Collection.Remove(cmd);
            }
            // clean the weak referrences 
            cleanWeakRef(cmd);
            // clean the cmd
            cmd = null;
        }
        /// <summary>
        /// Remove the user log from model 
        /// </summary>
        /// <param name="userLog"></param>
        public void removeFromModel(UserLog userLog, Operation stub) {
            if (userLog == null || stub == null) {
                return;
            }
            List<UserLogItem> list = new List<UserLogItem>();
            list.AddRange(userLog.LogItems.ToArray());
            foreach (UserLogItem item in list) {
                removeFromModel(item);
            }

            if (userLog.Collection != null) {
                userLog.Collection.Remove(userLog);
            }
            // clean the weak referrences 
            cleanWeakRef(userLog);
            // clean the user log  
            if (stub.LogEnd == userLog) {
                stub.LogEnd = null;
            } else if (stub.LogStart == userLog) {
                stub.LogStart = null;
            }
            userLog = null;
        }
        /// <summary>
        /// Remove the user log item from model 
        /// </summary>
        /// <param name="logItem"></param>
        public void removeFromModel(UserLogItem logItem) {
            if (logItem == null) {
                return;
            }
            logItem.Item = null;
            
            if (logItem.Collection != null) {
                logItem.Collection.Remove(logItem);
            }
            // clean the weak referrences 
            cleanWeakRef(logItem);
            // clean the user log  
            logItem = null;
        }
        /// <summary>
        /// Clean all the weak referrences from the BaseElement. 
        /// 1. A ListRef referrenced to be 
        /// 2. be is an Stub element's property value. 
        /// </summary>
        /// <param name="be"></param>
        private void cleanWeakRef(BaseElement be) {
            if (be == null) {
                return;
            }
            List<object> olist = new List<object>();
            olist.AddRange(be.WeakRef.ToArray());
            foreach (object obj in olist) {
                if (obj is ListRef) {
                    ListRef listRef = obj as ListRef;
                    listRef.Remove(be);
                } else if (obj is BaseElement) {
                    cleanProperty(obj as BaseElement, be);
                } else {
                    be.WeakRef.Remove(obj);
                }
            }
        }
        /// <summary>
        /// Clean set the Property value of stubBE as null if its current value is propValue obj. 
        /// </summary>
        /// <param name="stubBE"></param>
        /// <param name="propValue"></param>
        private void cleanProperty(BaseElement stubBE, BaseElement propValue) {
            if (stubBE == null || propValue == null) {
                return;
            }
            // Using reflection to update the instance property value            
            PropertyInfo[] properties = stubBE.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (properties.Length <= 0) {
                return;
            }
            foreach (PropertyInfo item in properties) {                
                object value = item.GetValue(stubBE, null);
                if (value is BaseElement && value == propValue) {
                    item.SetValue(stubBE,null,null);
                    propValue.WeakRef.Remove(stubBE);
                }
            }
        }        
        #endregion Model Add/Remove fucntions
        #region model validation 
        /// <summary>
        /// Clean dirty object/references from BigModel. dirty means : 
        /// 1. An entity is not find from the true container palce. 
        /// 2. remove dirty reference that referreced a object that can not be found from the true container 
        /// </summary>
        /// <param name="model"></param>
        public void validateBigModel(BigModel model) { 
            //TODO 
        }
        /// <summary>
        /// clean dirty object/reference form the ScriptRoot model, dirty means: 
        /// 1. An entity is not find from the true container palce. 
        /// 2. remove dirty reference that referreced a object that can not be found from the true container 
        /// </summary>
        /// <param name="sroot"></param>
        public void validateScriptRoot(ScriptRoot sroot) {
            if (sroot == null) {
                return;
            }
                        
        }
        

        #endregion model validation 
        /// <summary>
        /// time format must be :
        /// HH:mm:ss, HH: 0-24, mm:0-59, ss :0-59
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool isValidTime(string time) {
            if (time != null && time.Length > 0) {
                string[] ts = time.Split(':');
                if (ts.Length == 3) {
                    decimal hh = getDecimal(ts[0]);
                    decimal mm = getDecimal(ts[1]);
                    decimal ss = getDecimal(ts[2]);
                    if (hh != decimal.MinValue && mm != decimal.MinValue && ss != decimal.MinValue) {
                        if (hh >= 0 && hh < 24 && mm >= 0 && mm < 60 && ss >= 0 && ss < 60) {
                            return true;
                        }
                    }                    
                }
            }
            return false;
        }
        /// <summary>
        /// Input is an operation exeTime string, with the format "HH:mm:ss", return DateTime object or DateTime.MinValue if errors 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private DateTime getDateTime(object input) {
            if (input is string) {
                string str = input as string;
                if (isValidTime(str)) {
                    string[] ts = str.Split(':');
                    int h = (int)getDecimal(ts[0]);
                    int m = (int)getDecimal(ts[1]);
                    int s = (int)getDecimal(ts[2]);
                    return new DateTime(1980, 12, 20, h, m, s);
                }
            }
            return DateTime.MinValue;
        }
        /// <summary>
        /// Input is an operation exeTime string, with the format "HH:mm:ss", 
        /// return DateTime ajusted by the date or DateTime.MinValue if errors 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime getOpExeDateTime(string time, DateTime date) {
            if (time != null && date != DateTime.MinValue) {
                DateTime t1 = getDateTime(time);                
                return new DateTime(date.Year, date.Month, date.Day, t1.Hour, t1.Minute, t1.Second);
            }
            return DateTime.MinValue;
        }
        /// <summary>
        /// check whether the obj maybe is a time string or parameter with time type.
        /// 
        /// Object can be string, Parameter, Expression, GF
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool isMaybeTimeValue(object obj) {
            if (obj is string) {
                return isValidTime(obj.ToString());
            } else if (obj is Parameter) {
                Parameter param = obj as Parameter;
                return param.Type == ParamType.DATETIME;
            } else if (obj is Expression) {
                Expression exp = obj as Expression;
                return exp.Type == ParamType.DATETIME;
            } else if (obj is GlobalFunction) {
                GlobalFunction gf = obj as GlobalFunction;
                return gf.Type == ParamType.DATETIME;
            }
            return false;
        }
        /// <summary>
        /// Check whether the obj is a parameter and it is sensitive 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool isSensitiveParam(object obj) {
            if (obj is Parameter) {
                Parameter param = obj as Parameter;
                return param.Sensitive;
            }
            return false;
        }
    }
}
