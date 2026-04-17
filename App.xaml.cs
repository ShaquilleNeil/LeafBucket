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
            var userName = await SecureStorage.GetAsync("userName");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(role))
                return;

           
            if (IsTokenExpired(idToken))
            {
                SecureStorage.RemoveAll();
                return; 
            }

            SessionManager.UserId = userId;
            SessionManager.IdToken = idToken;
            SessionManager.Location = location;
            SessionManager.UserName = userName;

            if (role == "Customer")
                MainPage = new AppShell();
            else if (role == "Farmer")
                MainPage = new FarmerShell();
        }
        catch { }
    }

    private bool IsTokenExpired(string idToken)
    {
        try
        {
        
            var parts = idToken.Split('.');
            if (parts.Length != 3) return true;

           
            var payload = parts[1];
           
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));

            var doc = System.Text.Json.JsonDocument.Parse(json);
            var exp = doc.RootElement.GetProperty("exp").GetInt64();
            var expiry = DateTimeOffset.FromUnixTimeSeconds(exp);

            return expiry < DateTimeOffset.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}
