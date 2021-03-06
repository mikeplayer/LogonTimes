﻿using System;
using System.Collections.Generic;
using LogonTimes.DataModel;

namespace LogonTimes.People
{
    public interface IUserManagement
    {
        event EventHandler<PersonEventArgs> PersonLoadedFromUserAccount;
        event EventHandler PersonLoadingComplete;

        string UserDomain { get; }
        Person GetPersonDetail(string personName);
        List<HoursPerDay> HoursPerDayForPerson(Person person);
        //List<HoursPerDay> HoursPerDayForPerson(string personName);
        List<Person> LoadPeople();
        bool PersonIsRestricted(string personName);
        bool PersonIsRestricted(Person person);
        void SetPersonToRestricted(string personName);
        void SetPersonToUnrestricted(string personName);
        void UpdateHoursPerDay(int hoursPerDayId, float? newValue);
        void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed);
        List<LogonTimeAllowed> LogonTimesAllowed(Person person);
        //List<LogonTimeAllowed> LogonTimesAllowed(string userName);
    }
}