using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;


namespace StockFilter
{
	public class Data
	{
		private Data()
		{
			souce = "Data Source=stocks.db";
			CreateDB();
		}

		private static Data _this = new Data();

		static public Data share()
		{
			return _this;
		}

		static string souce;

		public void SaveQuote(Quote c)
		{
			//CreateDB();
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			string sql = "insert or replace into stock_info values("+c._info._market+", "+c._info._code+", \""+c._info._name+"\");";
			cmd.CommandText = sql;
			cmd.ExecuteNonQuery();
			conn.Close();
			Output.Log(sql + "\n");
		}

		/// <summary>
		/// Loads all empty quotes.
		/// </summary>
		/// <returns>
		/// The all quotes only have basic information like code.
		/// </returns>
		public List<Quote> LoadAllQuotesEmpty()
		{
			List<Quote> allQ = new List<Quote>();
			return allQ;
		}

		public date LastData(Quote q)
		{
			return new date();
		}

		public Quote LoadQuoteDetail(Quote q)
		{
			return new Quote();
		}

		public void CreateDB()
		{
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();

			//cmd.CommandText = "drop table stock_info";
			//cmd.ExecuteNonQuery();

			cmd.CommandText = "CREATE TABLE if not exists stock_info(si_market int not null, si_code int not null, si_name string not null, primary key(si_code, si_market));";
			cmd.ExecuteNonQuery();

			conn.Close();
		}
	}
}

