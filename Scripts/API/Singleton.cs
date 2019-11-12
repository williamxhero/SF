namespace StockFilter
{
	public class Singleton<T> where T: Singleton<T>, new()
	{
		static T This;
		public static T Ins { get { if (This == null) This = new T(); return This; } }
	}
}
