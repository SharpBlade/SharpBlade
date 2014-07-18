// ---------------------------------------------------------------------------------------
// <copyright file="RenderTarget.cs" company="SharpBlade">
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using SharpBlade.Helpers;
using SharpBlade.Integration;
using SharpBlade.Native;
using SharpBlade.Razer.Exceptions;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Common methods for rendering to a display.
    /// </summary>
    public abstract class RenderTarget : IRenderTarget, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget" /> class.
        /// </summary>
        /// <param name="targetDisplay">
        /// The <see cref="RazerAPI.TargetDisplay" /> to which content will be rendered.
        /// </param>
        internal RenderTarget(RazerAPI.TargetDisplay targetDisplay, int height, int width)
        {
            DisplayHeight = height;
            DisplayWidth = width;
            TargetDisplay = targetDisplay;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RenderTarget" /> class.
        /// Allows an object to try to free resources and perform other
        /// cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~RenderTarget()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the currently active form, null if no form is set.
        /// </summary>
        public Form CurrentForm { get; protected set; }

        /// <summary>
        /// Gets or sets the current image being displayed on the render target.
        /// </summary>
        public abstract string CurrentImage { get; protected set; }

        /// <summary>
        /// Gets the currently rendering Native window, <c>IntPtr.Zero</c> if no window set
        /// </summary>
        public IntPtr CurrentNativeWindow { get; protected set; }

        /// <summary>
        /// Gets the currently rendering WPF window, null if no window is set.
        /// </summary>
        public Window CurrentWindow { get; protected set; }

        /// <summary>
        /// Gets the height of this <see cref="IRenderTarget" /> in pixels.
        /// </summary>
        public int DisplayHeight { get; private set; }

        /// <summary>
        /// Gets the width of this <see cref="IRenderTarget" /> in pixels.
        /// </summary>
        public int DisplayWidth { get; private set; }

        /// <summary>
        /// Gets the <see cref="RazerAPI.TargetDisplay" /> that content will be
        /// rendered to.
        /// </summary>
        public RazerAPI.TargetDisplay TargetDisplay { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Integration.IRenderer" /> instance used to
        /// manage timed rendering of objects.
        /// </summary>
        internal IRenderer Renderer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object has been disposed.
        /// </summary>
        protected bool Disposed { get; set; }

        /// <summary>
        /// Clears anything drawing to the target display.
        /// Also clears the current image if one is set.
        /// </summary>
        /// <remarks>
        /// Will not attempt to clear the image if the object is disposing,
        /// in order to avoid sending commands to the hardware.
        /// </remarks>
        public void Clear()
        {
            Clear(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Draws a <see cref="Bitmap" /> to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="bitmap">The bitmap object to draw.</param>
        /// <remarks>
        /// The size of the bitmap object should match the size of the target
        /// display. For the <see cref="Touchpad" />, the size is specified by
        /// <see cref="RazerAPI.TouchpadWidth" /> and <see cref="RazerAPI.TouchpadHeight"/>,
        /// for dynamic keys, it's specified by <see cref="RazerAPI.DynamicKeyWidth" /> and
        /// <see cref="RazerAPI.DynamicKeyHeight" />.
        /// </remarks>
        public void DrawBitmap(Bitmap bitmap)
        {
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format16bppRgb565);

            var buffer = new RazerAPI.BufferParams
            {
                PixelType = RazerAPI.PixelType.RGB565,
                DataSize = (uint)(bitmap.Width * bitmap.Height * sizeof(ushort)),
                PtrData = data.Scan0
            };

            var ptrToImageStruct = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
            Marshal.StructureToPtr(buffer, ptrToImageStruct, true);

            var result = RazerAPI.NativeMethods.RzSBRenderBuffer(TargetDisplay, ptrToImageStruct);

            // Free resources before handling return
            Marshal.FreeHGlobal(ptrToImageStruct);
            bitmap.UnlockBits(data);

            bitmap.Dispose();

            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBRenderBuffer", result);
        }

        /// <summary>
        /// Draws the specified form to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="form">Form to draw.</param>
        public void DrawForm(Form form)
        {
            var bmp = new Bitmap(DisplayWidth, DisplayHeight);
            form.DrawToBitmap(bmp, form.Bounds);
            DrawBitmap(bmp);
            bmp.Dispose();
        }

        /// <summary>
        /// Draws the specified native window to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="windowHandle">The window handle of the window to draw.</param>
        public void DrawNativeWindow(IntPtr windowHandle)
        {
            var img = ScreenCapture.CaptureWindow(windowHandle);
            var bitmapToRender = new Bitmap(img, DisplayWidth, DisplayHeight);
            DrawBitmap(bitmapToRender);
            bitmapToRender.Dispose();
            img.Dispose();
        }

        /// <summary>
        /// Draws a WPF window to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="window">Window object to draw.</param>
        /// <param name="winFormsComponents">Array of KeyValuePairs containing a WindowsFormsHost as the key and a WinForms control as the value.
        /// These pairs will be overlaid on the bitmap that is passed to the SwitchBlade device.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "SO said it's safe to dispose MemoryStream multiple times")]
        public void DrawWindow(Window window, IEnumerable<EmbeddedWinFormsControl> winFormsComponents = null)
        {
            var rtb = new RenderTargetBitmap(
                DisplayWidth,
                DisplayHeight,
                96,
                96,
                System.Windows.Media.PixelFormats.Pbgra32);

            rtb.Render(window);

            BitmapEncoder encoder = new BmpBitmapEncoder();

            // CA2202 warning marked here, for reference, complains about possible multiple dispose of MemoryStream stream
            using (var stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream);

                using (var bitmap = new Bitmap(stream))
                {
                    if (winFormsComponents != null)
                    {
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            foreach (var component in winFormsComponents)
                                graphics.DrawImage(component.Draw(), component.Bounds);
                        }
                    }

                    DrawBitmap(bitmap);
                }

                encoder.Frames.Clear();
            }

            rtb.Clear();
        }

        /// <summary>
        /// Sets an <see cref="IBitmapProvider" /> to provide the target display
        /// with a <see cref="Bitmap" /> object to draw.
        /// Initializes the polling interval to 42ms (circa 24 FPS).
        /// </summary>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        public void SetBitmapProvider(IBitmapProvider provider)
        {
            SetBitmapProvider(provider, new TimeSpan(0, 0, 0, 0, 42));
        }

        /// <summary>
        /// Sets an <see cref="IBitmapProvider" /> to provide the target display
        /// with a <see cref="Bitmap" /> object to draw.
        /// </summary>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        /// <param name="interval">How often to query the object for a bitmap and draw it.</param>
        public void SetBitmapProvider(IBitmapProvider provider, TimeSpan interval)
        {
            Clear();
            Renderer = new BitmapRenderer(this, provider, interval);
        }

        /// <summary>
        /// Set a static image to be displayed on the target display.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public abstract void SetImage(string image);

        /// <summary>
        /// Sets the form to be rendered to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="form">The new form to render.</param>
        /// <param name="method">The method to use for rendering the form.</param>
        /// <param name="interval">Interval to poll drawing functions at,
        /// only used if RenderMethod is set to Polling.
        /// Default value 55ms (circa 18 FPS).</param>
        public void SetForm(Form form, RenderMethod method = RenderMethod.Event, int interval = 55)
        {
            Clear();

            CurrentForm = form;

            if (method == RenderMethod.Event)
                CurrentForm.Paint += FormPaintHandler;
            else
                Renderer = new WinFormsRenderer(this, CurrentForm, interval);
        }

        /// <summary>
        /// Sets the native window to be rendered to this touchpad
        /// Initializes the polling interval to 42ms (circa 24 FPS)
        /// </summary>
        /// <param name="windowHandle">the handle for the window to render</param>
        public void SetNativeWindow(IntPtr windowHandle)
        {
            Clear();

            CurrentNativeWindow = windowHandle;

            Renderer = new NativeRenderer(this, windowHandle, new TimeSpan(0, 0, 0, 0, 42));
        }

        /// <summary>
        /// Sets the WPF window to be rendered to this <see cref="RenderTarget" />.
        /// Initializes the polling interval to 42ms (circa 24 FPS)
        /// if called with RenderMethod set to Polling.
        /// </summary>
        /// <param name="window">The new window to render.</param>
        /// <param name="method">The method to use for rendering the window.</param>
        public void SetWindow(Window window, RenderMethod method = RenderMethod.Event)
        {
            SetWindow(window, method, new TimeSpan(0, 0, 0, 0, 42));
        }

        /// <summary>
        /// Sets the WPF window to be rendered to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="window">The new window to render.</param>
        /// <param name="method">The method to use for rendering the window</param>
        /// <param name="interval">The interval to poll the window at,
        /// only used if RenderMethod is Polling.</param>
        public void SetWindow(Window window, RenderMethod method, TimeSpan interval)
        {
            Clear();

            CurrentWindow = window;

            if (method == RenderMethod.Event)
                CurrentWindow.ContentRendered += WindowContentRenderedHandler;
            else
                Renderer = new WpfRenderer(this, CurrentWindow, interval);
        }

        /// <summary>
        /// Clears anything drawing to the render target.
        /// Also clears the current image if one is set.
        /// </summary>
        /// <remarks>
        /// Will not attempt to clear the image if the object is disposing,
        /// in order to avoid sending commands to the hardware.
        /// </remarks>
        /// <param name="disposing">True if this is called from <see cref="Dispose()" />.</param>
        protected virtual void Clear(bool disposing)
        {
            // We don't want to risk sending commands to the hardware
            // when we are disposing RenderTarget.
            if (!disposing)
                ClearImage();

            ClearNativeWindow();
            ClearForm();
            ClearWindow();

            if (Renderer == null)
                return;

            Renderer.Dispose();
            Renderer = null;
        }

        /// <summary>
        /// Clears the image currently on the render target.
        /// </summary>
        protected abstract void ClearImage();

        /// <summary>
        /// Disposes of this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from parameter-less <see cref="Dispose()" />, false otherwise.</param>
        protected void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
                Clear(true);

            Disposed = true;
        }

        /// <summary>
        /// Clears the current form from touchpad
        /// and stops rendering of it.
        /// </summary>
        private void ClearForm()
        {
            if (CurrentForm != null && Renderer == null)
                CurrentForm.Paint -= FormPaintHandler;

            CurrentForm = null;
        }

        /// <summary>
        /// Clears the current native window from
        /// the touchpad and stops rendering of it
        /// </summary>
        private void ClearNativeWindow()
        {
            CurrentNativeWindow = IntPtr.Zero;
        }

        /// <summary>
        /// Clears the current WPF window from
        /// touchpad and stops rendering of it.
        /// </summary>
        private void ClearWindow()
        {
            if (CurrentWindow != null && Renderer == null)
                CurrentWindow.ContentRendered -= WindowContentRenderedHandler;

            CurrentWindow = null;
        }

        /// <summary>
        /// Wrapper method to listen for the Paint event on a WinForms Form
        /// and render to touchpad.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void FormPaintHandler(object sender, PaintEventArgs e)
        {
            DrawForm(CurrentForm);
        }

        /// <summary>
        /// Wrapper method to listen for the ContentRendered event on a WPF
        /// Window and render to touchpad.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void WindowContentRenderedHandler(object sender, EventArgs e)
        {
            DrawWindow(CurrentWindow);
        }
    }
}
