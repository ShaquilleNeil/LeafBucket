using LeafBucket.Helpers;
using LeafBucket.Models;
using LeafBucket.Services;

namespace LeafBucket.Views.Customer;

[QueryProperty(nameof(FarmerId), "FarmerId")]
public partial class FarmerPublicPage : ContentPage
{
    private readonly ProductService _productService = new();
    private readonly AuthService _authService = new();

    private string _farmerId;

    public string FarmerId
    {
        get => _farmerId;
        set
        {
            _farmerId = value;
            LoadPage(_farmerId);
        }
    }

    public FarmerPublicPage()
    {
        InitializeComponent();
    }

    private async void LoadPage(string farmerId)
    {
        try
        {
           
            var farmer = await _authService.fetchUser(farmerId, SessionManager.IdToken);
            if (farmer != null)
            {
                farmerNameLabel.Text = $"{farmer.firstName} {farmer.lastName}";
                farmNameLabel.Text = farmer.farmName ?? "";
                locationLabel.Text = farmer.address ?? "";

                if (!string.IsNullOrEmpty(farmer.profilePhoto))
                    farmerPhoto.Source = ImageSource.FromUri(new Uri(farmer.profilePhoto));
            }

            
            var products = await _productService.fetchProductsByFarmerID(farmerId);

            if (products == null || products.Count == 0)
            {
                emptyState.IsVisible = true;
                productsCollection.IsVisible = false;
                return;
            }

            emptyState.IsVisible = false;
            productsCollection.IsVisible = true;
            productsCollection.ItemsSource = products;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FARMER PUBLIC ERROR: {ex.Message}");
            await DisplayAlert("Error", ex.Message, "OK");
        }



    }


    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product product)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Navigation.PushAsync(new ProductDetailsPage(product));
        }
    }
}