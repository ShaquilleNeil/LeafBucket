using LeafBucket.Helpers;
using LeafBucket.Views.Auth;

namespace LeafBucket;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new LoginPage());

        _ = TryRestoreSession();
    }

    private async Task TryRestoreSession()
    {
        try
        {
            var userId = await SecureStorage.GetAsync("userId");
            var idToken = await SecureStorage.GetAsync("idToken");
            var role = await SecureStorage.GetAsync("role");
            var location = await SecureStorage.GetAsync("location");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(role))
                return;

            SessionManager.UserId = userId;
            SessionManager.IdToken = idToken;
            SessionManager.Location = location;

            if (role == "Customer")
                MainPage = new AppShell();
            else if (role == "Farmer")
                MainPage = new FarmerShell();
        }
        catch
        {
           
        }
    }
}
