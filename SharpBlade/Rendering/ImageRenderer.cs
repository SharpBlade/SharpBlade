// ---------------------------------------------------------------------------------------
// <copyright file="ImageRenderer.cs" company="SharpBlade">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Timers;

    using SharpBlade.Annotations;

    /// <summary>
    /// Renders a static image.
    /// </summary>
    public abstract class ImageRenderer : Renderer
    {
        /// <summary>
        /// Timer object used for redrawing the image
        /// at a specified interval.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// The image file to render.
        /// </summary>
        private string _image;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderer" /> class.
        /// </summary>
        /// <param name="image">The image file to render.</param>
        /// <param name="interval">The interval (in milliseconds) at which to redraw the image.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The values used for the properties won't cause exceptions.")]
        protected ImageRenderer(string image, int interval = 42)
        {
            _image = image;

            _timer = new Timer(interval) { AutoReset = true, Enabled = false };
            _timer.Elapsed += TimerTick;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ImageRenderer" />
        /// is currently active (redrawing image based on timer).
        /// </summary>
        public override bool Active
        {
            get { return _timer.Enabled; }
        }

        /// <summary>
        /// Gets or sets the image which this image renderer is drawing.
        /// </summary>
        [PublicAPI]
        public virtual string Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                Draw();
            }
        }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) used for the redraw timer.
        /// </summary>
        public override int Interval
        {
            get { return (int)_timer.Interval; }
            set { _timer.Interval = value; }
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
        /// <param name="disposing">True if this is called from <see cref="Dispose" />, false otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _timer.Dispose();
        }

        /// <summary>
        /// Callback method for the timer used to redraw the image file.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            Draw();
        }
    }
}
