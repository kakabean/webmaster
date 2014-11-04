using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.lib.engine
{
    public class Constants
    {
        /// <summary>
        /// if is DEV_MODE, it will not connect to server to check update, else 
        /// it will connect to server to check update each time app start. 
        /// </summary>
        public static readonly bool IS_DEV_MODE = true;

        // some special chars internal used, it is used to mark some char that user 
        // can inputed from the keyboard. 
        public static readonly string BLANK_TEXT = "\r";

        // html element tag attribute used by WebElement 
        public static readonly string HE_TAG = "tag";
        public static readonly string HE_ID = "id";
        public static readonly string HE_STYLE = "style";
        public static readonly string HE_INNER_TEXT = "innertext";
        public static readonly string HE_GINDEX = "gIndex";
        public static readonly string URL_BLANK = "";//"about:blank";

        /// <summary>
        /// model element string hidden seperation start flag, e.g for operaiton log 
        /// </summary>
        public static readonly string STR_START_FLAG = (char)0x03 + "";
        /// <summary>
        /// model element string hidden seperation end flag, e.g for operaiton log 
        /// </summary>
        public static readonly string STR_END_FLAG = (char)0x04 + ""; // mark the filed end 
        /// <summary>
        /// string seperator 
        /// </summary>
        public static readonly char STR_SEPERATOR = (char)0x1c;
        /// <summary>
        /// WebElement existed check time out 
        /// </summary>
        public static readonly int WE_CHECK_TIMEOUT = 10000;
        /// <summary>
        /// WebElement existed check time out for condition input
        /// </summary>        
        public static readonly int CONDITION_INPUT_WE_CHECK_TIMEOUT = 10000;//2000;
        /// <summary>
        /// default page load timeout = 1 minute.
        /// </summary>
        public static readonly int PAGE_LOAD_TIME_OUT = 60000;
        /// <summary>
        /// Max length of the condition input value that can be loged in APP log
        /// </summary>
        public static readonly int LOG_MAX_CON_INPUT_LENGTH = 256;
        public static readonly string PARAM_SENS_ERR_MSG = "Error Parameter";
        /// <summary>
        /// Just use one char to seperate different color.
        /// </summary>
        public static readonly int LOG_COLOR_OP = -12490271; // "RoyalBlue"; 
        public static readonly int LOG_COLOR_PROC = -16777077; // "DarkBlue"; 
        public static readonly int LOG_COLOR_WE = -8388480; // "Purple"; 
        public static readonly int LOG_COLOR_WEA = -16777077; //"DarkBlue"; 
        public static readonly int LOG_COLOR_OP_INPUT = -8355840; // "Olive"; 
        public static readonly int LOG_COLOR_OPC = -5952982; // "Brown"; 
        public static readonly int LOG_COLOR_CONGRP = -2987746; // "Chocolate"; 
        public static readonly int LOG_COLOR_CON = -2987746; // "Chocolate"; 
        public static readonly int LOG_COLOR_RESULT_TRUE = -16751616; // "DarkGreen"; 
        public static readonly int LOG_COLOR_RESULT_FALSE = -7667712; // "DarkRed"; 
        public static readonly int LOG_COLOR_VALUE = -8355840; // "Olive"; 
        public static readonly int LOG_COLOR_TIME = -16776961; // "Blue"; 
        public static readonly int LOG_COLOR_PARAM = -5952982; // "Brown"; 
        public static readonly int LOG_COLOR_STR = -16777216; // "Black"; 

        /// <summary>
        /// This is used for user log color mapping key. 
        /// </summary>
        public static readonly string LOG_USER_KEY_WEA = "WEA";
        public static readonly string LOG_USER_KEY_PARAM = "Parameter";
        public static readonly string LOG_USER_KEY_TIME = "Time";
        public static readonly string LOG_USER_KEY_STR = "string";
        /// <summary>
        /// It is only used for Expression and Condition input with type == ParamType.DateTime
        /// </summary>
        public static readonly string DATETIME_NOW = "Time_Now";
        /// <summary>
        /// User logon status, user current status is login
        /// </summary>
        public static readonly string USER_STATUS_LOGIN = "user.status.login";
        public static readonly string USER_STATUS_LOGOUT = "user.status.logout";
        /// <summary>
        /// Update exe file name 
        /// </summary>
        public static readonly string UPDATER_NAME = "update.exe";
        public static readonly string IDE_NAME = "MayiIDE.exe";
        public static readonly string BROWSER_NAME = "MayiBrowser.exe";

        public static readonly string LOCAL_CONFIG_XML = "userconfig.xml";
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
        /// <summary>
        /// This will be replace by config file later 
        /// </summary>
        public static readonly string WEB_HOME = "http://www.autowebkit.com/forum.php";
    }
}
