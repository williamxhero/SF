using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StockFilter
{
	partial class DB
	{
		public class TradeDay
		{
			[BsonId]
			public int Date;
		}

		public struct DBDate
		{
			public int Y;
			public int M;
			public int D;
			public static DBDate FromInt(int v) {
				if(v < 10000) return new DBDate { Y = 1980, M = 1, D = 1 };
				int Year = v / 10000;
				int Mon = (v - Year * 10000) / 100;
				int Day = (v - Year * 10000 - Mon*100);
				return new DBDate { Y =Year, M = Mon , D =  Day};
			}
			public static int ToInt(DateTime dt) { return dt.Year * 10000 + dt.Month * 100 + dt.Day; }
			public static int ToInt(DBDate dt) { return dt.Y * 10000 + dt.M * 100 + dt.D; }
			public int ToInt() { return ToInt(this);}
			public static implicit operator DBDate(DateTime v) { return new DBDate {Y = v.Year, M = v.Month, D = v.Day}; }
			public static implicit operator DateTime(DBDate v) { return new DateTime(v.Y, v.M, v.D);}
		}

		public class DBSecMgr
		{
			[BsonId]
			public string IdCode;
			public int BOD;
			public bool Male;
			public string Name;
			public string Desc;
		}

		public class DBMarket
		{
			[BsonId]
			public string IdCode;
			public string Name;
			public string Desc;
		}

		public class DBIndustry
		{
			[BsonId]
			public string IdCode;
			public string Name;
			public int Start;
		}

		public class DBConcept
		{
			[BsonId]
			public string IdCode;
			public string Name;
			public int Start;
		}

		public class DBSecurity
		{
			[BsonId]
			public string IdCode;
			public string Code;
			public string Name;
			public float Price;
			public string Desc;
			public string Type;
			public int Start;
			public int End;
			public string Market;
			public string[] Industry;
			public string[] Concept;
			public string[] Manager;
		}

		public DBIndustry ToIndustry(Industry i)
		{
			return new DBIndustry { IdCode = i.IdCode, Name = i.Name, Start = DBDate.ToInt(i.Start) };
		}

		public DBSecurity ToSecurity(Security s)
		{
			return new DBSecurity {
				IdCode = s.IdCode,
				Code = s.Code,
				Desc = s.Desc,
				End = DBDate.ToInt(s.End),
				Name = s.Name,
				Price = 0,
				Start = DBDate.ToInt(s.Start),
				Type = ToString(s.Type),
				Industry = null,
				Manager = null,
				Market = s.Mkt.Code,
				Concept = null,
			};
		}

		public static Security ToSecurity(DBSecurity sec)
		{
			var Mkt = ToMarket(sec.Market);
			var Type = ToType(sec.Type);
			Concept[] Cpts = new Concept[sec.Concept.Length];
			Industry[] Inds = new Industry[sec.Industry.Length];

			return new Security { Code = sec.Code, Mkt = Mkt, Name = sec.Name,
				Price = sec.Price, Type = Type, Start = DBDate.FromInt(sec.Start), End = DBDate.FromInt(sec.End),
			 Desc = sec.Desc, Concept = Cpts, Industry = Inds, Rate = 0 };
		}

		Market ToMarket(DBMarket mkt)
		{
			return MarketMgr.Ins.Get(mkt.IdCode);
		}

		private static Market ToMarket(string code)
		{
			return MarketMgr.Ins.Get(code);
		}

		private static string ToString(SecurityType st)
		{
			switch (st)
			{
				case SecurityType.Stock: return "stock";
				case SecurityType.Fund: return "fund";
				case SecurityType.Index: return "index";
				case SecurityType.Futures: return "futures";
				case SecurityType.ETF: return "etf";
			}
			return "stock";
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

	}

	partial class DB
	{
		static public DB Ins = new DB();

		MongoClient Client;

		IMongoDatabase Db;
		const string DbName = "Traders";

		IMongoCollection<DBMarket> MktCol;
		const string MktColName = "Markets";

		IMongoCollection<DBSecurity> SecCol;
		const string SecColName = "Securities";

		IMongoCollection<DBIndustry> IndCol;
		const string IndColName = "Industries";

		IMongoCollection<DBConcept> CptCol;
		const string CptColName = "Concepts";

		IMongoCollection<DBSecMgr> SMgrCol;
		const string SMgrColName = "Managers";

		IMongoCollection<TradeDay> TdDayCol;
		const string TdDayColName = "TradeDays";

		public bool Connect()
		{
			if (Db != null) return true;

			Client = new MongoClient("mongodb://192.168.3.41:27017");
			if (Client == null) return false;

			Db = Client.GetDatabase(DbName);
			if (Db == null) return false;

			MktCol = Db.GetCollection<DBMarket>(MktColName);
			SecCol = Db.GetCollection<DBSecurity>(SecColName);
			IndCol = Db.GetCollection<DBIndustry>(IndColName);
			CptCol = Db.GetCollection<DBConcept>(CptColName);
			SMgrCol = Db.GetCollection<DBSecMgr>(SMgrColName);
			TdDayCol = Db.GetCollection<TradeDay>(TdDayColName);

			LoadRefs();

			return AnyIsNotNull(Client, Db, MktCol, SecCol, IndCol, CptCol, SMgrCol); 
		}

		public bool IsConnected() { return Db != null; }

		void LoadRefs()
		{
			var Mkts = MktCol.Find(_ => true).ToList();
			foreach (var M in Mkts)
			{
				MarketMgr.Ins.Add(ToMarket(M));
			}
		}

		bool AnyIsNotNull(params object[] obj)
		{
			foreach(var O in obj)
			{
				if (O == null) return false;
			}
			return true;
		}


		UpdateResult InsertOrUpdateOne<T>(
			IMongoCollection<T> collection, T newInstance, 
			FilterDefinition<T> filter = null, UpdateOptions options = null, 
			CancellationToken cancellationToken = default(CancellationToken) )
		{
			var PropNameVals = GetPropertyValue(newInstance);

			if (filter == null)
			{
				foreach(var PNV in PropNameVals)
				{
					if (PNV.Value == null) continue;
					if (PNV.Key == "_id")
					{
						filter = Builders<T>.Filter.Eq("_id", PNV.Value);
						break;
					}
				}
			}

			if(filter == null)
			{
				return null;
			}

			var Result = collection.Find(filter);
			var Items = Result.ToList();
			if (Items.Count == 0)
			{
				collection.InsertOne(newInstance);
				return null;
			}

			var Update = Builders<T>.Update;
			UpdateDefinition<T> Ud = null;
			foreach (var PNV in PropNameVals)
			{
				if (PNV.Value == null) continue;
				if (Ud == null) Ud = Update.Set(PNV.Key, PNV.Value);
				else Ud = Ud.Set(PNV.Key, PNV.Value);
			}

			if (Ud == null) return null;

			return collection.UpdateOne(filter, Ud, options, cancellationToken);
		}

		List<KeyValuePair<string, object>> GetPropertyValue(object o)
		{
			var Ret = new List<KeyValuePair<string, object>>();
			var Filds = o.GetType().GetFields();
			foreach(var F in Filds)
			{
				string Name = F.Name;
				var Attr = F.GetCustomAttributes(typeof(BsonIdAttribute), true);
				if(Attr.Count() >= 1)
				{
					Name = "_id";
				}
				Ret.Add(new KeyValuePair<string, object>(Name, F.GetValue(o)));
			}

			return Ret;
		}

		public DateTime GetLastTradeDay()
		{
			if (!Connect()) return DateTime.MinValue;
			var Res  = TdDayCol.Find(x => true).
				SortByDescending(d => d.Date).
				Limit(1).
				FirstOrDefault();
			
			return DBDate.FromInt(Res?.Date??0);
		}

		public void InsertTradeDays(DateTime[] days)
		{
			if (!Connect()) return;

			foreach(var D in days)
			{
				DBDate DbD = D;
				var Td = new TradeDay { Date = DbD.ToInt() };
				TdDayCol.InsertOne(Td);
			}
		}

		public void UpdateIndustry(Industry[] inds)
		{
			if (!Connect()) return;

			foreach(var I in inds)
			{
				InsertOrUpdateOne(IndCol, new DBIndustry { IdCode = I.IdCode, Name = I.Name, Start = DBDate.ToInt(I.Start) });
			}
		}

		public void UpdateMarkets(Market[] mkts)
		{
			if (!Connect()) return;

			foreach(var M in mkts)
			{
				InsertOrUpdateOne(MktCol, new DBMarket { IdCode = M.Code, Desc = M.Desc, Name = M.Name });
			}
		}

		public List<Market> GetAllMarket()
		{
			var Ret = new List<Market>();
			if (!Connect()) return Ret;

			var Mkts = MktCol.Find(x => true);
			var MList = Mkts.ToList();

			foreach (var M in MList)
			{
				Ret.Add(ToMarket(M));
			}
			return Ret;
		}

		public void Save(Security q)
		{
			if (!Connect()) return;
			InsertOrUpdateOne(SecCol, ToSecurity(q));
		}

		public void Save(Security[] qs)
		{
			if (!Connect()) return;
			foreach(var Q in qs)
			{
				InsertOrUpdateOne(SecCol, ToSecurity(Q));
			}
		}

		public Security GetOne(string secCode)
		{
			if (!Connect()) return null;

			var FltBdr = Builders<DBSecurity>.Filter;
			var Ftr = FltBdr.Or(FltBdr.Eq("_id", secCode), FltBdr.Eq("Code", secCode));
			var Fd = SecCol.Find(Ftr);
			return ToSecurity(Fd.First());
		}
	}
}
