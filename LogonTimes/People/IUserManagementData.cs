using LogonTimes.DataModel;
using System;
using System.Collections.Generic;

namespace LogonTimes.People
{
    public interface IUserManagementData
    {
        List<Person> People { get; }
        event EventHandler<PersonEventArgs> PersonModificationFinished;
        void AddPerson(Person person);
        HoursPerDay GetHoursPerDay(int hoursPerDayId);
        void MakePersonRestricted(Person person);
        void MakePersonUnrestricted(Person person);
        void UpdateHourPerDay(HoursPerDay hourPerDay);
        void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
    }
}