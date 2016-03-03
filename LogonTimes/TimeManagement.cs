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

        private void CreateCurrentEvent(int eventTypeId)
        {
            var message = new StringBuilder();
            message.Append("Create current event");
            message.Append(crlf);
            if (currentPerson.IsRestricted)
            {
                message.Append("User is restricted");
                message.Append(crlf);
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
                        DataAccess.Instance.AddOrUpdateLogonTime(currentEvent);
                        previousLogon.CorrespondingEventId = currentEvent.LogonTimeId;
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
            var message = new StringBuilder();
            message.Append("Check user state");
            message.Append(crlf);
            if (currentSession != null && currentPerson.IsRestricted)
            {
                message.Append("current session != null and ");
                message.Append(currentSession.UserName);
                message.Append(" is restricted");
                int minutesRemaining = (int)((currentPerson.MaximumHoursToday - HoursLoggedOnToday.TotalHours) * 60);
                DateTime today = DateTime.Today;
                DateTime currentDateTime = DateTime.Now;
                DateTime fiveMinuteWarning = currentDateTime.AddMinutes(5);
                DateTime tenMinuteWarning = currentDateTime.AddMinutes(10);
                if (!GetLogonTimeAllowed(currentDateTime).Permitted || minutesRemaining <= 0)
                {
                    LogSessionOff();
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
                    message.Append(sessionEvent);
                    message.Append(crlf);
                    message.Append("User: ");
                    message.Append(session.UserName);
                    message.Append(crlf);
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
                        message.Append("Current event exists");
                        message.Append(crlf);
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
                    message.Append(sessionEvent);
                    message.Append(crlf);
                    message.Append(ex.Message);
                    message.Append(crlf);
                    message.Append(ex.StackTrace);
                    message.Append(crlf);
                }
                if (eventType != null)
                {
                    if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                    {
                        message.Append("Event type not null");
                        message.Append(crlf);
                    }
                    if (!eventType.IsLoggedOn)
                    {
                        if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                        {
                            message.Append("Log off event");
                            message.Append(crlf);
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
                        message.Append("Log on event");
                        message.Append(crlf);
                    }
                    if (string.IsNullOrEmpty(userName))
                    {
                        if (Logger.Instance.ShouldLog(DebugLevels.Debug))
                        {
                            message.Append("Username empty");
                            message.Append(crlf);
                        }
                        return;     //Can't do anything here.  unlikely to happen anyway as the only events with no name appear to be logoff events
                    }
                    currentPerson = userManagement.GetPersonDetail(userName);
                    CreateCurrentEvent(eventType.EventTypeId);
                    CreateCurrentEvent(pendingEventId);
                    tenMinuteWarningIssued = false;
                    fiveMinuteWarningIssued = false;
                    CheckUserState();
                }
            }
            catch (Exception ex)
            {
                message.Append("Error occurred in NewSessionEvent ");
                message.Append(sessionEvent);
                message.Append(crlf);
                message.Append(ex.Message);
                message.Append(crlf);
                message.Append(ex.StackTrace);
                message.Append(crlf);
            }
            Logger.Instance.Log(message.ToString(), DebugLevels.Error);
        }

        public void UpdateLogins()
        {
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
