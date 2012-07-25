using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace StockFilter
{
	/// <summary>
	/// Data. History part.
	/// </summary>
	public partial class Data
	{
		//WebStockDetail pWSD = new WebStockDetail_GOOG();
		WebStockDetail pWSD = new WebStockDetail_YAHOO();

		private string GetHistoryTableName(Quote q)
		{
			return "history_" + q.CodeStr + "_" + q.market;
		}

		private void UpdatePeriodDetail(Quote q, ref date dfrm, ref date to)
		{
			pWSD.FetchData(q, dfrm, to);
		}

		private void UpdateAllDetail(Quote q)
		{
			pWSD.FetchDataAllDates(q);
		}

		public void UpdateDetail(Quote q)
		{
			long lastDate = LastData(q);
			if (lastDate == 0) {
				UpdateAllDetail(q);
				return;
			}
			DateTime now = DateTime.Now.ToLocalTime();
			long nowDayTimeStamp = Util.GetUnixTimeStamp(now.Year, now.Month, now.Day);
			if (nowDayTimeStamp > lastDate) {
				date dFrom = Util.GetDate(lastDate);
				date dTo = Util.GetDate(nowDayTimeStamp);
				UpdatePeriodDetail(q, ref dFrom, ref dTo);
			}
		}

		public void SaveQuoteDetail(Quote q)
		{
			CreateDBHistory(q);
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			foreach (var pair in q.History) {
				dateData dt = pair.Value;

				string sql = "insert or replace into " + GetHistoryTableName(q) + " values(" +
					", " + dt._date + 
					", " + dt._indic._volume +
					", " + dt._price._open +
					", " + dt._price._high +
					", " + dt._price._low +
					", " + dt._price._close +
					");";
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}
			conn.Close();
		}

		private long LastData(Quote q)
		{
			if (!TableExist(GetHistoryTableName(q)))
				return 0;

			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			string sql = "select max(date) from " + GetHistoryTableName(q) + ";";
			cmd.CommandText = sql;
			SqliteDataReader rdr = cmd.ExecuteReader();
			long date = 0;
			if (rdr.Read()) {
				date = rdr.GetInt64(0);
			}
			conn.Close();
			return date;
		}

		public void LoadQuoteDetail(Quote q)
		{
			if (!TableExist(GetHistoryTableName(q)))
				return;

			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			string sql = "select date, volume, open, high, low, close from " + GetHistoryTableName(q) + ";";
			cmd.CommandText = sql;
			SqliteDataReader rdr = cmd.ExecuteReader();
			information info;
		
			while (rdr.Read()) {
				dateData dd;
				dd._date = rdr.GetInt64(0);
				dd._indic._volume = rdr.GetInt64(1);
				dd._price._open = rdr.GetDouble(2);
				dd._price._high = rdr.GetDouble(3);
				dd._price._low = rdr.GetDouble(4);
				dd._price._close = rdr.GetDouble(5);
				q.OverrideDateData(dd);
			}
			conn.Close();
		}

		/// <summary>
		/// Creates the information DB.
		/// </summary>
		private void CreateDBHistory(Quote q)
		{
			if (!File.Exists(dbfile) || !TableExist(GetHistoryTableName(q))) {
				DoCreateDBHistory(q);
			}
		}

		private void DoCreateDBHistory(Quote q)
		{
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			//cmd.CommandText = "drop table stock_info";
			//cmd.ExecuteNonQuery();

			cmd.CommandText = "CREATE TABLE if not exists " + GetHistoryTableName(q) + "(date INTEGER not null primary key, open REAL, high REAL, low REAL, close REAL, volume INTEGER);";
			cmd.ExecuteNonQuery();

			conn.Close();
		}
	}//class
}

