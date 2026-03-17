using MauiApp1;

namespace LeafBucket.pages.authentication;

public partial class SignUp : ContentPage
{
	public SignUp()
	{
		InitializeComponent();
	}

	private void LoginNavigation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new Login());
	}
}