// ---------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="SharpBlade">
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

namespace SharpBlade.Razer
{
    using System;
    using System.Runtime.InteropServices;

    using SharpBlade.Native;

    /// <summary>
    /// Class containing native RazerAPI methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The DLL file containing the SDK functions.
        /// </summary>
        /// <remarks>Must be located in the system PATH.</remarks>
        private const string DllName = "RzSwitchbladeSDK2.dll";

        /// <summary>
        /// Sets the callback function for application event callbacks.
        /// </summary>
        /// <param name="callback">
        /// Pointer to a callback function. If this argument is set to NULL, the routine clears the previously set callback function.
        /// </param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBAppEventSetCallback([In] AppEventCallback callback);

        /// <summary>
        /// Enables or disables the keyboard capture functionality.
        /// </summary>
        /// <param name="enable">The enable state. true enables the capture while false disables it.</param>
        /// <remarks>
        /// When the capture is enabled, the SDK application can receive keyboard
        /// input events through the callback assigned using <see cref="RzSBKeyboardCaptureSetCallback" />.
        /// The OS will not receive any keyboard input from the Switchblade device as long as the capture is active.
        /// Hence, applications must release the capture when no longer in use (call <see cref="RzSBEnableGesture" /> with false as parameter).
        /// The function only affects the keyboard device where the application is running. Other keyboard devices will work normally.
        /// </remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBCaptureKeyboard([MarshalAs(UnmanagedType.U1)] bool enable);

        /// <summary>
        /// Sets the callback function for dynamic key events.
        /// </summary>
        /// <param name="callback">
        /// Pointer to a callback function. If this argument is set to NULL, the routine clears the previously set callback function.
        /// </param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBDynamicKeySetCallback([In] DynamicKeyCallback callback);

