using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DessertApp.ViewModel
{
    public class AllOrders
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerFname { get; set; }
        public string CustomerLname { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalPrice { get; set; }
        public System.DateTime OrderDate { get; set; }
    }
}