using System;
using System.Collections.Generic;
using LogonTimes.DataModel;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.IO;
using System.Management;
using LogonTimes.IoC;

namespace LogonTimes.Applications
{
    public class FileServices : IFileServices
    {
        private IApplicationManagementData dataAccess;

        public FileServices()
        {
            dataAccess = IocRegistry.GetInstance<IApplicationManagementData>();
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
                            if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(installLocation))
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

        public bool HasPermission(Person person, Application application)
        {
            FileSecurity security = File.GetAccessControl(application.ApplicationPath);
            var rules = security.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            foreach (FileSystemAccessRule rule in rules)
            {
                SelectQuery query = new SelectQuery("Win32_UserAccount", string.Format("SID = '{0}'", rule.IdentityReference.Value));
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject userAccount in searcher.Get())
                {
                    if (person.SID.Equals(userAccount["SID"].ToString()))
                    {
                        var accessType = rule.AccessControlType;
                        if ((rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read)
                        {
                            if (accessType.Equals(AccessControlType.Deny))
                            {
                                return false;
                            }
                        }
                    }
                }
                if (person.SID.Equals(rule.IdentityReference.Value))
                {
                    var accessType = rule.AccessControlType;
                    if ((rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read)
                    {
                        if (accessType.Equals(AccessControlType.Deny))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void SetFileSecurity(PersonApplication personApplication, AccessControlType controlType)
        {
            FileSecurity security = File.GetAccessControl(personApplication.Application.ApplicationPath);
            var rules = security.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            foreach (var rule in rules)
            {
                var currentRule = (FileSystemAccessRule)rule;
                var person = dataAccess.PersonForSID(currentRule.IdentityReference.Value);
                var accessType = currentRule.AccessControlType;
            }
        }
    }
}
