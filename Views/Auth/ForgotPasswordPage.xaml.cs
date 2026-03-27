using LeafBucket.ViewModels.Auth;

namespace LeafBucket.Views.Auth;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage()
	{
		InitializeComponent();
        BindingContext = new ForgotPasswordViewModel();
	}
}


