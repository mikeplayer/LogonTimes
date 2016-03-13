using System;
using System.ServiceProcess;
using System.Windows.Forms;
using LogonTimes.UI;
using System.Security.Principal;
using System.Diagnostics;
using LogonTimes.Logging;
using StructureMap;
using LogonTimes.IoC;

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
            Bootstrapper.BootStrap();
            ILogger logger = IocRegistry.GetInstance<ILogger>();
            if (args.Length > 0)
            {
                if (args[0].Equals("/configure"))
                {
                    WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                    bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                    if (!hasAdministrativeRight)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.UseShellExecute = true;
                        startInfo.WorkingDirectory = Environment.CurrentDirectory;
                        startInfo.FileName = Application.ExecutablePath;
                        startInfo.Verb = "runas";
                        try
                        {
                            Process p = Process.Start(startInfo);
                        }
                        catch (Exception ex)
                        {
                            logger.LogException("Error starting configuration process", DebugLevels.Error, ex);
                        }
                        return;
                    }
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
}
