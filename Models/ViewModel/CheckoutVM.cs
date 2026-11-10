using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _25DH110651_MyStore.Models.ViewModel
{
    public class CheckoutVM
    {
        public List<CartItem> CartItems { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingAddress { get; set; }
        public string Username { get; set; }
    }
}