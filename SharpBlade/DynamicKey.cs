// ---------------------------------------------------------------------------------------
// <copyright file="DynamicKey.cs" company="SharpBlade">
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
using System.Collections.Generic;

using SharpBlade.Logging;
using SharpBlade.Razer;
using SharpBlade.Rendering;

namespace SharpBlade
{
    /// <summary>
    /// Represents a dynamic key on the SwitchBlade device
    /// </summary>
    public sealed class DynamicKey : RenderTarget
    {
        /// <summary>
        /// A mapping dictionary to translate a <see cref="DynamicKeyType" />
        /// value to <see cref="TargetDisplay" />.
        /// </summary>
        private static readonly Dictionary<DynamicKeyType, TargetDisplay> TargetDisplayMapping =
            new Dictionary<DynamicKeyType, TargetDisplay>
            {
                { DynamicKeyType.DK1, TargetDisplay.DK1 },
                { DynamicKeyType.DK2, TargetDisplay.DK2 },
                { DynamicKeyType.DK3, TargetDisplay.DK3 },
                { DynamicKeyType.DK4, TargetDisplay.DK4 },
                { DynamicKeyType.DK5, TargetDisplay.DK5 },
                { DynamicKeyType.DK6, TargetDisplay.DK6 },
                { DynamicKeyType.DK7, TargetDisplay.DK7 },
                { DynamicKeyType.DK8, TargetDisplay.DK8 },
                { DynamicKeyType.DK9, TargetDisplay.DK9 },
                { DynamicKeyType.DK10, TargetDisplay.DK10 }
            };

        /// <summary>
        /// The log instance associated with this object.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicKey" /> class.
        /// </summary>
        /// <param name="keyType">The type of dynamic key being initialized.</param>
        /// <param name="image">Image to set for this key's depressed state.</param>
        /// <param name="pressedImage">Image to set for this key's pressed state.</param>
        /// <param name="callback">The function to call when this key is pressed.</param>
        internal DynamicKey(
            DynamicKeyType keyType,
            string image,
            string pressedImage = null,
            EventHandler callback = null)
            : base(TargetDisplayMapping[keyType], Razer.Constants.DynamicKeyHeight, Razer.Constants.DynamicKeyWidth)
        {
            _log = LogManager.GetLogger(this);

            if (string.IsNullOrEmpty(pressedImage))
            {
                _log.Debug("pressedImage is null, setting to value of image");
                pressedImage = image;
            }

            _log.Debug("Setting default states");
            State = DynamicKeyState.None;
            PreviousState = DynamicKeyState.None;
            KeyType = keyType;

            Images = new DynamicKeyImageRenderer(keyType, image, pressedImage);

            // Set{Up,Down}Image will also set the relevant properties
            _log.Debug("Setting images");
            Images.Draw();

            if (callback != null)
            {
                _log.Debug("Setting callback");
                Pressed += callback;
            }
        }

        /// <summary>
        /// Raised when a dynamic key is pressed.
        /// </summary>
        public event EventHandler Pressed;

        /// <summary>
        /// Gets the instance of <see cref="DynamicKeyImageRenderer" /> that
        /// manages the static images for this dynamic key.
        /// </summary>
        /// <remarks>
        /// Be wary when using this property and the <see cref="Renderer" />
        /// property, careless switching between the two without calling their
        /// respective <c>Stop</c> methods can cause the dynamic key to
        /// switch back and forth between different images or bitmaps,
        /// due to two different renderers fighting against each other.
        /// </remarks>
        public DynamicKeyImageRenderer Images { get; private set; }

        /// <summary>
        /// Gets the <see cref="DynamicKeyType" /> of this key.
        /// </summary>
        public DynamicKeyType KeyType { get; private set; }

        /// <summary>
        /// Gets the previous state of this key.
        /// </summary>
        public DynamicKeyState PreviousState { get; private set; }

        /// <summary>
        /// Gets the current state of this key.
        /// </summary>
        public DynamicKeyState State { get; private set; }

        /// <summary>
        /// Disables this dynamic key (sets to blank image).
        /// </summary>
        public void Disable()
        {
            Images.Image = Switchblade.Instance.DisabledDynamicKeyImagePath;
        }

        /// <summary>
        /// Sets a static image to both states of the dynamic key.
        /// </summary>
        /// <param name="image">Path to the image file.</param>
        public override void Draw(string image)
        {
            Draw(image, image);
        }

        /// <summary>
        /// Sets the images for each state of the dynamic key.
        /// </summary>
        /// <param name="image">Image for the UP state.</param>
        /// <param name="downImage">Image for the DOWN state.</param>
        public void Draw(string image, string downImage)
        {
            Images.Up = image;
            Images.Down = downImage;
        }

        /// <summary>
        /// Sets the image that is displayed and refreshed on this key.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <param name="interval">The interval (in milliseconds) at which to refresh the image.</param>
        /// <remarks>This will set the image on both the UP and DOWN states.</remarks>
        public override void Set(string image, int interval = 42)
        {
            Set(image, image, interval);
        }

        /// <summary>
        /// Sets the images to be drawn and refreshed on this key.
        /// </summary>
        /// <param name="image">Path to UP image.</param>
        /// <param name="downImage">Path to DOWN image.</param>
        /// <param name="interval">The interval (in milliseconds) at which to refresh the images.</param>
        public void Set(string image, string downImage, int interval = 42)
        {
            Images.Stop();
            Images.Up = image;
            Images.Down = downImage;
            Images.Interval = interval;
        }

        /// <summary>
        /// Sets the <see cref="Renderer{T}" /> to be used for this <see cref="DynamicKey" /> and
        /// calls its <see cref="Renderer{T}.Start" /> method.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="RenderTarget" /> that the renderer is compatible with.
        /// </typeparam>
        /// <param name="renderer">An instance of the <see cref="Renderer{T}" /> class.</param>
        /// <remarks>
        /// This overload also calls the <see cref="Renderer{T}.Stop" /> method
        /// on <see cref="DynamicKeyImageRenderer" /> prior to setting the new renderer and starting it,
        /// to avoid possible collisions.
        /// </remarks>
        public override void Set<T>(Renderer<T> renderer)
        {
            Images.Stop();

            base.Set(renderer);
        }

        /// <summary>
        /// Updates this key's state and handles events.
        /// </summary>
        /// <param name="state">The new state to assign to this key.</param>
        internal void UpdateState(DynamicKeyState state)
        {
            PreviousState = State;
            State = state;
            if (State == DynamicKeyState.Up
                && (PreviousState == DynamicKeyState.Down || PreviousState == DynamicKeyState.None))
                OnPressed();
        }

        /// <summary>
        /// Clears the image currently on the dynamic key.
        /// </summary>
        protected override void ClearImage()
        {
            Images.Stop();
            Images.Image = Switchblade.Instance.DisabledDynamicKeyImagePath;
        }

        /// <summary>
        /// Raises Pressed event to subscribers.
        /// </summary>
        private void OnPressed()
        {
            var func = Pressed;
            if (func == null)
                return;

            try
            {
                func(this, null);
            }
            catch (ObjectDisposedException ex)
            {
                _log.ErrorFormat("OnPressed: ObjectDisposedException: {0}", ex.Message);
            }
        }
    }
}
