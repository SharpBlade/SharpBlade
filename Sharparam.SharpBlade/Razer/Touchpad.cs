//---------------------------------------------------------------------------------------
// <copyright file="Touchpad.cs" company="SharpBlade">
//     Copyright (c) 2013-2014 by Adam Hellberg and Brandon Scott.
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
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Sharparam.SharpBlade.Extensions;
using Sharparam.SharpBlade.Helpers;
using Sharparam.SharpBlade.Integration;
using Sharparam.SharpBlade.Logging;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer.Events;
using Sharparam.SharpBlade.Razer.Exceptions;

namespace Sharparam.SharpBlade.Razer
{
    /// <summary>
    /// The SwitchBlade touchpad.
    /// </summary>
    public class Touchpad : IDisposable
    {
        /// <summary>
        /// Instance of Touchpad for the singleton.
        /// </summary>
        private static Touchpad _instance;

        /// <summary>
        /// Gesture callback that is used as parameter in <see cref="RazerAPI.NativeMethods.RzSBGestureSetCallback" />.
        /// </summary>
        private static RazerAPI.TouchpadGestureCallbackFunctionDelegate _gestureCallback;

        /// <summary>
        /// Log object for the <see cref="Touchpad" />.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Indicates whether the <see cref="Touchpad" /> has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Currently active gestures.
        /// </summary>
        private RazerAPI.GestureType _activeGesturesType;

        /// <summary>
        /// Currently active gestures that are being forwarded to the OS.
        /// </summary>
        private RazerAPI.GestureType _activeOSGesturesType;
        
        /// <summary>
        /// Whether or not all gestures are currently enabled.
        /// </summary>
        private bool _allGestureEnabled;

        /// <summary>
        /// Whether or not all gestures are currently being forwarded to the OS.
        /// </summary>
        private bool _allOSGestureEnabled;

