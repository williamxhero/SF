using System;
using System.Collections.Generic;

namespace StockFilter
{
	public class Account
	{
		public class PriceVolume
		{
			public string Code;
			public double Price;
			public long Volume;

			public PriceVolume(string code, double price, long vol)
			{
				Code = code;
				Price = price;
				Volume = vol;
			}
		}//class
		
		public Market Mkt;
		public double Found;
		Dictionary<string, PriceVolume> Holding = new Dictionary<string, PriceVolume>();

		public long GetVolume(string code)
		{
			return Holding.TryGetValue(code, out PriceVolume Pv) ? Pv.Volume : 0;
		}

		public long CanBuy(double price)
		{
			long Share = (long)(Found / price);
			Share = (Share / 100) * 100;

			double TotalCost = Mkt.GetBuyTotalFound(price, Share);
			while (TotalCost > Found)
			{
				Share -= 100;
				if (Share <= 0) break;
				TotalCost = Mkt.GetBuyTotalFound(price, Share);
			}
			return Math.Max(Share, 0);
		}

		public long CanSell(string code)
		{
			return GetVolume(code);
		}

		public void Buy(Security q, double price, long vol)
		{
			double TotalFound = price * vol;
			if (TotalFound > Found){
				vol = (long)(Found / price);
				Output.Log($"Insurfacient founds for ${vol} {q.ToString()} X {vol}. deduce to X{vol}");
			}
			if (vol == 0){
				Output.Log("No more money");
			}

			Holding[q.Code] = new PriceVolume(q.Code, price, vol);
			Output.Log("BUY " + q.ToString() + ", X," + vol + ", @," + price + " (," + (price * vol) + ",). found : ," + Found);
		}

		public void Sell(string code, double price, long vol)
		{
			long VolHas = GetVolume(code);
			if (VolHas <= 0){
				string Desc = $"Currently holds no quote as {code}, Can not sell {code}X{vol}@{price}";
				Output.Log(Desc);
				throw new InvalidOperationException(Desc);
			}

			if (vol > VolHas){
				Output.Log($"Can not sell {code}X{vol}@{price}, Only {VolHas} available.");
				vol = VolHas;
			}

			var Hld = Holding[code];
			Hld.Volume -= vol;
			if(Hld.Volume <= 0){
				Holding.Remove(code);
			}

			double FoundGain = price * vol;
			FoundGain -= Mkt.GetSellSurcharge(FoundGain);
			Found += FoundGain;

			Output.Log($"SELL {code}X{vol}@{price}({(price * vol)}). found : {Found}");
		}

		public void SaveHistory() { } 
		public void LoadHistory() { }

	}//class

}//namespace

