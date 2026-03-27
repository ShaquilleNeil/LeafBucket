using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class AddProductPage : ContentPage
{
	public AddProductPage()
	{
		InitializeComponent();
        BindingContext = new AddProductViewModel();
	}
}



