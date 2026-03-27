using LeafBucket.ViewModels.Customer;

namespace LeafBucket.Views.Customer;


public partial class MapPage : ContentPage
{
	public MapPage()
	{
		InitializeComponent();
        BindingContext = new MapViewModel();
	}
}




