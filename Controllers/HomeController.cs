using ShoppingCartApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCartApp.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext(); // Create an instance of the ApplicationDbContext

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        // GET: Contact
        public ActionResult Contact()
        {
            return View();
        }

        // POST: Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactMessage contact)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now; // Or DateTime.UtcNow to avoid timezone issues.
                db.ContactMessages.Add(contact);
                db.SaveChanges();
                TempData["Message"] = "Your message has been sent!";
                return RedirectToAction("Contact");
            }


            return View(contact);
        }
    }
}