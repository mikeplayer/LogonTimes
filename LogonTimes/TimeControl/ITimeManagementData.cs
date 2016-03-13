using LogonTimes.DataModel;
using System;
using System.Collections.Generic;

namespace LogonTimes.TimeControl
{
    public interface ITimeManagementData
    {
        List<EventType> EventTypes { get; }
        List<LogonTime> LogonTimes { get; }
        void AddOrUpdateLogonTime(LogonTime logonTime);
        void CheckForUpdates();
        EventType GetEventType(int eventTypeId);
    }
}