        /// <summary>
        /// <see cref="Renderer" /> instance used to render forms or windows.
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// Prevents a default instance of the <see cref="Touchpad" /> class from being created.
        /// </summary>
        private Touchpad()
        {
            CurrentNativeWindow = IntPtr.Zero;
            _log = LogManager.GetLogger(this);
            _log.Info("Setting disabled image");
            _log.Debug("Setting gesture callback");
            _gestureCallback = HandleTouchpadGesture;
            var result = RazerAPI.NativeMethods.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBGestureSetCallback", result);
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
        /// Raised when a gesture occurs on the touchpad.
        /// </summary>
        public event EventHandler<GestureEventArgs> Gesture;

        /// <summary>
        /// Raised when the touchpad is pressed.
        /// </summary>
        public event EventHandler<PressEventArgs> Press;

        /// <summary>
        /// Raised when the touchpad is tapped.
        /// </summary>
        public event EventHandler<TapEventArgs> Tap;

        /// <summary>
        /// Raised when a finger(s?) flick on the touchpad.
        /// </summary>
        public event EventHandler<FlickEventArgs> Flick;

        /// <summary>
        /// Raised when a pinch motion is performed on the touchpad.
        /// </summary>
        public event EventHandler<ZoomEventArgs> Zoom;

        /// <summary>
        /// Raised when a rotating motion is performed on the touchpad.
        /// </summary>
        public event EventHandler<RotateEventArgs> Rotate;

        /// <summary>
        /// Raised when the finger moves on the touchpad.
        /// </summary>
        public event EventHandler<MoveEventArgs> Move;

        /// <summary>
        /// Raised when a finger is held on the touchpad.
        /// </summary>
        public event EventHandler<GestureEventArgs> Hold;

        /// <summary>
        /// Raised when a finger is released from the touchpad.
        /// </summary>
        public event EventHandler<ReleaseEventArgs> Release;

        /// <summary>
        /// Raised when a scrolling motion is performed on the touchpad.
        /// </summary>
        public event EventHandler<GestureEventArgs> Scroll;

        /// <summary>
        /// Specifies what method to use for
        /// rendering a form or window.
        /// </summary>
        public enum RenderMethod
        {
            /// <summary>
            /// Event-based. Subscribes to the form's/window's
            /// paint/draw event and performs an update to the SBUI.
            /// This method is the most resource intensive.
            /// </summary>
            Event,

            /// <summary>
            /// Creates a timer and performs drawing update
            /// at a specified interval.
            /// Less resource intensive than event-based drawing.
            /// </summary>
            Polling
        }

        /// <summary>
        /// Gets the currently active form, null if no form is set.
        /// </summary>
        public Form CurrentForm { get; private set; }

        /// <summary>
        /// Gets the currently rendering Native window, <c>IntPtr.Zero</c> if no window set
        /// </summary>
        public IntPtr CurrentNativeWindow { get; private set; }

        /// <summary>
        /// Gets the currently rendering WPF window, null if no window is set.
        /// </summary>
        public Window CurrentWindow { get; private set; }

        /// <summary>
        /// Gets the path to the image currently shown on the touchpad,
        /// or null if no image is showing.
        /// </summary>
        public string CurrentImage { get; private set; }

        /// <summary>
        /// Gets singleton instance of Touchpad.
        /// </summary>
        internal static Touchpad Instance
        {
            get { return _instance ?? (_instance = new Touchpad()); }
        }

        /// <summary>
        /// Disposes of this touchpad.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Gesture Methods

        /// <summary>
        /// Sets whether a gesture should be handled by the touchpad.
        /// </summary>
        /// <param name="gestureType">The gesture type to set.</param>
        /// <param name="enabled">True to enable gesture, false to disable.</param>
        public void SetGesture(RazerAPI.GestureType gestureType, bool enabled)
        {
            RazerAPI.GestureType newGesturesType;
            if (gestureType == RazerAPI.GestureType.All)
                newGesturesType = gestureType;
            else if (gestureType == RazerAPI.GestureType.None)
            {
                if (_activeGesturesType == RazerAPI.GestureType.None)
                {
                    _log.Debug("Active gestures already set to none, aborting.");
                    return;
                }

                if (!enabled)
                {
                    // Request to "disable no gesture"?
                    // Then just enable all, since that's the same
                    _log.Debug("Requested to set none disabled, calling set all enabled instead");
                    SetGesture(RazerAPI.GestureType.All, true);
                    return;
                }

                newGesturesType = gestureType;
            }
            else if (enabled)
            {
                if (_activeGesturesType.Has(gestureType) &&
                    !(_activeGesturesType == RazerAPI.GestureType.All && !_allGestureEnabled))
                {
                    _log.Debug("Active gestures already have requested value");
                    _log.DebugFormat("_activeGestures == {0}", _activeGesturesType);
                    _log.DebugFormat("_allGestureEnabled == {0}", _allGestureEnabled);
                    return;
                }

                newGesturesType = _activeGesturesType.Include(gestureType);
            }
            else
            {
                if (!_activeGesturesType.Has(gestureType))
                {
                    _log.DebugFormat("Request to disable gesture already disabled: {0}", gestureType);
                    _log.DebugFormat("_activeGestures == {0}", _activeGesturesType);
                    return;
                }

                newGesturesType = _activeGesturesType.Remove(gestureType);
            }

            var result = RazerAPI.NativeMethods.RzSBEnableGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBGestureEnable", result);

            result = RazerAPI.NativeMethods.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBGestureSetCallback", result);

            _activeGesturesType = newGesturesType;
            _allGestureEnabled = _activeGesturesType == RazerAPI.GestureType.All && enabled;
        }

        /// <summary>
        /// Enables a gesture to be handled by the touchpad.
        /// </summary>
        /// <param name="gestureType">Gesture to enable.</param>
        public void EnableGesture(RazerAPI.GestureType gestureType)
        {
            SetGesture(gestureType, true);
        }

        /// <summary>
        /// Disables a gesture from being handled by the touchpad.
        /// </summary>
        /// <param name="gestureType">Gesture to disable.</param>
        public void DisableGesture(RazerAPI.GestureType gestureType)
        {
            SetGesture(gestureType, false);
        }

        /// <summary>
        /// Sets whether a gesture should be forwarded to the host operating system.
        /// </summary>
        /// <param name="gestureType">Gesture to set.</param>
        /// <param name="enabled">True to enable forwarding, false to disable.</param>
        public void SetOSGesture(RazerAPI.GestureType gestureType, bool enabled)
        {
            RazerAPI.GestureType newGesturesType;
            if (gestureType == RazerAPI.GestureType.All)
            {
                // Invert the enabled value because of how Razer API works
                // UPDATE (2013-08-31): Seems to have changed, we no longer need to invert
                ////enabled = !enabled;
                // "ALL" replaces any other gesture, so we don't want to include/remove it
                newGesturesType = gestureType;
            }
            else if (gestureType == RazerAPI.GestureType.None)
            {
                if (_activeOSGesturesType == RazerAPI.GestureType.None)
                    return;

                if (!enabled) 
                {
                    // Request to "disable no gesture"?
                    // Then just enable all, since that's the same
                    SetOSGesture(RazerAPI.GestureType.All, true);
                    return;
                }

                // "NONE" replaces any other gesture, so we don't want to include/remove it
                newGesturesType = gestureType;
            }
            else if (enabled)
            {
                if (_activeOSGesturesType.Has(gestureType) || !(_activeOSGesturesType == RazerAPI.GestureType.All && !_allOSGestureEnabled))
                    return;
                newGesturesType = _activeOSGesturesType.Include(gestureType);
            }
            else
            {
                if (!_activeOSGesturesType.Has(gestureType))
                    return;
                newGesturesType = _activeOSGesturesType.Remove(gestureType);
            }

            var result = RazerAPI.NativeMethods.RzSBEnableGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBGestureEnable", result);

            result = RazerAPI.NativeMethods.RzSBEnableOSGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBGestureSetOSNotification", result);

            _activeOSGesturesType = newGesturesType;
            _allOSGestureEnabled = _activeGesturesType == RazerAPI.GestureType.All && enabled;
        }

        /// <summary>
        /// Enables a gesture to be forwarded to the host operating system.
        /// </summary>
        /// <param name="gestureType">Gesture to forward.</param>
        public void EnableOSGesture(RazerAPI.GestureType gestureType)
        {
            SetOSGesture(gestureType, true);
        }

        /// <summary>
        /// Disables forwarding of a gesture.
        /// </summary>
        /// <param name="gestureType">Gesture to disable.</param>
        public void DisableOSGesture(RazerAPI.GestureType gestureType)
        {
            SetOSGesture(gestureType, false);
        }

        #endregion Gesture Methods

        #region Drawing Methods

        /// <summary>
        /// Renders a raw bitmap to the touchpad display.
        /// </summary>
        /// <param name="bitmap">The bitmap object to draw.</param>
        /// <remarks>Please make sure your bitmap object size matches the size
        /// of the touchpad display, or you may get unexpected results.</remarks>
        public void DrawBitmap(Bitmap bitmap)
        {
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb565);

            var buffer = new RazerAPI.BufferParams
            {
                PixelType = RazerAPI.PixelType.RGB565,
                DataSize = (uint)(bitmap.Width * bitmap.Height * sizeof(ushort)),
                PtrData = data.Scan0
            };

            var ptrToImageStruct = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
            Marshal.StructureToPtr(buffer, ptrToImageStruct, true);

            var result = RazerAPI.NativeMethods.RzSBRenderBuffer(RazerAPI.TargetDisplay.Widget, ptrToImageStruct);

            // Free resources before handling return
            Marshal.FreeHGlobal(ptrToImageStruct);
            bitmap.UnlockBits(data);

            bitmap.Dispose();

            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBRenderBuffer", result);
        }

