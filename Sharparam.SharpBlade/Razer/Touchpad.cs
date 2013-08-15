/* Touchpad.cs
 *
 * Copyright © 2013 by Adam Hellberg
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Disclaimer: SwitchBladeSteam is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SharpBlade.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Sharparam.SharpBlade.Extensions;
using Sharparam.SharpBlade.Helpers;
using Sharparam.SharpBlade.Logging;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer.Events;

namespace Sharparam.SharpBlade.Razer
{
    /// <summary>
    /// The SwitchBlade touchpad.
    /// </summary>
    public class Touchpad : IDisposable
    {
        /// <summary>
        /// Thrown when a gesture occurs on the touchpad.
        /// </summary>
        public event GestureEventHandler Gesture;

        private readonly ILog _log;

        private RazerAPI.GestureType _activeGesturesType;
        private RazerAPI.GestureType _activeOSGesturesType;

        private bool _allGestureEnabled;
        private bool _allOSGestureEnabled;

        private static RazerAPI.TouchpadGestureCallbackFunctionDelegate _gestureCallback;

        /// <summary>
        /// Will be set to null if no active form.
        /// </summary>
        public Form CurrentForm { get; private set; }

        /// <summary>
        /// Current image shown on the Touchpad, null if not using static image.
        /// </summary>
        public string CurrentImage { get; private set; }

        internal Touchpad()
        {
            _log = LogManager.GetLogger(this);
            _log.Info("Setting disabled image");
            _log.Debug("Setting gesture callback");
            _gestureCallback = HandleTouchpadGesture;
            var hResult = RazerAPI.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureSetCallback", hResult);
        }

        /// <summary>
        /// Disposes of this touchpad.
        /// </summary>
        public void Dispose()
        {
            
        }
        
        private void OnGesture(RazerAPI.GestureType gestureType, uint parameter, ushort x, ushort y, ushort z)
        {
            var func = Gesture;
            if (func != null)
                func(this, new GestureEventArgs(gestureType, parameter, x, y, z));
        }

        /// <summary>
        /// Sets the form to be rendered to this touchpad.
        /// </summary>
        /// <param name="form">The new form to render.</param>
        public void SetForm(Form form)
        {
            ClearForm();

            CurrentForm = form;

            //CurrentForm.Paint += DrawForm;
        }

        /// <summary>
        /// Clears the current form from touchpad
        /// and stops rendering of it.
        /// </summary>
        public void ClearForm()
        {
            if (CurrentForm != null)
            {
                //CurrentForm.Paint -= DrawForm;
            }
        }

        /// <summary>
        /// Set a static image to be displayed on the touchpad.
        /// </summary>
        /// <param name="image">Path to image.</param>
        public void SetImage(string image)
        {
            image = IO.GetAbsolutePath(image);

            var hResult = RazerAPI.RzSBSetImageTouchpad(image);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBSetImageTouchpad", hResult);

            CurrentImage = image;
        }

        /// <summary>
        /// Clears the image currently on the touchpad.
        /// </summary>
        public void ClearImage()
        {
            var hResult = RazerAPI.RzSBSetImageTouchpad(IO.GetAbsolutePath(Constants.BlankTouchpadImage));
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBSetImageTouchpad", hResult);

            CurrentImage = null;
        }

        /// <summary>
        /// Clears current form and image on the touchpad (if any).
        /// </summary>
        public void ClearAll()
        {
            ClearForm();
            ClearImage();
        }

        /// <summary>
        /// Stops rendering of forms and images.
        /// </summary>
        [Obsolete("Renamed to ClearAll")]
        public void StopAll()
        {
            ClearAll();
        }

        /// <summary>
        /// Sets whether a gesture should be handled by the touchpad.
        /// </summary>
        /// <param name="gestureType">The gesture type to set.</param>
        /// <param name="enabled">True to enable gesture, false to disable.</param>
        public void SetGesture(RazerAPI.GestureType gestureType, bool enabled)
        {
            RazerAPI.GestureType newGesturesType;
            if (gestureType == RazerAPI.GestureType.All)
            {
                newGesturesType = gestureType;
                enabled = !enabled;
            }
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
                if (_activeGesturesType.Has(gestureType) && !(_activeGesturesType == RazerAPI.GestureType.All && !_allGestureEnabled))
                {
                    _log.Debug("Active gestures already have requested value");
                    _log.DebugFormat("_activeGestures == {0}", _activeGesturesType);
                    _log.DebugFormat("_allGestureEnabled == {0}", _allGestureEnabled);
                    return;
                }
                newGesturesType = _activeOSGesturesType.Include(gestureType);
            }
            else
            {
                if (!_activeGesturesType.Has(gestureType))
                {
                    _log.DebugFormat("Request to disable gesture already disabled: {0}", gestureType);
                    _log.DebugFormat("_activeGestures == {0}", _activeGesturesType);
                    return;
                }
                newGesturesType = _activeOSGesturesType.Remove(gestureType);
            }

            var hResult = RazerAPI.RzSBEnableGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureEnable", hResult);

            hResult = RazerAPI.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureSetCallback", hResult);

            _activeGesturesType = newGesturesType;
            _allGestureEnabled = _activeGesturesType == RazerAPI.GestureType.All && !enabled;
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
                enabled = !enabled;
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

            var hResult = RazerAPI.RzSBEnableGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureEnable", hResult);

            hResult = RazerAPI.RzSBEnableOSGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureSetOSNotification", hResult);

            _activeOSGesturesType = newGesturesType;
            _allOSGestureEnabled = _activeGesturesType == RazerAPI.GestureType.All && !enabled;
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

        public void DrawBitmap(Bitmap bitmap)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                       ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb565);

            var buffer = new RazerAPI.BufferParams
            {
                PixelType = RazerAPI.PixelType.RGB565,
                DataSize = (uint) (bitmap.Width * bitmap.Height * sizeof (ushort)),
                PtrData = data.Scan0
            };

            var ptrToImageStruct = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
            Marshal.StructureToPtr(buffer, ptrToImageStruct, true);

            var hResult = RazerAPI.RzSBRenderBuffer(RazerAPI.TargetDisplay.Widget, ptrToImageStruct);

            // Free resources before handling return

            Marshal.FreeHGlobal(ptrToImageStruct);
            bitmap.UnlockBits(data);

            bitmap.Dispose();

            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBRenderBuffer", hResult);
        }

        /// <summary>
        /// Draws the specified form to the touchpad.
        /// </summary>
        /// <param name="form">Form to draw.</param>
        public void DrawForm(Form form)
        {
            // Big thanks to ben_a_adams at the Razer Developer forum
            // for sharing his drawing code.

            if (form == null || form.IsDisposed)
                return;

            var source = new Bitmap(RazerAPI.TouchpadWidth, RazerAPI.TouchpadHeight);
            var dest = new Bitmap(source.Width, source.Height, PixelFormat.Format16bppRgb565);
            form.DrawToBitmap(dest, new Rectangle(0, 0, dest.Width, dest.Height));

            using (var g = Graphics.FromImage(dest))
                g.DrawImageUnscaled(dest, 0, 0);

            var data = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb565);

            var buffer = new RazerAPI.BufferParams
            {
                PixelType = RazerAPI.PixelType.RGB565,
                DataSize = (uint) (dest.Width * dest.Height * sizeof (short)),
                PtrData = data.Scan0
            };

            var ptrToImageStruct = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
            Marshal.StructureToPtr(buffer, ptrToImageStruct, true);

            var hResult = RazerAPI.RzSBRenderBuffer(RazerAPI.TargetDisplay.Widget, ptrToImageStruct);

            // Free resources before handling return

            Marshal.FreeHGlobal(ptrToImageStruct);
            dest.UnlockBits(data);

            dest.Dispose();
            source.Dispose();

            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBRenderBuffer", hResult);
        }

        /// <summary>
        /// Draws a WPF window to the touchpad.
        /// </summary>
        /// <param name="window">Window object to draw.</param>
        public void DrawWindow(Window window)
        {
            var rtb = new RenderTargetBitmap(RazerAPI.TouchpadWidth, RazerAPI.TouchpadHeight, 96, 96,
                                                System.Windows.Media.PixelFormats.Pbgra32);
            rtb.Render(window);

            using (var stream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(rtb));
                enc.Save(stream);
                var bitmap = new Bitmap(stream);
                DrawBitmap(bitmap);
            }
        }

        private HRESULT HandleTouchpadGesture(RazerAPI.GestureType gestureType, uint dwParameters, ushort wXPos, ushort wYPos, ushort wZPos)
        {
            OnGesture(gestureType, dwParameters, wXPos, wYPos, wZPos);
            return HRESULT.RZSB_OK;
        }

        private void FormPaintHandler(object sender, PaintEventArgs e)
        {
            DrawForm(CurrentForm);
        }
    }
}
