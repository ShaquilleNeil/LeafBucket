using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class EditProductPage : ContentPage
{
	public EditProductPage()
	{
		InitializeComponent();
        BindingContext = new EditProductViewModel();
	}
}



