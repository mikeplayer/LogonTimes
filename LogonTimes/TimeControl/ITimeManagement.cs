using System.Collections.Generic;
using Cassia;
using LogonTimes.DataModel;

namespace LogonTimes.TimeControl
{
    public interface ITimeManagement
    {
        List<LogonTimeAllowed> LogonTimesAllowed(string userName);
        void NewSessionEvent(ITerminalServicesSession session, string sessionEvent);
        void UpdateLogins();
    }
}