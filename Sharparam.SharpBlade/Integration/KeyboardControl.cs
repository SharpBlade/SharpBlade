/* KeyboardControl.cs
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
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Sharparam.SharpBlade.Native;

namespace Sharparam.SharpBlade.Integration
{
    /// <summary>
    /// Helper class to send input to either a WinForms or WPF control.
    /// </summary>
    internal class KeyboardControl
    {
        private readonly System.Windows.Forms.Control _winFormControl;
        private readonly System.Windows.Controls.Control _wpfControl;

        /// <summary>
        /// Specifies if the caller wishes for keyboard capture
        /// to be disabled (released) after the ENTER key has
        /// been pressed.
        /// </summary>
        internal readonly bool ReleaseOnEnter;

        /// <summary>
        /// Creates a new WinForms keyboard control class.
        /// </summary>
        /// <param name="control">The WinForms control to receive input.</param>
        /// <param name="releaseOnEnter">Whether or not to relase control when enter is pressed.</param>
        internal KeyboardControl(System.Windows.Forms.Control control, bool releaseOnEnter)
        {
            _winFormControl = control;
            _wpfControl = null;
            ReleaseOnEnter = releaseOnEnter;
        }

        /// <summary>
        /// Creates a new WPF keyboard control class.
        /// </summary>
        /// <param name="control">The WPF control to receive input.</param>
        /// <param name="releaseOnEnter">Whether or not to relase control when enter is pressed.</param>
        internal KeyboardControl(System.Windows.Controls.Control control, bool releaseOnEnter)
        {
            _winFormControl = null;
            _wpfControl = control;
            ReleaseOnEnter = releaseOnEnter;
        }

        /// <summary>
        /// Sends a KeyDown event to the active control.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        internal void SendKeyDown(WinAPI.VirtualKey key)
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
        internal void SendKeyUp(WinAPI.VirtualKey key)
        {
            if (_winFormControl != null && !_winFormControl.IsDisposed)
                SendWinFormKeyUp(key);
            else if (_wpfControl != null)
                SendWPFKeyUp(key);
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

        private void SendWinFormKeyDown(WinAPI.VirtualKey key)
        {
            WinAPI.PostMessage(_winFormControl.Handle, (uint) WinAPI.MessageType.KEYDOWN, (IntPtr) key, IntPtr.Zero);
        }

        private void SendWinFormKeyUp(WinAPI.VirtualKey key)
        {
            WinAPI.PostMessage(_winFormControl.Handle, (uint) WinAPI.MessageType.KEYUP, (IntPtr) key, IntPtr.Zero);
        }

        private void SendWinFormChar(char character)
        {
            WinAPI.PostMessage(_winFormControl.Handle, (uint) WinAPI.MessageType.CHAR, (IntPtr) character, IntPtr.Zero);
        }

        private void SendWPFKeyDown(WinAPI.VirtualKey key)
        {
            // Conversion magic, don't blink!
            var wpfKey = KeyInterop.KeyFromVirtualKey((int) key);

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(_wpfControl), 0, wpfKey)
                {
                    RoutedEvent = Keyboard.PreviewKeyDownEvent
                });

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(_wpfControl), 1, wpfKey)
                {
                    RoutedEvent = Keyboard.KeyDownEvent
                });
        }

        private void SendWPFKeyUp(WinAPI.VirtualKey key)
        {
            // Conversion magic, don't blink!
            var wpfKey = KeyInterop.KeyFromVirtualKey((int) key);

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(_wpfControl), 0, wpfKey)
                {
                    RoutedEvent = Keyboard.PreviewKeyUpEvent
                });

            _wpfControl.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(_wpfControl), 1, wpfKey)
                {
                    RoutedEvent = Keyboard.KeyUpEvent
                });
        }

        private void SendWPFChar(char character)
        {
            // Because MS CBA to let us send chars directly
            var str = character.ToString(CultureInfo.InvariantCulture);
            _wpfControl.RaiseEvent(
                new TextCompositionEventArgs(InputManager.Current.PrimaryKeyboardDevice,
                                             new TextComposition(InputManager.Current, _wpfControl, str))
                {
                    RoutedEvent = TextCompositionManager.TextInputEvent
                });
        }
    }
}
