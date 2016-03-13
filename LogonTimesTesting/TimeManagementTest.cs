using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using LogonTimes.TimeControl;
using LogonTimes.People;
using LogonTimes.DataModel;
using System.Collections.Generic;
using LogonTimes.Logging;
using Cassia;

namespace LogonTimesTesting
{
    [TestClass]
    public class TimeManagementTest : BaseTest
    {
        private ITerminalServicesSession mockTerminalServicesSession;
        ITimeManagementData mockTimeManagementData;
        IUserManagement mockUserManagement;
        ILogger mockLogger;

        [TestInitialize]
        public void SetupTests()
        {
            SetupTimeManagement();
        }

        private TimeManagement SetupTimeManagement()
        {
            mockTerminalServicesSession = MockRepository.GenerateMock<ITerminalServicesSession>();
            mockTimeManagementData = MockRepository.GenerateMock<ITimeManagementData>();
            mockUserManagement = MockRepository.GenerateMock<IUserManagement>();
            mockLogger = MockRepository.GenerateMock<ILogger>();
            var eventTypes = new List<EventType>();
            var eventType = new EventType
            {
                EventTypeName = "Pending",
                EventTypeId = 1
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = "NewDayLogon",
                EventTypeId = 2
            };
            eventTypes.Add(eventType);
            mockTimeManagementData.Stub(x => x.EventTypes).Return(eventTypes);
            mockTimeManagementData.Stub(x => x.LogonTimes).Return(new List<LogonTime>());
            mockLogger.Stub(x => x.ShouldLog(Arg<DebugLevels>.Is.Anything)).Return(false);
            Initialise(x =>
            {
                x.AddDependency(mockTimeManagementData);
                x.AddDependency(mockUserManagement);
                x.AddDependency(mockLogger);
            });
            return new TimeManagement();
        }

        [TestMethod]
        public void LogonTimesAllowed_NoLogonTimes_NothingReturned()
        {
            var mockTimeManagement = MockRepository.GenerateMock<ITimeManagementData>();
            var mockUserManagement = MockRepository.GenerateMock<IUserManagementData>();
            var eventTypes = new List<EventType>();
            var eventType = new EventType
            {
                EventTypeName = "Pending",
                EventTypeId = 1
            };
            eventTypes.Add(eventType);
            eventType = new EventType
            {
                EventTypeName = "NewDayLogon",
                EventTypeId = 2
            };
            eventTypes.Add(eventType);
            mockTimeManagement.Stub(x => x.EventTypes).Return(eventTypes);
            mockTimeManagement.Stub(x => x.LogonTimes).Return(new List<LogonTime>());
            Initialise(x =>
            {
                x.AddDependency(mockTimeManagement);
                x.AddDependency(mockUserManagement);
            });
            var timeManagement = new TimeManagement();
            mockTimeManagement.AssertWasCalled(x => x.LogonTimes);
            mockTimeManagement.AssertWasCalled(x => x.EventTypes);
        }
    }
}
