// ---------------------------------------------------------------------------------------
// <copyright file="GestureTypes.cs" company="SharpBlade">
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
using System.Diagnostics.CodeAnalysis;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Gesture types supported by the device.
    /// </summary>
    [Flags]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags",
        Justification = "This was translated from native code.")]
    public enum GestureTypes
    {
        /// <summary>
        /// Invalid or no gesture.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// A press on the touchpad.
        /// </summary>
        Press = 0x00000001,

        /// <summary>
        /// A tap on the touchpad.
        /// </summary>
        Tap = 0x00000002,

        /// <summary>
        /// Flick with finger(s?) on the touchpad.
        /// </summary>
        Flick = 0x00000004,

        /// <summary>
        /// To fingers pinching out on touchpad.
        /// </summary>
        Zoom = 0x00000008,

        /// <summary>
        /// Two fingers rotating on touchpad.
        /// </summary>
        Rotate = 0x00000010,

        /// <summary>
        /// Finger is moving around on touchpad.
        /// </summary>
        Move = 0x00000020,

        /// <summary>
        /// Finger is being held on touchpad.
        /// </summary>
        Hold = 0x00000040,

        /// <summary>
        /// Finger was released from touchpad.
        /// </summary>
        Release = 0x00000080,

        /// <summary>
        /// Scroll gesture.
        /// </summary>
        Scroll = 0x00000100,

        /// <summary>
        /// Every gesture.
        /// </summary>
        All = 0x0000FFFF
    }
}
