using System;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;
using Cassia;
using System.Text;

namespace LogonTimes
{
    public partial class LogonTimes : ServiceBase
    {

        private EventLog eventLog;
        private const string crlf = "\r\n";
        private TimeManagement timeManagement = new TimeManagement();
        System.Timers.Timer timerx = new System.Timers.Timer();

        public LogonTimes()
        {
            string eventLogSource = "LogonTimesSource";
            string eventLogLog = "LogonTimesLog";

            InitializeComponent();
            timerx.Elapsed += Timerx_Elapsed;
            timerx.Interval = 60000;
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
                timeManagement.EventLog = eventLog;
            }
            catch (Exception ex)
            {
                WriteError("Overall", ex);
            }
        }

        private void Timerx_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeManagement.UpdateLogins();
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
            timerx.Start();
            eventLog.WriteEntry("Start");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeManagement.UpdateLogins();
        }

        protected override void OnStop()
        {
            timerx.Stop();
            eventLog.WriteEntry("Stop");
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
            ITerminalServicesManager manager = new TerminalServicesManager();
            using (ITerminalServer server = manager.GetLocalServer())
            {
                server.Open();
                ITerminalServicesSession session = server.GetSession(changeDescription.SessionId);
                //var message = new StringBuilder();
                //message.Append("User: ");
                //message.Append(session.UserName);
                //message.Append(crlf);
                //message.Append("Change Reason: ");
                //message.Append(changeDescription.Reason.ToString());
                //eventLog.WriteEntry(message.ToString());
                timeManagement.NewSessionEvent(session, changeDescription.Reason.ToString());
            }
        }
    }
}
