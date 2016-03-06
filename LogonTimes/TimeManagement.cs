using System.Collections.Generic;
using System.Linq;
using System;
using Cassia;
using System.Threading;
using System.Text;
using LogonTimes.DataModel;
using LogonTimes.Logging;

namespace LogonTimes
{
    public class TimeManagement
    {
        private UserManagement userManagement = new UserManagement();
        private List<LogonTime> logonTimesToday;
        private ITerminalServicesSession currentSession = null;
        private DateTime currentDateTime = DateTime.Now;
        private Person currentPerson = null;
        private LogonTime currentEvent = null;
        private int pendingEventId;
        private int newDayLogonEventId;
        private bool logoffMessageGiven = false;
        private bool tenMinuteWarningIssued = false;
        private bool fiveMinuteWarningIssued = false;
        private const string crlf = "\r\n";

        #region constructors
        public TimeManagement()
        {
            pendingEventId = DataAccess.Instance.EventTypes.First(x => x.EventTypeName.Equals("Pending")).EventTypeId;
            newDayLogonEventId = DataAccess.Instance.EventTypes.First(x => x.EventTypeName.Equals("NewDayLogon")).EventTypeId;
            LoadLogonTimes();
        }
        #endregion

        #region private methods
        private void LoadLogonTimes()
        {
            logonTimesToday = DataAccess.Instance.LogonTimes.Where(x => x.EventTime >= DateTime.Today).ToList();
        }

        private LogonTime PreviousLogon()
        {
            var options = logonTimesToday.Where(x => x.PersonId == currentPerson.PersonId
                && x.EventType.IsLoggedOn
                && x.CorrespondingEventId == null).OrderByDescending(x => x.EventTime);
            if (options.Any())
            {
                return options.First();
            }
            return null;
        }

        private UserManagement UserManagement
        {
            get
            {
                if (userManagement == null)
                {
                    userManagement = new UserManagement();
                }
                return userManagement;
            }
        }

        private void AddLineToMessage(StringBuilder stringBuilder, string message)
        {
            stringBuilder.Append(message);
            stringBuilder.Append(crlf);
        }

        private void CreateCurrentEvent(int eventTypeId)
        {
            var message = new StringBuilder();
            AddLineToMessage(message, "Create current event");
            if (currentPerson != null)
            {
                AddLineToMessage(message, string.Format("Current person: {0}", currentPerson.LogonName));
            }
            if (currentPerson != null && currentPerson.IsRestricted)
            {
                AddLineToMessage(message, "User is restricted");
                currentEvent = new LogonTime
                {
                    EventTime = DateTime.Now,
                    EventTypeId = eventTypeId,
                    PersonId = currentPerson.PersonId
                };
                DataAccess.Instance.AddOrUpdateLogonTime(currentEvent);
                logonTimesToday.Add(currentEvent);
                if (!currentEvent.EventType.IsLoggedOn)
                {
                    var previousLogon = PreviousLogon();
                    if (previousLogon != null)
                    {
                        currentEvent.CorrespondingEventId = previousLogon.LogonTimeId;
                        AddLineToMessage(message, "Updating current event with previous event ID");
                        message.Append("Current event ID: ");
                        AddLineToMessage(message, currentEvent.LogonTimeId.ToString());
                        message.Append("Corresponding event ID: ");
                        AddLineToMessage(message, currentEvent.CorrespondingEventId.ToString());
                        DataAccess.Instance.AddOrUpdateLogonTime(currentEvent);

                        previousLogon.CorrespondingEventId = currentEvent.LogonTimeId;
                        AddLineToMessage(message, "Updating previous event with current event ID");
                        message.Append("Previous event ID: ");
                        AddLineToMessage(message, previousLogon.LogonTimeId.ToString());
                        message.Append("Corresponding event ID: ");
                        AddLineToMessage(message, previousLogon.CorrespondingEventId.ToString());
                        DataAccess.Instance.AddOrUpdateLogonTime(previousLogon);
                    }
                }
            }
            Logger.Instance.Log(message.ToString(), DebugLevels.Debug);
        }

