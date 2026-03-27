using LeafBucket.Views.Auth;

namespace LeafBucket;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new LoginPage());
    }
}
