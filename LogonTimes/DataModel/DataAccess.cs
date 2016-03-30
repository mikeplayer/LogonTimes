using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LogonTimes.Logging;
using System.Threading;
using LogonTimes.IoC;
using LogonTimes.TimeControl;
using LogonTimes.People;
using LogonTimes.DateHandling;
using LogonTimes.Applications;

namespace LogonTimes.DataModel
{
    public class DataAccess : IDataAccess
        , ITestServiceRunningData
        , ILogonTimesConfigurationData
        , IWorkingItemsData
        , ITimeManagementData
        , IUserManagementData
        , IApplicationManagementData
    {
        private IDates dates;
        private List<Application> applications;
        private List<DayOfWeek> daysOfWeek;
        private List<EventType> eventTypes;
        private List<HoursPerDay> hoursPerDays;
        private List<LogonTime> logonTimes;
        private List<LogonTimeAllowed> logonTimeAlloweds;
        private List<Person> persons;
        private List<PersonApplication> personApplications;
        private List<SystemSettingType> systemSettingTypes;
        private List<TimePeriod> timePeriods;
        public event EventHandler<PersonEventArgs> PersonModificationFinished;
        private List<Person> peopleToBeRestricted = new List<Person>();
        private object restrictedLock = new object();
        private List<Person> peopleToBeUnrestricted = new List<Person>();
        private object unrestrictedLock = new object();
        private List<LogonTimeAllowed> logonTimeAllowedForUpdate = new List<LogonTimeAllowed>();
        private object logonTimeLock = new object();
        private ILogger logger;
        public readonly DateTime DateStarted;

        public DataAccess()
        {
            logger = IocRegistry.GetInstance<ILogger>();
            dates = IocRegistry.GetInstance<IDates>();
            DateStarted = dates.Now;
        }

        public void CheckForUpdates()
        {
            var configUpdated = SystemSettingTypesEnum.ConfigurationChanged.Detail();
            if (configUpdated.BoolValue.Value)
            {
                logger.Log("Configuration changed - refreshing data", DebugLevels.Debug);
                applications = null;
                daysOfWeek = null;
                eventTypes = null;
                hoursPerDays = null;
                logonTimes = null;
                logonTimeAlloweds = null;
                persons = null;
                personApplications = null;
                systemSettingTypes = null;
                timePeriods = null;
                configUpdated.SystemSetting = false.ToString();
                UpdateSystemSettingDetail(configUpdated);
            }
        }

        public bool StillWorking
        {
            get
            {
                return (WorkingItemCount > 0);
            }
        }

        public int WorkingItemCount
        {
            get
            {
                return peopleToBeRestricted.Count + peopleToBeUnrestricted.Count + logonTimeAllowedForUpdate.Count;
            }
        }

        #region Applications
        public List<Application> Applications
        {
            get
            {
                if (applications == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        applications = (from application in db.Applications select application).ToList();
                    }
                }
                return applications;
            }
        }

        public void AddApplication(Application application)
        {
            using (var db = new LogonTimesDB())
            {
                application.ApplicationId = Convert.ToInt32(db.InsertWithIdentity(application));
                Applications.Add(application);
            }
        }
        #endregion Applications

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

        public void AddOrUpdateLogonTime(LogonTime logonTime)
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

        private void UpdateLogonTimeAllowedDetail()
        {
            lock (logonTimeLock)
            {
                if (!logonTimeAllowedForUpdate.Any())
                {
                    return;
                }
                var logonTimeAllowed = logonTimeAllowedForUpdate.First();
                logonTimeAllowedForUpdate.Remove(logonTimeAllowed);
                using (var db = new LogonTimesDB())
                {
                    db.Update(logonTimeAllowed);
                }
            }
        }

