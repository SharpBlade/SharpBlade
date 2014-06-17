// ---------------------------------------------------------------------------------------
// <copyright file="IRenderTarget.cs" company="SharpBlade">
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
using System.Drawing;

using SharpBlade.Integration;
using SharpBlade.Native;

namespace SharpBlade.Razer
{
    /// <summary>
    /// A class implementing methods to draw bitmaps and images
    /// to a supported target display.
    /// </summary>
    public interface IRenderTarget
    {
        /// <summary>
        /// Gets the path to the image currently shown on the render target,
        /// or null if no image is showing.
        /// </summary>
        string CurrentImage { get; }

        /// <summary>
        /// Gets the <see cref="RazerAPI.TargetDisplay" /> that content will be
        /// rendered to.
        /// </summary>
        RazerAPI.TargetDisplay TargetDisplay { get; }

        /// <summary>
        /// Clears anything drawing to the target display.
        /// Also clears the current image if one is set.
        /// </summary>
        /// <remarks>
        /// Will not attempt to clear the image if the object is disposing,
        /// in order to avoid sending commands to the hardware.
        /// </remarks>
        void Clear();

        /// <summary>
        /// Draws a bitmap to the target display.
        /// </summary>
        /// <param name="bitmap">The bitmap object to draw.</param>
        /// <remarks>
        /// The size of the bitmap object should match the size of the target
        /// display. For the <see cref="Touchpad" />, the size is specified by
        /// <see cref="RazerAPI.TouchpadWidth" /> and <see cref="RazerAPI.TouchpadHeight"/>,
        /// for dynamic keys, it's specified by <see cref="RazerAPI.DynamicKeyWidth" /> and
        /// <see cref="RazerAPI.DynamicKeyHeight" />.
        /// </remarks>
        void DrawBitmap(Bitmap bitmap);

        /// <summary>
        /// Sets an <see cref="IBitmapProvider" /> to provide the target display
        /// with a <see cref="Bitmap" /> object to draw.
        /// Initializes the polling interval to 42ms (circa 24 FPS).
        /// </summary>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        void SetBitmapProvider(IBitmapProvider provider);

        /// <summary>
        /// Sets an <see cref="IBitmapProvider" /> to provide the target display
        /// with a <see cref="Bitmap" /> object to draw.
        /// </summary>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        /// <param name="interval">How often to query the object for a bitmap and draw it.</param>
        void SetBitmapProvider(IBitmapProvider provider, TimeSpan interval);

        /// <summary>
        /// Set a static image to be displayed on the target display.
        /// </summary>
        /// <param name="image">Path to image.</param>
        void SetImage(string image);
    }
}
