/* WinAPI.cs
 *
 * Copyright © 2013 by Adam Hellberg
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
        // ReSharper restore InconsistentNaming
    }
}
