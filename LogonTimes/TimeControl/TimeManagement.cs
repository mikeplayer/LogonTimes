using System.Collections.Generic;
using System.Linq;
using System;
using Cassia;
using System.Threading;
using System.Text;
using LogonTimes.DataModel;
using LogonTimes.Logging;
using LogonTimes.People;
using LogonTimes.IoC;
using LogonTimes.DateHandling;

namespace LogonTimes.TimeControl
{
    public class TimeManagement : ITimeManagement
    {
        private IUserManagement userManagement;
        private IEventManagement eventManagement;
        private ITerminalServicesSession currentSession = null;
        private IDates dates;
        private DateTime currentDateTime;
        private int pendingEventId;
        private int newDayLogonEventId;
        private bool logoffMessageGiven = false;
        private bool tenMinuteWarningIssued = false;
        private bool fiveMinuteWarningIssued = false;
        private ITimeManagementData dataAccess;
        private ILogger logger;

        #region constructors
        public TimeManagement()
        {
            dataAccess = IocRegistry.GetInstance<ITimeManagementData>();
            userManagement = IocRegistry.GetInstance<IUserManagement>();
            eventManagement = IocRegistry.GetInstance<IEventManagement>();
            logger = IocRegistry.GetInstance<ILogger>();
            dates = IocRegistry.GetInstance<IDates>();
            currentDateTime = dates.Now;
            pendingEventId = dataAccess.EventTypes.First(x => x.EventTypeName.Equals("Pending")).EventTypeId;
            newDayLogonEventId = dataAccess.EventTypes.First(x => x.EventTypeName.Equals("NewDayLogon")).EventTypeId;
        }
        #endregion

        #region private methods
        private LogonTimeAllowed GetLogonTimeAllowed(DateTime timeWanted)
        {
            if (eventManagement.CurrentPerson == null)
            {
                return null;
            }
            int dayNumber = (int)timeWanted.DayOfWeek;
            return eventManagement.CurrentPerson.LogonTimesAllowed.First(x => x.DayNumber == dayNumber
                && x.PersonId == eventManagement.CurrentPerson.PersonId
                && x.TimePeriod.PeriodStart.TimeOfDay <= timeWanted.TimeOfDay
                && x.TimePeriod.PeriodEnd.TimeOfDay > timeWanted.TimeOfDay);
        }

        private void LogSessionOff()
        {
            var message = new StringBuilder();
            message.Append("User ");
            message.Append(currentSession.UserName);
            message.Append(" has been automatically logged off");
            logger.Log(message.ToString(), DebugLevels.Info);
            currentSession.MessageBox("Logging off now"
                , "You are not permitted to be logged on at this time and will be logged off automatically"
                , RemoteMessageBoxButtons.Ok
                , RemoteMessageBoxIcon.Stop
                , RemoteMessageBoxDefaultButton.Button1
                , RemoteMessageBoxOptions.None
                , TimeSpan.FromSeconds(3)
                , true);
            Thread.Sleep(500);
            currentSession.Disconnect(false);
        }

        private void IssueWarning(int noOfMinutes)
        {
            var message = new StringBuilder();
            message.Append("User ");
            message.Append(currentSession.UserName);
            message.Append(" has received a ");
            message.Append(noOfMinutes);
            message.Append(" minute warning");
            logger.Log(message.ToString(), DebugLevels.Debug);
            message = new StringBuilder();
            if (noOfMinutes == 0 || noOfMinutes == 1)
            {
                message.Append("You will be automatically logged off in 1 minute");
            }
            else
            {
                message.Append("You will be automatically logged off in ");
                message.Append(noOfMinutes);
                message.Append(" minutes");
            }
            currentSession.MessageBox(message.ToString()
                , "You are nearly out of time"
                , RemoteMessageBoxButtons.Ok
                , RemoteMessageBoxIcon.Stop
                , RemoteMessageBoxDefaultButton.Button1
                , RemoteMessageBoxOptions.None
                , TimeSpan.FromSeconds(3)
                , false);
            if (noOfMinutes <= 5)
            {
                fiveMinuteWarningIssued = true;
            }
            tenMinuteWarningIssued = true;  //Don't want to issue a 10 minute warning if the 5 minute warning has already been done
        }

