using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using DevAge.Drawing;
using System.Text;
using LogonTimes.DataModel;
using LogonTimes.TimeControl;
using LogonTimes.People;
using System.Security.Principal;
using LogonTimes.IoC;
using LogonTimes.DateHandling;
using Microsoft.Win32;
using LogonTimes.Applications;

namespace LogonTimes.UI
{
    public partial class LogonTimesConfiguration : Form
    {
        private ILogonTimesConfigurationData dataAccess;
        private IUserManagement userManagement;
        private ITimeManagement timeManagement;
        private IApplicationManagement applicationManagement;
        private IDates dates;
        private bool isLoading = true;
        private int currentFocusItem;
        private SourceGrid.Cells.Views.Cell permittedCellView;
        private SourceGrid.Cells.Views.Cell blockedCellView;
        private WhenAllowedValuesList whenAllowedValuesList = new WhenAllowedValuesList();
        private WhenAllowedValues currentHoverTarget = null;
        private delegate void SetPersonCallback(Person person);
        private delegate void SetLoadingCallback(bool isLoading);

        #region Initialisation
        public LogonTimesConfiguration()
        {
            InitializeComponent();
            dataAccess = IocRegistry.GetInstance<ILogonTimesConfigurationData>();
            timeManagement = IocRegistry.GetInstance<ITimeManagement>();
            userManagement = IocRegistry.GetInstance<IUserManagement>();
            applicationManagement = IocRegistry.GetInstance<IApplicationManagement>();
            dates = IocRegistry.GetInstance<IDates>();
            CheckPermissions();
            LoadUsers();
            SetupLogonTimesGrid();
            LoadPrograms();
        }

        private void CheckPermissions()
        {
            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) ? true : false;
            if (!isAdmin)
            {
                MessageBox.Show("This program must be run as an administrator\r\nRight click and select 'Run as administrator'", "Must run as an administrator");
                System.Windows.Forms.Application.Exit();
            }
        }

        private void AddPerson(Person person)
        {
            if (listPeople.InvokeRequired)
            {
                SetPersonCallback personCallback = new SetPersonCallback(AddPerson);
                Invoke(personCallback, new object[] { person });
            }
            else
            {
                ListViewItem personItem = new ListViewItem(person.LogonName);
                personItem.Checked = userManagement.PersonIsRestricted(person);
                listPeople.Items.Add(personItem);
            }
        }

        private void LoadUsers()
        {
            SetIsLoadingPeople(true);
            userManagement.PersonLoadedFromUserAccount += UserManagementPersonLoaded;
            userManagement.PersonLoadingComplete += UserManagementPersonLoadingComplete;
            var people = userManagement.LoadPeople();

            foreach (var person in people)
            {
                AddPerson(person);
            }
            SetItemAvailability();
        }

        private void LoadPrograms()
        {
            DateTime start = DateTime.Now;
            foreach (var application in applicationManagement.Applications)
            {
                string[] items = { application.ApplicationName, application.ApplicationPath };
                ListViewItem applicationItem = new ListViewItem(items);
                lvApplications.Items.Add(applicationItem);
            }
            DateTime end = DateTime.Now;
            var difference = end.Subtract(start);
            MessageBox.Show(string.Format("Load software time: {0}", difference));
        }

        private void SetIsLoadingPeople(bool isLoading)
        {
            if (listPeople.InvokeRequired)
            {
                var loadingCallback = new SetLoadingCallback(SetIsLoadingPeople);
                Invoke(loadingCallback, new object[] { isLoading });
            }
            else
            {
                if (isLoading)
                {
                    listPeople.Columns[0].Text = "Person (loading...)";
                }
                else
                {
                    listPeople.Columns[0].Text = "Person";
                }
            }
        }

        private void UserManagementPersonLoadingComplete(object sender, EventArgs e)
        {
            SetIsLoadingPeople(false);
        }

