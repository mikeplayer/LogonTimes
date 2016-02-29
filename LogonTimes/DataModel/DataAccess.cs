using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;

namespace LogonTimes.DataModel
{
    public static class DataAccess
    {
        private static List<DayOfWeek> daysOfWeek;
        private static List<EventType> eventTypes;
        private static List<HoursPerDay> hoursPerDays;
        private static List<LogonTime> logonTimes;
        private static List<LogonTimeAllowed> logonTimeAlloweds;
        private static List<Person> persons;
        private static List<TimePeriod> timePeriods;

        #region Days of week
        public static List<DayOfWeek> DaysOfWeek
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

        public static DayOfWeek GetDayOfWeek(int dayNumber)
        {
            if (!DaysOfWeek.Any(x => x.DayNumber == dayNumber))
            {
                return null;
            }
            return DaysOfWeek.First(x => x.DayNumber == dayNumber);
        }
        #endregion

        #region Event types
        public static List<EventType> EventTypes
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

        public static EventType GetEventType(int eventTypeId)
        {
            if (!EventTypes.Any(x => x.EventTypeId == eventTypeId))
            {
                return null;
            }
            return EventTypes.First(x => x.EventTypeId == eventTypeId);
        }
        #endregion

        #region Hours per day
        public static List<HoursPerDay> HoursPerDays
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

        public static HoursPerDay GetHoursPerDay(int hoursPerDayId)
        {
            if (!HoursPerDays.Any(x => x.HoursPerDayId == hoursPerDayId))
            {
                return null;
            }
            return HoursPerDays.First(x => x.HoursPerDayId == hoursPerDayId);
        }

        public static void AddHourPerDay(HoursPerDay hourPerDay)
        {
            using (var db = new LogonTimesDB())
            {
                hourPerDay.HoursPerDayId = Convert.ToInt32(db.InsertWithIdentity(hourPerDay));
                HoursPerDays.Add(hourPerDay);
            }
        }

        public static void UpdateHourPerDay(HoursPerDay hourPerDay)
        {
            using (var db = new LogonTimesDB())
            {
                db.Update(hourPerDay);
            }
        }

        public static void DeleteHourPerDay(HoursPerDay hourPerDay)
        {
            using (var db = new LogonTimesDB())
            {
                db.Delete(hourPerDay);
                HoursPerDays.Remove(hourPerDay);
            }
        }
        #endregion

        #region Logon times
        public static List<LogonTime> LogonTimes
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

        public static void AddLogonTime(LogonTime logonTime)
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

        public static void DeleteLogonTime(LogonTime logonTime)
        {
            using (var db = new LogonTimesDB())
            {
                db.Delete(logonTime);
                LogonTimes.Remove(logonTime);
            }
        }
        #endregion

        #region Logon Times Allowed
        public static List<LogonTimeAllowed> LogonTimesAllowed
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

        public static void AddLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            using (var db = new LogonTimesDB())
            {
                logonTimeAllowed.LogonTimeAllowedId = Convert.ToInt32(db.InsertWithIdentity(logonTimeAllowed));
                LogonTimesAllowed.Add(logonTimeAllowed);
            }
        }

        public static void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            using (var db = new LogonTimesDB())
            {
                db.Update(logonTimeAllowed);
            }
        }

        public static void DeleteLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            using (var db = new LogonTimesDB())
            {
                db.Delete(logonTimeAllowed);
                LogonTimesAllowed.Remove(logonTimeAllowed);
            }
        }
        #endregion

        #region Person
        public static List<Person> People
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

        public static void AddPerson(Person person)
        {
            using (var db = new LogonTimesDB())
            {
                person.PersonId = Convert.ToInt32(db.InsertWithIdentity(person));
                People.Add(person);
            }
        }

        public static void MakePersonRestricted(Person person)
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

        public static void MakePersonUnrestricted(Person person)
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

        public static Person GetPerson(int personId)
        {
            if (!People.Any(x => x.PersonId == personId))
            {
                return null;
            }
            return People.First(x => x.PersonId == personId);
        }
        #endregion

        #region Time periods
        public static List<TimePeriod> TimePeriods
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

        public static TimePeriod GetTimePeriod(int timePeriodId)
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
