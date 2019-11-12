using System;

namespace StockFilter
{
	class DataSourceSZ : IDataSource
	{
		public bool Connect()
		{ 
			throw new NotImplementedException();
		}

		public bool Disconnect()
		{
			throw new NotImplementedException();
		}

		public Security GetFirstQuote()
		{
			throw new NotImplementedException();
		}

		public Security.RangeData GetFirstRange(string code, DateTime dtFrom, DateTime dtTo, int dayAve)
		{
			throw new NotImplementedException();
		}

		public Security.TickData GetFirstTickData(string code, DateTime dtFrom, DateTime dtTo)
		{
			throw new NotImplementedException();
		}

		public Security GetNextQuote()
		{
			throw new NotImplementedException();
		}

		public Security.RangeData GetNextRange()
		{
			throw new NotImplementedException();
		}

		public Security.TickData GetNextTickData()
		{
			throw new NotImplementedException();
		}

		public Security GetQuote(string code)
		{
			throw new NotImplementedException();
		}
	}
}
