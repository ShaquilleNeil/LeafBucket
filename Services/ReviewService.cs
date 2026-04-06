using LeafBucket.Helpers;
using LeafBucket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeafBucket.Services
{
    public class ReviewService
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private readonly HttpClient _httpClient;
        private const string projectId = "leafbucket-ed79a";

        public ReviewService()
        {
            _httpClient = new HttpClient();
        }


        public async Task addReview(Review review) {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/reviews";


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                fields = new
                {
                    reviewId = new { stringValue = review.reviewId },
                    customerId = new { stringValue = userId },
                    farmerId = new { stringValue = review.farmerId },
                    productId = new { stringValue = review.productId },
                    orderId = new { stringValue = review.orderId },
                    rating = new { integerValue = review.rating },
                    comment = new { stringValue = review.comment },
                    photos = new
                    {
                        arrayValue = new
                        {
                            values = review.photos.Select(photo => new { stringValue = photo }).ToArray()
                        }
                    },
                    farmerReply = new { stringValue = review.farmerReply ?? "" },
                    createdAt = new { timestampValue = DateTime.UtcNow.ToString("o") },
                    updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };




            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error posting review: {errorContent}");
            }
        }


        public async Task<List<Review>> fetchReviews(string farmerId)
        {
            var idToken = SessionManager.IdToken;

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery?key={ApiKey}";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                structuredQuery = new
                {
                    from = new[]
                    {
                        new { collectionId = "reviews" }
                    },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "farmerId" },
                            op = "EQUAL",
                            value = new { stringValue = farmerId }
                        }
                    },
                    orderBy = new[]
                    {
                        new
                        {
                            field = new { fieldPath = "createdAt" },
                            direction = "DESCENDING"
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, body);

            if (!response.IsSuccessStatusCode) { 
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching reviews: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var reviews = new List<Review>();

            foreach (var item in json.RootElement.EnumerateArray()) {
                if (item.TryGetProperty("document", out var document)) { 
                    
                    var fields = document.GetProperty("fields");
                    var review = new Review
                    {
                        reviewId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                        customerId = fields.GetProperty("customerId").GetProperty("stringValue").GetString(),
                        farmerId = fields.GetProperty("farmerId").GetProperty("stringValue").GetString(),
                        productId = fields.GetProperty("productId").GetProperty("stringValue").GetString(),
                        orderId = fields.GetProperty("orderId").GetProperty("stringValue").GetString(),
                        rating = fields.GetProperty("rating").GetProperty("integerValue").GetInt32(),
                        comment = fields.GetProperty("comment").GetProperty("stringValue").GetString(),
                        photos = fields.GetProperty("photos").GetProperty("arrayValue").GetProperty("values")
                            .EnumerateArray()
                            .Select(photo => photo.GetProperty("stringValue").GetString())
                            .ToList(),
                        farmerReply = fields.GetProperty("farmerReply").GetProperty("stringValue").GetString(),
                        createdAt = DateTime.Parse(fields.GetProperty("createdAt").GetProperty("timestampValue").GetString()),
                        updatedAt = DateTime.Parse(fields.GetProperty("updatedAt").GetProperty("timestampValue").GetString())
                    };
                    reviews.Add(review);
                }
            }
            return reviews;

        }



        public async Task replyToReview(string reviewId, string farmerReply) {
            var idToken = SessionManager.IdToken;
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/reviews/{reviewId}?key={ApiKey}&updateMask.fieldPaths=farmerReply&updateMask.fieldPaths=updatedAt";


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                fields = new
                {
                    farmerReply = new { stringValue = farmerReply },
                    updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };

            var response = await _httpClient.PatchAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error replying to review: {errorContent}");
            }

        }






    }
}
