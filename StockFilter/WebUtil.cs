using System;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;



namespace StockFilter
{

	/// <summary>
	/// Fetch data from web
	/// </summary>
	public class WebUtil
	{
		static WebUtil _util = new WebUtil();
		public WebUtil Share()
		{
			return _util;
		}

		private void ShowMsg (String str)
		{
			Trace.TraceInformation ("begin web fetching:" + str);
		}

		public delegate void procWebString (string raw_content);

		private void FetchWebPage (string url, procWebString func)
		{
			ShowMsg (url);

			WebRequest request = WebRequest.Create (url);
			request.Credentials = CredentialCache.DefaultCredentials;
			HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
			Stream dataStream = response.GetResponseStream ();
			Encoding enc = Encoding.GetEncoding (response.CharacterSet);
			StreamReader reader = new StreamReader (dataStream, enc);
			string rawstring = reader.ReadToEnd ();
			reader.Close ();
			dataStream.Close ();
			response.Close ();
			func(rawstring);
		}

	}
}

