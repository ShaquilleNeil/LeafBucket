using LeafBucket.Services;
using LeafBucket.Models;

namespace LeafBucket.Views.Farmer;

public partial class ProductsPage : ContentPage
{
	ProductService productService = new ProductService();
	List<Product> _allProducts = new List<Product>();

    public ProductsPage()
	{
		InitializeComponent();
        
	}

	public void SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
	
		var SearchText = e.NewTextValue.ToLower();
		if (string.IsNullOrWhiteSpace(SearchText)) {
				FarmerProductsCollection.ItemsSource = _allProducts;
			return;
        }

		var filteredProducts = _allProducts.Where(p => p.name.ToLower().Contains(SearchText)).ToList();
		FarmerProductsCollection.ItemsSource = filteredProducts;
    }


    protected override async void OnAppearing()
	{
		base.OnAppearing();
		try {
            _allProducts = await productService.fetchFarmerProducts();
            FarmerProductsCollection.ItemsSource = _allProducts;
        } catch (Exception ex) { 
		   await DisplayAlert("Error", $"Failed to load products: {ex.Message}", "OK");
        }
		

    }

	public void AddProduct_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new AddProductPage());
    }


}


