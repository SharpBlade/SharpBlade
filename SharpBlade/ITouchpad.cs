// ---------------------------------------------------------------------------------------
// <copyright file="ITouchpad.cs" company="SharpBlade">
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
using SharpBlade.Rendering;

namespace SharpBlade
{
    /// <summary>
    /// <see cref="Touchpad" /> interface.
    /// </summary>
    public interface ITouchpad : IRenderTarget
    {
        /// <summary>
        /// Raised when a finger(s?) flick on the touchpad.
        /// </summary>
        event EventHandler<FlickEventArgs> Flick;

        /// <summary>
        /// Raised when a gesture occurs on the touchpad.
        /// </summary>
        event EventHandler<GestureEventArgs> Gesture;

        /// <summary>
        /// Raised when a finger is held on the touchpad.
        /// </summary>
        event EventHandler<GestureEventArgs> Hold;

        /// <summary>
        /// Raised when the finger moves on the touchpad.
        /// </summary>
        event EventHandler<MoveEventArgs> Move;

        /// <summary>
        /// Raised when the touchpad is pressed.
        /// </summary>
        event EventHandler<PressEventArgs> Press;

        /// <summary>
        /// Raised when a finger is released from the touchpad.
        /// </summary>
        event EventHandler<ReleaseEventArgs> Release;

        /// <summary>
        /// Raised when a rotating motion is performed on the touchpad.
        /// </summary>
        event EventHandler<RotateEventArgs> Rotate;

        /// <summary>
        /// Raised when a scrolling motion is performed on the touchpad.
        /// </summary>
        event EventHandler<GestureEventArgs> Scroll;

        /// <summary>
        /// Raised when the touchpad is tapped.
        /// </summary>
        event EventHandler<TapEventArgs> Tap;

        /// <summary>
        /// Raised when a pinch motion is performed on the touchpad.
        /// </summary>
        event EventHandler<ZoomEventArgs> Zoom;

        /// <summary>
        /// Gets or sets a value indicating whether this object has been disposed.
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Gets or sets the <see cref="IRenderer" /> instance used to
        /// manage timed rendering of objects.
        /// </summary>
        IRenderer Renderer { get; }

        /// <summary>
        /// Disables a gesture from being handled by the touchpad.
        /// </summary>
        /// <param name="gestureTypes">Gesture to disable.</param>
        [CLSCompliant(false)]
        void DisableGesture(GestureTypes gestureTypes);

        /// <summary>
        /// Disables forwarding of a gesture.
        /// </summary>
        /// <param name="gestureTypes">Gesture to disable.</param>
        [CLSCompliant(false)]
        void DisableOSGesture(GestureTypes gestureTypes);

        /// <summary>
        /// Enables a gesture to be handled by the touchpad.
        /// </summary>
        /// <param name="gestureTypes">Gesture to enable.</param>
        [CLSCompliant(false)]
        void EnableGesture(GestureTypes gestureTypes);

        /// <summary>
        /// Enables a gesture to be forwarded to the host operating system.
        /// </summary>
        /// <param name="gestureTypes">Gesture to forward.</param>
        [CLSCompliant(false)]
        void EnableOSGesture(GestureTypes gestureTypes);

        /// <summary>
        /// Sets whether a gesture should be handled by the touchpad.
        /// </summary>
        /// <param name="gestureTypes">The gesture type to set.</param>
        /// <param name="enabled">True to enable gesture, false to disable.</param>
        [CLSCompliant(false)]
        void SetGesture(GestureTypes gestureTypes, bool enabled);

        /// <summary>
        /// Sets whether a gesture should be forwarded to the host operating system.
        /// </summary>
        /// <param name="gestureTypes">Gesture to set.</param>
        /// <param name="enabled">True to enable forwarding, false to disable.</param>
        [CLSCompliant(false)]
        void SetOSGesture(GestureTypes gestureTypes, bool enabled);
    }
}