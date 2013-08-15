/* WinAPI.cs
 *
 * Copyright © 2013 by Adam Hellberg and Brandon Scott.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Disclaimer: SharpBlade is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SharpBlade.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

using System;
using System.Runtime.InteropServices;

namespace Sharparam.SharpBlade.Native
{
    /// <summary>
    /// Native WinAPI functions.
    /// </summary>
    /// <remarks>Documentation from MSDN.</remarks>
    public static class WinAPI
    {
        // ReSharper disable InconsistentNaming
        // Functions

        /// <summary>
        /// Retrieves a handle to the specified standard device (standard input, standard output, or standard error).
        /// </summary>
        /// <param name="nStdHandle">The standard device.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the specified device,
        /// or a redirected handle set by a previous call to SetStdHandle.
        /// The handle has GENERIC_READ and GENERIC_WRITE access rights,
        /// unless the application has used SetStdHandle to set a standard handle with lesser access.
        /// If the function fails, the return value is INVALID_HANDLE_VALUE.
        /// To get extended error information, call GetLastError.
        /// If an application does not have associated standard handles,
        /// such as a service running on an interactive desktop,
        /// and has not redirected them, the return value is NULL.
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        /// <returns>
        /// If the function succeeds, the return value is nonzero (true).
        /// If the function fails, the return value is zero (false).
        /// To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// A process can be associated with only one console,
        /// so the AllocConsole function fails if the calling process already has a console.
        /// A process can use the FreeConsole function to detach itself from its current console,
        /// then it can call AllocConsole to create a new console or AttachConsole to attach to another console.
        /// If the calling process creates a child process, the child inherits the new console.
        /// AllocConsole initializes standard input, standard output,
        /// and standard error handles for the new console.
        /// The standard input handle is a handle to the console's input buffer,
        /// and the standard output and standard error handles are handles to the console's screen buffer.
        /// To retrieve these handles, use the GetStdHandle function.
        /// This function is primarily used by graphical user interface (GUI) application to create a console window.
        /// GUI applications are initialized without a console. Console applications are initialized with a console,
        /// unless they are created as detached processes (by calling the CreateProcess function with the DETACHED_PROCESS flag).
        /// </remarks>
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool AllocConsole();

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <returns>
        /// If the function succeeds, the return value is nonzero (true).
        /// If the function fails, the return value is zero (false).
        /// To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// A process can be attached to at most one console.
        /// If the calling process is not already attached to a console,
        /// the error code returned is ERROR_INVALID_PARAMETER (87).
        /// A process can use the FreeConsole function to detach itself from its console.
        /// If other processes share the console, the console is not destroyed,
        /// but the process that called FreeConsole cannot refer to it.
        /// A console is closed when the last process attached to it terminates or calls FreeConsole.
        /// After a process calls FreeConsole,
        /// it can call the AllocConsole function to create a new console or AttachConsole to attach to another console.
        /// </remarks>
        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int FreeConsole();

        /// <summary>
        /// The ShowScrollBar function shows or hides the specified scroll bar.
        /// </summary>
        /// <param name="hWnd">Handle to a scroll bar control or a window with a standard scroll bar, depending on the value of the wBar parameter.</param>
        /// <param name="wBar">Specifies the scroll bar(s) to be shown or hidden.</param>
        /// <param name="bShow">Specifies whether the scroll bar is shown or hidden. If this parameter is TRUE, the scroll bar is shown; otherwise, it is hidden.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero (true).
        /// If the function fails, the return value is zero (false). To get extended error information, call GetLastError. 
        /// </returns>
        /// <remarks>You should not call this function to hide a scroll bar while processing a scroll bar message. </remarks>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        /// <summary>
        /// The EnableScrollBar function enables or disables one or both scroll bar arrows.
        /// </summary>
        /// <param name="hWnd">Handle to a window or a scroll bar control, depending on the value of the wSBflags parameter.</param>
        /// <param name="wSBflags">Specifies the scroll bar type.</param>
        /// <param name="wArrows">Specifies whether the scroll bar arrows are enabled or disabled and indicates which arrows are enabled or disabled.</param>
        /// <returns>
        /// If the arrows are enabled or disabled as specified, the return value is nonzero (true).
        /// If the arrows are already in the requested state or an error occurs, the return value is zero (false).
        /// To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool EnableScrollBar(IntPtr hWnd, uint wSBflags, uint wArrows);

        /// <summary>
        /// Sends the specified message to a window or windows.
        /// The SendMessage function calls the window procedure
        /// for the specified window and does not return until
        /// the window procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose window procedure will receive the message.
        /// If this parameter is HWND_BROADCAST ((HWND)0xffff),
        /// the message is sent to all top-level windows in the system,
        /// including disabled or invisible unowned windows, overlapped windows,
        /// and pop-up windows; but the message is not sent to child windows.
        /// Message sending is subject to UIPI.
        /// The thread of a process can send messages only to message queues of
        /// threads in processes of lesser or equal integrity level.
        /// </param>
        /// <param name="Msg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        /// <remarks>
        /// When a message is blocked by UIPI the last error, retrieved with GetLastError, is set to 5 (access denied).
        /// Applications that need to communicate using HWND_BROADCAST should use the RegisterWindowMessage function to
        /// obtain a unique message for inter-application communication.
        /// The system only does marshalling for system messages (those in the range 0 to (WM_USER-1)).
        /// To send other messages (those >= WM_USER) to another process, you must do custom marshalling.
        /// If the specified window was created by the calling thread, the window procedure is called immediately
        /// as a subroutine. If the specified window was created by a different thread, the system switches to that
        /// thread and calls the appropriate window procedure. Messages sent between threads are processed only when
        /// the receiving thread executes message retrieval code. The sending thread is blocked until the receiving
        /// thread processes the message. However, the sending thread will process incoming nonqueued messages while
        /// waiting for its message to be processed. To prevent this, use SendMessageTimeout with SMTO_BLOCK set.
        /// For more information on nonqueued messages, see Nonqueued Messages.
        /// An accessibility application can use SendMessage to send WM_APPCOMMAND messages to the shell to launch
        /// applications. This functionality is not guaranteed to work for other types of applications.
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // Constants

        /// <summary>
        /// The standard input device. Initially, this is the console input buffer, CONIN$.
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;

        /// <summary>
        /// The standard output device. Initially, this is the active console screen buffer, CONOUT$.
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;

        /// <summary>
        /// The standard error device. Initially, this is the active console screen buffer, CONOUT$.
        /// </summary>
        public const int STD_ERROR_HANDLE = -12;

        /// <summary>
        /// The code page recommended for consoles spawned with <see cref="AllocConsole" />.
        /// </summary>
        public const int CODE_PAGE = 437;

        /// <summary>
        /// Horizontal scrollbar.
        /// </summary>
        public const int SB_HORZ = 0;

        /// <summary>
        /// Vertical scrollbar.
        /// </summary>
        public const int SB_VERT = 1;

        /// <summary>
        /// A scrollbar control.
        /// </summary>
        public const int SB_CTL  = 2;

        /// <summary>
        /// Both scrollbars (horizontal and vertical).
        /// </summary>
        public const int SB_BOTH = 3;

        /// <summary>
        /// Enables both arrows on a scroll bar.
        /// </summary>
        public const uint ESB_ENABLE_BOTH = 0;

        /// <summary>
        /// Disables both arrows on a scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_BOTH = 3;

        /// <summary>
        /// Disables the left arrow on a horizontal scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_LEFT = 1;

        /// <summary>
        /// Disables the right arrow on a horizontal scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_RIGHT = 2;

        /// <summary>
        /// Disables the up arrow on a vertical scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_UP = 1;

        /// <summary>
        /// Disables the down arrow on a vertical scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_DOWN = 2;

        /// <summary>
        /// Disables the left arrow on a horizontal scroll bar or the up arrow of a vertical scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_LTUP = 1;

        /// <summary>
        /// Disables the right arrow on a horizontal scroll bar or the down arrow of a vertical scroll bar.
        /// </summary>
        public const uint ESB_DISABLE_RTDN = 2;

        /// <summary>
        /// Posted to the window with the keyboard focus when a nonsystem key is pressed.
        /// A nonsystem key is a key that is pressed when the ALT key is not pressed. 
        /// </summary>
        public const int WM_KEYDOWN = 0x0100;

        /// <summary>
        /// Posted to the window with the keyboard focus when a nonsystem key is released.
        /// A nonsystem key is a key that is pressed when the ALT key is not pressed,
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        public const int WM_KEYUP = 0x0101;

        /// <summary>
        /// Posted to the window with the keyboard focus when a WM_KEYDOWN message is translated
        /// by the TranslateMessage function.
        /// The WM_CHAR message contains the character code of the key that was pressed.
        /// </summary>
        public const int WM_CHAR = 0x0102;

        /// <summary>
        /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's
        /// standard horizontal scroll bar. This message is also sent to the owner of a horizontal
        /// scroll bar control when a scroll event occurs in the control. 
        /// </summary>
        public const int WM_HSCROLL = 0x0114;

        /// <summary>
        /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's
        /// standard vertical scroll bar. This message is also sent to the owner of a vertical
        /// scroll bar control when a scroll event occurs in the control. 
        /// </summary>
        public const int WM_VSCROLL = 0x0115;

        #region Virtual-key codes

                /// <summary>
        /// Virtual-key codes used by the system.
        /// </summary>
        public enum VirtualKey
        {
            /// <summary>
            /// Left mouse button.
            /// </summary>
            LBUTTON = 0x01,

            /// <summary>
            /// Right mouse button.
            /// </summary>
            RBUTTON = 0x02,

            /// <summary>
            /// Control-break processing.
            /// </summary>
            CANCEL = 0x03,

            /// <summary>
            /// Middle mouse button (three-button mouse).
            /// </summary>
            MBUTTON = 0x04,

            /// <summary>
            /// X1 mouse button.
            /// </summary>
            XBUTTON1 = 0x05,

            /// <summary>
            /// X2 mouse button.
            /// </summary>
            XBUTTON2 = 0x06,

            /// <summary>
            /// BACKSPACE key.
            /// </summary>
            BACK = 0x08,

            /// <summary>
            /// TAB key.
            /// </summary>
            TAB = 0x09,

            /// <summary>
            /// CLEAR key.
            /// </summary>
            CLEAR = 0x0C,

            /// <summary>
            /// ENTER key.
            /// </summary>
            RETURN = 0x0D,

            /// <summary>
            /// SHIFT key.
            /// </summary>
            SHIFT = 0x10,

            /// <summary>
            /// CTRL key.
            /// </summary>
            CONTROL = 0x11,

            /// <summary>
            /// ALT key.
            /// </summary>
            MENU = 0x12,

            /// <summary>
            /// PAUSE key.
            /// </summary>
            PAUSE = 0x13,

            /// <summary>
            /// CAPS LOCK key.
            /// </summary>
            CAPITAL = 0x14,

            /// <summary>
            /// IME Kana mode.
            /// </summary>
            KANA = 0x15,

            /// <summary>
            /// IME Hanguel mode (maintained for compatibility; use VK_HANGUL).
            /// </summary>
            HANGUEL = 0x15,

            /// <summary>
            /// IME Hangul mode.
            /// </summary>
            HANGUL = 0x15,

            /// <summary>
            /// IME Junja mode.
            /// </summary>
            JUNJA = 0x17,

            /// <summary>
            /// IME final mode.
            /// </summary>
            FINAL = 0x18,

            /// <summary>
            /// IME Hanja mode.
            /// </summary>
            HANJA = 0x19,

            /// <summary>
            /// IME Kanji mode.
            /// </summary>
            KANJI = 0x19,

            /// <summary>
            /// ESC key.
            /// </summary>
            ESCAPE = 0x1B,

            /// <summary>
            /// IME convert.
            /// </summary>
            CONVERT = 0x1C,

            /// <summary>
            /// IME nonconvert.
            /// </summary>
            NONCONVERT = 0x1D,

            /// <summary>
            /// IME accept.
            /// </summary>
            ACCEPT = 0x1E,

            /// <summary>
            /// IME mode change request.
            /// </summary>
            MODECHANGE = 0x1F,

            /// <summary>
            /// SPACEBAR.
            /// </summary>
            SPACE = 0x20,

            /// <summary>
            /// PAGE UP key.
            /// </summary>
            PRIOR = 0x21,

            /// <summary>
            /// PAGE DOWN key.
            /// </summary>
            NEXT = 0x22,

            /// <summary>
            /// END key.
            /// </summary>
            END = 0x23,

            /// <summary>
            /// HOME key.
            /// </summary>
            HOME = 0x24,

            /// <summary>
            /// LEFT ARROW key.
            /// </summary>
            LEFT = 0x25,

            /// <summary>
            /// UP ARROW key.
            /// </summary>
            UP = 0x26,

            /// <summary>
            /// RIGHT ARROW key.
            /// </summary>
            RIGHT = 0x27,

            /// <summary>
            /// DOWN ARROW key.
            /// </summary>
            DOWN = 0x28,

            /// <summary>
            /// SELECT key.
            /// </summary>
            SELECT = 0x29,

            /// <summary>
            /// PRINT key.
            /// </summary>
            PRINT = 0x2A,

            /// <summary>
            /// EXECUTE key.
            /// </summary>
            EXECUTE = 0x2B,

            /// <summary>
            /// PRINT SCREEN key.
            /// </summary>
            SNAPSHOT = 0x2C,

            /// <summary>
            /// INS key.
            /// </summary>
            INSERT = 0x2D,

            /// <summary>
            /// DEL key.
            /// </summary>
            DELETE = 0x2E,

            /// <summary>
            /// HELP key.
            /// </summary>
            HELP = 0x2F,

            /// <summary>
            /// 0 key.
            /// </summary>
            Zero = 0x30,

            /// <summary>
            /// 1 key.
            /// </summary>
            One = 0x31,

            /// <summary>
            /// 2 key.
            /// </summary>
            Two = 0x32,

            /// <summary>
            /// 3 key.
            /// </summary>
            Three = 0x33,

            /// <summary>
            /// 4 key.
            /// </summary>
            Four = 0x34,

            /// <summary>
            /// 5 key.
            /// </summary>
            Five = 0x35,

            /// <summary>
            /// 6 key.
            /// </summary>
            Six = 0x36,

            /// <summary>
            /// 7 key.
            /// </summary>
            Seven = 0x37,

            /// <summary>
            /// 8 key.
            /// </summary>
            Eight = 0x38,

            /// <summary>
            /// 9 key.
            /// </summary>
            Nine = 0x39,

            /// <summary>
            /// A key.
            /// </summary>
            A = 0x41,

            /// <summary>
            /// B key.
            /// </summary>
            B = 0x42,

            /// <summary>
            /// C key.
            /// </summary>
            C = 0x43,

            /// <summary>
            /// D key.
            /// </summary>
            D = 0x44,

            /// <summary>
            /// E key.
            /// </summary>
            E = 0x45,

            /// <summary>
            /// F key.
            /// </summary>
            F = 0x46,

            /// <summary>
            /// G key.
            /// </summary>
            G = 0x47,

            /// <summary>
            /// H key.
            /// </summary>
            H = 0x48,

            /// <summary>
            /// I key.
            /// </summary>
            I = 0x49,

            /// <summary>
            /// J key.
            /// </summary>
            J = 0x4A,

            /// <summary>
            /// K key.
            /// </summary>
            K = 0x4B,

            /// <summary>
            /// L key.
            /// </summary>
            L = 0x4C,

            /// <summary>
            /// M key.
            /// </summary>
            M = 0x4D,

            /// <summary>
            /// N key.
            /// </summary>
            N = 0x4E,

            /// <summary>
            /// O key.
            /// </summary>
            O = 0x4F,

            /// <summary>
            /// P key.
            /// </summary>
            P = 0x50,

            /// <summary>
            /// Q key.
            /// </summary>
            Q = 0x51,

            /// <summary>
            /// R key.
            /// </summary>
            R = 0x52,

            /// <summary>
            /// S key.
            /// </summary>
            S = 0x53,

            /// <summary>
            /// T key.
            /// </summary>
            T = 0x54,

            /// <summary>
            /// U key.
            /// </summary>
            U = 0x55,

            /// <summary>
            /// V key.
            /// </summary>
            V = 0x56,

            /// <summary>
            /// W key.
            /// </summary>
            W = 0x57,

            /// <summary>
            /// X key.
            /// </summary>
            X = 0x58,

            /// <summary>
            /// Y key.
            /// </summary>
            Y = 0x59,

            /// <summary>
            /// Z key.
            /// </summary>
            Z = 0x5A,


            /// <summary>
            /// Left Windows key (Natural keyboard) .
            /// </summary>
            LWIN = 0x5B,

            /// <summary>
            /// Right Windows key (Natural keyboard).
            /// </summary>
            RWIN = 0x5C,

            /// <summary>
            /// Applications key (Natural keyboard).
            /// </summary>
            APPS = 0x5D,

            /// <summary>
            /// Computer Sleep key.
            /// </summary>
            SLEEP = 0x5F,

            /// <summary>
            /// Numeric keypad 0 key.
            /// </summary>
            NUMPAD0 = 0x60,

            /// <summary>
            /// Numeric keypad 1 key.
            /// </summary>
            NUMPAD1 = 0x61,

            /// <summary>
            /// Numeric keypad 2 key.
            /// </summary>
            NUMPAD2 = 0x62,

            /// <summary>
            /// Numeric keypad 3 key.
            /// </summary>
            NUMPAD3 = 0x63,

            /// <summary>
            /// Numeric keypad 4 key.
            /// </summary>
            NUMPAD4 = 0x64,

            /// <summary>
            /// Numeric keypad 5 key.
            /// </summary>
            NUMPAD5 = 0x65,

            /// <summary>
            /// Numeric keypad 6 key.
            /// </summary>
            NUMPAD6 = 0x66,

            /// <summary>
            /// Numeric keypad 7 key.
            /// </summary>
            NUMPAD7 = 0x67,

            /// <summary>
            /// Numeric keypad 8 key.
            /// </summary>
            NUMPAD8 = 0x68,

            /// <summary>
            /// Numeric keypad 9 key.
            /// </summary>
            NUMPAD9 = 0x69,

            /// <summary>
            /// Multiply key.
            /// </summary>
            MULTIPLY = 0x6A,

            /// <summary>
            /// Add key.
            /// </summary>
            ADD = 0x6B,

            /// <summary>
            /// Separator key.
            /// </summary>
            SEPARATOR = 0x6C,

            /// <summary>
            /// Subtract key.
            /// </summary>
            SUBTRACT = 0x6D,

            /// <summary>
            /// Decimal key.
            /// </summary>
            DECIMAL = 0x6E,

            /// <summary>
            /// Divide key.
            /// </summary>
            DIVIDE = 0x6F,

            /// <summary>
            /// F1 key.
            /// </summary>
            F1 = 0x70,

            /// <summary>
            /// F2 key.
            /// </summary>
            F2 = 0x71,

            /// <summary>
            /// F3 key.
            /// </summary>
            F3 = 0x72,

            /// <summary>
            /// F4 key.
            /// </summary>
            F4 = 0x73,

            /// <summary>
            /// F5 key.
            /// </summary>
            F5 = 0x74,

            /// <summary>
            /// F6 key.
            /// </summary>
            F6 = 0x75,

            /// <summary>
            /// F7 key.
            /// </summary>
            F7 = 0x76,

            /// <summary>
            /// F8 key.
            /// </summary>
            F8 = 0x77,

            /// <summary>
            /// F9 key.
            /// </summary>
            F9 = 0x78,

            /// <summary>
            /// F10 key.
            /// </summary>
            F10 = 0x79,

            /// <summary>
            /// F11 key.
            /// </summary>
            F11 = 0x7A,

            /// <summary>
            /// F12 key.
            /// </summary>
            F12 = 0x7B,

            /// <summary>
            /// F13 key.
            /// </summary>
            F13 = 0x7C,

            /// <summary>
            /// F14 key.
            /// </summary>
            F14 = 0x7D,

            /// <summary>
            /// F15 key.
            /// </summary>
            F15 = 0x7E,

            /// <summary>
            /// F16 key.
            /// </summary>
            F16 = 0x7F,

            /// <summary>
            /// F17 key.
            /// </summary>
            F17 = 0x80,

            /// <summary>
            /// F18 key.
            /// </summary>
            F18 = 0x81,

            /// <summary>
            /// F19 key.
            /// </summary>
            F19 = 0x82,

            /// <summary>
            /// F20 key.
            /// </summary>
            F20 = 0x83,

            /// <summary>
            /// F21 key.
            /// </summary>
            F21 = 0x84,

            /// <summary>
            /// F22 key.
            /// </summary>
            F22 = 0x85,

            /// <summary>
            /// F23 key.
            /// </summary>
            F23 = 0x86,

            /// <summary>
            /// F24 key.
            /// </summary>
            F24 = 0x87,

            /// <summary>
            /// NUM LOCK key.
            /// </summary>
            NUMLOCK = 0x90,

            /// <summary>
            /// SCROLL LOCK key.
            /// </summary>
            SCROLL = 0x91,

            /// <summary>
            /// Left SHIFT key.
            /// </summary>
            LSHIFT = 0xA0,

            /// <summary>
            /// Right SHIFT key.
            /// </summary>
            RSHIFT = 0xA1,

            /// <summary>
            /// Left CONTROL key.
            /// </summary>
            LCONTROL = 0xA2,

            /// <summary>
            /// Right CONTROL key.
            /// </summary>
            RCONTROL = 0xA3,

            /// <summary>
            /// Left MENU key.
            /// </summary>
            LMENU = 0xA4,

            /// <summary>
            /// Right MENU key.
            /// </summary>
            RMENU = 0xA5,

            /// <summary>
            /// Browser Back key.
            /// </summary>
            BROWSER_BACK = 0xA6,

            /// <summary>
            /// Browser Forward key.
            /// </summary>
            BROWSER_FORWARD = 0xA7,

            /// <summary>
            /// Browser Refresh key.
            /// </summary>
            BROWSER_REFRESH = 0xA8,

            /// <summary>
            /// Browser Stop key.
            /// </summary>
            BROWSER_STOP = 0xA9,

            /// <summary>
            /// Browser Search key .
            /// </summary>
            BROWSER_SEARCH = 0xAA,

            /// <summary>
            /// Browser Favorites key.
            /// </summary>
            BROWSER_FAVORITES = 0xAB,

            /// <summary>
            /// Browser Start and Home key.
            /// </summary>
            BROWSER_HOME = 0xAC,

            /// <summary>
            /// Volume Mute key.
            /// </summary>
            VOLUME_MUTE = 0xAD,

            /// <summary>
            /// Volume Down key.
            /// </summary>
            VOLUME_DOWN = 0xAE,

            /// <summary>
            /// Volume Up key.
            /// </summary>
            VOLUME_UP = 0xAF,

            /// <summary>
            /// Next Track key.
            /// </summary>
            MEDIA_NEXT_TRACK = 0xB0,

            /// <summary>
            /// Previous Track key.
            /// </summary>
            MEDIA_PREV_TRACK = 0xB1,

            /// <summary>
            /// Stop Media key.
            /// </summary>
            MEDIA_STOP = 0xB2,

            /// <summary>
            /// Play/Pause Media key.
            /// </summary>
            MEDIA_PLAY_PAUSE = 0xB3,

            /// <summary>
            /// Start Mail key.
            /// </summary>
            LAUNCH_MAIL = 0xB4,

            /// <summary>
            /// Select Media key.
            /// </summary>
            LAUNCH_MEDIA_SELECT = 0xB5,

            /// <summary>
            /// Start Application 1 key.
            /// </summary>
            LAUNCH_APP1 = 0xB6,

            /// <summary>
            /// Start Application 2 key.
            /// </summary>
            LAUNCH_APP2 = 0xB7,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the ';:' key.
            /// </summary>
            OEM_1 = 0xBA,

            /// <summary>
            /// For any country/region, the '+' key.
            /// </summary>
            OEM_PLUS = 0xBB,

            /// <summary>
            /// For any country/region, the ',' key.
            /// </summary>
            OEM_COMMA = 0xBC,

            /// <summary>
            /// For any country/region, the '-' key.
            /// </summary>
            OEM_MINUS = 0xBD,

            /// <summary>
            /// For any country/region, the '.' key.
            /// </summary>
            OEM_PERIOD = 0xBE,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the '/?' key.
            /// </summary>
            OEM_2 = 0xBF,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the '`~' key.
            /// </summary>
            OEM_3 = 0xC0,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the '[{' key.
            /// </summary>
            OEM_4 = 0xDB,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the '\|' key.
            /// </summary>
            OEM_5 = 0xDC,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the ']}' key.
            /// </summary>
            OEM_6 = 0xDD,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// For the US standard keyboard, the 'single-quote/double-quote' key.
            /// </summary>
            OEM_7 = 0xDE,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// </summary>
            OEM_8 = 0xDF,

            /// <summary>
            /// Either the angle bracket key or the backslash key on the RT 102-key keyboard.
            /// </summary>
            OEM_102 = 0xE2,

            /// <summary>
            /// IME PROCESS key.
            /// </summary>
            PROCESSKEY = 0xE5,

            /// <summary>
            /// Used to pass Unicode characters as if they were keystrokes.
            /// The VK_PACKET key is the low word of a 32-bit
            /// Virtual Key value used for non-keyboard input methods.
            /// For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP.
            /// </summary>
            PACKET = 0xE7,

            /// <summary>
            /// Attn key.
            /// </summary>
            ATTN = 0xF6,

            /// <summary>
            /// CrSel key.
            /// </summary>
            CRSEL = 0xF7,

            /// <summary>
            /// ExSel key.
            /// </summary>
            EXSEL = 0xF8,

            /// <summary>
            /// Erase EOF key.
            /// </summary>
            EREOF = 0xF9,

            /// <summary>
            /// Play key.
            /// </summary>
            PLAY = 0xFA,

            /// <summary>
            /// Zoom key.
            /// </summary>
            ZOOM = 0xFB,

            /// <summary>
            /// Reserved .
            /// </summary>
            NONAME = 0xFC,

            /// <summary>
            /// PA1 key.
            /// </summary>
            PA1 = 0xFD,

            /// <summary>
            /// Clear key.
            /// </summary>
            OEM_CLEAR = 0xFE,
        }

        #endregion Virtual-key codes

        // ReSharper restore InconsistentNaming
    }
}
