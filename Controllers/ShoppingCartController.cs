using Microsoft.AspNet.Identity;
using ShoppingCartApp.Models;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCartApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext _context;

        public ShoppingCartController()
        {
            _context = new ApplicationDbContext();
        }

        // Add product to cart
        public ActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return HttpNotFound();

            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem == null)
            {
                cart.Add(new CartItem { ProductId = product.ProductId, Quantity = 1, Product = product });
            }
            else
            {
                cartItem.Quantity++;
            }

            SaveCartItems(cart);
            return RedirectToAction("ViewCart", "ShoppingCart");
        }

        // View Cart
        public ActionResult ViewCart()
        {
            var cart = (List<CartItem>)Session["Cart"] ?? new List<CartItem>();
            return View(cart);
        }

        // Update Cart
        public ActionResult UpdateCart(int id, int quantity)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                if (quantity == 0) cart.Remove(cartItem);
                else cartItem.Quantity = quantity;
            }

            SaveCartItems(cart);
            return RedirectToAction("ViewCart");
        }

        // Remove from Cart
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null) cart.Remove(cartItem);

            SaveCartItems(cart);
            return RedirectToAction("ViewCart");
        }

        //public ActionResult Checkout()
        //{
        //    var cart = GetCartItems();  // Retrieve cart items from session

        //    if (!cart.Any())
        //    {
        //        // If the cart is empty, redirect to the products page
        //        return RedirectToAction("Index", "Products");
        //    }

        //    // Get the logged-in user's ID
        //    var userId = User.Identity.GetUserId();

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return RedirectToAction("Login", "Account");  // Redirect to login if user is not authenticated
        //    }

        //    // Create a new order with initial status as "Pending"
        //    var order = new Order
        //    {
        //        UserId = userId,  // Set the logged-in user's ID
        //        OrderDate = DateTime.Now,  // Set the order date
        //        TotalPrice = cart.Sum(c => c.Quantity * c.Product.Price),  // Calculate total price
        //        Status = "Pending",  // Set the initial order status to "Pending"
        //        OrderItems = cart.Select(c => new OrderItem
        //        {
        //            ProductId = c.Product.ProductId,
        //            Quantity = c.Quantity,
        //            UnitPrice = c.Product.Price
        //        }).ToList()  // Create a list of order items from the cart
        //    };

        //    // Save the order to the database
        //    _context.Orders.Add(order);
        //    _context.SaveChanges();

        //    // Clear the cart after checkout
        //    ClearCart();

        //    // Redirect to the order confirmation page
        //    return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
        //}

        // Checkout
        [Authorize] // Ensure only logged-in users can checkout
        public ActionResult Checkout()
        {
            var cart = (List<CartItem>)Session["Cart"];
            if (cart == null || !cart.Any())
            {
                // If the cart is empty, redirect to the products page
                return RedirectToAction("Index", "Product");
            }

            // Get the logged-in user's ID
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            // Create a new order
            var order = new ShoppingCartApp.Models.Order
            {
                UserId = userId, // Get the actual logged-in user's ID
                OrderDate = DateTime.Now,
                TotalPrice = cart.Sum(c => c.Quantity * c.Product.Price),
                OrderItems = cart.Select(c => new OrderItem
                {
                    ProductId = c.Product.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                }).ToList()
            };
            // Save the order to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear the cart after successful checkout
            Session["Cart"] = null;

            return View("OrderConfirmation", order);
        }


        // Order Confirmation process (ShoppingCartController)
        public ActionResult OrderConfirmation(int id)
        {
            // Find the order by its ID and include the associated OrderItems and Products
            var order = _context.Orders
                                .Include("OrderItems.Product") // Use string-based Include for older versions of EF
                                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return HttpNotFound(); // If the order doesn't exist, return 404
            }

            // Ensure that the OrderItems property is initialized
            if (order.OrderItems == null)
            {
                order.OrderItems = new List<OrderItem>(); // Initialize to avoid null reference exceptions
            }

            // Return the OrderConfirmation view with the order details
            return View(order);
        }






        // Helper method to retrieve the cart items from the session
        private List<CartItem> GetCartItems()
        {
            var cart = (List<CartItem>)Session["Cart"] ?? new List<CartItem>();
            return cart;
        }

        // Helper method to save the cart items to the session
        private void SaveCartItems(List<CartItem> cart)
        {
            Session["Cart"] = cart;
        }

        // Helper method to clear the cart after successful checkout
        private void ClearCart()
        {
            Session["Cart"] = null;
        }

        // Update the status of an order (Admin/Employee Functionality)
        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.Status = newStatus;  // Update the status to the new value
            _context.SaveChanges();

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmOrder(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic for confirming order goes here
                // E.g., Save the order and address details in the database

                return RedirectToAction("OrderConfirmation"/*, new { id = orderId }*/);
            }

            // Return the view again with model data if validation fails
            return View("Checkout", model);
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