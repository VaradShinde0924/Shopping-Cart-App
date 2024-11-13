using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public List<BestSellingProductViewModel> BestSellingProducts { get; set; }
        public List<ActiveUserViewModel> ActiveUsers { get; set; }
    }
}