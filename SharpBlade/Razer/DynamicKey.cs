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
//     Disclaimer: SharpBlade is in no way affiliated
//     with Razer and/or any of its employees and/or licensors.
//     Adam Hellberg does not take responsibility for any harm caused, direct
//     or indirect, to any Razer peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using SharpBlade.Helpers;
using SharpBlade.Logging;
using SharpBlade.Native;
using SharpBlade.Razer.Exceptions;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Represents a dynamic key on the SwitchBlade device
    /// </summary>
    public sealed class DynamicKey : RenderTarget
    {
        /// <summary>
        /// A mapping dictionary to translate a <see cref="RazerAPI.DynamicKeyType" />
        /// value to <see cref="RazerAPI.TargetDisplay" />.
        /// </summary>
        private static readonly Dictionary<RazerAPI.DynamicKeyType, RazerAPI.TargetDisplay> TargetDisplayMapping =
            new Dictionary<RazerAPI.DynamicKeyType, RazerAPI.TargetDisplay>
            {
                { RazerAPI.DynamicKeyType.DK1, RazerAPI.TargetDisplay.DK1 },
                { RazerAPI.DynamicKeyType.DK2, RazerAPI.TargetDisplay.DK2 },
                { RazerAPI.DynamicKeyType.DK3, RazerAPI.TargetDisplay.DK3 },
                { RazerAPI.DynamicKeyType.DK4, RazerAPI.TargetDisplay.DK4 },
                { RazerAPI.DynamicKeyType.DK5, RazerAPI.TargetDisplay.DK5 },
                { RazerAPI.DynamicKeyType.DK6, RazerAPI.TargetDisplay.DK6 },
                { RazerAPI.DynamicKeyType.DK7, RazerAPI.TargetDisplay.DK7 },
                { RazerAPI.DynamicKeyType.DK8, RazerAPI.TargetDisplay.DK8 },
                { RazerAPI.DynamicKeyType.DK9, RazerAPI.TargetDisplay.DK9 },
                { RazerAPI.DynamicKeyType.DK10, RazerAPI.TargetDisplay.DK10 }
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
            RazerAPI.DynamicKeyType keyType,
            string image,
            string pressedImage = null,
            EventHandler callback = null)
            : base(TargetDisplayMapping[keyType], RazerAPI.DynamicKeyHeight, RazerAPI.DynamicKeyWidth)
        {
            Contract.Requires(!string.IsNullOrEmpty(image));
            Contract.Ensures(_log != null);
            Contract.Ensures(!string.IsNullOrEmpty(UpImage));
            Contract.Ensures(!string.IsNullOrEmpty(DownImage));

            _log = LogManager.GetLogger(this);

            if (string.IsNullOrEmpty(pressedImage))
            {
                _log.Debug("pressedImage is null, setting to value of image");
                pressedImage = image;
            }

            _log.Debug("Setting default states");
            State = RazerAPI.DynamicKeyState.None;
            PreviousState = RazerAPI.DynamicKeyState.None;
            KeyType = keyType;

            // Set{Up,Down}Image will also set the relevant properties
            _log.Debug("Setting images");
            SetUpImage(image);
            SetDownImage(pressedImage);

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
        /// Gets the current image being displayed on the render target.
        /// </summary>
        public override string CurrentImage
        {
            get { return UpImage; }
            protected set { SetImage(value); }
        }

        /// <summary>
        /// Gets the image displayed on this key when in DOWN state.
        /// </summary>
        public string DownImage { get; private set; }

        /// <summary>
        /// Gets the <see cref="RazerAPI.DynamicKeyType" /> of this key.
        /// </summary>
        public RazerAPI.DynamicKeyType KeyType { get; private set; }

        /// <summary>
        /// Gets the previous state of this key.
        /// </summary>
        public RazerAPI.DynamicKeyState PreviousState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this key is
        /// using a single image for both UP and DOWN states.
        /// </summary>
        public bool SingleImage
        {
            get
            {
                return UpImage == DownImage;
            }
        }

        /// <summary>
        /// Gets the current state of this key.
        /// </summary>
        public RazerAPI.DynamicKeyState State { get; private set; }

        /// <summary>
        /// Gets the image displayed on this key when in the UP state.
        /// </summary>
        public string UpImage { get; private set; }

        /// <summary>
        /// Disables this dynamic key (sets to blank image).
        /// </summary>
        public void Disable()
        {
            Contract.Assume(RazerManager.Instance != null);
            SetImage(RazerManager.Instance.DisabledDynamicKeyImagePath);
        }

        /// <summary>
        /// Refreshes this dynamic key to make sure its
        /// images are up to date.
        /// </summary>
        public void Refresh()
        {
            SetImages(UpImage, DownImage);
        }

        /// <summary>
        /// Sets the image displayed when key is DOWN.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public void SetDownImage(string image)
        {
            Contract.Requires(!string.IsNullOrEmpty(image));
            Contract.Ensures(!string.IsNullOrEmpty(DownImage));
            SetImage(image, RazerAPI.DynamicKeyState.Down);
        }

        /// <summary>
        /// Sets the image that is displayed on this key.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <remarks>This will set the image on both the UP and DOWN states.</remarks>
        public override void SetImage(string image)
        {
            SetUpImage(image);
            SetDownImage(image);
        }

        /// <summary>
        /// Sets an image for a specific key state.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <param name="state">State to display image in.</param>
        public void SetImage(string image, RazerAPI.DynamicKeyState state)
        {
            Contract.Requires(!string.IsNullOrEmpty(image));
            Contract.Requires(
                state == RazerAPI.DynamicKeyState.Up || state == RazerAPI.DynamicKeyState.Down,
                "State can only be up or down.");
            Contract.Ensures(!string.IsNullOrEmpty(UpImage) || !string.IsNullOrEmpty(DownImage));

            _log.DebugFormat("Setting {0} on {1} to {2}", state, KeyType, image);

            var result = RazerAPI.NativeMethods.RzSBSetImageDynamicKey(KeyType, state, GenericMethods.GetAbsolutePath(image));
            if (!HRESULT.RZSB_SUCCESS(result))
                throw new RazerNativeException("RzSBSetImageDynamicKey", result);

            if (state == RazerAPI.DynamicKeyState.Up)
                UpImage = image;
            else
                DownImage = image;
        }

        /// <summary>
        /// Sets the images that are displayed on this key.
        /// </summary>
        /// <param name="image">Path to image displayed when this key is in the "UP" state.</param>
        /// <param name="pressedImage">Path to the image displayed when this key is in the "DOWN" state.</param>
        public void SetImages(string image, string pressedImage)
        {
            Contract.Requires(!string.IsNullOrEmpty(image) && !string.IsNullOrEmpty(pressedImage));
            Contract.Ensures(!string.IsNullOrEmpty(UpImage) && !string.IsNullOrEmpty(DownImage));

            SetUpImage(image);
            SetDownImage(pressedImage);
        }

        /// <summary>
        /// Sets the image displayed when key is UP.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public void SetUpImage(string image)
        {
            Contract.Requires(!string.IsNullOrEmpty(image));
            Contract.Ensures(!string.IsNullOrEmpty(UpImage));
            SetImage(image, RazerAPI.DynamicKeyState.Up);
        }

        /// <summary>
        /// Updates this key's state and handles events.
        /// </summary>
        /// <param name="state">The new state to assign to this key.</param>
        internal void UpdateState(RazerAPI.DynamicKeyState state)
        {
            PreviousState = State;
            State = state;
            if (State == RazerAPI.DynamicKeyState.Up
                && (PreviousState == RazerAPI.DynamicKeyState.Down || PreviousState == RazerAPI.DynamicKeyState.None))
                OnPressed();
        }

        /// <summary>
        /// Clears the image currently on the dynamic key.
        /// </summary>
        protected override void ClearImage()
        {
            Contract.Assume(RazerManager.Instance != null);
            SetImage(RazerManager.Instance.DisabledDynamicKeyImagePath);
        }

        /// <summary>
        /// The contract invariant method for <see cref="DynamicKey" />.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_log != null);
            Contract.Invariant(!string.IsNullOrEmpty(UpImage));
            Contract.Invariant(!string.IsNullOrEmpty(DownImage));
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
