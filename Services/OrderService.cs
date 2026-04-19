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
    public class OrderService
    {
        private const string ApiKey = "AIzaSyBAOYgdYOimcRj5s4492INJBfes5dd72Sc";
        private readonly HttpClient _httpClient;
        private const string projectId = "leafbucket-ed79a";

        public OrderService()
        {
            _httpClient = new HttpClient();
        }


        public async Task placeOrder(Order order)
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;

            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/orders";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
            {
                fields = new
                {
                    orderId = new { stringValue = order.orderId },
                    customerId = new { stringValue = userId },
                    farmerId = new
                    {
                        arrayValue = new
                        {
                            values = order.farmerIds.Select(id => new { stringValue = id }).ToArray()
                        }
                    },
                    items = new
                    {
                        arrayValue = new
                        {
                            values = order.items.Select(item => new
                            {
                                mapValue = new
                                {
                                    fields = new
                                    {
                                        productId = new { stringValue = item.productId },
                                        name = new { stringValue = item.name },
                                        farmerId = new { stringValue = item.farmerId },
                                        quantity = new { integerValue = item.quantity.ToString() },
                                        price = new { doubleValue = item.price }
                                    }
                                }
                            }).ToArray()
                        }
                    },
                    subtotal = new { doubleValue = order.subtotal },
                    deliveryFee = new { doubleValue = order.deliveryFee },
                    tax = new { doubleValue = order.tax },
                    total = new { doubleValue = order.total },
                    status = new { stringValue = "Placed" },
                    shippingAddress = new { stringValue = order.shippingAddress },
                    paymentMethod = new { stringValue = order.paymentMethod },
                    createdAt = new { timestampValue = DateTime.UtcNow.ToString("o") },
                    updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };

            var bodyJson = System.Text.Json.JsonSerializer.Serialize(body);
            Console.WriteLine($"Request body: {bodyJson}");

            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error placing order: {errorContent}");
            }
        }

        public async Task updateOrderStatus(string orderId, string status) {
            var idToken = SessionManager.IdToken;
            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/orders/{orderId}?key={ApiKey}&updateMask.fieldPaths=status&updateMask.fieldPaths=updatedAt";


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

            var body = new
                {
                fields = new
                {
                    status = new { stringValue = status },
                    updatedAt = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };

            var response = await _httpClient.PatchAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error updating order: {errorContent}");
            }


        }


        public async Task<List<Order>> fetchFarmerOrders() {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;



            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";

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
                            field = new { fieldPath = "farmerId" },
                            op = "ARRAY_CONTAINS",
                            value = new { stringValue = userId }
                        }
                    }
                }
            };


            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching orders: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var orders = new List<Order>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");
                    var order = new Order
                    {
                        orderId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                        farmerIds = fields.GetProperty("farmerId")
                                    .GetProperty("arrayValue")
                                    .GetProperty("values")
                                    .EnumerateArray()
                                    .Select(f => f.GetProperty("stringValue").GetString())
                                    .ToList(),
                        customerId = fields.GetProperty("customerId").GetProperty("stringValue").GetString(),
                        shippingAddress = fields.GetProperty("shippingAddress").GetProperty("stringValue").GetString(),
                        paymentMethod = fields.GetProperty("paymentMethod").GetProperty("stringValue").GetString(),
                        status = fields.GetProperty("status").GetProperty("stringValue").GetString(),
                        subtotal = ParseDouble(fields.GetProperty("subtotal")),
                        deliveryFee = ParseDouble(fields.GetProperty("deliveryFee")),
                        tax = ParseDouble(fields.GetProperty("tax")),
                        total = ParseDouble(fields.GetProperty("total")),
                        createdAt = DateTime.TryParse(
        fields.GetProperty("createdAt").GetProperty("timestampValue").GetString(),
        out var parsedDate) ? parsedDate : DateTime.UtcNow,
                        items = fields.GetProperty("items")
    .GetProperty("arrayValue")
    .GetProperty("values")
    .EnumerateArray()
    .Select(item => new OrderItem
    {
        productId = item.GetProperty("mapValue").GetProperty("fields")
                        .GetProperty("productId").GetProperty("stringValue").GetString(),
        name = item.GetProperty("mapValue").GetProperty("fields")
                    .GetProperty("name").GetProperty("stringValue").GetString(),
        farmerId = item.GetProperty("mapValue").GetProperty("fields")
                       .GetProperty("farmerId").GetProperty("stringValue").GetString(),
        quantity = int.Parse(item.GetProperty("mapValue").GetProperty("fields")
                        .GetProperty("quantity").GetProperty("integerValue")
                        .GetString() ?? "0"),
        price = item.GetProperty("mapValue").GetProperty("fields")
                    .GetProperty("price").GetProperty("doubleValue").GetDouble()
    }).ToList()
                    };
                    orders.Add(order);
                }
            }

            return orders;
        }


        public async Task<List<Order>> fetchCustomerOrders()
        {
            var userId = SessionManager.UserId;
            var idToken = SessionManager.IdToken;



            var url = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents:runQuery";

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
                            value = new { stringValue = userId }
                        }
                    }
                }
            };


            var response = await _httpClient.PostAsJsonAsync($"{url}?key={ApiKey}", body);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error fetching orders: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var orders = new List<Order>();

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("document", out var document))
                {
                    var fields = document.GetProperty("fields");
                    var order = new Order
                    {
                        orderId = document.GetProperty("name").GetString()?.Split('/').Last() ?? "",
                        farmerIds = fields.GetProperty("farmerId")
                                    .GetProperty("arrayValue")
                                    .GetProperty("values")
                                    .EnumerateArray()
                                    .Select(f => f.GetProperty("stringValue").GetString())
                                    .ToList(),
                        customerId = fields.GetProperty("customerId").GetProperty("stringValue").GetString(),
                        shippingAddress = fields.GetProperty("shippingAddress").GetProperty("stringValue").GetString(),
                        paymentMethod = fields.GetProperty("paymentMethod").GetProperty("stringValue").GetString(),
                        status = fields.GetProperty("status").GetProperty("stringValue").GetString(),
                        subtotal = ParseDouble(fields.GetProperty("subtotal")),
                        deliveryFee = ParseDouble(fields.GetProperty("deliveryFee")),
                        tax = ParseDouble(fields.GetProperty("tax")),
                        total = ParseDouble(fields.GetProperty("total")),
                        createdAt = DateTime.TryParse(
        fields.GetProperty("createdAt").GetProperty("timestampValue").GetString(),
        out var parsedDate) ? parsedDate : DateTime.UtcNow,
                        items = fields.GetProperty("items")
    .GetProperty("arrayValue")
    .GetProperty("values")
    .EnumerateArray()
    .Select(item => new OrderItem
    {
        productId = item.GetProperty("mapValue").GetProperty("fields")
                        .GetProperty("productId").GetProperty("stringValue").GetString(),
        name = item.GetProperty("mapValue").GetProperty("fields")
                    .GetProperty("name").GetProperty("stringValue").GetString(),
        farmerId = item.GetProperty("mapValue").GetProperty("fields")
                       .GetProperty("farmerId").GetProperty("stringValue").GetString(),
        quantity = int.Parse(item.GetProperty("mapValue").GetProperty("fields")
                        .GetProperty("quantity").GetProperty("integerValue")
                        .GetString() ?? "0"),
        price = item.GetProperty("mapValue").GetProperty("fields")
                    .GetProperty("price").GetProperty("doubleValue").GetDouble()
    }).ToList()
                    };
                    orders.Add(order);
                }
            }

            return orders;
        }

        private double ParseDouble(JsonElement element)
        {
            if (element.TryGetProperty("doubleValue", out var d))
                return d.GetDouble();
            if (element.TryGetProperty("integerValue", out var i))
                return double.Parse(i.GetString() ?? "0");
            return 0;
        }


    }
}
