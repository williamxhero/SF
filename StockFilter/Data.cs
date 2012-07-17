using System;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;



namespace StockFilter
{

	public class Data
	{
		private string _rawWebString = "";

		public Data ()
		{
		}

		public void Fetch (string quote)
		{
			FetchWebPage(@"http://ichart.finance.yahoo.com/table.csv?s=300072.sz");
		}

		private void ShowMsg (String str)
		{
			Trace.TraceInformation ("SYS:" + str);
		}

		private void FetchWebPage (string url)
		{ 
			//_enc = Encoding::GetEncoding("GBK");
			//StreamReader^ cmdStrm = gcnew StreamReader("test.html", Encoding::GetEncoding("GB2312"));
			//_buffer = cmdStrm->ReadToEnd();

			//return;

			ShowMsg ("begin");

			WebRequest request = WebRequest.Create (url);
			request.Credentials = CredentialCache.DefaultCredentials;
			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			Stream dataStream = response.GetResponseStream ();
			Encoding enc = Encoding.GetEncoding (response.CharacterSet);
			StreamReader reader = new StreamReader (dataStream, enc);
			string rawcsv = reader.ReadToEnd ();
			reader.Close ();
			dataStream.Close ();
			response.Close ();

			processCSV(rawcsv);
		}


		private void processCSV(string rawcsv)
		{
			//save to file
			//update index file.

		}


	}
}

