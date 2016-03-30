using System.Collections.Generic;
using LogonTimes.DataModel;

namespace LogonTimes.Applications
{
    public interface IApplicationManagementData
    {
        List<Application> Applications { get; }
        void AddApplication(Application application);
        void AddOrUpdatePersonApplication(PersonApplication personApplication);
        Person PersonForSID(string value);
    }
}