        private TimeSpan HoursLoggedOnToday
        {
            get
            {
                TimeSpan result = new TimeSpan(0);
                var currentLogons = eventManagement.LogonTimesToday.Where(x => x.PersonId == eventManagement.CurrentPerson.PersonId && !x.EventType.IsLoggedOn);
                foreach (var logon in currentLogons)
                {
                    result = result.Add(logon.LoggedOnDuration);
                }
                return result;
            }
        }

        private void CheckUserState()
        {
            dataAccess.CheckForUpdates();
            var message = new StringBuilder();
            logger.AddLineToMessage(message, "Check user state");
            if (currentSession != null && eventManagement.CurrentPerson != null && eventManagement.CurrentPerson.IsRestricted)
            {
                logger.AddLineToMessage(message, string.Format("current session != null and {0} is restricted", currentSession.UserName));
                int minutesRemaining = (int)((eventManagement.CurrentPerson.MaximumHoursToday - HoursLoggedOnToday.TotalHours) * 60);
                DateTime today = dates.Today;
                DateTime currentDateTime = dates.Now;
                DateTime fiveMinuteWarning = currentDateTime.AddMinutes(5);
                DateTime tenMinuteWarning = currentDateTime.AddMinutes(10);
                if (!logoffMessageGiven && (!GetLogonTimeAllowed(currentDateTime).Permitted || minutesRemaining <= 0))
                {
                    LogSessionOff();
                    logoffMessageGiven = true;
                    return;
                }
                var currentPersonLoginTimes = eventManagement.LogonTimesToday.Where(x => x.PersonId == eventManagement.CurrentPerson.PersonId);
                var nextLogonTime = GetLogonTimeAllowed(fiveMinuteWarning);
                if (!fiveMinuteWarningIssued && (!nextLogonTime.Permitted || minutesRemaining < 5))
                {
                    if (!nextLogonTime.Permitted)
                    {
                        DateTime nextTimespanTime = today.Add(nextLogonTime.TimePeriod.PeriodStart.TimeOfDay);
                        var difference = nextTimespanTime.Subtract(currentDateTime).TotalMinutes;
                        IssueWarning((int)difference);
                    }
                    else
                    {
                        IssueWarning(minutesRemaining);
                    }
                    return;
                }
                nextLogonTime = GetLogonTimeAllowed(tenMinuteWarning);
                if (!tenMinuteWarningIssued && (!nextLogonTime.Permitted || minutesRemaining < 10))
                {
                    if (!nextLogonTime.Permitted)
                    {
                        DateTime nextTimespanTime = today.Add(nextLogonTime.TimePeriod.PeriodStart.TimeOfDay);
                        var difference = nextTimespanTime.Subtract(currentDateTime).TotalMinutes;
                        IssueWarning((int)difference);
                    }
                    else
                    {
                        IssueWarning(minutesRemaining);
                    }
                    return;
                }
                //int total
            }
            logger.Log(message.ToString(), DebugLevels.Debug);
        }

        private void SetCurrentPersonLoggedOff(StringBuilder message, string userName, EventType eventType)
        {
            AddToLogMessage(message, "Log off event", DebugLevels.Debug);
            if (string.IsNullOrEmpty(userName) && eventManagement.CurrentPerson == null)
            {
                return;     //Can't do anything with this - the person must have been logged on before this process started so we don't know who they are
            }
            if (!string.IsNullOrEmpty(userName))
            {
                if (eventManagement.CurrentPerson == null || !userName.Equals(eventManagement.CurrentPerson.LogonName))
                {
                    eventManagement.CurrentPerson = userManagement.GetPersonDetail(userName);
                    LogSetCurrentUser(message);
                    eventManagement.CurrentEvent = null;    //We don't want to overwrite any previously saved events, so force the creation of a new one
                }
            }
            if (eventManagement.CurrentEvent == null)
            {
                eventManagement.CreateCurrentEvent(eventType.EventTypeId);
            }
            eventManagement.UpdateCurrentEvent(eventType.EventTypeId);
            eventManagement.CurrentPerson = null;
            eventManagement.CurrentEvent = null;
            return;
        }