        /// <summary>
        /// Enables or disables gesture events.
        /// </summary>
        /// <param name="gestureTypes"><see cref="GestureTypes" /> to be enabled or disabled.</param>
        /// <param name="enable">The enable state. true enables the gesture while false disables it.</param>
        /// <remarks>
        /// In nearly all cases, gestural events are preceded by a <see cref="GestureTypes.Press" /> event.
        /// With multiple finger gestures, the first finger contact registers as a press,
        /// and the touchpad reports subsequent contacts as the appropriate compound gesture (tap, flick, zoom or rotate).
        /// </remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBEnableGesture(
            [In] GestureTypes gestureTypes,
            [In] [MarshalAs(UnmanagedType.U1)] bool enable);

        /// <summary>
        /// Enables or disables gesture event forwarding to the OS.
        /// </summary>
        /// <param name="gestureTypes"><see cref="GestureTypes" /> to be enabled or disabled.</param>
        /// <param name="enable">The enable state. true enables the gesture while false disables it.</param>
        /// <remarks>
        /// Setting the <see cref="GestureTypes.Press"/> for OS gesture is equivalent to
        /// <see cref="GestureTypes.Press"/>, <see cref="GestureTypes.Move" /> and <see cref="GestureTypes.Release" />.
        /// </remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBEnableOSGesture(
            [In] GestureTypes gestureTypes,
            [In] [MarshalAs(UnmanagedType.U1)] bool enable);

        /// <summary>
        /// Sets the callback function for gesture events.
        /// </summary>
        /// <param name="callback">
        /// Pointer to a callback function. If this argument is set to NULL, the routine clears the previously set callback function.
        /// </param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBGestureSetCallback([In] TouchpadGestureCallback callback);

        /// <summary>
        /// Sets the callback function for dynamic key events. [sic]
        /// </summary>
        /// <param name="callback">
        /// Pointer to a callback function. If this argument is set to NULL, the routine clears the previously set callback function.
        /// </param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBKeyboardCaptureSetCallback([In] KeyboardCallback callback);

        /// <summary>
        /// Collects information about the SDK and the hardware supported.
        /// </summary>
        /// <param name="capabilities">
        /// A pointer to a previously allocated structure of type <see cref="Capabilities"/>.
        /// On successful execution, this routine fills the parameters in capabilities with the
        /// proper information about the SDK and supported hardware.
        /// </param>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBQueryCapabilities(out Capabilities capabilities);

        /// <summary>
        /// Controls output to the Switchblade display.
        /// The application can send bitmap data buffer directly to the Switchblade track pad
        /// display thru this function providing are more direct and faster way of updating the display.
        /// </summary>
        /// <param name="target">
        /// Specifies the target location on the Switchblade display – the main display or one of the dynamic key areas.
        /// Please refer to the definition for <see cref="TargetDisplay" /> for accepted values.
        /// </param>
        /// <param name="bufferParams">
        /// A pointer to a buffer parameter structure of type <see cref="BufferParams" /> that
        /// must be filled with the appropriate information for the image being sent to the render buffer.
        /// This input parameter is an RGB565 bitmap image buffer with a bottom-up orientation.
        /// Please refer to the definition for <see cref="BufferParams" /> for further detail.
        /// </param>
        /// <remarks>
        /// Since the function accepts the buffer for bottom-up bitmap,
        /// the application should invert the original image along its vertical axis prior to calling the function.
        /// This can be done easily with <c>BitBlit</c> and <c>StretchBlt</c> APIs.
        /// </remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBRenderBuffer(
            [In] TargetDisplay target,
            [In] /*ref BufferParams*/ IntPtr bufferParams);

        /// <summary>
        /// Set images on the Switchblade UI’s Dynamic Keys.
        /// </summary>
        /// <param name="dk"><see cref="DynamicKeyType" /> indicating which key to set the image on.</param>
        /// <param name="state">
        /// The desired dynamic key state (up, down) for the specified image. See <see cref="DynamicKeyState" /> for accepted values.
        /// </param>
        /// <param name="filename">
        /// The image file path for the given state. This image should be 115 x 115 pixels in dimension.
        /// Accepted file formats are BMP, GIF, JPG, and PNG.
        /// </param>
        /// <remarks>Animation in GIF files are not supported.</remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBSetImageDynamicKey(
            [In] DynamicKeyType dk,
            [In] DynamicKeyState state,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string filename);

        /// <summary>
        /// Places an image on the main Switchblade display.
        /// </summary>
        /// <param name="filename">
        /// File path to the image to be placed on the main Switchblade display.
        /// This image should be 800 x 480 pixels in dimension. Accepted file formats are BMP, GIF, JPG, and PNG.
        /// </param>
        /// <remarks>Animation in GIF files are not supported.</remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBSetImageTouchpad([In] [MarshalAs(UnmanagedType.LPWStr)] string filename);

        /// <summary>
        /// Grants access to the Switchblade device, establishing application connections.
        /// </summary>
        /// <remarks>
        /// <see cref="RzSBStart" /> sets up the connections that allow an application to access the Switchblade hardware device.
        /// This routine returns <see cref="HRESULT.RZSB_OK" /> on success, granting the calling application control of the device.
        /// Subsequent calls to this routine prior to a matching <see cref="RzSBStop" /> call are ignored.
        /// <para><see cref="RzSBStart" /> must be called before other Switchblade SDK routines will succeed.
        /// <see cref="RzSBStart" /> must always be accompanied by an <see cref="RzSBStop" />.</para>
        /// <para>COM initialization should be called prior to calling <see cref="RzSBStart" />.
        /// If the application developer intends to use Single-Threaded Apartment model (STA) and call the SDK
        /// functions within the same thread where the COM was initialized, then <c>CoInitialize()</c> should be called
        /// before <see cref="RzSBStart" />.</para>
        /// <para>Note that some MFC controls automatically initializes to STA.</para>
        /// <para>If the application developer intends to call the SDK functions on different threads,
        /// then the <c>CoInitializeEx()</c> should be called before <see cref="RzSBStart" />.</para>
        /// <para>Note: When the <see cref="RzSBStart()" /> is called without the COM being initialized
        /// (e.g. thru calling <c>CoInitializeEx</c>)
        /// the SDK initializes the COM to Multi-Threaded Apartment (MTA) model.
        /// As such, callers must invoke SDK functions from an MTA thread.
        /// Future SDK versions will move these calls into an isolated STA, giving application developers additional
        /// freedom to use COM in any fashion.
        /// However, application developers may already implement their own processing to isolate the SDK
        /// initialization and calls to avoid the issues for COM in different threading models.</para>
        /// </remarks>
        /// <returns><see cref="HRESULT" /> object indicating success or failure.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern HRESULT RzSBStart();

        /// <summary>
        /// Cleans up the Switchblade device connections and releases it for other applications.
        /// </summary>
        /// <remarks>
        /// <see cref="RzSBStop" /> cleans up the connections made by <see cref="RzSBStart" />.
        /// This routine releases an application’s control of the Switchblade hardware device,
        /// allowing other applications to take control.
        /// Subsequent calls to this routine prior to a matching <see cref="RzSBStart" /> are ignored.
        /// If an application terminates after calling <see cref="RzSBStart" />
        /// without a matching call to <see cref="RzSBStop" />,
        /// other applications may fail to acquire control of the Switchblade device.
        /// In this case, manually kill the framework processes.
        /// </remarks>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void RzSBStop();
    }
}
