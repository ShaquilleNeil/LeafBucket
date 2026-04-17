using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafBucket.Models
{
    public class Order
    {
        public string orderId { get; set; }

        public string customerId { get; set; }

        public List<string> farmerIds { get; set; }

        public List<OrderItem> items { get; set; }

        public double subtotal { get; set; }

        public double deliveryFee { get; set; }

        public double tax { get; set; }

        public double total { get; set; }

        public string status { get; set; }
        public string shippingAddress { get; set; }

        public string paymentMethod { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }

        public string ShortOrderId => orderId?.Length >= 8
    ? $"Order #{orderId[..8].ToUpper()}"
    : $"Order #{orderId?.ToUpper() ?? "??????"}";

        public string FormattedDate => createdAt != DateTime.MinValue
            ? createdAt.ToString("MMM d, yyyy")
            : "Date unavailable";

        public string ItemSummary => items != null
            ? $"{items.Count} item{(items.Count > 1 ? "s" : "")} · ${total:0.00}"
            : "0 items";

        public string StatusColor => status switch
        {
            "Placed" => "#1565C0",
            "Preparing" => "#E65100",
            "Ready" => "#558B2F",
            "Completed" => "#757575",
            "Cancelled" => "#C62828",
            _ => "#9E9E9E"
        };
    }
}
