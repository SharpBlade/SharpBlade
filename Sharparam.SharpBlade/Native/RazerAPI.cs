/* RazerAPI.cs
 *
 * Copyright © 2013 by Adam Hellberg
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Disclaimer: SwitchBladeSteam is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SharpBlade.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

// Credits to itsbth for helping with P/Invoke

/* ╔══════════════════════════════╗
 * ║ Help table for mapping types ║
 * ╠════════╦═════════╦═══════════╩──────────┐
 * │ C Type │ C# Type │ MarshalAs            │
 * ├────────┼─────────┼──────────────────────┤
 * │ BYTE   │ Byte    │                      │
 * │ UINT   │ UInt32  │                      │
 * │ HWND   │ IntPtr  │                      │
 * │ WORD   │ UInt16  │                      │
 * │ DWORD  │ UInt32  │                      │
 * │ LPARAM │ Int32   │                      │
 * │ WPARAM │ UInt32  │                      │
 * │ LPWSTR │ String  │ UnmanagedType.LPWStr │
 * └────────┴─────────┴──────────────────────┘
 */

// 2013-04-05: Major update to reflect changes in the new SDK.

using System;
using System.Runtime.InteropServices;

namespace Sharparam.SharpBlade.Native
{
    public static class RazerAPI
    {
        // Native functions from SwitchBladeSDK32.dll, all functions are __cdecl calls

        #region File Constants

        public const string DllName = "RzSwitchbladeSDK2.dll";

        #endregion File Constants

        #region Functions

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBStart();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void RzSBStop();
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBQueryCapabilities(out Capabilities pSBSDKCap);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBRenderBuffer([In] TargetDisplay dwTarget, [In] /*ref BufferParams*/ IntPtr bufferParams);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBSetImageDynamicKey(
            [In] DynamicKeyType dk,
            [In] DynamicKeyState state,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string filename);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBSetImageTouchpad([In] [MarshalAs(UnmanagedType.LPWStr)] string filename);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBEnableGesture([In] GestureType gestureType, [In] bool bEnable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBEnableOSGesture([In] GestureType gestureType, [In] bool bEnable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBCaptureKeyboard(bool bEnable);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBAppEventSetCallback([In] AppEventCallbackDelegate callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBDynamicKeySetCallback([In] DynamicKeyCallbackFunctionDelegate callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBGestureSetCallback([In] TouchpadGestureCallbackFunctionDelegate callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern HRESULT RzSBKeyboardCaptureSetCallback([In] KeyboardCallbackFunctionDelegate callback);

        #endregion Functions

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT DynamicKeyCallbackFunctionDelegate(DynamicKeyType dynamicKeyType, DynamicKeyState dynamicKeyState);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT AppEventCallbackDelegate(AppEventType appEventType, uint dwAppMode, uint dwProcessID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT TouchpadGestureCallbackFunctionDelegate(GestureType gestureType, uint dwParameters, ushort wXPos, ushort wYPos, ushort wZPos);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate HRESULT KeyboardCallbackFunctionDelegate(uint uMsg, UIntPtr wParam, IntPtr lParam);

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Capabilities
        {
            public ulong Version;

            public ulong BEVersion;

            public HardwareType HardwareType;

            public ulong NumSurfaces;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxSupportedSurfaces)]
            public Point[] Surfacegeometry;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxSupportedSurfaces)]
            public uint[] Pixelformat;

            public byte NumDynamicKeys;

            public Point DynamicKeyArrangement;

            public Point DynamicKeySize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BufferParams
        {
            public PixelType PixelType;
            public uint DataSize;
            public IntPtr PtrData;
        }

        #endregion Structs

        #region Enumerations

        public enum DynamicKeyState
        {
            None = 0,
            Up,
            Down,
            Hold,
            Invalid
        }

        public enum Direction
        {
            None = 0,
            Left,
            Right,
            Up,
            Down,
            Invalid
        }

        public enum DynamicKeyType
        {
            None = 0,
            DK1,
            DK2,
            DK3,
            DK4,
            DK5,
            DK6,
            DK7,
            DK8,
            DK9,
            DK10,
            Invalid,
            Count = 10
        }

        public enum TargetDisplay
        {
            Widget = ((1 << 16) | (0)),
            DK1    = ((1 << 16) | (1)),
            DK2    = ((1 << 16) | (2)),
            DK3    = ((1 << 16) | (3)),
            DK4    = ((1 << 16) | (4)),
            DK5    = ((1 << 16) | (5)),
            DK6    = ((1 << 16) | (6)),
            DK7    = ((1 << 16) | (7)),
            DK8    = ((1 << 16) | (8)),
            DK9    = ((1 << 16) | (9)),
            DK10   = ((1 << 16) | (10))
        }

        public enum PixelType
        {
            RGB565 = 0
        }

        public enum AppEventType
        {
            None = 0,
            Activated,
            Deactivated,
            Close,
            Exit,
            Invalid
        }

        public enum AppEventMode
        {
            Applet = 0x02,
            Normal = 0x04
        }

        [Flags]
        public enum GestureType : uint
        {
            None    = 0x00000000,
            Press   = 0x00000001,
            Tap     = 0x00000002,
            Flick   = 0x00000004,
            Zoom    = 0x00000008,
            Rotate  = 0x00000010,
            Move    = 0x00000020,
            Hold    = 0x00000040,
            Release = 0x00000080,
            Scroll  = 0x00000100,
            All     = 0xFFFF
        }

        public enum HardwareType
        {
            Invalid = 0,
            Switchblade,
            Undefined
        }

        #endregion Enumerations

        #region Constants

        /*
         * definitions for the Dynamic Key display region of the Switchblade
         */
        public const int DynamicKeysPerRow = 5;
        public const int DynamicKeysRows = 2;
        public const int DynamicKeysCount = DynamicKeysPerRow * DynamicKeysRows;
        public const int DynamicKeyWidth = 115;
        public const int DynamicKeyHeight = 115;
        public const int DynamicKeyImageDataSize = DynamicKeyWidth * DynamicKeyHeight * sizeof(ushort);

        /*
         * definitions for the Touchpad display region of the Switchblade
         */
        public const int TouchpadWidth = 800;
        public const int TouchpadHeight = 480;
        public const int TouchpadImageDataSize = TouchpadWidth * TouchpadHeight * sizeof(ushort);

        public const int DisplayColorDepth = 16;

        public const int MaxStringLength = 260;

        public const int MaxSupportedSurfaces = 2;

        public const int PixelFormatInvalid = 0;
        public const int PixelFormatRgb565 = 1;

        #endregion Constants

        #region Macros

        public static bool ValidGesture(uint a) { return (a & (uint) GestureType.All) != 0; }
        public static bool SingleGesture(uint a) { return 0 == ((a - a) & a); }

        #endregion Macros
    }
}
