using System;

namespace StockFilter
{

	public enum IndustryCode
	{
		SWL1, //申万一级行业
		SWL2, //申万二级行业
		SWL3, //申万三级行业
		JQL1, //聚宽一级行业
		JQL2, //聚宽二级行业
		ZJH, //证监会行业
	}

	public enum SecurityType
	{
		Stock,
		Fund,
		Index,
		Futures,
		ETF,
	}

	public class Concept
	{
		public string Code;
		public string Name;
		public DateTime Start;
	}

	public class Industry
	{
		public string IdCode { get { return $"{Index}.{Code}"; } }
		public IndustryCode Code;
		public string Index;
		public string Name;
		public DateTime Start;
	}

	public partial class Security
	{
		public string IdCode { get { return $"{Code}.{Mkt.Code}"; } }
		public string Code = "";
		public string Name = "";
		public string Desc = "";
		public SecurityType Type;
		public double Price;
		public float Rate;
		public DateTime Start;
		public DateTime End = DateTime.MaxValue;
		public Market Mkt;
		public Industry[] Industry;
		public Concept[] Concept;
		public override string ToString() { return $"{Name}({Code}.{Mkt.Code}/{Desc})"; }
	}//class


	//Inner Type
	public partial class Security
	{
		public class RangeData
		{
			public DateTime Date;
			public int DayCnt;
			public PriceRange Price;
			public Indicator Indic;
		}

		public class TickData
		{
			public DateTime Date;
			public double Price;
			public long Volume;
		}

		public class PriceRange
		{
			public double Open;
			public double High;
			public double Low;
			public double Close;
		}

		public class Indicator
		{
			public double Volume;
		}

	}//class
}

