using LogonTimes.DataModel;
using LogonTimes.IoC;
using System.Windows.Forms;

namespace LogonTimes.UI
{
    public partial class WorkingItems : Form
    {
        private delegate void SetCallback();
        private IWorkingItemsData dataAccess;
        System.Timers.Timer timer = new System.Timers.Timer();
        int itemsLeft = 0;

        public WorkingItems()
        {
            InitializeComponent();
            dataAccess = IocRegistry.GetInstance<IWorkingItemsData>();
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
                itemsLeft = dataAccess.WorkingItemCount;
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
