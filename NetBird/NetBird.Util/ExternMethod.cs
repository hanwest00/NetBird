using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetBird.Util
{
    public class ExternMethod
    {
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern int GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern int SetForegroundWindow(int hwnd);
    }
}
