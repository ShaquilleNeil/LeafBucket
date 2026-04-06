namespace LeafBucket.Components;

public partial class FarmerProductCard : ContentView
{
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


    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }



    // Properties
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


}