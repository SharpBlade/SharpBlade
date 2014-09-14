// ---------------------------------------------------------------------------------------
// <copyright file="WpfRenderer.cs" company="SharpBlade">
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
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using SharpBlade.Annotations;
    using SharpBlade.Integration;

    /// <summary>
    /// Renders WPF windows.
    /// </summary>
    public sealed class WpfRenderer : Renderer<IRenderTarget>
    {
        /// <summary>
        /// Timer to control rendering of window when
        /// poll mode is in use.
        /// </summary>
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// A boolean to keep track of whether a handler has been registered
        /// for the WPF window's ContentRendered event.
        /// </summary>
        private bool _handlerAdded;

        /// <summary>
        /// WPF Window to render.
        /// Null if no WPF Window assigned.
        /// </summary>
        private Window _window;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfRenderer" /> class.
        /// Sets the target to <c>null</c>, this constructor is meant for
        /// when an instance is created to later be used with <see cref="IRenderTarget.Set{T}" />.
        /// </summary>
        /// <param name="target">The <see cref="IRenderTarget" /> to render the window to.</param>
        /// <param name="window">The window to render.</param>
        /// <param name="interval">The interval to render the window at.</param>
        /// <remarks>
        /// Trying to call <see cref="Draw()" />, <see cref="Start" />, or <see cref="Stop"/>
        /// while <see cref="Renderer{T}.Target" /> is null will cause a runtime error.
        /// </remarks>
        [PublicAPI]
        internal WpfRenderer(IRenderTarget target, Window window, int interval = 42)
            : this(target, window, RenderMethod.Polling, interval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfRenderer" /> class.
        /// </summary>
        /// <param name="target">The <see cref="IRenderTarget" /> to render the window to.</param>
        /// <param name="window">WPF window to render.</param>
        /// <param name="method">The render method to use for drawing the window.</param>
        /// <param name="interval">The interval to render the window at.
        /// Only used if <paramref name="method" /> is set to <see cref="RenderMethod.Polling" />.</param>
        internal WpfRenderer(IRenderTarget target, Window window, RenderMethod method, int interval = 42)
            : base(target)
        {
            _window = window;
            Method = method;

            if (Method == RenderMethod.Polling)
            {
                _timer = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(interval),
                    DispatcherPriority.Render,
                    TimerTick,
                    Dispatcher.CurrentDispatcher);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="WpfRenderer" />
        /// is currently active (redrawing based on event or timer).
        /// </summary>
        public override bool Active
        {
            get
            {
                return _handlerAdded || _timer.IsEnabled;
            }
        }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) used for the redraw timer.
        /// </summary>
        public override int Interval
        {
            get
            {
                return (int)_timer.Interval.TotalMilliseconds;
            }

            set
            {
                _timer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="RenderMethod" /> in use for this renderer.
        /// </summary>
        public RenderMethod Method { get; private set; }

        /// <summary>
        /// Gets or sets the WPF window to render.
        /// </summary>
        [PublicAPI]
        public Window Window
        {
            get { return _window; }
            set { _window = value; }
        }

        /// <summary>
        /// Draws a WPF window to a <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="target">The <see cref="RenderTarget" /> to render the window to.</param>
        /// <param name="window">Window object to draw.</param>
        /// <param name="winFormsComponents">Array of KeyValuePairs containing a WindowsFormsHost as the key and a WinForms control as the value.
        /// These pairs will be overlaid on the bitmap that is passed to the SwitchBlade device.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "WpfRenderer as a whole works on Windows, not Visuals.")]
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "SO said it's safe to dispose MemoryStream multiple times")]
        public static void Draw(
            IRenderTarget target,
            Window window,
            IEnumerable<EmbeddedWinFormsControl> winFormsComponents = null)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var rtb = new RenderTargetBitmap(
                target.DisplayWidth,
                target.DisplayHeight,
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
                        // This is safe to assume because above we are creating the bitmap
                        // with PixelFormat Pbgra32
                        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            foreach (var component in winFormsComponents)
                                graphics.DrawImage(component.Draw(), component.Bounds);
                        }
                    }

                    target.Draw(bitmap);
                }

                encoder.Frames.Clear();
            }

            rtb.Clear();
        }

        /// <summary>
        /// Force a redraw of the <see cref="Window" /> associated with this
        /// <see cref="WpfRenderer" /> to the <see cref="RenderTarget" />.
        /// </summary>
        public override void Draw()
        {
            if (_window == null)
                Stop();
            else
                Draw(Target, _window);
        }

        /// <summary>
        /// Starts continuous rendering to the render target.
        /// </summary>
        public override void Start()
        {
            switch (Method)
            {
                case RenderMethod.Event:
                    if (_handlerAdded)
                        break;
                    _window.ContentRendered += ContentRenderedHandler;
                    _handlerAdded = true;
                    break;

                case RenderMethod.Polling:
                    if (!_timer.IsEnabled)
                        _timer.Start();
                    break;
            }
        }

        /// <summary>
        /// Stops an ongoing continuous render operation.
        /// </summary>
        public override void Stop()
        {
            switch (Method)
            {
                case RenderMethod.Event:
                    if (!_handlerAdded)
                        break;
                    _window.ContentRendered -= ContentRenderedHandler;
                    _handlerAdded = false;
                    break;

                case RenderMethod.Polling:
                    if (_timer.IsEnabled)
                        _timer.Stop();
                    break;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Renderer{T}.Dispose()" />, false otherwise.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_timer != null)
                _timer.Stop();
        }

        /// <summary>
        /// Wrapper method to listen for the ContentRendered event on a WPF
        /// Window and render to touchpad.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ContentRenderedHandler(object sender, EventArgs e)
        {
            Draw();
        }

        /// <summary>
        /// Callback for the tick event on the WPF render timer.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
