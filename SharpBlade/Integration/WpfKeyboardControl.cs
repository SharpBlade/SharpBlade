// ---------------------------------------------------------------------------------------
// <copyright file="WpfKeyboardControl.cs" company="SharpBlade">
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

namespace SharpBlade.Integration
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using SharpBlade.Logging;
    using SharpBlade.Native.WinAPI;

    /// <summary>
    /// Helper class to send key events to a WPF control.
    /// </summary>
    internal sealed class WpfKeyboardControl : KeyboardControl
    {
        /// <summary>
        /// <see cref="log4net.ILog" /> instance for this class.
        /// </summary>
        private static readonly log4net.ILog Log = LogManager.GetLogger(typeof(WpfKeyboardControl));

        /// <summary>
        /// The WPF <see cref="Control" /> upon which the events are raised.
        /// </summary>
        private readonly Control _control;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfKeyboardControl" /> class.
        /// </summary>
        /// <param name="control">A WPF <see cref="Control" /> to raise key events on.</param>
        /// <param name="releaseOnEnter">Whether or not to release control when enter is pressed.</param>
        internal WpfKeyboardControl(Control control, bool releaseOnEnter)
            : base(releaseOnEnter)
        {
            _control = control;
        }

        /// <summary>
        /// Sends a TextComposition event to the WPF control.
        /// </summary>
        /// <param name="character">The character that was typed.</param>
        internal override void SendChar(char character)
        {
            // Because MS CBA to let us send chars directly
            var str = character.ToString(CultureInfo.InvariantCulture);
            _control.RaiseEvent(
                new TextCompositionEventArgs(
                    InputManager.Current.PrimaryKeyboardDevice,
                    new TextComposition(InputManager.Current, _control, str))
                {
                    RoutedEvent = TextCompositionManager.TextInputEvent
                });
        }

        /// <summary>
        /// Sends a WPF KeyDown event to the WPF control.
        /// </summary>
        /// <param name="key">Key that was pressed.</param>
        internal override void SendKeyDown(VirtualKey key)
        {
            SendKeyEvents(key, Keyboard.PreviewKeyDownEvent, Keyboard.KeyDownEvent);
        }

        /// <summary>
        /// Sends a WPF KeyUp event to the WPF control.
        /// </summary>
        /// <param name="key">Key that was released.</param>
        internal override void SendKeyUp(VirtualKey key)
        {
            SendKeyEvents(key, Keyboard.PreviewKeyUpEvent, Keyboard.KeyUpEvent);
        }

        /// <summary>
        /// Sends a pair of events to the WPF control.
        /// </summary>
        /// <param name="key">Key relevant to the event.</param>
        /// <param name="previewEvent">The preview event to raise.</param>
        /// <param name="normalEvent">The actual event to raise.</param>
        private void SendKeyEvents(VirtualKey key, RoutedEvent previewEvent, RoutedEvent normalEvent)
        {
            // Conversion magic, don't blink!
            var wpfKey = KeyInterop.KeyFromVirtualKey((int)key);

            var source = PresentationSource.FromVisual(_control);

            if (source == null)
            {
                Log.Error("SendKeyEvents: source is null");
                return;
            }

            _control.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, wpfKey)
                {
                    RoutedEvent = previewEvent
                });

            _control.RaiseEvent(
                new KeyEventArgs(Keyboard.PrimaryDevice, source, 1, wpfKey)
                {
                    RoutedEvent = normalEvent
                });
        }
    }
}
