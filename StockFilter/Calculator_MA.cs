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
			DoCalc_MA(q.History);
		}

		private void DoCalc_MA(List<dateData> history)
		{
			const int MADayUnit = 5; //a week
			const int MADayUnitNum = 4;
			const int MAMaxDays = MADayUnit * MADayUnitNum;

			double[] DayMA = new double[MAMaxDays];

			for (int index = 0; index < history.Count; index++){
				int count = index + 1;
				if(count >= 60) break;

				dateData ddM = history [index];
				double priceM = ddM._price._close;
				Console.Write(priceM);

				for (int unit  = MADayUnit; unit < MAMaxDays; unit += MADayUnit){
					Output.LogNR(",");
					if (count < unit){
						DayMA [unit] += priceM; //increase
						continue;
					}

					if (count == unit){
						DayMA [unit] += priceM;
						DayMA [unit] /= unit; //calc 1st average
					}
					else{ //moving average
						dateData ddM_n = history [index - unit];
						double priceM_n = ddM_n._price._close;
						DayMA [unit] = DayMA [unit] - (priceM_n / unit) + (priceM / unit);
					}

					DayMA [unit] = Math.Round(DayMA[unit], 2);
					Console.Write(DayMA[unit]);
				}//for

				Output.Log("");

				count++;
			}

			Output.Log("MA calc Finished");
		}//DoCalc_MA


	}//class

}//namespace

