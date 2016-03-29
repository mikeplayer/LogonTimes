using System.Collections.Generic;
using LogonTimes.DataModel;
using LogonTimes.IoC;
using Microsoft.Win32;
using System.Linq;
using System;

namespace LogonTimes.Applications
{
    public class ApplicationManagement : IApplicationManagement
    {
        private IApplicationManagementData dataAccess;
        private List<Application> applications;

        public ApplicationManagement()
        {
            dataAccess = IocRegistry.GetInstance<IApplicationManagementData>();
        }

        public IEnumerable<Application> Applications
        {
            get
            {
                if (applications == null)
                {
                    applications = dataAccess.Applications;
                    LoadRegisteredApplications();
                }
                return applications;
            }
        }

        private void LoadRegisteredApplications()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string displayName = null;
                        string installLocation = null;
                        var displayNameItem = subkey.GetValue("DisplayName");
                        if (displayNameItem != null)
                        {
                            displayName = displayNameItem.ToString();
                        }
                        var installLocationItem = subkey.GetValue("InstallLocation");
                        if (installLocationItem != null)
                        {
                            installLocation = installLocationItem.ToString();
                        }
                        if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(installLocation))
                        {
                            if (!Applications.Any(x => x.ApplicationName.Equals(displayName) && x.ApplicationPath.Equals(installLocation)))
                            {
                                AddApplication(displayName, installLocation);
                            }
                        }
                    }
                }
            }
        }

        public void AddApplication(string applicationName, string applicationPath)
        {
            Application application = new Application
            {
                ApplicationName = applicationName,
                ApplicationPath = applicationPath
            };
            dataAccess.AddApplication(application);
        }

        public void RestrictAccess(Person person, Application application)
        {
            PersonApplication personApplication = person.PersonApplications.First(x => x.ApplicationId.Equals(application.ApplicationId));
            if (personApplication == null)
            {
                personApplication = new PersonApplication
                {
                    PersonId = person.PersonId,
                    ApplicationId = application.ApplicationId,
                    Permitted = false
                };
                dataAccess.AddOrUpdatePersonApplication(personApplication);
            }
        }

        public void UnrestrictAccess(Person person, Application application)
        {
            PersonApplication personApplication = person.PersonApplications.First(x => x.ApplicationId.Equals(application.ApplicationId));
            if (personApplication == null)
            {
                return;
            }
            personApplication.Permitted = true;
            dataAccess.AddOrUpdatePersonApplication(personApplication);
        }
    }
}
