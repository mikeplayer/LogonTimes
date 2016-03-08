using LogonTimes.DataModel;
using LogonTimes.SystemService;
using System;
using System.Windows.Forms;

namespace LogonTimes.UI
{
    public partial class TestServiceRunning : Form
    {
        delegate void SetTextCallback(string text);
        private TimeManagement timeManagement = new TimeManagement();
        System.Timers.Timer timer = new System.Timers.Timer();
        bool timerRunning = false;
        int noOfTicks = 0;

        public TestServiceRunning()
        {
            InitializeComponent();
            timer.Interval = 15000;
            timer.Elapsed += Timer_Elapsed;
            LoadEventTypes();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeManagement.UpdateLogins();
            noOfTicks++;
            if (noOfTicks == 1)
            {
                SetLabelText(string.Format("Ticked (1) time"));
            }
            else
            {
                SetLabelText(string.Format("Ticked ({0}) times", noOfTicks));
            }
        }

        private void SetLabelText(string text)
        {
            if (lblTicked.InvokeRequired)
            {
                SetTextCallback callback = new SetTextCallback(SetLabelText);
                Invoke(callback, new object[] { text });
            }
            else
            {
                lblTicked.Text = text;
            }
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

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            timerRunning = !timerRunning;
            if (timerRunning)
            {
                btnStartTimer.Text = "Stop timer";
                timer.Start();
            }
            else
            {
                btnStartTimer.Text = "Start timer";
                timer.Stop();
            }
        }
    }
}
