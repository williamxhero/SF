using System;
using System.Collections.Generic;

namespace StockFilter
{
	public delegate void CalcQuote(Quote q);

	/// <summary>
	/// Quote manager. Quotes Detail part.
	/// </summary>
	public partial class QuoteManager
	{
		public void CalcAllQuotes(CalcQuote cq)
		{
			LoadInformation();
			foreach (var q in _allQuote) {
				q.Value.LoadHistory();
				cq(q.Value);
				q.Value.UnloadHistory();
			}
		}
	}//class

	/// <summary>
	/// Quote manager. Quotes information part.
	/// </summary>
	public partial class QuoteManager
	{
		private QuoteManager()
		{

		}

		private static QuoteManager _this = new QuoteManager();

		public static QuoteManager Static {
			get {
				return _this;
			}
		}

		/// <summary>
		/// load from desk
		/// </summary>
		public void LoadInformation()
		{
			List<Quote> Qlist = Data.Static.LoadAllQuotesInfomation();
			int i  =0;
			foreach (Quote q in Qlist) {
				if(i > 0) break;
				_allQuote.Add(q.CodeInt, q);
				i++;
			}
		}

		/// <summary>
		/// fetch from net. add new quotes into database
		/// </summary>
		public void UpdateInformation()
		{
			Data.Static.UpdateQuotes(SaveQuote);
		}

		/// <summary>
		/// code to quota information
		/// </summary>
		private Dictionary<int, Quote> _allQuote = new Dictionary<int, Quote>();

		private void SaveQuote(Quote q)
		{
			if (_allQuote.ContainsKey(q.CodeInt))
				return;
			_allQuote.Add(q.CodeInt, q);
		}

	}//class

}//namespace

