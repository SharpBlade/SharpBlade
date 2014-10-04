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

namespace SharpBlade.Integration
{
    using SharpBlade.Native.WinAPI;

    /// <summary>
    /// Helper class to send input to a GUI control.
    /// </summary>
    internal abstract class KeyboardControl
    {
        /// <summary>
        /// Specifies if the caller wishes for keyboard capture
        /// to be disabled (released) after the ENTER key has
        /// been pressed.
        /// </summary>
        internal readonly bool ReleaseOnEnter;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardControl" /> class.
        /// </summary>
        /// <param name="releaseOnEnter">Whether or not to release control when enter is pressed.</param>
        protected KeyboardControl(bool releaseOnEnter)
        {
            ReleaseOnEnter = releaseOnEnter;
        }

        /// <summary>
        /// Sends a char event to the active control.
        /// </summary>
        /// <param name="character">The character that was typed.</param>
        internal abstract void SendChar(char character);

        /// <summary>
        /// Sends a KeyDown event to the active control.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        internal abstract void SendKeyDown(VirtualKey key);

        /// <summary>
        /// Sends a KeyUp event to the active control.
        /// </summary>
        /// <param name="key">The key that was released.</param>
        internal abstract void SendKeyUp(VirtualKey key);
    }
}
