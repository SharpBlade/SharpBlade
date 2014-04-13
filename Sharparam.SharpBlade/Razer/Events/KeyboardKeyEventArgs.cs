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
//     Disclaimer: SharpBlade is in no way affiliated
//     with Razer and/or any of its employees and/or licensors.
//     Adam Hellberg does not take responsibility for any harm caused, direct
//     or indirect, to any Razer peripherals via the use of SharpBlade.
// 
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

using System;

using Sharparam.SharpBlade.Extensions;
using Sharparam.SharpBlade.Native.WinAPI;

namespace Sharparam.SharpBlade.Razer.Events
{
    /// <summary>
    /// EventArgs class for the KeyboardKey event.
    /// </summary>
    public class KeyboardKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Key that had its state changed.
        /// </summary>
        public readonly User32.VirtualKey Key;

        /// <summary>
        /// Modifiers pressed.
        /// </summary>
        public readonly ModifierKeys Modifiers;

        /// <summary>
        /// Whether or not the shift key is pressed.
        /// </summary>
        public readonly bool ShiftPressed;

        /// <summary>
        /// Whether or not the control key is pressed.
        /// </summary>
        public readonly bool ControlPressed;

        /// <summary>
        /// Whether or not the alt key is pressed.
        /// </summary>
        public readonly bool AltPressed;

        /// <summary>
        /// Whether or not the caps lock key is active.
        /// </summary>
        public readonly bool CapsLockActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardKeyEventArgs" /> class.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        /// <param name="modifiers">Modifiers active during key press.</param>
        internal KeyboardKeyEventArgs(User32.VirtualKey key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;

            ShiftPressed = Modifiers.Has(ModifierKeys.Shift);
            ControlPressed = Modifiers.Has(ModifierKeys.Control);
            AltPressed = Modifiers.Has(ModifierKeys.Alt);
            CapsLockActive = Modifiers.Has(ModifierKeys.CapsLock);
        }
    }
}
