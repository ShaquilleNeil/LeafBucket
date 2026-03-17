using LeafBucket.pages.customer;
using LeafBucket.pages.farmer;
namespace LeafBucket;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		 Routing.RegisterRoute("CustomerHome", typeof(CustomerHome));
         Routing.RegisterRoute("FarmerHome", typeof(FarmerDashboard));
	}
}