        private LogonTimeAllowed GetLogonTimeAllowed(DateTime timeWanted)
        {
            if (currentPerson == null)
            {
                return null;
            }
            int dayNumber = (int)timeWanted.DayOfWeek;
            return currentPerson.LogonTimesAllowed.First(x => x.DayNumber == dayNumber
                && x.PersonId == currentPerson.PersonId
                && x.TimePeriod.PeriodStart.TimeOfDay <= timeWanted.TimeOfDay
                && x.TimePeriod.PeriodEnd.TimeOfDay > timeWanted.TimeOfDay);
        }

        private void LogSessionOff()
        {
            var message = new StringBuilder();
            message.Append("User ");
            message.Append(currentSession.UserName);
            message.Append(" has been automatically logged off");
            Logger.Instance.Log(message.ToString(), DebugLevels.Info);
            currentSession.MessageBox("Logging off now"
                , "You are not permitted to be logged on at this time and will be logged off automatically"
                , RemoteMessageBoxButtons.Ok
                , RemoteMessageBoxIcon.Stop
                , RemoteMessageBoxDefaultButton.Button1
                , RemoteMessageBoxOptions.None
                , TimeSpan.FromSeconds(3)
                , false);
            Thread.Sleep(3000);
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
            Logger.Instance.Log(message.ToString(), DebugLevels.Debug);
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
                var currentLogons = logonTimesToday.Where(x => x.PersonId == currentPerson.PersonId && !x.EventType.IsLoggedOn);
                foreach (var logon in currentLogons)
                {
                    result = result.Add(logon.LoggedOnDuration);
                }
                return result;
            }
        }

