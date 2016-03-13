using System.Collections.Generic;
using Cassia;
using LogonTimes.DataModel;

namespace LogonTimes.TimeControl
{
    public interface ITimeManagement
    {
        void NewSessionEvent(ITerminalServicesSession session, string sessionEvent);
        void UpdateLogins();
    }
}