        public void UpdateLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
        {
            logonTimeAllowedForUpdate.Add(logonTimeAllowed);
            Thread restrictPersonThread = new Thread(UpdateLogonTimeAllowedDetail);
            restrictPersonThread.IsBackground = true;
            restrictPersonThread.Start();
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

        private void MakePersonRestrictedDetail()
        {
            if (!peopleToBeRestricted.Any())
            {
                return;
            }
            Person person;
            lock (restrictedLock)
            {
                person = peopleToBeRestricted.First();
                peopleToBeRestricted.Remove(person);
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
            EventHandler<PersonEventArgs> handler = PersonModificationFinished;
            if (handler != null)
            {
                handler(this, new PersonEventArgs(person));
            }
        }

        public void MakePersonRestricted(Person person)
        {
            peopleToBeRestricted.Add(person);
            Thread restrictPersonThread = new Thread(MakePersonRestrictedDetail);
            restrictPersonThread.IsBackground = true;
            restrictPersonThread.Start();
        }

        private void MakePersonUnrestrictedDetail()
        {
            if (!peopleToBeUnrestricted.Any())
            {
                return;
            }
            Person person;
            lock (unrestrictedLock)
            {
                person = peopleToBeUnrestricted.First();
                peopleToBeUnrestricted.Remove(person);
            }
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
            EventHandler<PersonEventArgs> handler = PersonModificationFinished;
            if (handler != null)
            {
                handler(this, new PersonEventArgs(person));
            }
        }

        public void MakePersonUnrestricted(Person person)
        {
            peopleToBeUnrestricted.Add(person);
            Thread unrestrictPersonThread = new Thread(MakePersonUnrestrictedDetail);
            unrestrictPersonThread.IsBackground = true;
            unrestrictPersonThread.Start();
        }

        public Person GetPerson(int personId)
        {
            if (!People.Any(x => x.PersonId == personId))
            {
                return null;
            }
            return People.First(x => x.PersonId == personId);
        }

        public Person PersonForSID(string SID)
        {
            return People.FirstOrDefault(x => x.SID.Equals(SID));
        }
        #endregion

        #region PersonApplication
        public List<PersonApplication> PersonApplications
        {
            get
            {
                if (personApplications == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        personApplications = (from personApplication in db.PersonApplications select personApplication).ToList();
                    }
                }
                return personApplications;
            }
        }

        public void AddOrUpdatePersonApplication(PersonApplication personApplication)
        {
            using (var db = new LogonTimesDB())
            {
                if (PersonApplications.Any(x => x.PersonApplicationId == personApplication.PersonApplicationId))
                {
                    db.Update(personApplication);
                    PersonApplications.Remove(personApplication);
                }
                else
                {
                    personApplication.PersonApplicationId = Convert.ToInt32(db.InsertWithIdentity(personApplication));
                }
                PersonApplications.Add(personApplication);
            }
        }
        #endregion PersonApplication

        #region System settings
        public List<SystemSettingType> SystemSettingTypes
        {
            get
            {
                if (systemSettingTypes == null)
                {
                    using (var db = new LogonTimesDB())
                    {
                        systemSettingTypes = (from systemSettingType in db.SystemSettingTypes select systemSettingType).ToList();
                    }
                }
                return systemSettingTypes;
            }
        }

        public SystemSettingDetail GetSystemSettingDetail(string setting)
        {
            var systemSettingType = SystemSettingTypes.First(x => x.SystemSettingName.Equals(setting));
            if (systemSettingType != null)
            {
                using (var db = new LogonTimesDB())
                {
                    var systemSettings = (from systemSettingDetail
                                         in db.SystemSettingDetails
                                          where systemSettingDetail.SystemSettingNameId == systemSettingType.SystemSettingNameId
                                          select systemSettingDetail).ToList();
                    if (systemSettings.Any())
                    {
                        return systemSettings.First();
                    }
                }
            }
            return null;
        }

        public void UpdateSystemSettingDetail(SystemSettingDetail setting)
        {
            using (var db = new LogonTimesDB())
            {
                db.Update(setting);
            }
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
