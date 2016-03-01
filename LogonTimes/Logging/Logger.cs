using System;
using System.Diagnostics;
using System.IO;

namespace LogonTimes.Logging
{
    public class Logger
    {
        private static readonly Logger instance = new Logger();
        private EventLog eventLog;
        private int currentLogLevel;

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
        #endregion

        #region Public methods
        public void Log(string message, DebugLevels debugLevel)
        {
            if (ShouldLog(debugLevel))
            {
                eventLog.WriteEntry(message);
            }
        }

        public bool ShouldLog(DebugLevels debugLevel)
        {
            return (currentLogLevel >= (int)debugLevel);
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
