using System;
using System.IO;

namespace Sharparam.SharpBlade.Logging
{
    /// <summary>
    /// A simple log to use with the logging system.
    /// Writes output to the specified target TextWriter object.
    /// </summary>
    public class SimpleLog : ILog
    {
        /// <summary>
        /// Gets or sets the target TextWriter that output
        /// will be written to.
        /// This field defaults to Console.Out.
        /// </summary>
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
        public TextWriter Target = Console.Out;
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore FieldCanBeMadeReadOnly.Global

        // Helper method to write output
        private void Write(string format, params object[] args)
        {
            Target.WriteLine(format, args);
        }

        public void Debug(object message)
        {
            Write("[DEBUG]: {0}", message);
        }

        public void Info(object message)
        {
            Write("[INFO]: {0}", message);
        }

        public void Warn(object message)
        {
            Write("[WARN]: {0}", message);
        }

        public void Error(object message)
        {
            Write("[ERROR]: {0}", message);
        }

        public void Fatal(object message)
        {
            Write("[FATAL]: {0}", message);
        }

        public void Debug(object message, Exception exception)
        {
            Write("[DEBUG]: Exception of type {0} with message: {1}; Stack trace: {3}", exception.GetType(),
                  exception.Message, exception.StackTrace);
        }

        public void Info(object message, Exception exception)
        {
            Write("[INFO]: Exception of type {0} with message: {1}; Stack trace: {3}", exception.GetType(),
                  exception.Message, exception.StackTrace);
        }

        public void Warn(object message, Exception exception)
        {
            Write("[WARN]: Exception of type {0} with message: {1}; Stack trace: {3}", exception.GetType(),
                  exception.Message, exception.StackTrace);
        }

        public void Error(object message, Exception exception)
        {
            Write("[ERROR]: Exception of type {0} with message: {1}; Stack trace: {3}", exception.GetType(),
                  exception.Message, exception.StackTrace);
        }

        public void Fatal(object message, Exception exception)
        {
            Write("[FATAL]: Exception of type {0} with message: {1}; Stack trace: {3}", exception.GetType(),
                  exception.Message, exception.StackTrace);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Write("[DEBUG]: " + format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Write("[INFO]: " + format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Write("[WARN]: " + format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Write("[ERROR]: " + format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Write("[FATAL]: " + format, args);
        }

        public void Exception(Exception exception)
        {
            Write("[EXCEPTION]: {0} with message: {1}; Stack trace: {2}", exception.GetType(), exception.Message,
                  exception.StackTrace);
        }
    }
}
