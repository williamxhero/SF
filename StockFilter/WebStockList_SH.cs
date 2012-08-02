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

		protected override string GetMaxPgBegin(){return @"页/共<strong>";}
		protected override string GetMaxPgEnd(){return @"</strong>页";}

		protected override int GetWebPageIndex(int pg){
			int begin_index = (pg - 1) * 50 + 1;
			return begin_index;
		}

		protected override string GetWebPageURL(int cid, int webIndex)
		{
			string url = @"http://www.sse.com.cn/sseportal/webapp/datapresent/SSEQueryStockInfoAct?reportName=BizCompStockInfoRpt&CURSOR=" + webIndex;
			return url;
		}

		protected override string GetContentBegin(){return @"证券简称</td>";}
		protected override string GetContentEnd(){return @"页/共<strong>";}

		private static string codeBegin = "COMPANY_CODE=";

		protected override int RecordOne(string list, int curpos, ref information info)
		{
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

			info.CodeInt = nCode;
			info._market._value = market.type.ShangHai;
			info._name = strName;

			return curpos;
		}

		private static bool szzs_added = false;
		private void AddSZZS()
		{
			if(szzs_added) return;
			szzs_added = true;

			information info = information.EMPTY;
			info.CodeInt = 1;
			info._market._value = market.type.ShangHai;
			info._name = "上证指数";
			SaveInfomation(info);
		}

	}//class
}//namespace

