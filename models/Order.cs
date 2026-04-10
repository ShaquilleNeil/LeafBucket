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


    }
}
