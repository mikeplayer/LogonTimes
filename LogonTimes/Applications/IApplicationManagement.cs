using LogonTimes.DataModel;
using System.Collections.Generic;

namespace LogonTimes.Applications
{
    public interface IApplicationManagement
    {
        IEnumerable<Application> Applications { get; }
    }
}
