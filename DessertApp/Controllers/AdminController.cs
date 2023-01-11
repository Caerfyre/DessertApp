using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DessertApp.ViewModel;

namespace DessertApp.Controllers
{
    public class AdminController : Controller
    {
        dessertAppDBEntities2 dbe = new dessertAppDBEntities2();

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            var typesList = (from type in dbe.DessertTypes select type).ToList();
            var usersList = (from user in dbe.Users select user).ToList();
            var dessertsList = (from dessert in dbe.Desserts select dessert).ToList();
            var AllOrdersList = (from order in dbe.Orders
                                 join customer in dbe.Users on order.CustomerID equals customer.userID
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
            SelectList list = new SelectList(typesList, "DessertTypeID", "TypeName");
            ViewBag.dessertTypes = list;
            ViewData["typesNum"] = typesList.Count();
            ViewData["dessertsNum"] = dessertsList.Count();
            ViewData["usersNum"] = usersList.Count();
            ViewData["ordersNum"] = AllOrdersList.Count();
            
            return View();
        }

       public ActionResult AddCategory()
        {
            return View();
        } 

     
        public ActionResult AddingCategory(FormCollection FormInput)
        {
           
            DessertType type = new DessertType();

            type.TypeName = FormInput["typeName"];
            type.TypeDesc = FormInput["typeDesc"];

            dbe.DessertTypes.Add(type);
            dbe.SaveChanges();
            

            return RedirectToAction("AddCategory");
        }


        public ActionResult ShowCategories()
        {
            
            var typesList = (from type in dbe.DessertTypes select type).ToList();

            //ViewData[] function that stores information in the controller to be retrieved in the view
            ViewData["categList"] = typesList;

            return View();
        }

        public ActionResult DeleteCategory()
        {
  
            var categID = Convert.ToInt16(Request["CategID"]);
            var deleteType = (from type in dbe.DessertTypes where type.DessertTypeID == categID select type).FirstOrDefault();

            dbe.DessertTypes.Remove(deleteType);
            dbe.SaveChanges();

            return RedirectToAction("ShowCategories");
        }


        public ActionResult AddDessert()
        {
            var typesList = dbe.DessertTypes.ToList();
            SelectList list = new SelectList(typesList, "DessertTypeID", "TypeName");
            ViewBag.dessertTypes = list;

            return View();
        }

        public ActionResult AddingDessert(FormCollection FormInput)
        {
            Dessert dessert = new Dessert();

            dessert.DessertName = FormInput["dessertName"];
            dessert.DessertTypeID = Convert.ToInt16(FormInput["DessertTypesList"]);
            dessert.DessertPrice = Convert.ToDecimal(FormInput["dessertPrice"]);

            dbe.Desserts.Add(dessert);
            dbe.SaveChanges();

            return RedirectToAction("AddDessert");
        }

        public ActionResult ShowDesserts()
        {
          
            var dessertsList = (from dessert in dbe.Desserts select dessert).ToList();
            ViewData["dessertList"] = dessertsList;

            return View();
        }

        public ActionResult DeleteDessert()
        {
           
            var dessertID = Convert.ToInt16(Request["dessertID"]);
            var deleteDessert = (from dessert in dbe.Desserts where dessert.DessertID == dessertID select dessert).FirstOrDefault();

            dbe.Desserts.Remove(deleteDessert);
            dbe.SaveChanges();

            return RedirectToAction("ShowDesserts");
        }


        public ActionResult EditCategory()
        {
            

            var categID = Convert.ToInt16(Request["CategID"]);
            var editCateg = dbe.DessertTypes.Find(categID);

            return View("EditCategory", editCateg);
        }

        public ActionResult EditingCategory(DessertType editCateg)
        {
            dbe.Entry(editCateg).State = EntityState.Modified;
            dbe.SaveChanges();

            return RedirectToAction("ShowCategories");
        }


        public ActionResult EditDessert()
        {           
            var typesList = dbe.DessertTypes.ToList();
            SelectList list = new SelectList(typesList, "DessertTypeID", "TypeName");
            ViewBag.dessertTypes = list;

            var dessertID = Convert.ToInt16(Request["dessertID"]);
            var editDessert= dbe.Desserts.Find(dessertID);

            return View("EditDessert", editDessert);
        }

        public ActionResult EditingDessert(Dessert editDessert)
        {
            dbe.Entry(editDessert).State = EntityState.Modified;
            dbe.SaveChanges();

            return RedirectToAction("ShowDesserts");
        }

        public ActionResult ShowUsers()
        {
          
            var usersList = (from user in dbe.Users select user).ToList();

            ViewData["usersList"] = usersList;
            return View();
        }

        public ActionResult AddUsers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddingUsers(FormCollection FormInput)
        {

            User user = new User();

            user.username = FormInput["username"];
            user.fname = FormInput["fname"];
            user.lname = FormInput["lname"];
            user.password = FormInput["password"];
            user.userType = FormInput["role"];

            dbe.Users.Add(user);
            dbe.SaveChanges();

            return RedirectToAction("AddUsers");
        }

        public ActionResult DeleteUser()
        {
            
            var userID = Convert.ToInt16(Request["userID"]);
            var deleteUser = (from user in dbe.Users where user.userID == userID select user).FirstOrDefault();

            dbe.Users.Remove(deleteUser);
            dbe.SaveChanges();

            return RedirectToAction("ShowUsers");
        }

        public ActionResult EditUser()
        {
           
            string[] roles = { "Admin", "Customer" };
            var roleList = roles.ToList();
            SelectList list = new SelectList(roleList);
            ViewBag.rolesList = list;

            var userID = Convert.ToInt16(Request["userID"]);
            var editedUser = dbe.Users.Find(userID);
           
            return View("EditUser", editedUser);
        }

        public ActionResult EditingUser(User editedUser)
        {
          
            dbe.Entry(editedUser).State = EntityState.Modified;
            dbe.SaveChanges();

            return RedirectToAction("ShowUsers");
        }

        public ActionResult ViewProfile()
        {
           
            var accID = Convert.ToInt16(Session["userID"]);
            var userInfo = (from user in dbe.Users where user.userID == accID select user).FirstOrDefault();
            
            ViewData["userInfo"] = userInfo;
            return View();
        }

        public ActionResult EditProfile()
        {
   
            var accID = Convert.ToInt16(Request["accID"]);
            var editedProfile = dbe.Users.Find(accID);

            return View("EditProfile", editedProfile);
        }

        public ActionResult EditingProfile(User editedProfile)
        {
 
            dbe.Entry(editedProfile).State = EntityState.Modified;
            dbe.SaveChanges();

            return RedirectToAction("ViewProfile");
        }

        public ActionResult ShowOrders()
        {
            var AllOrdersList = (from order in dbe.Orders
                                 join customer in dbe.Users on order.CustomerID equals customer.userID
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


            return View(AllOrdersList);
        }

        public ActionResult ViewOrderDetails()
        {
            var orderID = Convert.ToInt16(Request["orderID"]);

            List<CartItem> detailsList = (from orderline in dbe.OrderLines
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
         
            ViewData["orderID"] = orderID;

            return View(new OrderDetails() { Cartitem = detailsList, Orderinfo = custDetails }) ;
        }

    }
}