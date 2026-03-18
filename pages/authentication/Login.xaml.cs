using MauiApp1;
using LeafBucket.Services.Auth;
using LeafBucket.pages.customer;
using LeafBucket.pages.farmer;

namespace LeafBucket.pages.authentication;

public partial class Login : ContentPage
{
	private AuthService _authService = new AuthService();
	public Login()
	{
		InitializeComponent();
	}


	private void CreateAccountNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new SignUp());
	}

	private void ForgotPasswordNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ForgotPassword());
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

        // ✅ Safety check (IMPORTANT)
        if (auth == null || string.IsNullOrEmpty(auth.localId) || string.IsNullOrEmpty(auth.idToken))
        {
            await DisplayAlert("Error", "Authentication failed.", "OK");
            return;
        }

        Console.WriteLine("Auth success");

        var user = await _authService.fetchUser(auth.localId, auth.idToken);

        // ✅ Safety check
        if (user == null || string.IsNullOrEmpty(user.role))
        {
            await DisplayAlert("Error", "User data is invalid", "OK");
            return;
        }

        Console.WriteLine($"Role: {user.role}");

        if (user.role == "Customer")
    Application.Current.MainPage = new AppShell();
else if (user.role == "Farmer")
    Application.Current.MainPage = new NavigationPage(new FarmerDashboard());
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