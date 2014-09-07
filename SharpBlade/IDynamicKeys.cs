// ---------------------------------------------------------------------------------------
// <copyright file="IDynamicKeys.cs" company="SharpBlade">
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

using System;

using SharpBlade.Events;
using SharpBlade.Razer;

namespace SharpBlade
{
    /// <summary>
    /// <see cref="DynamicKeys" /> interface.
    /// </summary>
    public interface IDynamicKeys
    {
        /// <summary>
        /// Raised when a dynamic key event occurs.
        /// </summary>
        event EventHandler<DynamicKeyEventArgs> DynamicKeyEvent;

        /// <summary>
        /// Gets a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The type of key to get.</param>
        /// <returns>
        /// An instance of <see cref="IDynamicKey" /> if the requested
        /// type has been enabled, otherwise <c>null</c>.
        /// </returns>
        IDynamicKey this[DynamicKeyType keyType] { get; }

        /// <summary>
        /// Disables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to disable.</param>
        void DisableDynamicKey(DynamicKeyType keyType);

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        IDynamicKey EnableDynamicKey(DynamicKeyType keyType, string image, bool replace = false);

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="callback">Callback called when this key is released.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        IDynamicKey EnableDynamicKey(
            DynamicKeyType keyType,
            EventHandler<DynamicKeyEventArgs> callback,
            string image,
            bool replace = false);

        /// <summary>
        /// Enables a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to enable.</param>
        /// <param name="image">Image to display on this key when in the UP state.</param>
        /// <param name="pressedImage">Image to display on this key when in the DOWN state.</param>
        /// <param name="replace">True to override this key's previous configuration
        /// if it has already been enabled, otherwise returns current key if already enabled.</param>
        /// <returns>The dynamic key that was enabled.</returns>
        IDynamicKey EnableDynamicKey(
            DynamicKeyType keyType,
            string image,
            string pressedImage = null,
            bool replace = false);

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
        IDynamicKey EnableDynamicKey(
            DynamicKeyType keyType,
            EventHandler<DynamicKeyEventArgs> callback,
            string image,
            string pressedImage = null,
            bool replace = false);

        /// <summary>
        /// Gets a specific dynamic key.
        /// </summary>
        /// <param name="keyType">The key type to get.</param>
        /// <returns><see cref="DynamicKey" /> object representing the specified key type.</returns>
        IDynamicKey GetDynamicKey(DynamicKeyType keyType);
    }
}