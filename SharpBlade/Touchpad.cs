// ---------------------------------------------------------------------------------------
// <copyright file="Touchpad.cs" company="SharpBlade">
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
using System.Diagnostics.Contracts;
using System.Linq;

using SharpBlade.Events;
using SharpBlade.Helpers;
using SharpBlade.Logging;
using SharpBlade.Native;
using SharpBlade.Razer;

namespace SharpBlade
{
    /// <summary>
    /// The SwitchBlade touchpad.
    /// </summary>
    public sealed class Touchpad : RenderTarget
    {
        /// <summary>
        /// Gesture callback that is used as parameter in <see cref="NativeMethods.RzSBGestureSetCallback" />.
        /// </summary>
        private static TouchpadGestureCallback _gestureCallback;

        /// <summary>
        /// Instance of Touchpad for the singleton.
        /// </summary>
        private static Touchpad _instance;

        /// <summary>
        /// Log object for the <see cref="Touchpad" />.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Prevents a default instance of the <see cref="Touchpad" /> class from being created.
        /// </summary>
        private Touchpad() : base(Razer.TargetDisplay.Widget, Razer.Constants.TouchpadHeight, Razer.Constants.TouchpadWidth)
        {
            Contract.Ensures(_log != null);

            CurrentNativeWindow = IntPtr.Zero;
            _log = LogManager.GetLogger(this);
            _log.Info("Setting disabled image");
            _log.Debug("Setting gesture callback");
            _gestureCallback = HandleTouchpadGesture;
            var result = NativeMethods.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBGestureSetCallback", result);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Touchpad" /> class.
        /// Allows an object to try to free resources and perform other
        /// cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Touchpad()
        {
            Dispose(false);
        }

        /// <summary>
        /// Raised when a finger(s?) flick on the touchpad.
        /// </summary>
        public event EventHandler<FlickEventArgs> Flick;

        /// <summary>
        /// Raised when a gesture occurs on the touchpad.
        /// </summary>
        public event EventHandler<GestureEventArgs> Gesture;

        /// <summary>
        /// Raised when a finger is held on the touchpad.
        /// </summary>
        public event EventHandler<GestureEventArgs> Hold;

        /// <summary>
        /// Raised when the finger moves on the touchpad.
        /// </summary>
        public event EventHandler<MoveEventArgs> Move;

        /// <summary>
        /// Raised when the touchpad is pressed.
        /// </summary>
        public event EventHandler<PressEventArgs> Press;

        /// <summary>
        /// Raised when a finger is released from the touchpad.
        /// </summary>
        public event EventHandler<ReleaseEventArgs> Release;

        /// <summary>
        /// Raised when a rotating motion is performed on the touchpad.
        /// </summary>
        public event EventHandler<RotateEventArgs> Rotate;

        /// <summary>
        /// Raised when a scrolling motion is performed on the touchpad.
        /// </summary>
        public event EventHandler<GestureEventArgs> Scroll;

        /// <summary>
        /// Raised when the touchpad is tapped.
        /// </summary>
        public event EventHandler<TapEventArgs> Tap;

        /// <summary>
        /// Raised when a pinch motion is performed on the touchpad.
        /// </summary>
        public event EventHandler<ZoomEventArgs> Zoom;

        /// <summary>
        /// Gets the path to the image currently shown on the touchpad,
        /// or null if no image is showing.
        /// </summary>
        public override string CurrentImage { get; protected set; }

        /// <summary>
        /// Gets singleton instance of Touchpad.
        /// </summary>
        internal static Touchpad Instance
        {
            get
            {
                Contract.Ensures(Contract.Result<Touchpad>() != null);
                return _instance ?? (_instance = new Touchpad());
            }
        }

        #region Gesture Methods

        /// <summary>
        /// Disables a gesture from being handled by the touchpad.
        /// </summary>
        /// <param name="gestureTypes">Gesture to disable.</param>
        [CLSCompliant(false)]
        public void DisableGesture(GestureTypes gestureTypes)
        {
            SetGesture(gestureTypes, false);
        }

        /// <summary>
        /// Disables forwarding of a gesture.
        /// </summary>
        /// <param name="gestureTypes">Gesture to disable.</param>
        [CLSCompliant(false)]
        public void DisableOSGesture(GestureTypes gestureTypes)
        {
            SetOSGesture(gestureTypes, false);
        }

        /// <summary>
        /// Enables a gesture to be handled by the touchpad.
        /// </summary>
        /// <param name="gestureTypes">Gesture to enable.</param>
        [CLSCompliant(false)]
        public void EnableGesture(GestureTypes gestureTypes)
        {
            SetGesture(gestureTypes, true);
        }

        /// <summary>
        /// Enables a gesture to be forwarded to the host operating system.
        /// </summary>
        /// <param name="gestureTypes">Gesture to forward.</param>
        [CLSCompliant(false)]
        public void EnableOSGesture(GestureTypes gestureTypes)
        {
            SetOSGesture(gestureTypes, true);
        }

        /// <summary>
        /// Sets whether a gesture should be handled by the touchpad.
        /// </summary>
        /// <param name="gestureTypes">The gesture type to set.</param>
        /// <param name="enabled">True to enable gesture, false to disable.</param>
        [CLSCompliant(false)]
        public void SetGesture(GestureTypes gestureTypes, bool enabled)
        {
            _log.DebugFormat("SetGesture is {0} gestures: {1}", enabled ? "enabling" : "disabling", gestureTypes);

            // TODO: Fix this hacky workaround when Razer fixes their code
            var values =
                Enum.GetValues(gestureTypes.GetType())
                    .Cast<Enum>()
                    .Where(gestureTypes.HasFlag)
                    .Cast<GestureTypes>();

            HRESULT result;

            foreach (var value in values)
            {
                result = NativeMethods.RzSBEnableGesture(value, enabled);
                if (HRESULT.RZSB_FAILED(result))
                    throw new NativeCallException("RzSBEnableGesture", result);
            }

            result = NativeMethods.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBGestureSetCallback", result);
        }

        /// <summary>
        /// Sets whether a gesture should be forwarded to the host operating system.
        /// </summary>
        /// <param name="gestureTypes">Gesture to set.</param>
        /// <param name="enabled">True to enable forwarding, false to disable.</param>
        [CLSCompliant(false)]
        public void SetOSGesture(GestureTypes gestureTypes, bool enabled)
        {
            _log.DebugFormat("SetOSGesture is {0} gestures: {1}", enabled ? "enabling" : "disabling", gestureTypes);

            // TODO: Fix this hacky workaround when Razer fixes their code
            var values =
                Enum.GetValues(gestureTypes.GetType())
                    .Cast<Enum>()
                    .Where(gestureTypes.HasFlag)
                    .Cast<GestureTypes>();

            foreach (var value in values)
            {
                var result = NativeMethods.RzSBEnableGesture(value, enabled);
                if (HRESULT.RZSB_FAILED(result))
                    throw new NativeCallException("RzSBEnableGesture", result);

                result = NativeMethods.RzSBEnableOSGesture(value, enabled);
                if (HRESULT.RZSB_FAILED(result))
                    throw new NativeCallException("RzSBEnableOSGesture", result);
            }
        }

        #endregion Gesture Methods

        #region Drawing Methods

        /// <summary>
        /// Set a static image to be displayed on the touchpad.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public override void SetImage(string image)
        {
            Clear();

            image = GenericMethods.GetAbsolutePath(image);

            var result = NativeMethods.RzSBSetImageTouchpad(image);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBSetImageTouchpad", result);

            CurrentImage = image;
        }

        /// <summary>
        /// Clears the image currently on the touchpad.
        /// </summary>
        protected override void ClearImage()
        {
            var result =
                NativeMethods.RzSBSetImageTouchpad(
                    GenericMethods.GetAbsolutePath(Switchblade.Instance.BlankTouchpadImagePath));
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBSetImageTouchpad", result);

            CurrentImage = null;
        }

        #endregion Drawing Methods

        #region Event Dispatchers

        /// <summary>
        /// Raises flick gesture event to subscribers.
        /// </summary>
        /// <param name="touchCount">Number of touch points</param>
        /// <param name="direction">Direction of flick.</param>
        private void OnFlick(uint touchCount, Direction direction)
        {
            var func = Flick;
            if (func != null)
                func(this, new FlickEventArgs(touchCount, direction));
        }

        /// <summary>
        /// Raises gesture event to subscribers.
        /// </summary>
        /// <param name="gestureTypes">Type of gesture.</param>
        /// <param name="parameter">Contextual parameter associated with gesture.</param>
        /// <param name="x">X position of gesture.</param>
        /// <param name="y">Y position of gesture.</param>
        /// <param name="z">Z position of gesture.</param>
        private void OnGesture(GestureTypes gestureTypes, uint parameter, ushort x, ushort y, ushort z)
        {
            var func = Gesture;
            if (func != null)
                func(this, new GestureEventArgs(gestureTypes, parameter, x, y, z));
        }

        /// <summary>
        /// Raises hold gesture event to subscribers.
        /// </summary>
        /// <param name="param">Parameter associated with hold gesture.</param>
        /// <param name="x">X position of hold.</param>
        /// <param name="y">Y position of hold.</param>
        /// <param name="z">Z position of hold.</param>
        private void OnHold(uint param, ushort x, ushort y, ushort z)
        {
            var func = Hold;
            if (func != null)
                func(this, new GestureEventArgs(GestureTypes.Hold, param, x, y, z));
        }

        /// <summary>
        /// Raises move gesture event to subscribers.
        /// </summary>
        /// <param name="x">New X position.</param>
        /// <param name="y">New Y position.</param>
        private void OnMove(ushort x, ushort y)
        {
            var func = Move;
            if (func != null)
                func(this, new MoveEventArgs(x, y));
        }

        /// <summary>
        /// Raises press gesture event to subscribers.
        /// </summary>
        /// <param name="touchCount">Number of touch points.</param>
        /// <param name="x">X position of press.</param>
        /// <param name="y">Y position of press.</param>
        private void OnPress(uint touchCount, ushort x, ushort y)
        {
            var func = Press;
            if (func != null)
                func(this, new PressEventArgs(touchCount, x, y));
        }

        /// <summary>
        /// Raises release gesture event to subscribers.
        /// </summary>
        /// <param name="touchCount">Number of touch points.</param>
        /// <param name="x">X position of release.</param>
        /// <param name="y">Y position of release.</param>
        private void OnRelease(uint touchCount, ushort x, ushort y)
        {
            var func = Release;
            if (func != null)
                func(this, new ReleaseEventArgs(touchCount, x, y));
        }

        /// <summary>
        /// Raises rotate gesture event to subscribers.
        /// </summary>
        /// <param name="direction">Rotate direction.</param>
        private void OnRotate(RotateDirection direction)
        {
            var func = Rotate;
            if (func != null)
                func(this, new RotateEventArgs(direction));
        }

        /// <summary>
        /// Raises scroll gesture event to subscribers.
        /// </summary>
        /// <param name="param">Parameter associated with this scroll event.</param>
        /// <param name="x">X position of scroll.</param>
        /// <param name="y">Y position of scroll.</param>
        /// <param name="z">Z position of scroll.</param>
        private void OnScroll(uint param, ushort x, ushort y, ushort z)
        {
            var func = Scroll;
            if (func != null)
                func(this, new GestureEventArgs(GestureTypes.Scroll, param, x, y, z));
        }

        /// <summary>
        /// Raises tap gesture event to subscribers.
        /// </summary>
        /// <param name="x">X position of tap.</param>
        /// <param name="y">Y position of tap.</param>
        private void OnTap(ushort x, ushort y)
        {
            var func = Tap;
            if (func != null)
                func(this, new TapEventArgs(x, y));
        }

        /// <summary>
        /// Raises zoom gesture event to subscribers.
        /// </summary>
        /// <param name="direction">Zoom direction.</param>
        private void OnZoom(ZoomDirection direction)
        {
            var func = Zoom;
            if (func != null)
                func(this, new ZoomEventArgs(direction));
        }

        #endregion Event Dispatchers

        #region Native Handlers

        /// <summary>
        /// Handles the native gesture event sent from Razer SDK.
        /// </summary>
        /// <param name="gestureTypes">Type of gesture.</param>
        /// <param name="parameters">Parameters associated with gesture event.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="z">Z position.</param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        private HRESULT HandleTouchpadGesture(
            GestureTypes gestureTypes,
            uint parameters,
            ushort x,
            ushort y,
            ushort z)
        {
            OnGesture(gestureTypes, parameters, x, y, z);

            switch (gestureTypes)
            {
                case GestureTypes.Press:
                    OnPress(parameters, x, y);
                    break;

                case GestureTypes.Tap:
                    OnTap(x, y);
                    break;

                case GestureTypes.Flick:
                {
                    Contract.Assume(Enum.IsDefined(typeof(Direction), z));
                    var direction = (Direction)z;
                    OnFlick(parameters, direction);
                    break;
                }

                case GestureTypes.Zoom:
                {
                    ZoomDirection direction;
                    switch (parameters)
                    {
                        case 1:
                            direction = ZoomDirection.In;
                            break;

                        case 2:
                            direction = ZoomDirection.Out;
                            break;

                        default:
                            direction = ZoomDirection.Invalid;
                            break;
                    }

                    OnZoom(direction);
                    break;
                }

                case GestureTypes.Rotate:
                {
                    RotateDirection direction;
                    switch (parameters)
                    {
                        case 1:
                            direction = RotateDirection.Clockwise;
                            break;

                        case 2:
                            direction = RotateDirection.Counterclockwise;
                            break;

                        default:
                            direction = RotateDirection.Invalid;
                            break;
                    }

                    OnRotate(direction);
                    break;
                }

                case GestureTypes.Move:
                    OnMove(x, y);
                    break;

                case GestureTypes.Hold:
                    OnHold(parameters, x, y, z);
                    break;

                case GestureTypes.Release:
                    OnRelease(parameters, x, y);
                    break;

                case GestureTypes.Scroll:
                    OnScroll(parameters, x, y, z);
                    break;
            }

            return HRESULT.RZSB_OK;
        }

        #endregion Native Handlers

        /// <summary>
        /// The contract invariant method for <see cref="Touchpad" />.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_log != null);
        }
    }
}
