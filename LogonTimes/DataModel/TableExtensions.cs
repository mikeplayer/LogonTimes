using System.Collections.Generic;
using System.Linq;
using LinqToDB;

namespace LogonTimes.DataModel
{
    #region Hours per day
    public partial class HoursPerDay
    {
        public Person Person
        {
            get
            {
                return DataAccess.GetPerson(PersonId);
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return DataAccess.GetDayOfWeek(DayNumber);
            }
        }
    }
    #endregion

    #region Logon time
    public partial class LogonTime
    {
        public Person Person
        {
            get
            {
                return DataAccess.GetPerson(PersonId);
            }
        }

        public EventType EventType
        {
            get
            {
                return DataAccess.GetEventType(EventTypeId);
            }
        }
    }
    #endregion

    #region Logon time allowed
    public partial class LogonTimeAllowed
    {
        public Person Person
        {
            get
            {
                return DataAccess.GetPerson(PersonId);
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return DataAccess.GetDayOfWeek(DayNumber);
            }
        }

        public TimePeriod TimePeriod
        {
            get
            {
                return DataAccess.GetTimePeriod(TimePeriodId);
            }
        }
    }
    #endregion

    #region Person
    public partial class Person
    {
        public List<HoursPerDay> HoursPerDay
        {
            get
            {
                return DataAccess.HoursPerDays.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<LogonTime> LogonTimes
        {
            get
            {
                return DataAccess.LogonTimes.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<LogonTimeAllowed> LogonTimesAllowed
        {
            get
            {
                return DataAccess.LogonTimesAllowed.Where(x => x.PersonId == PersonId).ToList();
            }
        }
    }
    #endregion
}
