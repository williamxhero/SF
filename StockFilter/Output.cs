using System;
using System.Diagnostics;
using System.IO;

namespace StockFilter
{
	public class Output
	{
		private StreamWriter w = null;
		private static Output op = new Output();
		private Output()
		{
			w = File.AppendText("exception_log.txt");
		}

		~Output(){
			if(w != null) w.Close();
		}

		public static void Log(string str)
		{
			Console.WriteLine(str);
		}

		public static void LogException(string str)
		{
			Console.WriteLine(str);
			op.w.WriteLine(DateTime.Now.ToLongTimeString() + " : " + str);
		}
	}
}

