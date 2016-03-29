using LogonTimes.DataModel;
using System.Collections.Generic;

namespace LogonTimes.Applications
{
    public interface IApplicationManagement
    {
        IEnumerable<Application> Applications { get; }

        void AddApplication(string applicationName, string applicationPath);

        void RestrictAccess(Person person, Application application);

        void UnrestrictAccess(Person person, Application application);
    }
}
