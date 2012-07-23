using System;

namespace StockFilter
{
	public struct market
	{
		///@note never change the enum value.
		public enum type
		{
			All = 0,
			ShenZhen = 1,
			ShangHai = 2,
		}

		public override string ToString()
		{
			return ((int)_value).ToString();
		}

		public market(type v)
		{
			_value = v;
		}

		public type _value;
	}//market

	public struct information
	{
		public int _code;
		public market _market;
		public string _name; ///< text code like MSFT 吴忠仪表
		public void copy(information other)
		{
			_code = other._code;
			_market = other._market;
			_name = other._name;
		}
		public information(int c, market m, string n)
		{
			_code = c;
			_market = m;
			_name = n;
		}
		public static information EMPTY;
	}//information

	public struct date
	{
		public int year;
		public int month;
		public int day;
		public override string ToString()
		{
			return year.ToString("D4") + "-" + month.ToString("D2") + "-" + day.ToString("D2");
		}
	}//date

	public struct price
	{
		public double _open;
		public double _high;
		public double _low;
		public double _close;
	}//price

	public struct indicator
	{
		public float _volume;
	}//indicator

	public struct dateData
	{
		public long _date;
		public price _price;
		public indicator _indic;
	}//dateData

}

