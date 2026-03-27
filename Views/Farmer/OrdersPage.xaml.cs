using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class OrdersPage : ContentPage
{
	public OrdersPage()
	{
		InitializeComponent();
        BindingContext = new OrdersViewModel();
	}
}