        /// <summary>
        /// Draws the specified form to the touchpad.
        /// </summary>
        /// <param name="form">Form to draw.</param>
        public void DrawForm(Form form)
        {
            var bmp = new Bitmap(RazerAPI.TouchpadWidth, RazerAPI.TouchpadHeight);
            form.DrawToBitmap(bmp, form.Bounds);
            DrawBitmap(bmp);
            bmp.Dispose();
        }

        /// <summary>
        /// Draws the specified native window to the touchpad
        /// </summary>
        /// <param name="windowHandle">the window handle of the window to draw</param>
        public void DrawNativeWindow(IntPtr windowHandle)
        {
            var img = ScreenCapture.CaptureWindow(windowHandle);
            var bitmapToRender = new Bitmap(img, RazerAPI.TouchpadWidth, RazerAPI.TouchpadHeight);
            DrawBitmap(bitmapToRender);
            bitmapToRender.Dispose();
            img.Dispose();
        }

        /// <summary>
        /// Draws a WPF window to the touchpad.
        /// </summary>
        /// <param name="window">Window object to draw.</param>
        /// <param name="winFormsComponents">Array of KeyValuePairs containing a WindowsFormsHost as the key and a WinForms control as the value.
        /// These pairs will be overlaid on the bitmap that is passed to the SwitchBlade device.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "SO said it's safe to dispose MemoryStream multiple times")]
        public void DrawWindow(Window window, IEnumerable<EmbeddedWinFormsControl> winFormsComponents = null)
        {
            var rtb = new RenderTargetBitmap(
                RazerAPI.TouchpadWidth, RazerAPI.TouchpadHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            rtb.Render(window);

            BitmapEncoder encoder = new BmpBitmapEncoder();

            using (var stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream);

                using (var bitmap = new Bitmap(stream))
                {
                    if (winFormsComponents != null)
                        using (var graphics = Graphics.FromImage(bitmap))
                            foreach (var component in winFormsComponents)
                                graphics.DrawImage(component.Draw(), component.Bounds);
                    DrawBitmap(bitmap);
                }
                
                encoder.Frames.Clear();
            } // CA2202 warning marked here, for reference, complains about possible multiple dispose of MemoryStream stream

            rtb.Clear();
        }

