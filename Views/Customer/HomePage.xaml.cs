using LeafBucket.ViewModels.Customer;





namespace LeafBucket.Views.Customer
{
public partial class HomePage : ContentPage
{
       
	public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomeViewModel();
	}

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchBar = (SearchBar)sender;

            //searchResults.ItemsSource = DataService.GetSearchResults(searchBar.Text);

        }
    }


}



