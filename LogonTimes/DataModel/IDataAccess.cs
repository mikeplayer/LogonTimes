using System.Collections.Generic;

namespace LogonTimes.DataModel
{
    public interface IDataAccess
    {
        List<DayOfWeek> DaysOfWeek { get; }
        List<EventType> EventTypes { get; }
        List<HoursPerDay> HoursPerDays { get; }
        List<LogonTime> LogonTimes { get; }
        List<LogonTimeAllowed> LogonTimesAllowed { get; }
        List<Person> People { get; }
        List<TimePeriod> TimePeriods { get; }

        void AddHourPerDay(HoursPerDay hourPerDay);
        void AddLogonTime(LogonTime logonTime);
        void AddLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
        void AddPerson(Person person);
        void DeleteHourPerDay(HoursPerDay hourPerDay);
        void DeleteLogonTime(LogonTime logonTime);
        void DeleteLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
        DayOfWeek GetDayOfWeek(int dayNumber);
        EventType GetEventType(int eventTypeId);
        HoursPerDay GetHoursPerDay(int hoursPerDayId);
        Person GetPerson(int personId);
        TimePeriod GetTimePeriod(int timePeriodId);
        void MakePersonRestricted(Person person);
        void MakePersonUnrestricted(Person person);
        void UpdateHourPerDay(HoursPerDay hourPerDay);
        void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
    }
}