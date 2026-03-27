namespace LeafBucket.Components;


public partial class ProductCard : ContentView
{
    public ProductCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
    BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(ProductCard),
        "Default"
    );

    public static readonly BindableProperty DescriptionProperty =
  BindableProperty.Create(
      nameof(Description),
      typeof(string),
      typeof(ProductCard),
      "Default"
  );

    // Product Type
    public static readonly BindableProperty ProductTypeProperty =
        BindableProperty.Create(nameof(ProductType), typeof(string), typeof(ProductCard), "");

    // Image
    public static readonly BindableProperty ImageUrlProperty =
        BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(ProductCard), "");

    // Location
    public static readonly BindableProperty LocationProperty =
        BindableProperty.Create(nameof(Location), typeof(string), typeof(ProductCard), "");

    // Price
    public static readonly BindableProperty PriceProperty =
        BindableProperty.Create(nameof(Price), typeof(string), typeof(ProductCard), "");


    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

   

    // Properties
    public string ProductType
    {
        get => (string)GetValue(ProductTypeProperty);
        set => SetValue(ProductTypeProperty, value);
    }

    public string ImageUrl
    {
        get => (string)GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    public string Location
    {
        get => (string)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    public string Price
    {
        get => (string)GetValue(PriceProperty);
        set => SetValue(PriceProperty, value);
    }
}



