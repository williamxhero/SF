using System;

namespace StockFilter
{

	class RunCb : IRunTraderCallback
	{
		public void OnTick(Security.RangeData rd)
		{
			throw new NotImplementedException();
		}

		public void OnTickEnd(Security q)
		{
			throw new NotImplementedException();
		}

		public void OnTickStart(Security q)
		{
			throw new NotImplementedException();
		}
	}

	class Main
	{
		public static Main Ins = new Main();

		public void Ini()
		{
			Web.Ins.Connect();
			DB.Ins.Connect();
		}

		public void Update()
		{
			UpdateTradeDays();
			UpdateAllMarkets();
			//UpdateAllIndustries();
			UpdateAllSecurities();
		}

		public void UpdateTradeDays()
		{
			var LastD = DB.Ins.GetLastTradeDay();
			var Today = DateTime.Now;
			if (LastD.Year == Today.Year && LastD.Month == Today.Month && LastD.Day == Today.Day) return;
			var Days = Web.Ins.GetAllTradeDays(LastD);
			DB.Ins.InsertTradeDays(Days.ToArray());
		}

		public void UpdateAllMarkets()
		{
			MarketMgr.Ins.Add(new MarketSH());
			MarketMgr.Ins.Add(new MarketSZ());

			//var MList = DB.Ins.GetAllMarket();
			//DB.Ins.UpdateMarkets(MarketMgr.Ins.All);
		}

		public void UpdateAllIndustries()
		{
			var All = Web.Ins.GetAllIndustries();
			DB.Ins.UpdateIndustry(All.ToArray());
		}

		public void UpdateAllSecurities()
		{
			var AllSecs = Web.Ins.GetAllSecurities(Web.APISecurityType.stock, DateTime.Now);
			DB.Ins.Save(AllSecs.ToArray());
		}


		public void Run()
		{
			Ini();
			Update();


			//RunTrade.Ins.Init(new RunCb());


			//var API1 = SdJQ.Call(new JQData.APIGetSecurityInfo { code = "600373" });

		}
	}
}
