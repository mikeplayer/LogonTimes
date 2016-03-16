using System.Collections.Generic;
using LogonTimes.DataModel;

namespace LogonTimes.TimeControl
{
    public interface IEventManagement
    {
        LogonTime CurrentEvent { get; set; }
        Person CurrentPerson { get; set; }
        List<LogonTime> LogonTimesToday { get; }

        void UpdateCurrentEvent(int? newEventId);
        void CreateCurrentEvent(int eventTypeId);
        void LoadLogonTimes();
    }
}