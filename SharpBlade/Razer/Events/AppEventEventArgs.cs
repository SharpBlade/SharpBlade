﻿// ---------------------------------------------------------------------------------------
// <copyright file="AppEventEventArgs.cs" company="SharpBlade">
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

using SharpBlade.Native;

namespace SharpBlade.Razer.Events
{
    /// <summary>
    /// Event generated by Razer API when apps are switched.
    /// </summary>
    public class AppEventEventArgs : EventArgs
    {
        /// <summary>
        /// The event type.
        /// </summary>
        private readonly RazerAPI.AppEventType _eventType;

        /// <summary>
        /// The first DWORD parameter passed to the callback function.
        /// </summary>
        private readonly uint _firstParam;

        /// <summary>
        /// The second DWORD parameter passed to the callback function.
        /// </summary>
        private readonly uint _secondParam;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppEventEventArgs" /> class.
        /// </summary>
        /// <param name="eventType">App event type.</param>
        /// <param name="firstParam">App event mode associated with this event.</param>
        /// <param name="secondParam">Process ID associated with event.</param>
        internal AppEventEventArgs(RazerAPI.AppEventType eventType, uint firstParam, uint secondParam)
        {
            _eventType = eventType;
            _firstParam = firstParam;
            _secondParam = secondParam;
        }

        /// <summary>
        /// Gets the first DWORD (<c>uint</c>) parameter passed to the app event callback.
        /// </summary>
        [CLSCompliant(false)]
        public uint FirstParameter
        {
            get { return _firstParam; }
        }

        /// <summary>
        /// Gets the second DWORD (<c>uint</c>) parameter passed to the app event callback.
        /// </summary>
        [CLSCompliant(false)]
        public uint SecondParameter
        {
            get { return _secondParam; }
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public RazerAPI.AppEventType EventType
        {
            get { return _eventType; }
        }
    }
}
