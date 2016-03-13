using System;
using System.Collections.Generic;

namespace LogonTimes.DataModel
{
    public interface ITestServiceRunningData
    {
        List<EventType> EventTypes { get; }
    }
}