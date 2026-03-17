using LeafBucket.pages.authentication;
using MauiApp1;
namespace LeafBucket;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		 MainPage = new NavigationPage(new Login());
	}
}
