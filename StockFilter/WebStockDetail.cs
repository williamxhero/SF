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

		public void FetchData(Quote q, date date_from, date date_to)
		{
			string code = _src.GetCode(q);
			DoFetchData(code, date_from, date_to);
		}

		public void FetchDataAllDates(Quote q)
		{
			date dt;
			dt.year = 0;
			dt.month = 0;
			dt.day = 0;
			string code = _src.GetCode(q);
			List<dateData> dataList = DoFetchData(code, dt, dt);
			q.SetDetail(dataList);
		}

		protected virtual List<dateData> DoFetchData(string code, date date_from, date date_to)
		{
			return new List<dateData>();
		}
	}

}//namespace

