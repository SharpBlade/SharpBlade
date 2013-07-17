/* ILog.cs
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

using System;

namespace Sharparam.SharpBlade.Logging
{
    /// <summary>
    /// Provides methods for logging.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Writes a debug message to the logging output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Debug(object message);

        /// <summary>
        /// Writes an informational message to the logging output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Info(object message);

        /// <summary>
        /// Writes a warning message to the logging output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Warn(object message);

        /// <summary>
        /// Writes an error message to the logging output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Error(object message);

        /// <summary>
        /// Writes a fatal message to the logging output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Fatal(object message);

        /// <summary>
        /// Writes a debug message to the logging output
        /// accompanied by an exception stack trace.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">Exception object to read stack trace from.</param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// Writes an informational message to the logging output
        /// accompanied by an exception stack trace.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">Exception object to read stack trace from.</param>
        void Info(object message, Exception exception);

        /// <summary>
        /// Writes a warning message to the logging output
        /// accompanied by an exception stack trace.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">Exception object to read stack trace from.</param>
        void Warn(object message, Exception exception);

        /// <summary>
        /// Writes an error message to the logging output
        /// accompanied by an exception stack trace.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">Exception object to read stack trace from.</param>
        void Error(object message, Exception exception);

        /// <summary>
        /// Writes a fatal message to the logging output
        /// accompanied by an exception stack trace.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">Exception object to read stack trace from.</param>
        void Fatal(object message, Exception exception);

        /// <summary>
        /// Writes a formatted debug message to the logging output.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format arguments.</param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Writes a formatted informational message to the logging output.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format arguments.</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Writes a formatted warning message to the logging output.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format arguments.</param>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        /// Writes a formatted error message to the logging output.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format arguments.</param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Writes a formatted fatal message to the logging output.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format arguments.</param>
        void FatalFormat(string format, params object[] args);

        /// <summary>
        /// Writes exception details to logging output.
        /// </summary>
        /// <param name="exception">Exception object to read details from.</param>
        void Exception(Exception exception);
    }
}
