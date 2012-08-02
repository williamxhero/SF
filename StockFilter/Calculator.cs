using System;

namespace StockFilter
{
	public partial class Calculator
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

		public void CalculateALL(CalcQuote cq)
		{
			QuoteManager qm = QuoteManager.Static;
			try {
				qm.LoadInformation();
				foreach (var q in qm.AllQuotes) {
					q.Value.LoadHistory();
					if (cq != null)
						cq(q.Value);
					q.Value.UnloadHistory();
				}
			} catch (Exception e) {
				Output.LogException(e.Message);
			}
		}
	}//clas
}

