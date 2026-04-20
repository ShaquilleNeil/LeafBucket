using LeafBucket.Services;

namespace LeafBucket.Views.Auth;

public partial class ForgotPasswordPage : ContentPage
{
    private readonly AuthService _authService = new AuthService();

    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnSendResetClicked(object sender, EventArgs e)
    {
        errorLabel.IsVisible = false;

        var email = emailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(email))
        {
            errorLabel.Text = "Please enter your email.";
            errorLabel.IsVisible = true;
            return;
        }

        try
        {
            await _authService.forgetPassword(email);
            await DisplayAlert("Success", "Reset link sent! Check your email.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            errorLabel.Text = ex.Message;
            errorLabel.IsVisible = true;
        }
    }
}