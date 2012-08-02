using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace StockFilter
{
	/// <summary>
	/// DataSource. Information part.
	/// </summary>
	public partial class DataSource
	{		
		private static string TableNameInfo = "stock_info";

		/// <summary>
		/// Will replace the old entry.
		/// </summary>
		/// <param name='c'>
		/// quote to save into DB.
		/// </param>
		public void SaveQuoteInfo_DB(Quote c)
		{
			CreateDBInformation();
			SqliteConnection conn = null;
			string sql = "";

			try {
				conn = new SqliteConnection(souce);
				conn.Open();
				SqliteCommand cmd = conn.CreateCommand();
				sql = "insert or replace into " + TableNameInfo + " values(" + c.market + ", " + c.CodeInt + ", \"" + c.Name + "\");";
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			} catch (Exception e) {
				Output.LogException(" Save Quote Information error (Quest:" + c.Describe + "): " + e.Message);
			}
			if(conn != null ) conn.Close();
			Output.Log(sql + "\n");
		}

		public void GetQuoteInfo_Web(WebStockList.OnQuoteUpdated oqu)
		{
			List<WebStockList> parsers = new List<WebStockList>();
			parsers.Add(new WebStockList_SH());
			parsers.Add(new WebStockList_SZ());

			foreach (var parser in parsers) {
				parser.GetAllQuotesInfomation(oqu);

			}
		}

		/// <summary>
		/// Loads all empty quotes.
		/// </summary>
		/// <returns>
		/// The all quotes only have basic information like code.
		/// </returns>
		public List<Quote> LoadQuotesInfo_DB()
		{
			List<Quote> allQ = new List<Quote>();
			SqliteConnection conn = null;
			string sql = "";
			try {
				conn = new SqliteConnection(souce);
				conn.Open();
				SqliteCommand cmd = conn.CreateCommand();
				sql = "select si_market, si_code, si_name from " + TableNameInfo + ";";
				cmd.CommandText = sql;
				SqliteDataReader rdr = cmd.ExecuteReader();
				information info = information.EMPTY;
				int count = 0;
				while (rdr.Read()) {
					info._market._value = (market.type)rdr.GetInt32(0);
					info.CodeInt = rdr.GetInt32(1);
					info._name = rdr.GetString(2);
					Quote q = new Quote(info);
					allQ.Add(q);
					Output.Log("" + ++count + " quotes loaded - " + q.Describe);
				}
			} catch (Exception e) {
				Output.LogException("sql(" + sql + ") error : " + e.Message);
			}

			if (conn != null)
				conn.Close();
			return allQ;
		}

		/// <summary>
		/// Creates the information DB.
		/// </summary>
		private void CreateDBInformation()
		{
			if (!File.Exists(dbfile) || !TableExist(TableNameInfo)) {
				DoCreateDBInformation();
			}
		}

		private void DoCreateDBInformation()
		{
			SqliteConnection conn = null;
			try{
			conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			//cmd.CommandText = "drop table stock_info";
			//cmd.ExecuteNonQuery();

			cmd.CommandText = "CREATE TABLE if not exists " + TableNameInfo + "(si_market INTEGER not null, si_code INTEGER not null, si_name TEXT not null, primary key(si_code, si_market));";
			cmd.ExecuteNonQuery();
			}catch(Exception e){
				Output.LogException("Create table(" + TableNameInfo + ") error : " +  e.Message);
			}
			conn.Close();
		}
	}//class

}//namespace

