using LeafBucket.ViewModels.Customer;

namespace LeafBucket.Views.Customer;

public partial class CheckoutPage : ContentPage
{
	public CheckoutPage()
	{
		InitializeComponent();
        BindingContext = new CheckoutViewModel();
	}
}


