using LeafBucket.ViewModels.Customer;

namespace LeafBucket.Views.Customer;

public partial class CartPage : ContentPage
{
	public CartPage()
	{
		InitializeComponent();
        BindingContext = new CartViewModel();
	}
}


