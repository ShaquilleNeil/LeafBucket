using LeafBucket.Models;
using LeafBucket.Services;

namespace LeafBucket.Views.Farmer;

public partial class FarmerReviewPage : ContentPage
{
    private readonly ReviewService _reviewService = new();

    public FarmerReviewPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReviews();
    }

    private async Task LoadReviews()
    {
        try
        {
            var reviews = await _reviewService.fetchReviews(Helpers.SessionManager.UserId!);

            reviewsStack.Children.Clear();

            if (reviews == null || reviews.Count == 0)
            {
                emptyState.IsVisible = true;
                return;
            }

            emptyState.IsVisible = false;

            foreach (var review in reviews.OrderByDescending(r => r.createdAt))
            {
                var card = new Frame
                {
                    Padding = new Thickness(16),
                    CornerRadius = 12,
                    HasShadow = true,
                    BackgroundColor = Colors.White
                };

                var stack = new StackLayout { Spacing = 8 };

               
                var header = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                };
                header.Add(new Label
                {
                    Text = review.customerName ?? "Customer",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    TextColor = Colors.Black
                }, 0, 0);
                header.Add(new Label
                {
                    Text = review.FormattedDate,
                    FontSize = 12,
                    TextColor = Color.FromArgb("#888"),
                    HorizontalOptions = LayoutOptions.End
                }, 1, 0);
                stack.Children.Add(header);

                stack.Children.Add(new Label
                {
                    Text = review.StarDisplay,
                    FontSize = 18,
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
                        BorderColor = Color.FromArgb("#C8E6C9")
                    };
                    var replyStack = new StackLayout { Spacing = 2 };
                    replyStack.Children.Add(new Label
                    {
                        Text = "Your reply",
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
                else
                {
                    var capturedReview = review;
                    var replyButton = new Button
                    {
                        Text = "REPLY",
                        BackgroundColor = Colors.White,
                        TextColor = Color.FromArgb("#2E7D32"),
                        BorderColor = Color.FromArgb("#2E7D32"),
                        BorderWidth = 1.5,
                        CornerRadius = 8,
                        FontSize = 13,
                        FontAttributes = FontAttributes.Bold,
                        Padding = new Thickness(16, 8),
                        HorizontalOptions = LayoutOptions.Start
                    };
                    replyButton.Clicked += async (s, e) =>
                    {
                        var reply = await DisplayPromptAsync(
                            "Reply to Review",
                            "Write your reply:",
                            "Submit",
                            "Cancel",
                            placeholder: "Enter your reply...");

                        if (string.IsNullOrWhiteSpace(reply)) return;

                        try
                        {
                            await _reviewService.replyToReview(capturedReview.reviewId!, reply);
                            await DisplayAlert("Success", "Reply submitted!", "OK");
                            await LoadReviews();
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", ex.Message, "OK");
                        }
                    };
                    stack.Children.Add(replyButton);
                }

                card.Content = stack;
                reviewsStack.Children.Add(card);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load reviews: {ex.Message}", "OK");
        }
    }
}