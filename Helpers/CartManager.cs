using LeafBucket.Models;
using LeafBucket.Helpers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LeafBucket.Helpers
{
    public class CartManager
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private const string projectId = "leafbucket-ed79a";
        private static readonly HttpClient _httpClient = new HttpClient();
        private static List<CartItem> _cartItems = new List<CartItem>();

        public static async void AddItem(CartItem cartItem)
        {
            var existing = _cartItems.FirstOrDefault(i => i.productId == cartItem.productId);
            if (existing != null)
            {
                if (existing.quantity < existing.stockQuantity)
                    existing.quantity++;
            }
            else
            {
                _cartItems.Add(cartItem);
            }
            await SyncCartToFirestore();
        }

        public static async void RemoveItem(string productId)
        {
            var existing = _cartItems.FirstOrDefault(i => i.productId == productId);
            if (existing != null)
            {
                _cartItems.Remove(existing);
                await RemoveCartItemFromFirestore(productId);
            }
        }

        public static void UpdateQuantity(string productId, int quantity)
        {
            var existing = _cartItems.FirstOrDefault(i => i.productId == productId);
            if (existing != null)
            {
                if (quantity <= existing.stockQuantity)
                    existing.quantity = quantity;
            }
        }

        public static async void ClearCart()
        {
            await ClearCartFromFirestore();
            _cartItems.Clear();
        }

        public static double GetTotal()
        {
            return _cartItems.Sum(i => i.price * i.quantity);
        }

        public static List<CartItem> GetCartItems()
        {
            return _cartItems;
        }

        public static async Task SyncCartToFirestore()
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;
            if (string.IsNullOrEmpty(userId)) return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            foreach (var item in _cartItems)
            {
                var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{userId}/cart/{item.productId}?key={ApiKey}";

                var body = new
                {
                    fields = new
                    {
                        productId = new { stringValue = item.productId ?? "" },
                        name = new { stringValue = item.name ?? "" },
                        price = new { doubleValue = item.price },
                        quantity = new { integerValue = item.quantity.ToString() },
                        imageUrl = new { stringValue = item.imageUrl ?? "" },
                        unit = new { stringValue = item.unit ?? "" },
                        stockQuantity = new { integerValue = item.stockQuantity.ToString() },
                        farmerId = new { stringValue = item.farmerId ?? "" }
                    }
                };

                await _httpClient.PatchAsJsonAsync(url, body);
            }
        }

        public static async Task RemoveCartItemFromFirestore(string productId)
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;
            if (string.IsNullOrEmpty(userId)) return;

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{userId}/cart/{productId}?key={ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);
            await _httpClient.SendAsync(request);
        }

        public static async Task ClearCartFromFirestore()
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;
            if (string.IsNullOrEmpty(userId)) return;

            var itemsToDelete = _cartItems.ToList();
            foreach (var item in itemsToDelete)
                await RemoveCartItemFromFirestore(item.productId);
        }

        public static async Task FetchCartFromFirestore()
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;
            if (string.IsNullOrEmpty(userId)) return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/users/{userId}/cart?key={ApiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var items = new List<CartItem>();

            if (!json.RootElement.TryGetProperty("documents", out var documents))
                return;

            foreach (var doc in documents.EnumerateArray())
            {
                if (!doc.TryGetProperty("fields", out var fields)) continue;

                items.Add(new CartItem
                {
                    productId = fields.GetProperty("productId").GetProperty("stringValue").GetString(),
                    name = fields.GetProperty("name").GetProperty("stringValue").GetString(),
                    price = fields.GetProperty("price").GetProperty("doubleValue").GetDouble(),
                    quantity = int.Parse(fields.GetProperty("quantity").GetProperty("integerValue").GetString() ?? "1"),
                    imageUrl = fields.GetProperty("imageUrl").GetProperty("stringValue").GetString(),
                    unit = fields.GetProperty("unit").GetProperty("stringValue").GetString(),
                    stockQuantity = int.Parse(fields.GetProperty("stockQuantity").GetProperty("integerValue").GetString() ?? "0"),
                    farmerId = fields.GetProperty("farmerId").GetProperty("stringValue").GetString()
                });
            }

            _cartItems = items;
        }
    }
}