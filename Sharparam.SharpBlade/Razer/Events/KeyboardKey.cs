/* KeyboardKey.cs
 *
 * Copyright © 2013 by Adam Hellberg and Brandon Scott
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
 * Disclaimer: SwitchBlade is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SharpBlade.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

using System;
using Sharparam.SharpBlade.Extensions;
using Sharparam.SharpBlade.Native;

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
        public readonly WinAPI.VirtualKey Key;

        /// <summary>
        /// Modifiers pressed.
        /// </summary>
        public readonly ModifierKey Modifiers;

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

        internal KeyboardKeyEventArgs(WinAPI.VirtualKey key, ModifierKey modifiers)
        {
            Key = key;
            Modifiers = modifiers;

            ShiftPressed = Modifiers.Has(ModifierKey.Shift);
            ControlPressed = Modifiers.Has(ModifierKey.Control);
            AltPressed = Modifiers.Has(ModifierKey.Alt);
            CapsLockActive = Modifiers.Has(ModifierKey.CapsLock);
        }
    }

    /// <summary>
    /// Handler delegate for the KeyboardKey event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void KeyboardKeyEventHandler(object sender, KeyboardKeyEventArgs e);
}
