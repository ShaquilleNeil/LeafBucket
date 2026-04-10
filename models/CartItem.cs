using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafBucket.Models
{
    public class CartItem
    {
        public string productId { get; set; }
        public string name { get; set; }
        public string farmerId { get; set; }
        public string imageUrl { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public string unit { get; set; }
        public int stockQuantity { get; set; }
    }
}
