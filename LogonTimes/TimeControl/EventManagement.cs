﻿using LogonTimes.DataModel;
using LogonTimes.IoC;
using LogonTimes.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogonTimes.TimeControl
{
    public class EventManagement : IEventManagement
    {
        #region constructors
        private ILogger logger;
        private ITimeManagementData dataAccess;

        public EventManagement()
        {
            logger = IocRegistry.GetInstance<ILogger>();
            dataAccess = IocRegistry.GetInstance<ITimeManagementData>();
            LoadLogonTimes();
        }
        #endregion constructors

        #region public properties
        public Person CurrentPerson { get; set; }
        public LogonTime CurrentEvent { get; set; }
        public List<LogonTime> LogonTimesToday { get; set; }
        #endregion public properties

        #region private methods

        private LogonTime PreviousLogon()
        {
            var options = LogonTimesToday.Where(x => x.PersonId == CurrentPerson.PersonId
                && x.EventType.IsLoggedOn
                && x.CorrespondingEventId == null).OrderByDescending(x => x.EventTime);
            if (options.Any())
            {
                return options.First();
            }
            return null;
        }
        #endregion private methods

        #region public methods
        public void LoadLogonTimes()
        {
            LogonTimesToday = dataAccess.LogonTimes.Where(x => x.EventTime >= DateTime.Today).ToList();
        }

        public void CreateCurrentEvent(int eventTypeId)
        {
            var message = new StringBuilder();
            logger.AddLineToMessage(message, "Create current event");
            if (CurrentPerson != null)
            {
                logger.AddLineToMessage(message, string.Format("Current person: {0}", CurrentPerson.LogonName));
            }
            if (logger.ShouldLog(DebugLevels.Debug))
            {
                var eventType = dataAccess.GetEventType(eventTypeId);
                logger.AddLineToMessage(message, eventType.EventTypeName);
            }
            if (CurrentPerson != null && CurrentPerson.IsRestricted)
            {
                logger.AddLineToMessage(message, "User is restricted");
                CurrentEvent = new LogonTime
                {
                    EventTime = DateTime.Now,
                    EventTypeId = eventTypeId,
                    PersonId = CurrentPerson.PersonId
                };
                dataAccess.AddOrUpdateLogonTime(CurrentEvent);
                LogonTimesToday.Add(CurrentEvent);
                if (!CurrentEvent.EventType.IsLoggedOn)
                {
                    var previousLogon = PreviousLogon();
                    if (previousLogon != null)
                    {
                        CurrentEvent.CorrespondingEventId = previousLogon.LogonTimeId;
                        logger.AddLineToMessage(message, "Updating current event with previous event ID");
                        message.Append("Current event ID: ");
                        logger.AddLineToMessage(message, CurrentEvent.LogonTimeId.ToString());
                        message.Append("Corresponding event ID: ");
                        logger.AddLineToMessage(message, CurrentEvent.CorrespondingEventId.ToString());
                        dataAccess.AddOrUpdateLogonTime(CurrentEvent);

                        previousLogon.CorrespondingEventId = CurrentEvent.LogonTimeId;
                        logger.AddLineToMessage(message, "Updating previous event with current event ID");
                        message.Append("Previous event ID: ");
                        logger.AddLineToMessage(message, previousLogon.LogonTimeId.ToString());
                        message.Append("Corresponding event ID: ");
                        logger.AddLineToMessage(message, previousLogon.CorrespondingEventId.ToString());
                        dataAccess.AddOrUpdateLogonTime(previousLogon);
                    }
                }
            }
            logger.Log(message.ToString(), DebugLevels.Debug);
        }
        #endregion public methods
    }
}
