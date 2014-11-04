using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;
using WebMaster.lib.ui;
using System.Text.RegularExpressions;

namespace WebMaster.lib.gf
{
    /// <summary>
    /// Global Function manager 
    /// </summary>
    public class GFManager
    {
        #region constants 
        public static readonly string GF_CATE_STR = "gf.category.string";
        public static readonly string GF_CATE_NUM = "gf.category.number";
        #endregion constants 
        #region i18n info
        /// <summary>
        /// Get the GlobalFunction category list. 
        /// </summary>
        /// <returns></returns>
        public static List<string> getGFCategoryTexts() {
            List<string> list = new List<string>();
            list.Add(getGF_CategoryNameText(GF_CATE_STR));
            list.Add(getGF_CategoryNameText(GF_CATE_NUM));
            return list;
        }
        /// <summary>
        /// return GF category name with locale or string.Empty if not found 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string getGF_CategoryNameText(string key) {
            string text = string.Empty;
            if (GF_CATE_STR == key || GF_CATE_NUM == key) {
                text = LangUtil.getMsg(key);
            }
            return text;
        }
        /// <summary>
        /// Get the string global function command list or empty list if errors 
        /// </summary>
        /// <returns></returns>
        public static List<GF_CMD> getStrGF_CMDList() {
            List<GF_CMD> list = new List<GF_CMD>();
            list.Add(GF_CMD.STR_GET_BTN_START_END);
            list.Add(GF_CMD.STR_GET_BY_START_END);
            list.Add(GF_CMD.STR_SPLIT);
            return list;
        }
        /// <summary>
        /// Get the Number global function command list or empty list if errors 
        /// </summary>
        /// <returns></returns>
        public static List<GF_CMD> getNumGF_CMDList() {
            List<GF_CMD> list = new List<GF_CMD>();
            list.Add(GF_CMD.NUM_GET_AVG);
            list.Add(GF_CMD.NUM_GET_MAX);
            list.Add(GF_CMD.NUM_GET_MIN);            
            return list;
        }
        /// <summary>
        /// Get GF_CMD by locale text or GF_CMD.NONE if not found
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static GF_CMD getGF_CMD(string text) {
            GF_CMD cmd = GF_CMD.NONE;
            if (text != null) {
                if (text.Equals(LangUtil.getMsg("gf.num.get_avg.name"))) {
                    cmd = GF_CMD.NUM_GET_AVG;
                } else if (text.Equals(LangUtil.getMsg("gf.num.get_max.name"))) {
                    cmd = GF_CMD.NUM_GET_MAX;
                } else if (text.Equals(LangUtil.getMsg("gf.num.get_min.name"))) {
                    cmd = GF_CMD.NUM_GET_MIN;
                } else if (text.Equals(LangUtil.getMsg("gf.str.get_by_bse.name"))) {
                    cmd = GF_CMD.STR_GET_BTN_START_END;
                } else if (text.Equals(LangUtil.getMsg("gf.str.get_by_se.name"))) {
                    cmd = GF_CMD.STR_GET_BY_START_END;
                } else if( text.Equals(LangUtil.getMsg("gf.str.split.name"))){
                    cmd = GF_CMD.STR_SPLIT ;
                }
            }
            return cmd;
        }
        /// <summary>
        /// return GF_CMD name with locale or string.Empty if not found 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string getGF_CMDNameText(GF_CMD cmd) {
            string text = string.Empty;
            if (cmd == GF_CMD.NUM_GET_AVG) {
                text = LangUtil.getMsg("gf.num.get_avg.name");
            } else if (cmd == GF_CMD.NUM_GET_MAX) {
                text = LangUtil.getMsg("gf.num.get_max.name");
            } else if (cmd == GF_CMD.NUM_GET_MIN) {
                text = LangUtil.getMsg("gf.num.get_min.name");
            } else if (cmd == GF_CMD.STR_GET_BTN_START_END) {
                text = LangUtil.getMsg("gf.str.get_by_bse.name");
            } else if (cmd == GF_CMD.STR_GET_BY_START_END) {
                text = LangUtil.getMsg("gf.str.get_by_se.name");
            } else if (cmd == GF_CMD.STR_SPLIT) {
                text = LangUtil.getMsg("gf.str.split.name");
            }
            return text;                
        }
        /// <summary>
        /// return GF_CMD description text with locale info or string.Empty if not found 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string getGF_CMDDesText(GF_CMD cmd) {
            string text = string.Empty;
            if (cmd == GF_CMD.NUM_GET_AVG) {
                text = LangUtil.getMsg("gf.num.get_avg.des");
            } else if (cmd == GF_CMD.NUM_GET_MAX) {
                text = LangUtil.getMsg("gf.num.get_max.des");
            } else if (cmd == GF_CMD.NUM_GET_MIN) {
                text = LangUtil.getMsg("gf.num.get_min.des");
            } else if (cmd == GF_CMD.STR_GET_BTN_START_END) {
                text = LangUtil.getMsg("gf.str.get_by_bse.des");
            } else if (cmd == GF_CMD.STR_GET_BY_START_END) {
                text = LangUtil.getMsg("gf.str.get_by_se.des");
            } else if (cmd == GF_CMD.STR_SPLIT) {
                text = LangUtil.getMsg("gf.str.split.des");
            }
            return text;
        }
        /// <summary>
        /// Get the command parameter name with english locale. or return string.Empty. 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="position">Parameter order in the parameter list</param>
        /// <returns></returns>
        public static string getCmdParamNameText(GF_CMD cmd, int position) {
            string name = string.Empty;
            if (cmd == GF_CMD.NUM_GET_AVG || cmd == GF_CMD.NUM_GET_MAX || cmd == GF_CMD.NUM_GET_MIN) {
                name = "num" + position;
            } else if (cmd == GF_CMD.STR_GET_BTN_START_END) {
                if (position == 0) {
                    name = "source";
                } else if (position == 1) {
                    name = "start";
                } else if (position == 2) {
                    name = "end";
                }
            } else if (cmd == GF_CMD.STR_GET_BY_START_END) {
                if (position == 0) {
                    name = "source";
                } else if (position == 1) {
                    name = "start";
                } else if (position == 2) {
                    name = "end";
                }
            } else if (cmd == GF_CMD.STR_SPLIT) {
                if (position == 0) {
                    name = "source";
                } else if (position == 1) {
                    name = "spliter";
                } else {
                    name = "Parameter";
                }
            }
            return name;
        }
        /// <summary>
        /// Get the command parameter description with locale. or return string.Empty. 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="position">Parameter order in the parameter list</param>
        /// <returns></returns>
        public static string getCmdParamDesText(GF_CMD cmd, int position) {
            string des = string.Empty;
            if (cmd == GF_CMD.NUM_GET_AVG || cmd == GF_CMD.NUM_GET_MAX || cmd == GF_CMD.NUM_GET_MIN) {
                des = LangUtil.getMsg("gf.num.get.param");
            } else if (cmd == GF_CMD.STR_GET_BTN_START_END) {
                if (position == 0) {
                    des = LangUtil.getMsg("gf.str.get_by_bse.p0");
                } else if (position == 1) {
                    des = LangUtil.getMsg("gf.str.get_by_bse.p1");
                } else if (position == 2) {
                    des = LangUtil.getMsg("gf.str.get_by_bse.p2");
                }
            } else if (cmd == GF_CMD.STR_GET_BY_START_END) {
                if (position == 0) {
                    des = LangUtil.getMsg("gf.str.get_by_se.p0");
                } else if (position == 1) {
                    des = LangUtil.getMsg("gf.str.get_by_se.p1");
                } else if (position == 2) {
                    des = LangUtil.getMsg("gf.str.get_by_se.p2");
                }
            } else if (cmd == GF_CMD.STR_SPLIT) {
                if (position == 0) {
                    des = LangUtil.getMsg("gf.str.split.p0");
                } else if (position == 1) {
                    des = LangUtil.getMsg("gf.str.split.p1");
                } else {
                    des = LangUtil.getMsg("gf.str.split.pn");
                }
            }
            return des;
        }
        #endregion i18n info 
        #region GF
        /// <summary>
        /// Result is a string or decimal or string.Empty(for string type)|decimal.MinValue(for number type) if errors 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        public static object getGFResult(GlobalFunction gf) {
            if (gf != null) {
                if (gf.Cmd == GF_CMD.NUM_GET_AVG) {
                    return getResult_NUM_GET_AVG(gf);
                } else if (gf.Cmd == GF_CMD.NUM_GET_MAX) {
                    return getResult_NUM_GET_MAX(gf);
                } else if (gf.Cmd == GF_CMD.NUM_GET_MIN) {
                    return getResult_NUM_GET_MIN(gf);
                } else if (gf.Cmd == GF_CMD.STR_GET_BTN_START_END) {
                    return getResult_STR_GET_BTN_START_END(gf);
                } else if (gf.Cmd == GF_CMD.STR_GET_BY_START_END) {
                    return getResult_STR_GET_BY_START_END(gf);
                } else if (gf.Cmd == GF_CMD.STR_SPLIT) {
                    return getResult_STR_SPLIT(gf);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// splt the src string with pattern regex, and set the sub strings into parameters inorder. 
        /// 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        private static object getResult_STR_SPLIT(GlobalFunction gf) {
            if (gf != null && gf.Cmd == GF_CMD.STR_SPLIT && gf.Params != null && gf.Params.Count >= 3) { 
                string src = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(0)).ToString();
                string pattern  = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(1)).ToString();
                pattern = validatePattern(pattern);
                Match match = Regex.Match(src, pattern);
                if (match.Success == false) {
                    Parameter p = gf.Params.get(2) as Parameter;
                    p.RealValue = "Split_Failed";
                } else {
                    string[] ss = Regex.Split(src, pattern);
                    for (int i = 0; i < ss.Length; i++) {
                        int index = i + 2;
                        if (index < gf.Params.Count) {
                            Parameter p = gf.Params.get(2 + i) as Parameter;
                            if (p.Type == ParamType.DATETIME || p.Type == ParamType.NUMBER || p.Type == ParamType.STRING) {
                                p.RealValue = ss[i];
                            }
                        } else {
                            break;
                        }
                    }
                }
            }
            return "Split_OK";
        }
        /// <summary>
        /// pattern start with [], it means that this is an regex string, else it is a normal string, 
        /// return proper pattern string or string.Empty if errors 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static string validatePattern(string pattern) {
            if (pattern != null) {
                StringBuilder sb = new StringBuilder();
                sb.Append(pattern);
                // handle regex, remove the start and end flag %
                if (pattern.StartsWith("%") && pattern.EndsWith("%")) {
                    sb.Remove(0, 1);
                    sb.Remove(sb.Length - 1, 1);
                } else {
                    // handle normal string, 
                    sb.Replace("$", @"\$");
                    sb.Replace("(", @"\(");
                    sb.Replace(")", @"\)");
                    sb.Replace("*", @"\*");
                    sb.Replace("+", @"\+");
                    sb.Replace(".", @"\.");
                    sb.Replace("[", @"\[");
                    sb.Replace("]", @"\]");
                    sb.Replace("?", @"\?");
                    sb.Replace(@"\", @"\\");
                    sb.Replace("/", @"\/");
                    sb.Replace("^", @"\^");
                    sb.Replace("{", @"\{");
                    sb.Replace("}", @"\}");
                    sb.Replace("|", @"\|");
                }
                return sb.ToString();
            }

            return string.Empty;
        }
        /// <summary>
        /// Get the substring that is from the start and end string, include start and end string. 
        /// or string.Empty if errors. 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        private static object getResult_STR_GET_BY_START_END(GlobalFunction gf) {
            string result = string.Empty;
            if (gf != null && gf.Cmd == GF_CMD.STR_GET_BY_START_END && gf.Params != null && gf.Params.Count == 3) {
                string source = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(0)).ToString();
                string start  = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(1)).ToString();
                string end    = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(2)).ToString();
                if (source.Length >0 && start.Length > 0 && end.Length > 0) {
                    int istart = source.IndexOf(start);
                    int iend = source.IndexOf(end);
                    if (istart != -1 && iend != -1 && istart < iend) {
                        // make sure the end string last char index is bigger than start string
                        int iend_last = iend + end.Length - 1;
                        int istart_last = istart + start.Length - 1;
                        if (iend_last >= istart_last) {
                            int length = iend_last - istart +1;
                            result = source.Substring(istart, length);
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Get the substring that is between the start and end string, NOT include start and end string. 
        /// or string.Empty if errors. 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        private static object getResult_STR_GET_BTN_START_END(GlobalFunction gf) {
            string result = string.Empty;
            if (gf != null && gf.Cmd == GF_CMD.STR_GET_BTN_START_END && gf.Params != null && gf.Params.Count == 3) {
                string source = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(0)).ToString();
                string start = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(1)).ToString();
                string end = ModelManager.Instance.getRuntimeCommonParamValue(gf.Params.get(2)).ToString();
                if (source.Length > 0 && start.Length > 0 && end.Length > 0) {
                    int istart = source.IndexOf(start);
                    int iend = source.IndexOf(end);
                    if (istart != -1 && iend != -1) {
                        // make sure the start string last char index is less than end string first char
                        int istart_last = istart + start.Length - 1;
                        if (istart_last < iend) {
                            int length = iend - istart_last -1 ;
                            result = source.Substring(istart_last+1 , length );
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Return the min value of all parameters or decimal.MinValue if errors. 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        private static decimal getResult_NUM_GET_MIN(GlobalFunction gf) {
            decimal result = decimal.MinValue;
            if (gf != null && gf.Params != null && gf.Params.Count > 0) {
                decimal min = decimal.MaxValue;
                foreach (object obj in gf.Params) {
                    decimal d = ModelManager.Instance.getDecimal(obj);
                    if (obj is Parameter) {
                        Parameter param = obj as Parameter;
                        if (param.Sensitive) {
                            d = decimal.MaxValue;
                        }
                    }
                    if (d != decimal.MinValue && d < min) {
                        min = d;
                    }
                }
                if (min != decimal.MaxValue) {
                    result = min;
                }
            }
            return result;
        }
        /// <summary>
        /// Return the min value of all parameters or decimal.MinValue if errors. 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        private static decimal getResult_NUM_GET_MAX(GlobalFunction gf) {
            decimal result = decimal.MinValue;
            if (gf != null && gf.Params != null && gf.Params.Count > 0) {
                decimal max = decimal.MinValue;
                foreach (object obj in gf.Params) {
                    decimal d = ModelManager.Instance.getDecimal(obj);
                    if (obj is Parameter) {
                        Parameter param = obj as Parameter;
                        if (param.Sensitive) {
                            d = decimal.MinValue;
                        }
                    }
                    if (d != decimal.MinValue && d> max) {
                        max = d;
                    }
                }
                if (max != decimal.MinValue) {
                    result = max;
                }
            }
            return result;
        }
        /// <summary>
        /// Get all parameters' average value or decimal.MinValue if errors 
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        private static decimal getResult_NUM_GET_AVG(GlobalFunction gf) {
            decimal result = decimal.MinValue;
            if (gf != null && gf.Params != null && gf.Params.Count > 0) {
                decimal total = 0;
                foreach (object obj in gf.Params) {
                    decimal d = ModelManager.Instance.getDecimal(obj);
                    if (obj is Parameter) {
                        Parameter param = obj as Parameter;
                        if (param.Sensitive) {
                            d = decimal.Zero;
                        }
                    }
                    if (d != decimal.MinValue) {
                        total += d;
                    }
                }
                result = total / gf.Params.Count;
            }
            return result;
        }
        /// <summary>
        /// Get the GF_CMD parameters count or 0 if errors, if the GF_CMD parameters count are not 
        /// fixed, it will returned init.MaxValue. 
        /// </summary>
        /// <param name="globalFunction"></param>
        /// <returns></returns>
        public static int getGF_CMDParamsCount(GlobalFunction gf) {
            int count = 0;
            if (gf != null) {
                if (gf.Cmd == GF_CMD.NUM_GET_AVG || gf.Cmd == GF_CMD.NUM_GET_MAX || gf.Cmd == GF_CMD.NUM_GET_MIN || gf.Cmd == GF_CMD.STR_SPLIT) {
                    count = int.MaxValue;
                } else if (gf.Cmd == GF_CMD.STR_GET_BTN_START_END || gf.Cmd == GF_CMD.STR_GET_BY_START_END) {
                    count = 3;
                }                
            }
            return count;
        }
        #endregion GF
        #region validation msg 
        /// <summary>
        /// Check whether the GlobalFunction is valid and give relative msg. If valid, return a msg with MsgType.VALID, if failed
        /// return error msg. 
        /// 1. ignore the name validation check. 
        /// 2. check the description max length exceed. 
        /// 3. check the Cmd/Type/Parameters matches. - String 
        /// 4. check the Cmd/Type/Parameters matches. - Number
        /// 5. check parameter with sensitive type
        /// </summary>
        /// <param name="gf"></param>
        /// <returns></returns>
        public static ValidationMsg getValidMsg(GlobalFunction gf) {
            // 1. ignore the name validation check. 
            string prefix = LangUtil.getMsg("model.GF.name"); //UtilFunction
            ValidationMsg msg = ModelManager.Instance.getInvalidNameMsg(gf, prefix);
            if (msg.Type != MsgType.VALID) {
                return msg;
            }
            // 2. check the description max length exceed.
            if (gf.Description != null && gf.Description.Length >= ModelManager.BE_DESC_MAX_LENGTH) {
                msg.Type = MsgType.ERROR;
                string txt1 = LangUtil.getMsg("valid.be.des.len.exceed.msg", ModelManager.BE_DESC_MAX_LENGTH); // description length exceed, max = {0}.
                msg.Msg = prefix + txt1;                
                return msg;
            }
            // 3. check the Cmd/Type/Parameters matches. - String 
            if (gf.Type == ParamType.STRING) {
                if (!(gf.Cmd == GF_CMD.STR_GET_BTN_START_END || gf.Cmd == GF_CMD.STR_GET_BY_START_END || gf.Cmd == GF_CMD.STR_SPLIT)) {
                    msg.Type = MsgType.ERROR;
                    msg.Msg = prefix + " - " + LangUtil.getMsg("valid.gf.type.msg1");// Type and command doesn't match.
                    return msg;
                }
                foreach (object obj in gf.Params) {
                    if (obj == null) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = prefix + " - " + LangUtil.getMsg("valid.gf.param.msg1");// Parameter can not be null
                        return msg;
                    }
                }
            }
            // 4. check the Cmd/Type/Parameters matches. - Number
            if (gf.Type == ParamType.NUMBER) {
                if(!(gf.Cmd == GF_CMD.NUM_GET_AVG || gf.Cmd == GF_CMD.NUM_GET_MAX || gf.Cmd == GF_CMD.NUM_GET_MIN)){
                    msg.Type = MsgType.ERROR;
                    msg.Msg = prefix + " - " + LangUtil.getMsg("valid.gf.type.msg1");// Type and command doesn't match.
                    return msg;
                }
                for(int i=0 ;i< gf.Params.Count; i++){
                    object obj = gf.Params.get(i);
                    bool maybe = ModelManager.Instance.isMaybeNumberValue(obj);
                    if (maybe == false) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = prefix + " - " + LangUtil.getMsg("valid.gf.type.msg2",i);// Parameter should be a number object. index = {0}
                        return msg;
                    }
                }
            }
            // 5. check parameter with sensitive type
            foreach (object obj in gf.Params) {
                if (obj is Parameter) {
                    Parameter param = obj as Parameter;
                    if (param.Sensitive) {
                        msg.Type = MsgType.ERROR;
                        msg.Msg = LangUtil.getMsg("valid.gf.param.msg3",prefix); // Sensitive parameter can not be used as {0} parameter
                        return msg;
                    }
                }
            }
            return msg;
        }
        #endregion validation msg 
    }
}
