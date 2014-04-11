//---------------------------------------------------------------------------------------
// <copyright file="RazerManager.cs" company="SharpBlade">
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
using System.Linq;
using System.Windows.Forms;

using Sharparam.SharpBlade.Integration;
using Sharparam.SharpBlade.Logging;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Native.WinAPI;
using Sharparam.SharpBlade.Razer.Events;
using Sharparam.SharpBlade.Razer.Exceptions;

namespace Sharparam.SharpBlade.Razer
{
    /// <summary>
    /// Manages everything related to Razer and its devices.
    /// </summary>
    public class RazerManager : IDisposable
    {
        /// <summary>
        /// Array of 3-tuples to pair Virtual Keys (1st element) with their ModifierKeys counterpart (3rd element),
        /// the second element is used when checking for VK status (pressed or toggled, VK specific).
        /// </summary>
        private static readonly Tuple<User32.VirtualKey, int, ModifierKeys>[] KeyMapping =
        {
            Tuple.Create(User32.VirtualKey.SHIFT, User32.KEY_PRESSED, ModifierKeys.Shift),
            Tuple.Create(User32.VirtualKey.CONTROL, User32.KEY_PRESSED, ModifierKeys.Control),
            Tuple.Create(User32.VirtualKey.MENU, User32.KEY_PRESSED, ModifierKeys.Alt),
            Tuple.Create(User32.VirtualKey.CAPITAL, User32.KEY_TOGGLED, ModifierKeys.CapsLock)
        };

        /// <summary>
        /// RazerManager instance for singleton.
        /// </summary>
        private static RazerManager _instance;

        /// <summary>
        /// App event callback that is used as parameter in <see cref="RazerAPI.RzSBAppEventSetCallback" />.
        /// </summary>
        private static RazerAPI.AppEventCallbackDelegate _appEventCallback;

        /// <summary>
        /// Dynamic key callback that is used as parameter in <see cref="RazerAPI.RzSBDynamicKeySetCallback" />.
        /// </summary>
        private static RazerAPI.DynamicKeyCallbackFunctionDelegate _dynamicKeyCallback;

        /// <summary>
        /// Keyboard callback that is used as parameter in <see cref="RazerAPI.RzSBKeyboardCaptureSetCallback" />.
        /// </summary>
        private static RazerAPI.KeyboardCallbackFunctionDelegate _keyboardCallback;

        /// <summary>
        /// Log object for the <see cref="RazerManager" />.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Contains all active and enabled dynamic key objects.
        /// </summary>
        private readonly DynamicKey[] _dynamicKeys;

        /// <summary>
        /// Indicates whether the <see cref="RazerManager" /> has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Holds reference to the <see cref="KeyboardControl" /> instance.
        /// </summary>
        private KeyboardControl _keyboardControl;

