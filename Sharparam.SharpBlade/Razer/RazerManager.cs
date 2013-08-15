/* RazerManager.cs
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
using System.IO;
using Sharparam.SharpBlade.Logging;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer.Events;
using Sharparam.SharpBlade.Razer.Exceptions;

namespace Sharparam.SharpBlade.Razer
{
    public delegate void VoidDelegate();

    public class RazerManager : IDisposable
    {
        public event AppEventEventHandler AppEvent;
        public event DynamicKeyEventHandler DynamicKeyEvent;
        public event KeyboardEventRawHandler KeyboardRawEvent;
        public event KeyboardEventHandler KeyboardEvent;

        private const string RazerControlFile = "DO_NOT_TOUCH__RAZER_CONTROL_FILE";

        private readonly ILog _log;
        private static readonly ILog StaticLog = LogManager.GetLogger(typeof (RazerManager));

        private Touchpad _touchpad;
        private readonly DynamicKey[] _dynamicKeys;

        // Native code callbacks
        private static RazerAPI.AppEventCallbackDelegate _appEventCallback;
        private static RazerAPI.DynamicKeyCallbackFunctionDelegate _dkCallback;
        private static RazerAPI.KeyboardCallbackFunctionDelegate _kbCallBack;

        public RazerManager()
        {
            _log = LogManager.GetLogger(this);

            _log.Info("RazerManager is initializing");

            if (File.Exists(RazerControlFile))
            {
                _log.Error("Detected control file presence, throwing exception.");
                throw new RazerUnstableShutdownException();
            }

            CreateControlFile();

            _log.Debug("Calling RzSBStart()");

            var hResult = RazerAPI.RzSBStart();
            if (HRESULT.RZSB_FAILED(hResult))
            {
                // Try one more time
                hResult = RazerAPI.RzSBStart();
                if (HRESULT.RZSB_FAILED(hResult))
                    NativeCallFailure("RzSBStart", hResult);
            }

            _log.Info("Setting up dynamic keys");

            _log.Debug("Creating new app event callback");
            _appEventCallback = HandleAppEvent;
            _log.Debug("Calling RzSBAppEventSetCallback");
            hResult = RazerAPI.RzSBAppEventSetCallback(_appEventCallback);
            if (HRESULT.RZSB_FAILED(hResult))
                NativeCallFailure("RzSBAppEventSetCallback", hResult);

            _log.Info("Setting up touchpad");
            _touchpad = new Touchpad();

            _log.Debug("Creating dynamic key callback");
            _dkCallback = HandleDynamicKeyEvent;
            _log.Debug("Calling RzSBDynamicKeySetCallback");
            hResult = RazerAPI.RzSBDynamicKeySetCallback(_dkCallback);
            if (HRESULT.RZSB_FAILED(hResult))
                NativeCallFailure("RzSBDynamicKeySetCallback", hResult);

            _log.Debug("Creating keyboard callback");
            _kbCallBack = HandleKeyboardRawEvent;
            _log.Debug("Calling RzKeyboardSetCallback");
            hResult = RazerAPI.RzSBKeyboardCaptureSetCallback(_kbCallBack);
            if (HRESULT.RZSB_FAILED(hResult))
                NativeCallFailure("RzKeyboardSetCallback", hResult);

            _log.Debug("Creating keyboard callback");
            _kbCallBack = HandleKeyboardEvent;
            _log.Debug("Calling RzKeyboardSetCallback");
            hResult = RazerAPI.RzSBKeyboardCaptureSetCallback(_kbCallBack);
            if (HRESULT.RZSB_FAILED(hResult))
                NativeCallFailure("RzKeyboardSetCallback", hResult);

            _log.Debug("Initializing dynamic key arrays");

            _dynamicKeys = new DynamicKey[RazerAPI.DynamicKeysCount];
        }

        public void Dispose()
        {
            _log.Debug("RazerManager is disposing...");

            if (_touchpad != null)
            {
                _touchpad.Dispose();
                _touchpad = null;
            }

            _log.Debug("Dispose: Calling Stop()");
            Stop();
        }

        private void OnAppEvent(RazerAPI.AppEventType type, RazerAPI.AppEventMode mode, uint processId)
        {
            var func = AppEvent;
            if (func != null)
                func(this, new AppEventEventArgs(type, mode, processId));
        }

        private void OnDynamicKeyEvent(RazerAPI.DynamicKeyType keyType, RazerAPI.DynamicKeyState state)
        {
            var func = DynamicKeyEvent;
            if (func != null)
                func(this, new DynamicKeyEventArgs(keyType, state));
        }

        private void OnKeyboardRawEvent(uint uMsg, UIntPtr wParam, IntPtr lParam)
        {
            var func = KeyboardRawEvent;
            if (func != null)
                func(this, new KeyboardEventRawArgs(uMsg, wParam, lParam));
        }

        private void OnKeyboardEvent(uint uMsg, UIntPtr wParam, IntPtr lParam)
        {
            var func = KeyboardEvent;
            if (func != null)
                func(this, new KeyboardEventArgs(uMsg, wParam, lParam));
        }

        public static void CreateControlFile()
        {
            try
            {
                if (File.Exists(RazerControlFile))
                    StaticLog.Warn("CreateControlFile: File already exists");
                else
                {
                    File.Create(RazerControlFile).Close(); 
                    StaticLog.Info("CreateControlFile: Success!");
                }
            }
            catch (IOException ex)
            {
                StaticLog.ErrorFormat("CreateControlFile: [IOException] Failed to create control file: {0}", ex.Message);
            }
        }

        public static void DeleteControlFile()
        {
            try
            {
                if (File.Exists(RazerControlFile))
                {
                    File.Delete(RazerControlFile);
                    StaticLog.Info("DeleteControlFile: Success!");
                }
                else
                    StaticLog.Warn("DeleteControlFile: File does not exist");
            }
            catch (IOException ex)
            {
                StaticLog.ErrorFormat("DeleteControlFile: [IOException] Failed to delete control file: {0}", ex.Message);
            }
        }

        public static void Stop(bool cleanup = true)
        {
            StaticLog.Info("RazerManager is stopping! Calling RzSBStop...");
            RazerAPI.RzSBStop();
            if (cleanup)
                DeleteControlFile();
            StaticLog.Info("RazerManager has stopped.");
        }

        internal static void NativeCallFailure(string func, HRESULT result)
        {
            StaticLog.FatalFormat("Call to RazerAPI native function {0} FAILED with error: {1}", func, result.ToString());
            StaticLog.Debug("Throwing exception...");
            throw new RazerNativeException(result);
        }

        public Touchpad GetTouchpad()
        {
            return _touchpad;
        }

        public DynamicKey GetDynamicKey(RazerAPI.DynamicKeyType keyType)
        {
            return _dynamicKeys[(int) keyType - 1];
        }

        public DynamicKey EnableDynamicKey(RazerAPI.DynamicKeyType keyType, DynamicKeyPressedEventHandler callback, string upImage, string downImage = null, bool replace = false)
        {
            var index = (int) keyType - 1;
            if (_dynamicKeys[index] != null && !replace)
            {
                _log.Info("Dynamic key already enabled and replace is false.");
                return _dynamicKeys[index];
            }

            _log.Debug("Resetting dynamic key (DisableDynamicKey)");
            DisableDynamicKey(keyType);
            try
            {
                _log.Debug("Creating new DynamicKey object");
                var dk = new DynamicKey(keyType, upImage, downImage, callback);
                _dynamicKeys[index] = dk;
            }
            catch (RazerNativeException ex)
            {
                _log.ErrorFormat("Failed to enable dynamic key {0}: {1}", keyType, ex.Hresult);
                return null;
            }

            return _dynamicKeys[index];
        }

        public void DisableDynamicKey(RazerAPI.DynamicKeyType keyType)
        {
            var index = (int) keyType - 1;
            var dk = _dynamicKeys[index];
            if (dk != null)
                dk.Disable();
            _dynamicKeys[index] = null;
        }

        private HRESULT HandleAppEvent(RazerAPI.AppEventType type, uint appMode, uint processId)
        {
            var hResult = HRESULT.RZSB_OK;
            if (type == RazerAPI.AppEventType.Invalid || type == RazerAPI.AppEventType.None)
            {
                _log.DebugFormat("Unsupported AppEventType: {0}", type);
                return hResult;
            }

            OnAppEvent(type, (RazerAPI.AppEventMode) appMode, processId);

            return hResult;
        }

        private HRESULT HandleDynamicKeyEvent(RazerAPI.DynamicKeyType keyType, RazerAPI.DynamicKeyState state)
        {
            var result = HRESULT.RZSB_OK;

            _log.Debug("Raising DynamicKeyEvent event");
            OnDynamicKeyEvent(keyType, state);

            var index = (int) keyType - 1;
            var dk = _dynamicKeys[index];
            if (dk == null)
            {
                _log.Debug("Key has not been registered by app");
                return result;
            }

            _log.Debug("Updating key state");
            // UpdateState will check if it's a valid press and call any event subscribers
            dk.UpdateState(state);

            return result;
        }

        private HRESULT HandleKeyboardRawEvent(uint uMsg, UIntPtr wParam, IntPtr lParam)
        {
            var result = HRESULT.RZSB_OK;

            _log.Debug("Raising HandleKeyboardRawEvent event");
            OnKeyboardRawEvent(uMsg, wParam, lParam);

            return result;
        }
        private HRESULT HandleKeyboardEvent(uint uMsg, UIntPtr wParam, IntPtr lParam)
        {
            var result = HRESULT.RZSB_OK;

            _log.Debug("Raising HandleKeyboardEvent event");
            OnKeyboardEvent(uMsg,wParam,lParam);

            return result;
        }
    }
}
