using LogonTimes.DataModel;
using LogonTimes.SystemService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogonTimes
{
    public partial class TestServiceRunning : Form
    {
        private TimeManagement timeManagement = new TimeManagement();

        public TestServiceRunning()
        {
            InitializeComponent();
            LoadEventTypes();
        }

        private void LoadEventTypes()
        {
            cmbEventTypes.Items.Clear();
            foreach (var eventType in DataAccess.Instance.EventTypes)
            {
                cmbEventTypes.Items.Add(eventType);
            }
            cmbEventTypes.SelectedIndex = 0;
        }

        private void btnUpdateLogins_Click(object sender, EventArgs e)
        {
            timeManagement.UpdateLogins();
        }

        private void btnChangeSession_Click(object sender, EventArgs e)
        {
            var mockSession = new MockTerminalServicesSession();
            mockSession.UserName = txtUserName.Text;
            timeManagement.NewSessionEvent(mockSession, cmbEventTypes.SelectedItem.ToString());
            MessageBox.Show("Done");
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            var configuration = new LogonTimesConfiguration();
            configuration.Show();
        }
    }
}
