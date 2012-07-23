using System;
using System.Collections.Generic;

namespace StockFilter
{
	/// <summary>
	/// Quote information
	/// </summary>
	public class Quote
	{
		private information _info;
		private Dictionary<long, dateData> _history = null;

		public void OverrideDateData(dateData dd)
		{
			//long timeStamp = Util.GetUnixTimeStamp(dd._date.year, dd._date.month, dd._date.day);
			_history[dd._date] = dd;
		}

		public string CodeStr{
			get{return _info._code.ToString("D6");}
		}

		public int CodeInt{
			get{return _info._code;}
		}

		public string Name{
			get{return _info._name;}
		}

		public market market{
			get{return _info._market;}
		}

		public Quote()
		{
		}

		public Quote(information info)
		{
			_info = info;
		}

		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void LoadHistory()
		{
			UpdateHistory();
			Data.share().LoadQuoteDetail(this);
		}

		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void UnloadHistory()
		{
			if(_history != null){
				_history.Clear();
				_history = null;
			}
		}

		private void UpdateHistory()
		{
			_history = new Dictionary<long, dateData>();
			Data.share().UpdateDetail(this);
		}

		public void SaveInformation()
		{
			Data.share().SaveQuoteInformation(this);
		}

	}//class 

}

