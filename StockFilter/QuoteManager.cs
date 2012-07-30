using System;
using System.Collections.Generic;

namespace StockFilter
{
	public delegate void CalcQuote(Quote q);

	/// <summary>
	/// Quote manager.
	/// </summary>
	public partial class QuoteManager
	{
		private QuoteManager()	{	}
		private static QuoteManager _this = new QuoteManager();
		public static QuoteManager Static {
			get {
				return _this;
			}
		}
	}//class

	/// <summary>
	/// Quote manager. Quotes information part.
	/// </summary>
	public partial class QuoteManager
	{
		/// <summary>
		/// code to quota information
		/// </summary>
		private Dictionary<int, Quote> _allQuote = new Dictionary<int, Quote>();

		/// <summary>
		/// load from desk
		/// </summary>
		public void LoadInformation()
		{
			_allQuote.Clear();
			List<Quote> Qlist = DataSource.Static.LoadQuotesInfo_DB();
			int i  = 0;
			foreach (Quote q in Qlist) {
				//if(i > 100) break;
				_allQuote.Add(q.CodeInt, q);
				i++;
			}
		}

		/// <summary>
		/// fetch from net. add new quotes into database
		/// </summary>
		public void UpdateQuotesInformation()
		{
			DataSource.Static.GetQuoteInfo_Web(KeepQuote);
			SaveAllQuotesInfo();
		}

		public void CalcAllQuotes(CalcQuote cq)
		{
			LoadInformation();
			foreach (var q in _allQuote) {
				q.Value.LoadHistory();
				if(cq != null) cq(q.Value);
				q.Value.UnloadHistory();
			}
		}

		private void SaveAllQuotesInfo()
		{
			foreach (var q in _allQuote) {
				DataSource.Static.SaveQuoteInfo_DB(q.Value);
			}
		}

		private void KeepQuote(Quote q)
		{
			if (_allQuote.ContainsKey(q.CodeInt))
				return;
			_allQuote.Add(q.CodeInt, q);
		}

	}//class

}//namespace

