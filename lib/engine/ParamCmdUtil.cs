using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebMaster.lib.engine;
using WebMaster.lib;

namespace WebMaster.lib.engine
{
    class ParamCmdUtil
    {
        /// <summary>
        /// update the src value to the target parameter, update opw.Status = OpStatus.UPDATE_PARAM_WE_NOT_FOUND if WE not found .
        /// return true if update sucessfully, or return false if failed. 
        /// </summary>
        /// <param name="update"></param>
        /// <param name="opw"></param>
        /// <param name="engine"></param>
        /// <param name="sb_sysLog"></param>
        /// <returns></returns>
        public static bool doUpdateParameter(ParamCmd update,OpWrapper opw, WebEngine engine, StringBuilder sb_sysLog) {
            if (update == null || opw == null || engine == null || sb_sysLog == null) {
                return false;
            }
            // check and make sure all the WebElement was with real value if have
            // just check the first one if there is more than one candidate HtmlElement find 
            WebElement we = ModelManager.Instance.checkWE4ParamCmdIfNeed(update.Src, opw, engine, Constants.CONDITION_INPUT_WE_CHECK_TIMEOUT);
            // if there are one WebElement was not find when update parameters, just return.
            if (we != null && we.IsRealElement == false) {
                opw.Status = OpStatus.UPDATE_PARAM_WE_NOT_FOUND;
                opw.NullWE = we;
                opw.ParamCmd = update;
                // log mapping errors 
                engine.Logger.logParamCmdWENotFound(sb_sysLog,we);
                return false;
            }

            // handle Assign parameter value command 
            engine.Logger.logParamCmdExeBefore(update, sb_sysLog);
            if (update.Cmd == PARAM_CMD.ASSIGN) {                
                doAssignCMD(update,opw,engine);
            } else if (update.Cmd == PARAM_CMD.UPDATE_SET_ADD) {
                doSetAdd(update,opw,engine);
            }
            engine.Logger.logParamCmdExeEnd(update, sb_sysLog);
            return true;
        }

        private static void doSetAdd(ParamCmd update, OpWrapper opw, WebEngine engine) {
            //TODO throw new NotImplementedException();
        }
        /// <summary>
        /// Assign the src values to the target paramter value, only target effect for the 
        /// target param type == String || Number
        /// </summary>
        /// <param name="cmd"></param>
        private static void doAssignCMD(ParamCmd cmd,OpWrapper opw, WebEngine engine) {
            if (cmd == null) {
                Log.println_eng("ParamCmdUtil, ERROR, Assign command is null. ");
                return;
            } else if (cmd.Src == null || cmd.Target == null) {
                Log.println_eng("ParamCmdUtil, Error, Assign command parameters error, one parameter is null. ");
                return;
            }
            object param0 = cmd.Src;
            object value = ModelManager.Instance.getRuntimeCommonParamValue(param0);            
            Parameter param1 = cmd.Target as Parameter;
            if (param1 != null) {
                updateParamValue(param1, value);
            }
        }        
        /// <summary>
        /// only effective if the param is String or Number type 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        private static void updateParamValue(Parameter param, object value) {
            if (param == null) {
                return;
            }
            if (param.Type == ParamType.STRING) {
                param.RealValue = value;
            }
            if (param.Type == ParamType.NUMBER) {
                decimal dec = ModelManager.Instance.getDecimal(value);
                if (dec != decimal.MinValue) {
                    param.RealValue = dec;
                }                
            }
        }
    }
}
