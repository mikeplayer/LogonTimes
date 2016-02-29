using System;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LogonTimes
{

    //[DllImport("wtsapi32.dll", SetLastError = true)]
    //static extern bool WTSSendMessage(
    //            IntPtr hServer,
    //            [MarshalAs(UnmanagedType.I4)] int SessionId,
    //            String pTitle,
    //            [MarshalAs(UnmanagedType.U4)] int TitleLength,
    //            String pMessage,
    //            [MarshalAs(UnmanagedType.U4)] int MessageLength,
    //            [MarshalAs(UnmanagedType.U4)] int Style,
    //            [MarshalAs(UnmanagedType.U4)] int Timeout,
    //            [MarshalAs(UnmanagedType.U4)] out int pResponse,
    //            bool bWait);

    [DllImport("Wtsapi32.dll", SetLastError = true)]
    static extern bool WTSQuerySessionInformation(
                IntPtr hServer,
                uint sessionId,
                WTS_INFO_CLASS wtsInfoClass,
                out IntPtr ppBuffer,
                out uint pBytesReturned
            );
    public partial class LogonTimes : ServiceBase
    {
        private EventLog eventLog;

        public LogonTimes()
        {
            string eventLogSource = "LogonTimesSource";
            string eventLogLog = "LogonTimesLog";

            InitializeComponent();
            try
            {
                //try
                //{
                //    EventLog.DeleteEventSource(eventLogSource, "Application");
                //}
                //catch (Exception ex)
                //{
                //    WriteError("Delete", ex);
                //}
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

        protected override void OnStart(string[] args)
        {
            timer.Enabled = true;
            eventLog.WriteEntry("Start");
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            eventLog.WriteEntry("Stop");
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
            eventLog.WriteEntry(string.Format("Change description: {0}", changeDescription.ToString()));
            eventLog.WriteEntry(string.Format("Change Reason: {0}", changeDescription.Reason.ToString()));
            eventLog.WriteEntry(string.Format("Change SessionId: {0}", changeDescription.SessionId.ToString()));
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }
    }
}
