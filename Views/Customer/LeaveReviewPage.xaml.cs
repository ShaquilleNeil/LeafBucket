using LeafBucket.Helpers;
using LeafBucket.Models;
using LeafBucket.Services;

namespace LeafBucket.Views.Customer;

public partial class LeaveReviewPage : ContentPage
{
    private readonly Product _product;
    private readonly ReviewService _reviewService = new();
    private int _selectedRating = 0;

    private readonly Color activeColor = Color.FromArgb("#FFB300");
    private readonly Color inactiveColor = Color.FromArgb("#E0E0E0");

    public LeaveReviewPage(Product product)
    {
        InitializeComponent();
        _product = product;
        productNameLabel.Text = product.name;
    }

    private void OnStarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter == null) return;

        _selectedRating = int.Parse(button.CommandParameter.ToString()!);
        UpdateStars(_selectedRating);
    }

    private void UpdateStars(int rating)
    {
        star1.TextColor = rating >= 1 ? activeColor : inactiveColor;
        star2.TextColor = rating >= 2 ? activeColor : inactiveColor;
        star3.TextColor = rating >= 3 ? activeColor : inactiveColor;
        star4.TextColor = rating >= 4 ? activeColor : inactiveColor;
        star5.TextColor = rating >= 5 ? activeColor : inactiveColor;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (_selectedRating == 0)
        {
            await DisplayAlert("Error", "Please select a rating.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(commentEntry.Text))
        {
            await DisplayAlert("Error", "Please write a comment.", "OK");
            return;
        }

        try
        {
            var review = new Review
            {
                reviewId = Guid.NewGuid().ToString(),
                productId = _product.productId,
                productName = _product.name,
                farmerId = _product.farmerId,
                customerId = SessionManager.UserId,
                customerName = SessionManager.UserName,
                rating = _selectedRating,
                comment = commentEntry.Text,
                farmerReply = "",
                photos = new List<string?>(),
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow,
                orderId = ""
            };

            await _reviewService.addReview(review);
            await DisplayAlert("Success", "Review submitted!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}