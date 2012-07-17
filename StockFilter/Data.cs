using System;

namespace StockFilter
{
	public class Data
	{
		private static Data _this = new Data();

		static public Data share ()
		{
			return _this;
		}

		public void SaveQuote(Quote c)
		{

		}

		public date LastData(code c)
		{
			return new date();
		}

		/// <summary>
		/// Loads the quote from database
		/// </summary>
		/// <returns>
		/// The quote.
		/// </returns>
		/// <param name='code'>
		/// for example : "600111"
		/// </param>
		public Quote LoadQuote(code c)
		{
			return new Quote();
		}
	}
}

