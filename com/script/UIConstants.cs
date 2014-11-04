using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMaster.com.script
{
    /// <summary>
    /// this class is used for constants values about UI
    /// </summary>
    public class UIConstants
    {
        /// <summary>
        /// flow properties view perferered height
        /// </summary>
        public static readonly int PREFER_PROPVIEW_HEIGHT = 232;
        /// <summary>
        /// How many pixels the label position from the connection start point. 
        /// </summary>
        public static readonly int CONN_LABEL_DISTANCE = 40;
        public static readonly int CONN_LABEL_OFFSET = 10;
        public static readonly string NAME_MANDATORY = "Name is Mandatory";
        public static readonly string WE_PANEL_SCRIPT_ROOT_IS_NULL = "You need create a script first ! ";
        /// <summary>
        /// default scripts load path 
        /// </summary>
        public static readonly string SCRIPT_PATH = "D:\\ZhangHui\\mywork\\WebMasterAll\\TestCases";
        // ####### user profile relative 
        public static readonly string USER_GUEST = "Guest";

        #region image names 
        public static readonly string IMG_SROOT = "script16.png";
        public static readonly string IMG_WEG = "wegrp16.gif";
        public static readonly string IMG_WE = "web_elem16.gif";
        public static readonly string IMG_WEA = "we_attr16.png";
        public static readonly string IMG_PARMGRP = "paramgrp16.gif";
        public static readonly string IMG_PARAM = "param16.gif";
        public static readonly string IMG_OP_START = "op_start16.gif";
        public static readonly string IMG_OP_END = "op_stop16.gif";
        public static readonly string IMG_OP_CLICK = "op_click16.gif";
        public static readonly string IMG_OP_INPUT = "op_input16.gif";
        public static readonly string IMG_OP_URL_T = "op_url16.png";
        public static readonly string IMG_OP_PROC = "op_process16.png";
        public static readonly string IMG_OP_NOP = "op_nop16.gif";
        public static readonly string IMG_TIME = "time16.gif";
        public static readonly string IMG_STR = "string16.png";
        public static readonly string IMG_GF_CATE = "gf_category16.gif";
        public static readonly string IMG_GF_CMD = "gf_cmd16.gif";
        #endregion image names 

        public static readonly string COLOR_MAPPING_EXP_BORDER = "ActiveCaption";
        public static readonly string COLOR_MAPPING_EXP_TEXT = "Blue";
        public static readonly string COLOR_BG_CANVAS = "LightBlue";        
    }
}
