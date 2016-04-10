using LogonTimes.DataModel;
using System.Collections.Generic;

namespace LogonTimes.Applications
{
    public interface IApplicationManagement
    {
        IEnumerable<Application> Applications { get; }
        void RestrictAccess(Person person, Application application);
        void UnrestrictAccess(Person person, Application application);
        void CheckApplicationPermissions(Person currentPerson);
        void AddPath(string selectedPath);
    }
}
