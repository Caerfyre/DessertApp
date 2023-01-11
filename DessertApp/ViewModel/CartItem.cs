using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DessertApp.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public int OrderQuantity { get; set; }
        public decimal Price { get; set; }
    }
}