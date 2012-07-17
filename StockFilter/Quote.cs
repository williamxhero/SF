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

	public class price
	{
		float _open;
		float _high;
		float _low;
		float _close;
	}

	public class indicator
	{
		float _volume;
	}

	public class DateData
	{
		date _date;
		price _price;
		indicator _indic;
	}

	/// <summary>
	/// Quote infomation
	/// </summary>
	public class Quote
	{
		private string _code;
		private string _name;
		List<DateData> _history;

		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void Load ()
		{

		}

		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void Unload ()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		public void Update ()
		{
			// @"http://ichart.finance.yahoo.com/table.csv?s=300072.sz");
		}
	}

	/// <summary>
	/// Quote manager.
	/// </summary>
	public class QuoteManager
	{
		/// <summary>
		/// code to quota infomation
		/// </summary>
		Dictionary<string, Quote> dictionary = new Dictionary<string, Quote> ();

		/// <summary>
		/// fetch from net.
		/// </summary>
		public void Update ()
		{
			//50 each page. so the cursor should be 1, 51, 101, 151,...
			//http://www.sse.com.cn/sseportal/webapp/datapresent/SSEQueryStockInfoAct?reportName=BizCompStockInfoRpt&CURSOR=101
			Save ();
		}

		/// <summary>
		/// load from desk
		/// </summary>
		public void Load ()
		{
		}

		public void Save ()
		{

		}
	}
}

