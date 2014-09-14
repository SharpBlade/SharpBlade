// ---------------------------------------------------------------------------------------
// <copyright file="DynamicKeyImageRenderer.cs" company="SharpBlade">
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
    using SharpBlade.Annotations;
    using SharpBlade.Native;
    using SharpBlade.Razer;

    /// <summary>
    /// Renders a static image to a dynamic key.
    /// </summary>
    public sealed class DynamicKeyImageRenderer : ImageRenderer
    {
        /// <summary>
        /// The image file to render for the UP state.
        /// </summary>
        private string _up;

        /// <summary>
        /// The image file to render for the DOWN state.
        /// </summary>
        private string _down;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicKeyImageRenderer" /> class.
        /// </summary>
        /// <param name="keyType">The type of DK to render to.</param>
        /// <param name="image">The image to render (both states).</param>
        /// <param name="interval">The interval (in milliseconds) at which to refresh the image.</param>
        [PublicAPI]
        internal DynamicKeyImageRenderer(DynamicKeyType keyType, string image, int interval = 42)
            : this(keyType, image, image, interval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicKeyImageRenderer" /> class.
        /// </summary>
        /// <param name="keyType">The type of DK to render to.</param>
        /// <param name="up">The image to render (UP state).</param>
        /// <param name="down">The image to render (DOWN state).</param>
        /// <param name="interval">The interval (in milliseconds) at which to refresh the image.</param>
        internal DynamicKeyImageRenderer(DynamicKeyType keyType, string up, string down, int interval = 42)
            : base(up, interval)
        {
            _up = up;
            _down = down;
            KeyType = keyType;
        }

        /// <summary>
        /// Gets or sets the image file to be set for the DOWN state of the dynamic key.
        /// </summary>
        public string Down
        {
            get
            {
                return _down;
            }

            set
            {
                _down = value;
                DrawDown();
            }
        }

        /// <summary>
        /// Gets or sets an image file to use for both states.
        /// </summary>
        /// <remarks>
        /// The get accessor for this property will return the image for the UP state.
        /// </remarks>
        public override string Image
        {
            get
            {
                return _up;
            }

            set
            {
                _up = value;
                _down = value;

                Draw();
            }
        }

        /// <summary>
        /// Gets or sets the image file to be set for the UP state of the dynamic key.
        /// </summary>
        public string Up
        {
            get
            {
                return _up;
            }

            set
            {
                _up = value;
                DrawUp();
            }
        }

        /// <summary>
        /// Gets or sets the dynamic key type to set the image on.
        /// </summary>
        public DynamicKeyType KeyType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the same image file is rendered
        /// for both dynamic key states.
        /// </summary>
        [PublicAPI]
        public bool SingleFile
        {
            get { return Up == Down; }
        }

        /// <summary>
        /// Sets the image for the specified dynamic key's up and down states.
        /// </summary>
        /// <param name="keyType">The dynamic key to modify.</param>
        /// <param name="image">Image file for both states.</param>
        [PublicAPI]
        public static void Draw(DynamicKeyType keyType, string image)
        {
            Draw(keyType, image, image);
        }

        /// <summary>
        /// Sets the images for the specified dynamic key's up and down states.
        /// </summary>
        /// <param name="keyType">The dynamic key to modify.</param>
        /// <param name="upImage">Image file for the UP state.</param>
        /// <param name="downImage">Image file for the DOWN state.</param>
        public static void Draw(DynamicKeyType keyType, string upImage, string downImage)
        {
            Draw(keyType, DynamicKeyState.Up, upImage);
            Draw(keyType, DynamicKeyState.Down, downImage);
        }

        /// <summary>
        /// Sets the images for the specified state on the specified dynamic key.
        /// </summary>
        /// <param name="keyType">The dynamic key to modify.</param>
        /// <param name="state">The state to update.</param>
        /// <param name="image">Image file for the specified state.</param>
        /// <exception cref="NativeCallException">
        /// Thrown if there is an error calling the native functions to update the images.
        /// </exception>
        public static void Draw(DynamicKeyType keyType, DynamicKeyState state, string image)
        {
            var result = NativeMethods.RzSBSetImageDynamicKey(keyType, state, image);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBSetImageDynamicKey", result);
        }

        /// <summary>
        /// Force a redraw of the files to the respective
        /// dynamic key states on the dynamic key.
        /// </summary>
        public override void Draw()
        {
            Draw(KeyType, Up, Down);
        }

        /// <summary>
        /// Force a redraw of the image for the UP state.
        /// </summary>
        public void DrawUp()
        {
            Draw(KeyType, DynamicKeyState.Up, Image);
        }

        /// <summary>
        /// Force a redraw of the image for the DOWN state.
        /// </summary>
        public void DrawDown()
        {
            Draw(KeyType, DynamicKeyState.Down, Down);
        }
    }
}
