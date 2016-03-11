using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LogonTimes.Logging
{
    public class Logger : IDisposable, ILogger
    {
        private static readonly Logger instance = new Logger();
        private EventLog eventLog;
        private int currentLogLevel;
        private const string crlf = "\r\n";

        #region constructors
        private Logger()
        {
            currentLogLevel = Properties.Settings.Default.DebugLevel;
            OpenLog();
        }

        public static Logger Instance
        {
            get
            {
                return instance;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    eventLog.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
        #endregion

        #region Public methods
        public void Log(string message, DebugLevels debugLevel)
        {
            if (ShouldLog(debugLevel))
            {
                eventLog.WriteEntry(message);
            }
        }

        public void LogException(string header, DebugLevels debugLevel, Exception ex)
        {
            var message = new StringBuilder();
            message.Append(header);
            AddLineToMessage(message, ex.Message);
            AddLineToMessage(message, ex.StackTrace);
        }

        public bool ShouldLog(DebugLevels debugLevel)
        {
            return (currentLogLevel >= (int)debugLevel);
        }

        public void AddLineToMessage(StringBuilder stringBuilder, string message)
        {
            stringBuilder.Append(message);
            stringBuilder.Append(crlf);
        }
        #endregion

        #region Private methods
        private void OpenLog()
        {
            string eventLogSource = "LogonTimesSource";
            string eventLogLog = "LogonTimesLog";
            try
            {
                eventLog = new EventLog();
                if (!EventLog.Exists(eventLogLog))
                {
                    try
                    {
                        EventLog.CreateEventSource(eventLogSource, eventLogLog);
                    }
                    catch (Exception ex)
                    {
                        WriteError("Create", ex);
                    }
                }
                eventLog.Source = eventLogSource;
                eventLog.Log = eventLogLog;
            }
            catch (Exception ex)
            {
                WriteError("Overall", ex);
            }
        }

        private void WriteError(string source, Exception ex)
        {
            using (StreamWriter outputFile = new StreamWriter(@"C:\Temp\LogonError.txt", true))
            {
                outputFile.WriteLine(source);
                outputFile.WriteLine(ex.Message);
                outputFile.WriteLine(ex.StackTrace);
            }
        }
        #endregion
    }
}
