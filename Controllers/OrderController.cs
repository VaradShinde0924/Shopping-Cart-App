using ShoppingCartApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext _context;

        public OrderController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: My Orders (for the logged-in user)
        public ActionResult MyOrders()
        {
            var userId = User.Identity.GetUserId();
            var orders = _context.Orders
                                 .Include("OrderItems")
                                 .Include("OrderItems.Product")
                                 .Where(o => o.UserId == userId)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToList();

            return View(orders); // Return the view with the user's orders
        }

        public ActionResult Details(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = _context.Orders.Include("OrderItems").Include("OrderItems.Product")
                                       .FirstOrDefault(o => o.OrderId == id && o.UserId == userId);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        public ActionResult CancelOrder(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id && o.UserId == userId);

            if (order == null || order.Status != "Processing")
                return RedirectToAction("Index");

            order.Status = "Cancelled";
            _context.SaveChanges();

            return RedirectToAction("Index");
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