using System;
using System.Collections.Generic;

namespace LogonTimes.DataModel
{
    public interface ILogonTimesConfigurationData
    {
        List<DayOfWeek> DaysOfWeek { get; }
        bool StillWorking { get; }
        List<TimePeriod> TimePeriods { get; }
        void UpdateSystemSettingDetail(SystemSettingDetail setting);
    }
}