        /// <summary>
        /// Sets the form to be rendered to this touchpad.
        /// </summary>
        /// <param name="form">The new form to render.</param>
        /// <param name="method">The method to use for rendering the form.</param>
        /// <param name="interval">Interval to poll drawing functions at,
        /// only used if RenderMethod is set to Polling.
        /// Default value 55ms (circa 18 FPS).</param>
        public void SetForm(Form form, RenderMethod method = RenderMethod.Event, int interval = 55)
        {
            ClearForm();

            CurrentForm = form;

            if (method == RenderMethod.Event)
                CurrentForm.Paint += FormPaintHandler;
            else
                _renderer = new Renderer(this, CurrentForm, interval);
        }

        /// <summary>
        /// Clears the current form from touchpad
        /// and stops rendering of it.
        /// </summary>
        public void ClearForm()
        {
            if (CurrentForm != null && _renderer == null)
                CurrentForm.Paint -= FormPaintHandler;

            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }

            CurrentForm = null;
        }

        /// <summary>
        /// Sets the native window to be rendered to this touchpad
        /// Initializes the polling interval to 42ms (circa 24 FPS)
        /// </summary>
        /// <param name="windowHandle">the handle for the window to render</param>
        public void SetNativeWindow(IntPtr windowHandle)
        {
            ClearNativeWindow();

            CurrentNativeWindow = windowHandle;

            _renderer = new Renderer(this, windowHandle, new TimeSpan(0, 0, 0, 0, 42));
        }

