﻿namespace LogonTimes.UI
{
    partial class LogonTimesConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogonTimesConfiguration));
            this.splitter = new System.Windows.Forms.SplitContainer();
            this.listPeople = new System.Windows.Forms.ListView();
            this.colPerson = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabLogonTimes = new System.Windows.Forms.TabPage();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.gridWhen = new SourceGrid.Grid();
            this.lblWhen = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.dgvHoursAllowed = new System.Windows.Forms.DataGridView();
            this.HoursPerDayId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnHoursAllowed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblDetails = new System.Windows.Forms.Label();
            this.tabApplicationAccess = new System.Windows.Forms.TabPage();
            this.pnlApplications = new System.Windows.Forms.Panel();
            this.btnRestrictAccess = new System.Windows.Forms.Button();
            this.lvApplications = new System.Windows.Forms.ListView();
            this.AppName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AppPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolTipPersonList = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabLogonTimes.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoursAllowed)).BeginInit();
            this.tabApplicationAccess.SuspendLayout();
            this.pnlApplications.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter
            // 
            this.splitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitter.Location = new System.Drawing.Point(6, 12);
            this.splitter.Name = "splitter";
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.listPeople);
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.tabMain);
            this.splitter.Size = new System.Drawing.Size(1206, 554);
            this.splitter.SplitterDistance = 263;
            this.splitter.TabIndex = 2;
            this.splitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter_SplitterMoved);
            // 
            // listPeople
            // 
            this.listPeople.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPeople.AutoArrange = false;
            this.listPeople.BackColor = System.Drawing.SystemColors.Control;
            this.listPeople.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listPeople.CheckBoxes = true;
            this.listPeople.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPerson});
            this.listPeople.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listPeople.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listPeople.HideSelection = false;
            this.listPeople.Location = new System.Drawing.Point(3, 30);
            this.listPeople.MultiSelect = false;
            this.listPeople.Name = "listPeople";
            this.listPeople.OwnerDraw = true;
            this.listPeople.Size = new System.Drawing.Size(257, 521);
            this.listPeople.TabIndex = 2;
            this.toolTipPersonList.SetToolTip(this.listPeople, "Select the checkbox next to people that have restricted logon hours");
            this.listPeople.UseCompatibleStateImageBehavior = false;
            this.listPeople.View = System.Windows.Forms.View.Details;
            this.listPeople.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listPeople_DrawColumnHeader);
            this.listPeople.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listPeople_DrawItem);
            this.listPeople.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listPeople_DrawSubItem);
            this.listPeople.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listPeople_ItemChecked);
            this.listPeople.SelectedIndexChanged += new System.EventHandler(this.listPeople_SelectedIndexChanged);
            this.listPeople.Leave += new System.EventHandler(this.listPeople_Leave);
            // 
            // colPerson
            // 
            this.colPerson.Text = "Person";
            this.colPerson.Width = 115;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabLogonTimes);
            this.tabMain.Controls.Add(this.tabApplicationAccess);
            this.tabMain.Location = new System.Drawing.Point(3, 3);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(936, 548);
            this.tabMain.TabIndex = 6;
            // 
            // tabLogonTimes
            // 
            this.tabLogonTimes.BackColor = System.Drawing.SystemColors.Control;
            this.tabLogonTimes.Controls.Add(this.pnlDetail);
            this.tabLogonTimes.Location = new System.Drawing.Point(4, 22);
            this.tabLogonTimes.Name = "tabLogonTimes";
            this.tabLogonTimes.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogonTimes.Size = new System.Drawing.Size(928, 522);
            this.tabLogonTimes.TabIndex = 0;
            this.tabLogonTimes.Text = "Logon Times";
            // 
            // pnlDetail
            // 
            this.pnlDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetail.Controls.Add(this.label2);
            this.pnlDetail.Controls.Add(this.textBox2);
            this.pnlDetail.Controls.Add(this.label1);
            this.pnlDetail.Controls.Add(this.textBox1);
            this.pnlDetail.Controls.Add(this.gridWhen);
            this.pnlDetail.Controls.Add(this.lblWhen);
            this.pnlDetail.Controls.Add(this.lblTotal);
            this.pnlDetail.Controls.Add(this.dgvHoursAllowed);
            this.pnlDetail.Controls.Add(this.lblDetails);
            this.pnlDetail.Location = new System.Drawing.Point(6, 6);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(916, 510);
            this.pnlDetail.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 488);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Enabled";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(7, 485);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(25, 20);
            this.textBox2.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(169, 488);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Blocked";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.BackColor = System.Drawing.Color.Aqua;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(138, 485);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(25, 20);
            this.textBox1.TabIndex = 9;
            // 
            // gridWhen
            // 
            this.gridWhen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridWhen.DefaultWidth = 8;
            this.gridWhen.EnableSort = true;
            this.gridWhen.Location = new System.Drawing.Point(7, 291);
            this.gridWhen.Name = "gridWhen";
            this.gridWhen.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridWhen.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridWhen.Size = new System.Drawing.Size(906, 176);
            this.gridWhen.TabIndex = 8;
            this.gridWhen.TabStop = true;
            this.toolTipPersonList.SetToolTip(this.gridWhen, "Hover over time period for details");
            this.gridWhen.ToolTipText = "";
            this.gridWhen.Click += new System.EventHandler(this.gridWhen_Click);
            this.gridWhen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridWhen_MouseMove);
            // 
            // lblWhen
            // 
            this.lblWhen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblWhen.AutoSize = true;
            this.lblWhen.Location = new System.Drawing.Point(4, 275);
            this.lblWhen.Name = "lblWhen";
            this.lblWhen.Size = new System.Drawing.Size(177, 13);
            this.lblWhen.TabIndex = 7;
            this.lblWhen.Text = "When can person use the computer";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(4, 30);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(146, 13);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "Total hours allowed each day";
            // 
            // dgvHoursAllowed
            // 
            this.dgvHoursAllowed.AllowUserToAddRows = false;
            this.dgvHoursAllowed.AllowUserToDeleteRows = false;
            this.dgvHoursAllowed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvHoursAllowed.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvHoursAllowed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHoursAllowed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHoursAllowed.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.HoursPerDayId,
            this.columnDay,
            this.columnHoursAllowed});
            this.dgvHoursAllowed.GridColor = System.Drawing.SystemColors.Control;
            this.dgvHoursAllowed.Location = new System.Drawing.Point(7, 46);
            this.dgvHoursAllowed.Name = "dgvHoursAllowed";
            this.dgvHoursAllowed.RowHeadersWidth = 4;
            this.dgvHoursAllowed.Size = new System.Drawing.Size(363, 175);
            this.dgvHoursAllowed.TabIndex = 4;
            this.dgvHoursAllowed.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHoursAllowed_CellEndEdit);
            // 
            // HoursPerDayId
            // 
            this.HoursPerDayId.FillWeight = 10F;
            this.HoursPerDayId.HeaderText = "Id";
            this.HoursPerDayId.MinimumWidth = 25;
            this.HoursPerDayId.Name = "HoursPerDayId";
            this.HoursPerDayId.ReadOnly = true;
            this.HoursPerDayId.Visible = false;
            this.HoursPerDayId.Width = 25;
            // 
            // columnDay
            // 
            this.columnDay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnDay.FillWeight = 55F;
            this.columnDay.HeaderText = "Day";
            this.columnDay.MinimumWidth = 70;
            this.columnDay.Name = "columnDay";
            this.columnDay.ReadOnly = true;
            this.columnDay.Width = 70;
            // 
            // columnHoursAllowed
            // 
            this.columnHoursAllowed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Format = "N1";
            dataGridViewCellStyle1.NullValue = "(unlimited)";
            this.columnHoursAllowed.DefaultCellStyle = dataGridViewCellStyle1;
            this.columnHoursAllowed.FillWeight = 45F;
            this.columnHoursAllowed.HeaderText = "Hours Allowed";
            this.columnHoursAllowed.MinimumWidth = 100;
            this.columnHoursAllowed.Name = "columnHoursAllowed";
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.lblDetails.Location = new System.Drawing.Point(3, 0);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(93, 23);
            this.lblDetails.TabIndex = 2;
            this.lblDetails.Text = "Details for ";
            // 
            // tabApplicationAccess
            // 
            this.tabApplicationAccess.BackColor = System.Drawing.SystemColors.Control;
            this.tabApplicationAccess.Controls.Add(this.pnlApplications);
            this.tabApplicationAccess.Location = new System.Drawing.Point(4, 22);
            this.tabApplicationAccess.Name = "tabApplicationAccess";
            this.tabApplicationAccess.Padding = new System.Windows.Forms.Padding(3);
            this.tabApplicationAccess.Size = new System.Drawing.Size(928, 522);
            this.tabApplicationAccess.TabIndex = 1;
            this.tabApplicationAccess.Text = "Application Access";
            // 
            // pnlApplications
            // 
            this.pnlApplications.Controls.Add(this.label3);
            this.pnlApplications.Controls.Add(this.btnRestrictAccess);
            this.pnlApplications.Controls.Add(this.lvApplications);
            this.pnlApplications.Location = new System.Drawing.Point(6, 6);
            this.pnlApplications.Name = "pnlApplications";
            this.pnlApplications.Size = new System.Drawing.Size(916, 510);
            this.pnlApplications.TabIndex = 1;
            // 
            // btnRestrictAccess
            // 
            this.btnRestrictAccess.Location = new System.Drawing.Point(25, 461);
            this.btnRestrictAccess.Name = "btnRestrictAccess";
            this.btnRestrictAccess.Size = new System.Drawing.Size(294, 23);
            this.btnRestrictAccess.TabIndex = 1;
            this.btnRestrictAccess.Text = "Add another directory that you wish to restrict access";
            this.btnRestrictAccess.UseVisualStyleBackColor = true;
            this.btnRestrictAccess.Click += new System.EventHandler(this.btnRestrictAccess_Click);
            // 
            // lvApplications
            // 
            this.lvApplications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvApplications.BackColor = System.Drawing.SystemColors.Control;
            this.lvApplications.CheckBoxes = true;
            this.lvApplications.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.AppName,
            this.AppPath});
            this.lvApplications.Location = new System.Drawing.Point(3, 36);
            this.lvApplications.MultiSelect = false;
            this.lvApplications.Name = "lvApplications";
            this.lvApplications.Size = new System.Drawing.Size(910, 405);
            this.lvApplications.TabIndex = 0;
            this.lvApplications.UseCompatibleStateImageBehavior = false;
            this.lvApplications.View = System.Windows.Forms.View.Details;
            this.lvApplications.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvApplications_ItemChecked);
            // 
            // AppName
            // 
            this.AppName.Text = "Application Name";
            this.AppName.Width = 150;
            // 
            // AppPath
            // 
            this.AppPath.Text = "Application Path";
            this.AppPath.Width = 400;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select folder that you wish to restrict access";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Check the items you wish to restrict";
            // 
            // LogonTimesConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 568);
            this.Controls.Add(this.splitter);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogonTimesConfiguration";
            this.Text = "Logon Times Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogonTimesConfiguration_FormClosing);
            this.Load += new System.EventHandler(this.LogonTimesConfiguration_Load);
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
            this.splitter.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabLogonTimes.ResumeLayout(false);
            this.pnlDetail.ResumeLayout(false);
            this.pnlDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoursAllowed)).EndInit();
            this.tabApplicationAccess.ResumeLayout(false);
            this.pnlApplications.ResumeLayout(false);
            this.pnlApplications.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitter;
        private System.Windows.Forms.ListView listPeople;
        private System.Windows.Forms.ColumnHeader colPerson;
        private System.Windows.Forms.ToolTip toolTipPersonList;
        private System.Windows.Forms.Panel pnlDetail;
        private System.Windows.Forms.Label lblWhen;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.DataGridView dgvHoursAllowed;
        private System.Windows.Forms.DataGridViewTextBoxColumn HoursPerDayId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnHoursAllowed;
        private System.Windows.Forms.Label lblDetails;
        private SourceGrid.Grid gridWhen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabLogonTimes;
        private System.Windows.Forms.TabPage tabApplicationAccess;
        private System.Windows.Forms.ListView lvApplications;
        private System.Windows.Forms.ColumnHeader AppName;
        private System.Windows.Forms.ColumnHeader AppPath;
        private System.Windows.Forms.Panel pnlApplications;
        private System.Windows.Forms.Button btnRestrictAccess;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label3;
    }
}