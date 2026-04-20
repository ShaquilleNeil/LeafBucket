using LeafBucket.Helpers;
using LeafBucket.Services;
using System.Text.RegularExpressions;

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

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            EmailErrorLabel.Text = "Please enter a valid email address.";
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
                EmailErrorLabel.Text = "Authentication failed.";
                EmailErrorLabel.IsVisible = true;
                return;
            }

            var user = await _authService.fetchUser(auth.localId, auth.idToken);

            if (user == null || string.IsNullOrEmpty(user.role))
            {
                EmailErrorLabel.Text = "User data is invalid.";
                EmailErrorLabel.IsVisible = true;
                return;
            }

            SessionManager.Location = user.address;
            SessionManager.UserId = auth.localId;
            SessionManager.IdToken = auth.idToken;
            SessionManager.UserName = user.firstName + " " + user.lastName;

            await SecureStorage.SetAsync("userId", auth.localId);
            await SecureStorage.SetAsync("idToken", auth.idToken);
            await SecureStorage.SetAsync("role", user.role);
            await SecureStorage.SetAsync("location", user.address);
            await SecureStorage.SetAsync("userName", user.firstName + " " + user.lastName);

            if (user.role == "Customer")
            {
                await CartManager.FetchCartFromFirestore();
                Application.Current.MainPage = new AppShell();
            }
            else if (user.role == "Farmer")
            {
                Application.Current.MainPage = new FarmerShell();
            }
            else
            {
                EmailErrorLabel.Text = "Unknown user role.";
                EmailErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            var message = ex.Message;

            if (message.Contains("INVALID_PASSWORD") || message.Contains("INVALID_LOGIN_CREDENTIALS"))
                PasswordErrorLabel.Text = "Incorrect password. Please try again.";
            else if (message.Contains("EMAIL_NOT_FOUND") || message.Contains("USER_NOT_FOUND"))
                EmailErrorLabel.Text = "No account found with this email.";
            else if (message.Contains("TOO_MANY_ATTEMPTS"))
                EmailErrorLabel.Text = "Too many attempts. Please try again later.";
            else
                EmailErrorLabel.Text = "Login failed. Please check your credentials.";

            EmailErrorLabel.IsVisible = true;
            PasswordErrorLabel.IsVisible = PasswordErrorLabel.Text != null && PasswordErrorLabel.Text != "";
        }
    }
}