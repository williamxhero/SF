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

		protected override int[] GetClass()
		{
			//2 :  A quotes    ( 2 includes  ->    5 : small/middle companies   6 : venture  }
			return new int[]{2};
		}

		static string keystring = "当前第1页  共";
		static string endKey = "页</td>";

		protected override string ParseMaxPage(string rawString)
		{
			//found the max page info:
			string num = Mid(rawString, keystring, endKey);
			return num;
		}

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

		static string listbegin = @"所属行业";
		static string listend = @"当前第";

		protected override string GetWebPageTableString(string rawString)
		{
			string list = Mid(rawString, listbegin, listend);
			return list;
		}

		static string nameKeyPoint1 = "点击查看详细资料";

		//numerical 
		static string codeBegin = "class='cls-data-td'align='center'>";
		static string codeEnd = "</td>";

		//name for the company
		static string nameBegin = ".html\"><u>";
		static string nameEnd = "</u>";

		//short name for A stock market.
		static string shortBegin = "class='cls-data-td'align='center'>";
		static string shortEnd = "</td>";

		protected override int RecordOne(string list, int curpos, out information info)
		{
			info = information.EMPTY;

			int keypoint = list.IndexOf(nameKeyPoint1, curpos);
			if (keypoint < 0) 
				return -1; //no more code;
			curpos = keypoint;

			//code
			int pos_after_end;
			string strCode = Mid(list, curpos, codeBegin, codeEnd, out pos_after_end);
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
			string strName = Mid(list, curpos, shortBegin, shortEnd, out pos_after_end);
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
