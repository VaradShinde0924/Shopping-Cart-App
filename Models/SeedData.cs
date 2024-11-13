using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class SeedData
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.Add(new Product { Name = "Product 1", Description = "Description for Product 1", Price = 100, Quantity = 10 });
                context.Products.Add(new Product { Name = "Product 2", Description = "Description for Product 2", Price = 200, Quantity = 5 });
                context.SaveChanges();
            }
        }
    }
}