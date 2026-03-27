using LeafBucket.ViewModels.Customer;

namespace LeafBucket.Views.Customer;

public partial class OrderDetailsPage : ContentPage
{
	public OrderDetailsPage()
	{
		InitializeComponent();
        BindingContext = new OrderDetailsViewModel();
	}
}


