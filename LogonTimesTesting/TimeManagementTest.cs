using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using LogonTimes.TimeControl;
using LogonTimes.People;
using LogonTimes.DataModel;
using System.Collections.Generic;
using LogonTimes.Logging;
using Cassia;
using LogonTimes.DateHandling;
using System.Linq;

namespace LogonTimesTesting
{
    [TestClass]
    public class TimeManagementTest : BaseTest
    {
        private IDataAccess mockDataAccess;
        private ITerminalServicesSession mockTerminalServicesSession;
        private ITimeManagementData mockTimeManagementData;
        private IUserManagement mockUserManagement;
        private ILogger mockLogger;
        private IDates mockDates;
        private IEventManagement eventManagement;
        private const string logonEvent = "TestLogonEvent";
        private const string logoffEvent = "TestLogoffEvent";
        private const string noTriggerLogonEvent = "TestLogonEventNoTrigger";
        private const string userName = "Test";
        private List<Person> people = new List<Person>();
        private List<LogonTimes.DataModel.DayOfWeek> daysOfWeek;
        private List<TimePeriod> timePeriods;
        private List<LogonTimeAllowed> logonTimesAllowed;
        private List<HoursPerDay> hoursPerDay;

        #region mock data
        //Would normally use some sort of data mocking for this, but for this we need specific matching data

        private void SetupDaysOfWeek()
        {
            daysOfWeek = new List<LogonTimes.DataModel.DayOfWeek>();
            var values = Enum.GetValues(typeof(System.DayOfWeek));
            foreach (var day in values)
            {
                var dayValue = new LogonTimes.DataModel.DayOfWeek
                {
                    DayName = day.ToString(),
                    DayNumber = (int)day,
                    SortOrder = (int)day,
                };
                daysOfWeek.Add(dayValue);
            }
            mockDataAccess.Stub(x => x.DaysOfWeek).Return(daysOfWeek);
        }

        private void SetupTimePeriods()
        {
            timePeriods = new List<TimePeriod>();
            DateTime startTime = DateTime.MinValue;
            for (int i = 0; i < 24; i++)
            {
                var timePeriod = new TimePeriod
                {
                    TimePeriodId = i,
                    PeriodStart = startTime.AddHours(i),
                    PeriodEnd = startTime.AddHours(i + 1).AddMilliseconds(-1),
                };
                timePeriods.Add(timePeriod);
            }
            mockDataAccess.Stub(x => x.TimePeriods).Return(timePeriods);
        }

        private void SetupEventTypes()
        {
            var eventTypes = new List<EventType>();
            var eventType = new EventType
            {
                EventTypeName = "Pending",
                EventTypeId = 1,
                IsLoggedOn = false,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = "NewDayLogon",
                EventTypeId = 2,
                IsLoggedOn = true,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = logonEvent,
                EventTypeId = 3,
                IsLoggedOn = true,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = logoffEvent,
                EventTypeId = 4,
                IsLoggedOn = false,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = noTriggerLogonEvent,
                EventTypeId = 5,
                IsLoggedOn = false,
                TriggersEvent = false,
            };
            eventTypes.Add(eventType);
            mockTimeManagementData.Stub(x => x.EventTypes).Return(eventTypes);
            mockDataAccess.Stub(x => x.EventTypes).Return(eventTypes);
        }

        private void MockHoursPerDay(int personId)
        {
            hoursPerDay = new List<HoursPerDay>();
            for (int i = 0; i < 7; i++)
            {
                var hourPerDay = new HoursPerDay { HoursPerDayId = i, PersonId = personId, DayNumber = i, HoursAllowed = 1 };
                hoursPerDay.Add(hourPerDay);
            }
            mockDataAccess.Stub(x => x.HoursPerDays).Return(hoursPerDay);
        }

        private void MockLogonTimesAllowed(int personId, bool permitted)
        {
            int id = 0;
            logonTimesAllowed = new List<LogonTimeAllowed>();
            foreach (var day in daysOfWeek)
            {
                foreach (var timePeriod in timePeriods)
                {
                    id++;
                    var logonTimeAllowed = new LogonTimeAllowed
                    {
                        LogonTimeAllowedId = id,
                        PersonId = personId,
                        DayNumber = day.DayNumber,
                        TimePeriodId = timePeriod.TimePeriodId,
                        Permitted = permitted
                    };
                }
            }
            mockDataAccess.Stub(x => x.LogonTimesAllowed).Return(logonTimesAllowed);
        }
        #endregion mock data

        #region Setup
        [TestInitialize]
        public void SetupTests()
        {
            SetupTimeManagement();
        }

        private void SetupTimeManagement()
        {
            mockDataAccess = MockRepository.GenerateMock<IDataAccess>();
            mockTerminalServicesSession = MockRepository.GenerateMock<ITerminalServicesSession>();
            mockTimeManagementData = MockRepository.GenerateMock<ITimeManagementData>();
            mockUserManagement = MockRepository.GenerateMock<IUserManagement>();
            mockLogger = MockRepository.GenerateMock<ILogger>();
            mockDates = MockRepository.GenerateMock<IDates>();
            eventManagement = MockRepository.GenerateMock<IEventManagement>();

            SetupDaysOfWeek();
            SetupEventTypes();
            SetupTimePeriods();

            mockTimeManagementData.Stub(x => x.LogonTimes).Return(new List<LogonTime>());

            mockLogger.Stub(x => x.ShouldLog(Arg<DebugLevels>.Is.Anything)).Return(false);

            mockDates.Stub(x => x.Today).Return(DateTime.Today);
            mockDates.Stub(x => x.Now).Return(DateTime.Now);

            var person = new Person
            {
                PersonId = 1,
                LogonName = userName
            };
            people.Add(person);
            mockUserManagement.Stub(x => x.GetPersonDetail(userName)).Return(person);
            mockTerminalServicesSession.Stub(x => x.UserName).Return(userName);

            Initialise(x =>
            {
                x.AddDependency(mockDataAccess);
                x.AddDependency(mockTimeManagementData);
                x.AddDependency(mockUserManagement);
                x.AddDependency(mockLogger);
                x.AddDependency(mockDates);
                x.AddDependency(eventManagement);
            });
        }
        #endregion Setup

        [TestMethod]
        public void NewSessionEvent_EventWithTrigger_CurrentPersonCalled()
        {
            var timeManagement = new TimeManagement();
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEvent);
            eventManagement.AssertWasCalled(x => x.CurrentPerson = people.First());
        }

        [TestMethod]
        public void NewSessionEvent_EventNoTrigger_NoCurrentPerson()
        {
            var timeManagement = new TimeManagement();
            timeManagement.NewSessionEvent(mockTerminalServicesSession, noTriggerLogonEvent);
            eventManagement.AssertWasNotCalled(x => x.CurrentPerson = people.First());
        }

        [TestMethod]
        public void UpdateLoginsAndLogon_NewDay_CanLogon()
        {
            const int personId = 1;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            var person = new Person
            {
                PersonId = personId,
                LogonName = userName
            };
            var eventManagement = new EventManagement();
            AddDependency(x =>
            {
                x.AddDependency(eventManagement);
            });
            var timeManagement = new TimeManagement();
        }
    }
}
