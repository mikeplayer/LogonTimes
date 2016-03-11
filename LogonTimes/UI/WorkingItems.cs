using LogonTimes.DataModel;
using System.Windows.Forms;

namespace LogonTimes.UI
{
    public partial class WorkingItems : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        int itemsLeft = 0;
        private delegate void SetCallback();

        public WorkingItems()
        {
            InitializeComponent();
            UpdateCount();
            timer.Elapsed += TimerElapsed;
            timer.Interval = 200;
            timer.Start();
        }

        private void UpdateCount()
        {
            if (lblCount.InvokeRequired)
            {
                var callback = new SetCallback(UpdateCount);
                Invoke(callback);
            }
            else
            {
                itemsLeft = DataAccess.Instance.WorkingItemCount;
                lblCount.Text = string.Format("There are currently {0} items still processing", itemsLeft);
            }
        }

        private void CloseForm()
        {
            if (InvokeRequired)
            {
                var callback = new SetCallback(CloseForm);
                Invoke(callback);
            }
            else
            {
                Close();
            }
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateCount();
            if (itemsLeft == 0)
            {
                CloseForm();
            }
        }
    }
}
