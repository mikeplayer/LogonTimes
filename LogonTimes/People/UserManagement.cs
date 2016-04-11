using System.Collections.Generic;
using System.Linq;
using System.Management;
using LogonTimes.DataModel;
using System;
using System.Threading;
using LogonTimes.IoC;

namespace LogonTimes.People
{
    public class UserManagement : IUserManagement
    {
        public event EventHandler<PersonEventArgs> PersonLoadedFromUserAccount;
        public event EventHandler PersonLoadingComplete;
        private List<Person> peopleBeingModified = new List<Person>();
        private IUserManagementData dataAccess;

        public UserManagement()
        {
            dataAccess = IocRegistry.GetInstance<IUserManagementData>();
            dataAccess.PersonModificationFinished += DataAccessPersonModificationFinished;
        }

        private void DataAccessPersonModificationFinished(object sender, PersonEventArgs e)
        {
            peopleBeingModified.Remove(e.Person);
        }

        public List<Person> LoadPeople()
        {
            var people = dataAccess.People;
            Thread loadPersonThread = new Thread(AddNewPeople);
            loadPersonThread.IsBackground = true;
            loadPersonThread.Start();
            return people.ToList();
        }

        private void AddNewPeople()
        {
            var people = dataAccess.People;
            SelectQuery query = new SelectQuery("Win32_UserAccount", string.Format("Domain = '{0}'", Environment.MachineName));
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject userAccount in searcher.Get())
            {
                string personName = userAccount["Name"].ToString();
                var found = people.Any(x => x.LogonName.Equals(personName));
                if (!found)
                {
                    Person person = new Person()
                    {
                        LogonName = personName,
                        SID = userAccount["SID"].ToString()
                    };
                    dataAccess.AddPerson(person);
                    EventHandler<PersonEventArgs> handler = PersonLoadedFromUserAccount;
                    if (handler != null)
                    {
                        handler(this, new PersonEventArgs(person));
                    }
                }
            }
            EventHandler completeHandler = PersonLoadingComplete;
            if (completeHandler != null)
            {
                completeHandler(this, new EventArgs());
            }
        }

        public void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            dataAccess.UpdateLogonTimeAllowed(logonTimeAllowed);
        }

        public Person GetPersonDetail(string personName)
        {
            if (dataAccess.People.Any(x => x.LogonName.Equals(personName)))
            {
                var result = dataAccess.People.First(x => x.LogonName.Equals(personName));
                while (peopleBeingModified.Any(x => x.PersonId == result.PersonId))
                {
                    Thread.Sleep(100);
                }
                return result;
            }
            return null;
        }

        public bool PersonIsRestricted(Person person)
        {
            if (person.HoursPerDay.Any() || person.LogonTimesAllowed.Any())
            {
                return true;
            }
            return false;
        }

        public bool PersonIsRestricted(string personName)
        {
            var person = GetPersonDetail(personName);
            return PersonIsRestricted(person);
        }

        public void SetPersonToRestricted(string personName)
        {
            var person = GetPersonDetail(personName);
            peopleBeingModified.Add(person);
            dataAccess.MakePersonRestricted(person);
        }

        public void SetPersonToUnrestricted(string personName)
        {
            var person = GetPersonDetail(personName);
            peopleBeingModified.Add(person);
            dataAccess.MakePersonUnrestricted(person);
        }

        public void UpdateHoursPerDay(int hoursPerDayId, float? newValue)
        {
            var hoursAllowed = dataAccess.GetHoursPerDay(hoursPerDayId);
            hoursAllowed.HoursAllowed = newValue;
            dataAccess.UpdateHourPerDay(hoursAllowed);
        }

        public List<HoursPerDay> HoursPerDayForPerson(Person person)
        {
            if (person == null)
            {
                return null;
            }
            return person.HoursPerDay;
        }

        //public List<HoursPerDay> HoursPerDayForPerson(string personName)
        //{
        //    return HoursPerDayForPerson(GetPersonDetail(personName));
        //}

        //public List<LogonTimeAllowed> LogonTimesAllowed(string userName)
        //{
        //    return LogonTimesAllowed(GetPersonDetail(userName));
        //}

        public List<LogonTimeAllowed> LogonTimesAllowed(Person person)
        {
            return person.LogonTimesAllowed
                .OrderBy(x => x.DayNumber)
                .ThenBy(x => x.TimePeriod.PeriodStart)
                .ToList();
        }
    }
}
