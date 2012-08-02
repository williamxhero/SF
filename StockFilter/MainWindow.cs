using System;
using Gtk;
using StockFilter;
using System.IO;
using StockFilter;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();
	}
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void UpdateAllQuotes(object sender, EventArgs e)
	{
		QuoteManager.Static.UpdateAllQuotes();
	}	

	protected void AuthorClicked(object sender, EventArgs e)
	{
		throw new System.NotImplementedException();
	}	

	protected void CALC_MA(object sender, EventArgs e)
	{
		Calculator.Static.CalculateALL(Calculator.Static.Calc_MA);
	}

}//class
