using System;
using System.Collections.Generic;

namespace StockFilter
{
	public struct market
	{
		///@note never change the enum value.
		public enum type
		{
			All = 0,
			ShenZhen = 1,
			ShangHai = 2,
		}

		public override string ToString()
		{
			return ((int)_value).ToString();
		}

		public market(type v)
		{
			_value = v;
		}

		public type _value;
	}//market

	public struct information
	{
		public int _code;
		public market _market;
		public string _name; ///< text code like MSFT 吴忠仪表
		public void copy(information other)
		{
			_code = other._code;
			_market = other._market;
			_name = other._name;
		}
		public information(int c, market m, string n)
		{
			_code = c;
			_market = m;
			_name = n;
		}
		public static information EMPTY;
	}//information

	public struct date
	{
		public int year;
		public int month;
		public int day;
	}

	public struct price
	{
		public float _open;
		public float _high;
		public float _low;
		public float _close;
	}

	public struct indicator
	{
		public float _volume;
	}

	public struct dateData
	{
		public date _date;
		public price _price;
		public indicator _indic;
	}

	/// <summary>
	/// Quote information
	/// </summary>
	public class Quote
	{
		public information _info;
		//List<dateData> _history = new List<dateData>();

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
		public void Load()
		{

		}

		/// <summary>
		/// too much data need to load/unload _history dynamiclly
		/// </summary>
		public void Unload()
		{

			Save();
		}

		public void Save()
		{
			Data.share().SaveQuote(this);
		}
		/// <summary>
		/// 
		/// </summary>
		public void Update()
		{
			// @"http://ichart.finance.yahoo.com/table.csv?s=300072.sz");
		}

		private void ProceRawString(string raw)
		{

		}
	}

	/// <summary>
	/// Quote manager.
	/// </summary>
	public class QuoteManager
	{	
		List<WebStockList> _parsers = new List<WebStockList>();

		private QuoteManager()
		{
			//_parsers.Add(new WebStockList_SH());
			_parsers.Add(new WebStockList_SZ());
		}

		static private QuoteManager _this = new QuoteManager();

		static public QuoteManager share()
		{
			return _this;
		}
		/// <summary>
		/// code to quota information
		/// </summary>
		private Dictionary<int, Quote> _allQuote = new Dictionary<int, Quote>();

		private void SaveInfomation(information info)
		{
			Quote q = new Quote(info);
			q.Save();
			if(_allQuote.ContainsKey(info._code)) return;
			_allQuote.Add(info._code, q);
		}

		public Quote this [int num_code] {
			get {
				if (_allQuote.ContainsKey(num_code)) {
					return _allQuote [num_code];
				}
				throw new ArgumentOutOfRangeException("quote", "no such code : " + num_code);
			}
		}


		/// <summary>
		/// fetch from net. add new quotes into database
		/// </summary>
		public void Update()
		{
			foreach (var parser in _parsers) {
				parser.GetAllQuotesInfomation(SaveInfomation);
			}
		}

		/// <summary>
		/// load from desk
		/// </summary>
		public void Load()
		{

		}

	}
}

