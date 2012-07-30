using System;
using System.Collections.Generic;
using System.IO;

namespace StockFilter
{
	public class WebStockDetail_YAHOO : WebStockDetail
	{
		public WebStockDetail_YAHOO() : base(new source_YAHOO())
		{
		}

		protected override List<dateData> DoGetHistory(string  code4src, date date_from, date date_to)
		{
			List<dateData> list = new List<dateData>();
			try {
				string content;
				if (date_from.year == 0 || date_to.year == 0) {
					Output.Log("update quote " + code4src + " (desn't have all datas data)");
					content = WebUtil.Static.FetchWebPage(@"http://ichart.finance.yahoo.com/table.csv?s=" + code4src);
				} else {
					Output.Log("update quote " + code4src + " (desn't have data " + date_from + " to " + date_to + ")");
					string URI = @"http://ichart.finance.yahoo.com/table.csv?s=" + code4src 
						+ "&a=" + date_from.month
						+ "&b=" + date_from.day 
						+ "&c=" + date_from.year 
						+ "&d=" + date_to.month
						+ "&e=" + date_to.day 
						+ "&f=" + date_to.year;
					content = WebUtil.Static.FetchWebPage(URI);
				}

				//File.WriteAllText("code" + code4src + ".txt", content);

				string[] lines = content.Split("\n".ToCharArray());
				bool firstTitleLine = true;
				foreach (string line in lines) {

					if (firstTitleLine) {
						firstTitleLine = false;
						continue;
					}

					string[] values = line.Split(",".ToCharArray());
					if (values.Length >= 5) {
						dateData dt = new dateData();
						dt._date = Util.GetUnixTimeStamp(values [0]);
						dt._price._open = double.Parse(values [1]);
						dt._price._high = double.Parse(values [2]);
						dt._price._low = double.Parse(values [3]);
						dt._price._close = double.Parse(values [4]);
						dt._indic._volume = long.Parse(values [5]);
						list.Add(dt);
					}
				}

				Output.Log("YAHOO get history " + code4src + " from : " + date_from + " to : " + date_to);
			} catch (Exception e) {
				Output.LogException("Get history Error. code : " + code4src + " from : " + date_from + " to : " + date_to + "(msg : " + e.Message);
			}

			return list;
		}

		private void ProceContent()
		{

		}
	}
}

