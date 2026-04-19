using LeafBucket.Helpers;
using LeafBucket.Services;
using LeafBucket.Views.Auth;

namespace LeafBucket.Views.Customer;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService = new AuthService();

    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var user = await _authService.fetchUser(
            SessionManager.UserId,
            SessionManager.IdToken
        );

        if (user != null)
        {
            customerName.Text = $"{user.firstName} {user.lastName}";
            customerAddress.Text = user.address;
            customerPhone.Text = user.phoneNumber;
            customerEmail.Text = user.email;

            if (!string.IsNullOrEmpty(user.profilePhoto))
                customerPhoto.Source = ImageSource.FromUri(new Uri(user.profilePhoto));
        }
    }

    private void Logout_Clicked(object sender, EventArgs e)
    {
        SessionManager.UserId = null;
        SessionManager.IdToken = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}