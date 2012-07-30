using System;

namespace StockFilter
{
	public class Calculator
	{
		private static Calculator _this = new Calculator();

		private Calculator()
		{
		}

		public static Calculator Static {
			get {
				return _this;
			}
		}

		private void CalcQuote_MA(Quote q)
		{
			Output.Log("calculate MA trend of " + q.Name);


		}

		public void UpdateAll()
		{
			try {
				QuoteManager.Static.CalcAllQuotes(null);
			} catch (Exception e) {
				Output.LogException(e.Message);
			}
		}
	}//clas
}

