using LeafBucket.Helpers;
using LeafBucket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
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













    }
}
