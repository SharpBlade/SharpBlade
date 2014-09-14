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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Helper class to manage rendering complex structures to a render target.
    /// </summary>
    /// <typeparam name="T">The type of target to render to.</typeparam>
    /// <remarks>
    /// The <c>typeparam</c> can be used to restrict a renderer to certain types
    /// of render targets.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
        Justification = "IDisposable is needed on the interface so that RenderTarget can dispose it properly")]
    public abstract class Renderer<T> : Renderer, IRenderer<T> where T : class, IRenderTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer{T}" /> class.
        /// This constructor allows internal components to supply the target
        /// <see cref="IRenderTarget" /> object at the time of instantiation.
        /// </summary>
        /// <param name="target">The render target to render to.</param>
        internal Renderer(T target)
        {
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer{T}" /> class.
        /// This constructor does not initialize the <see cref="Target" /> property.
        /// </summary>
        protected Renderer()
        {
        }

        /// <summary>
        /// Gets a local instance of the SwitchBlade target.
        /// </summary>
        public T Target { get; internal set; }
    }
}
