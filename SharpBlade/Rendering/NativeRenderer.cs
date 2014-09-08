// ---------------------------------------------------------------------------------------
// <copyright file="NativeRenderer.cs" company="SharpBlade">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Timers;

    using SharpBlade.Helpers;

    /// <summary>
    /// A renderer to render a native window of any type that
    /// is able to supply a window handle.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class NativeRenderer : Renderer
    {
        /// <summary>
        /// Timer to control rendering of window when
        /// poll mode is in use
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// Native window handle.
        /// <c>IntPtr.Zero</c> if no native window assigned.
        /// </summary>
        private readonly IntPtr _handle = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeRenderer" /> class.
        /// Used for rendering a native window at the specified interval.
        /// </summary>
        /// <param name="handle">Native window handle to render.</param>
        /// <param name="interval">The interval (in milliseconds) to render the window at.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The values for the properties won't cause exceptions.")]
        public NativeRenderer(IntPtr handle, int interval = 42)
        {
            _handle = handle;

            _timer = new Timer(interval) { AutoReset = true, Enabled = false };
            _timer.Elapsed += TimerTick;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="NativeRenderer" />
        /// is currently active (redrawing based on a timer).
        /// </summary>
        public override bool Active
        {
            get
            {
                return _timer.Enabled;
            }
        }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) used for the redraw timer.
        /// </summary>
        public override int Interval
        {
            get
            {
                return (int)_timer.Interval;
            }

            set
            {
                _timer.Interval = value;
            }
        }

        /// <summary>
        /// Draws the specified native window to an <see cref="IRenderTarget" />.
        /// </summary>
        /// <param name="target">The <see cref="IRenderTarget" /> to draw to.</param>
        /// <param name="windowHandle">The window handle of the window to draw.</param>
        public static void Draw(IRenderTarget target, IntPtr windowHandle)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            using (var img = ScreenCapture.CaptureWindow(windowHandle))
            {
                using (var bitmapToRender = new Bitmap(img, target.DisplayWidth, target.DisplayHeight))
                    target.Draw(bitmapToRender);
            }
        }

        /// <summary>
        /// Force a redraw of the window handle associated with this
        /// <see cref="NativeRenderer" /> to the <see cref="Renderer{T}.Target" />.
        /// </summary>
        public override void Draw()
        {
            Draw(Target, _handle);
        }

        /// <summary>
        /// Starts continuous rendering to the render target.
        /// </summary>
        public override void Start()
        {
            if (!_timer.Enabled)
                _timer.Start();
        }

        /// <summary>
        /// Stops an ongoing continuous render operation.
        /// </summary>
        public override void Stop()
        {
            if (_timer.Enabled)
                _timer.Stop();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Renderer{T}.Dispose()" />, false otherwise.</param>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
            Justification = "The sealed keyword is present on the class already.")]
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Stop();
            _timer.Dispose();
        }

        /// <summary>
        /// Callback for the native tick event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
