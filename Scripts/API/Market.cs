using System.Collections.Generic;

namespace StockFilter
{
	public class Date
	{
		public int Year;
		public int Month;
		public int Day;
	}

	public class Market
	{
		public Security Get(string code) { return Ds?.GetQuote(code); }

		virtual public bool IsTradeDay(Date d) { return false; }
		virtual public Date GetFirstTradeDay(Date from) { return null; }
		virtual public Date GetNextTradeDay(Date until) { return null; }

		public double GetBuyTotalFound(double price, long atAmount) { double Amt = price * atAmount; Amt += GetBuySurcharge(Amt); return Amt; }
		public double GetSellTotalFound(double price, long atAmount) { double Amt = price * atAmount; Amt += GetSellSurcharge(Amt); return Amt; }

		virtual public string Name { get; }
		virtual public string Code { get; }
		virtual public string Desc { get; }
		virtual public int GetT() { return 1; } 
		virtual public bool Transfer(float amount) { return false; }
		virtual public bool Withdraw(float amount) { return false; }
		virtual public Account ConnectAccount(int accId) { return null; }
		virtual public double GetBuySurcharge(double amount) { return amount * 0.002; }
		virtual public double GetSellSurcharge(double amount) { return amount * 0.002; }

		protected Market(IDataSource ds) { Ds = ds; }

		public IDataSource Ds { get; private set; }
		public static readonly Market Unknown = new MarketUnknown();
	}//class


	public class MarketUnknown : Market
	{
		public MarketUnknown() : base(null) { }
		public override string Code { get { return "UNKNOW"; } }
		public override string Name { get { return "UNKNOW"; } }
	}


	public class MarketMgr : Singleton<MarketMgr>
	{
		static Market Make(string code)
		{
			switch (code)
			{
				case "XSHG": return new MarketSH();
				case "XSHE": return new MarketSZ();
			}
			return null;
		}

		public Market Add(Market mkt)
		{
			Markets[mkt.Code] = mkt;
			return mkt;
		}

		public int Count { get { return Markets.Count; } }

		public Market[] All
		{
			get
			{
				Market[] Ms = new Market[Markets.Count];
				Markets.Values.CopyTo(Ms, 0);
				return Ms;
			}
		}

		public Market Get(string marketCode)
		{
			// precise from market name.
			if (Markets.TryGetValue(marketCode, out Market Mkt))
			{
				return Mkt;
			}

			//fuzzy from market name.
			foreach (var Pair in Markets)
			{
				if (marketCode.Contains(Pair.Key))
				{
					return Pair.Value;
				}
			}

			//try to make new one.
			var NewM = Make(marketCode);
			if(NewM != null)
			{
				return Add(NewM);
			}

			return null;
		}

		Dictionary<string, Market> Markets = new Dictionary<string, Market>();
	}

}//namespace

