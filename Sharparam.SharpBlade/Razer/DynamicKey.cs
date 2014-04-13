// ---------------------------------------------------------------------------------------
//  <copyright file="DynamicKey.cs" company="SharpBlade">
//      Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.
//
//      Permission is hereby granted, free of charge, to any person obtaining a copy of
//      this software and associated documentation files (the "Software"), to deal in
//      the Software without restriction, including without limitation the rights to
//      use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//      of the Software, and to permit persons to whom the Software is furnished to do
//      so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
//
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//      WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//      CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//      Disclaimer: SharpBlade is in no way affiliated
//      with Razer and/or any of its employees and/or licensors.
//      Adam Hellberg does not take responsibility for any harm caused, direct
//      or indirect, to any Razer peripherals via the use of SharpBlade.
//
//      "Razer" is a trademark of Razer USA Ltd.
//  </copyright>
// ---------------------------------------------------------------------------------------

using System;

using Sharparam.SharpBlade.Helpers;
using Sharparam.SharpBlade.Logging;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer.Exceptions;

namespace Sharparam.SharpBlade.Razer
{
    /// <summary>
    /// Represents a dynamic key on the SwitchBlade device
    /// </summary>
    public class DynamicKey
    {
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
        {
            _log = LogManager.GetLogger(this);

            if (string.IsNullOrEmpty(image))
                throw new ArgumentException("Can't be null or empty", "image");

            if (string.IsNullOrEmpty(pressedImage))
            {
                _log.Debug("pressedImage is null, setting to value of image");
                pressedImage = image;
            }

            _log.Debug("Setting default states");
            State = RazerAPI.DynamicKeyState.None;
            PreviousState = RazerAPI.DynamicKeyState.None;
            UpImage = image;
            DownImage = pressedImage;
            KeyType = keyType;

            _log.Debug("Setting images");
            SetUpImage(UpImage);
            SetDownImage(DownImage);

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
        /// Gets the image displayed on this key when in the UP state.
        /// </summary>
        public string UpImage { get; private set; }

        /// <summary>
        /// Gets the image displayed on this key when in DOWN state.
        /// </summary>
        public string DownImage { get; private set; }

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
        /// Gets the <see cref="RazerAPI.DynamicKeyType" /> of this key.
        /// </summary>
        public RazerAPI.DynamicKeyType KeyType { get; private set; }

        /// <summary>
        /// Gets the current state of this key.
        /// </summary>
        public RazerAPI.DynamicKeyState State { get; private set; }

        /// <summary>
        /// Gets the previous state of this key.
        /// </summary>
        public RazerAPI.DynamicKeyState PreviousState { get; private set; }

        /// <summary>
        /// Sets the image that is displayed on this key.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <remarks>This will set the image on both the UP and DOWN states.</remarks>
        public void SetImage(string image)
        {
            SetUpImage(image);
            SetDownImage(image);
        }

        /// <summary>
        /// Sets the images that are displayed on this key.
        /// </summary>
        /// <param name="image">Path to image displayed when this key is in the "UP" state.</param>
        /// <param name="pressedImage">Path to the image displayed when this key is in the "DOWN" state.</param>
        public void SetImages(string image, string pressedImage)
        {
            SetUpImage(image);
            SetDownImage(pressedImage);
        }

        /// <summary>
        /// Sets an image for a specific key state.
        /// </summary>
        /// <param name="image">Path to image.</param>
        /// <param name="state">State to display image in.</param>
        public void SetImage(string image, RazerAPI.DynamicKeyState state)
        {
            if (state != RazerAPI.DynamicKeyState.Up && state != RazerAPI.DynamicKeyState.Down)
                throw new ArgumentException("State can only be up or down", "state");

            _log.DebugFormat("Setting {0} on {1} to {2}", state, KeyType, image);

            var result = RazerAPI.NativeMethods.RzSBSetImageDynamicKey(KeyType, state, IO.GetAbsolutePath(image));
            if (!HRESULT.RZSB_SUCCESS(result))
                throw new RazerNativeException("RzSBSetImageDynamicKey", result);

            if (state == RazerAPI.DynamicKeyState.Up)
                UpImage = image;
            else
                DownImage = image;
        }

        /// <summary>
        /// Sets the image displayed when key is UP.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public void SetUpImage(string image)
        {
            SetImage(image, RazerAPI.DynamicKeyState.Up);
        }

        /// <summary>
        /// Sets the image displayed when key is DOWN.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public void SetDownImage(string image)
        {
            SetImage(image, RazerAPI.DynamicKeyState.Down);
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
        /// Disables this dynamic key (sets to blank image).
        /// </summary>
        public void Disable()
        {
            SetImage(RazerManager.Instance.DisabledDynamicKeyImagePath);
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
                _log.ErrorFormat("OnKeyPressed: ObjectDisposedException: {0}", ex.Message);
            }
        }
    }
}
