using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class OrderDetailsPage : ContentPage
{
	public OrderDetailsPage()
	{
		InitializeComponent();
        BindingContext = new OrderDetailsViewModel();
	}
}



