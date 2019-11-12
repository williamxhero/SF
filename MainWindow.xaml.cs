using StockFilter;
using System.Windows;

namespace StockFilterW
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Main.Ins.Run();
		}
	}
}
