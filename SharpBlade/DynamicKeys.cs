// ---------------------------------------------------------------------------------------
// <copyright file="DynamicKeys.cs" company="SharpBlade">
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
    using System.Globalization;

    using SharpBlade.Events;
    using SharpBlade.Logging;
    using SharpBlade.Native;
    using SharpBlade.Razer;

    /// <summary>
    /// Manages the dynamic keys of a device.
    /// </summary>
    internal sealed class DynamicKeys : IDynamicKeys
    {
        /// <summary>
        /// Dynamic key callback that is used as parameter in <see cref="NativeMethods.RzSBDynamicKeySetCallback" />.
        /// </summary>
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static DynamicKeyCallback _callback;

        /// <summary>
        /// Instance of <see cref="DynamicKeys" /> for the singleton.
        /// </summary>
        private static DynamicKeys _instance;

        /// <summary>
        /// Contains all active and enabled dynamic key objects.
        /// </summary>
        private readonly DynamicKey[] _keys;

        /// <summary>
        /// Log object for the <see cref="DynamicKeys" />.
        /// </summary>
        private readonly log4net.ILog _log;

        /// <summary>
        /// Prevents a default instance of the <see cref="DynamicKeys" /> class from being created.
        /// </summary>
        private DynamicKeys()
        {
            _log = LogManager.GetLogger(this);

            _log.Info("Setting up dynamic keys");

            _log.Debug("Creating dynamic key callback");
            _callback = HandleDynamicKeyEvent;
            _log.Debug("Calling RzSBDynamicKeySetCallback");
            var result = NativeMethods.RzSBDynamicKeySetCallback(_callback);
            if (HRESULT.RZSB_FAILED(result))
                throw new NativeCallException("RzSBDynamicKeySetCallback", result);

            _log.Debug("Initializing dynamic key array");

            _keys = new DynamicKey[Constants.DynamicKeysCount];
        }

        /// <summary>
        /// Raised when a dynamic key event occurs.
        /// </summary>
        public event EventHandler<DynamicKeyEventArgs> DynamicKeyEvent;

        /// <summary>
        /// Gets singleton instance of <see cref="DynamicKeys" />.
        /// </summary>
        internal static DynamicKeys Instance
        {
            get
            {
                return _instance ?? (_instance = new DynamicKeys());
            }
        }

        /// <summary>
        /// Gets a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The type of key to get.</param>
        /// <returns>
        /// An instance of <see cref="IDynamicKey" />.
        /// </returns>
        public IDynamicKey this[DynamicKeyType keyType]
        {
            get
            {
                var index = (int)keyType - 1;
                if (index >= _keys.Length)
                    throw new ArgumentOutOfRangeException("keyType");
                return _keys[index] ?? Enable(keyType);
            }
        }

        /// <summary>
        /// Disables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to disable.</param>
        public void Disable(DynamicKeyType keyType)
        {
            var index = (int)keyType - 1;
            var dk = _keys[index];

            if (dk != null && !dk.Disposed)
            {
                dk.Enabled = false;
                dk.Dispose();
            }

            _keys[index] = null;
        }

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        public IDynamicKey Enable(DynamicKeyType keyType, string image, bool replace)
        {
            return Enable(keyType, image, null, replace);
        }

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="callback">Callback called when this key is released.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        public IDynamicKey Enable(
            DynamicKeyType keyType,
            EventHandler<DynamicKeyEventArgs> callback,
            string image,
            bool replace)
        {
            return Enable(keyType, callback, image, null, replace);
        }

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="pressedImage">Image to display on this key when in the DOWN state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        public IDynamicKey Enable(
            DynamicKeyType keyType,
            string image,
            string pressedImage = null,
            bool replace = false)
        {
            return Enable(keyType, null, image, pressedImage, replace);
        }

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="callback">Callback called when this key is released.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="pressedImage">Image to display on this key when in the DOWN state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        public IDynamicKey Enable(
            DynamicKeyType keyType,
            EventHandler<DynamicKeyEventArgs> callback = null,
            string image = null,
            string pressedImage = null,
            bool replace = false)
        {
            var index = (int)keyType - 1;
            if (_keys[index] != null && !replace)
            {
                _log.Info("Dynamic key already enabled and replace is false.");
                return _keys[index];
            }

            _log.Debug("Resetting dynamic key (Disable)");
            Disable(keyType);
            try
            {
                _log.Debug("Creating new DynamicKey object");
                var dk = new DynamicKey(keyType, image, pressedImage, callback);
                _keys[index] = dk;
            }
            catch (NativeCallException ex)
            {
                _log.ErrorFormat("Failed to enable dynamic key {0}: {1}", keyType, ex.HResult);
                throw new DynamicKeyException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to enable dynamic key {0} due to a native call exception.",
                        keyType),
                    ex);
            }

            return _keys[index];
        }

        /// <summary>
        /// Handles dynamic key events sent from Razer SDK.
        /// </summary>
        /// <param name="keyType">Dynamic key type.</param>
        /// <param name="state">New state of the dynamic key.</param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        private HRESULT HandleDynamicKeyEvent(DynamicKeyType keyType, DynamicKeyState state)
        {
            const int Result = HRESULT.RZSB_OK;

            _log.Debug("Raising DynamicKeyEvent event");
            OnDynamicKeyEvent(keyType, state);

            var index = (int)keyType - 1;
            var dk = _keys[index];
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
        /// Raises dynamic key event to subscribers.
        /// </summary>
        /// <param name="keyType">The dynamic key affected.</param>
        /// <param name="state">New key state.</param>
        private void OnDynamicKeyEvent(DynamicKeyType keyType, DynamicKeyState state)
        {
            var func = DynamicKeyEvent;
            if (func != null)
                func(this, new DynamicKeyEventArgs(keyType, state));
        }
    }
}
