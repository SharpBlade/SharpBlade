// ---------------------------------------------------------------------------------------
// <copyright file="TargetDisplay.cs" company="SharpBlade">
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

using System.Diagnostics.CodeAnalysis;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Target displays available on SwitchBlade device.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "This is translated from native code.")]
    public enum TargetDisplay
    {
        /// <summary>
        /// The touchpad screen.
        /// </summary>
        Widget = 0x10000,

        /// <summary>
        /// Dynamic key #1.
        /// </summary>
        DK1 = 0x10001,

        /// <summary>
        /// Dynamic key #2.
        /// </summary>
        DK2 = 0x10002,

        /// <summary>
        /// Dynamic key #3.
        /// </summary>
        DK3 = 0x10003,

        /// <summary>
        /// Dynamic key #4.
        /// </summary>
        DK4 = 0x10004,

        /// <summary>
        /// Dynamic key #5.
        /// </summary>
        DK5 = 0x10005,

        /// <summary>
        /// Dynamic key #6.
        /// </summary>
        DK6 = 0x10006,

        /// <summary>
        /// Dynamic key #7.
        /// </summary>
        DK7 = 0x10007,

        /// <summary>
        /// Dynamic key #8.
        /// </summary>
        DK8 = 0x10008,

        /// <summary>
        /// Dynamic key #9.
        /// </summary>
        DK9 = 0x10009,

        /// <summary>
        /// Dynamic key #10.
        /// </summary>
        DK10 = 0x1000A
    }
}
