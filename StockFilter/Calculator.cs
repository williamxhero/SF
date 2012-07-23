using System;

namespace StockFilter
{
	public class Calculator
	{
		private static Calculator _this = new Calculator();

		private Calculator()
		{
		}

		public static Calculator share()
		{
			return _this;
		}

		private void CalcQuote_MA(Quote q)
		{

		}

		public void CalcMA()
		{
			QuoteManager.share().CalcAllQuotes(CalcQuote_MA);
		}
	}//clas
}

