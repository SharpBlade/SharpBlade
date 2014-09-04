// ---------------------------------------------------------------------------------------
// <copyright file="ScreenCapture.cs" company="SharpBlade">
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
using System.Drawing;
using System.Drawing.Imaging;

using SharpBlade.Logging;
using SharpBlade.Native.WinAPI;

namespace SharpBlade.Helpers
{
    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public static class ScreenCapture
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop.
        /// </summary>
        /// <returns>Image object of desktop screenshot.</returns>
        public static Image CaptureScreen()
        {
            return CaptureWindow(User32.NativeMethods.GetDesktopWindow());
        }

        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="fileName">
        /// Filename to save screenshot to.
        /// </param>
        /// <param name="format">
        /// Image format to save in.
        /// </param>
        public static void CaptureScreenToFile(string fileName, ImageFormat format)
        {
            var img = CaptureScreen();
            img.Save(fileName, format);
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window.
        /// </summary>
        /// <param name="handle">
        /// The handle to the window. (In windows forms, this is obtained by the Handle property).
        /// </param>
        /// <returns>
        /// Image object of a specific window.
        /// </returns>
        public static Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.NativeMethods.GetWindowDC(handle);

            // get the size
            var windowRect = new User32.Rect();
            User32.NativeMethods.GetWindowRect(handle, ref windowRect);
            var width = windowRect.Right - windowRect.Left;
            var height = windowRect.Bottom - windowRect.Top;

            // create a device context we can copy to
            var hdcDest = GDI32.NativeMethods.CreateCompatibleDC(hdcSrc);

            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            var bitmapHandle = GDI32.NativeMethods.CreateCompatibleBitmap(hdcSrc, width, height);

            // select the bitmap object
            var oldHandle = GDI32.NativeMethods.SelectObject(hdcDest, bitmapHandle);

            // bitblt over
            GDI32.NativeMethods.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);

            // restore selection
            GDI32.NativeMethods.SelectObject(hdcDest, oldHandle);

            // clean up
            GDI32.NativeMethods.DeleteDC(hdcDest);
            var released = User32.NativeMethods.ReleaseDC(handle, hdcSrc);

            // TODO: This could make for bad performance if the log call is frequent
            // Or is it safe to assume it won't happen often enough to be a concern?
            if (released == 0) // DC was not released
                LogManager.GetLogger(typeof(ScreenCapture)).Error("ReleaseDC returned zero");

            // get a .NET image object for it
            Image img = Image.FromHbitmap(bitmapHandle);

            // free up the Bitmap object
            GDI32.NativeMethods.DeleteObject(bitmapHandle);
            return img;
        }

        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle">
        /// Handle of window to capture.
        /// </param>
        /// <param name="fileName">
        /// Filename to save the captured image to.
        /// </param>
        /// <param name="format">
        /// Image format to save image in.
        /// </param>
        public static void CaptureWindowToFile(IntPtr handle, string fileName, ImageFormat format)
        {
            var img = CaptureWindow(handle);
            img.Save(fileName, format);
        }
    }
}