        /// <summary>
        /// Clears the current native window from 
        /// the touchpad and stops rendering of it
        /// </summary>
        public void ClearNativeWindow()
        {
            CurrentNativeWindow = IntPtr.Zero;

            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }
        }

        /// <summary>
        /// Sets the WPF window to be rendered to this touchpad.
        /// Initializes the polling interval to 42ms (circa 24 FPS)
        /// if called with RenderMethod set to Polling.
        /// </summary>
        /// <param name="window">The new window to render.</param>
        /// <param name="method">The method to use for rendering the window.</param>
        public void SetWindow(Window window, RenderMethod method = RenderMethod.Event)
        {
            SetWindow(window, method, new TimeSpan(0, 0, 0, 0, 42));
        }

        /// <summary>
        /// Sets the WPF window to be rendered to this touchpad.
        /// </summary>
        /// <param name="window">The new window to render.</param>
        /// <param name="method">The method to use for rendering the window</param>
        /// <param name="interval">The interval to poll the window at,
        /// only used if RenderMethod is Polling.</param>
        public void SetWindow(Window window, RenderMethod method, TimeSpan interval)
        {
            ClearWindow();

            CurrentWindow = window;

            if (method == RenderMethod.Event)
                CurrentWindow.ContentRendered += WindowContentRenderedHandler;
            else
                _renderer = new Renderer(this, CurrentWindow, interval);
        }

        /// <summary>
        /// Clears the current WPF window from
        /// touchpad and stops rendering of it.
        /// </summary>
        public void ClearWindow()
        {
            if (CurrentWindow != null && _renderer == null)
                CurrentWindow.ContentRendered -= WindowContentRenderedHandler;

            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }

            CurrentWindow = null;
        }

        /// <summary>
        /// Set a static image to be displayed on the touchpad.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public void SetImage(string image)
        {
            image = IO.GetAbsolutePath(image);

            var result = RazerAPI.NativeMethods.RzSBSetImageTouchpad(image);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBSetImageTouchpad", result);

            CurrentImage = image;
        }

        /// <summary>
        /// Clears the image currently on the touchpad.
        /// </summary>
        public void ClearImage()
        {
            var result = RazerAPI.NativeMethods.RzSBSetImageTouchpad(IO.GetAbsolutePath(RazerManager.Instance.BlankTouchpadImagePath));
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBSetImageTouchpad", result);

            CurrentImage = null;
        }

        /// <summary>
        /// Clears current form and image on the touchpad (if any).
        /// </summary>
        public void ClearAll()
        {
            ClearForm();
            ClearWindow();
            ClearNativeWindow();
            ClearImage();
        }

        #endregion Drawing Methods

        /// <summary>
        /// Disposes of this <see cref="Touchpad" />.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from parameter-less <see cref="Dispose()" />, false otherwise.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _renderer.Dispose();

            _disposed = true;
        }

        #region Event Dispatchers

        /// <summary>
        /// Raises gesture event to subscribers.
        /// </summary>
        /// <param name="gestureType">Type of gesture.</param>
        /// <param name="parameter">Contextual parameter associated with gesture.</param>
        /// <param name="x">X position of gesture.</param>
        /// <param name="y">Y position of gesture.</param>
        /// <param name="z">Z position of gesture.</param>
        private void OnGesture(RazerAPI.GestureType gestureType, uint parameter, ushort x, ushort y, ushort z)
        {
            var func = Gesture;
            if (func != null)
                func(this, new GestureEventArgs(gestureType, parameter, x, y, z));
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
        /// Raises flick gesture event to subscribers.
        /// </summary>
        /// <param name="touchCount">Number of touch points</param>
        /// <param name="direction">Direction of flick.</param>
        private void OnFlick(uint touchCount, RazerAPI.Direction direction)
        {
            var func = Flick;
            if (func != null)
                func(this, new FlickEventArgs(touchCount, direction));
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
                func(this, new GestureEventArgs(RazerAPI.GestureType.Hold, param, x, y, z));
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
                func(this, new GestureEventArgs(RazerAPI.GestureType.Scroll, param, x, y, z));
        }

        #endregion Event Dispatchers

        #region Native Handlers

        /// <summary>
        /// Handles the native gesture event sent from Razer SDK.
        /// </summary>
        /// <param name="gestureType">Type of gesture.</param>
        /// <param name="parameters">Parameters associated with gesture event.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="z">Z position.</param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        private HRESULT HandleTouchpadGesture(RazerAPI.GestureType gestureType, uint parameters, ushort x, ushort y, ushort z)
        {
            OnGesture(gestureType, parameters, x, y, z);

            switch (gestureType)
            {
                case RazerAPI.GestureType.Press:
                    OnPress(parameters, x, y);
                    break;
                case RazerAPI.GestureType.Tap:
                    OnTap(x, y);
                    break;
                case RazerAPI.GestureType.Flick:
                {
                    var direction = (RazerAPI.Direction)z;
                    OnFlick(parameters, direction);
                    break;
                }

                case RazerAPI.GestureType.Zoom:
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

                case RazerAPI.GestureType.Rotate:
                {
                    RotateDirection direction;
                    switch (parameters)
                    {
                        case 1:
                            direction = RotateDirection.Clockwise;
                            break;
                        case 2:
                            direction = RotateDirection.CounterClockwise;
                            break;
                        default:
                            direction = RotateDirection.Invalid;
                            break;
                    }

                    OnRotate(direction);
                    break;
                }

                case RazerAPI.GestureType.Move:
                    OnMove(x, y);
                    break;
                case RazerAPI.GestureType.Hold:
                    OnHold(parameters, x, y, z);
                    break;
                case RazerAPI.GestureType.Release:
                    OnRelease(parameters, x, y);
                    break;
                case RazerAPI.GestureType.Scroll:
                    OnScroll(parameters, x, y, z);
                    break;
            }

            return HRESULT.RZSB_OK;
        }

        #endregion Native Handlers

        #region Form/Window draw handlers

        /// <summary>
        /// Wrapper method to listen for the Paint event on a WinForms Form
        /// and render to touchpad.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void FormPaintHandler(object sender, PaintEventArgs e)
        {
            DrawForm(CurrentForm);
        }

        /// <summary>
        /// Wrapper method to listen for the ContentRendered event on a WPF
        /// Window and render to touchpad.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void WindowContentRenderedHandler(object sender, EventArgs e)
        {
            DrawWindow(CurrentWindow);
        }

        #endregion Form/Window draw handlers
    }
}
