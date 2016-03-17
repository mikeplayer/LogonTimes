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
        #region local variables
        private IDataAccess mockDataAccess;
        private ITerminalServicesSession mockTerminalServicesSession;
        private ITimeManagementData mockTimeManagementData;
        private IUserManagement mockUserManagement;
        private ILogger mockLogger;
        private IDates mockDates;
        private IEventManagement eventManagement;
        private const string pendingEventName = "Pending";
        private const string newDayLogonEventName = "NewDayLogon";
        private const string logonEventName = "TestLogonEvent";
        private const string logoffEventName = "TestLogoffEvent";
        private const string noTriggerLogonEventName = "TestLogonEventNoTrigger";
        private const string userName = "Test";
        private const double hoursAllowedPerDay = 1.5;
        private List<EventType> eventTypes;
        private List<Person> people = new List<Person>();
        private List<LogonTimes.DataModel.DayOfWeek> daysOfWeek;
        private List<TimePeriod> timePeriods;
        private List<LogonTimeAllowed> logonTimesAllowed;
        private List<HoursPerDay> hoursPerDay;
        private List<LogonTime> logonTimes;
        private DateTime todayValue = DateTime.Today;
        private DateTime nowValue = DateTime.Now;
        private int noOfMessages = 0;
        private bool loggedOff = false;
        #endregion local variables

        #region mock data
        //Would normally use some sort of data mocking for this, but for this we need specific matching data

        #region dates
        delegate DateTime MockedGetDate();
        private void SetupDates()
        {
            MockedGetDate getToday = () => { return todayValue; };
            mockDates.Stub(x => x.Today).Do(getToday);
            MockedGetDate getNow = () => { return nowValue; };
            mockDates.Stub(x => x.Now).Do(getNow);
        }
        #endregion dates

        #region days of week
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
        #endregion days of week

        #region time periods
        delegate TimePeriod MockedGetTimePeriod(int timePeriodId);

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
            MockedGetTimePeriod getTimePeriod = (n) => { return timePeriods.FirstOrDefault(x => x.TimePeriodId == n); };
            mockDataAccess.Stub(x => x.GetTimePeriod(Arg<int>.Is.Anything))
                .Do(getTimePeriod);
        }
        #endregion time periods

        #region event types
        delegate EventType MockedGetEventType(int eventTypeId);

        private EventType SpecificEventType(string eventTypeName)
        {
            return eventTypes.First(x => x.EventTypeName.Equals(eventTypeName));
        }

        private void SetupEventTypes()
        {
            eventTypes = new List<EventType>();
            var eventType = new EventType
            {
                EventTypeName = pendingEventName,
                EventTypeId = 1,
                IsLoggedOn = false,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = newDayLogonEventName,
                EventTypeId = 2,
                IsLoggedOn = true,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = logonEventName,
                EventTypeId = 3,
                IsLoggedOn = true,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = logoffEventName,
                EventTypeId = 4,
                IsLoggedOn = false,
                TriggersEvent = true,
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = noTriggerLogonEventName,
                EventTypeId = 5,
                IsLoggedOn = false,
                TriggersEvent = false,
            };
            eventTypes.Add(eventType);
            mockTimeManagementData.Stub(x => x.EventTypes).Return(eventTypes);
            mockDataAccess.Stub(x => x.EventTypes).Return(eventTypes);
            MockedGetEventType getEventType = (n) => { return eventTypes.FirstOrDefault(x => x.EventTypeId == n); };
            mockDataAccess.Stub(x => x.GetEventType(Arg<int>.Is.Anything))
                .Do(getEventType);
        }
        #endregion event types

        #region hours per day
        private void MockHoursPerDay(int personId)
        {
            hoursPerDay = new List<HoursPerDay>();
            for (int i = 0; i < 7; i++)
            {
                var hourPerDay = new HoursPerDay { HoursPerDayId = i, PersonId = personId, DayNumber = i, HoursAllowed = hoursAllowedPerDay };
                hoursPerDay.Add(hourPerDay);
            }
            mockDataAccess.Stub(x => x.HoursPerDays).Return(hoursPerDay);
        }
        #endregion hours per day

        #region logon times allowed
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
                    logonTimesAllowed.Add(logonTimeAllowed);
                }
            }
            mockDataAccess.Stub(x => x.LogonTimesAllowed).Return(logonTimesAllowed);
        }
        #endregion logon times allowed

        #region logon times
        delegate LogonTime MockedGetLogonTime(int logonTimeId);

        private void SetupLogonTimes()
        {
            logonTimes = new List<LogonTime>();
            mockTimeManagementData.Stub(x => x.LogonTimes).Return(logonTimes);
            mockDataAccess.Stub(x => x.LogonTimes).Return(logonTimes);
            MockedGetLogonTime getLogonTime = (n) => { return logonTimes.FirstOrDefault(x => x.LogonTimeId == n); };
            mockDataAccess.Stub(x => x.GetLogonTime(Arg<int>.Is.Anything))
                .Do(getLogonTime);
            mockTimeManagementData.Stub(x => x.AddOrUpdateLogonTime(Arg<LogonTime>.Is.Anything)).WhenCalled(_ =>
            {
                var logonTime = (LogonTime)_.Arguments[0];
                if (logonTime.LogonTimeId == 0)
                {
                    var lastItem = logonTimes.OrderByDescending(x => x.LogonTimeId).FirstOrDefault();
                    if (lastItem != null)
                    {
                        logonTime.LogonTimeId = lastItem.LogonTimeId + 1;
                    }
                    else
                    {
                        logonTime.LogonTimeId = 1;
                    }
                    logonTimes.Add(logonTime);
                }
                else
                {
                    var item = logonTimes.FirstOrDefault(x => x.LogonTimeId == logonTime.LogonTimeId);
                    item.EventTime = logonTime.EventTime;
                    item.EventTypeId = logonTime.EventTypeId;
                }
            });
        }

        private void MockLogonTimesTimeUsedUp(int personId, int minutesToAddToStart)
        {
            var logonEventType = SpecificEventType(logonEventName);
            var logoffEventType = SpecificEventType(logoffEventName);
            var logonTime = new LogonTime { PersonId = personId, LogonTimeId = 1, CorrespondingEventId = 2, EventTime = nowValue.AddHours(hoursAllowedPerDay * -1).AddMinutes(minutesToAddToStart), EventTypeId = logonEventType.EventTypeId };
            logonTimes.Add(logonTime);
            logonTime = new LogonTime { PersonId = personId, LogonTimeId = 2, CorrespondingEventId = 1, EventTime = nowValue, EventTypeId = logoffEventType.EventTypeId };
            logonTimes.Add(logonTime);
        }
        #endregion logon times

        #region people
        private void SetupPeople()
        {
            var person = new Person
            {
                PersonId = 1,
                LogonName = userName
            };
            people.Add(person);
            mockUserManagement.Stub(x => x.GetPersonDetail(userName)).Return(person);
            mockTerminalServicesSession.Stub(x => x.UserName).Return(userName);
        }
        #endregion people
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

            mockLogger.Stub(x => x.ShouldLog(Arg<DebugLevels>.Is.Anything)).Return(false);

            Initialise(x =>
            {
                x.AddDependency(mockDataAccess);
                x.AddDependency(mockTimeManagementData);
                x.AddDependency(mockUserManagement);
                x.AddDependency(mockLogger);
                x.AddDependency(mockDates);
            });

            SetupDates();
            SetupDaysOfWeek();
            SetupEventTypes();
            SetupTimePeriods();
            SetupLogonTimes();
            SetupPeople();
        }

        private void AddEventManager(bool realInstance)
        {
            if (realInstance)
            {
                eventManagement = new EventManagement();
            }
            else
            {
                eventManagement = MockRepository.GenerateMock<IEventManagement>();
            }
            AddDependency(x =>
            {
                x.AddDependency(eventManagement);
            });
        }
        #endregion Setup

        #region Terminal services
        delegate RemoteMessageBoxResult MockedMessageBox(string message
            , string header
            , RemoteMessageBoxButtons button
            , RemoteMessageBoxIcon icon
            , RemoteMessageBoxDefaultButton defaultButton
            , RemoteMessageBoxOptions options
            , TimeSpan timeout
            , bool synchronous);
        delegate void MockedDisconnect(bool synchronous);

        private void DoAssertions(bool msgboxCalled, bool logOffCalled)
        {
            if (msgboxCalled)
            {
                mockTerminalServicesSession.AssertWasCalled(x => x.MessageBox(Arg<string>.Is.Anything
                    , Arg<string>.Is.Anything
                    , Arg<RemoteMessageBoxButtons>.Is.Anything
                    , Arg<RemoteMessageBoxIcon>.Is.Anything
                    , Arg<RemoteMessageBoxDefaultButton>.Is.Anything
                    , Arg<RemoteMessageBoxOptions>.Is.Anything
                    , Arg<TimeSpan>.Is.Anything
                    , Arg<bool>.Is.Anything));
            }
            else
            {
                mockTerminalServicesSession.AssertWasNotCalled(x => x.MessageBox(Arg<string>.Is.Anything
                    , Arg<string>.Is.Anything
                    , Arg<RemoteMessageBoxButtons>.Is.Anything
                    , Arg<RemoteMessageBoxIcon>.Is.Anything
                    , Arg<RemoteMessageBoxDefaultButton>.Is.Anything
                    , Arg<RemoteMessageBoxOptions>.Is.Anything
                    , Arg<TimeSpan>.Is.Anything
                    , Arg<bool>.Is.Anything));
            }
            if (logOffCalled)
            {
                mockTerminalServicesSession.AssertWasCalled(x => x.Disconnect(Arg<bool>.Is.Anything));
            }
            else
            {
                mockTerminalServicesSession.AssertWasNotCalled(x => x.Disconnect(Arg<bool>.Is.Anything));
            }
        }

        private void SetupToCountTerminalServicesEvents(TimeManagement timeManagement)
        {
            MockedMessageBox messageBox = (mesage, header, button, icon, defaultButton, options, timeout, sync) =>
            {
                noOfMessages++;
                return RemoteMessageBoxResult.Ok;
            };

            mockTerminalServicesSession.Stub(x => x.MessageBox(Arg<string>.Is.Anything
                , Arg<string>.Is.Anything
                , Arg<RemoteMessageBoxButtons>.Is.Anything
                , Arg<RemoteMessageBoxIcon>.Is.Anything
                , Arg<RemoteMessageBoxDefaultButton>.Is.Anything
                , Arg<RemoteMessageBoxOptions>.Is.Anything
                , Arg<TimeSpan>.Is.Anything
                , Arg<bool>.Is.Anything))
                .Do(messageBox);
            MockedDisconnect disconnect = (synchronous) =>
            {
                timeManagement.NewSessionEvent(mockTerminalServicesSession, logoffEventName);
                loggedOff = true;

            };
            mockTerminalServicesSession.Stub(x => x.Disconnect(Arg<bool>.Is.Anything)).Do(disconnect);
        }
        #endregion Terminal services

        #region New session event, testing hours per day
        [TestMethod]
        public void NewSessionEventHoursPerDay_EventWithTrigger_CurrentPersonCalled()
        {
            //Arrange
            AddEventManager(false);
            var timeManagement = new TimeManagement();
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            eventManagement.AssertWasCalled(x => x.CurrentPerson = people.First());
        }

        [TestMethod]
        public void NewSessionEventHoursPerDay_EventNoTrigger_NoCurrentPerson()
        {
            //Arrange
            AddEventManager(false);
            var timeManagement = new TimeManagement();
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, noTriggerLogonEventName);
            //Assert
            eventManagement.AssertWasNotCalled(x => x.CurrentPerson = people.First());
        }

        [TestMethod]
        public void NewSessionEventHoursPerDay_HoursWillBeExceededIn5Minutes_WarningIssuedNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            MockLogonTimesTimeUsedUp(personId, 5);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, false);
        }

        [TestMethod]
        public void NewSessionEventHoursPerDay_HoursExceededBy5Minutes_WarningIssuedNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            MockLogonTimesTimeUsedUp(personId, -5);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, true);
        }

        [TestMethod]
        public void NewSessionEventHoursPerDay_HoursExceededBy0Minutes_WarningIssuedNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            MockLogonTimesTimeUsedUp(personId, 0);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, true);
        }

        [TestMethod]
        public void NewSessionEventHoursPerDay_HoursExceededThenNewDayLogon_NoWarningNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            MockLogonTimesTimeUsedUp(personId, 0);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today.AddDays(1);
            nowValue = DateTime.Now.AddDays(1);
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(false, false);
        }
        #endregion New session event, testing hours per day

        #region Update logins, testing hours per day
        [TestMethod]
        public void UpdateLoginsHoursPerDay_TimeNotExpired_NoWarningNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = DateTime.Now.AddHours(hoursAllowedPerDay * -1);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            nowValue = DateTime.Now.AddMinutes(-20);
            //Act
            timeManagement.UpdateLogins();
            //Assert
            DoAssertions(false, false);
        }

        [TestMethod]
        public void UpdateLoginsHoursPerDay_TimeExpiresIn5Minutes_WarningGivenNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = DateTime.Now.AddHours(hoursAllowedPerDay * -1);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            nowValue = DateTime.Now.AddMinutes(-5);
            //Act
            timeManagement.UpdateLogins();
            //Assert
            DoAssertions(true, false);
        }

        [TestMethod]
        public void UpdateLoginsHoursPerDay_TimeExpired_WarningGivenLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = DateTime.Now.AddHours(hoursAllowedPerDay * -1);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            nowValue = DateTime.Now;
            //Act
            timeManagement.UpdateLogins();
            //Assert
            DoAssertions(true, true);
        }

        [TestMethod]
        public void UpdateLoginsHoursPerDay_TimeWithin10MinutesThenTimeWithin8Minutes_WarningNotGiven()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = DateTime.Now.AddHours(hoursAllowedPerDay * -1);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            nowValue = DateTime.Now.AddMinutes(-10);
            timeManagement.UpdateLogins();
            DoAssertions(true, false);
            mockTerminalServicesSession.Expect(x => x.MessageBox(Arg<string>.Is.Anything
                , Arg<string>.Is.Anything
                , Arg<RemoteMessageBoxButtons>.Is.Anything
                , Arg<RemoteMessageBoxIcon>.Is.Anything
                , Arg<RemoteMessageBoxDefaultButton>.Is.Anything
                , Arg<RemoteMessageBoxOptions>.Is.Anything
                , Arg<TimeSpan>.Is.Anything
                , Arg<bool>.Is.Anything)).Repeat.Never();
            //Act
            nowValue = DateTime.Now.AddMinutes(-8);
            timeManagement.UpdateLogins();
            //Assert
            mockTerminalServicesSession.VerifyAllExpectations();
        }

        [TestMethod]
        public void UpdateLoginsHoursPerDay_Every5MinutesFrom5MinutesBeforeTo5MinutesAfterTimeUp_3Messages1Logoff()
        {
            //Arrange
            int personId = people.First().PersonId;
            noOfMessages = 0;
            loggedOff = false;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, true);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = DateTime.Now.AddHours(hoursAllowedPerDay * -1);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            nowValue = DateTime.Now.AddMinutes(-15);
            SetupToCountTerminalServicesEvents(timeManagement);
            //Act
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now.AddMinutes(-12);
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now.AddMinutes(-10);
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now.AddMinutes(-8);
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now.AddMinutes(-5);
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now.AddMinutes(-3);
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now;
            timeManagement.UpdateLogins();
            nowValue = DateTime.Now.AddMinutes(2);
            timeManagement.UpdateLogins();
            //Assert
            Assert.IsTrue(loggedOff);
            Assert.AreEqual(3, noOfMessages);
        }
        #endregion Update logins, testing hours per day

        #region New session event, testing logon times allowed
        [TestMethod]
        public void NewSessionEventLoginTimeAllowed_LogonNotAllowed_MessageIssuedLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = DateTime.Now.AddHours(hoursAllowedPerDay * -1);
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, true);
        }

        [TestMethod]
        public void NewSessionEventLoginTimeAllowed_LogonAllowedFor10Minutes_MessageIssuedNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-10);
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, false);
        }

        [TestMethod]
        public void NewSessionEventLoginTimeAllowed_LogonAllowedFor5Minutes_MessageIssuedNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-5);
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, false);
        }

        [TestMethod]
        public void NewSessionEventLoginTimeAllowed_LogonAllowedFor0Minutes_MessageIssuedLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay);
            //Act
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Assert
            DoAssertions(true, true);
        }
        #endregion New session event, testing logon times allowed

        #region Update logins, testing logon times allowed
        [TestMethod]
        public void UpdateLoginsLoginTimeAllowed_TimeNotExpired_NoWarningNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-20);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Act
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-15);
            timeManagement.UpdateLogins();
            //Assert
            DoAssertions(false, false);
        }

        [TestMethod]
        public void UpdateLoginsLoginTimeAllowed_TimeExpiresIn5Minutes_WarningGivenNotLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-20);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Act
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-5);
            timeManagement.UpdateLogins();
            //Assert
            DoAssertions(true, false);
        }

        [TestMethod]
        public void UpdateLoginsLoginTimeAllowed_TimeExpired_WarningGivenLoggedOff()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-20);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Act
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay);
            timeManagement.UpdateLogins();
            //Assert
            DoAssertions(true, true);
        }

        [TestMethod]
        public void UpdateLoginsLoginTimeAllowed_TimeWithin10MinutesThenTimeWithin8Minutes_WarningNotGiven()
        {
            //Arrange
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-20);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            //Act
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-10);
            timeManagement.UpdateLogins();
            DoAssertions(true, false);
            mockTerminalServicesSession.Expect(x => x.MessageBox(Arg<string>.Is.Anything
                , Arg<string>.Is.Anything
                , Arg<RemoteMessageBoxButtons>.Is.Anything
                , Arg<RemoteMessageBoxIcon>.Is.Anything
                , Arg<RemoteMessageBoxDefaultButton>.Is.Anything
                , Arg<RemoteMessageBoxOptions>.Is.Anything
                , Arg<TimeSpan>.Is.Anything
                , Arg<bool>.Is.Anything)).Repeat.Never();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-8);
            timeManagement.UpdateLogins();
            //Assert
            mockTerminalServicesSession.VerifyAllExpectations();
        }

        [TestMethod]
        public void UpdateLoginsLoginTimeAllowed_Every5MinutesFrom5MinutesBeforeTo5MinutesAfterTimeUp_3Messages1Logoff()
        {
            //Arrange
            noOfMessages = 0;
            loggedOff = false;
            int personId = people.First().PersonId;
            MockHoursPerDay(personId);
            MockLogonTimesAllowed(personId, false);
            var pickLogon = logonTimesAllowed.FirstOrDefault(x => x.DayNumber == (int)DateTime.Today.DayOfWeek
                && x.TimePeriod.PeriodEnd.TimeOfDay >= nowValue.TimeOfDay
                && x.TimePeriod.PeriodStart.TimeOfDay <= nowValue.TimeOfDay);
            Assert.IsTrue(pickLogon != null);
            pickLogon.Permitted = true;
            AddEventManager(true);
            var timeManagement = new TimeManagement();
            todayValue = DateTime.Today;
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-20);
            timeManagement.NewSessionEvent(mockTerminalServicesSession, logonEventName);
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-10);
            SetupToCountTerminalServicesEvents(timeManagement);
            //Act
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-12);
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-10);
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-8);
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-5);
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(-3);
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay);
            timeManagement.UpdateLogins();
            nowValue = todayValue.Add(pickLogon.TimePeriod.PeriodEnd.TimeOfDay).AddMinutes(2);
            timeManagement.UpdateLogins();
            //Assert
            Assert.IsTrue(loggedOff);
            Assert.AreEqual(3, noOfMessages);
        }
        #endregion Update logins, testing logon times allowed
    }
}
