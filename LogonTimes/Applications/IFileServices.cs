using LogonTimes.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogonTimes.Applications
{
    public interface IFileServices
    {
        List<Application> RegisteredApplications { get; }

        bool HasPermission(Person person, Application application);
    }
}
