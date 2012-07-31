using System;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System.Collections.Generic;

namespace StockFilter
{
	public class WebStockDetail_GOOG : WebStockDetail
	{
		private static SpreadsheetsService service = null;

		public WebStockDetail_GOOG() : base(new source_GOOG())
		{
		}

		private void Login_OAuth2()
		{
			////////////////////////////////////////////////////////////////////////////
			// STEP 1: Configure how to perform OAuth 2.0
			////////////////////////////////////////////////////////////////////////////

			// TODO: Update the following information with that obtained from
			// https://code.google.com/apis/console. After registering
			// your application, these will be provided for you.

			string CLIENT_ID = "563378205629.apps.googleusercontent.com";

			// This is the OAuth 2.0 Client Secret retrieved
			// above.  Be sure to store this value securely.  Leaking this
			// value would enable others to act on behalf of your application!
			string CLIENT_SECRET = "LTjwzC8lyx6gTcTR-yKvJojy";

			// Space separated list of scopes for which to request access.
			string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

			// This is the Redirect URI for installed applications.
			// If you are building a web application, you have to set your
			// Redirect URI at https://code.google.com/apis/console.
			string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

			////////////////////////////////////////////////////////////////////////////
			// STEP 2: Set up the OAuth 2.0 object
			////////////////////////////////////////////////////////////////////////////

			// OAuth2Parameters holds all the parameters related to OAuth 2.0.
			OAuth2Parameters parameters = new OAuth2Parameters();

			// Set your OAuth 2.0 Client Id (which you can register at
			// https://code.google.com/apis/console).
			parameters.ClientId = CLIENT_ID;

			// Set your OAuth 2.0 Client Secret, which can be obtained at
			// https://code.google.com/apis/console.
			parameters.ClientSecret = CLIENT_SECRET;

			// Set your Redirect URI, which can be registered at
			// https://code.google.com/apis/console.
			parameters.RedirectUri = REDIRECT_URI;

			////////////////////////////////////////////////////////////////////////////
			// STEP 3: Get the Authorization URL
			////////////////////////////////////////////////////////////////////////////

			// Set the scope for this particular service.
			parameters.Scope = SCOPE;

			// Get the authorization url.  The user of your application must visit
			// this url in order to authorize with Google.  If you are building a
			// browser-based application, you can redirect the user to the authorization
			// url.

			string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
			Console.WriteLine(authorizationUrl);
			Console.WriteLine("Please visit the URL above to authorize your OAuth "
				+ "request token.  Once that is complete, type in your access code to "
				+ "continue..."
			);

			//string authPage = WebUtil.Share().FetchWebPage(authorizationUrl);
			parameters.AccessCode = "4/XafgMCkzSYVgiFNBD9J0n9Kkp4X-.AuaA_wRiBmARgrKXntQAax0MeXZ4cQI";//Console.ReadLine();

			////////////////////////////////////////////////////////////////////////////
			// STEP 4: Get the Access Token
			////////////////////////////////////////////////////////////////////////////

			// Once the user authorizes with Google, the request token can be exchanged
			// for a long-lived access token.  If you are building a browser-based
			// application, you should parse the incoming request token from the url and
			// set it in OAuthParameters before calling GetAccessToken().

			OAuthUtil.GetAccessToken(parameters);
			string accessToken = parameters.AccessToken;
			Console.WriteLine("OAuth Access Token: " + accessToken);

			// Initialize the variables needed to make the request
			GOAuth2RequestFactory requestFactory =
				new GOAuth2RequestFactory(null, "MySpreadsheetIntegration-v1", parameters);
			service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
			service.RequestFactory = requestFactory;
		}

		private void Login_UsrPwd()
		{
			string USERNAME = "williamxhero@gmail.com";
			string PASSWORD = "wi725hi844";

			service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
			service.setUserCredentials(USERNAME, PASSWORD);
		}

		private WorksheetEntry worksheet = null;

		private void ConnectTable()
		{
			// Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
			SpreadsheetQuery query = new SpreadsheetQuery();

			// Make a request to the API and get all spreadsheets.
			SpreadsheetFeed feed = service.Query(query);
			if (feed.Entries.Count == 0) {
				return;
			}

			SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries [0];
			WorksheetFeed wsFeed = spreadsheet.Worksheets;
			worksheet = (WorksheetEntry)wsFeed.Entries [0];
		}

		private ListFeed GetList()
		{
			AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
			ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
			ListFeed listFeed = service.Query(listQuery);
			return listFeed;
		}

		private CellFeed GetCells()
		{
			CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
			CellFeed cellFeed = service.Query(cellQuery);
			return cellFeed;
		}

		private void SetCell(string code, date date_from, date date_to)
		{
			// Fetch the cell feed of the worksheet.
			CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
			CellFeed cellFeed = service.Query(cellQuery);
			CellEntry cell1st = (CellEntry)cellFeed.Entries [0];
			cell1st.InputValue = "=GoogleFinance(\"" 
				+ code + "\", \"ALL\", \"" 
				+ date_from.ToString() + "\", \"" 
				+ date_to.ToString() 
				+ "\", \"DAILY\")";
			cell1st.Update();
			Output.Log("Get From Google sheet " + cell1st.InputValue);
			//Console.Write(" cnt : " + cellFeed.Entries.Count);
		}

		protected override List<dateData> DoGetHistory(string  code4src, date date_from, date date_to)
		{
			Login_UsrPwd();
			ConnectTable();
			List<dateData> list = new List<dateData>();

			try {
				SetCell(code4src, date_from, date_to);
				ListFeed listFeed = GetList();

				foreach (ListEntry row in listFeed.Entries) {
					string time = row.Elements [0].Value;
					string open = row.Elements [1].Value;
					string close = row.Elements [2].Value;
					string high = row.Elements [3].Value;
					string low = row.Elements [4].Value;
					string vol = row.Elements [5].Value;

					Output.Log(code4src + "|" + time + "|" + open + "|" + close + "|" + high + "|" + low + "|" + vol);

					try {
						dateData dd = new dateData();
						string[] date_time = time.Split(" ".ToCharArray());
						dd._date = Util.GetUnixTimeStamp(date_time [0]);
						dd._indic._volume = long.Parse(vol);
						dd._price._open = double.Parse(open);
						dd._price._high = double.Parse(high);
						dd._price._low = double.Parse(low);
						dd._price._close = double.Parse(close);
						list.Add(dd);
					} catch (Exception e) {
						string desc = code4src + " data from web format incorrect : " + e.Message;
						Output.Log(desc);
						Output.LogException(desc);
					}
				}

				Output.Log("YAHOO get history " + code4src + " from : " + date_from + " to : " + date_to);
			} catch (Exception e) {
				Output.LogException("get From Google failed : " + code4src + " " + date_from + " - " + date_to + " msg : " + e.Message);
			}

			return list;
		}//function


	}//class
}//namespace

