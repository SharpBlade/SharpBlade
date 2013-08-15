/* KeyboardControl.cs
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

namespace Sharparam.SharpBlade.Razer.Structs
{
    /// <summary>
    /// Information provided by keyboard-enabled forms. [WIP]
    /// </summary>
    [Obsolete("No longer used as the relevant functions in RazerAPI have been deprecated.")]
    public struct KeyboardControl
    {
        /// <summary>
        /// Handle to the form.
        /// </summary>
        public IntPtr Handle;

        /// <summary>
        /// Should keyboard capture be released when user presses enter?
        /// </summary>
        public bool ReleaseOnEnter;

        /// <summary>
        /// Creates a new <see cref="KeyboardControl" /> struct.
        /// </summary>
        /// <param name="handle">Handle to the form.</param>
        /// <param name="releaseOnEnter">Release control on enter?</param>
        public KeyboardControl(IntPtr handle, bool releaseOnEnter = true)
        {
            Handle = handle;
            ReleaseOnEnter = releaseOnEnter;
        }
    }
}
