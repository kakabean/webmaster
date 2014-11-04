using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace WebMaster.lib.hooks
{
    #region Class HookEventArgs
    public class HookEventArgs : EventArgs
    {
        public int HookCode;	// Hook code
        public IntPtr wParam;	// WPARAM argument
        public IntPtr lParam;	// LPARAM argument
    }
    #endregion

    #region Enum HookType
    // Hook Types
    // For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }
    #endregion
    public class WinHook
    {
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        //Declare the hook handle as an int.
        protected int hHook = 0;
        //Declare MouseHookProcedure as a HookProc type.
        protected HookProc hookProc = null;
        protected HookType hookType;

        #region event handler
        public delegate void HookEventHandler(object sender, HookEventArgs e);
        // ************************************************************************

        // ************************************************************************
        // Event: HookInvoked 
        public event HookEventHandler HookInvoked;
        protected void OnHookInvoked(HookEventArgs e) {
            if (HookInvoked != null)
                HookInvoked(this, e);
        }
        #endregion event handler
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hookType">Hook type</param>
        public WinHook(HookType hookType) {
            this.hookType = hookType;
            hookProc = new HookProc(this.CoreHookProc);
        }
        public WinHook(HookType hookType, HookProc func) {
            this.hookType = hookType;
            hookProc = func;
        }

        protected int CoreHookProc(int code, IntPtr wParam, IntPtr lParam) {
            if (code < 0) {
                return CallNextHookEx(hHook, code, wParam, lParam);
            }
            // Let clients determine what to do
            HookEventArgs e = new HookEventArgs();
            e.HookCode = code;
            e.wParam = wParam;
            e.lParam = lParam;
            OnHookInvoked(e);

            // Yield to the next hook in the chain
            return CallNextHookEx(hHook, code, wParam, lParam);
        }
        /// <summary>
        /// install the hook function, real code is handled in HookInvoked event. 
        /// </summary>
        public void Install() {
            hHook = SetWindowsHookEx(hookType, hookProc, IntPtr.Zero, (int)AppDomain.GetCurrentThreadId());
        }

        public void Uninstall() {
            UnhookWindowsHookEx(hHook);
            hHook = 0;
        }

        public bool IsInstalled {
            get { return hHook != 0; }
        }

        //Declare the wrapper managed POINT class.
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        //Declare the wrapper managed MouseHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        #region Win32 API imports
        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //This is the Import for the CallNextHookEx function.
        //Use this function to pass the hook information to the next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        #endregion Win32 API imports
    }
}
