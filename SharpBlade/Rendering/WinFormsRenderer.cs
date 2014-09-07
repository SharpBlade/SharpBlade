// ---------------------------------------------------------------------------------------
// <copyright file="WinFormsRenderer.cs" company="SharpBlade">
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using SharpBlade.Annotations;

namespace SharpBlade.Rendering
{
    /// <summary>
    /// Renders WinForms forms.
    /// </summary>
    internal sealed class WinFormsRenderer : Renderer
    {
        /// <summary>
        /// WinForms Form to render.
        /// Null if no WinForms Form assigned.
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// Timer used to control rendering of form when
        /// poll mode is in use.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// A boolean value to keep track of whether an event handler
        /// has been added to the form's Paint event.
        /// </summary>
        private bool _handlerAdded;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsRenderer" /> class.
        /// </summary>
        /// <param name="form">WinForms form to render.</param>
        /// <param name="interval">Interval (in milliseconds) to redraw at.</param>
        [PublicAPI]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is public.")]
        public WinFormsRenderer(Form form, int interval = 42)
            : this(form, RenderMethod.Polling, interval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsRenderer" /> class.
        /// </summary>
        /// <param name="form">WinForms form to render.</param>
        /// <param name="method">Render method to use.</param>
        /// <param name="interval">Interval (in milliseconds) to poll at when <paramref name="method" />
        /// is set to <see cref="RenderMethod.Polling" />.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "It makes no sense to dispose _timer.")]
        public WinFormsRenderer(Form form, RenderMethod method, int interval = 42)
        {
            _form = form;
            Method = method;

            // Event rendering doesn't need any setup
            if (Method == RenderMethod.Polling)
            {
                _timer = new Timer { Interval = interval };
                _timer.Tick += TimerTick;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="WinFormsRenderer" />
        /// is currently active (redrawing based on event or timer).
        /// </summary>
        public override bool Active
        {
            get
            {
                return _handlerAdded || _timer.Enabled;
            }
        }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) used for the redraw timer.
        /// </summary>
        public override int Interval
        {
            get
            {
                return _timer.Interval;
            }

            set
            {
                _timer.Interval = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="RenderMethod" /> currently in use.
        /// </summary>
        public RenderMethod Method { get; private set; }

        /// <summary>
        /// Draws the specified form to an <see cref="IRenderTarget" />.
        /// </summary>
        /// <param name="target">The <see cref="RenderTarget" /> to draw the form to.</param>
        /// <param name="form">Form to draw.</param>
        public static void Draw(IRenderTarget target, Form form)
        {
            using (var bmp = new Bitmap(target.DisplayWidth, target.DisplayHeight))
            {
                form.DrawToBitmap(bmp, form.Bounds);
                target.Draw(bmp);
            }
        }

        /// <summary>
        /// Force a redraw of the <see cref="Form" /> to the <see cref="Renderer{T}.Target" />.
        /// </summary>
        public override void Draw()
        {
            if (_form != null)
                Draw(Target, _form);
        }

        /// <summary>
        /// Starts continuous rendering of the <see cref="Form" /> to the <see cref="RenderTarget" />.
        /// </summary>
        public override void Start()
        {
            switch (Method)
            {
                case RenderMethod.Event:
                    if (_handlerAdded)
                        break;
                    _form.Paint += PaintHandler;
                    _handlerAdded = true;
                    break;

                case RenderMethod.Polling:
                    if (!_timer.Enabled)
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
                        return;
                    _form.Paint -= PaintHandler;
                    _handlerAdded = false;
                    break;

                case RenderMethod.Polling:
                    if (_timer.Enabled)
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

            if (_handlerAdded && _form != null)
                _form.Paint -= PaintHandler;

            if (_timer != null)
                _timer.Dispose();
        }

        /// <summary>
        /// Wrapper method to listen for the Paint event on a WinForms Form
        /// and render to the <see cref="RenderTarget" />.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PaintHandler(object sender, PaintEventArgs e)
        {
            Draw();
        }

        /// <summary>
        /// Callback for the tick event on the WinForms render timer.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
