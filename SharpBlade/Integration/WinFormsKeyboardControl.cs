// ---------------------------------------------------------------------------------------
// <copyright file="WinFormsKeyboardControl.cs" company="SharpBlade">
//     Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of
//     this software and associated documentation files (the "Software"), to deal in
//     the Software without restriction, including without limitation the rights to
//     use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//     of the Software, and to permit persons to whom the Software is furnished to do
//     so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//     CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//     Disclaimer: SharpBlade is in no way affiliated with Razer and/or any of
//     its employees and/or licensors. Adam Hellberg and/or Brandon Scott do not
//     take responsibility for any harm caused, direct or indirect, to any Razer
//     peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

namespace SharpBlade.Integration
{
    using System;
    using System.Windows.Forms;

    using SharpBlade.Native.WinAPI;

    /// <summary>
    /// Helper class to send key events to a WinForms control.
    /// </summary>
    internal sealed class WinFormsKeyboardControl : KeyboardControl
    {
        /// <summary>
        /// The WinForms <see cref="Control" /> upon which to raise key events.
        /// </summary>
        private readonly Control _control;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsKeyboardControl" /> class.
        /// </summary>
        /// <param name="control">A WinForms <see cref="Control" /> to raise key events on.</param>
        /// <param name="releaseOnEnter">Whether or not to release control when enter is pressed.</param>
        internal WinFormsKeyboardControl(Control control, bool releaseOnEnter)
            : base(releaseOnEnter)
        {
            _control = control;
        }

        /// <summary>
        /// Sends WM_CHAR to WinForms control.
        /// </summary>
        /// <param name="character">Character that was typed.</param>
        internal override void SendChar(char character)
        {
            User32.NativeMethods.PostMessage(
                _control.Handle,
                (uint)User32.MessageType.CHAR,
                (IntPtr)character,
                IntPtr.Zero);
        }

        /// <summary>
        /// Sends WM_KEYDOWN to WinForms control.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        internal override void SendKeyDown(VirtualKey key)
        {
            User32.NativeMethods.PostMessage(
                _control.Handle,
                (uint)User32.MessageType.KEYDOWN,
                (IntPtr)key,
                IntPtr.Zero);
        }

        /// <summary>
        /// Sends WM_KEYUP to WinForms control.
        /// </summary>
        /// <param name="key">Key that was released.</param>
        internal override void SendKeyUp(VirtualKey key)
        {
            User32.NativeMethods.PostMessage(
                _control.Handle,
                (uint)User32.MessageType.KEYUP,
                (IntPtr)key,
                IntPtr.Zero);
        }
    }
}
