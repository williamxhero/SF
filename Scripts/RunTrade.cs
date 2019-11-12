using System;
using System.Collections.Generic;

namespace StockFilter
{
	public interface IRunTraderCallback
	{
		void OnTickStart(Security q);
		void OnTick(Security.RangeData rd);
		void OnTickEnd(Security q);
	}

	public class RunTrade : Singleton<RunTrade>
	{
		public void Init(IRunTraderCallback cb)
		{
			IRTCb = cb;
		}

		IRunTraderCallback IRTCb;

		public void TimeStart(DateTime start, DateTime end)
		{
			for (DateTime T = start; T < end; T.AddDays(1))
			{
				foreach (Security Q in Monitor)
				{
					IRTCb.OnTickStart(Q);
					var Rng= Q.Mkt.Ds.GetFirstRange(Q.Code, start, end, 1);
					while(Rng != null)
					{
						IRTCb.OnTick(Rng);
						Rng = Q.Mkt.Ds.GetNextRange();
					}
					IRTCb.OnTickEnd(Q);
				}
			}
		}

		public void AddMonitor(Market mkt, string code)
		{
			var Q = mkt.Get(code);
			if (Q != null)
			{
				Monitor.Add(Q);
			}
		}

		List<Security> Monitor = new List<Security>();
	}
}
