using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using Cassia;

namespace LogonTimes.SystemService
{
    public class MockTerminalServicesSession : ITerminalServicesSession
    {
        public int ClientBuildNumber
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IClientDisplay ClientDisplay
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IPAddress ClientIPAddress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ClientName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ConnectionState ConnectionState
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? ConnectTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? CurrentTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? DisconnectTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DomainName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan IdleTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? LastInputTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime? LoginTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ITerminalServer Server
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int SessionId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public NTAccount UserAccount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string UserName
        {
            get; set;
        }

        public string WindowStationName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect(bool synchronous)
        {
            MessageBox("Logged off");
        }

        public IList<ITerminalServicesProcess> GetProcesses()
        {
            throw new NotImplementedException();
        }

        public void Logoff()
        {
            throw new NotImplementedException();
        }

        public void Logoff(bool synchronous)
        {
            throw new NotImplementedException();
        }

        public void MessageBox(string text)
        {
            MessageBox(text, "");
        }

        public void MessageBox(string text, string caption)
        {
            MessageBox(text, caption, RemoteMessageBoxIcon.Information);
        }

        public void MessageBox(string text, string caption, RemoteMessageBoxIcon icon)
        {
            MessageBox(text, caption, RemoteMessageBoxButtons.Ok, icon, RemoteMessageBoxDefaultButton.Button1, RemoteMessageBoxOptions.None, new TimeSpan(0, 0, 1), true);
        }

        public RemoteMessageBoxResult MessageBox(string text, string caption, RemoteMessageBoxButtons buttons, RemoteMessageBoxIcon icon, RemoteMessageBoxDefaultButton defaultButton, RemoteMessageBoxOptions options, TimeSpan timeout, bool synchronous)
        {
            System.Windows.Forms.MessageBox.Show(text, caption);
            return RemoteMessageBoxResult.Ok;
        }
    }
}
