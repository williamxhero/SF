using System;
using System.Collections.Generic;


namespace StockFilter
{
	public class date
	{
		int year;
		int month;
		int day;
	}

	/// <summary>
	/// Quote infomation
	/// </summary>
	public class Quote
	{
		private string _code;
		private string _name;
		public class Data
		{
			date _dat;
			float _open;
			float _high;
			float _low;
			float _close;
			float _volume;
		}

		List<Data> _history;
	}


	/// <summary>
	/// Quote manager.
	/// </summary>
	public class QuoteManager
	{
		/// <summary>
		/// code to quota infomation
		/// </summary>
		Dictionary<string, Quote> dictionary = new Dictionary<string, Quote>();

		/// <summary>
		/// fetch from net.
		/// </summary>
		public void Update()
		{
			Save();
		}

		/// <summary>
		/// load from desk
		/// </summary>
		public void Load()
		{
			//50 each page. so the cursor should be 1, 51, 101, 151,...
			//http://www.sse.com.cn/sseportal/webapp/datapresent/SSEQueryStockInfoAct?reportName=BizCompStockInfoRpt&CURSOR=101
		}

		public void Save()
		{
		}
	}
}

