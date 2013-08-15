using System;

namespace Sharparam.SharpBlade.Logging
{
    /// <summary>
    /// This log provider will return an instance of
    /// <see cref="SimpleLog" /> to the caller.
    /// </summary>
    /// <remarks>
    /// Please note that it will be impossible to determine
    /// the source of a log message as the SimpleLog does
    /// not save the caller's type. For more advanced
    /// logging capabilities, please implement your own
    /// log manager.
    /// </remarks>
    public class SimpleLogProvider : ILogProvider
    {
        public ILog GetLogger(Type type)
        {
            return new SimpleLog();
        }

        public ILog GetLogger(string name)
        {
            return new SimpleLog();
        }
    }
}
