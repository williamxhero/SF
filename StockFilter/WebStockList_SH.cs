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
		private int pageNum = 0;
		private string rawString;

		protected override void DoFillList()
		{
			ReadAllPages();
		}

		private void ReadAllPages()
		{
			if (pageNum == 0) {
				ReadPageCount();
			}

			for (int i = 0; i< pageNum; i++) {
				Output.Log("Web fetching SH stock codes. page " + i);
				ReadPage(i);
			}
		}

		private string ReadPageRaw(int begin_index)
		{
			string url = @"http://www.sse.com.cn/sseportal/webapp/datapresent/SSEQueryStockInfoAct?reportName=BizCompStockInfoRpt&CURSOR=" + begin_index;
			return WebUtil.Share().FetchWebPage(url);			
		}

		static string listbegin = @"<span class=""table3"">查询条件：</span>";
		static string listend = @"页/共<strong>";
		static string codeBegin = "COMPANY_CODE=";

		int RecordOne(ref string list, int curpos)
		{
			do{
				int code_pos = list.IndexOf(codeBegin, curpos);
				curpos = code_pos + codeBegin.Length;
				if (code_pos < 0) return curpos;

				string strCode = list.Substring(curpos, 6);
				int nCode = int.Parse(strCode);
				if(nCode < 600000) 
					break;

				int name_pos = list.IndexOf("<td ", curpos);
				if (name_pos < 0) 
					break;
				curpos = name_pos + 4;

				name_pos = list.IndexOf(">", curpos);
				if(name_pos < 0) 
					break;
				curpos = name_pos = name_pos + 1;

				int name_pos_end = list.IndexOf("</td>", curpos);	
				if(name_pos_end < 0) 
					break;
				curpos = name_pos_end + 5;

				string strName = list.Substring(name_pos, name_pos_end - name_pos); 
				if(strCode == "") 
					break;
				AddInfo(nCode, strName, market.type.ShangHai);
				return curpos;

			}while(false);

			throw new FormatException("web content was uncompeleted. current string pos : " + list.Substring(curpos, 512));
		}

		private void ReadPage(int pg)
		{
			//50 each page. so the cursor should be 1, 51, 101, 151,...
			int begin_index = pg * 50 + 1;
			int cell_index = begin_index + 50;

			//save one fetching time.  1st page already fetched by  ReadPageCount();
			if (begin_index > 1){
				rawString = ReadPageRaw(begin_index);
			}

			//fastforward to  key point 
			string list =Mid(rawString, listbegin, listend);
			//StringBuilder sb = new StringBuilder(list);

			int curpos = 0;
			//found each quote info:
			for (int i = 1; i< cell_index; i++) {
				curpos = RecordOne(ref list, curpos);
			}
		}

		static string keystring = "页/共<strong>";
		static string endKey = "</strong>页";

		private int ReadPageCount()
		{
			rawString = ReadPageRaw(1);
			//found the max page info:
			string num =Mid(rawString, keystring, endKey);
			pageNum = int.Parse(num);
			return pageNum;
		}


	}//class
}//namespace

