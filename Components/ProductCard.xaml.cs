

using LeafBucket.Models;

namespace LeafBucket.Components;


public partial class ProductCard : ContentView
{
    public ProductCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty NameProperty =
    BindableProperty.Create(nameof(Name), typeof(string), typeof(ProductCard), "");

    public static readonly BindableProperty DescriptionProperty =
  BindableProperty.Create(
      nameof(Description),
      typeof(string),
      typeof(ProductCard),
      "Default"
  );

    // Product Type
    public static readonly BindableProperty CategoryProperty =
    BindableProperty.Create(nameof(Category), typeof(string), typeof(ProductCard), "");

    // Image
    public static readonly BindableProperty ImageUrlProperty =
        BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(ProductCard), "");

    // Location
    public static readonly BindableProperty LocationProperty =
        BindableProperty.Create(nameof(Location), typeof(string), typeof(ProductCard), "");

    // Price
    public static readonly BindableProperty PriceProperty =
        BindableProperty.Create(nameof(Price), typeof(string), typeof(ProductCard), "");


    public static readonly BindableProperty ProductProperty =
        BindableProperty.Create(nameof(Product), typeof(Product), typeof(ProductCard), null);

    public Product Product { get; set; }


    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
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









  