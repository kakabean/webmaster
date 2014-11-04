using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WebMaster.lib
{
    public static class HotkeyUtil
    {
        #region System API
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, HotkeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        /// <summary> 
        /// Register a hot key 
        /// </summary> 
        /// <param name="hWnd">window handler that will own the hotkey</param> 
        /// <param name="fsModifiers">ALT,CTRL,SHIFT</param> 
        /// <param name="vk">key</param> 
        /// <param name="callBack">Call back method when hotkey pressed</param> 
        public static void Regist(IntPtr hWnd, HotkeyModifiers fsModifiers, Keys vk, HotKeyCallBackHanlder callBack) {
            int id = keyid++;
            if (!RegisterHotKey(hWnd, id, fsModifiers, vk)) {
                throw new Exception("regist hotkey fail.");
            }
            keymap[id] = callBack;
        }

        /// <summary> 
        /// remove a hotkey
        /// </summary> 
        /// <param name="hWnd">Owner window of the hotkey</param> 
        /// <param name="callBack">Callback method</param> 
        public static void UnRegist(IntPtr hWnd, HotKeyCallBackHanlder callBack) {
            foreach (KeyValuePair<int, HotKeyCallBackHanlder> var in keymap) {
                if (var.Value == callBack) {
                    UnregisterHotKey(hWnd, var.Key);
                }
            }
        }
        /// <summary>
        /// process hotkey 
        /// </summary>
        /// <param name="m"></param>
        public static void ProcessHotKey(System.Windows.Forms.Message m) {
            if (m.Msg == WM_HOTKEY) {
                int id = m.WParam.ToInt32();
                HotKeyCallBackHanlder callback;
                if (keymap.TryGetValue(id, out callback)) {
                    callback();
                }
            }
        }

        const int WM_HOTKEY = 0x312;
        static int keyid = 8768;
        static Dictionary<int, HotKeyCallBackHanlder> keymap = new Dictionary<int, HotKeyCallBackHanlder>();

        public delegate void HotKeyCallBackHanlder();
    }

    public enum HotkeyModifiers
    {
        NONE = 0x0,
        ALT = 0x1,
        CTRL = 0x2,
        SHIFT = 0x4,
        WIN = 0x8
    }

}
