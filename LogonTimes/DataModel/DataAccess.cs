using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace LogonTimes.DataModel
{
    public class DataAccess : IDataAccess
    {
        private List<DayOfWeek> daysOfWeek;
        private List<EventType> eventTypes;
        private List<HoursPerDay> hoursPerDays;
        private List<LogonTime> logonTimes;
        private List<LogonTimeAllowed> logonTimeAlloweds;
        private List<Person> persons;
        private List<TimePeriod> timePeriods;
        private static readonly DataAccess instance = new DataAccess();

        private DataAccess() { }

        public static DataAccess Instance
        {
            get
            {
                return instance;
            }
        }

        #region Days of week
        public List<DayOfWeek> DaysOfWeek
        {
            get
            {
                if (daysOfWeek == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        daysOfWeek = (from day in db.DayOfWeeks select day).OrderBy(x => x.SortOrder).ToList();
                    }
                }
                return daysOfWeek;
            }
        }

        public DayOfWeek GetDayOfWeek(int dayNumber)
        {
            if (!DaysOfWeek.Any(x => x.DayNumber == dayNumber))
            {
                return null;
            }
            return DaysOfWeek.First(x => x.DayNumber == dayNumber);
        }
        #endregion

        #region Event types
        public List<EventType> EventTypes
        {
            get
            {
                if (eventTypes == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        eventTypes = (from eventType in db.EventTypes select eventType).ToList();
                    }
                }
                return eventTypes;
            }
        }

        public EventType GetEventType(int eventTypeId)
        {
            if (!EventTypes.Any(x => x.EventTypeId == eventTypeId))
            {
                return null;
            }
            return EventTypes.First(x => x.EventTypeId == eventTypeId);
        }
        #endregion

        #region Hours per day
        public List<HoursPerDay> HoursPerDays
        {
            get
            {
                if (hoursPerDays == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        hoursPerDays = (from hoursPerDay in db.HoursPerDays select hoursPerDay).ToList();
                    }
                }
                return hoursPerDays;
            }
        }

        public HoursPerDay GetHoursPerDay(int hoursPerDayId)
        {
            if (!HoursPerDays.Any(x => x.HoursPerDayId == hoursPerDayId))
            {
                return null;
            }
            return HoursPerDays.First(x => x.HoursPerDayId == hoursPerDayId);
        }

        public void AddHourPerDay(HoursPerDay hourPerDay)
        {
            using (var db = new LogonTimesDB())
            {
                hourPerDay.HoursPerDayId = Convert.ToInt32(db.InsertWithIdentity(hourPerDay));
                HoursPerDays.Add(hourPerDay);
            }
        }

        public void UpdateHourPerDay(HoursPerDay hourPerDay)
        {
            using (var db = new LogonTimesDB())
            {
                db.Update(hourPerDay);
            }
        }

        public void DeleteHourPerDay(HoursPerDay hourPerDay)
        {
            using (var db = new LogonTimesDB())
            {
                db.Delete(hourPerDay);
                HoursPerDays.Remove(hourPerDay);
            }
        }
        #endregion

        #region Logon times
        public List<LogonTime> LogonTimes
        {
            get
            {
                if (logonTimes == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        logonTimes = (from logonTime in db.LogonTimes select logonTime).ToList();
                    }
                }
                return logonTimes;
            }
        }

        public void AddLogonTime(LogonTime logonTime)
        {
            using (var db = new LogonTimesDB())
            {
                if (LogonTimes.Any(x => x.LogonTimeId == logonTime.LogonTimeId))
                {
                    db.Update(logonTime);
                    LogonTimes.Remove(logonTime);
                }
                else
                {
                    logonTime.LogonTimeId = Convert.ToInt32(db.InsertWithIdentity(logonTime));
                }
                LogonTimes.Add(logonTime);
            }
        }

        public void DeleteLogonTime(LogonTime logonTime)
        {
            using (var db = new LogonTimesDB())
            {
                db.Delete(logonTime);
                LogonTimes.Remove(logonTime);
            }
        }

        public LogonTime GetLogonTime(int logonTimeId)
        {
            if (!LogonTimes.Any(x => x.LogonTimeId == logonTimeId))
            {
                return null;
            }
            return LogonTimes.First(x => x.LogonTimeId == logonTimeId);
        }
        #endregion

        #region Logon Times Allowed
        public List<LogonTimeAllowed> LogonTimesAllowed
        {
            get
            {
                if (logonTimeAlloweds == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        logonTimeAlloweds = (from logonTimeAllowed in db.LogonTimeAlloweds select logonTimeAllowed).ToList();
                    }
                }
                return logonTimeAlloweds;
            }
        }

        public void AddLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            using (var db = new LogonTimesDB())
            {
                logonTimeAllowed.LogonTimeAllowedId = Convert.ToInt32(db.InsertWithIdentity(logonTimeAllowed));
                LogonTimesAllowed.Add(logonTimeAllowed);
            }
        }

        public void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            using (var db = new LogonTimesDB())
            {
                db.Update(logonTimeAllowed);
            }
        }

        public void DeleteLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            using (var db = new LogonTimesDB())
            {
                db.Delete(logonTimeAllowed);
                LogonTimesAllowed.Remove(logonTimeAllowed);
            }
        }
        #endregion

        #region Person
        public List<Person> People
        {
            get
            {
                if (persons == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        persons = (from person in db.People select person).ToList();
                    }
                }
                return persons;
            }
        }

        public void AddPerson(Person person)
        {
            using (var db = new LogonTimesDB())
            {
                person.PersonId = Convert.ToInt32(db.InsertWithIdentity(person));
                People.Add(person);
            }
        }

        public void MakePersonRestricted(Person person)
        {
            using (var db = new LogonTimesDB())
            {
                if (!person.HoursPerDay.Any())
                {
                    var hoursPerDayList = new List<HoursPerDay>();
                    foreach (var day in DaysOfWeek)
                    {
                        var hoursPerDay = new HoursPerDay
                        {
                            PersonId = person.PersonId,
                            DayNumber = day.DayNumber
                        };
                        hoursPerDayList.Add(hoursPerDay);
                    }
                    db.BulkCopy(hoursPerDayList);
                    hoursPerDays = null;    //Make sure it gets reloaded after the bulk copy
                }
                if (!person.LogonTimesAllowed.Any())
                {
                    var logonTimeAllowedList = new List<LogonTimeAllowed>();
                    foreach (var day in DaysOfWeek)
                    {
                        foreach (var timePeriod in TimePeriods)
                        {
                            var loginTimeAllowed = new LogonTimeAllowed
                            {
                                PersonId = person.PersonId,
                                DayNumber = day.DayNumber,
                                TimePeriodId = timePeriod.TimePeriodId,
                                Permitted = true
                            };
                            logonTimeAllowedList.Add(loginTimeAllowed);
                        }
                    }
                    db.BulkCopy(logonTimeAllowedList);
                    logonTimeAlloweds = null;    //Make sure it gets reloaded after the bulk copy
                }
            }
        }

        public void MakePersonUnrestricted(Person person)
        {
            using (var db = new LogonTimesDB())
            {
                if (person.HoursPerDay.Any())
                {
                    db.HoursPerDays
                        .Where(x => x.PersonId == person.PersonId)
                        .Delete();
                    hoursPerDays = null;    //Make sure it gets reloaded after the bulk delete
                }
                if (person.LogonTimesAllowed.Any())
                {
                    db.LogonTimeAlloweds
                        .Where(x => x.PersonId == person.PersonId)
                        .Delete();
                    logonTimeAlloweds = null;   //Make sure it gets reloaded after the bulk delete
                }
            }
        }

        public Person GetPerson(int personId)
        {
            if (!People.Any(x => x.PersonId == personId))
            {
                return null;
            }
            return People.First(x => x.PersonId == personId);
        }
        #endregion

        #region Time periods
        public List<TimePeriod> TimePeriods
        {
            get
            {
                if (timePeriods == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        timePeriods = (from timePeriod in db.TimePeriods select timePeriod).ToList();
                    }
                }
                return timePeriods;
            }
        }

        public TimePeriod GetTimePeriod(int timePeriodId)
        {
            if (!TimePeriods.Any(x => x.TimePeriodId == timePeriodId))
            {
                return null;
            }
            return TimePeriods.First(x => x.TimePeriodId == timePeriodId);
        }
        #endregion
    }
}
