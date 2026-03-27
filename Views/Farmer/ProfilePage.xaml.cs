using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
        BindingContext = new ProfileViewModel();
	}
}


