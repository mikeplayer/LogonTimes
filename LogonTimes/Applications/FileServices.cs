using System.Collections.Generic;
using LogonTimes.DataModel;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.IO;
using LogonTimes.IoC;
using LogonTimes.People;

namespace LogonTimes.Applications
{
    public class FileServices : IFileServices
    {
        private IApplicationManagementData dataAccess;
        private IUserManagement userManagement;

        public FileServices()
        {
            dataAccess = IocRegistry.GetInstance<IApplicationManagementData>();
            userManagement = IocRegistry.GetInstance<IUserManagement>();
        }

        public List<Application> RegisteredApplications
        {
            get
            {
                var applications = new List<Application>();
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
                            if (!string.IsNullOrEmpty(displayName) 
                                && !string.IsNullOrEmpty(installLocation)
                                && Directory.Exists(installLocation))
                            {
                                Application application = new Application
                                {
                                    ApplicationName = displayName,
                                    ApplicationPath = installLocation
                                };
                                applications.Add(application);
                            }
                        }
                    }
                }
                return applications;
            }
        }

        List<FileSystemAccessRule> DenyRules(Person person, Application application)
        {
            var result = new List<FileSystemAccessRule>();
            FileSecurity security = File.GetAccessControl(application.ApplicationPath);
            var rules = security.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            foreach (FileSystemAccessRule rule in rules)
            {
                if (person.SID.Equals(rule.IdentityReference.Value))
                {
                    var accessType = rule.AccessControlType;
                    if (accessType.Equals(AccessControlType.Deny))
                    {
                        result.Add(rule);
                    }
                }
            }
            return result;
        }

        public bool HasPermission(Person person, Application application)
        {
            foreach (var rule in DenyRules(person, application))
            {
                if ((rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetFileSecurity(PersonApplication personApplication, AccessControlType controlType)
        {
            FileSecurity security = File.GetAccessControl(personApplication.Application.ApplicationPath);
            foreach (var rule in DenyRules(personApplication.Person, personApplication.Application))
            {
                if ((rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl)
                {
                    if (controlType.Equals(AccessControlType.Deny))
                    {
                        return;     //Already set how we want it
                    }
                    security.RemoveAccessRule(rule);
                    return;
                }
            }
            //We will either have removed the deny rule or the rule doesn't exist - so we have already handled an Allow case
            string account = string.Format(@"{0}\{1}", userManagement.UserDomain, personApplication.Person.LogonName);
            security.AddAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, controlType));
            File.SetAccessControl(personApplication.Application.ApplicationPath, security);
        }
    }
}
