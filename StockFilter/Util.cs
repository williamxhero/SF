using System;
using System.Text;

namespace StockFilter
{
	public class Util
	{
		public static string Mid(string str, string begin, string end)
		{
			int pos_after_end;
			return Util.Mid(str, 0, begin, end, out pos_after_end);
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

		/// <summary>
		/// Strings to date.
		/// </summary>
		/// <returns>
		/// the date.
		/// </returns>
		/// <param name='dt_str'>
		/// Dt_str.  format must be "YYYY-MM-DD"
		/// </param>
		public static date StringToDate(string dt_str)
		{
			string[] vals = dt_str.Split("-_.".ToCharArray());
			if(vals.Length < 3) throw new ArgumentException("StringToDate() argument format must be \"YYYY-MM-DD\"");
			date dt;
			int.TryParse(vals[0], out dt.year);
			int.TryParse(vals[1], out dt.month);
			int.TryParse(vals[2], out dt.day);
			return dt;
		}

		public static long GetUnixTimeStamp(DateTime dt)
		{
			return GetUnixTimeStamp(dt.Year, dt.Month, dt.Day);
		}

		public static long GetUnixTimeStamp(string date_str)
		{
			return GetUnixTimeStamp(StringToDate(date_str));
		}

		public static long GetUnixTimeStamp(date dt)
		{
			return GetUnixTimeStamp(dt.year, dt.month, dt.day);
		}

		static DateTime epoch = new DateTime(1970, 1, 1).ToLocalTime();

		public static long GetUnixTimeStamp(int year, int month, int day)
		{
			//DateTime epoch = new DateTime(1970, 1, 1).ToLocalTime();
			DateTime theDate = new DateTime(year, month, day).ToLocalTime();
			TimeSpan span = theDate - epoch;
			return (long)span.TotalSeconds;
		}

		public static date GetDate(long unixTimeStamp)
		{
   			//DateTime epoch = new DateTime(1970, 1, 1).ToLocalTime();
   			DateTime t = epoch.AddSeconds(unixTimeStamp);
			date dt;
			dt.year = t.Year;
			dt.month = t.Month;
			dt.day = t.Day;
			return dt;
		}
	}//class

}//namespace

