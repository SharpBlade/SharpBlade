// ---------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="SharpBlade">
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
    /// Constant values from <c>SwitchBladeSDK_defines.h</c>.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Color depth of the device's display areas.
        /// </summary>
        public const int DisplayColorDepth = 16;

        /// <summary>
        /// The height of one dynamic key, in pixels.
        /// </summary>
        /// <remarks>Note that this refers to the height of the display area on a dynamic key,
        /// not physical size.</remarks>
        public const int DynamicKeyHeight = 115;

        /// <summary>
        /// Size of image data for one dynamic key.
        /// </summary>
        public const int DynamicKeyImageDataSize = DynamicKeyWidth * DynamicKeyHeight * sizeof(ushort);

        /// <summary>
        /// Total number of dynamic keys that exist on the device.
        /// </summary>
        public const int DynamicKeysCount = DynamicKeysPerRow * DynamicKeysRows;

        /// <summary>
        /// Number of dynamic keys per row on the device.
        /// </summary>
        public const int DynamicKeysPerRow = 5;

        /// <summary>
        /// Number of rows on the dynamic keys.
        /// </summary>
        public const int DynamicKeysRows = 2;

        /// <summary>
        /// The width of one dynamic key, in pixels.
        /// </summary>
        /// <remarks>Note that this refers to the width of the display area on a dynamic key,
        /// not physical size.</remarks>
        public const int DynamicKeyWidth = 115;

        /// <summary>
        /// Max string length.
        /// </summary>
        public const int MaxStringLength = 260;

        /// <summary>
        /// Maximum supported surfaces.
        /// </summary>
        public const int MaxSupportedSurfaces = 2;

        /// <summary>
        /// Invalid pixel format.
        /// </summary>
        public const int PixelFormatInvalid = 0;

        /// <summary>
        /// RGB565 pixel format, used by standard SwitchBlade devices.
        /// </summary>
        public const int PixelFormatRgb565 = 1;

        /// <summary>
        /// Height of the touchpad on standard devices.
        /// </summary>
        public const int TouchpadHeight = 480;

        /// <summary>
        /// Size of image data to cover the touchpad.
        /// </summary>
        public const int TouchpadImageDataSize = TouchpadWidth * TouchpadHeight * sizeof(ushort);

        /// <summary>
        /// Width of the touchpad on standard devices.
        /// </summary>
        public const int TouchpadWidth = 800;
    }
}
