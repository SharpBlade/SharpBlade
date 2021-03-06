﻿// ---------------------------------------------------------------------------------------
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

namespace SharpBlade
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using SharpBlade.Events;
    using SharpBlade.Helpers;
    using SharpBlade.Logging;
    using SharpBlade.Razer;
    using SharpBlade.Rendering;

    /// <summary>
    /// Represents a dynamic key on the SwitchBlade device
    /// </summary>
    internal sealed class DynamicKey : RenderTarget, IDynamicKey
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
        /// Blank bitmap object used for clearing the dynamic key.
        /// </summary>
        private readonly Bitmap _blankBitmap;

        /// <summary>
        /// The log instance associated with this object.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Whether or not this key is enabled.
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicKey" /> class.
        /// </summary>
        /// <param name="keyType">The type of dynamic key being initialized.</param>
        /// <param name="image">Image to set for this key's depressed state.</param>
        /// <param name="pressedImage">Image to set for this key's pressed state.</param>
        /// <param name="callback">The function to call when this key is released.</param>
        internal DynamicKey(
            DynamicKeyType keyType,
            string image = null,
            string pressedImage = null,
            EventHandler<DynamicKeyEventArgs> callback = null)
            : base(TargetDisplayMapping[keyType], Constants.DynamicKeyHeight, Constants.DynamicKeyWidth)
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

            _log.Debug("Creating blank bitmap");
            _blankBitmap = GenericMethods.GetBlankBitmap(DisplayWidth, DisplayHeight);

            _log.Debug("Setting images");
            if (image != null)
                Draw(image, pressedImage);

            if (callback != null)
            {
                _log.Debug("Setting callback");
                Released += callback;
            }

            _enabled = true;
        }

        /// <summary>
        /// Raised when the state of this key changes.
        /// </summary>
        /// <remarks>
        /// This event is raised for <b>every</b> state change, it can
        /// potentially lead to very frequent calls.
        /// </remarks>
        public event EventHandler<DynamicKeyEventArgs> Changed; 

        /// <summary>
        /// Raised when a dynamic key is pressed.
        /// </summary>
        public event EventHandler<DynamicKeyEventArgs> Pressed;

        /// <summary>
        /// Raised when this key is released.
        /// </summary>
        public event EventHandler<DynamicKeyEventArgs> Released;

        /// <summary>
        /// Gets or sets a value indicating whether this dynamic key
        /// is currently enabled (propagating events).
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                if (!value)
                    Clear();

                _enabled = value;
            }
        }

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
        /// Sets a static image to both states of the dynamic key.
        /// </summary>
        /// <param name="image">Path to the image file.</param>
        public override void Draw(string image)
        {
            DynamicKeyImageRenderer.Draw(KeyType, image);
        }

        /// <summary>
        /// Sets the images for each state of the dynamic key.
        /// </summary>
        /// <param name="image">Image for the UP state.</param>
        /// <param name="downImage">Image for the DOWN state.</param>
        public void Draw(string image, string downImage)
        {
            DynamicKeyImageRenderer.Draw(KeyType, image, downImage);
        }

        /// <summary>
        /// Sets the image for a specific state on this dynamic key.
        /// </summary>
        /// <param name="state">The state to set an image for.</param>
        /// <param name="image">The image file to set.</param>
        public void Draw(DynamicKeyState state, string image)
        {
            DynamicKeyImageRenderer.Draw(KeyType, state, image);
        }

        /// <summary>
        /// Sets the image for the DOWN state on this key.
        /// </summary>
        /// <param name="image">Image file to set for the state.</param>
        public void DrawDown(string image)
        {
            Draw(DynamicKeyState.Down, image);
        }

        /// <summary>
        /// Sets the image for the UP state on this key.
        /// </summary>
        /// <param name="image">Image file to set for the state.</param>
        public void DrawUp(string image)
        {
            Draw(DynamicKeyState.Up, image);
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
            Renderer = new DynamicKeyImageRenderer(KeyType, image, downImage, interval);
        }

        /// <summary>
        /// Updates this key's state and handles events.
        /// </summary>
        /// <param name="state">The new state to assign to this key.</param>
        internal void UpdateState(DynamicKeyState state)
        {
            PreviousState = State;
            State = state;

            // Don't notify subscribers unless key is enabled
            if (!Enabled)
                return;

            OnChanged();
            if (State == DynamicKeyState.Up
                && (PreviousState == DynamicKeyState.Down || PreviousState == DynamicKeyState.None))
                OnReleased();
            else if (State == DynamicKeyState.Down
                && (PreviousState == DynamicKeyState.Up || PreviousState == DynamicKeyState.None))
                OnPressed();
        }

        /// <summary>
        /// Clears the image currently on the dynamic key.
        /// </summary>
        protected override void ClearImage()
        {
            BitmapRenderer.Draw(this, _blankBitmap);
        }

        /// <summary>
        /// Disposes of this <see cref="DynamicKey" />.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from parameter-less <see cref="Dispose" />, false otherwise.</param>
        [SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose",
            Justification = "It's already calling base dispose in all possible flows (Dispose is ever only true if base dispose has run).")]
        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
                _blankBitmap.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises <see cref="Changed" /> event to subscribers.
        /// </summary>
        private void OnChanged()
        {
            var func = Changed;
            if (func == null)
                return;

            try
            {
                func(this, new DynamicKeyEventArgs(KeyType, State));
            }
            catch (ObjectDisposedException ex)
            {
                _log.ErrorFormat("OnChanged: ObjectDisposedException: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Raises <see cref="Pressed" /> event to subscribers.
        /// </summary>
        private void OnPressed()
        {
            var func = Pressed;
            if (func == null)
                return;

            try
            {
                func(this, new DynamicKeyEventArgs(KeyType, State));
            }
            catch (ObjectDisposedException ex)
            {
                _log.ErrorFormat("OnPressed: ObjectDisposedException: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Raises <see cref="Released" /> event to subscribers.
        /// </summary>
        private void OnReleased()
        {
            var func = Released;
            if (func == null)
                return;

            try
            {
                func(this, new DynamicKeyEventArgs(KeyType, State));
            }
            catch (ObjectDisposedException ex)
            {
                _log.ErrorFormat("OnReleased: ObjectDisposedException: {0}", ex.Message);
            }
        }
    }
}
