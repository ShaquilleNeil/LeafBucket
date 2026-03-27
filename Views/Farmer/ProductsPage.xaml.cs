using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class ProductsPage : ContentPage
{
	public ProductsPage()
	{
		InitializeComponent();
        BindingContext = new ProductsViewModel();
	}
}


