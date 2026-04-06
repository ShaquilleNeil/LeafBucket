using LeafBucket.ViewModels.Customer;
using LeafBucket.Services;
using LeafBucket.Models;




namespace LeafBucket.Views.Customer
{
public partial class HomePage : ContentPage
{

        private List<Product> _allProducts = new List<Product>();
        ProductService productService = new ProductService();
        public HomePage()
	    {
		InitializeComponent();
        
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchBar = (SearchBar)sender;
            var searchText = searchBar.Text?.Trim().ToLower() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(searchText)) {
                    ProductsCollection.ItemsSource = _allProducts;
                return;
            }

            var filtered = _allProducts.Where(p => p.name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
            ProductsCollection.ItemsSource = filtered;


        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                _allProducts = await productService.fetchAllProducts();
                ProductsCollection.ItemsSource = _allProducts;
            }
            catch(Exception ex) {
               await DisplayAlert("Error", "Failed to load products: " + ex.Message, "OK");
            }
           
        }


        private async void ProductSelected(object sender, SelectionChangedEventArgs e) {
            if (e.CurrentSelection.FirstOrDefault() is Product selected) {
                await Navigation.PushAsync(new ProductDetailsPage(selected));
                ProductsCollection.SelectedItem = null;
            }
        }

        private async void openCart(object sender, EventArgs e) {
            await Navigation.PushAsync(new CartPage());
        }


    }


}