        private void UserManagementPersonLoaded(object sender, PersonEventArgs e)
        {
            AddPerson(e.Person);
        }

        private void LogonTimesConfiguration_Load(object sender, EventArgs e)
        {
            resizePersonColumn();
            isLoading = false;
        }

        private void SetupLogonTimesGrid()
        {
            var permittedColour = Color.White;
            RectangleBorder permittedBorder = new RectangleBorder(new BorderLine(permittedColour), new BorderLine(permittedColour));
            DevAge.Drawing.VisualElements.ColumnHeader permittedView = new DevAge.Drawing.VisualElements.ColumnHeader();
            permittedView.Border = permittedBorder;
            permittedView.BackColor = permittedColour;
            permittedView.BackgroundColorStyle = BackgroundColorStyle.Solid;
            permittedCellView = new SourceGrid.Cells.Views.Cell();
            permittedCellView.Background = permittedView;

            var blockedColour = Color.Aqua;
            RectangleBorder blockedBorder = new RectangleBorder(new BorderLine(blockedColour), new BorderLine(blockedColour));
            DevAge.Drawing.VisualElements.ColumnHeader blockedView = new DevAge.Drawing.VisualElements.ColumnHeader();
            blockedView.Border = blockedBorder;
            blockedView.BackColor = blockedColour;
            blockedView.BackgroundColorStyle = BackgroundColorStyle.Solid;
            blockedCellView = new SourceGrid.Cells.Views.Cell();
            blockedCellView.Background = blockedView;

            var superFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            var standardFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
            const int dayColumnCount = 12;
            int timePeriodCount = dataAccess.TimePeriods.Count;
            int halfWay = timePeriodCount / 2;
            int timePeriodsPerHour = timePeriodCount / 24;
            int defaultWidth = gridWhen.Width / (timePeriodCount + dayColumnCount);
            gridWhen.DefaultWidth = defaultWidth;
            gridWhen.ColumnsCount = timePeriodCount + dayColumnCount;
            gridWhen.RowsCount = dataAccess.DaysOfWeek.Count + 2;    //no of days + 2 header lines

            RectangleBorder border = new RectangleBorder(new BorderLine(SystemColors.Control), new BorderLine(SystemColors.Control));

            DevAge.Drawing.VisualElements.ColumnHeader flatHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
            flatHeader.Border = border;
            flatHeader.BackColor = SystemColors.Control;
            flatHeader.BackgroundColorStyle = BackgroundColorStyle.Solid;
            SourceGrid.Cells.Views.ColumnHeader superHeaderView = new SourceGrid.Cells.Views.ColumnHeader();
            superHeaderView.Font = superFont;
            superHeaderView.Background = flatHeader;
            superHeaderView.ForeColor = Color.Black;
            SourceGrid.Cells.Views.ColumnHeader standardHeaderView = new SourceGrid.Cells.Views.ColumnHeader();
            standardHeaderView.Font = standardFont;
            standardHeaderView.Background = flatHeader;
            standardHeaderView.ForeColor = Color.Black;

            SourceGrid.Cells.Views.Cell cellView = new SourceGrid.Cells.Views.Cell();
            cellView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(SystemColors.Control, Color.White, 45);
            cellView.Border = border;

            int rowNumber = 2;
            foreach (var day in dataAccess.DaysOfWeek)
            {
                gridWhen[rowNumber, 0] = new SourceGrid.Cells.ColumnHeader(day.DayName);
                gridWhen[rowNumber, 0].ColumnSpan = dayColumnCount;
                gridWhen[rowNumber, 0].View = standardHeaderView;
                rowNumber++;
            }
            gridWhen[0, dayColumnCount] = new SourceGrid.Cells.ColumnHeader("Midnight");
            gridWhen[0, dayColumnCount].ColumnSpan = halfWay;
            gridWhen[0, dayColumnCount].View = superHeaderView;
            gridWhen[0, halfWay + dayColumnCount] = new SourceGrid.Cells.ColumnHeader("Noon");
            gridWhen[0, halfWay + dayColumnCount].ColumnSpan = halfWay;
            gridWhen[0, halfWay + dayColumnCount].View = superHeaderView;
            for (int i = 0; i < 24; i++)
            {
                if (i % 12 == 0)
                {
                    gridWhen[1, dayColumnCount + (i * timePeriodsPerHour)] = new SourceGrid.Cells.ColumnHeader("12");
                }
                else
                {
                    gridWhen[1, dayColumnCount + (i * timePeriodsPerHour)] = new SourceGrid.Cells.ColumnHeader(string.Format("{0}", i % 12));
                }
                gridWhen[1, dayColumnCount + (i * timePeriodsPerHour)].ColumnSpan = timePeriodsPerHour;
                gridWhen[1, dayColumnCount + (i * timePeriodsPerHour)].View = standardHeaderView;
            }
            int col = dayColumnCount;
            foreach (var timePeriod in dataAccess.TimePeriods)
            {
                int row = 2;
                foreach (var day in dataAccess.DaysOfWeek)
                {
                    LogonTimeAllowed loginTimeAllowed = new LogonTimeAllowed();
                    loginTimeAllowed.Permitted = true;
                    WhenAllowedValues cellContent = new WhenAllowedValues(loginTimeAllowed);
                    cellContent.GridColumn = col;
                    cellContent.GridRow = row;
                    cellContent.WhichDay = day;
                    cellContent.WhichTimePeriod = timePeriod;
                    whenAllowedValuesList.Add(cellContent);
                    gridWhen[row, col] = new SourceGrid.Cells.Cell(cellContent);
                    gridWhen[row, col].View = permittedCellView;
                    row++;
                }
                col++;
            }
        }
        #endregion

