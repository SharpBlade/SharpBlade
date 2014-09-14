// ---------------------------------------------------------------------------------------
// <copyright file="IDynamicKey.cs" company="SharpBlade">
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

    using SharpBlade.Annotations;
    using SharpBlade.Events;
    using SharpBlade.Razer;

    /// <summary>
    /// <see cref="DynamicKey" /> interface.
    /// </summary>
    public interface IDynamicKey
    {
        /// <summary>
        /// Raised when the state of this key changes.
        /// </summary>
        [PublicAPI]
        event EventHandler<DynamicKeyEventArgs> Changed;

        /// <summary>
        /// Raised when this key is pressed.
        /// </summary>
        [PublicAPI]
        event EventHandler<DynamicKeyEventArgs> Pressed;

        /// <summary>
        /// Raised when this key is released.
        /// </summary>
        [PublicAPI]
        event EventHandler<DynamicKeyEventArgs> Released;

        /// <summary>
        /// Gets the <see cref="DynamicKeyType" /> of this key.
        /// </summary>
        [PublicAPI]
        DynamicKeyType KeyType { get; }

        /// <summary>
        /// Gets the previous state of this key.
        /// </summary>
        [PublicAPI]
        DynamicKeyState PreviousState { get; }

        /// <summary>
        /// Gets the current state of this key.
        /// </summary>
        [PublicAPI]
        DynamicKeyState State { get; }

        /// <summary>
        /// Gets a value indicating whether this object has been disposed.
        /// </summary>
        [PublicAPI]
        bool Disposed { get; }

        /// <summary>
        /// Gets a value indicating whether this dynamic key
        /// is currently enabled (propagating events).
        /// </summary>
        [PublicAPI]
        bool Enabled { get; }

        /// <summary>
        /// Disables this dynamic key (sets to blank image
        /// and stops event propagation).
        /// </summary>
        /// <remarks>
        /// Events for this dynamic key will not be propagated again
        /// until a call to <see cref="Enable" /> is made.
        /// </remarks>
        [PublicAPI]
        void Disable();

        /// <summary>
        /// Sets the images for each state of the dynamic key.
        /// </summary>
        /// <param name="image">Image for the UP state.</param>
        /// <param name="downImage">Image for the DOWN state.</param>
        [PublicAPI]
        void Draw(string image, string downImage);

        /// <summary>
        /// Sets the image for a specific state on this dynamic key.
        /// </summary>
        /// <param name="state">The state to set an image for.</param>
        /// <param name="image">The image file to set.</param>
        [PublicAPI]
        void Draw(DynamicKeyState state, string image);

        /// <summary>
        /// Sets the image for the DOWN state on this key.
        /// </summary>
        /// <param name="image">Image file to set for the state.</param>
        [PublicAPI]
        void DrawDown(string image);

        /// <summary>
        /// Sets the image for the UP state on this key.
        /// </summary>
        /// <param name="image">Image file to set for the state.</param>
        [PublicAPI]
        void DrawUp(string image);

        /// <summary>
        /// Enables event propagation for this dynamic key.
        /// </summary>
        [PublicAPI]
        void Enable();

        /// <summary>
        /// Sets the images to be drawn and refreshed on this key.
        /// </summary>
        /// <param name="image">Path to UP image.</param>
        /// <param name="downImage">Path to DOWN image.</param>
        /// <param name="interval">The interval (in milliseconds) at which to refresh the images.</param>
        [PublicAPI]
        void Set(string image, string downImage, int interval = 42);
    }
}
