using System;
using System.Diagnostics;
using System.IO;

namespace LogonTimes.EventLogHandlers
{
    public class EventLogHandler
    {
        private static readonly EventLogHandler instance = new EventLogHandler();
        private EventLog eventLog;

        #region constructors
        private EventLogHandler()
        {
            OpenLog();
        }

        public static EventLogHandler Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region Public methods
        public void WriteToEventLog(string message)
        {
            eventLog.WriteEntry(message);
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
