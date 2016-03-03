using System;
using System.ServiceProcess;
using System.Windows.Forms;
using LogonTimes.UI;

namespace LogonTimes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("/configure"))
                {
                    Application.EnableVisualStyles();
                    Application.Run(new LogonTimesConfiguration());
                    return;
                }
                if (args[0].Equals("/testservice"))
                {
                    Application.EnableVisualStyles();
                    Application.Run(new TestServiceRunning());
                    return;
                }
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new LogonTimes()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
    /*
    SessionUnlock
    SessionLock
    ConsoleConnect
    ConsoleDisconnect
    SessionLogon
    SessionLogoff
    */
}
