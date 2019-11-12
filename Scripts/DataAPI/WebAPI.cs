using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;


namespace StockFilter
{
	/// <summary>
	/// Data Structure Part.
	/// </summary>
	public partial class Web
	{
		public class APIIndustry
		{
			public string index { get; set; }
			public string name { get; set; }
			public string start_date { get; set; }
		}

		public class APISecurity
		{
			public string code { get; set; }
			public string display_name { get; set; }
			public string name { get; set; }
			public string start_date { get; set; }
			public string end_date { get; set; }
			public string type { get; set; }
		}

		public class APIDateDay
		{
			public int Y;
			public int M;
			public int D;
			public APIDateDay() { var Td = DateTime.Now; Y = Td.Year; M = Td.Month; D = Td.Day; }
			public APIDateDay(int y, int m, int d) { Y = y; M = m; D = d; }
			public override string ToString() { return $"{Y:D4}-{M:D2}-{D:D2}"; }
			public static implicit operator APIDateDay(DateTime v) { return new APIDateDay { Y = v.Year, M = v.Month, D = v.Day }; }
		}

		public enum APISecurityType
		{
			stock,
			fund,
			index,
			futures,
			eft,
		}

		public static Industry ToIndustry(APIIndustry ind, APIIndustryCode code)
		{
			return new Industry { Code = ToIndCode(code), Index = ind.index, Name = ind.name, Start = DateTime.Parse(ind.start_date) };
		}

		public static Security ToSecurity(APISecurity s)
		{
			Market Mkt = ToMarket(s.code);
			SecurityType Typ = ToType(s.type);
			var End = (s.end_date == "2200-01-01") ? DateTime.MaxValue : DateTime.Parse(s.end_date);
			var Code = s.code;
			var DotIdx = Code.IndexOf(".");
			if (DotIdx >= 0)
			{
				Code = Code.Substring(0, DotIdx);
			}
			return new Security 
			{
				Code = Code,
				Name = s.display_name,
				Mkt = Mkt,
				Type = Typ,
				Desc = s.name,
				End = End,
				Start = DateTime.Parse(s.start_date)
			};
		}

		private static Market ToMarket(string secCode)
		{
			var DotIdx = secCode.IndexOf('.');

			if (DotIdx > 1)
			{
				var Code = secCode.Substring(0, DotIdx);
				var MktStr = secCode.Substring(DotIdx+1);
				return MarketMgr.Ins.Get(MktStr);
			}

			return MarketMgr.Ins.Get(secCode);
		}

		private static SecurityType ToType(string type)
		{
			SecurityType Typ = SecurityType.Stock;
			switch (type.ToLower())
			{
				case "stock": Typ = SecurityType.Stock; break;
				case "fund": Typ = SecurityType.Fund; break;
				case "index": Typ = SecurityType.Index; break;
				case "futures": Typ = SecurityType.Futures; break;
				case "eft": Typ = SecurityType.ETF; break;
			}

			return Typ;
		}

		static IndustryCode ToIndCode(APIIndustryCode c)
		{
			switch (c)
			{
				case APIIndustryCode.jq_l1: return IndustryCode.JQL1;
				case APIIndustryCode.jq_l2: return IndustryCode.JQL2;
				case APIIndustryCode.sw_l1: return IndustryCode.SWL1;
				case APIIndustryCode.sw_l2: return IndustryCode.SWL2;
				case APIIndustryCode.sw_l3: return IndustryCode.SWL3;
				case APIIndustryCode.zjw: return IndustryCode.ZJH;
			}
			return IndustryCode.ZJH;
		}
	}


	/// <summary>
	/// API defination part.
	/// </summary>
	public partial class Web
	{
		public abstract class API
		{
			[NonSerialized]
			public bool Success = false;
			public abstract void SetResult(string content);

			public string token = "";
			public readonly string method;
			protected API(string meth) { method = meth; }
		}

		public class APIGetSecurityInfo : API
		{
			public APIGetSecurityInfo() : base("get_security_info") { }
			public string code = "";
			[NonSerialized]
			public Security Result;
			public override void SetResult(string content)
			{
				using (TextReader Tr = new StringReader(content))
				{
					var CsvRr = new CsvReader(Tr);
					var Secus = CsvRr.GetRecords<APISecurity>();
					foreach (var Se in Secus)
					{
						Result = ToSecurity(Se);
					}
				}
			}
		}

		public enum APIIndustryCode
		{
			sw_l1, //申万一级行业
			sw_l2, //申万二级行业
			sw_l3, //申万三级行业
			jq_l1, //聚宽一级行业
			jq_l2, //聚宽二级行业
			zjw, //证监会行业
		}

