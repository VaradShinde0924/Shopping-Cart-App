using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartApp.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "How can we help you?")]
        public string IssueType { get; set; }  // Add this property to match the view

        [Required]
        public string Message { get; set; }

        public DateTime? CreatedAt { get; set; }


    }
}
