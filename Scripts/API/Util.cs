using System;

namespace StockFilter
{
	public class Util
	{
		public static long GetUnixTimeStamp(DateTime dt)
		{
			return GetUnixTimeStamp(dt.Year, dt.Month, dt.Day);
		}

		static readonly DateTime epoch = new DateTime(1970, 1, 1).ToLocalTime();

		public static long GetUnixTimeStamp(int year, int month, int day)
		{
			DateTime theDate = new DateTime(year, month, day).ToLocalTime();
			TimeSpan span = theDate - epoch;
			return (long)span.TotalSeconds;
		}
	}//class

}//namespace

