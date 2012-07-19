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
		List<infomation> _newQuoteInfo = new List<infomation>();

		public List<infomation> GetAllQuotesInfomation()
		{
			DoFillList();
			return _newQuoteInfo;
		}

		protected void AddInfo(int num_code, string str_code, market.type type)
		{
			var info = new infomation();
			info._code = num_code;
			info._name = str_code;
			info._market._value = type;
			_newQuoteInfo.Add(info);
		}

		protected virtual void DoFillList()
		{
		}

		public static string Mid(string str, string begin, string end)
		{
			int index = str.IndexOf(begin);
			if(index < 0) return "";
			index += begin.Length;
			int end_idx = str.IndexOf(end, index);
			if(end_idx < 0) return "";
			string num = str.Substring(index, end_idx - index);
			return num;
		}
	}


}

