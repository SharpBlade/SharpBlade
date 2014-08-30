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
//     Disclaimer: SharpBlade is in no way affiliated
//     with Razer and/or any of its employees and/or licensors.
//     Adam Hellberg does not take responsibility for any harm caused, direct
//     or indirect, to any Razer peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

using System;
using System.Timers;

using SharpBlade.Razer;

namespace SharpBlade.Integration
{
    /// <summary>
    /// A renderer that queries an object implementing <see cref="IBitmapProvider" />
    /// to get a <see cref="System.Drawing.Bitmap" /> that is then sent to the
    /// <see cref="Touchpad" /> for drawing.
    /// </summary>
    internal class BitmapRenderer : Renderer<RenderTarget>
    {
        /// <summary>
        /// The Bitmap provider.
        /// </summary>
        private readonly IBitmapProvider _provider;

        /// <summary>
        /// Timer to control how often the RenderTarget should refresh.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapRenderer" /> class.
        /// </summary>
        /// <param name="renderTarget">The <see cref="Touchpad" /> instance this renderer should communicate with.</param>
        /// <param name="provider">An object implementing the <see cref="IBitmapProvider" /> interface.</param>
        /// <param name="interval">How often to refresh the RenderTarget.</param>
        internal BitmapRenderer(RenderTarget renderTarget, IBitmapProvider provider, TimeSpan interval)
            : base(renderTarget)
        {
            _provider = provider;
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += TimerTick;
            _timer.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (_timer == null)
                return;
            _timer.Stop();
            _timer.Dispose();
        }

        /// <summary>
        /// Event handler for the Timer's Tick event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            RenderTarget.DrawBitmap(_provider.Bitmap);
        }
    }
}