        #region people list drawing stuff
        private void resizePersonColumn()
        {
            listPeople.Columns[0].Width = listPeople.Width;
        }

        private void listPeople_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
            Font font = new Font(listPeople.Font.FontFamily, listPeople.Font.Size, FontStyle.Bold);
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                e.Graphics.DrawString(e.Header.Text, font, Brushes.Black, e.Bounds, format);
            }
            //e.DrawText();
        }

        private void splitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            resizePersonColumn();
        }

        private void listPeople_Leave(object sender, EventArgs e)
        {
            if (listPeople.FocusedItem != null)
            {
                currentFocusItem = listPeople.FocusedItem.Index;
            }
        }

        private void listPeople_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            //if (e.Item.Selected)
            //{
            //    if (currentFocusItem == e.Item.Index)
            //    {
            //        e.Item.ForeColor = Color.Black;
            //        e.Item.BackColor = Color.LightBlue;
            //        currentFocusItem = -1;
            //    }
            //    else if (listPeople.Focused)  // If the selected item has focus
            //    {
            //        e.Item.ForeColor = SystemColors.HighlightText;
            //        e.Item.BackColor = SystemColors.Highlight;
            //    }
            //}
            //else
            //{
            //    e.Item.ForeColor = listPeople.ForeColor;
            //    e.Item.BackColor = listPeople.BackColor;
            //}

            //e.DrawBackground();
            //e.DrawText();
            //Rectangle rect = e.Bounds;
            //rect.Width = 15;
            //rect.Height = 15;

            //ControlPaint.DrawCheckBox(e.Graphics, rect.X, rect.Y + 1, rect.Width, rect.Height, ButtonState.Flat);
        }

        private void listPeople_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            //if (e.Item.Selected)
            //{
            //    if (currentFocusItem == e.Item.Index)
            //    {
            //        e.Item.ForeColor = Color.Black;
            //        e.Item.BackColor = Color.LightBlue;
            //        currentFocusItem = -1;
            //    }
            //    else if (listPeople.Focused)  // If the selected item has focus
            //    {
            //        e.Item.ForeColor = SystemColors.HighlightText;
            //        e.Item.BackColor = SystemColors.Highlight;
            //    }
            //}
            //else
            //{
            //    e.Item.ForeColor = listPeople.ForeColor;
            //    e.Item.BackColor = listPeople.BackColor;
            //}

            //e.DrawBackground();
            //e.DrawText();
        }
        #endregion

        #region local methods
        private string CurrentPerson
        {
            get
            {
                if (listPeople.SelectedItems.Count == 0)
                {
                    return string.Empty;
                }
                return listPeople.SelectedItems[0].Text;
            }
        }

        private void SetItemAvailability()
        {
            bool isRestricted = false;
            if (!string.IsNullOrEmpty(CurrentPerson))
            {
                isRestricted = userManagement.PersonIsRestricted(CurrentPerson);
            }
            pnlDetail.Visible = isRestricted;
        }

        private void LoadPersonData()
        {
            lblDetails.Text = string.Format("Details for {0}", CurrentPerson);
            lblWhen.Text = string.Format("When can {0} use the computer", CurrentPerson);
            lblTotal.Text = string.Format("Total hours allowed each day for {0}", CurrentPerson);
            dgvHoursAllowed.Rows.Clear();
            dgvHoursAllowed.Refresh();
            var hoursPerDay = userManagement.HoursPerDayForPerson(CurrentPerson);
            if (hoursPerDay == null)
            {
                return;
            }
            foreach (var record in hoursPerDay)
            {
                dgvHoursAllowed.Rows.Add(new object[] { record.HoursPerDayId, record.DayOfWeek.DayName, record.HoursAllowed });
            }
            var logonTimesAllowed = userManagement.LogonTimesAllowed(CurrentPerson);
            foreach (var logonTimeAllowed in logonTimesAllowed)
            {
                SetBackColour(whenAllowedValuesList.SetLogonTimeAllowed(logonTimeAllowed));
            }
        }

        private void SetBackColour(WhenAllowedValues whenAllowedValues)
        {
            var cell = gridWhen[whenAllowedValues.GridRow, whenAllowedValues.GridColumn];
            if (whenAllowedValues.LogonTimeAllowed.Permitted)
            {
                cell.View = permittedCellView;
            }
            else
            {
                cell.View = blockedCellView;
            }

        }
        #endregion

        #region event handlers
        private void listPeople_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!isLoading)
            {
                ListViewItem item = (ListViewItem)e.Item;
                string personName = item.Text;
                if (item.Checked)
                {
                    userManagement.SetPersonToRestricted(personName);
                }
                else
                {
                    userManagement.SetPersonToUnrestricted(personName);
                }
                SetItemAvailability();
            }
        }

        private void SetWaiting(bool waiting)
        {
            if (waiting)
            {
                Cursor.Current = Cursors.WaitCursor;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void listPeople_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetWaiting(true);
            SetItemAvailability();
            LoadPersonData();
            SetWaiting(false);
        }

        private void btnCancelChanges_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void dgvHoursAllowed_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int rowNumber = e.RowIndex;
            int id = (int)dgvHoursAllowed.Rows[rowNumber].Cells[0].Value;
            var newValueText = dgvHoursAllowed.Rows[rowNumber].Cells[2].Value;
            if (newValueText == null || string.IsNullOrEmpty(newValueText.ToString()))
            {
                userManagement.UpdateHoursPerDay(id, null);
            }
            else
            {
                float newValue = 0;
                float.TryParse(newValueText.ToString(), out newValue);
                userManagement.UpdateHoursPerDay(id, newValue);
            }
        }

        private void gridWhen_Click(object sender, EventArgs e)
        {
            SetWaiting(true);
            var startCell = gridWhen[gridWhen.Selection.ActivePosition.Row, gridWhen.Selection.ActivePosition.Column];
            var startCellValue = (WhenAllowedValues)startCell.Value;
            bool startIsPermit = startCellValue.LogonTimeAllowed.Permitted;
            var selectionRegion = gridWhen.Selection.GetSelectionRegion();
            var start = selectionRegion[0].Start;
            var end = selectionRegion[0].End;
            for (int i = start.Row; i <= end.Row; i++)
            {
                for (int j = start.Column; j <= end.Column; j++)
                {
                    var cell = gridWhen[i, j];
                    var currentCellValue = (WhenAllowedValues)cell.Value;
                    currentCellValue.LogonTimeAllowed.Permitted = !startIsPermit;
                    userManagement.UpdateLogonTimeAllowed(currentCellValue.LogonTimeAllowed);
                    SetBackColour(currentCellValue);
                }
            }
            gridWhen.Refresh();
            gridWhen.Selection.ResetSelection(false);
            SetWaiting(false);
        }

        private void gridWhen_MouseMove(object sender, MouseEventArgs e)
        {
            var child = gridWhen.GetChildAtPoint(e.Location);
            var cellPosition = gridWhen.MouseCellPosition;
            var cell = gridWhen[cellPosition.Row, cellPosition.Column];
            if (cell != null)
            {
                if (cell.Value is WhenAllowedValues)
                {
                    var value = (WhenAllowedValues)cell.Value;
                    if (currentHoverTarget == null || currentHoverTarget != value)
                    {
                        currentHoverTarget = value;
                        DateTime dateForFormat = dates.Today;
                        dateForFormat = dateForFormat + value.WhichTimePeriod.PeriodStart.TimeOfDay;
                        StringBuilder toolTip = new StringBuilder();
                        toolTip.Append(dateForFormat.ToString("h:mm tt "));
                        toolTip.Append(currentHoverTarget.WhichDay.DayName);
                        if (currentHoverTarget.LogonTimeAllowed.Permitted)
                        {
                            toolTip.Append(" permitted");
                        }
                        else
                        {
                            toolTip.Append(" blocked");
                        }
                        toolTipPersonList.SetToolTip(gridWhen, toolTip.ToString());
                    }
                }
            }
        }

        private void LogonTimesConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            var configUpdated = SystemSettingTypesEnum.ConfigurationChanged.Detail();
            configUpdated.SystemSetting = true.ToString();
            dataAccess.UpdateSystemSettingDetail(configUpdated);
            if (dataAccess.StillWorking)
            {
                var workingItems = new WorkingItems();
                workingItems.ShowDialog(this);
            }
        }
        #endregion

        #region subclasses
        private class WhenAllowedValuesList
        {
            private List<WhenAllowedValues> whenAllowedValuesList = new List<WhenAllowedValues>();

            public void Add(WhenAllowedValues whenAllowedValues)
            {
                whenAllowedValuesList.Add(whenAllowedValues);
            }

            //public void SetLogonTimeAllowed(Day day, TimePeriod timePeriod, LogonTimeAllowed logonTimeAllowed)
            public WhenAllowedValues SetLogonTimeAllowed(LogonTimeAllowed logonTimeAllowed)
            {
                var result = whenAllowedValuesList.First(x => x.WhichDay.DayNumber == logonTimeAllowed.DayOfWeek.DayNumber && x.WhichTimePeriod.TimePeriodId == logonTimeAllowed.TimePeriod.TimePeriodId);
                result.LogonTimeAllowed = logonTimeAllowed;
                return result;
            }
        }

        private class WhenAllowedValues
        {
            public LogonTimeAllowed LogonTimeAllowed { get; set; }

            public int GridRow { get; set; }

            public int GridColumn { get; set; }

            public DataModel.DayOfWeek WhichDay { get; set; }

            public TimePeriod WhichTimePeriod { get; set; }

            public WhenAllowedValues(LogonTimeAllowed logonTimeAllowed)
            {
                LogonTimeAllowed = logonTimeAllowed;
            }

            public override string ToString()
            {
                return " ";
            }
        }
        #endregion
    }
}
