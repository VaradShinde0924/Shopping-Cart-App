using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }  // Foreign key to User

        public virtual ApplicationUser User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Processing"; // Status property

        public bool IsCancelled { get; set; } = false;
        public virtual ICollection<OrderItem> OrderItems { get; set; }  // Navigation property
        
    }
}