using System;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

namespace StockFilter
{
	/// <summary>
	/// DataSource communiating with different sources,
	/// like DB, web, files...
	/// </summary>
	public partial class DataSource
	{
		private static DataSource _this = new DataSource();
		public static DataSource Static {
			get {
				return _this;
			}
		}
	};

	/// <summary>
	/// DataSource. DB basic part.
	/// </summary>
	public partial class DataSource
	{
		private static string dbfile;
		private static string souce;

		private DataSource()
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
			bool hasrow = rdr.HasRows;
			conn.Close();
			return hasrow;
		}

		private long RowNum(String tableName)
		{
			SqliteConnection conn = new SqliteConnection(souce);
			conn.Open();
			SqliteCommand cmd = conn.CreateCommand();
			cmd.CommandText = "select count(0) from " + tableName;
			SqliteDataReader rdr = cmd.ExecuteReader();
			long cnt = 0;
			if (rdr.Read()) {
				cnt = rdr.GetInt64(0);
			}
			conn.Close();
			return cnt;
		}
	}//class
}//namespace

