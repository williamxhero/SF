using System;

namespace StockFilter
{
	public class source
	{
		public source(market t)
		{
			_mrkt = t;
		}

		protected market _mrkt;
		public virtual string Code(string code){return code;}

	}//class


	public class source_YAHOO : source
	{
		public source_YAHOO(market t) : base(t){}
		public override string Code(string code)
		{
			switch(_mrkt._value)
			{
				case market.type.ShangHai: return code + ".ss";
				case market.type.ShenZhen: return code + ".sz";
			}
			return "";
		}
	}//class


	public class source_HEXUN : source
	{
		public source_HEXUN(market t) : base(t){}
		public override string Code(string code)
		{
			switch(_mrkt._value)
			{
				case market.type.ShangHai: return code + ".sh";
				case market.type.ShenZhen: return code + ".sz";
			}
			return "";
		}
	}//class


}//namespace

