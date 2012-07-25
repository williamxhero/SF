using System;

namespace StockFilter
{
	public class source
	{
		public virtual string GetCode(Quote q){return q.CodeStr;}

	}//class

	public class source_YAHOO : source
	{
		public override string GetCode(Quote q)
		{
			switch(q.market._value)
			{
				case market.type.ShangHai: return q.CodeStr + ".ss";
				case market.type.ShenZhen: return q.CodeStr + ".sz";
			}
			return "";
		}
	}//class

	public class source_GOOG : source
	{
		public override string GetCode(Quote q)
		{
			switch(q.market._value)
			{
				case market.type.ShangHai: return q.CodeStr + ".ss";
				case market.type.ShenZhen: return q.CodeStr + ".sz";
			}
			return "";
		}
	}//class

	public class source_HEXUN : source
	{
		public override string GetCode(Quote q)
		{
			switch(q.market._value)
			{
				case market.type.ShangHai: return q.CodeStr + ".sh";
				case market.type.ShenZhen: return q.CodeStr + ".sz";
			}
			return "";
		}
	}//class


}//namespace

