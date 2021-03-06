﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace LogonTimes.DataModel
{
	/// <summary>
	/// Database       : LogonTimes
	/// Data Source    : G:\Development\LogonTimes\LogonTimes\LogonTimes.mdb
	/// Server Version : 04.00.0000
	/// </summary>
	public partial class LogonTimesDB : LinqToDB.Data.DataConnection
	{
		public ITable<Application>         Applications         { get { return this.GetTable<Application>(); } }
		public ITable<DayOfWeek>           DayOfWeeks           { get { return this.GetTable<DayOfWeek>(); } }
		public ITable<EventType>           EventTypes           { get { return this.GetTable<EventType>(); } }
		public ITable<HoursPerDay>         HoursPerDays         { get { return this.GetTable<HoursPerDay>(); } }
		public ITable<LogonTime>           LogonTimes           { get { return this.GetTable<LogonTime>(); } }
		public ITable<LogonTimeAllowed>    LogonTimeAlloweds    { get { return this.GetTable<LogonTimeAllowed>(); } }
		public ITable<Person>              People               { get { return this.GetTable<Person>(); } }
		public ITable<PersonApplication>   PersonApplications   { get { return this.GetTable<PersonApplication>(); } }
		public ITable<SystemSettingDetail> SystemSettingDetails { get { return this.GetTable<SystemSettingDetail>(); } }
		public ITable<SystemSettingType>   SystemSettingTypes   { get { return this.GetTable<SystemSettingType>(); } }
		public ITable<TimePeriod>          TimePeriods          { get { return this.GetTable<TimePeriod>(); } }

		public LogonTimesDB()
		{
			InitDataContext();
		}

		public LogonTimesDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}


	[Table("Application")]
	public partial class Application
    {
		[PrimaryKey, Identity] public int    ApplicationId   { get; set; } // Long
		[Column,     NotNull ] public string ApplicationName { get; set; } // text(255)
		[Column,     NotNull ] public string ApplicationPath { get; set; } // text(1000)
	}

	[Table("DayOfWeek")]
	public partial class DayOfWeek
	{
		[PrimaryKey, Identity] public int    DayNumber { get; set; } // Long
		[Column,     NotNull ] public string DayName   { get; set; } // text(50)
		[Column,     NotNull ] public int    SortOrder { get; set; } // Long
	}

	[Table("EventType")]
	public partial class EventType
	{
		[PrimaryKey, Identity] public int    EventTypeId   { get; set; } // Long
		[Column,     NotNull ] public string EventTypeName { get; set; } // text(50)
		[Column,     NotNull ] public bool   IsLoggedOn    { get; set; } // Bit
		[Column,     NotNull ] public bool   TriggersEvent { get; set; } // Bit
	}

	[Table("HoursPerDay")]
	public partial class HoursPerDay
	{
		[PrimaryKey, Identity] public int     HoursPerDayId { get; set; } // Long
		[Column,     NotNull ] public int     PersonId      { get; set; } // Long
		[Column,     NotNull ] public int     DayNumber     { get; set; } // Long
		[Column,     Nullable] public double? HoursAllowed  { get; set; } // Double
	}

	[Table("LogonTime")]
	public partial class LogonTime
	{
		[PrimaryKey, Identity] public int      LogonTimeId          { get; set; } // Long
		[Column,     NotNull ] public int      PersonId             { get; set; } // Long
		[Column,     NotNull ] public int      EventTypeId          { get; set; } // Long
		[Column,     NotNull ] public DateTime EventTime            { get; set; } // DateTime
		[Column,     Nullable] public int?     CorrespondingEventId { get; set; } // Long
	}

	[Table("LogonTimeAllowed")]
	public partial class LogonTimeAllowed
	{
		[PrimaryKey, Identity] public int  LogonTimeAllowedId { get; set; } // Long
		[Column,     NotNull ] public int  PersonId           { get; set; } // Long
		[Column,     NotNull ] public int  DayNumber          { get; set; } // Long
		[Column,     NotNull ] public int  TimePeriodId       { get; set; } // Long
		[Column,     NotNull ] public bool Permitted          { get; set; } // Bit
	}

	[Table("Person")]
	public partial class Person
	{
		[PrimaryKey, Identity] public int    PersonId  { get; set; } // Long
		[Column,     NotNull ] public string LogonName { get; set; } // text(100)
		[Column,     NotNull ] public string SID       { get; set; } // text(255)
	}

	[Table("PersonApplication")]
	public partial class PersonApplication
	{
		[PrimaryKey, Identity] public int  PersonApplicationId  { get; set; } // Long
		[Column,     NotNull ] public int  PersonId             { get; set; } // Long
		[Column,     NotNull ] public int  ApplicationId        { get; set; } // Long
		[Column,     NotNull ] public bool Permitted            { get; set; } // Bit
	}

	[Table("SystemSettingDetails")]
	public partial class SystemSettingDetail
	{
		[PrimaryKey, Identity] public int    SystemSettingNameId { get; set; } // Long
		[Column,     Nullable] public string SystemSetting       { get; set; } // text(255)
	}

	[Table("SystemSettingTypes")]
	public partial class SystemSettingType
	{
		[PrimaryKey, Identity] public int    SystemSettingNameId { get; set; } // Long
		[Column,     Nullable] public string SystemSettingName   { get; set; } // text(255)
		[Column,     Nullable] public string DataType            { get; set; } // text(255)
	}

	[Table("TimePeriod")]
	public partial class TimePeriod
	{
		[PrimaryKey, Identity] public int      TimePeriodId { get; set; } // Long
		[Column,     NotNull ] public DateTime PeriodStart  { get; set; } // DateTime
		[Column,     NotNull ] public DateTime PeriodEnd    { get; set; } // DateTime
	}

	public static partial class TableExtensions
	{
		public static DayOfWeek Find(this ITable<DayOfWeek> table, int DayNumber)
		{
			return table.FirstOrDefault(t =>
				t.DayNumber == DayNumber);
		}

		public static EventType Find(this ITable<EventType> table, int EventTypeId)
		{
			return table.FirstOrDefault(t =>
				t.EventTypeId == EventTypeId);
		}

		public static HoursPerDay Find(this ITable<HoursPerDay> table, int HoursPerDayId)
		{
			return table.FirstOrDefault(t =>
				t.HoursPerDayId == HoursPerDayId);
		}

		public static LogonTime Find(this ITable<LogonTime> table, int LogonTimeId)
		{
			return table.FirstOrDefault(t =>
				t.LogonTimeId == LogonTimeId);
		}

		public static LogonTimeAllowed Find(this ITable<LogonTimeAllowed> table, int LogonTimeAllowedId)
		{
			return table.FirstOrDefault(t =>
				t.LogonTimeAllowedId == LogonTimeAllowedId);
		}

		public static Person Find(this ITable<Person> table, int PersonId)
		{
			return table.FirstOrDefault(t =>
				t.PersonId == PersonId);
		}

		public static SystemSettingDetail Find(this ITable<SystemSettingDetail> table, int SystemSettingNameId)
		{
			return table.FirstOrDefault(t =>
				t.SystemSettingNameId == SystemSettingNameId);
		}

		public static SystemSettingType Find(this ITable<SystemSettingType> table, int SystemSettingNameId)
		{
			return table.FirstOrDefault(t =>
				t.SystemSettingNameId == SystemSettingNameId);
		}

		public static TimePeriod Find(this ITable<TimePeriod> table, int TimePeriodId)
		{
			return table.FirstOrDefault(t =>
				t.TimePeriodId == TimePeriodId);
		}
	}
}
