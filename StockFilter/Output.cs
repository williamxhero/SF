using System;
using System.Diagnostics;

namespace StockFilter
{
	public class Output
	{
		public Output()
		{
		}

		public static void Log(string str)
		{
			Console.WriteLine(str);
		}

		public static void LogException(string str)
		{
			Console.WriteLine(str);
		}
	}
}

