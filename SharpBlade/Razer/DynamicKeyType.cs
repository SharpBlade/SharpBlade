// ---------------------------------------------------------------------------------------
// <copyright file="DynamicKeyType.cs" company="SharpBlade">
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

using SharpBlade.Annotations;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Dynamic keys available on the SwitchBlade device.
    /// </summary>
    public enum DynamicKeyType
    {
        /// <summary>
        /// None of the keys.
        /// </summary>
        [UsedImplicitly]
        None = 0,

        /// <summary>
        /// Key #1.
        /// </summary>
        DK1,

        /// <summary>
        /// Key #2.
        /// </summary>
        DK2,

        /// <summary>
        /// Key #3.
        /// </summary>
        DK3,

        /// <summary>
        /// Key #4.
        /// </summary>
        DK4,

        /// <summary>
        /// Key #5.
        /// </summary>
        DK5,

        /// <summary>
        /// Key #6.
        /// </summary>
        DK6,

        /// <summary>
        /// Key #7.
        /// </summary>
        DK7,

        /// <summary>
        /// Key #8.
        /// </summary>
        DK8,

        /// <summary>
        /// Key #9.
        /// </summary>
        DK9,

        /// <summary>
        /// Key #10.
        /// </summary>
        DK10,

        /// <summary>
        /// Invalid dynamic key.
        /// </summary>
        [UsedImplicitly]
        Invalid,

        /// <summary>
        /// Number of keys available.
        /// </summary>
        [PublicAPI]
        Count = 10
    }
}
