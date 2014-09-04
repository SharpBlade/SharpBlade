// ---------------------------------------------------------------------------------------
// <copyright file="KeyboardKeyEventArgs.cs" company="SharpBlade">
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

using SharpBlade.Extensions;
using SharpBlade.Native.WinAPI;

namespace SharpBlade.Events
{
    /// <summary>
    /// EventArgs class for the KeyboardKey event.
    /// </summary>
    public class KeyboardKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the alt key is pressed.
        /// </summary>
        private readonly bool _altPressed;

        /// <summary>
        /// Whether or not the caps lock key is active.
        /// </summary>
        private readonly bool _capsLockActive;

        /// <summary>
        /// Whether or not the control key is pressed.
        /// </summary>
        private readonly bool _controlPressed;

        /// <summary>
        /// Key that had its state changed.
        /// </summary>
        private readonly User32.VirtualKey _key;

        /// <summary>
        /// Modifiers pressed.
        /// </summary>
        private readonly ModifierKeys _modifiers;

        /// <summary>
        /// Whether or not the shift key is pressed.
        /// </summary>
        private readonly bool _shiftPressed;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardKeyEventArgs" /> class.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        /// <param name="modifiers">Modifiers active during key press.</param>
        internal KeyboardKeyEventArgs(User32.VirtualKey key, ModifierKeys modifiers)
        {
            _key = key;
            _modifiers = modifiers;

            _shiftPressed = _modifiers.Has(ModifierKeys.Shift);
            _controlPressed = _modifiers.Has(ModifierKeys.Control);
            _altPressed = _modifiers.Has(ModifierKeys.Alt);
            _capsLockActive = _modifiers.Has(ModifierKeys.CapsLock);
        }

        /// <summary>
        /// Gets a value indicating whether or not the alt key is pressed.
        /// </summary>
        public bool AltPressed
        {
            get
            {
                return _altPressed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the caps lock key is active.
        /// </summary>
        public bool CapsLockActive
        {
            get
            {
                return _capsLockActive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the control key is pressed.
        /// </summary>
        public bool ControlPressed
        {
            get
            {
                return _controlPressed;
            }
        }

        /// <summary>
        /// Gets the key that had its state changed.
        /// </summary>
        [CLSCompliant(false)]
        public User32.VirtualKey Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Gets the modifiers that were pressed.
        /// </summary>
        public ModifierKeys Modifiers
        {
            get
            {
                return _modifiers;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the shift key is pressed.
        /// </summary>
        public bool ShiftPressed
        {
            get
            {
                return _shiftPressed;
            }
        }
    }
}
