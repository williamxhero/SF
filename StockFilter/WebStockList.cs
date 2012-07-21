using System;
using System.Collections.Generic;
using  System.Text;

namespace StockFilter
{
	/// <summary>
	/// Web parser.
	/// </summary>
	public class WebStockList
	{
		public delegate void SaveStockInfomation(information info);

		private int pageNum = 0;
		private string rawString = "";
		private SaveStockInfomation _callback;

		public void GetAllQuotesInfomation(SaveStockInfomation callback)
		{
			_callback = callback;
			int[] cids = GetClass();
			foreach (int cid in cids) {
				pageNum = 0;
				rawString = "";
				ReadAllPages(cid);
			}
		}

		private void AddInfo(information info)
		{
			_callback(info);
		}

		private int ReadPageCount(int cid)
		{
			rawString = ReadPageRaw(cid, 1);
			string pg = ParseMaxPage(rawString);
			int pageNum;
			if (! int.TryParse(pg, out pageNum)) {
				return 0;
			}
			return pageNum;
		}

		private void ReadAllPages(int cid)
		{
			Output.Log("BEGIN  Fetch all stock information.");

			if (pageNum == 0) {
				pageNum = ReadPageCount(cid);
				Output.Log("class : " + cid + ". total page:" + pageNum);
			}

			for (int pg = 1; pg <= pageNum; pg++) {
				ReadPageWithRetry(cid, pg);
				Output.Log("-----------\n");
			}
			Output.Log("FINISHED  Fetch all stock information.");
		}

		protected void RecordOne_Throw(int code, string list, int curpos)
		{
			throw new FormatException("code(" + code + ") web format error. current string pos : " + list.Substring(curpos, 128));
		}

		protected virtual int[] GetClass()
		{
			return new int[]{0};
		}

		protected virtual string ParseMaxPage(string rawString)
		{
			return "0";
		}

		protected virtual int GetWebPageIndex(int pg)
		{
			return pg;
		}

		protected virtual string GetWebPageURL(int cid, int webIndex)
		{
			return "";
		}

		protected virtual string GetWebPageTableString(string rawString)
		{
			return "";
		}

		protected virtual int RecordOne(string list, int curpos, out information info)
		{
			info = information.EMPTY;
			return 0;
		}

		protected virtual string GetMarketName()
		{
			return "";
		}

		protected virtual int EntriesPerPage()
		{
			return 0;
		}

		private void ReadPageWithRetry(int cid, int pg)
		{
			try {
				ReadPage(cid, pg);
			} catch (FormatException e) {
				ReadPage_Retry(cid, pg, e.Message);
			} catch (Exception e) {
				Output.Log("general error : " + e.Message + ". move to next page");
			}
		}
		
		private string ReadPageRaw(int cid, int begin_index)
		{
			string url = GetWebPageURL(cid, begin_index);
			return WebUtil.Share().FetchWebPage(url);			
		}

		private void ReadPage(int cid, int pg)
		{
			//50 each page. so the cursor should be 1, 51, 101, 151,...
			int begin_index = GetWebPageIndex(pg);
			Output.Log("Fetching " + GetMarketName() + " stock codes. page: " + pg + " web page: " + begin_index);

			//save one fetching time.  1st page already fetched by  ReadPageCount();
			if (begin_index > 1) {
				rawString = ReadPageRaw(cid, begin_index);
			}

			//fastforward to  key point 
			string list = GetWebPageTableString(rawString);
			list = RemoveAllBlanks(list);

			int curpos = 0;
			int entriesNum = EntriesPerPage();
			//found each quote info:
			for (int i = 0; i< entriesNum; i++) {
				information info;
				curpos = RecordOne(list, curpos, out info);
				if (curpos == -1) {
					if (pg < pageNum) {
						throw new FormatException("format error : class(" + cid + ")page(" + pg + ") only have " + i + "/" + entriesNum + " entries");
					} else {
						break;
					}
				}
				AddInfo(info);
			}
		}

		private void ReadPage_Retry(int cid, int pg, string message)
		{
			Output.Log("page " + pg + " has format error : " + message + ".  retry one more time.");
			try {
				ReadPage(cid, pg);
				Output.Log("Now it's ok");
			} catch (FormatException e2) {
				Output.Log("still has format error. stop trying, move to next page. (" + e2.Message + ").");
			} catch (Exception e) {
				Output.Log("general error : " + e.Message + ". move to next page");
			}
		}

		public static string Mid(string str, string begin, string end)
		{
			int pos_after_end;
			return Mid(str, 0, begin, end, out pos_after_end);
		}

		public static string Mid(string str, int offset, string begin, string end, out int pos_after_end)
		{
			pos_after_end = -1;
			int beg_idx = str.IndexOf(begin, offset);
			if (beg_idx < 0)
				return "";
			beg_idx += begin.Length;
			int end_idx = str.IndexOf(end, beg_idx);
			if (end_idx < 0)
				return "";
			string num = str.Substring(beg_idx, end_idx - beg_idx);
			pos_after_end = end_idx + end.Length;
			return num;
		}

		public static string RemoveAllBlanks(string str)
		{
			StringBuilder sb = new StringBuilder(str);
			sb.Replace("\n", "");
			sb.Replace("\r", "");
			sb.Replace(" ", "");
			sb.Replace("\t", "");
			return sb.ToString();
		}
	}


}

