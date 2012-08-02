using System;
using System.Collections.Generic;


namespace StockFilter
{

	public class Account
	{
		public struct PriceVolume
		{
			public double price;
			public long volume;
			public PriceVolume(double p, long v){
				price = p;
				volume = v;
			}
		}//struct

		Dictionary<	string, List<PriceVolume>> QuotesHistory = new Dictionary<string, List<PriceVolume>>();
		public double found;

		private long Has(Quote q)
		{
			if( ! QuotesHistory.ContainsKey(q.CodeStr)){
				return 0;
			}
			var priceVolHist = QuotesHistory[q.CodeStr];
			long num = 0;
			foreach(var pricVol in priceVolHist){
				num += pricVol.volume;
			}
			return num;
		}

		public void Buy(Quote q, double price, long vol)
		{
			double totalFoundNeeds = price * vol;
			if(totalFoundNeeds > found){
				vol = (long)(found / price);
				Output.Log("inserfacient founds for $" + vol + " " + q.Describe + "X" + vol + ". deduce to X" + vol);
			}
			if(vol == 0) {
				Output.Log("No more money");
			}
			if( ! QuotesHistory.ContainsKey(q.CodeStr)){
				QuotesHistory.Add(q.CodeStr, new List<PriceVolume>());
			}
			var priceVolHist = QuotesHistory[q.CodeStr];
			priceVolHist.Add(new PriceVolume(price, vol));

			Output.Log("BUY " + q.Describe + ", X," + vol + ", @," + price + " (," + ( price * vol ) + ",). found : ," + found);
		}

		public void Sell(Quote q, double price, long vol)
		{
			long volHas = Has(q);
			if( volHas == 0){
				string desc = "Currently Holds no quotes. Can not sell X" + vol + " " + q.Describe + " @" + price;
				Output.Log(desc);
				throw new InvalidOperationException(desc);
			}

			if(vol > volHas){
				Output.Log("Only " + volHas + " available. Can not sell X" + vol + ". sell all of " + volHas + " " + q.Describe + " @" + price);
				vol = volHas;
			}

			var priceVolHist = QuotesHistory[q.CodeStr];
			priceVolHist.Add(new PriceVolume(price, -vol));

			double foundGain = price* vol;
			foundGain -= foundGain * 0.02; //service charge
			found += foundGain;

			Output.Log("SELL " + q.Describe + ", X," + vol + ", @," + price + "(," + ( price * vol ) + ",). found : ," + found);
		}

		public void SaveHistory()
		{
		}

		public void LoadHistory()
		{
		}

	}//class


	public class Market
	{
		public Market()
		{
		}

		public void OpenAccount()
		{

		}

		public Account ConnectAccount()
		{
			return new Account();
		}

	}//class

}//namespace

