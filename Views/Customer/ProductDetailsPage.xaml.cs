using LeafBucket.ViewModels.Customer;
using LeafBucket.Models;
using LeafBucket.Services;
using LeafBucket.Helpers;

namespace LeafBucket.Views.Customer;

public partial class ProductDetailsPage : ContentPage
{
    private Product _product;
    private int _quantity = 1;
    private readonly ReviewService _reviewService = new();

    public ProductDetailsPage(Product product)
    {
        InitializeComponent();
        _product = product;

        if (!string.IsNullOrEmpty(product.imageUrl))
            productImage.Source = ImageSource.FromUri(new Uri(product.imageUrl));

        productName.Text = product.name;
        productPrice.Text = $"${product.price:F2}";
        productDescription.Text = product.description;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(500);
        await LoadReviews();
        await CheckIfCanReview();
    }

    private async Task LoadReviews()
    {
        try
        {
            var reviews = await _reviewService.fetchProductReviews(_product.productId!);

            reviewsStack.Children.Clear();

            if (reviews == null || reviews.Count == 0)
            {
                noReviewsLabel.IsVisible = true;
                ratingStack.IsVisible = false;
                return;
            }

            noReviewsLabel.IsVisible = false;
            ratingStack.IsVisible = true;

            
            var avg = reviews.Average(r => r.rating);
            avgRatingLabel.Text = avg.ToString("0.0");
            starsLabel.Text = new string('★', (int)Math.Round(avg)) +
                              new string('☆', 5 - (int)Math.Round(avg));
            reviewCountLabel.Text = $"{reviews.Count} review{(reviews.Count > 1 ? "s" : "")}";

           
            foreach (var review in reviews.OrderByDescending(r => r.createdAt))
            {
                var card = new Frame
                {
                    Padding = new Thickness(16),
                    CornerRadius = 12,
                    HasShadow = false,
                    BackgroundColor = Colors.White,
                    BorderColor = Color.FromArgb("#E0E0E0")
                };

                var stack = new StackLayout { Spacing = 6 };

                stack.Children.Add(new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Children =
                    {
                        new Label
                        {
                            Text = review.customerName ?? "Customer",
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 14,
                            TextColor = Colors.Black
                        },
                        new Label
                        {
                            Text = review.FormattedDate,
                            FontSize = 12,
                            TextColor = Color.FromArgb("#888"),
                            HorizontalOptions = LayoutOptions.End
                        }
                    }
                });

                stack.Children.Add(new Label
                {
                    Text = review.StarDisplay,
                    FontSize = 16,
                    TextColor = Color.FromArgb("#FFB300")
                });

                stack.Children.Add(new Label
                {
                    Text = review.comment,
                    FontSize = 13,
                    TextColor = Colors.Black
                });

               
                if (!string.IsNullOrEmpty(review.farmerReply))
                {
                    var replyFrame = new Frame
                    {
                        Padding = new Thickness(12),
                        CornerRadius = 8,
                        HasShadow = false,
                        BackgroundColor = Color.FromArgb("#F1F8E9"),
                        BorderColor = Color.FromArgb("#C8E6C9"),
                        Margin = new Thickness(0, 4, 0, 0)
                    };
                    var replyStack = new StackLayout { Spacing = 2 };
                    replyStack.Children.Add(new Label
                    {
                        Text = "Farmer's reply",
                        FontSize = 11,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#2E7D32")
                    });
                    replyStack.Children.Add(new Label
                    {
                        Text = review.farmerReply,
                        FontSize = 13,
                        TextColor = Colors.Black
                    });
                    replyFrame.Content = replyStack;
                    stack.Children.Add(replyFrame);
                }

                card.Content = stack;
                reviewsStack.Children.Add(card);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"REVIEWS ERROR: {ex.Message}");
        }
    }

    private async Task CheckIfCanReview()
    {
        try
        {
            var hasOrdered = await _reviewService.hasCustomerOrderedProduct(_product.productId!);
            var hasReviewed = (await _reviewService.fetchProductReviews(_product.productId!))
                .Any(r => r.customerId == SessionManager.UserId);

            leaveReviewButton.IsVisible = hasOrdered && !hasReviewed;
        }
        catch
        {
            leaveReviewButton.IsVisible = false;
        }
    }

    private async void OnLeaveReviewClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LeaveReviewPage(_product));
    }

    private void Increase_Clicked(object sender, EventArgs e)
    {
        if (_quantity < _product.stockQuantity)
        {
            _quantity++;
            quantityLabel.Text = _quantity.ToString();
        }
    }

    private void Decrease_Clicked(object sender, EventArgs e)
    {
        if (_quantity > 1)
        {
            _quantity--;
            quantityLabel.Text = _quantity.ToString();
        }
    }

    private async void AddToCart_Clicked(object sender, EventArgs e)
    {
        var cartItem = new CartItem
        {
            productId = _product.productId,
            name = _product.name,
            price = _product.price,
            quantity = _quantity,
            imageUrl = _product.imageUrl,
            unit = _product.unit,
            stockQuantity = _product.stockQuantity,
            farmerId = _product.farmerId
        };
        CartManager.AddItem(cartItem);
        await DisplayAlert("Success", "Product added to cart!", "OK");
        await Navigation.PopAsync();
    }
}