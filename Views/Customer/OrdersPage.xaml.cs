using LeafBucket.ViewModels.Customer;

namespace LeafBucket.Views.Customer;

public partial class OrdersPage : ContentPage
{
	public OrdersPage()
	{
		InitializeComponent();
        BindingContext = new OrdersViewModel();
	}
}


