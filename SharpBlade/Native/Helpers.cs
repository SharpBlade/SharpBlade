// ---------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="SharpBlade">
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

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SharpBlade.Native
{
    /// <summary>
    /// Provides helper methods for working with native code.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Gets the message associated with the specified error code.
        /// </summary>
        /// <param name="err">The error code.</param>
        /// <returns>The message associated with this error code.</returns>
        public static string GetErrorMessage(int err)
        {
            return GetWin32Exception(err).Message;
        }

        /// <summary>
        /// Gets the last error code that was set by native code.
        /// </summary>
        /// <returns>The error code.</returns>
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// Gets the last error code and its associated info that was set by native code.
        /// </summary>
        /// <param name="message">Will be set to the message associated with the error code.</param>
        /// <returns>The error code.</returns>
        public static int GetLastErrorInfo(out string message)
        {
            var err = GetLastError();
            message = GetWin32Exception(err).Message;
            return err;
        }

        /// <summary>
        /// Gets the message associated with the last error.
        /// </summary>
        /// <returns>The message associated with the code of the last error.</returns>
        public static string GetLastErrorMessage()
        {
            return GetErrorMessage(GetLastError());
        }

        /// <summary>
        /// Gets a Win32Exception object for the last error code.
        /// </summary>
        /// <returns>An exception object with more detailed information about the last error.</returns>
        public static Win32Exception GetLastWin32Exception()
        {
            return GetWin32Exception(GetLastError());
        }

        /// <summary>
        /// Gets a Win32Exception object for the specified error code.
        /// </summary>
        /// <param name="err">The error code.</param>
        /// <returns>An exception object with more detailed information about the error.</returns>
        public static Win32Exception GetWin32Exception(int err)
        {
            return new Win32Exception(err);
        }
    }
}
