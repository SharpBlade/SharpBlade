using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sharparam.SharpBlade.Native;

namespace Sharparam.SharpBlade.Razer.Events
{
    public class KeyboardEventArgs : EventArgs
    {
        public readonly uint UMsg;
        public readonly UIntPtr WParam;
        public readonly IntPtr LParam;


        internal KeyboardEventArgs(uint uMsg, UIntPtr wParam, IntPtr lParam)
        {
            UMsg = uMsg;
            WParam = wParam;
            LParam = lParam;
        }
    }

    public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);
}
