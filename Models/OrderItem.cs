using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; } // Primary key

        public int OrdetId { get; set; }
        public int ProductId { get; set; } // Foreign key to Product
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; } // Navigation property




    }
}