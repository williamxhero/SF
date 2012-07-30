using System;
using System.Collections.Generic;

namespace StockFilter
{
	public class WebStockDetail
	{
		protected source _src;
		public WebStockDetail(source src)
		{
			_src = src;
		}

		public List<dateData> GetHistory(Quote q, date date_from, date date_to)
		{
			string code = _src.GetCode(q);
			Output.Log("update quote " + q.Describe + " (desn't have data " + date_from + " to " + date_to + ")");
			return DoGetHistory(code, date_from, date_to);
		}

		public List<dateData> GetHistory(Quote q)
		{
			date dt;
			dt.year = 0;
			dt.month = 0;
			dt.day = 0;
			string code = _src.GetCode(q);
			Output.Log("update quote " + q.Describe + " (desn't have all datas data)");
			List<dateData> dataList = DoGetHistory(code, dt, dt);
			return dataList;
		}

		protected virtual List<dateData> DoGetHistory(string code4src, date date_from, date date_to)
		{
			return new List<dateData>();
		}
	}

}//namespace

