using System;
using Gtk;
using StockFilter;

public partial class MainWindow: Gtk.Window
{	
	//private StockFilter.Data _dat = new StockFilter.Data();

	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();
		QuoteManager.share().Update();
		StockFilter.Data.share().CreateDB();
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void FetchFromNet(object sender, EventArgs e)
	{

	}

}
