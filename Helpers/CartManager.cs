using AndroidX.AppCompat.View.Menu;
using LeafBucket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeafBucket.Models;

namespace LeafBucket.Helpers
{
    public class CartManager
    {
        private static List<CartItem> _cartItems = new List<CartItem>();


        public static void AddItem(CartItem cartItem) {
            var existing = _cartItems.FirstOrDefault(i => i.productId == cartItem.productId);

            if (existing != null) {
                if(existing.quantity < existing.stockQuantity)
                    existing.quantity ++;
            } else {
                _cartItems.Add(cartItem);
            }
        }

        public static void RemoveItem(string productId) {
            var existing = _cartItems.FirstOrDefault(i => i.productId == productId);
            if (existing != null) {
                _cartItems.Remove(existing);
            }
        }

        public static void UpdateQuantity(string productId, int quantity) {
            var existing = _cartItems.FirstOrDefault(i => i.productId == productId);

            if (existing != null) {
                if (quantity <= existing.stockQuantity)
                    existing.quantity = quantity;
                
            }

        }

        public static void ClearCart() {
            _cartItems.Clear();
        }

        public static double GetTotal() {
           return _cartItems.Sum(i => i.price * i.quantity);
        }

        public static List<CartItem> GetCartItems() {
            return _cartItems;
        }
    }
}
