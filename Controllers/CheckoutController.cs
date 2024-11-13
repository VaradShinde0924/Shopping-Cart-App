using Microsoft.AspNet.Identity;
using ShoppingCartApp.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCartApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Checkout
        [Authorize]
        public ActionResult Index()
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProcessPayment(string stripeToken)
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var totalAmount = (long)(cart.Sum(c => c.Quantity * c.Product.Price) * 100); // Stripe processes in cents

            StripeConfiguration.ApiKey = "sk_test_..."; // Your Stripe Secret Key

            var options = new ChargeCreateOptions
            {
                Amount = totalAmount,
                Currency = "usd",
                Description = "Order Payment",
                Source = stripeToken,
            };

            var service = new ChargeService();
            Charge charge;

            try
            {
                charge = service.Create(options);
            }
            catch (StripeException ex)
            {
                // Handle Stripe error
                ModelState.AddModelError("", $"Payment processing error: {ex.Message}");
                return View("Index", cart); // Return to the checkout view with the cart
            }

            if (charge.Status == "succeeded")
            {
                // Payment successful, create the order
                var order = CreateOrder(cart);
                _context.Orders.Add(order);
                _context.SaveChanges();

                // Clear the cart after placing the order
                Session["Cart"] = null;

                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }

            ModelState.AddModelError("", "Payment failed. Please try again.");
            return View("Index", cart);
        }

        public ActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders.Include("OrderItems").Include("OrderItems.Product").FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        private List<CartItem> GetCart()
        {
            return (List<CartItem>)Session["Cart"] ?? new List<CartItem>();
        }

        private Order CreateOrder(List<CartItem> cart)
        {
            return new Order
            {
                UserId = User.Identity.GetUserId(),
                OrderDate = DateTime.Now,
                TotalPrice = cart.Sum(c => c.Quantity * c.Product.Price),
                Status = "Processing",
                OrderItems = cart.Select(c => new OrderItem
                {
                    ProductId = c.Product.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                }).ToList()
            };
        }
    }
}