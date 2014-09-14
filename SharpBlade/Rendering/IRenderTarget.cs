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
//     Disclaimer: SharpBlade is in no way affiliated with Razer and/or any of
//     its employees and/or licensors. Adam Hellberg and/or Brandon Scott do not
//     take responsibility for any harm caused, direct or indirect, to any Razer
//     peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

namespace SharpBlade.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;

    using SharpBlade.Annotations;
    using SharpBlade.Integration;
    using SharpBlade.Razer;

    /// <summary>
    /// A class implementing methods to draw bitmaps and images
    /// to a supported target display.
    /// </summary>
    public interface IRenderTarget : IDisposable
    {
        /// <summary>
        /// Gets the height of this <see cref="IRenderTarget" /> in pixels.
        /// </summary>
        int DisplayHeight { get; }

        /// <summary>
        /// Gets the width of this <see cref="IRenderTarget" /> in pixels.
        /// </summary>
        int DisplayWidth { get; }

        /// <summary>
        /// Gets the <see cref="IRenderer" /> instance used to
        /// manage timed rendering of objects.
        /// </summary>
        [PublicAPI]
        IRenderer Renderer { get; set; }

        /// <summary>
        /// Gets the <see cref="Razer.TargetDisplay" /> that content will be
        /// rendered to.
        /// </summary>
        TargetDisplay TargetDisplay { get; }

        /// <summary>
        /// Clears anything drawing to the target display.
        /// Also clears the current image if one is set.
        /// </summary>
        /// <remarks>
        /// Will not attempt to clear the image if the object is disposing,
        /// in order to avoid sending commands to the hardware.
        /// </remarks>
        [PublicAPI]
        void Clear();

        /// <summary>
        /// Draws a static image to the target display.
        /// </summary>
        /// <param name="image">Path to the image file.</param>
        [PublicAPI]
        void Draw(string image);

        /// <summary>
        /// Draws a bitmap to the target display.
        /// </summary>
        /// <param name="bitmap">The bitmap object to draw.</param>
        /// <remarks>
        /// The size of the bitmap object should match the size of the target
        /// display. For the <see cref="Touchpad" />, the size is specified by
        /// <see cref="Razer.Constants.TouchpadWidth" /> and <see cref="Razer.Constants.TouchpadHeight"/>,
        /// for dynamic keys, it's specified by <see cref="Razer.Constants.DynamicKeyWidth" /> and
        /// <see cref="Razer.Constants.DynamicKeyHeight" />.
        /// </remarks>
        void Draw(Bitmap bitmap);

        /// <summary>
        /// Draws the specified form to the target display.
        /// </summary>
        /// <param name="form">Form to draw.</param>
        [PublicAPI]
        void Draw(Form form);

        /// <summary>
        /// Draws the specified native window to the target display.
        /// </summary>
        /// <param name="windowHandle">The window handle of the window to draw.</param>
        [PublicAPI]
        void Draw(IntPtr windowHandle);

        /// <summary>
        /// Draws a WPF window to the target display.
        /// </summary>
        /// <param name="window">Window object to draw.</param>
        /// <param name="winFormsComponents">Array of KeyValuePairs containing a WindowsFormsHost as the key and a WinForms control as the value.
        /// These pairs will be overlaid on the bitmap that is passed to the SwitchBlade device.</param>
        [PublicAPI]
        void Draw(Window window, IEnumerable<EmbeddedWinFormsControl> winFormsComponents = null);

        /// <summary>
        /// Sets an <see cref="IBitmapProvider" /> to provide this <see cref="IRenderTarget" />
        /// with a <see cref="Bitmap" /> object to draw.
        /// </summary>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        /// <param name="interval">The interval at which to query the provider
        /// for a new <see cref="Bitmap" /> and draw it.</param>
        [PublicAPI]
        void Set(IBitmapProvider provider, int interval = 42);

        /// <summary>
        /// Sets a form to be rendered to this <see cref="IRenderTarget" />.
        /// </summary>
        /// <param name="form">The form to render.</param>
        /// <param name="method">The method to use for rendering the form.</param>
        /// <param name="interval">Interval (in milliseconds) at which to render the form
        /// (only used when <paramref name="method" /> is set to <see cref="RenderMethod.Polling" />.</param>
        [PublicAPI]
        void Set(Form form, RenderMethod method = RenderMethod.Polling, int interval = 42);

        /// <summary>
        /// Sets a native window to be rendered to this <see cref="IRenderTarget" />.
        /// </summary>
        /// <param name="handle">A handle to the window that is to be rendered.</param>
        /// <param name="interval">Interval (in milliseconds) at which to render the window.</param>
        [PublicAPI]
        void Set(IntPtr handle, int interval = 42);

        /// <summary>
        /// Sets a WPF window to be rendered to this <see cref="IRenderTarget" />.
        /// </summary>
        /// <param name="window">The window to render.</param>
        /// <param name="method">The method to use for rendering the window.</param>
        /// <param name="interval">Interval (in milliseconds) at which to render the window
        /// (only used when <paramref name="method" /> is set to <see cref="RenderMethod.Polling" />.</param>
        [PublicAPI]
        void Set(Window window, RenderMethod method = RenderMethod.Polling, int interval = 42);

        /// <summary>
        /// Set a static image to be displayed on the target display.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <param name="interval">The interval (in milliseconds) at which to refresh the image.</param>
        [PublicAPI]
        void Set(string image, int interval = 42);

        /// <summary>
        /// Sets the <see cref="IRenderer{T}" /> to be used for this <see cref="IRenderTarget" /> and
        /// calls its <see cref="M:IRenderer{T}.Start" /> method.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="RenderTarget" /> that the renderer is compatible with.
        /// </typeparam>
        /// <param name="renderer">An instance of the <see cref="IRenderer{T}" /> class.</param>
        [PublicAPI]
        void Set<T>(IRenderer<T> renderer) where T : class, IRenderTarget;
    }
}