		public class APIGetAllIndustries : API
		{
			public APIGetAllIndustries() : base("get_industries") { }
			public APIIndustryCode code = APIIndustryCode.zjw;
			public List<Industry> Result = new List<Industry>();

			public override void SetResult(string content)
			{
				using (TextReader Tr = new StringReader(content))
				{
					var CsvRr = new CsvReader(Tr);
					var Secus = CsvRr.GetRecords<APIIndustry>();
					foreach (var Se in Secus)
					{
						Industry S = ToIndustry(Se, code);
						Result.Add(S);
					}
				}
			}
		}

		public class APIGetAllTradeDays : API
		{
			public APIGetAllTradeDays() : base("get_trade_days") { }

			public APIDateDay date;
			public APIDateDay end_date = new APIDateDay();

			[NonSerialized]
			public List<DateTime> Result = new List<DateTime>();
			public override void SetResult(string content)
			{
				using (TextReader Tr = new StringReader(content))
				{
					while(true)
					{
						var Line = Tr.ReadLine();
						if (Line == null) break;
						Result.Add(DateTime.Parse(Line));
					}
				}
			}
		}

		public class APIGetAllSecurities : API
		{
			public APIGetAllSecurities() : base("get_all_securities") { }
			public APISecurityType code = APISecurityType.stock;
			public APIDateDay date = new APIDateDay();
			[NonSerialized]
			public List<Security> Result = new List<Security>();
			public override void SetResult(string content)
			{
				using (TextReader Tr = new StringReader(content))
				{
					var CsvRr = new CsvReader(Tr);
					var Secus = CsvRr.GetRecords<APISecurity>();
					foreach (var Se in Secus)
					{
						Security S = ToSecurity(Se);
						Result.Add(S);
					}
				}
			}
		}

	}//class JQData


	/// <summary>
	/// API functions
	/// </summary>
	public partial class Web
	{
		public static Web Ins = new Web();

		readonly string Url = "https://dataapi.joinquant.com/apis";
		string Token = null;

		public bool Connect()
		{
			if (Token != null) return true;

			try
			{
				using (var client = new HttpClient())
				{
					//需要添加System.Web.Extensions
					//生成JSON请求信息
					string json = new JavaScriptSerializer().Serialize(new
					{
						method = "get_token",
						mob = "15802171704",
						pwd = "hewei13431"
					});
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					//POST请求并等待结果
					var result = client.PostAsync(Url, content).Result;
					if (!result.IsSuccessStatusCode)
					{
						Token = null;
						return false;
					}

					//读取返回的TOKEN
					Token = result.Content.ReadAsStringAsync().Result;

					if (Token.Contains("error"))
					{
						Token = null;
						return false;
					}
					return true;
				}
			}
			catch {
				Token = null;
				return false;
			}
		}

		public bool IsConnected() { return Token != null; }

		public List<Security> GetAllSecurities(APISecurityType type, DateTime onDate)
		{
			var AllSecs = Call(new APIGetAllSecurities { code = type, date = onDate});
			return AllSecs.Result;
		}

		public List<DateTime> GetAllTradeDays(DateTime from)
		{
			var All = Call(new APIGetAllTradeDays { date = from });
			return All.Result;
		}

		public List<Industry> GetAllIndustries()
		{
			var AllSecs = Call(new APIGetAllIndustries {});
			return AllSecs.Result;
		}

		public T Call<T>(T api) where T : API
		{
			if (!Connect()) return api;
			string RetCont = "";

			using (var client = new HttpClient())
			{
				api.token = Token;
				var Fields = api.GetType().GetFields();
				Dictionary<string, string> StrApi = new Dictionary<string, string>();
				StringBuilder Sb = new StringBuilder();
				Sb.Append("{");
				foreach (var F in Fields)
				{
					if (F.Attributes.HasFlag(FieldAttributes.NotSerialized)) continue;
					Sb.Append("\"");
					Sb.Append(F.Name);
					Sb.Append("\":\"");
					Sb.Append(F.GetValue(api).ToString());
					Sb.Append("\",");
				}
				Sb.Remove(Sb.Length - 1, 1);
				Sb.Append("}");
				string Body = Sb.ToString();
				var Content = new StringContent(Body, Encoding.UTF8, "application/json");

				var Result = client.PostAsync(Url, Content).Result;
				RetCont = Result.Content.ReadAsStringAsync().Result;
				if (string.IsNullOrEmpty(RetCont))
				{
					return api;
				}
			}

			api.SetResult(RetCont);
			api.Success = true;
			return api;
		}

	}//class
}
