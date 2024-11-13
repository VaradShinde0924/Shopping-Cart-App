using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoppingCartApp.Models;
using System.IO;
using PagedList;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string searchTerm, string sortOrder, string category, int? page)
        {
            var products = db.Products.AsQueryable();

            var categories = new List<string> { "Electronics", "Deals of the Day", "Sports and Health", "Babies and Toys", "Men's Fashion", "Watches", "Groceries", "Home and Lifestyle", "Women’s Fashion", "Automobiles" };
            ViewBag.Categories = categories;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }


            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParam = sortOrder == "Price" ? "price_desc" : "Price";

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "Price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            // Define the list of categories
            ViewBag.Categories = new List<string>
    {
        "Deals of the Day",
        "Electronics",
        "Sports and Health",
        "Babies and Toys",
        "Groceries",
        "Home and Lifestyle",
        "Women’s Fashion",
        "Men’s Fashion",
        "Watches",
        "Automobiles"
    };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "ProductId,Name,Description,Price,Quantity,Category,ImageFileName,Image")] Product product)
        {
            if (ModelState.IsValid)
            {
                // Check if an image file is provided
                if (product.Image != null && product.Image.ContentLength > 0)
                {
                    // Generate a unique file name for the image
                    string fileName = Path.GetFileNameWithoutExtension(product.Image.FileName);
                    string extension = Path.GetExtension(product.Image.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension; // Add timestamp to avoid file conflicts

                    // Save the image file to the server
                    string folderPath = Server.MapPath("~/Images");
                    string filePath = Path.Combine(folderPath, fileName);
                    product.Image.SaveAs(filePath);

                    // Set the ImageFileName property for later use (storing relative path in DB)
                    product.ImageFileName = fileName;
                }

                // Add the product to the database and save changes
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            // Reload the categories in case of an invalid model state
            ViewBag.Categories = new List<string>
    {
        "Deals of the Day",
        "Electronics",
        "Sports and Health",
        "Babies and Toys",
        "Groceries",
        "Home and Lifestyle",
        "Women’s Fashion",
        "Men’s Fashion",
        "Watches",
        "Automobiles"
    };

            return View(product);
        }



        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Populate ViewBag.Categories for the dropdown list
            ViewBag.Categories = new SelectList(
                new List<string>
                {
            "Electronics", "Deals of the Day", "Sports and Health", "Babies and Toys",
            "Men's Fashion", "Watches", "Groceries", "Home and Lifestyle",
            "Women’s Fashion", "Automobiles"
                }, product.Category);

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Description,Price,Quantity,Category")] Product product, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing product from the database
                var existingProduct = db.Products.Find(product.ProductId);
                if (existingProduct == null)
                {
                    return HttpNotFound();
                }

                // Update the basic product fields
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Category = product.Category;

                // Check if a new image file is provided
                if (Image != null && Image.ContentLength > 0)
                {
                    // Save the new image
                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    Image.SaveAs(path);

                    // Update the ImageFileName with the new filename
                    existingProduct.ImageFileName = fileName;
                }

                // Save the updated product
                db.Entry(existingProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, populate ViewBag.Categories again before returning the view
            ViewBag.Categories = new SelectList(
                new List<string>
                {
            "Electronics", "Deals of the Day", "Sports and Health", "Babies and Toys",
            "Men's Fashion", "Watches", "Groceries", "Home and Lifestyle",
            "Women’s Fashion", "Automobiles"
                }, product.Category);

            return View(product);
        }




        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);

            // Check if there is a file associated with the product
            if (product.Image != null)
            {
                var fileName = product.Image.FileName;  // Get the image file name

                // Construct the full path to the image file
                var imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);

                // Check if the file exists and then delete it
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Remove the product from the database
            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ProductsByCategory(string category)
        {
            // Fetch products belonging to the selected category
            var products = db.Products.Where(p => p.Category == category).ToList();

            // Pass the category name to the view through ViewBag
            ViewBag.Category = category;

            return View(products); // No pagination, just return the full list
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}