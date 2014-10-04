// ---------------------------------------------------------------------------------------
// <copyright file="IDisplayStateFile.cs" company="SharpBlade">
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

    /// <summary>
    /// <see cref="DisplayStateFile" /> interface.
    /// </summary>
    public interface IDisplayStateFile : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this instance of <see cref="DisplayStateFile" /> has been disposed.
        /// </summary>
        [PublicAPI]
        bool Disposed { get; }

        /// <summary>
        /// Gets or sets a value indicating whether SharpBlade should monitor for changes
        /// to the <c>RzDisplayState</c> file and rename it in case it's improperly named
        /// by Razer's SDK.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is only really needed when the app's executable file contains a dot in
        /// its file name (For example Foo.Bar.exe). SharpBlade will monitor for changes to
        /// files named like the app but without any extension and rename them with an
        /// <c>rzdisplaystate</c> file extension, which makes it possible for the SBUI SDK
        /// to use them as thumbnails on the device.
        /// </para>
        /// <para>
        /// Use the value from <see cref="WorkaroundRequired" /> to determine whether
        /// the workaround is needed or not.
        /// </para>
        /// </remarks>
        [PublicAPI]
        bool Enabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the workaround method for generating <c>RzDisplayState</c>
        /// files is required for thumbnail functionality on the device.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the SharpBlade workaround should be enabled by setting
        /// the value of <see cref="Enabled" /> to <c>true</c>.
        /// </remarks>
        /// <example>
        /// // This will enable the SharpBlade workaround if the app is not compatible with Razer's code.
        /// Switchblade.Instance.DisplayStateFile.Enabled = Switchblade.Instance.DisplayStateFile.WorkaroundRequired;
        /// </example>
        [PublicAPI]
        bool WorkaroundRequired { get; }

        /// <summary>
        /// Force running the workaround regardless of compatibility.
        /// </summary>
        /// <remarks>
        /// This may have unintended side-effects when run in an app that
        /// is already compatible with Razer's existing generation code.
        /// </remarks>
        [PublicAPI]
        void Fix();
    }
}
