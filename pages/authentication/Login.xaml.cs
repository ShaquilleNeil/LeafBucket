using MauiApp1;

namespace LeafBucket.pages.authentication;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}


	private void CreateAccountNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new SignUp());
	}
}