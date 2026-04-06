using LeafBucket.Helpers;
using LeafBucket.Services;
using LeafBucket.ViewModels.Customer;
using LeafBucket.Views.Auth;

namespace LeafBucket.Views.Customer;

public partial class ProfilePage : ContentPage
{
	private readonly AuthService _authService = new AuthService();
	public ProfilePage()
	{
		InitializeComponent();
        BindingContext = new ProfileViewModel();
		
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Console.WriteLine($"UserId: {SessionManager.UserId}");

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
        }
    }

    private async void Logout_Clicked(Object sender, EventArgs e) { 
        SessionManager.UserId = null;
        SessionManager.IdToken = null;

        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}



