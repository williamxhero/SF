using System;
using System.Collections.Generic;
using  System.Text;

namespace StockFilter
{
	public class WebStockList_SZ : WebStockList
	{
		protected override string GetMarketName()
		{
			return "SZ";
		}

		protected override int EntriesPerPage()
		{
			return 10;
		}

		protected override int[] GetClass(string rawstring)
		{
			//2 :  A quotes    ( 2 includes  ->    5 : small/middle companies   6 : venture  }
			return new int[]{2};
		}

		protected override string GetMaxPgBegin(){return @"当前第1页  共";}
		protected override string GetMaxPgEnd(){return @"页</td>";}

		protected override int GetWebPageIndex(int pg)
		{
			return pg;
		}

		protected override string GetWebPageURL(int cid, int webIndex)
		{
			string url = @"http://www.szse.cn/szseWeb/FrontController.szse?ACTIONID=7&AJAX=AJAX-TRUE&CATALOGID=1110&TABKEY=tab" + cid 
				+ "&tab" + cid + "PAGENUM=" + webIndex;
			return url;
		}


		protected override string GetContentBegin(){return @"所属行业";}
		protected override string GetContentEnd(){return @"当前第";}

		private static string nameKeyPoint1 = "点击查看详细资料";

		//numerical 
		private static string codeBegin = "class='cls-data-td'align='center'>";
		private static string codeEnd = "</td>";

		//name for the company
		//private static string nameBegin = ".html\"><u>";
		//private static string nameEnd = "</u>";

		//short name for A stock market.
		private static string shortBegin = "class='cls-data-td'align='center'>";
		private static string shortEnd = "</td>";

		protected override int RecordOne(string list, int curpos, ref information info)
		{
			int keypoint = list.IndexOf(nameKeyPoint1, curpos);
			if (keypoint < 0) 
				return -1; //no more code;
			curpos = keypoint;

			//code
			int pos_after_end;
			string strCode = Util.Mid(list, curpos, codeBegin, codeEnd, out pos_after_end);
			if (pos_after_end < 0)
				RecordOne_Throw(-2, list, curpos);

			int nCode;
			if (! int.TryParse(strCode, out nCode)) {
				RecordOne_Throw(-3, list, curpos);
			}
			if (nCode == 0)
				RecordOne_Throw(nCode, list, curpos);

			curpos = pos_after_end;

			//name:


			//shorten market name
			string strName = Util.Mid(list, curpos, shortBegin, shortEnd, out pos_after_end);
			if (pos_after_end < 0)
				RecordOne_Throw(nCode, list, curpos);

			curpos = pos_after_end;

			info._code = nCode;
			info._market._value = market.type.ShenZhen;
			info._name = strName;

			return curpos;
		}
	
	}//class
}//namespace

