using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Dashboard()
        {
            var totalOrders = _context.Orders.Count();
            var totalSales = _context.Orders.Sum(o => o.TotalPrice);

            var bestSellingProducts = _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new BestSellingProductViewModel
                {
                    ProductId = g.Key,
                    ProductName = g.FirstOrDefault().Product.Name,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(5)
                .ToList() ?? new List<BestSellingProductViewModel>();

            var activeUsers = _context.Orders
                .GroupBy(o => o.UserId)
                .Select(g => new ActiveUserViewModel
                {
                    UserId = g.Key,
                    UserEmail = g.FirstOrDefault().User.Email,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalPrice)
                })
                .OrderByDescending(u => u.TotalOrders)
                .Take(5)
                .ToList() ?? new List<ActiveUserViewModel>();

            var viewModel = new DashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                BestSellingProducts = bestSellingProducts,
                ActiveUsers = activeUsers
            };

            return View(viewModel);
        }

        public ActionResult Orders()
        {
            var orders = _context.Orders
                                 .Include("OrderItems")
                                 .Include("OrderItems.Product")
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToList();
            return View(orders);
        }

        public ActionResult Products()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public ActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}