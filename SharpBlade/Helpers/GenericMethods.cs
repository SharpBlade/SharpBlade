// ---------------------------------------------------------------------------------------
// <copyright file="GenericMethods.cs" company="SharpBlade">
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

namespace SharpBlade.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Helper functions for generic operations.
    /// </summary>
    internal static class GenericMethods
    {
        /// <summary>
        /// Gets the absolute path to the specified file.
        /// </summary>
        /// <param name="path">(Relative) path to resolve.</param>
        /// <returns>The absolute path to the specified relative path on the file system.</returns>
        internal static string GetAbsolutePath(string path)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var combined = Path.Combine(currentDirectory, path);
            return combined;
        }

        /// <summary>
        /// Gets a blank (filled with black) <see cref="Bitmap" /> object with
        /// the specified dimensions.
        /// </summary>
        /// <param name="width">Width of the bitmap to create.</param>
        /// <param name="height">Height of the bitmap to create.</param>
        /// <returns>
        /// An instance of <see cref="Bitmap" /> that matches the specified dimensions
        /// and is filled with the color black.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "There isn't really anywhere that could throw an exception here.")]
        internal static Bitmap GetBlankBitmap(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (var gfx = Graphics.FromImage(bmp))
                gfx.FillRectangle(Brushes.Black, 0, 0, width, height);
            return bmp;
        }
    }
}
