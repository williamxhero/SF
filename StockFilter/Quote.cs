using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StockFilter
{
	/// <summary>
	/// Quote information
	/// </summary>
	public partial class Quote
	{
		private information _info;
		private List<dateData> _history = null;

		public List<dateData> History {
			get{ return _history;}
		}

		public string CodeStr {
			get{ return _info._code.ToString("D6");}
		}

		public int CodeInt {
			get{ return _info._code;}
		}

		public string Name {
			get{ return _info._name;}
		}

		public market market {
			get{ return _info._market;}
		}

		public string Describe {
			get{ return Name + "(" + CodeStr + ")";}
		}

		public Quote()
		{
		}

		public Quote(information info)
		{
			_info = info;
		}

		public long LastDate {
			get {
				if (_history == null || _history.Count == 0) {
					return 0;
				}
				return _history [_history.Count - 1]._date;
			}
		}

		private void SortHistory()
		{
			if (_history != null) {
				_history.Sort();
			}
		}
	}//class

	public partial class Quote
	{
		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void LoadHistory()
		{
			_history = DataSource.Static.LoadHistory_DB(this);
			Output.Log("History loaded. " + _history.Count +" entries ");
			if(_history.Count == 0) {
				Output.Log( Describe + " history is empty");
				return;
			}
			SortHistory();
		}
		
		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void UnloadHistory()
		{
			if (_history != null) {
				_history.Clear();
				_history = null;
			}
			Output.Log("History Data Unloaded");
		}

		public void UpdateHistory()
		{
			Output.Log("\nUpdate history for" + Describe);

			LoadHistory();
			List<dateData> newData = DataSource.Static.GetHistory_Web(this);
			if (newData.Count > 0) {
				newData.Sort();
				//WriteListToFile("list_from_web.txt", newData);
				_history.AddRange(newData);
				DataSource.Static.SaveHistory_DB(this);
			}else{
				Output.Log("didn't get new data of (" + Describe + ") from web");
			}
			//load after save to check to correctness of DB.
			//newData = DataSource.Static.LoadHistory_DB(this);
			//WriteListToFile("list_from_DB.txt", newData);

			UnloadHistory();
		}
	}//class

	public partial class Quote
	{
		public void SaveInformation()
		{
			DataSource.Static.SaveQuoteInfo_DB(this);
		}
	}//class


	//TEST
	public partial class Quote
	{
		//save data to file in the format just as web-source
		//and use merge tool to compare.
		private void WriteListToFile(string file, List<dateData> list)
		{
			StringBuilder sb = new StringBuilder();
			foreach (dateData dd in list) {

				sb.AppendLine(
				                 Util.GetDate(dd._date) + "," +
					dd._price._open.ToString() + "," +
					dd._price._high.ToString() + "," +
					dd._price._low + "," +
					dd._price._close + "," +
					dd._indic._volume
				);
			}

			string content = sb.ToString();
			File.WriteAllText(file, content);
		}
	}//class 

}

