using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DessertApp.ViewModel
{
    public class OrderLineInfo
    {
        public int OrderlineID { get; set; }
        public int OrderID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderQuantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}