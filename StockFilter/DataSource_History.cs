using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace StockFilter
{
	/// <summary>
	/// DataSource. History part.
	/// </summary>
	public partial class DataSource
	{
		public List<dateData> GetHistory_Web(Quote q)
		{
			if (q.LastDate == 0) {
				return pWSD.GetHistory(q);
				//return pWSD.GetHistory(q);
			}
			DateTime now = DateTime.Now.ToLocalTime();
			long nowDayTimeStamp = Util.GetUnixTimeStamp(now.Year, now.Month, now.Day);
			if (nowDayTimeStamp > q.LastDate) {
				date dFrom = Util.GetDate(q.LastDate);
				date dTo = Util.GetDate(nowDayTimeStamp);
				//return pWSD.GetHistory(q, dFrom, dTo);
				return pWSD_G.GetHistory(q, dFrom, dTo);
			} else {
				Output.Log(q.Describe + " already updated to latest.");
			}
			return new List<dateData>();
		}

		public void SaveHistory_DB(Quote q)
		{
			CreateDBHistory(q);
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			foreach (dateData dt in q.History) {
				string sql = "insert or replace into " + GetTableNameHistory_DB(q) + 
					" values(" + dt._date + 
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

		public List<dateData> LoadHistory_DB(Quote q)
		{
			List<dateData> dataList = new List<dateData>();
			string tblNm = GetTableNameHistory_DB(q);
			if (!TableExist(tblNm))
				return dataList;

			SqliteConnection conn = null;
			string sql = "";

			try {
				conn = new SqliteConnection(souce);
				conn.Open();
				SqliteCommand cmd = conn.CreateCommand();
				sql = "select date, volume, open, high, low, close from " + tblNm + " order by date desc;";
				cmd.CommandText = sql;
				SqliteDataReader rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					dateData dd = new dateData();
					dd._date = rdr.GetInt64(0);
					dd._indic._volume = rdr.GetInt64(1);
					dd._price._open = rdr.GetDouble(2);
					dd._price._high = rdr.GetDouble(3);
					dd._price._low = rdr.GetDouble(4);
					dd._price._close = rdr.GetDouble(5);
					dataList.Add(dd);
				}
			} catch (Exception e) {
				Output.LogException(" sql (" + sql + ") error : " + e.Message); 
			}
			if (conn != null)
				conn.Close();
			return dataList;
		}

		//WebStockDetail pWSD = new WebStockDetail_GOOG();
		private WebStockDetail pWSD = new WebStockDetail_YAHOO();
		private WebStockDetail pWSD_G = new WebStockDetail_GOOG();

		private string GetTableNameHistory_DB(Quote q)
		{
			return "history_" + q.CodeStr + "_" + q.market;
		}

		/// <summary>
		/// Creates the information DB.
		/// </summary>
		private void CreateDBHistory(Quote q)
		{
			if (!File.Exists(dbfile) || !TableExist(GetTableNameHistory_DB(q))) {
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

			cmd.CommandText = "CREATE TABLE if not exists " + GetTableNameHistory_DB(q) + "(date INTEGER not null UNIQUE primary key, volume INTEGER, open REAL, high REAL, low REAL, close REAL);";
			cmd.ExecuteNonQuery();

			conn.Close();
		}
	}//class
}

