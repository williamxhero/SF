using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace StockFilter
{
	/// <summary>
	/// Data. Information part.
	/// </summary>
	public partial class Data
	{		
		private static string TableNameInfo = "stock_info";

		/// <summary>
		/// Will replace the old entry.
		/// </summary>
		/// <param name='c'>
		/// quote to save into DB.
		/// </param>
		public void SaveQuoteInformation(Quote c)
		{
			CreateDBInformation();
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			string sql = "insert or replace into " + TableNameInfo + " values("+c.market+", "+c.CodeInt+", \""+c.Name+"\");";
			cmd.CommandText = sql;
			cmd.ExecuteNonQuery();
			conn.Close();
			Output.Log(sql + "\n");
		}

		public void UpdateQuotes(WebStockList.OnQuoteUpdated oqu)
		{
			List<WebStockList> parsers = new List<WebStockList>();
			parsers.Add(new WebStockList_SH());
			parsers.Add(new WebStockList_SZ());

			foreach (var parser in parsers) {
				parser.GetAllQuotesInfomation(oqu);

			}
		}

		public static List<Quote> Test()
		{
			List<Quote> allQ = new List<Quote>();
			return new List<Quote>();
		}

		/// <summary>
		/// Loads all empty quotes.
		/// </summary>
		/// <returns>
		/// The all quotes only have basic information like code.
		/// </returns>
		public List<Quote> LoadAllQuotesInfomation()
		{
			List<Quote> allQ = new List<Quote>();

			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			string sql = "select si_market, si_code, si_name from " + TableNameInfo + ";";
			cmd.CommandText = sql;
			SqliteDataReader rdr = cmd.ExecuteReader();
			information info;
			while(rdr.Read()){
				info._market._value = (market.type) rdr.GetInt32(0);
				info._code = rdr.GetInt32(1);
				info._name = rdr.GetString(2);
				allQ.Add(new Quote(info));
			}
			conn.Close();
			return allQ;
		}

		/// <summary>
		/// Creates the information DB.
		/// </summary>
		private void CreateDBInformation()
		{
			if( !File.Exists(dbfile) ||  !TableExist(TableNameInfo)){
				DoCreateDBInformation();
			}
		}

		private void DoCreateDBInformation()
		{
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			//cmd.CommandText = "drop table stock_info";
			//cmd.ExecuteNonQuery();

			cmd.CommandText = "CREATE TABLE if not exists " + TableNameInfo + "(si_market INTEGER not null, si_code INTEGER not null, si_name TEXT not null, primary key(si_code, si_market));";
			cmd.ExecuteNonQuery();

			conn.Close();
		}
	}//class

}//namespace

