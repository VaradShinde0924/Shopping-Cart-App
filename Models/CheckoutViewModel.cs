using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartApp.Models
{
    public class CheckoutViewModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Landmark { get; set; }
        public string AddressType { get; set; } // Home or Work
        public string PaymentOption { get; set; } // CashOnDelivery or UPI
        public string UPIOption { get; set; } // GooglePay or PhonePe
        public decimal TotalPrice { get; set; }
    }
}