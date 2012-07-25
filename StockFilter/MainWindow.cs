using System;
using Gtk;
using StockFilter;

public partial class MainWindow: Gtk.Window
{	
	//private StockFilter.Data _dat = new StockFilter.Data();

	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();
		Calculator.Static.CalcMA();
		//QuoteManager.Static.LoadInformation();
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
