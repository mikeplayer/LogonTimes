using System.Collections.Generic;
using System.Linq;
using System.Management;
using LogonTimes.DataModel;

namespace LogonTimes
{
    public class UserManagement
    {
        public UserManagement()
        {
        }

        public List<Person> LoadPeople()
        {
            var people = DataAccess.People;
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
                    DataAccess.AddPerson(person);
                }
            }
            return people.ToList();
        }

        public Person GetPersonDetail(string personName)
        {
            if (DataAccess.People.Any(x => x.LogonName.Equals(personName)))
            {
                return DataAccess.People.First(x => x.LogonName.Equals(personName));
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
            DataAccess.MakePersonRestricted(person);
        }

        public void SetPersonToUnrestricted(string personName)
        {
            var person = GetPersonDetail(personName);
            DataAccess.MakePersonUnrestricted(person);
        }

        public void UpdateHoursPerDay(int hoursPerDayId, float? newValue)
        {
            var hoursAllowed = DataAccess.GetHoursPerDay(hoursPerDayId);
            hoursAllowed.HoursAllowed = newValue;
            DataAccess.UpdateHourPerDay(hoursAllowed);
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
