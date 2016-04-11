using LogonTimes.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;

namespace LogonTimes.Applications
{
    public interface IFileServices
    {
        List<Application> RegisteredApplications { get; }

        bool HasPermission(Person person, Application application);
        void SetFileSecurity(PersonApplication personApplication, AccessControlType deny);
    }
}
