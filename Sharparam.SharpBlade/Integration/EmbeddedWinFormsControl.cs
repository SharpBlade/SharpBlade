// ---------------------------------------------------------------------------------------
// <copyright file="EmbeddedWinFormsControl.cs" company="SharpBlade">
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

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Sharparam.SharpBlade.Integration
{
    /// <summary>
    /// Contains references to a <see cref="WindowsFormsHost" /> object and
    /// a <see cref="System.Windows.Forms.Control" /> embedded in the host. Provides methods
    /// for drawing the control structure to bitmap.
    /// </summary>
    public struct EmbeddedWinFormsControl
    {
        /// <summary>
        /// The hosted WinForms control.
        /// </summary>
        public readonly Control Control;

        /// <summary>
        /// The <see cref="WindowsFormsHost" /> object hosting the control object.
        /// </summary>
        public readonly WindowsFormsHost Host;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedWinFormsControl" /> struct.
        /// </summary>
        /// <param name="host">The WindowsFormsHost object containing the control.</param>
        /// <param name="control">The WinForms control contained in the host.</param>
        public EmbeddedWinFormsControl(WindowsFormsHost host, Control control)
        {
            Host = host;
            Control = control;
        }

        /// <summary>
        /// Gets the size and location of the component, in pixels.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Host.Margin.Left, (int)Host.Margin.Top, Control.Width, Control.Height);
            }
        }

        /// <summary>
        /// Draws the structure to a bitmap.
        /// </summary>
        /// <returns>Bitmap with the control structure rendered.</returns>
        public Bitmap Draw()
        {
            var rect = Bounds;
            var bitmap = new Bitmap(rect.Width, rect.Height);
            Control.DrawToBitmap(bitmap, Control.Bounds);
            return bitmap;
        }
    }
}
