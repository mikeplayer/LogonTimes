using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Cassia;

namespace LogonTimes.SystemService
{
    public class MockSystemService : ITerminalServicesSession
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
            get
            {
                throw new NotImplementedException();
            }
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void MessageBox(string text, string caption)
        {
            throw new NotImplementedException();
        }

        public void MessageBox(string text, string caption, RemoteMessageBoxIcon icon)
        {
            throw new NotImplementedException();
        }

        public RemoteMessageBoxResult MessageBox(string text, string caption, RemoteMessageBoxButtons buttons, RemoteMessageBoxIcon icon, RemoteMessageBoxDefaultButton defaultButton, RemoteMessageBoxOptions options, TimeSpan timeout, bool synchronous)
        {
            throw new NotImplementedException();
        }
    }
}
