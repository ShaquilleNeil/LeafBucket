using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class FarmerOrderDetailsPage : ContentPage
{
	public FarmerOrderDetailsPage()
	{
		InitializeComponent();
        BindingContext = new OrderDetailsViewModel();
	}
}



