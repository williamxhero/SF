using System;

namespace StockFilter
{

	class DataSourceSH : IDataSource
	{
		public bool Connect()
		{
			return DB.Ins.IsConnected() && Web.Ins.IsConnected();
		}

		public bool Disconnect()
		{
			return true;
		}

		public Security.RangeData GetFirstRange(string code, DateTime dtFrom, DateTime dtTo, int dayAve)
		{
			throw new NotImplementedException();
		}

		public Security.TickData GetFirstTickData(string code, DateTime dtFrom, DateTime dtTo)
		{
			throw new NotImplementedException();
		}

		public Security GetFirstQuote()
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