        /// <summary>
        /// Prevents a default instance of the <see cref="RazerManager" /> class from being created.
        /// </summary>
        /// <remarks>
        /// The constructor will disable all OS gestures, to enable them, use the Touchpad property
        /// and call the EnableOSGesture() method to enable gestures wanted.
        /// </remarks>
        private RazerManager()
        {
            _log = LogManager.GetLogger(this);

            _log.Info("RazerManager is initializing");

            _log.Debug("Setting default values for touchpad/dynamickey blank/disabled images");
            BlankTouchpadImagePath = Constants.BlankTouchpadImage;
            DisabledDynamicKeyImagePath = Constants.DisabledDynamicKeyImage;

            _log.Debug("Calling RzSBStart()");

            var result = RazerAPI.RzSBStart();
            if (HRESULT.RZSB_FAILED(result))
            {
                // Try one more time
                result = RazerAPI.RzSBStart();
                if (HRESULT.RZSB_FAILED(result))
                    throw new RazerNativeException("RzSBStart", result);
            }

            _log.Info("Setting up dynamic keys");

            _log.Debug("Creating new app event callback");
            _appEventCallback = HandleAppEvent;
            _log.Debug("Calling RzSBAppEventSetCallback");
            result = RazerAPI.RzSBAppEventSetCallback(_appEventCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBAppEventSetCallback", result);

            _log.Info("Setting up touchpad");
            Touchpad = Touchpad.Instance;

            Touchpad.DisableOSGesture(RazerAPI.GestureType.All);

            _log.Debug("Creating dynamic key callback");
            _dynamicKeyCallback = HandleDynamicKeyEvent;
            _log.Debug("Calling RzSBDynamicKeySetCallback");
            result = RazerAPI.RzSBDynamicKeySetCallback(_dynamicKeyCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBDynamicKeySetCallback", result);

            _log.Info("Setting up keyboard");

            // Keyboard capture callback
            _log.Debug("Creating keyboard callback");
            _keyboardCallback = HandleKeyboardEvent;
            _log.Debug("Calling RzSBDynamicKeySetCallback");
            result = RazerAPI.RzSBKeyboardCaptureSetCallback(_keyboardCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new RazerNativeException("RzSBKeyboardCaptureSetCallback", result);

            _log.Debug("Initializing dynamic key arrays");

            _dynamicKeys = new DynamicKey[RazerAPI.DynamicKeysCount];
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RazerManager" /> class.
        /// Allows an object to try to free resources and perform other
        /// cleanup operations before it is reclaimed by garbage collection.
        /// <see cref="RazerManager" /> will call <see cref="RazerAPI.RzSBStop" />
        /// if not disposed at the point of finalization.
        /// </summary>
        ~RazerManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Raised when an app event occurs.
        /// </summary>
        public event EventHandler<AppEventEventArgs> AppEvent;

        /// <summary>
        /// Raised when a dynamic key event occurs.
        /// </summary>
        public event EventHandler<DynamicKeyEventArgs> DynamicKeyEvent;

        /// <summary>
        /// Raised when a keyboard raw event occurs.
        /// </summary>
        public event EventHandler<KeyboardRawEventArgs> KeyboardRawEvent;

        /// <summary>
        /// Raised when a keyboard char event occurs.
        /// </summary>
        public event EventHandler<KeyboardCharEventArgs> KeyboardCharTyped;

        /// <summary>
        /// Raised when a keyboard key is pressed.
        /// </summary>
        public event EventHandler<KeyboardKeyEventArgs> KeyboardKeyDown;

        /// <summary>
        /// Raised when a keyboard key is released.
        /// </summary>
        public event EventHandler<KeyboardKeyEventArgs> KeyboardKeyUp;

        /// <summary>
        /// Gets singleton instance of RazerManager.
        /// </summary>
        public static RazerManager Instance
        {
            get { return _instance ?? (_instance = new RazerManager()); }
        }

        /// <summary>
        /// Gets or sets the image shown on Touchpad when it's blank or
        /// after <see cref="Razer.Touchpad.ClearImage" /> or <see cref="Razer.Touchpad.ClearAll" />
        /// have been called.
        /// </summary>
        /// <remarks>Defaults to <see cref="Constants.BlankTouchpadImage" /></remarks>
        public string BlankTouchpadImagePath { get; set; }

        /// <summary>
        /// Gets or sets the image shown on dynamic keys when disabled.
        /// </summary>
        /// <remarks>Defaults to <see cref="Constants.DisabledDynamicKeyImage" /></remarks>
        public string DisabledDynamicKeyImagePath { get; set; }

        /// <summary>
        /// Gets the touchpad on the keyboard.
        /// </summary>
        public Touchpad Touchpad { get; private set; }

        /// <summary>
        /// Gets a value indicating whether keyboard capture is enabled or not.
        /// </summary>
        public bool KeyboardCapture { get; private set; }

        /// <summary>
        /// Disposes of this <see cref="RazerManager" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops all Razer interaction.
        /// </summary>
        public void Stop()
        {
            _log.Info("RazerManager is stopping! Calling RzSBStop...");
            RazerAPI.RzSBStop();
            _log.Info("RazerManager has stopped.");
        }

        /// <summary>
        /// Gets a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to get.</param>
        /// <returns><see cref="DynamicKey" /> object representing the specified key type.</returns>
        public DynamicKey GetDynamicKey(RazerAPI.DynamicKeyType keyType)
        {
            return _dynamicKeys[(int)keyType - 1];
        }

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="presssedImage">Image to display on this key when in the DOWN state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        public DynamicKey EnableDynamicKey(RazerAPI.DynamicKeyType keyType, string image, string presssedImage = null, bool replace = false)
        {
            return EnableDynamicKey(keyType, null, image, presssedImage, replace);
        }

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="callback">Callback called when this key is pressed.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="pressedImage">Image to display on this key when in the DOWN state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        public DynamicKey EnableDynamicKey(RazerAPI.DynamicKeyType keyType, EventHandler callback, string image, string pressedImage = null, bool replace = false)
        {
            var index = (int)keyType - 1;
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
                var dk = new DynamicKey(keyType, image, pressedImage, callback);
                _dynamicKeys[index] = dk;
            }
            catch (RazerNativeException ex)
            {
                _log.ErrorFormat("Failed to enable dynamic key {0}: {1}", keyType, ex.Hresult);
                throw new RazerDynamicKeyException(string.Format("Failed to enable dynamic key {0} due to a native call exception.", keyType), ex);
            }

            return _dynamicKeys[index];
        }

        /// <summary>
        /// Disables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to disable.</param>
        public void DisableDynamicKey(RazerAPI.DynamicKeyType keyType)
        {
            var index = (int)keyType - 1;
            var dk = _dynamicKeys[index];
            if (dk != null)
                dk.Disable();
            _dynamicKeys[index] = null;
        }

        /// <summary>
        /// Enables or disables keyboard capture.
        /// </summary>
        /// <param name="enabled">Whether or not to enable keyboard capture.</param>
        public void SetKeyboardCapture(bool enabled)
        {
            if (enabled == KeyboardCapture)
                return;

            var hresult = RazerAPI.RzSBCaptureKeyboard(enabled);
            if (HRESULT.RZSB_FAILED(hresult))
                throw new RazerNativeException("RzSBCaptureKeyboard", hresult);

            if (!enabled)
                _keyboardControl = null;

            KeyboardCapture = enabled;
        }

        /// <summary>
        /// Starts forwarding keyboard events to the specified WinForms control.
        /// </summary>
        /// <param name="control">THe control to forward input to.</param>
        /// <param name="releaseOnEnter">If true, keyboard capture will cease when the enter key is pressed,
        /// otherwise, <see cref="SetKeyboardCapture" /> has to be called explicitly with false as the argument.</param>
        public void StartWinFormsControlKeyboardCapture(Control control, bool releaseOnEnter = true)
        {
            SetKeyboardCapture(true);
            _keyboardControl = new KeyboardControl(control, releaseOnEnter);
        }

        /// <summary>
        /// Starts forwarding keyboard events to the specified WPF control.
        /// </summary>
        /// <param name="control">The control to forward input to.</param>
        /// <param name="releaseOnEnter">If true, keyboard capture will cease when the enter key is pressed,
        /// otherwise, <see cref="SetKeyboardCapture" /> has to be called explicitly with false as the argument.</param>
        public void StartWPFControlKeyboardCapture(System.Windows.Controls.Control control, bool releaseOnEnter = true)
        {
            SetKeyboardCapture(true);
            _keyboardControl = new KeyboardControl(control, releaseOnEnter);
        }

        /// <summary>
        /// Disposes of this <see cref="RazerManager" />.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from parameter-less <see cref="Dispose()" />, false otherwise.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _log.Debug("RazerManager is disposing...");

            if (disposing)
            {
                if (Touchpad != null)
                {
                    Touchpad.Dispose();
                    Touchpad = null;
                }
            }

            _log.Debug("Dispose: Calling Stop()");
            Stop();

            _disposed = true;
        }

        /// <summary>
        /// Raises app event to subscribers.
        /// </summary>
        /// <param name="type">App event type.</param>
        /// <param name="mode">Mode associated with the app event.</param>
        /// <param name="processId">The process ID.</param>
        private void OnAppEvent(RazerAPI.AppEventType type, RazerAPI.AppEventMode mode, uint processId)
        {
            var func = AppEvent;
            if (func != null)
                func(this, new AppEventEventArgs(type, mode, processId));
        }

        /// <summary>
        /// Raises dynamic key event to subscribers.
        /// </summary>
        /// <param name="keyType">The dynamic key affected.</param>
        /// <param name="state">New key state.</param>
        private void OnDynamicKeyEvent(RazerAPI.DynamicKeyType keyType, RazerAPI.DynamicKeyState state)
        {
            var func = DynamicKeyEvent;
            if (func != null)
                func(this, new DynamicKeyEventArgs(keyType, state));
        }

        /// <summary>
        /// Raises raw keyboard event to subscribers.
        /// </summary>
        /// <param name="type">Key event type.</param>
        /// <param name="data">Data for event.</param>
        /// <param name="modifiers">Active modifiers.</param>
        private void OnKeyboardRawEvent(uint type, UIntPtr data, IntPtr modifiers)
        {
            var func = KeyboardRawEvent;
            if (func != null)
                func(this, new KeyboardRawEventArgs(type, data, modifiers));
        }

        /// <summary>
        /// Raises keyboard char typed event to subscribers.
        /// </summary>
        /// <param name="character">The character that was typed.</param>
        private void OnKeyboardCharTyped(char character)
        {
            var func = KeyboardCharTyped;
            if (func != null)
                func(this, new KeyboardCharEventArgs(character));
        }

        /// <summary>
        /// Raises key down event to subscribers.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        /// <param name="modifiers">Active modifier keys.</param>
        private void OnKeyboardKeyDown(User32.VirtualKey key, ModifierKeys modifiers)
        {
            var func = KeyboardKeyDown;
            if (func != null)
                func(this, new KeyboardKeyEventArgs(key, modifiers));
        }

        /// <summary>
        /// Raises key up event to subscribers.
        /// </summary>
        /// <param name="key">Key that was released.</param>
        /// <param name="modifiers">Active modifier keys.</param>
        private void OnKeyboardKeyUp(User32.VirtualKey key, ModifierKeys modifiers)
        {
            var func = KeyboardKeyUp;
            if (func != null)
                func(this, new KeyboardKeyEventArgs(key, modifiers));
        }

        /// <summary>
        /// Handles app event sent from Razer SDK.
        /// </summary>
        /// <param name="type">App event type.</param>
        /// <param name="appMode">Mode associated with app event.</param>
        /// <param name="processId">Process ID for event.</param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        private HRESULT HandleAppEvent(RazerAPI.AppEventType type, uint appMode, uint processId)
        {
            const int Result = HRESULT.RZSB_OK;
            if (type == RazerAPI.AppEventType.Invalid || type == RazerAPI.AppEventType.None)
            {
                _log.DebugFormat("Unsupported AppEventType: {0}", type);
                return Result;
            }

            OnAppEvent(type, (RazerAPI.AppEventMode)appMode, processId);

            return Result;
        }

        /// <summary>
        /// Handles dynamic key events sent from Razer SDK.
        /// </summary>
        /// <param name="keyType">Dynamic key type.</param>
        /// <param name="state">New state of the dynamic key.</param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        private HRESULT HandleDynamicKeyEvent(RazerAPI.DynamicKeyType keyType, RazerAPI.DynamicKeyState state)
        {
            const int Result = HRESULT.RZSB_OK;

            _log.Debug("Raising DynamicKeyEvent event");
            OnDynamicKeyEvent(keyType, state);

            var index = (int)keyType - 1;
            var dk = _dynamicKeys[index];
            if (dk == null)
            {
                _log.Debug("Key has not been registered by app");
                return Result;
            }

            _log.Debug("Updating key state");

            // UpdateState will check if it's a valid press and call any event subscribers
            dk.UpdateState(state);

            return Result;
        }

        /// <summary>
        /// Callback hander for the keyboard event sent from Razer SDK.
        /// </summary>
        /// <param name="type"><c>type</c> parameter.</param>
        /// <param name="data"><c>data</c> parameter.</param>
        /// <param name="modifiers"><c>modifiers</c> parameter.</param>
        /// <returns>An instance of <see cref="HRESULT" /> indicating success or failure.</returns>
        private HRESULT HandleKeyboardEvent(uint type, UIntPtr data, IntPtr modifiers)
        {
            var msgType = (User32.MessageType)type;
            var asChar = (char)data;

            OnKeyboardRawEvent(type, data, modifiers);

            // We only want to send the char event if it's a char that can actually be typed
            // So it doesn't handle SHIFT and CONTROL as "characters"
            if (msgType == User32.MessageType.CHAR && !char.IsControl(asChar))
            {
                OnKeyboardCharTyped(asChar);
                if (_keyboardControl != null)
                    _keyboardControl.SendChar(asChar);
            }
            else if (msgType == User32.MessageType.KEYDOWN || msgType == User32.MessageType.KEYUP)
            {
                var key = (User32.VirtualKey)(uint)data;

                // Workaround to get the modifier keys
                // Item1 = VK, Item2 = mask, Item3 = ModifierKey
                var modifierKeys =
                    KeyMapping.Where(mapping => (User32.GetKeyState((int)mapping.Item1) & mapping.Item2) != 0)
                              .Aggregate(ModifierKeys.None, (current, mapping) => current | mapping.Item3);

                if (msgType == User32.MessageType.KEYDOWN)
                {
                    OnKeyboardKeyDown(key, modifierKeys);
                    if (_keyboardControl != null)
                        _keyboardControl.SendKeyDown(key);
                }
                else if (msgType == User32.MessageType.KEYUP)
                {
                    OnKeyboardKeyUp(key, modifierKeys);
                    if (_keyboardControl != null)
                    {
                        _keyboardControl.SendKeyUp(key);
                        if (key == User32.VirtualKey.RETURN && _keyboardControl.ReleaseOnEnter)
                            SetKeyboardCapture(false);
                    }
                }
            }

            return HRESULT.RZSB_OK;
        }
    }
}
