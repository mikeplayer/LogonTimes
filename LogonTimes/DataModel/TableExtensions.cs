using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using System;
using System.Reflection;
using LogonTimes.IoC;
using LogonTimes.DateHandling;
using System.Text;

namespace LogonTimes.DataModel
{
    #region Applications
    public partial class Application
    {
        private IDataAccess dataAccess;

        public Application()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
        }

        public List<PersonApplication> PersonApplications
        {
            get
            {
                return dataAccess.PersonApplications.Where(x => x.ApplicationId == ApplicationId).ToList();
            }
        }
    }
    #endregion Applications

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
        private IDataAccess dataAccess;

        public HoursPerDay()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Person.LogonName, HoursAllowed);
        }

        public Person Person
        {
            get
            {
                return dataAccess.GetPerson(PersonId);
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return dataAccess.GetDayOfWeek(DayNumber);
            }
        }
    }
    #endregion

    #region Logon time
    public partial class LogonTime
    {
        private IDataAccess dataAccess;

        public LogonTime()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Person.LogonName, EventType.EventTypeName, EventTime);
        }

        public Person Person
        {
            get
            {
                return dataAccess.GetPerson(PersonId);
            }
        }

        public EventType EventType
        {
            get
            {
                return dataAccess.GetEventType(EventTypeId);
            }
        }

        public LogonTime CorrespondingEvent
        {
            get
            {
                if (CorrespondingEventId == null)
                {
                    return null;
                }
                return dataAccess.GetLogonTime(CorrespondingEventId.Value);
            }
        }

        public TimeSpan LoggedOnDuration
        {
            get
            {
                if (!EventType.IsLoggedOn && CorrespondingEvent != null)
                {
                    return EventTime.Subtract(CorrespondingEvent.EventTime);
                }
                return new TimeSpan(0);
            }
        }
    }
    #endregion

    #region Logon time allowed
    public partial class LogonTimeAllowed
    {
        private IDataAccess dataAccess;

        public LogonTimeAllowed()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Person.LogonName, DayOfWeek.DayName, TimePeriod.PeriodStart.ToString("hh:mm"), Permitted);
        }

        public Person Person
        {
            get
            {
                return dataAccess.GetPerson(PersonId);
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return dataAccess.GetDayOfWeek(DayNumber);
            }
        }

        public TimePeriod TimePeriod
        {
            get
            {
                return dataAccess.GetTimePeriod(TimePeriodId);
            }
        }
    }
    #endregion

    #region Person
    public partial class Person
    {
        private IDataAccess dataAccess;
        private IDates dates;

        public Person()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
            dates = IocRegistry.GetInstance<IDates>();
        }

        public override string ToString()
        {
            return LogonName;
        }

        public List<HoursPerDay> HoursPerDay
        {
            get
            {
                return dataAccess.HoursPerDays.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<LogonTime> LogonTimes
        {
            get
            {
                return dataAccess.LogonTimes.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<LogonTimeAllowed> LogonTimesAllowed
        {
            get
            {
                return dataAccess.LogonTimesAllowed.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public List<PersonApplication> PersonApplications
        {
            get
            {
                return dataAccess.PersonApplications.Where(x => x.PersonId == PersonId).ToList();
            }
        }

        public bool IsRestricted
        {
            get
            {
                return (HoursPerDay.Any()
                    || LogonTimesAllowed.Any());
            }
        }

        public double MaximumHoursToday
        {
            get
            {
                if (HoursPerDay.Any(x => x.DayNumber == (int)DateTime.Today.DayOfWeek))
                {
                    var hourPerDay = HoursPerDay.First(x => x.DayNumber == (int)dates.Today.DayOfWeek);
                    if (hourPerDay.HoursAllowed.HasValue)
                    {
                        return hourPerDay.HoursAllowed.Value;
                    }
                }
                return 24;
            }
        }
    }
    #endregion

    #region PersonApplication
    public partial class PersonApplication
    {
        private IDataAccess dataAccess;

        public PersonApplication()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
        }

        public override string ToString()
        {
            var detail = new StringBuilder();
            if (Person != null)
            {
                detail.Append(Person.LogonName);
                detail.Append(" ");
            }
            if (Application != null)
            {
                detail.Append(Application.ApplicationName);
                detail.Append(" ");
            }
            if (!Permitted)
            {
                detail.Append(" not");
            }
            detail.Append(" permitted");
            return detail.ToString();
        }

        public Application Application
        {
            get
            {
                return dataAccess.Applications.FirstOrDefault(x => x.ApplicationId == ApplicationId);
            }
        }

        public Person Person
        {
            get
            {
                return dataAccess.People.FirstOrDefault(x => x.PersonId == PersonId);
            }
        }
    }
    #endregion PersonApplication

    #region Time Period
    public partial class TimePeriod
    {
        public override string ToString()
        {
            return string.Format("{0} to {1}", PeriodStart.ToString("h:mm tt"), PeriodEnd.ToString("h:mm tt"));
        }
    }
    #endregion

    #region System settings
    public partial class SystemSettingDetail
    {
        private IDataAccess dataAccess;

        public SystemSettingDetail()
        {
            dataAccess = IocRegistry.GetInstance<IDataAccess>();
        }

        public SystemSettingType SystemSettingType
        {
            get
            {
                return dataAccess.SystemSettingTypes.First(x => x.SystemSettingNameId == SystemSettingNameId);
            }
        }

        public int? IntValue
        {
            get
            {
                if (SystemSettingType.DataType.Equals("int"))
                {
                    int result;
                    if (int.TryParse(SystemSetting, out result))
                    {
                        return result;
                    }
                }
                return null;
            }
        }

        public bool? BoolValue
        {
            get
            {
                if (SystemSettingType.DataType.Equals("bool"))
                {
                    bool result;
                    if (bool.TryParse(SystemSetting, out result))
                    {
                        return result;
                    }
                }
                return null;
            }
        }

        public string StringValue
        {
            get
            {
                if (SystemSettingType.DataType.Equals("string"))
                {
                    return SystemSetting;
                }
                return null;
            }
        }
    }

    public static class SystemSettings
    {
        private static IDataAccess dataAccess;

        private static IDataAccess DataAccess
        {
            get
            {
                if (dataAccess == null)
                {
                    dataAccess = IocRegistry.GetInstance<IDataAccess>();
                }
                return dataAccess;
            }
        }

        public static SystemSettingDetail Detail(this SystemSettingTypesEnum settingType)
        {
            SystemSettingAttribute attr = GetAttr(settingType);
            return DataAccess.GetSystemSettingDetail(attr.SystemSettingType);
        }

        public static bool? BoolValue(this SystemSettingTypesEnum settingType)
        {
            SystemSettingAttribute attr = GetAttr(settingType);
            var systemSetting = DataAccess.GetSystemSettingDetail(attr.SystemSettingType);
            if (systemSetting != null)
            {
                return systemSetting.BoolValue;
            }
            return null;
        }

        public static int? IntValue(this SystemSettingTypesEnum settingType)
        {
            SystemSettingAttribute attr = GetAttr(settingType);
            var systemSetting = DataAccess.GetSystemSettingDetail(attr.SystemSettingType);
            if (systemSetting != null)
            {
                return systemSetting.IntValue;
            }
            return null;
        }

        public static string StringValue(this SystemSettingTypesEnum settingType)
        {
            SystemSettingAttribute attr = GetAttr(settingType);
            var systemSetting = DataAccess.GetSystemSettingDetail(attr.SystemSettingType);
            if (systemSetting != null)
            {
                return systemSetting.StringValue;
            }
            return null;
        }

        private static SystemSettingAttribute GetAttr(SystemSettingTypesEnum settingType)
        {
            return (SystemSettingAttribute)Attribute.GetCustomAttribute(ForValue(settingType), typeof(SystemSettingAttribute));
        }

        private static MemberInfo ForValue(SystemSettingTypesEnum settingType)
        {
            return typeof(SystemSettingTypesEnum).GetField(Enum.GetName(typeof(SystemSettingTypesEnum), settingType));
        }
    }

    class SystemSettingAttribute : Attribute
    {
        internal SystemSettingAttribute(string systemSettingType)
        {
            SystemSettingType = systemSettingType;
        }
        public string SystemSettingType { get; private set; }
    }

    public enum SystemSettingTypesEnum
    {
        [SystemSetting("ConfigurationChanged")] ConfigurationChanged,
        [SystemSetting("Version")] Version,
    }
    #endregion
}
