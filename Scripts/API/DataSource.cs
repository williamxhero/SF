using System;
using System.Collections;

namespace StockFilter
{
	public interface IDataSource
	{
		bool Connect();
		bool Disconnect();
		
		Security GetFirstQuote();
		Security GetNextQuote();

		Security GetQuote(string code);

		Security.RangeData GetFirstRange(string code, DateTime dtFrom, DateTime dtTo, int dayAve);
		Security.RangeData GetNextRange();

		Security.TickData GetFirstTickData(string code, DateTime dtFrom, DateTime dtTo);
		Security.TickData GetNextTickData();
	}


}
