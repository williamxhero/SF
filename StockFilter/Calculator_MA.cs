using System;
using System.Collections.Generic;

namespace StockFilter
{
	public partial class Calculator
	{
		public void Calc_MA(Quote q)
		{
			Output.Log("calculate MA trend of " + q.Name);
			if (q.History.Count == 0){
				Output.Log(q.Describe + " has no history");
				return;
			}
			DoCalc_MA(q);
		}
		Account ac_ma;

		private void DoCalc_MA(Quote q )
		{
			List<dateData> history = q.History;
			ac_ma = Market.Static.ConnectAccount();
			ac_ma.found = 100000; //starting found : 10W

			int[] MADayUnit = new int[]{5, 10, 20, 60}; //a week
			Dictionary<int, double> MAValue = new Dictionary<int, double>();

			foreach (int ma in MADayUnit){
				MAValue [ma] = 0;
			}

			for (int index = 0; index < history.Count; index++){
				int count = index + 1;

				dateData ddM = history [index];
				double priceM = ddM._price._close;
				Console.Write(priceM);

				foreach (int ma in MADayUnit){
					double maVal = MAValue [ma];

					Output.LogNR(",");
					if (count < ma){
						maVal += priceM; //add only.
					}
					else{
						if (count == ma){//calc 1st average
							maVal += priceM;
							maVal /= ma; 
						}
						else{ //moving average
							dateData ddM_n = history [index - ma];
							double priceM_n = ddM_n._price._close;
							maVal = maVal - (priceM_n / ma) + (priceM / ma);
						}

						maVal = Math.Round(maVal, 2);
						Console.Write(maVal);
					}

					MAValue [ma] = maVal;
				}//for
				Output.Log("");

				MA_FindSellBuyPoint(q, MAValue, priceM);
			}

			Output.Log("MA calc Finished. Found : " + ac_ma.found);
		}//DoCalc_MA

		private enum pipeType
		{
			PT_NONE,
			PT_UP,
			PT_KEEP,
			PT_DOWN,
		}

		pipeType pt = pipeType.PT_NONE;

		private void MA_FindSellBuyPoint(Quote q, Dictionary<int, double> MAValue, double price)
		{
			double ma5 = MAValue[5];
			double ma10 = MAValue[10];
			double ma20 = MAValue[20];
			double ma60 = MAValue[60];

			if(ma5 < ma10 && ma10 < ma20 && ma20 < ma60){
				if(pt == pipeType.PT_NONE){
					pt = pipeType.PT_DOWN;
				}
				else if(pt != pipeType.PT_UP){
					//sell
					long sell_vol = ac_ma.CanSell(q);
					ac_ma.Sell(q, price, sell_vol);
					//ac_ma.Sell();
				}
			}
			else if(ma5 > ma10 && ma10 > ma20 && ma20 > ma60){
				if(pt == pipeType.PT_NONE){
					pt = pipeType.PT_UP;
				}
				else if(pt != pipeType.PT_UP){
					//buy
					long buy
				}
			}

		}//function

	}//class

}//namespace

