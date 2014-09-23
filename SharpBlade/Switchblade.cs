// ---------------------------------------------------------------------------------------
// <copyright file="Switchblade.cs" company="SharpBlade">
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
    using System.Linq;
    using System.Windows.Forms;

    using SharpBlade.Annotations;
    using SharpBlade.Events;
    using SharpBlade.Integration;
    using SharpBlade.Logging;
    using SharpBlade.Native;
    using SharpBlade.Native.WinAPI;
    using SharpBlade.Razer;

    /// <summary>
    /// Manages everything related to Razer and its devices.
    /// </summary>
    public sealed class Switchblade : ISwitchblade
    {
        /// <summary>
        /// Array of 3-tuples to pair Virtual Keys (1st element) with their ModifierKeys counterpart (3rd element),
        /// the second element is used when checking for VK status (pressed or toggled, VK specific).
        /// </summary>
        private static readonly Tuple<VirtualKey, int, ModifierKeys>[] KeyMapping =
        {
            Tuple.Create(VirtualKey.SHIFT, User32.KEY_PRESSED, ModifierKeys.Shift),
            Tuple.Create(VirtualKey.CONTROL, User32.KEY_PRESSED, ModifierKeys.Control),
            Tuple.Create(VirtualKey.MENU, User32.KEY_PRESSED, ModifierKeys.Alt),
            Tuple.Create(VirtualKey.CAPITAL, User32.KEY_TOGGLED, ModifierKeys.CapsLock)
        };

        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

        /// <summary>
        /// App event callback that is used as parameter in <see cref="NativeMethods.RzSBAppEventSetCallback" />.
        /// </summary>
        private static AppEventCallback _appEventCallback;

        /// <summary>
        /// Switchblade instance for singleton.
        /// </summary>
        private static Switchblade _instance;

        /// <summary>
        /// Keyboard callback that is used as parameter in <see cref="NativeMethods.RzSBKeyboardCaptureSetCallback" />.
        /// </summary>
        private static KeyboardCallback _keyboardCallback;

        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        /// <summary>
        /// Private reference to the <see cref="DisplayStateFile" /> object that
        /// manages the <c>rzdisplaystate</c> file.
        /// </summary>
        private readonly DisplayStateFile _displayStateFile;

        /// <summary>
        /// Private reference to the <see cref="DynamicKeys" /> object that
        /// manages the dynamic keys.
        /// </summary>
        private readonly DynamicKeys _dynamicKeys;

        /// <summary>
        /// Log object for the <see cref="Switchblade" />.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Private reference to the <see cref="Touchpad" /> class that
        /// handles touchpad-related operations.
        /// </summary>
        private readonly Touchpad _touchpad;

        /// <summary>
        /// Indicates whether the <see cref="Switchblade" /> has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Holds reference to the <see cref="KeyboardControl" /> instance.
        /// </summary>
        private KeyboardControl _keyboardControl;

        /// <summary>
        /// Prevents a default instance of the <see cref="Switchblade" /> class from being created.
        /// </summary>
        /// <remarks>
        /// The constructor will disable all OS gestures, to enable them, use the Touchpad property
        /// and call the EnableOSGesture() method to enable gestures wanted.
        /// </remarks>
        private Switchblade()
        {
            _log = LogManager.GetLogger(this);

            _log.Info("Switchblade is initializing");

            _log.Debug("Calling RzSBStart()");

            var result = NativeMethods.RzSBStart();
            if (HRESULT.RZSB_FAILED(result))
            {
                // Try one more time
                result = NativeMethods.RzSBStart();
                if (HRESULT.RZSB_FAILED(result))
                    throw new NativeCallException("RzSBStart", result);
            }

            _log.Debug("Querying SDK for capabilities struct...");

            Capabilities caps;
            result = NativeMethods.RzSBQueryCapabilities(out caps);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBQueryCapabilities", result);

            Capabilities = caps;

            _log.InfoFormat(
                "SDK start successful! Working against SDK ver {0} ({1})",
                Capabilities.Version,
                Capabilities.BEVersion);
            _log.InfoFormat(
                "Number of surfaces: {0}, number of DKs: {1}",
                Capabilities.SurfaceCount,
                Capabilities.DynamicKeyCount);

#if DEBUG
            _log.DebugFormat(
                "HW type: {0}, DK size: {1}x{2}, DK arr: X={3} Y={4}",
                Capabilities.HardwareType,
                Capabilities.DynamicKeySize.X,
                Capabilities.DynamicKeySize.Y,
                Capabilities.DynamicKeyArrangement.X,
                Capabilities.DynamicKeyArrangement.Y);

            for (var i = 0; i < Capabilities.SurfaceCount; i++)
            {
                var pf = Capabilities.Pixelformat[i];
                var sg = Capabilities.Surfacegeometry[i];
                _log.DebugFormat("Surface #{0}: PixelFormat={1}, SurfaceGeometry: X={2}, Y={3}", i, pf, sg.X, sg.Y);
            }
#endif

            _log.Debug("Creating new app event callback");
            _appEventCallback = HandleAppEvent;
            _log.Debug("Calling RzSBAppEventSetCallback");
            result = NativeMethods.RzSBAppEventSetCallback(_appEventCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBAppEventSetCallback", result);

            _log.Info("Setting up touchpad");
            _touchpad = SharpBlade.Touchpad.Instance;

            Touchpad.DisableOSGesture(GestureTypes.All);

            _log.Info("Setting up keyboard");

            // Keyboard capture callback
            _log.Debug("Creating keyboard callback");
            _keyboardCallback = HandleKeyboardEvent;
            _log.Debug("Calling RzSBDynamicKeySetCallback");
            result = NativeMethods.RzSBKeyboardCaptureSetCallback(_keyboardCallback);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBKeyboardCaptureSetCallback", result);

            _log.Info("Setting up dynamic keys");
            _dynamicKeys = SharpBlade.DynamicKeys.Instance;

            _log.Debug("Initializing the RzDisplayState file manager");
            _displayStateFile = new DisplayStateFile();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Switchblade" /> class.
        /// Allows an object to try to free resources and perform other
        /// cleanup operations before it is reclaimed by garbage collection.
        /// <see cref="Switchblade" /> will call <see cref="NativeMethods.RzSBStop" />
        /// if not disposed at the point of finalization.
        /// </summary>
        ~Switchblade()
        {
            Dispose(false);
        }

        /// <summary>
        /// Raised when the app is activated (put in foreground).
        /// </summary>
        public event EventHandler<AppEventEventArgs> Activated;

        /// <summary>
        /// Raised when an app event occurs.
        /// </summary>
        public event EventHandler<AppEventEventArgs> AppEvent;

        /// <summary>
        /// Raised when the CLOSE app event occurs.
        /// </summary>
        public event EventHandler<AppEventEventArgs> Close;

        /// <summary>
        /// Raised when the app is deactivated (put in background).
        /// </summary>
        public event EventHandler<AppEventEventArgs> Deactivated;

        /// <summary>
        /// Raised when the EXIT app event occurs.
        /// </summary>
        public event EventHandler<AppEventEventArgs> Exit;

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
        /// Raised when a keyboard raw event occurs.
        /// </summary>
        public event EventHandler<KeyboardRawEventArgs> KeyboardRawEvent;

        /// <summary>
        /// Gets singleton instance of Switchblade.
        /// </summary>
        [PublicAPI]
        public static ISwitchblade Instance
        {
            get
            {
                return _instance ?? (_instance = new Switchblade());
            }
        }

        /// <summary>
        /// Gets a structure describing the SDK and hardware capabilities of the system.
        /// </summary>
        public Capabilities Capabilities { get; private set; }

        /// <summary>
        /// Gets the dynamic key manager.
        /// </summary>
        public IDynamicKeys DynamicKeys
        {
            get { return _dynamicKeys; }
        }

        /// <summary>
        /// Gets the <see cref="IDisplayStateFile" /> instance associated with this
        /// <see cref="Switchblade" /> object.
        /// </summary>
        public IDisplayStateFile DisplayStateFile
        {
            get { return _displayStateFile; }
        }

        /// <summary>
        /// Gets a value indicating whether keyboard capture is enabled or not.
        /// </summary>
        public bool KeyboardCapture { get; private set; }

        /// <summary>
        /// Gets the touchpad on the keyboard.
        /// </summary>
        public ITouchpad Touchpad
        {
            get { return _touchpad; }
        }

        /// <summary>
        /// Disposes of this <see cref="Switchblade" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Enables or disables keyboard capture.
        /// </summary>
        /// <param name="enabled">Whether or not to enable keyboard capture.</param>
        public void SetKeyboardCapture(bool enabled)
        {
            if (enabled == KeyboardCapture)
                return;

            var hresult = NativeMethods.RzSBCaptureKeyboard(enabled);
            if (HRESULT.RZSB_FAILED(hresult))
                throw new NativeCallException("RzSBCaptureKeyboard", hresult);

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
        public void StartKeyboardCapture(Control control, bool releaseOnEnter = true)
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
        public void StartKeyboardCapture(System.Windows.Controls.Control control, bool releaseOnEnter = true)
        {
            SetKeyboardCapture(true);
            _keyboardControl = new KeyboardControl(control, releaseOnEnter);
        }

        /// <summary>
        /// Stops all Razer interaction.
        /// </summary>
        public void Stop()
        {
            _log.Info("Switchblade is stopping! Calling RzSBStop...");
            NativeMethods.RzSBStop();
            _log.Info("Switchblade has stopped.");
        }

        /// <summary>
        /// Disposes of this <see cref="Switchblade" />.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from parameter-less <see cref="Dispose()" />, false otherwise.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _log.Debug("Switchblade is disposing...");

            if (disposing)
            {
                if (!_touchpad.Disposed)
                    _touchpad.Dispose();

                if (!_displayStateFile.Disposed)
                    _displayStateFile.Dispose();
            }

            _log.Debug("Dispose: Calling Stop()");
            Stop();

            _disposed = true;
        }

        /// <summary>
        /// Handles app event sent from Razer SDK.
        /// </summary>
        /// <param name="type">App event type.</param>
        /// <param name="firstParam">The first DWORD parameter.</param>
        /// <param name="secondParam">The second DWORD parameter.</param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        private HRESULT HandleAppEvent(AppEventType type, uint firstParam, uint secondParam)
        {
            const int Result = HRESULT.RZSB_OK;
            if (type == AppEventType.Invalid || type == AppEventType.None)
            {
                _log.DebugFormat("Unsupported AppEventType: {0}", type);
                return Result;
            }

            OnAppEvent(type, firstParam, secondParam);

            switch (type)
            {
                case AppEventType.Activated:
                    OnActivated(firstParam, secondParam);
                    break;
                case AppEventType.Close:
                    OnClose(firstParam, secondParam);
                    break;
                case AppEventType.Deactivated:
                    OnDectivated(firstParam, secondParam);
                    break;
                case AppEventType.Exit:
                    OnExit(firstParam, secondParam);
                    break;
            }

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
                var key = (VirtualKey)(uint)data;

                // Workaround to get the modifier keys
                // Item1 = VK, Item2 = mask, Item3 = ModifierKey
                var modifierKeys =
                    KeyMapping.Where(
                        mapping => (User32.NativeMethods.GetKeyState((int)mapping.Item1) & mapping.Item2) != 0)
                              .Aggregate(ModifierKeys.None, (current, mapping) => current | mapping.Item3);

                if (msgType == User32.MessageType.KEYDOWN)
                {
                    OnKeyboardKeyDown(key, modifierKeys);
                    if (_keyboardControl != null)
                        _keyboardControl.SendKeyDown(key);
                }
                else
                {
                    OnKeyboardKeyUp(key, modifierKeys);
                    if (_keyboardControl != null)
                    {
                        _keyboardControl.SendKeyUp(key);
                        if (key == VirtualKey.RETURN && _keyboardControl.ReleaseOnEnter)
                            SetKeyboardCapture(false);
                    }
                }
            }

            return HRESULT.RZSB_OK;
        }

        /// <summary>
        /// Raises the activated app event to subscribers.
        /// </summary>
        /// <param name="firstParam">The first DWORD parameter.</param>
        /// <param name="secondParam">The second DWORD parameter.</param>
        private void OnActivated(uint firstParam, uint secondParam)
        {
            var func = Activated;
            if (func != null)
                func(this, new AppEventEventArgs(AppEventType.Activated, firstParam, secondParam));
        }

        /// <summary>
        /// Raises app event to subscribers.
        /// </summary>
        /// <param name="type">App event type.</param>
        /// <param name="firstParam">The first DWORD parameter.</param>
        /// <param name="secondParam">The second DWORD parameter.</param>
        private void OnAppEvent(AppEventType type, uint firstParam, uint secondParam)
        {
            var func = AppEvent;
            if (func != null)
                func(this, new AppEventEventArgs(type, firstParam, secondParam));
        }

        /// <summary>
        /// Raises the close app event to subscribers.
        /// </summary>
        /// <param name="firstParam">The first DWORD parameter.</param>
        /// <param name="secondParam">The second DWORD parameter.</param>
        private void OnClose(uint firstParam, uint secondParam)
        {
            var func = Close;
            if (func != null)
                func(this, new AppEventEventArgs(AppEventType.Close, firstParam, secondParam));
        }

        /// <summary>
        /// Raises the deactivated app event to subscribers.
        /// </summary>
        /// <param name="firstParam">The first DWORD parameter.</param>
        /// <param name="secondParam">The second DWORD parameter.</param>
        private void OnDectivated(uint firstParam, uint secondParam)
        {
            var func = Exit;
            if (func != null)
                func(this, new AppEventEventArgs(AppEventType.Deactivated, firstParam, secondParam));
        }

        /// <summary>
        /// Raises the exit app event to subscribers.
        /// </summary>
        /// <param name="firstParam">The first DWORD parameter.</param>
        /// <param name="secondParam">The second DWORD parameter.</param>
        private void OnExit(uint firstParam, uint secondParam)
        {
            var func = Exit;
            if (func != null)
                func(this, new AppEventEventArgs(AppEventType.Exit, firstParam, secondParam));
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
        private void OnKeyboardKeyDown(VirtualKey key, ModifierKeys modifiers)
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
        private void OnKeyboardKeyUp(VirtualKey key, ModifierKeys modifiers)
        {
            var func = KeyboardKeyUp;
            if (func != null)
                func(this, new KeyboardKeyEventArgs(key, modifiers));
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
    }
}
