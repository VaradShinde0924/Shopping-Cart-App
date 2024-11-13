using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        [Display(Name = "Image File Name")]
        public string ImageFileName { get; set; }

        [NotMapped]
        public HttpPostedFileBase Image { get; set; }  
    }
}
