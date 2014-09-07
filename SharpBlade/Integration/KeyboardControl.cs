// ---------------------------------------------------------------------------------------
// <copyright file="KeyboardControl.cs" company="SharpBlade">
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

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

using SharpBlade.Logging;
using SharpBlade.Native.WinAPI;

namespace SharpBlade.Integration
{
    /// <summary>
    /// Helper class to send input to either a WinForms or WPF control.
    /// </summary>
    internal class KeyboardControl
    {
        /// <summary>
        /// Specifies if the caller wishes for keyboard capture
        /// to be disabled (released) after the ENTER key has
        /// been pressed.
        /// </summary>
        internal readonly bool ReleaseOnEnter;

        /// <summary>
        /// The shared <see cref="log4net.ILog" /> instance for this class.
        /// </summary>
        private static readonly log4net.ILog Log = LogManager.GetLogger(typeof(KeyboardControl));

        /// <summary>
        /// The WinForms control that receives keyboard input.
        /// This field is null if a WPF control is used.
        /// </summary>
        private readonly System.Windows.Forms.Control _winFormControl;

        /// <summary>
        /// The WPF control that receives keyboard input.
        /// This field is null if a WinForms control is used.
        /// </summary>
        private readonly System.Windows.Controls.Control _wpfControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardControl" /> class.
        /// </summary>
        /// <param name="control">The WinForms control to receive input.</param>
        /// <param name="releaseOnEnter">Whether or not to release control when enter is pressed.</param>
        internal KeyboardControl(System.Windows.Forms.Control control, bool releaseOnEnter)
        {
            _winFormControl = control;
            _wpfControl = null;
            ReleaseOnEnter = releaseOnEnter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardControl" /> class.
        /// </summary>
        /// <param name="control">The WPF control to receive input.</param>
        /// <param name="releaseOnEnter">Whether or not to release control when enter is pressed.</param>
        internal KeyboardControl(System.Windows.Controls.Control control, bool releaseOnEnter)
        {
            _winFormControl = null;
            _wpfControl = control;
            ReleaseOnEnter = releaseOnEnter;
        }

        /// <summary>
        /// Sends a char event to the active control.
        /// </summary>
        /// <param name="character">The character that was typed.</param>
        internal void SendChar(char character)
        {
            if (_winFormControl != null && !_winFormControl.IsDisposed)
                SendWinFormChar(character);
            else if (_wpfControl != null)
                SendWPFChar(character);
        }

        /// <summary>
        /// Sends a KeyDown event to the active control.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        internal void SendKeyDown(User32.VirtualKey key)
        {
            if (_winFormControl != null && !_winFormControl.IsDisposed)
                SendWinFormKeyDown(key);
            else if (_wpfControl != null)
                SendWPFKeyDown(key);
        }

        /// <summary>
        /// Sends a KeyUp event to the active control.
        /// </summary>
        /// <param name="key">The key that was released.</param>
        internal void SendKeyUp(User32.VirtualKey key)
        {
            if (_winFormControl != null && !_winFormControl.IsDisposed)
                SendWinFormKeyUp(key);
            else if (_wpfControl != null)
                SendWPFKeyUp(key);
        }

        /// <summary>
        /// Sends WM_CHAR to WinForms control.
        /// </summary>
        /// <param name="character">Character that was typed.</param>
        private void SendWinFormChar(char character)
        {
            User32.NativeMethods.PostMessage(
                _winFormControl.Handle,
                (uint)User32.MessageType.CHAR,
                (IntPtr)character,
                IntPtr.Zero);
        }

        /// <summary>
        /// Sends WM_KEYDOWN to WinForms control.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        private void SendWinFormKeyDown(User32.VirtualKey key)
        {
            User32.NativeMethods.PostMessage(
                _winFormControl.Handle,
                (uint)User32.MessageType.KEYDOWN,
                (IntPtr)key,
                IntPtr.Zero);
        }

        /// <summary>
        /// Sends WM_KEYUP to WinForms control.
        /// </summary>
        /// <param name="key">Key that was released.</param>
        private void SendWinFormKeyUp(User32.VirtualKey key)
        {
            User32.NativeMethods.PostMessage(
                _winFormControl.Handle,
                (uint)User32.MessageType.KEYUP,
                (IntPtr)key,
                IntPtr.Zero);
        }

        /// <summary>
        /// Sends a TextComposition event to the WPF control.
        /// </summary>
        /// <param name="character">The character that was typed.</param>
        private void SendWPFChar(char character)
        {
            // Because MS CBA to let us send chars directly
            var str = character.ToString(CultureInfo.InvariantCulture);
            _wpfControl.RaiseEvent(
                new TextCompositionEventArgs(
                    InputManager.Current.PrimaryKeyboardDevice,
                    new TextComposition(InputManager.Current, _wpfControl, str))
                {
                    RoutedEvent = TextCompositionManager.TextInputEvent
                });
        }

        /// <summary>
        /// Sends a WPF KeyDown event to the WPF control.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        private void SendWPFKeyDown(User32.VirtualKey key)
        {
            // Conversion magic, don't blink!
            var wpfKey = KeyInterop.KeyFromVirtualKey((int)key);

            var source = PresentationSource.FromVisual(_wpfControl);

            if (source == null)
            {
                Log.Error("SendWPFKeyDown: source is null");
                return;
            }

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, wpfKey)
                {
                    RoutedEvent = Keyboard.PreviewKeyDownEvent
                });

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, source, 1, wpfKey)
                {
                    RoutedEvent = Keyboard.KeyDownEvent
                });
        }

        /// <summary>
        /// Sends a WPF KeyUp event to the WPF control.
        /// </summary>
        /// <param name="key">Key that was released.</param>
        private void SendWPFKeyUp(User32.VirtualKey key)
        {
            // Conversion magic, don't blink!
            var wpfKey = KeyInterop.KeyFromVirtualKey((int)key);

            var source = PresentationSource.FromVisual(_wpfControl);

            if (source == null)
            {
                Log.Error("SendWPFKeyUp: source is null");
                return;
            }

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, wpfKey)
                {
                    RoutedEvent = Keyboard.PreviewKeyUpEvent
                });

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, source, 1, wpfKey)
                {
                    RoutedEvent = Keyboard.KeyUpEvent
                });
        }
    }
}
