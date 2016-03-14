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

        [TestInitialize]
        public void SetupTests()
        {
            SetupTimeManagement();
        }

        private void SetupTimeManagement()
        {
            mockTerminalServicesSession = MockRepository.GenerateMock<ITerminalServicesSession>();
            mockTimeManagementData = MockRepository.GenerateMock<ITimeManagementData>();
            mockUserManagement = MockRepository.GenerateMock<IUserManagement>();
            mockLogger = MockRepository.GenerateMock<ILogger>();
            mockDates = MockRepository.GenerateMock<IDates>();
            eventManagement = MockRepository.GenerateMock<IEventManagement>();

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
                EventTypeName = logoffEvent,
                EventTypeId = 4,
                IsLoggedOn = false,
                TriggersEvent = false,
            };
            eventTypes.Add(eventType);
            mockTimeManagementData.Stub(x => x.EventTypes).Return(eventTypes);
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
                x.AddDependency(mockTimeManagementData);
                x.AddDependency(mockUserManagement);
                x.AddDependency(mockLogger);
                x.AddDependency(mockDates);
                x.AddDependency(eventManagement);
            });
        }

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
    }
}
