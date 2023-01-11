using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DessertApp.ViewModel;

namespace DessertApp.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        readonly dessertAppDBEntities2 dbe = new dessertAppDBEntities2();

        public ActionResult Dashboard()
        {
            var customerID = Convert.ToInt16(Session["userID"]);       
            var OrdersList = (from order in dbe.Orders
                              join customer in dbe.Users on order.CustomerID equals customer.userID
                              where order.CustomerID == customerID
                              select new AllOrders
                              {
                                  OrderID = order.OrderID,
                                  CustomerID = customer.userID,
                                  CustomerFname = customer.fname,
                                  CustomerLname = customer.lname,
                                  ItemCount = (int)order.ItemCount,
                                  TotalPrice = (decimal)order.TotalPrice,
                                  OrderDate = (DateTime)order.OrderDate,

                              }).ToList();
            var dessertsList = (from dessert in dbe.Desserts select dessert).ToList();
            var typesList = (from type in dbe.DessertTypes select type).ToList();
            ViewData["dessertsNum"] = dessertsList.Count();
            ViewData["ordersNum"] = OrdersList.Count();
            ViewData["typesNum"] = typesList.Count();
            return View();
        }

        public ActionResult Order()
        {
            
            var dessertsList = (from dessert in dbe.Desserts select dessert).ToList();

            ViewData["dessertList"] = dessertsList;

            return View();
        }

        public ActionResult AddToCart(FormCollection FormInput)
        {
            var prodID = Convert.ToInt16(Request["productID"]);
            var quant = Convert.ToInt16(FormInput["quantity"]);
            var dessertProd = dbe.Desserts.Find(prodID);
            var type = dbe.DessertTypes.Find(dessertProd.DessertTypeID);

            if (Session["cart"] == null)
            {
                List<CartItem> cart = new List<CartItem>();
                
                cart.Add(new CartItem()
                {
                    ProductID = prodID,
                    ProductName = dessertProd.DessertName,
                    ProductType = type.TypeName,
                    OrderQuantity = quant,
                    Price = Convert.ToDecimal(quant * dessertProd.DessertPrice),
                });
                Session["cart"] = cart;
            }
            else
            {
                List<CartItem> cart = (List<CartItem>)Session["cart"];
               
                cart.Add(new CartItem()
                {
                    ProductID = prodID,
                    ProductName = dessertProd.DessertName,
                    ProductType = type.TypeName,
                    OrderQuantity = quant,
                    Price = Convert.ToDecimal(quant * dessertProd.DessertPrice),
                });
                Session["cart"] = cart;
            }

            return RedirectToAction("Order");
        }

        public ActionResult RemoveFromCart()
        {
            var prodID = Convert.ToInt16(Request["productID"]);
            if(Session["cart"] != null)
            {
                List<CartItem> cart = (List<CartItem>)Session["cart"];
                var product = cart.First(prod=>prod.ProductID == prodID);
                cart.Remove(product);
                Session["cart"] = cart;
                if(cart.Count == 0)
                {
                   Session["cart"] = null;
                }
                
            }
            
            return RedirectToAction("Order");
        }

        public ActionResult ConfirmOrder(FormCollection FormInput)
        {
            var CustomerId = Convert.ToInt16(Session["userID"]);
            List<CartItem> cart = (List<CartItem>)Session["cart"];
            Order order = new Order();
            
            order.CustomerID = CustomerId;
            order.TotalPrice = Convert.ToDecimal(FormInput["totalPrice"]);
            order.OrderDate = DateTime.Now;
            order.ItemCount = cart.Count;

            dbe.Orders.Add(order);
            dbe.SaveChanges();

            foreach(var item in cart)
            {
                dbe.OrderLines.Add(new OrderLine()
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,                
                    OrderQuantity = item.OrderQuantity,
                    Price = item.Price
                   
                }); 
            }
            dbe.SaveChanges();
            Session["cart"] = null;

            return RedirectToAction("Order");
        }

        public ActionResult ShowOrderHistory()
        {
            var customerID = Convert.ToInt16(Session["userID"]);
            //var ordersList = (from order in dbe.Orders where order.CustomerID == customerID select order).ToList();

            //ViewData["ordersList"] = ordersList;
            var OrdersList = (from order in dbe.Orders
                                 join customer in dbe.Users on order.CustomerID equals customer.userID
                                 where order.CustomerID == customerID 
                                 select new AllOrders
                                 {
                                     OrderID = order.OrderID,
                                     CustomerID = customer.userID,
                                     CustomerFname = customer.fname,
                                     CustomerLname = customer.lname,
                                     ItemCount = (int)order.ItemCount,
                                     TotalPrice = (decimal)order.TotalPrice,
                                     OrderDate = (DateTime)order.OrderDate,

                                 }).ToList();

            return View(OrdersList);
        }

        public ActionResult CancelOrder()
        {
            var orderID = Convert.ToInt16(Request["orderID"]);
            var deleteOrder = (from order in dbe.Orders where order.OrderID == orderID select order).FirstOrDefault();

            dbe.Orders.Remove(deleteOrder);
            dbe.SaveChanges();

            return RedirectToAction("ShowOrderHistory");
 
        }

        public ActionResult ViewOrderDetails()
        {

            var orderID = Convert.ToInt16(Request["orderID"]);
          
            var detailsList = (from orderline in dbe.OrderLines
                                 join product in dbe.Desserts on orderline.ProductID equals product.DessertID
                                 join type in dbe.DessertTypes on product.DessertTypeID equals type.DessertTypeID
                               where orderline.OrderID == orderID
                               select new CartItem
                                 {
                                     ProductID = product.DessertID,
                                     ProductName = product.DessertName,
                                     ProductType = type.TypeName,
                                     OrderQuantity = (int)orderline.OrderQuantity,
                                     Price = (decimal)orderline.Price,

                                 }).ToList();

            List<AllOrders> custDetails = (from order in dbe.Orders
                                           join customer in dbe.Users on order.CustomerID equals customer.userID
                                           where order.OrderID == orderID
                                           select new AllOrders
                                           {
                                               OrderID = order.OrderID,
                                               CustomerID = customer.userID,
                                               CustomerFname = customer.fname,
                                               CustomerLname = customer.lname,
                                               ItemCount = (int)order.ItemCount,
                                               TotalPrice = (decimal)order.TotalPrice,
                                               OrderDate = (DateTime)order.OrderDate,

                                           }).ToList();

            return View(new OrderDetails() { Cartitem = detailsList, Orderinfo = custDetails });

        }

        public ActionResult ViewProfile()
        {
            dessertAppDBEntities2 dbe = new dessertAppDBEntities2();
     
            var accID = Convert.ToInt16(Session["userID"]);
            var userInfo = (from user in dbe.Users where user.userID == accID select user).FirstOrDefault();

            ViewData["userInfo"] = userInfo;
            return View();
        }

        public ActionResult EditProfile()
        {
            dessertAppDBEntities2 dbe = new dessertAppDBEntities2();

            var accID = Convert.ToInt16(Request["accID"]);
            var editedProfile = dbe.Users.Find(accID);

            return View("EditProfile", editedProfile);
        }

        public ActionResult EditingProfile(User editedProfile)
        {
            dessertAppDBEntities2 dbe = new dessertAppDBEntities2();

            dbe.Entry(editedProfile).State = EntityState.Modified;
            dbe.SaveChanges();

            return RedirectToAction("ViewProfile");
        }

    }
}