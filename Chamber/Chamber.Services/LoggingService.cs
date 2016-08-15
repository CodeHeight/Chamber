using System;
using System.Text;
using Chamber.Domain.Interfaces.Services;

namespace Chamber.Services
{
    public partial class LoggingService : ILoggingService
    {
        private const string LogFileNameOnly = @"LogFile";
        private const string LogFileExtension = @".txt";
        private const string LogFileDirectory = @"~/App_Data";

        private const string DateTimeFormat = @"dd/MM/yyyy HH:mm:ss";
        private static readonly Object LogLock = new Object();
        private static string _logFileFolder;
        //private static int _maxLogSize = 10000;
        //private static string _logFileName;

        public LoggingService()
        {
            // If we have no http context current then assume testing mode i.e. log file in run folder
            //_logFileFolder = HttpContext.Current != null ? HttpContext.Current.Server.MapPath(LogFileDirectory) : @".";
            _logFileFolder = System.Web.Hosting.HostingEnvironment.MapPath(LogFileDirectory);
            //_logFileName = MakeLogFileName(false);
        }

        public void Error(Exception ex)
        {
            const int maxExceptionDepth = 5;

            if (ex == null)
            {
                return;
            }

            var message = new StringBuilder(ex.Message);

            var inner = ex.InnerException;
            var depthCounter = 0;
            while (inner != null && depthCounter++ < maxExceptionDepth)
            {
                message.Append(" INNER EXCEPTION: ");
                message.Append(inner.Message);
                inner = inner.InnerException;
            }

            Write(message.ToString());
        }

        /// <summary>
        /// Logs an error based log with a message
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Write(message);
        }

        private static void Write(string message)
        {
            //we'll save to the database (future codeing.....)

            //if (message != "File does not exist.")
            //{
            //    try
            //    {
            //        // Note there is a lock here. This class is only suitable for error logging,
            //        // not ANY form of trace logging...
            //        lock (LogLock)
            //        {
            //            //if (Length() >= _maxLogSize)
            //            //{
            //            //    ArchiveLog();
            //            //}

            //            using (var tw = TextWriter.Synchronized(File.AppendText(_logFileName)))
            //            {
            //                var callStack = new StackFrame(2, true); // Go back one stack frame to get module info

            //                tw.WriteLine("{0} | {1} | {2} | {3} | {4} | {5}", DateTime.UtcNow.ToString(DateTimeFormat), callStack.GetMethod().Module.Name, callStack.GetMethod().Name, callStack.GetMethod().DeclaringType, callStack.GetFileLineNumber(), message);
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        // Not much to do if logging failed...
            //    }
            //}
        }
    }
}