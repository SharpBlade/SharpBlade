// ---------------------------------------------------------------------------------------
// <copyright file="ISwitchblade.cs" company="SharpBlade">
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
using System.Windows.Forms;

using SharpBlade.Events;
using SharpBlade.Razer;
using SharpBlade.Rendering;

namespace SharpBlade
{
    /// <summary>
    /// <see cref="Switchblade" /> interface.
    /// </summary>
    public interface ISwitchblade : IDisposable
    {
        /// <summary>
        /// Raised when an app event occurs.
        /// </summary>
        event EventHandler<AppEventEventArgs> AppEvent;

        /// <summary>
        /// Raised when a keyboard char event occurs.
        /// </summary>
        event EventHandler<KeyboardCharEventArgs> KeyboardCharTyped;

        /// <summary>
        /// Raised when a keyboard key is pressed.
        /// </summary>
        event EventHandler<KeyboardKeyEventArgs> KeyboardKeyDown;

        /// <summary>
        /// Raised when a keyboard key is released.
        /// </summary>
        event EventHandler<KeyboardKeyEventArgs> KeyboardKeyUp;

        /// <summary>
        /// Raised when a keyboard raw event occurs.
        /// </summary>
        event EventHandler<KeyboardRawEventArgs> KeyboardRawEvent;

        /// <summary>
        /// Gets or sets the image shown on Touchpad when it's blank or
        /// after <see cref="SharpBlade.Touchpad.ClearImage" /> or <see cref="RenderTarget.Clear()" />.
        /// have been called.
        /// </summary>
        /// <remarks>Defaults to <see cref="Constants.BlankTouchpadImage" /></remarks>
        string BlankTouchpadImagePath { get; set; }

        /// <summary>
        /// Gets a structure describing the SDK and hardware capabilities of the system.
        /// </summary>
        Capabilities Capabilities { get; }

        /// <summary>
        /// Gets the dynamic key manager.
        /// </summary>
        IDynamicKeys DynamicKeys { get; }

        /// <summary>
        /// Gets or sets the image shown on dynamic keys when disabled.
        /// </summary>
        /// <remarks>Defaults to <see cref="Constants.DisabledDynamicKeyImage" /></remarks>
        string DisabledDynamicKeyImagePath { get; set; }

        /// <summary>
        /// Gets the <see cref="IDisplayStateFile" /> instance associated with this
        /// <see cref="Switchblade" /> object.
        /// </summary>
        IDisplayStateFile DisplayStateFile { get; }

        /// <summary>
        /// Gets a value indicating whether keyboard capture is enabled or not.
        /// </summary>
        bool KeyboardCapture { get; }

        /// <summary>
        /// Gets the touchpad on the keyboard.
        /// </summary>
        ITouchpad Touchpad { get; }

        /// <summary>
        /// Enables or disables keyboard capture.
        /// </summary>
        /// <param name="enabled">Whether or not to enable keyboard capture.</param>
        void SetKeyboardCapture(bool enabled);

        /// <summary>
        /// Starts forwarding keyboard events to the specified WinForms control.
        /// </summary>
        /// <param name="control">THe control to forward input to.</param>
        /// <param name="releaseOnEnter">If true, keyboard capture will cease when the enter key is pressed,
        /// otherwise, <see cref="Switchblade.SetKeyboardCapture" /> has to be called explicitly with false as the argument.</param>
        void StartWinFormsControlKeyboardCapture(Control control, bool releaseOnEnter = true);

        /// <summary>
        /// Starts forwarding keyboard events to the specified WPF control.
        /// </summary>
        /// <param name="control">The control to forward input to.</param>
        /// <param name="releaseOnEnter">If true, keyboard capture will cease when the enter key is pressed,
        /// otherwise, <see cref="Switchblade.SetKeyboardCapture" /> has to be called explicitly with false as the argument.</param>
        void StartWpfControlKeyboardCapture(System.Windows.Controls.Control control, bool releaseOnEnter = true);

        /// <summary>
        /// Stops all Razer interaction.
        /// </summary>
        void Stop();
    }
}
