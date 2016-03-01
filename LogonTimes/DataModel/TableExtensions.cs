using System.Collections.Generic;
using System.Linq;
using LinqToDB;

namespace LogonTimes.DataModel
{
    #region Day of week
    public partial class DayOfWeek
    {
        public override string ToString()
        {
            return DayName;
        }
    }
    #endregion

    #region Event Type
    public partial class EventType
    {
        public override string ToString()
        {
            return EventTypeName;
        }
    }
    #endregion

    #region Hours per day
    public partial class HoursPerDay
    {
        public override string ToString()
        {
            return string.Format("{0} {1}", Person.LogonName, HoursAllowed);
        }

        public Person Person
        {
            get
            {
                return DataAccess.Instance.GetPerson(PersonId);
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return DataAccess.Instance.GetDayOfWeek(DayNumber);
            }
        }
    }
    #endregion

    #region Logon time
    public partial class LogonTime
    {
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Person.LogonName, EventType.EventTypeName, EventTime);
        }

        public Person Person
        {
            get
            {
                return DataAccess.Instance.GetPerson(PersonId);
            }
        }

        public EventType EventType
        {
            get
            {
                return DataAccess.Instance.GetEventType(EventTypeId);
            }
        }
    }
    #endregion

    #region Logon time allowed
    public partial class LogonTimeAllowed
    {
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Person.LogonName, DayOfWeek.DayName, TimePeriod.PeriodStart.ToString("hh:nn"), Permitted);
        }

        public Person Person
        {
            get
            {
                return DataAccess.Instance.GetPerson(PersonId);
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return DataAccess.Instance.GetDayOfWeek(DayNumber);
            }
        }

        public TimePeriod TimePeriod
        {
            get
            {
                return DataAccess.Instance.GetTimePeriod(TimePeriodId);
            }
        }
    }
    #endregion

    #region Person
    public partial class Person
    {
        public override string ToString()
        {
            return LogonName;
        }

        public List<HoursPerDay> HoursPerDay
        {
            get
            {
                return DataAccess.Instance.HoursPerDays.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<LogonTime> LogonTimes
        {
            get
            {
                return DataAccess.Instance.LogonTimes.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<LogonTimeAllowed> LogonTimesAllowed
        {
            get
            {
                return DataAccess.Instance.LogonTimesAllowed.Where(x => x.PersonId == PersonId).ToList();
            }
        }
    }
    #endregion

    #region Time Period
    public partial class TimePeriod
    {
        public override string ToString()
        {
            return string.Format("{0} to {1}", PeriodStart.ToString("h:mm tt"), PeriodEnd.ToString("h:mm tt"));
        }
    }
    #endregion
}
