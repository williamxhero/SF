using System;
using System.Collections.Generic;
using System.IO;

namespace StockFilter
{
	public class WebStockDetail_YAHOO : WebStockDetail
	{
		public WebStockDetail_YAHOO() : base(new source_YAHOO()){}

		protected override List<dateData> DoGetHistory(string  code4src, date date_from, date date_to)
		{
			string description = code4src + " from : " + date_from + " to : " + date_to;

			List<dateData> list = new List<dateData>();
			//long dateFrom = Util.GetUnixTimeStamp(date_from);
			string URI = "";

			try {

				if (date_from.year == 0 || date_to.year == 0) {
					//Output.Log("update quote " + code4src + " (desn't have all datas data)");
					URI = @"http://ichart.finance.yahoo.com/table.csv?s=" + code4src;
				} else {
					//Output.Log("update quote " + code4src + " (desn't have data " + date_from + " to " + date_to + ")");
					URI = @"http://ichart.finance.yahoo.com/table.csv?s=" + code4src 
						+ "&a=" + (date_from.month - 1)
						+ "&b=" + date_from.day
						+ "&c=" + date_from.year 
						+ "&d=" + (date_to.month - 1)
						+ "&e=" + date_to.day 
						+ "&f=" + date_to.year;
				}

				//File.WriteAllText("code" + code4src + ".txt", content);
				string content = WebUtil.Static.FetchWebPage(URI);

				string[] lines = content.Split("\n".ToCharArray());
				if(lines.Length <= 2){
					Output.Log(description + " NO data, ignore content : \"" + content + "\"");
					return list;
				}

				//titles:
				lines[0] = "";

				foreach (string line in lines) {
					if(line.Length == 0) continue;
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
					}else{
						Output.Log("format error, column less than 5 : \"" + line+ "\"");
					}
				}

				Output.Log("YAHOO get history " + description);
			
			
			} catch (Exception e) {
				Output.LogException("Get history Error. code : " + description + "(msg : " + e.Message);
			}

			return list;
		}

		private void ProceContent()
		{

		}
	}
}

