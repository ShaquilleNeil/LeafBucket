using LeafBucket.Helpers;
using LeafBucket.Services;

namespace LeafBucket.Views.Auth;

public partial class LoginPage : ContentPage
{
	private AuthService _authService = new AuthService();
	public LoginPage()
	{
		InitializeComponent();
        
	}


	private void CreateAccountNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new SignupPage());
	}

	private void ForgotPasswordNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ForgotPasswordPage());
	}

	private async void LoginButton_Clicked(object sender, EventArgs e)
{
    EmailErrorLabel.IsVisible = false;
    PasswordErrorLabel.IsVisible = false;

    string email = Email.Text?.Trim() ?? "";
    string password = Password.Text ?? "";

    if (string.IsNullOrEmpty(email))
    {
        EmailErrorLabel.Text = "Please enter your email.";
        EmailErrorLabel.IsVisible = true;
        return;
    }

    if (string.IsNullOrEmpty(password))
    {
        PasswordErrorLabel.Text = "Please enter your password.";
        PasswordErrorLabel.IsVisible = true;
        return;
    }

    try
    {
        var auth = await _authService.SignIn(email, password);

        if (auth == null || string.IsNullOrEmpty(auth.localId) || string.IsNullOrEmpty(auth.idToken))
        {
            await DisplayAlert("Error", "Authentication failed.", "OK");
            return;
        }

        Console.WriteLine("Auth success");

        var user = await _authService.fetchUser(auth.localId, auth.idToken);

    
        if (user == null || string.IsNullOrEmpty(user.role))
        {
            await DisplayAlert("Error", "User data is invalid", "OK");
            return;
        }

        Console.WriteLine($"Role: {user.role}");

            SessionManager.Location = user.address;
            SessionManager.UserId = auth.localId;
            SessionManager.IdToken = auth.idToken;

            await SecureStorage.SetAsync("userId", auth.localId);
            await SecureStorage.SetAsync("idToken", auth.idToken);
            await SecureStorage.SetAsync("role", user.role);
            await SecureStorage.SetAsync("location", user.address);

            if (user.role == "Customer")
    Application.Current.MainPage = new AppShell();
else if (user.role == "Farmer")
    Application.Current.MainPage = new FarmerShell();
        else
            await DisplayAlert("Error", "Unknown user role", "OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex}");
        await DisplayAlert("Error", ex.Message, "OK");
    }
}
	







}



