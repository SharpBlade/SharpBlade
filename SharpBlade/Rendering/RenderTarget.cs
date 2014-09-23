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
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;

    using SharpBlade.Events;
    using SharpBlade.Integration;
    using SharpBlade.Razer;

    /// <summary>
    /// Common methods for rendering to a display.
    /// </summary>
    public abstract class RenderTarget : IRenderTarget
    {
        /// <summary>
        /// Private backing field for the current <see cref="IRenderer" />.
        /// </summary>
        private IRenderer _renderer;

        /// <summary>
        /// Field to keep track of whether the <see cref="_renderer" /> has
        /// been disposed after a Deactivated app event.
        /// </summary>
        private bool _rendererSuspended;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget" /> class.
        /// </summary>
        /// <param name="targetDisplay">
        /// The <see cref="Razer.TargetDisplay" /> to which content will be rendered.
        /// </param>
        /// <param name="height">The height (in pixels) of the target display.</param>
        /// <param name="width">The width (in pixels) of the target display.</param>
        internal RenderTarget(TargetDisplay targetDisplay, int height, int width)
        {
            DisplayHeight = height;
            DisplayWidth = width;
            TargetDisplay = targetDisplay;

            var sb = Switchblade.Instance;
            sb.Activated += OnAppActivated;
            sb.Deactivated += OnAppDeactivated;
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
        /// Gets the height of this <see cref="IRenderTarget" /> in pixels.
        /// </summary>
        public int DisplayHeight { get; private set; }

        /// <summary>
        /// Gets the width of this <see cref="IRenderTarget" /> in pixels.
        /// </summary>
        public int DisplayWidth { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object has been disposed.
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="IRenderer" /> instance used to
        /// manage timed rendering of objects.
        /// </summary>
        /// <remarks>
        /// <para>To set custom renderers implementing <see cref="IRenderer{T}" />,
        /// the <see cref="Set{T}" /> method has to be used in order for the
        /// <see cref="RenderTarget" /> to properly set the
        /// <see cref="IRenderer{T}.Target" /> property.</para>
        /// <para>Failure to do so may cause the renderer to not operate.</para>
        /// </remarks>
        public IRenderer Renderer
        {
            get
            {
                return _renderer;
            }

            set
            {
                Clear();

                _renderer = value;
                
                if (_renderer != null)
                    _renderer.Start();
            }
        }

        /// <summary>
        /// Gets the <see cref="Razer.TargetDisplay" /> that content will be
        /// rendered to.
        /// </summary>
        public TargetDisplay TargetDisplay { get; private set; }

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
        /// Draws a static image to the target display.
        /// </summary>
        /// <param name="image">Path to the image file.</param>
        public abstract void Draw(string image);

        /// <summary>
        /// Draws a <see cref="Bitmap" /> to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="bitmap">The bitmap object to draw.</param>
        /// <remarks>
        /// The size of the bitmap object should match the size of the target
        /// display. For the <see cref="Touchpad" />, the size is specified by
        /// <see cref="Razer.Constants.TouchpadWidth" /> and <see cref="Razer.Constants.TouchpadHeight"/>,
        /// for dynamic keys, it's specified by <see cref="Razer.Constants.DynamicKeyWidth" /> and
        /// <see cref="Razer.Constants.DynamicKeyHeight" />.
        /// </remarks>
        public void Draw(Bitmap bitmap)
        {
            BitmapRenderer.Draw(this, bitmap);
        }

        /// <summary>
        /// Draws the specified form to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="form">Form to draw.</param>
        public void Draw(Form form)
        {
            WinFormsRenderer.Draw(this, form);
        }

        /// <summary>
        /// Draws the specified native window to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="windowHandle">The window handle of the window to draw.</param>
        public void Draw(IntPtr windowHandle)
        {
            NativeRenderer.Draw(this, windowHandle);
        }

        /// <summary>
        /// Draws a WPF window to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="window">Window object to draw.</param>
        /// <param name="winFormsComponents">Array of KeyValuePairs containing a WindowsFormsHost as the key and a WinForms control as the value.
        /// These pairs will be overlaid on the bitmap that is passed to the SwitchBlade device.</param>
        public void Draw(Window window, IEnumerable<EmbeddedWinFormsControl> winFormsComponents = null)
        {
            WpfRenderer.Draw(this, window, winFormsComponents);
        }

        /// <summary>
        /// Sets an <see cref="IBitmapProvider" /> to provide this <see cref="RenderTarget" />
        /// with a <see cref="Bitmap" /> object to draw.
        /// </summary>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        /// <param name="interval">The interval at which to query the provider
        /// for a new <see cref="Bitmap" /> and draw it.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Doesn't make sense to dispose the object we are creating here.")]
        public void Set(IBitmapProvider provider, int interval = 42)
        {
            Renderer = new BitmapRenderer(this, provider, interval);
        }

        /// <summary>
        /// Sets a form to be rendered to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="form">The form to render.</param>
        /// <param name="method">The method to use for rendering the form.</param>
        /// <param name="interval">Interval (in milliseconds) at which to render the form
        /// (only used when <paramref name="method" /> is set to <see cref="RenderMethod.Polling" />.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Doesn't make sense to dispose the object we are creating here.")]
        public void Set(Form form, RenderMethod method = RenderMethod.Polling, int interval = 42)
        {
            Renderer = new WinFormsRenderer(this, form, method, interval);
        }

        /// <summary>
        /// Sets a native window to be rendered to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="handle">A handle to the window that is to be rendered.</param>
        /// <param name="interval">Interval (in milliseconds) at which to render the window.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Doesn't make sense to dispose the object we are creating here.")]
        public void Set(IntPtr handle, int interval = 42)
        {
            Renderer = new NativeRenderer(this, handle, interval);
        }

        /// <summary>
        /// Sets a WPF window to be rendered to this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="window">The window to render.</param>
        /// <param name="method">The method to use for rendering the window.</param>
        /// <param name="interval">Interval (in milliseconds) at which to render the window
        /// (only used when <paramref name="method" /> is set to <see cref="RenderMethod.Polling" />.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Doesn't make sense to dispose the object we are creating here.")]
        public void Set(Window window, RenderMethod method = RenderMethod.Polling, int interval = 42)
        {
            Renderer = new WpfRenderer(this, window, method, interval);
        }

        /// <summary>
        /// Set a static image to be displayed on the target display.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <param name="interval">The interval (in milliseconds) at which to redraw the image file.</param>
        public abstract void Set(string image, int interval = 42);

        /// <summary>
        /// Sets the <see cref="IRenderer{T}" /> to be used for this <see cref="RenderTarget" /> and
        /// calls its <see cref="M:IRenderer{T}.Start" /> method.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="RenderTarget" /> that the renderer is compatible with.
        /// </typeparam>
        /// <param name="renderer">An instance of the <see cref="IRenderer{T}" /> class.</param>
        public virtual void Set<T>(IRenderer<T> renderer) where T : class, IRenderTarget
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            var targetRenderer = renderer as Renderer<T>;

            if (targetRenderer != null)
            {
                var target = this as T;

                if (target == null)
                {
                    throw new RenderTargetNotCompatibleException(
                        "Renderer type error, " + GetType() + " is not derived from " + typeof(T));
                }

                targetRenderer.Target = target;
            }
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
        protected void Clear(bool disposing)
        {
            // We don't want to risk sending commands to the hardware
            // when we are disposing target.
            if (!disposing)
                ClearImage();

            if (_renderer == null)
                return;

            _renderer.Stop();
            _renderer.Dispose();
            _renderer = null;
        }

        /// <summary>
        /// Clears the image currently on the render target.
        /// </summary>
        protected abstract void ClearImage();

        /// <summary>
        /// Disposes of this <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from parameter-less <see cref="Dispose()" />, false otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
                Clear(true);

            Disposed = true;
        }

        /// <summary>
        /// Runs when app is activated from a suspended state, starts the
        /// renderer back up if it was suspended by us when app was
        /// deactivated.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAppActivated(object sender, AppEventEventArgs e)
        {
            var renderer = Renderer;
            if (_rendererSuspended && renderer != null)
                renderer.Start();

            // We set the suspended tracking field to false regardless of above
            // outcome, to avoid possible errors on next deactivation.
            _rendererSuspended = false;
        }

        /// <summary>
        /// Runs when app is deactivated and put in the background by
        /// the SBUI system, stops the renderer (if active) and sets
        /// the suspended tracking field to start it back up after
        /// app activates again.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAppDeactivated(object sender, AppEventEventArgs e)
        {
            var renderer = Renderer;
            if (renderer == null || !renderer.Active)
                return;
            renderer.Stop();
            _rendererSuspended = true;
        }
    }
}
