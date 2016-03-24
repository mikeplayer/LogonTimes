using System;
using System.Collections.Generic;
using LogonTimes.DataModel;
using LogonTimes.IoC;

namespace LogonTimes.Applications
{
    public class ApplicationManagement : IApplicationManagement
    {
        private IApplicationManagementData dataAccess;

        public ApplicationManagement()
        {
            dataAccess = IocRegistry.GetInstance<IApplicationManagementData>();
        }

        public IEnumerable<Application> Applications
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
