using System;
using System.Collections.Generic;

namespace LogonTimes.DataModel
{
    public interface IDataAccess
    {
        List<Application> Applications { get; }
        List<DayOfWeek> DaysOfWeek { get; }
        List<EventType> EventTypes { get; }
        List<HoursPerDay> HoursPerDays { get; }
        List<LogonTime> LogonTimes { get; }
        List<LogonTimeAllowed> LogonTimesAllowed { get; }
        List<Person> People { get; }
        List<PersonApplication> PersonApplications { get; }
        bool StillWorking { get; }
        List<SystemSettingType> SystemSettingTypes { get; }
        List<TimePeriod> TimePeriods { get; }
        int WorkingItemCount { get; }

        event EventHandler<PersonEventArgs> PersonModificationFinished;

        void AddHourPerDay(HoursPerDay hourPerDay);
        void AddLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
        void AddOrUpdateLogonTime(LogonTime logonTime);
        void AddPerson(Person person);
        void CheckForUpdates();
        void DeleteHourPerDay(HoursPerDay hourPerDay);
        void DeleteLogonTime(LogonTime logonTime);
        void DeleteLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
        DayOfWeek GetDayOfWeek(int dayNumber);
        EventType GetEventType(int eventTypeId);
        HoursPerDay GetHoursPerDay(int hoursPerDayId);
        LogonTime GetLogonTime(int logonTimeId);
        Person GetPerson(int personId);
        SystemSettingDetail GetSystemSettingDetail(string setting);
        TimePeriod GetTimePeriod(int timePeriodId);
        void MakePersonRestricted(Person person);
        void MakePersonUnrestricted(Person person);
        void UpdateHourPerDay(HoursPerDay hourPerDay);
        void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
        void UpdateSystemSettingDetail(SystemSettingDetail setting);
    }
}