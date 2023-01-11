using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DessertApp.ViewModel
{
    public class OrderDetails
    {
        public List<CartItem> Cartitem { get; set; }
        public List<AllOrders> Orderinfo { get; set; }

    }

}