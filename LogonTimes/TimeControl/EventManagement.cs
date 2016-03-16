using LogonTimes.DataModel;
using LogonTimes.DateHandling;
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
        private IDates dates;
        private List<LogonTime> logonTimesToday;
        private DateTime logonTimesLastLoaded;

        public EventManagement()
        {
            logger = IocRegistry.GetInstance<ILogger>();
            dataAccess = IocRegistry.GetInstance<ITimeManagementData>();
            dates = IocRegistry.GetInstance<IDates>();
            LoadLogonTimes();
        }
        #endregion constructors

        #region public properties
        public Person CurrentPerson { get; set; }
        public LogonTime CurrentEvent { get; set; }
        public List<LogonTime> LogonTimesToday
        {
            get
            {
                if (logonTimesLastLoaded != dates.Today)
                {
                    LoadLogonTimes();
                }
                return logonTimesToday;
            }
        }
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

        #region Logging
        private void LogStartCurrentEvent(StringBuilder message, int eventTypeId)
        {
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
        }

        private void LogUpdatingCurrentEvent(StringBuilder message)
        {
            logger.AddLineToMessage(message, "Updating current event with previous event ID");
            message.Append("Current event ID: ");
            logger.AddLineToMessage(message, CurrentEvent.LogonTimeId.ToString());
            message.Append("Corresponding event ID: ");
            logger.AddLineToMessage(message, CurrentEvent.CorrespondingEventId.ToString());
        }

        private void LogUpdatingPreviousEvent(StringBuilder message, LogonTime previousLogon)
        {
            logger.AddLineToMessage(message, "Updating previous event with current event ID");
            message.Append("Previous event ID: ");
            logger.AddLineToMessage(message, previousLogon.LogonTimeId.ToString());
            message.Append("Corresponding event ID: ");
            logger.AddLineToMessage(message, previousLogon.CorrespondingEventId.ToString());
        }
        #endregion Logging

        #region public methods
        public void LoadLogonTimes()
        {
            logonTimesToday = dataAccess.LogonTimes.Where(x => x.EventTime >= dates.Today).ToList();
            logonTimesLastLoaded = dates.Today;
        }

        public void UpdateCurrentEvent(int? newEventId)
        {
            if (CurrentEvent == null)
            {
                return;
            }
            CurrentEvent.EventTime = dates.Now;
            if (newEventId.HasValue)
            {
                CurrentEvent.EventTypeId = newEventId.Value;
            }
            dataAccess.AddOrUpdateLogonTime(CurrentEvent);
        }

        public void CreateCurrentEvent(int eventTypeId)
        {
            var message = new StringBuilder();
            LogStartCurrentEvent(message, eventTypeId);
            if (CurrentPerson != null && CurrentPerson.IsRestricted)
            {
                var newEvent = dataAccess.GetEventType(eventTypeId);
                logger.AddLineToMessage(message, "User is restricted");
                if (CurrentEvent != null && !CurrentEvent.EventType.IsLoggedOn && !newEvent.IsLoggedOn)
                {
                    CurrentEvent.EventTime = dates.Now;
                    CurrentEvent.EventTypeId = eventTypeId;
                    dataAccess.AddOrUpdateLogonTime(CurrentEvent);
                }
                CurrentEvent = new LogonTime
                {
                    EventTime = dates.Now,
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
                        LogUpdatingCurrentEvent(message);
                        dataAccess.AddOrUpdateLogonTime(CurrentEvent);

                        previousLogon.CorrespondingEventId = CurrentEvent.LogonTimeId;
                        LogUpdatingPreviousEvent(message, previousLogon);
                        dataAccess.AddOrUpdateLogonTime(previousLogon);
                    }
                }
            }
            logger.Log(message.ToString(), DebugLevels.Debug);
        }
        #endregion public methods
    }
}
