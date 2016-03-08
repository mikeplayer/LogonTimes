using System.Collections.Generic;
using System.Linq;
using System.Management;
using LogonTimes.DataModel;
using System;
using System.Threading;

namespace LogonTimes
{
    public class UserManagement
    {
        public event EventHandler<PersonLoadedEventArgs> PersonLoadedFromUserAccount;
        public event EventHandler PersonLoadingComplete;
        private List<Person> peopleBeingModified = new List<Person>();

        public UserManagement()
        {
            DataAccess.Instance.PersonModificationFinished += DataAccessPersonModificationFinished;
        }

        private void DataAccessPersonModificationFinished(object sender, PersonLoadedEventArgs e)
        {
            peopleBeingModified.Remove(e.PersonLoaded);
        }

        public List<Person> LoadPeople()
        {
            var people = DataAccess.Instance.People;
            Thread loadPersonThread = new Thread(AddNewPeople);
            loadPersonThread.IsBackground = true;
            loadPersonThread.Start();
            return people.ToList();
        }

        private void AddNewPeople()
        {
            var people = DataAccess.Instance.People;
            SelectQuery query = new SelectQuery("Win32_UserAccount");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject userAccount in searcher.Get())
            {
                string personName = userAccount["Name"].ToString();
                var found = people.Any(x => x.LogonName.Equals(personName));
                if (!found)
                {
                    Person person = new Person();
                    person.LogonName = personName;
                    DataAccess.Instance.AddPerson(person);
                    EventHandler<PersonLoadedEventArgs> handler = PersonLoadedFromUserAccount;
                    if (handler != null)
                    {
                        handler(this, new PersonLoadedEventArgs(person));
                    }
                }
            }
            EventHandler completeHandler = PersonLoadingComplete;
            if (completeHandler != null)
            {
                completeHandler(this, new EventArgs());
            }
        }

        public Person GetPersonDetail(string personName)
        {
            if (DataAccess.Instance.People.Any(x => x.LogonName.Equals(personName)))
            {
                var result = DataAccess.Instance.People.First(x => x.LogonName.Equals(personName));
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
            DataAccess.Instance.MakePersonRestricted(person);
        }

        public void SetPersonToUnrestricted(string personName)
        {
            var person = GetPersonDetail(personName);
            peopleBeingModified.Add(person);
            DataAccess.Instance.MakePersonUnrestricted(person);
        }

        public void UpdateHoursPerDay(int hoursPerDayId, float? newValue)
        {
            var hoursAllowed = DataAccess.Instance.GetHoursPerDay(hoursPerDayId);
            hoursAllowed.HoursAllowed = newValue;
            DataAccess.Instance.UpdateHourPerDay(hoursAllowed);
        }

        public List<HoursPerDay> HoursPerDayForPerson(string personName)
        {
            var person = GetPersonDetail(personName);
            if (person == null)
            {
                return null;
            }
            return person.HoursPerDay;
        }
    }
}
