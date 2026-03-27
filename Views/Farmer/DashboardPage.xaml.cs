using LeafBucket.ViewModels.Farmer;

namespace LeafBucket.Views.Farmer;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
        BindingContext = new DashboardViewModel();
	}
}


