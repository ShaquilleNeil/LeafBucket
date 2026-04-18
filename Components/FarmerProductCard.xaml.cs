using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.Views.Farmer;

namespace LeafBucket.Components;

public partial class FarmerProductCard : ContentView
{

    private Product product = new Product();
	public FarmerProductCard()
	{
		InitializeComponent();
	}


    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(FarmerProductCard), "");

    // Product Type
    public static readonly BindableProperty CategoryProperty =
    BindableProperty.Create(nameof(Category), typeof(string), typeof(FarmerProductCard), "");

    // Image
    public static readonly BindableProperty ImageUrlProperty =
        BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(FarmerProductCard), "");

    // Price
    public static readonly BindableProperty PriceProperty =
        BindableProperty.Create(nameof(Price), typeof(string), typeof(FarmerProductCard), "");

    public static readonly BindableProperty StockQuantityProperty =
        BindableProperty.Create(nameof(StockQuantity), typeof(int), typeof(FarmerProductCard), 0);


    public static readonly BindableProperty ProductProperty =
    BindableProperty.Create(nameof(Product), typeof(Product), typeof(FarmerProductCard), default(Product));

    public Product? Product
    {
        get => (Product?)GetValue(ProductProperty);
        set => SetValue(ProductProperty, value);
    }


    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }




    public string Category
    {
        get => (string)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public string ImageUrl
    {
        get => (string)GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    public int StockQuantity
    {
        get => (int)GetValue(StockQuantityProperty);
        set => SetValue(StockQuantityProperty, value);
    }
   

    public string Price
    {
        get => (string)GetValue(PriceProperty);
        set => SetValue(PriceProperty, value);
    }


    public async void updateProduct(object sender, EventArgs e)
    {
        if (Product == null) return;
        await Navigation.PushAsync(new EditProductPage(Product));
    }


    public async void deleteProduct(object sender, EventArgs e)
    {
        if (Product == null) return;
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Delete", $"Delete {Product.name}?", "Yes", "No");
        if (!confirm) return;

        try
        {
            var service = new ProductService();
            await service.deleteProduct(Product);
            MessagingCenter.Send(this, "ProductDeleted");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }


}