        private void CheckUserState()
        {
            DataAccess.Instance.CheckForUpdates();
            var message = new StringBuilder();
            AddLineToMessage(message, "Check user state");
            if (currentSession != null && currentPerson != null && currentPerson.IsRestricted)
            {
                AddLineToMessage(message, string.Format("current session != null and {0} is restricted", currentSession.UserName));
                int minutesRemaining = (int)((currentPerson.MaximumHoursToday - HoursLoggedOnToday.TotalHours) * 60);
                DateTime today = DateTime.Today;
                DateTime currentDateTime = DateTime.Now;
                DateTime fiveMinuteWarning = currentDateTime.AddMinutes(5);
                DateTime tenMinuteWarning = currentDateTime.AddMinutes(10);
                if (!logoffMessageGiven || !GetLogonTimeAllowed(currentDateTime).Permitted || minutesRemaining <= 0)
                //if (!GetLogonTimeAllowed(currentDateTime).Permitted || minutesRemaining <= 0)
                {
                    LogSessionOff();
                    logoffMessageGiven = true;
                    return;
                }
                var currentPersonLoginTimes = logonTimesToday.Where(x => x.PersonId == currentPerson.PersonId);
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
            Logger.Instance.Log(message.ToString(), DebugLevels.Debug);
        }
        #endregion

        #region public methods
        public List<LogonTimeAllowed> LogonTimesAllowed(string userName)
        {
            Person person = UserManagement.GetPersonDetail(userName);
            return person.LogonTimesAllowed
                .OrderBy(x => x.DayNumber)
                .ThenBy(x => x.TimePeriod.PeriodStart)
                .ToList();
        }

        //public void UpdateLogonTimesAllowed()
        //{
        //    data.SubmitChanges();
        //    data.RefreshCache();
        //}

        public void NewSessionEvent(ITerminalServicesSession session, string sessionEvent)
        {
            var message = new StringBuilder();
            try
            {
                if (Logger.Instance.ShouldLog(DebugLevels.Info))
                {
                    message.Append("New session event ");
                    AddLineToMessage(message, sessionEvent);
                    message.Append("User: ");
                    AddLineToMessage(message, session.UserName);
                }
                if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                {
                    message.Append("Current user: ");
                    if (currentPerson == null)
                    {
                        message.Append("(Not set)");
                    }
                    else
                    {
                        message.Append(currentPerson.LogonName);
                    }
                    message.Append(crlf);
                    if (currentEvent == null)
                    {
                        message.Append("No current event");
                    }
                    else
                    {
                        AddLineToMessage(message, "Current event exists");
                        message.Append(currentEvent.PersonId);
                        message.Append(crlf);
                        message.Append(currentEvent.EventTypeId);
                    }
                    message.Append(crlf);
                }
                currentSession = session;
                string userName = session.UserName;
                EventType eventType = null;
                try
                {
                    eventType = DataAccess.Instance.EventTypes.First(x => x.EventTypeName.Equals(sessionEvent));
                }
                catch (Exception ex)
                {
                    message.Append("Error occurred retrieving event type ");
                    AddLineToMessage(message, sessionEvent);
                    AddLineToMessage(message, ex.Message);
                    AddLineToMessage(message, ex.StackTrace);
                }
                if (eventType != null && eventType.TriggersEvent)
                {
                    if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                    {
                        AddLineToMessage(message, "Event type not null");
                    }
                    if (!eventType.IsLoggedOn)
                    {
                        if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                        {
                            AddLineToMessage(message, "Log off event");
                        }
                        if (string.IsNullOrEmpty(userName) && currentPerson == null)
                        {
                            return;     //Can't do anything with this - the person must have been logged on before this process started so we don't know who they are
                        }
                        if (!string.IsNullOrEmpty(userName))
                        {
                            if (currentPerson == null || !userName.Equals(currentPerson.LogonName))
                            {
                                currentPerson = userManagement.GetPersonDetail(userName);
                                if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                                {
                                    message.Append("Just set current user (Not logon event): ");
                                    if (currentPerson == null)
                                    {
                                        message.Append("(Not set)");
                                    }
                                    else
                                    {
                                        message.Append(currentPerson.LogonName);
                                    }
                                    message.Append(crlf);
                                }
                                currentEvent = null;    //We don't want to overwrite any previously saved events, so force the creation of a new one
                            }
                        }
                        if (currentEvent == null)
                        {
                            CreateCurrentEvent(eventType.EventTypeId);
                        }
                        else
                        {
                            currentEvent.EventTypeId = eventType.EventTypeId;
                        }
                        if (currentEvent != null)
                        {
                            DataAccess.Instance.AddOrUpdateLogonTime(currentEvent);
                        }
                        currentPerson = null;
                        currentEvent = null;
                        return;
                    }
                    if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                    {
                        AddLineToMessage(message, "Log on event");
                    }
                    if (string.IsNullOrEmpty(userName))
                    {
                        if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                        {
                            AddLineToMessage(message, "Username empty");
                        }
                        return;     //Can't do anything here.  unlikely to happen anyway as the only events with no name appear to be logoff events
                    }
                    currentPerson = userManagement.GetPersonDetail(userName);
                    if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                    {
                        message.Append("Just set current user (end of NewSessionEvent): ");
                        if (currentPerson == null)
                        {
                            message.Append("(Not set)");
                        }
                        else
                        {
                            message.Append(currentPerson.LogonName);
                        }
                        message.Append(crlf);
                    }
                    CreateCurrentEvent(eventType.EventTypeId);
                    CreateCurrentEvent(pendingEventId);
                    logoffMessageGiven = false;
                    tenMinuteWarningIssued = false;
                    fiveMinuteWarningIssued = false;
                    CheckUserState();
                }
            }
            catch (Exception ex)
            {
                message.Append("Error occurred in NewSessionEvent ");
                AddLineToMessage(message, sessionEvent);
                AddLineToMessage(message, ex.Message);
                AddLineToMessage(message, ex.StackTrace);
            }
            Logger.Instance.Log(message.ToString(), DebugLevels.Error);
        }

        public void UpdateLogins()
        {
            logoffMessageGiven = false;
            var message = new StringBuilder();
            message.Append("Update logins");
            bool refreshLogonTimes = false;
            var newDateTime = DateTime.Now;
            if (currentEvent != null)
            {
                currentEvent.EventTime = DateTime.Now;
            }
            if (newDateTime.Day != currentDateTime.Day)
            {
                message.Append("New day");
                CreateCurrentEvent(newDayLogonEventId);
                CreateCurrentEvent(pendingEventId);
                refreshLogonTimes = true;
            }
            DataAccess.Instance.AddOrUpdateLogonTime(currentEvent);
            CheckUserState();
            currentDateTime = newDateTime;
            if (refreshLogonTimes)
            {
                message.Append("Refreshing logon times");
                LoadLogonTimes();
            }
            Logger.Instance.Log(message.ToString(), DebugLevels.Debug);
        }
        #endregion
    }
}
