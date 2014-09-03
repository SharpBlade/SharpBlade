// ---------------------------------------------------------------------------------------
// <copyright file="IRenderer.cs" company="SharpBlade">
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

namespace SharpBlade.Rendering
{
    /// <summary>
    /// Base interface for <see cref="Renderer{T}" />.
    /// </summary>
    public interface IRenderer : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this renderer is currently
        /// in an active state (redrawing based on a timer or event).
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Gets or sets the interval (in milliseconds) used for the redraw timer.
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// Force a redraw of the object associated with this
        /// <see cref="IRenderer" /> to the render target.
        /// </summary>
        void Draw();

        /// <summary>
        /// Starts continuous rendering to the render target.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops an ongoing continuous render operation.
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// Base interface for <see cref="Renderer{T}" />
    /// specifying interface stuff specific for render targets.
    /// </summary>
    /// <typeparam name="T">The type of target to render to.</typeparam>
    public interface IRenderer<out T> : IRenderer where T : class, IRenderTarget
    {
        /// <summary>
        /// Gets the <typeparamref name="T" /> which this <see cref="IRenderer{T}" />
        /// is rendering to.
        /// </summary>
        T Target { get; }
    }
}
