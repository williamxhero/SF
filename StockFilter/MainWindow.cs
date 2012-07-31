using System;
using Gtk;
using StockFilter;
using System.IO;

public partial class MainWindow: Gtk.Window
{	
	//private StockFilter.DataSource _dat = new StockFilter.DataSource();

	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();

		//Calculator.Static.UpdateAll();
		QuoteManager.Static.UpdateAllQuotes();
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void FetchFromNet(object sender, EventArgs e)
	{

	}	protected void AuthorClicked(object sender, EventArgs e)
	{
		throw new System.NotImplementedException();
	}


}
