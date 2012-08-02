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
			long lastDate = q.LastDate;
			if (lastDate == 0) {
				return pWSD.GetHistory(q);
				//return pWSD.GetHistory(q);
			}

			DateTime DTTo = DateTime.Now.ToLocalTime();
			DTTo = DTTo.AddDays(-1);//as today will not be the histroy
			long stampTo = Util.GetUnixTimeStamp(DTTo);

			date dtFrom = Util.GetDate(lastDate);
			DateTime DTFrom = new DateTime(dtFrom.year, dtFrom.month, dtFrom.day);
			DTFrom = DTFrom.AddDays(1); //the bland data only begins from the next day (of the latest data we have)
			long stampFrom = Util.GetUnixTimeStamp(DTFrom);

			if (stampTo > stampFrom) {
				date dFrom = Util.GetDate(stampFrom);
				date dTo = Util.GetDate(stampTo);
				return pWSD.GetHistory(q, dFrom, dTo);
				//return pWSD_G.GetHistory(q, dFrom, dTo);
			} else {
				Output.Log(q.Describe + " already updated to latest : " + dtFrom);
			}
			return new List<dateData>();
		}

		public void SaveHistory_DB(Quote q)
		{
			CreateDBHistory(q);
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			int entries = 0;
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
				entries++;
			}
			conn.Close();
			Output.Log(q.Describe + " history save -> DB " + entries + " entries");
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
		//private WebStockDetail pWSD_G = new WebStockDetail_GOOG();

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

		private void _DoCreateDBHistroy(SqliteConnection conn, string tblName)
		{
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			//cmd.CommandText = "drop table stock_info";
			//cmd.ExecuteNonQuery();

			cmd.CommandText = "CREATE TABLE if not exists " + tblName + "(date INTEGER not null UNIQUE primary key, volume INTEGER, open REAL, high REAL, low REAL, close REAL);";
			cmd.ExecuteNonQuery();
		}

		private void DoCreateDBHistory(Quote q)
		{
			string tblName = GetTableNameHistory_DB(q);
			SqliteConnection conn = null;
			const int tryTimes = 1;
			for (int i = 0; i <= tryTimes; i ++) {
				try {
					if(conn != null){
						conn.Close();
					}
					conn = new SqliteConnection(souce);
					_DoCreateDBHistroy(conn, tblName);
					break;
				} catch (Exception e) {
					string msg = "create table[" + tblName + "] failed.";
					if(i < tryTimes){
						msg += "Will try again " + ( i + 1) + "/" + tryTimes + ".";
					}
					msg += "msg : " + e.Message;
					Output.Log( msg );
				}
			}

			conn.Close();
		}
	}//class
}

