using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace StockFilter
{
	/// <summary>
	/// Data. Instance part.
	/// </summary>
	public partial class Data
	{
		private static Data _this = new Data();
		public static Data Static {
			get {
				return _this;
			}
		}
	};

	/// <summary>
	/// Data. DB basic part.
	/// </summary>
	public partial class Data
	{
		private static string dbfile;
		private static string souce;

		private Data()
		{
			dbfile = "stocks.db";
			souce = "Data Source=" + dbfile;
		}

		public void Vaccum()
		{
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			cmd.CommandText = "vacuum;";
			cmd.ExecuteNonQuery();
		}

		private bool TableExist(String tableName)
		{
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
			SqliteDataReader rdr = cmd.ExecuteReader();
			if (rdr.HasRows)
				return true;
			else
				return false;
		}
	}//class
}//namespace

