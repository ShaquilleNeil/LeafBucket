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
                    customerName = new { stringValue = review.customerName ?? "" },
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

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");

                    string SafeGet(string key)
                    {
                        if (fields.TryGetProperty(key, out var prop))
                            if (prop.TryGetProperty("stringValue", out var val))
                                return val.GetString() ?? "";
                        return "";
                    }

                    int SafeGetInt(string key)
                    {
                        if (fields.TryGetProperty(key, out var prop))
                            if (prop.TryGetProperty("integerValue", out var val))
                                return int.Parse(val.GetString() ?? "0");
                        return 0;
                    }

                    var review = new Review
                    {
                        reviewId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                        customerId = SafeGet("customerId"),
                        farmerId = SafeGet("farmerId"),
                        productId = SafeGet("productId"),
                        orderId = SafeGet("orderId"),
                        customerName = SafeGet("customerName"),
                        rating = SafeGetInt("rating"),
                        comment = SafeGet("comment"),
                        farmerReply = SafeGet("farmerReply"),
                        photos = fields.TryGetProperty("photos", out var photoProp) &&
                             photoProp.TryGetProperty("arrayValue", out var arr) &&
                             arr.TryGetProperty("values", out var vals)
                        ? vals.EnumerateArray()
                            .Select(p => p.GetProperty("stringValue").GetString())
                            .ToList()
                        : new List<string?>(),
                        createdAt = DateTime.TryParse(
                            fields.TryGetProperty("createdAt", out var d)
                                ? d.GetProperty("timestampValue").GetString() : "",
                            out var parsed) ? parsed : DateTime.UtcNow,
                        updatedAt = DateTime.TryParse(
                            fields.TryGetProperty("updatedAt", out var u)
                                ? u.GetProperty("timestampValue").GetString() : "",
                            out var parsedU) ? parsedU : DateTime.UtcNow
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


        public async Task<List<Review>> fetchProductReviews(string productId)
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
                    from = new[] { new { collectionId = "reviews" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "productId" },
                            op = "EQUAL",
                            value = new { stringValue = productId }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode) return new List<Review>();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var reviews = new List<Review>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("document", out var document)) continue;
                if (!document.TryGetProperty("fields", out var fields)) continue;

                reviews.Add(new Review
                {
                    reviewId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                    customerId = fields.GetProperty("customerId").GetProperty("stringValue").GetString(),
                    farmerId = fields.GetProperty("farmerId").GetProperty("stringValue").GetString(),
                    productId = fields.GetProperty("productId").GetProperty("stringValue").GetString(),
                    orderId = fields.GetProperty("orderId").GetProperty("stringValue").GetString(),
                    rating = int.Parse(fields.GetProperty("rating")
    .GetProperty("integerValue").GetString() ?? "0"),
                    comment = fields.GetProperty("comment").GetProperty("stringValue").GetString(),
                    farmerReply = fields.GetProperty("farmerReply").GetProperty("stringValue").GetString(),
                    customerName = fields.TryGetProperty("customerName", out var cn)
                        ? cn.GetProperty("stringValue").GetString() : "",
                    createdAt = DateTime.Parse(fields.GetProperty("createdAt")
                        .GetProperty("timestampValue").GetString() ?? "")
                });
            }

            return reviews;
        }

        public async Task<bool> hasCustomerOrderedProduct(string productId)
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
                    from = new[] { new { collectionId = "orders" } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "customerId" },
                            op = "EQUAL",
                            value = new { stringValue = SessionManager.UserId }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("document", out var document)) continue;
                if (!document.TryGetProperty("fields", out var fields)) continue;

                if (!fields.TryGetProperty("items", out var itemsField)) continue;

                var orderItems = itemsField.GetProperty("arrayValue")
                    .GetProperty("values")
                    .EnumerateArray();

                foreach (var orderItem in orderItems)
                {
                    var pid = orderItem.GetProperty("mapValue")
                        .GetProperty("fields")
                        .GetProperty("productId")
                        .GetProperty("stringValue")
                        .GetString();

                    if (pid == productId) return true;
                }
            }

            return false;
        }

    }
}
