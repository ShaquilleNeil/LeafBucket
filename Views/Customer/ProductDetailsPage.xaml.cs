using LeafBucket.ViewModels.Customer;

namespace LeafBucket.Views.Customer;

public partial class ProductDetailsPage : ContentPage
{
	public ProductDetailsPage()
	{
		InitializeComponent();
        BindingContext = new ProductDetailsViewModel();
	}
}


