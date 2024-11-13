using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }  // Primary Key
        public int ProductId { get; set; }  // Foreign Key to Product model
        public int Quantity { get; set; }  // Stores the quantity of the item in the cart
        public string CartId { get; set; }  // Unique identifier for the cart (session-based or user-based)

        public DateTime DateCreated { get; set; }  // Helps track when the cart item was added

        // Navigation property to the related product
        public virtual Product Product { get; set; }

        // Image path (optional, based on how you want to display the item in the cart)
        public string ImagePath { get; set; }
    }
}