using System;

namespace StockFilter
{
	class MarketSZ : Market
	{
		public override string Code { get { return "XSHE"; } }
		public override string Name { get { return "Shenzhen"; } }
		public override string Desc { get { return "China SZ stock market."; } }

		readonly float YHS = 1f / 1000f; //印花
		readonly float GHF = 0.002f / 100f; //过户
		readonly float SXF = 1f / 1000f; //手续

		public override double GetBuySurcharge(double amount)
		{
			return Math.Round(Math.Max(amount * SXF, 5) + (amount * GHF), 2);
		}

		public override double GetSellSurcharge(double amount)
		{
			return Math.Round(Math.Max(amount * SXF, 5) + amount * (YHS + GHF), 2);
		}

		public MarketSZ() : base(new DataSourceSZ()) { }
	}

}
