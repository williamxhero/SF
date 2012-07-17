using System;

namespace StockFilter
{
	public class Quote
	{
		public string _code;
		public string _name;

		public string GetCode()
		{

		}

		public void SetNumber(string num)
		{

		}
	}


	/// <summary>
	/// Quote manager.
	/// </summary>
	public class QuoteManager
	{
		/// <summary>
		/// code to quota infomation
		/// </summary>
		Dictionary<string, Quote> dictionary = new Dictionary<string, Quote>();

		/// <summary>
		/// fetch from net.
		/// </summary>
		public void Update()
		{
			Save();
		}

		/// <summary>
		/// load from desk
		/// </summary>
		public void Load()
		{
			//50 each page. so the cursor should be 1, 51, 101, 151,...
			//http://www.sse.com.cn/sseportal/webapp/datapresent/SSEQueryStockInfoAct?reportName=BizCompStockInfoRpt&CURSOR=101
		}

		public void Save()
		{
		}
	}
}

