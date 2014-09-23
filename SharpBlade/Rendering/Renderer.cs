// ---------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="SharpBlade">
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

    using SharpBlade.Events;
    using SharpBlade.Logging;

    /// <summary>
    /// Class that can render objects in arbitrary ways.
    /// This class is not directly associated with a render target,
    /// in contrast to <see cref="Renderer{T}" />.
    /// </summary>
    public abstract class Renderer : IRenderer
    {
        /// <summary>
        /// <see cref="log4net.ILog" /> instance for this class.
        /// </summary>
        private log4net.ILog _log;

        /// <summary>
        /// Field to keep track of whether this renderer has
        /// been suspended after a Deactivated app event.
        /// </summary>
        private bool _suspended;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer" /> class.
        /// </summary>
        protected Renderer()
        {
            _log = LogManager.GetLogger(this);

            var sb = Switchblade.Instance;
            sb.Activated += OnAppActivated;
            sb.Deactivated += OnAppDeactivated;
        }

        /// <summary>
        /// Gets a value indicating whether this renderer is currently
        /// in an active state (redrawing based on a timer or event).
        /// </summary>
        public abstract bool Active { get; }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) used for the redraw timer.
        /// </summary>
        public abstract int Interval { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Force a redraw of the object associated with this
        /// <see cref="Renderer{T}" />.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Starts continuous rendering to the render target.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops an ongoing continuous render operation.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose()" />, false otherwise.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Runs when app is activated from a suspended state, starts
        /// rendering back up if it was suspended by us when app was
        /// deactivated.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAppActivated(object sender, AppEventEventArgs e)
        {
            if (_suspended)
            {
                _log.Debug("Starting back up from being suspended.");
                Start();
            }

            // We set the suspended tracking field to false regardless of above
            // outcome, to avoid possible errors on next deactivation.
            _suspended = false;
        }

        /// <summary>
        /// Runs when app is deactivated and put in the background by
        /// the SBUI system, stops renderering (if active) and sets
        /// the suspended tracking field to start back up after
        /// app activates again.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAppDeactivated(object sender, AppEventEventArgs e)
        {
            if (!Active)
                return;

            _log.Debug("App is deactivating, suspending rendering...");
            Stop();
            _suspended = true;
        }
    }
}
