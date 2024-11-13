using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class ActiveUserViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

}