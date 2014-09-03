// ---------------------------------------------------------------------------------------
// <copyright file="AppEventType.cs" company="SharpBlade">
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

namespace SharpBlade.Razer
{
    /// <summary>
    /// App event types used by Razer's AppEvent callback system.
    /// </summary>
    public enum AppEventType
    {
        /// <summary>
        /// No/empty app event.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Switchblade framework has activated the SDK application.
        /// The application can resume its operations and update the Switchblade UI display.
        /// </summary>
        Activated,

        /// <summary>
        /// The application has been deactivated to make way for another application.
        /// In this state, the SDK application will not receive any Dynamic Key or Gesture events,
        /// nor will it be able to update the Switchblade displays.
        /// </summary>
        Deactivated,

        /// <summary>
        /// The Switchblade framework has initiated a request to close the application.
        /// The application should perform cleanup and can terminate on its own when done.
        /// </summary>
        Close,

        /// <summary>
        /// The Switchblade framework will forcibly close the application.
        /// This event is always preceded by the <see cref="Close" /> event.
        /// Cleanup should be done there.
        /// </summary>
        Exit,

        /// <summary>
        /// Invalid app event.
        /// </summary>
        Invalid
    }
}
