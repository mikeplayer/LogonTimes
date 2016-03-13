using System;
using System.ServiceProcess;
using System.IO;
using Cassia;
using LogonTimes.Logging;
using LogonTimes.TimeControl;
using LogonTimes.IoC;

namespace LogonTimes
{
    public partial class LogonTimes : ServiceBase
    {
        private const string crlf = "\r\n";
        private ITimeManagement timeManagement;
        private ILogger logger;
        System.Timers.Timer timer = new System.Timers.Timer();

        public LogonTimes()
        {
            InitializeComponent();
            logger = IocRegistry.GetInstance<ILogger>();
            timeManagement = IocRegistry.GetInstance<ITimeManagement>();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 60000;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
            timer.Start();
            logger.Log("Start", DebugLevels.Info);
        }

        protected override void OnStop()
        {
            timer.Stop();
            logger.Log("Stop", DebugLevels.Info);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
            ITerminalServicesManager manager = new TerminalServicesManager();
            using (ITerminalServer server = manager.GetLocalServer())
            {
                server.Open();
                ITerminalServicesSession session = server.GetSession(changeDescription.SessionId);
                timeManagement.NewSessionEvent(session, changeDescription.Reason.ToString());
            }
        }
    }
}