        private void RegisterEvent(StringBuilder message, string userName, EventType eventType)
        {
            AddToLogMessage(message, "Event type not null and triggers event", DebugLevels.Debug);
            if (!eventType.IsLoggedOn)
            {
                SetCurrentPersonLoggedOff(message, userName, eventType);
            }
            AddToLogMessage(message, "Log on event", DebugLevels.Debug);
            if (string.IsNullOrEmpty(userName))
            {
                AddToLogMessage(message, "Username empty", DebugLevels.Debug);
                return;     //Can't do anything here.  unlikely to happen anyway as the only events with no name appear to be logoff events
            }
            eventManagement.CurrentPerson = userManagement.GetPersonDetail(userName);
            LogSetCurrentUser(message);
            eventManagement.CreateCurrentEvent(eventType.EventTypeId);
            if (eventType.IsLoggedOn)
            {
                eventManagement.CreateCurrentEvent(pendingEventId);
            }
            logoffMessageGiven = false;
            tenMinuteWarningIssued = false;
            fiveMinuteWarningIssued = false;
            CheckUserState();
        }
        #endregion private methods

        #region Logging stuff
        private void LogNewSessionEvent(StringBuilder message, ITerminalServicesSession session, string sessionEvent)
        {
            if (logger.ShouldLog(DebugLevels.Info))
            {
                message.Append("New session event ");
                logger.AddLineToMessage(message, sessionEvent);
                message.Append("User: ");
                logger.AddLineToMessage(message, session.UserName);
            }
            if (logger.ShouldLog(DebugLevels.Debug))
            {
                message.Append("Current user: ");
                if (eventManagement.CurrentPerson == null)
                {
                    message.Append("(Not set)");
                }
                else
                {
                    message.Append(eventManagement.CurrentPerson.LogonName);
                }
                logger.AddLineToMessage(message, string.Empty);
                if (eventManagement.CurrentEvent == null)
                {
                    message.Append("No current event");
                }
                else
                {
                    logger.AddLineToMessage(message, "Current event exists");
                    message.Append(eventManagement.CurrentEvent.PersonId);
                    logger.AddLineToMessage(message, string.Empty);
                    message.Append(eventManagement.CurrentEvent.EventTypeId);
                }
                logger.AddLineToMessage(message, string.Empty);
            }
        }

        private void AddToLogMessage(StringBuilder message, string lineToLog, DebugLevels levelRequired)
        {
            if (logger.ShouldLog(levelRequired))
            {
                logger.AddLineToMessage(message, lineToLog);
            }
        }

        private void LogSetCurrentUser(StringBuilder message)
        {
            if (logger.ShouldLog(DebugLevels.Debug))
            {
                message.Append("Just set current user (Not logon event): ");
                if (eventManagement.CurrentPerson == null)
                {
                    message.Append("(Not set)");
                }
                else
                {
                    message.Append(eventManagement.CurrentPerson.LogonName);
                }
                logger.AddLineToMessage(message, string.Empty);
            }
        }
        #endregion Logging stuff

        #region public methods

        public void NewSessionEvent(ITerminalServicesSession session, string sessionEvent)
        {
            var message = new StringBuilder();
            try
            {
                LogNewSessionEvent(message, session, sessionEvent);
                currentSession = session;
                string userName = session.UserName;
                EventType eventType = null;
                try
                {
                    eventType = dataAccess.EventTypes.First(x => x.EventTypeName.Equals(sessionEvent));
                }
                catch (Exception ex)
                {
                    logger.LogException(string.Format("Error occurred retrieving event type {0}", sessionEvent), DebugLevels.Error, ex);
                }
                if (eventType != null && eventType.TriggersEvent)
                {
                    RegisterEvent(message, userName, eventType);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(string.Format("Error occurred in NewSessionEvent {0}", sessionEvent), DebugLevels.Error, ex);
            }
            logger.Log(message.ToString(), DebugLevels.Info);
        }

        public void UpdateLogins()
        {
            if (eventManagement.CurrentEvent == null)
            {
                return;
            }
            logoffMessageGiven = false;
            var message = new StringBuilder();
            message.Append("Update logins");
            bool refreshLogonTimes = false;
            var newDateTime = dates.Now;
            eventManagement.UpdateCurrentEvent(null);
            if (newDateTime.Day != currentDateTime.Day)
            {
                message.Append("New day");
                eventManagement.CreateCurrentEvent(newDayLogonEventId);
                eventManagement.CreateCurrentEvent(pendingEventId);
                refreshLogonTimes = true;
            }
            CheckUserState();
            currentDateTime = newDateTime;
            if (refreshLogonTimes)
            {
                message.Append("Refreshing logon times");
                eventManagement.LoadLogonTimes();
            }
            logger.Log(message.ToString(), DebugLevels.Debug);
        }
        #endregion
    }
}
