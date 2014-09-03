// ---------------------------------------------------------------------------------------
// <copyright file="KeyboardCallback.cs" company="SharpBlade">
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
using System.Runtime.InteropServices;

using SharpBlade.Native;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Function delegate for keyboard callback.
    /// </summary>
    /// <param name="uMsg">Indicates the keyboard event (WM_KEYDOWN, WM_KEYUP, WM_CHAR).</param>
    /// <param name="wParam">Indicates the key that was pressed (Virtual Key value).</param>
    /// <param name="lParam">Indicates key modifiers (CTRL, ALT, SHIFT).</param>
    /// <returns><see cref="HRESULT" /> object indicating success or failure</returns>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate HRESULT KeyboardCallback(uint uMsg, UIntPtr wParam, IntPtr lParam);
}
