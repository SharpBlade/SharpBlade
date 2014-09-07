// ---------------------------------------------------------------------------------------
// <copyright file="Capabilities.cs" company="SharpBlade">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

using SharpBlade.Annotations;

namespace SharpBlade.Razer
{
    /// <summary>
    /// Specifies the capabilities of this SwitchBlade device.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable",
        Justification = "This was translated from native code.")]
    public struct Capabilities
    {
        /// <summary>
        /// Version of SDK/hardware.
        /// </summary>
        [FieldOffset(0)]
        private readonly uint _version; // DWORD: 4 bytes

        /// <summary>
        /// BEVersion returned from capabilities function.
        /// </summary>
        [FieldOffset(4)]
        private readonly uint _beVersion; // DWORD: 4 bytes

        /// <summary>
        /// Type of device.
        /// </summary>
        [FieldOffset(8)]
        private readonly HardwareType _hardwareType; // C++ enum default size 4 bytes

        /// <summary>
        /// Number of surfaces available.
        /// </summary>
        [FieldOffset(12)]
        private readonly uint _numSurfaces; // DWORD: 4 bytes

        /// <summary>
        /// Surface geometry for each surface.
        /// </summary>
        /// <remarks>Contains <see cref="_numSurfaces" /> entries.</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MaxSupportedSurfaces)]
        [FieldOffset(16)]
        private readonly Point[] _surfacegeometry; // Win32 POINT 2*LONG: 4*2 = 8 bytes, sizeconst 2 gives 16 bytes

        /// <summary>
        /// Pixel format of each surface.
        /// </summary>
        /// <remarks>Contains <see cref="_numSurfaces" /> entries.</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MaxSupportedSurfaces)]
        [FieldOffset(32)]
        private readonly uint[] _pixelformat; // DWORD array, 4 bytes per DWORD, sizeconst 2 gives 4*2 = 8 bytes

        /// <summary>
        /// Number of dynamic keys available on device.
        /// </summary>
        [FieldOffset(40)]
        private readonly byte _numDynamicKeys; // BYTE: 1 byte

        /// <summary>
        /// Arrangement of the dynamic keys.
        /// </summary>
        [FieldOffset(44)]
        private readonly Point _dynamicKeyArrangement; // Win32 POINT has 2 LONG = 2*4 = 8 bytes

        /// <summary>
        /// Size of each dynamic key.
        /// </summary>
        [FieldOffset(52)]
        private readonly Point _dynamicKeySize; // Win32 POINT has 2 LONG = 2*4 = 8 bytes

        /// <summary>
        /// Gets the BEVersion returned from capabilities function.
        /// </summary>
        [CLSCompliant(false)]
        public uint BEVersion
        {
            get { return _beVersion; }
        }

        /// <summary>
        /// Gets the size of each dynamic key.
        /// </summary>
        [PublicAPI]
        public Point DynamicKeySize
        {
            get { return _dynamicKeySize; }
        }

        /// <summary>
        /// Gets the arrangement of the dynamic keys.
        /// </summary>
        [PublicAPI]
        public Point DynamicKeyArrangement
        {
            get { return _dynamicKeyArrangement; }
        }

        /// <summary>
        /// Gets the number of dynamic keys available on the device.
        /// </summary>
        public byte DynamicKeyCount
        {
            get { return _numDynamicKeys; }
        }

        /// <summary>
        /// Gets the type of device that is currently connected.
        /// </summary>
        [PublicAPI]
        public HardwareType HardwareType
        {
            get { return _hardwareType; }
        }

        /// <summary>
        /// Gets the pixel format of each surface.
        /// </summary>
        /// <remarks>Contains <see cref="SurfaceCount" /> entries.</remarks>
        [CLSCompliant(false)]
        [PublicAPI]
        public IList<uint> Pixelformat
        {
            get { return _pixelformat; }
        }

        /// <summary>
        /// Gets the surface geometry of each surface.
        /// </summary>
        /// <remarks>Contains <see cref="SurfaceCount" /> entries.</remarks>
        [PublicAPI]
        public IList<Point> Surfacegeometry
        {
            get { return _surfacegeometry; }
        }

        /// <summary>
        /// Gets the number of surfaces available.
        /// </summary>
        [CLSCompliant(false)]
        public uint SurfaceCount
        {
            get { return _numSurfaces; }
        }

        /// <summary>
        /// Gets the version of the SDK/hardware.
        /// </summary>
        [CLSCompliant(false)]
        public uint Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Compares two <see cref="Capabilities" /> structures for equality.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the structures are equal, false otherwise.</returns>
        public static bool operator ==(Capabilities left, Capabilities right)
        {
            return left._version == right._version && left._beVersion == right._beVersion
                   && left._hardwareType == right._hardwareType && left._numSurfaces == right._numSurfaces
                   && left._surfacegeometry.Equals(right._surfacegeometry)
                   && left._pixelformat.Equals(right._pixelformat) && left._numDynamicKeys == right._numDynamicKeys
                   && left._dynamicKeyArrangement.Equals(right._dynamicKeyArrangement)
                   && left._dynamicKeySize.Equals(right._dynamicKeySize);
        }

        /// <summary>
        /// Compares two <see cref="Capabilities" /> structures for inequality.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the structures are not equal, false otherwise.</returns>
        public static bool operator !=(Capabilities left, Capabilities right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Capabilities && this == (Capabilities)obj;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)_version;
                hashCode = (hashCode * 397) ^ (int)_beVersion;
                hashCode = (hashCode * 397) ^ (int)_hardwareType;
                hashCode = (hashCode * 397) ^ (int)_numSurfaces;
                hashCode = (hashCode * 397) ^ _surfacegeometry.GetHashCode();
                hashCode = (hashCode * 397) ^ _pixelformat.GetHashCode();
                hashCode = (hashCode * 397) ^ _numDynamicKeys.GetHashCode();
                hashCode = (hashCode * 397) ^ _dynamicKeyArrangement.GetHashCode();
                hashCode = (hashCode * 397) ^ _dynamicKeySize.GetHashCode();
                return hashCode;
            }
        }
    }
}
