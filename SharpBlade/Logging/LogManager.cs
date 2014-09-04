// ---------------------------------------------------------------------------------------
// <copyright file="LogManager.cs" company="SharpBlade">
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

#if DEBUG

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using log4net;
using log4net.Config;

using Microsoft.Win32.SafeHandles;

using SharpBlade.Native.WinAPI;

#else
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using log4net;
using log4net.Config;

#endif

namespace SharpBlade.Logging
{
    /// <summary>
    /// Provides helper methods for logging functions.
    /// </summary>
    public static class LogManager
    {
#if DEBUG

    /// <summary>
    /// Whether or not a console has been loaded and allocated for log output.
    /// </summary>
        private static bool _consoleLoaded;

#endif

        /// <summary>
        /// Whether or not a log4net instance (and thus its config)
        /// has been loaded.
        /// </summary>
        private static bool _loaded;

        /// <summary>
        /// Clears old log files from specified log directory.
        /// </summary>
        /// <param name="daysOld">Delete log files older than this number of days.</param>
        /// <param name="directory">The directory to check for log files.</param>
        /// <remarks>This will delete ALL files in the specified directory,
        /// regardless of file type.</remarks>
        // ReSharper disable UnusedMember.Global
        public static void ClearOldLogs(int daysOld = 7, string directory = "logs")
            // ReSharper restore UnusedMember.Global
        {
            var log = GetLogger(typeof(LogManager));

            if (!Directory.Exists(directory))
            {
                log.InfoFormat("Directory {0} not found, no logs to clear", directory);
                return;
            }

            var now = DateTime.Now;
            var max = new TimeSpan(daysOld, 0, 0, 0);
            var count = 0;
            foreach (var file in from file in Directory.GetFiles(directory)
                                 let modTime = File.GetLastAccessTime(file)
                                 let age = now.Subtract(modTime)
                                 where age > max
                                 select file)
            {
                try
                {
                    File.Delete(file);
                    log.InfoFormat("Deleted old log file: {0}", file);
                    count++;
                }
                catch (IOException ex)
                {
                    log.WarnFormat("Failed to delete log file: {0} ({1})", file, ex.Message);
                }
            }

            log.InfoFormat("Done! Cleared {0} log files.", count);
        }

        /// <summary>
        /// Destroys an open console, usually the one created by <see cref="SetupConsole" />.
        /// </summary>
        /// <remarks>Method body only compiled on DEBUG.</remarks>
        // ReSharper disable UnusedMember.Global
        public static void DestroyConsole() // ReSharper restore UnusedMember.Global
        {
#if DEBUG
            if (_consoleLoaded)
                Kernel32.NativeMethods.FreeConsole();
#endif
        }

        /// <summary>
        /// Gets a logger object associated with the specified object.
        /// </summary>
        /// <param name="sender">The object to get a logger for.</param>
        /// <returns>An <see cref="ILog" /> that provides logging features.</returns>
        public static ILog GetLogger(object sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            if (!_loaded)
                LoadConfig();

            var type = sender.GetType().ToString() == "System.RuntimeType" ? (Type)sender : sender.GetType();
            var logger = log4net.LogManager.GetLogger(type);

            return logger;
        }

        /// <summary>
        /// Loads a configuration for the log4net library.
        /// </summary>
        /// <param name="file">
        /// The configuration file to load.
        /// If null, tries to automatically load a config file based on assembly name,
        /// falls back to using default log4net configuration.
        /// </param>
        public static void LoadConfig(string file = null)
        {
            if (file == null)
            {
                var appConfigFile = AppDomain.CurrentDomain.FriendlyName + ".config";
                var appConfigLoaded = false;

                if (File.Exists(appConfigFile))
                {
                    var doc = XDocument.Load(appConfigFile);
                    var configElement = doc.Element("configuration");
                    if (configElement != null && configElement.Element("log4net") != null)
                    {
                        XmlConfigurator.Configure();
                        appConfigLoaded = true;
                    }
                }

                if (!appConfigLoaded)
                    BasicConfigurator.Configure();
            }
            else
            {
                if (File.Exists(file))
                    XmlConfigurator.Configure(new FileInfo(file));
                else
                {
                    LoadConfig();
                    return;
                }
            }

            _loaded = true;
        }

        /// <summary>
        /// Sets up a console for standard output.
        /// </summary>
        /// <remarks>Method body only compiled on DEBUG.</remarks>
        // ReSharper disable UnusedMember.Global
        public static void SetupConsole() // ReSharper restore UnusedMember.Global
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                return;

            Kernel32.NativeMethods.AllocConsole();
            var stdHandle = Kernel32.NativeMethods.GetStdHandle(Kernel32.STD_OUTPUT_HANDLE);
            var safeFileHandle = new SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = Encoding.GetEncoding(Kernel32.CODE_PAGE);
            var stdOut = new StreamWriter(fileStream, encoding) { AutoFlush = true };
            Console.SetOut(stdOut);
            _consoleLoaded = true;
#endif
        }
    }
}
