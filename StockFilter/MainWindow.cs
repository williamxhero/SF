using System;
using Gtk;


public partial class MainWindow: Gtk.Window
{	
	private StockFilter.Data _dat = new StockFilter.Data();

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	protected void FetchFromNet (object sender, EventArgs e)
	{
		_dat.Fetch();
	}

}
