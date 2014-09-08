// ---------------------------------------------------------------------------------------
// <copyright file="TouchpadImageRenderer.cs" company="SharpBlade">
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
    using SharpBlade.Native;
    using SharpBlade.Razer;

    /// <summary>
    /// Renders a static file to the touchpad.
    /// </summary>
    public sealed class TouchpadImageRenderer : ImageRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchpadImageRenderer" /> class.
        /// </summary>
        /// <param name="up">Path to the image to render.</param>
        /// <param name="interval">The interval at which to refresh the image.</param>
        public TouchpadImageRenderer(string up, int interval = 42)
            : base(up, interval)
        {
        }

        /// <summary>
        /// Draws a static image file to the touchpad.
        /// </summary>
        /// <param name="image">Path to the image file.</param>
        /// <exception cref="NativeCallException">
        /// Thrown if the call to set the image fails.
        /// </exception>
        public static void Draw(string image)
        {
            var result = NativeMethods.RzSBSetImageTouchpad(image);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBSetImageTouchpad", result);
        }

        /// <summary>
        /// Force a redraw of the image to the touchpad.
        /// </summary>
        public override void Draw()
        {
            Draw(Image);
        }
    }
}
