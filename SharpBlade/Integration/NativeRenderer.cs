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
//     Disclaimer: SharpBlade is in no way affiliated
//     with Razer and/or any of its employees and/or licensors.
//     Adam Hellberg does not take responsibility for any harm caused, direct
//     or indirect, to any Razer peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

using System;

using SharpBlade.Razer;

namespace SharpBlade.Integration
{
    /// <summary>
    /// A renderer to render a native window of any type that
    /// is able to supply a window handle.
    /// </summary>
    internal class NativeRenderer : Renderer
    {
        /// <summary>
        /// Timer to control rendering of window when
        /// poll mode is in use
        /// </summary>
        private readonly System.Timers.Timer _nativeTimer;

        /// <summary>
        /// Native window handle.
        /// <c>IntPtr.Zero</c> if no native window assigned.
        /// </summary>
        private readonly IntPtr _nativeWindow = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeRenderer" /> class.
        /// Used for rendering a native window at the specified interval.
        /// </summary>
        /// <param name="touchpad">Touchpad reference.</param>
        /// <param name="windowHandle">Native window handle to render.</param>
        /// <param name="interval">The interval to render the window at.</param>
        internal NativeRenderer(Touchpad touchpad, IntPtr windowHandle, TimeSpan interval)
            : base(touchpad)
        {
            _nativeWindow = windowHandle;

            _nativeTimer = new System.Timers.Timer(interval.TotalMilliseconds);
            _nativeTimer.Elapsed += NativeTimerTick;
            _nativeTimer.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (_nativeTimer == null)
                return;
            _nativeTimer.Stop();
            _nativeTimer.Dispose();
        }

        /// <summary>
        /// Callback for the native tick event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void NativeTimerTick(object sender, EventArgs e)
        {
            Touchpad.DrawNativeWindow(_nativeWindow);
        }
    }
}
