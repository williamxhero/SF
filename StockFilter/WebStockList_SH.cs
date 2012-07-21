using System;
using System.Collections.Generic;
using  System.Text;

namespace StockFilter
{
	/// <summary>
	/// S h_ parse.
	/// </summary>
	public class WebStockList_SH : WebStockList
	{
		protected override string GetMarketName(){ return "SH";}
		protected override int EntriesPerPage(){return 50;}

		static string keystring = "页/共<strong>";
		static string endKey = "</strong>页";
		protected override string ParseMaxPage(string rawString)
		{
			//found the max page info:
			string num = Mid(rawString, keystring, endKey);
			return num;
		}

		protected override int GetWebPageIndex(int pg){
			int begin_index = (pg - 1) * 50 + 1;
			return begin_index;
		}

		protected override string GetWebPageURL(int cid, int webIndex)
		{
			string url = @"http://www.sse.com.cn/sseportal/webapp/datapresent/SSEQueryStockInfoAct?reportName=BizCompStockInfoRpt&CURSOR=" + webIndex;
			return url;
		}

		static string listbegin = @"证券简称</td>";
		static string listend = @"页/共<strong>";

		protected override string GetWebPageTableString(string rawString)
		{						
			string list = Mid(rawString, listbegin, listend);
			return list;
		}


		static string codeBegin = "COMPANY_CODE=";

		protected override int RecordOne(string list, int curpos, out information info)
		{
			info = information.EMPTY;

			//code
			int code_pos = list.IndexOf(codeBegin, curpos);
			if (code_pos < 0){
				return -1; //this page don't have enough
			}
			curpos = code_pos + codeBegin.Length;
			string strCode = list.Substring(curpos, 6);
			int nCode;
			if( ! int.TryParse(strCode, out nCode)){
				RecordOne_Throw( -1, list, curpos);
			}
			if (nCode < 600000)
				RecordOne_Throw( nCode, list, curpos);

			//name
			int name_pos = list.IndexOf("<td", curpos); 
			if (name_pos < 0)
				RecordOne_Throw(nCode, list, curpos);

			curpos = name_pos + 4;
			name_pos = list.IndexOf(">", curpos);
			if (name_pos < 0)
				RecordOne_Throw(nCode, list, curpos);

			curpos = name_pos = name_pos + 1;
			int name_pos_end = list.IndexOf("</td>", curpos);
			if (name_pos_end < 0)
				RecordOne_Throw(nCode, list, curpos);

			string strName = list.Substring(name_pos, name_pos_end - name_pos); 
			if (strCode == "")
				RecordOne_Throw(nCode, list, curpos);

			curpos = name_pos_end + 5;

			info._code = nCode;
			info._market._value = market.type.ShangHai;
			info._name = strName;

			return curpos;
		}

	}//class
}//namespace

