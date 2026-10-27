using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _25DH110651_MyStore.Models.ViewModel
{
    public class CustomerVM
    {
        // --- Thông tin từ bảng User ---
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // --- Thông tin từ bảng Customer ---
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
    }
}