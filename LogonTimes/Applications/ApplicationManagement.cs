using System.Collections.Generic;
using LogonTimes.DataModel;
using LogonTimes.IoC;
using Microsoft.Win32;
using System.Linq;
using System;
using System.Security.AccessControl;
using System.IO;

namespace LogonTimes.Applications
{
    public class ApplicationManagement : IApplicationManagement
    {
        private IApplicationManagementData dataAccess;
        private IFileServices fileServices;
        private List<Application> applications;

        public ApplicationManagement()
        {
            dataAccess = IocRegistry.GetInstance<IApplicationManagementData>();
            fileServices = IocRegistry.GetInstance<IFileServices>();
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
            foreach (var application in fileServices.RegisteredApplications)
            {
                if (!Applications.Any(x => x.ApplicationName.Equals(application.ApplicationName) && x.ApplicationPath.Equals(application.ApplicationPath)))
                {
                    dataAccess.AddApplication(application);
                }
            }
        }

        public void RestrictAccess(Person person, Application application)
        {
            PersonApplication personApplication = person.PersonApplications.FirstOrDefault(x => x.ApplicationId.Equals(application.ApplicationId));
            if (personApplication == null)
            {
                personApplication = new PersonApplication
                {
                    PersonId = person.PersonId,
                    ApplicationId = application.ApplicationId,
                    Permitted = false
                };
            }
            else
            {
                personApplication.Permitted = false;
            }
            dataAccess.AddOrUpdatePersonApplication(personApplication);
            fileServices.SetFileSecurity(personApplication, AccessControlType.Deny);
        }

        public void UnrestrictAccess(Person person, Application application)
        {
            if (person == null)
            {
                return;
            }
            PersonApplication personApplication = person.PersonApplications.FirstOrDefault(x => x.ApplicationId.Equals(application.ApplicationId));
            if (personApplication == null)
            {
                return;
            }
            personApplication.Permitted = true;
            dataAccess.AddOrUpdatePersonApplication(personApplication);
            fileServices.SetFileSecurity(personApplication, AccessControlType.Allow);
        }

        public void CheckApplicationPermissions(Person person)
        {
            foreach (var application in Applications)
            {
                var personApplication = application.PersonApplications.FirstOrDefault(x => x.PersonId.Equals(person.PersonId));
                bool permitted = fileServices.HasPermission(person, application);
                if (!permitted && (personApplication == null || personApplication.Permitted))
                {
                    if (personApplication == null)
                    {
                        personApplication = new PersonApplication
                        {
                            PersonId = person.PersonId,
                            ApplicationId = application.ApplicationId,
                        };
                    }
                    personApplication.Permitted = false;
                    dataAccess.AddOrUpdatePersonApplication(personApplication);
                }
                if (permitted && personApplication != null && !personApplication.Permitted)
                {
                    personApplication.Permitted = true;
                    dataAccess.AddOrUpdatePersonApplication(personApplication);
                }
            }
        }
    }
}
