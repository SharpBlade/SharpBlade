// ---------------------------------------------------------------------------------------
// <copyright file="Renderer{T}.cs" company="SharpBlade">
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

    /// <summary>
    /// Helper class to manage rendering complex structures to a render target.
    /// </summary>
    /// <typeparam name="T">The type of target to render to.</typeparam>
    /// <remarks>
    /// The <c>typeparam</c> can be used to restrict a renderer to certain types
    /// of render targets.
    /// </remarks>
    public abstract class Renderer<T> : IRenderer<T> where T : class, IRenderTarget
    {
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
        /// Gets a local instance of the SwitchBlade target.
        /// </summary>
        public T Target { get; internal set; }

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
        /// <see cref="Renderer{T}" /> to the <see cref="Target" />.
        /// </summary>
        public virtual void Draw()
        {
            if (Target == null)
                throw new InvalidOperationException("Can't call Draw before Target is assigned");
        }

        /// <summary>
        /// Starts continuous rendering to the render target.
        /// </summary>
        public virtual void Start()
        {
            if (Target == null)
                throw new InvalidOperationException("Can't call Start before Target is assigned");
        }

        /// <summary>
        /// Stops an ongoing continuous render operation.
        /// </summary>
        public virtual void Stop()
        {
            if (Target == null)
                throw new InvalidOperationException("Can't call Stop before Target is assigned");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose()" />, false otherwise.</param>
        protected abstract void Dispose(bool disposing);
    }
}
