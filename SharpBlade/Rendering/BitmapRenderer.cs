// ---------------------------------------------------------------------------------------
// <copyright file="BitmapRenderer.cs" company="SharpBlade">
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
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Timers;

    using SharpBlade.Native;
    using SharpBlade.Razer;

    /// <summary>
    /// A renderer that queries an object implementing <see cref="IBitmapProvider" />
    /// to get a <see cref="System.Drawing.Bitmap" /> that is then sent to the
    /// <see cref="Touchpad" /> for drawing.
    /// </summary>
    public sealed class BitmapRenderer : Renderer<IRenderTarget>
    {
        /// <summary>
        /// The Bitmap provider.
        /// </summary>
        private readonly IBitmapProvider _provider;

        /// <summary>
        /// Timer to control how often the target should refresh.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapRenderer" /> class.
        /// </summary>
        /// <param name="target">The <see cref="IRenderTarget" /> to render the bitmap to.</param>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        /// <param name="interval">How often to refresh the target (in milliseconds).</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The values used for the properties won't cause exceptions.")]
        internal BitmapRenderer(IRenderTarget target, IBitmapProvider provider, int interval = 42)
            : base(target)
        {
            _provider = provider;
            _timer = new Timer(interval) { AutoReset = true, Enabled = false };
            _timer.Elapsed += TimerTick;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BitmapRenderer" />
        /// is currently active (timer running to redraw regularly).
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
        /// Draws a <see cref="Bitmap" /> to a <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="target">The <see cref="RenderTarget" /> to draw to.</param>
        /// <param name="bitmap">The bitmap object to draw.</param>
        /// <remarks>
        /// The size of the bitmap object should match the size of the target
        /// display. For the <see cref="Touchpad" />, the size is specified by
        /// <see cref="Razer.Constants.TouchpadWidth" /> and <see cref="Razer.Constants.TouchpadHeight"/>,
        /// for dynamic keys, it's specified by <see cref="Razer.Constants.DynamicKeyWidth" /> and
        /// <see cref="Razer.Constants.DynamicKeyHeight" />.
        /// </remarks>
        public static void Draw(IRenderTarget target, Bitmap bitmap)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format16bppRgb565);

            var buffer = new BufferParams
            {
                PixelType = PixelType.Rgb565,
                DataSize = (uint)(bitmap.Width * bitmap.Height * sizeof(ushort)),
                PtrData = data.Scan0
            };

            var ptrToImageStruct = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
            Marshal.StructureToPtr(buffer, ptrToImageStruct, true);

            var result = NativeMethods.RzSBRenderBuffer(target.TargetDisplay, ptrToImageStruct);

            // Free resources before handling return
            Marshal.FreeHGlobal(ptrToImageStruct);
            bitmap.UnlockBits(data);

            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBRenderBuffer", result);
        }

        /// <summary>
        /// Force a redraw of the <see cref="Bitmap" /> associated with this
        /// <see cref="BitmapRenderer" /> to the <see cref="Renderer{T}.Target" />.
        /// </summary>
        public override void Draw()
        {
            Draw(Target, _provider.Bitmap);
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
            Justification = "The class has sealed keyword already.")]
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Stop();
            _timer.Dispose();
        }

        /// <summary>
        /// Event handler for the Timer's Tick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            Draw();
        }
    }
}
