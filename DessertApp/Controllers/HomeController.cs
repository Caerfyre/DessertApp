using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace DessertApp.Controllers
{
    public class HomeController : Controller
    { 
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            dessertAppDBEntities2 dbe = new dessertAppDBEntities2();
            var dataItem = dbe.Users.Where( input => input.username == model.username && input.password == model.password).First();
            if (dataItem != null)
            {
                FormsAuthentication.SetAuthCookie(dataItem.username, false);

                if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    if (dataItem.userType == "Admin")
                    {
                        Session["userID"] = dataItem.userID;
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {

                        Session["userID"] = dataItem.userID;
                        return RedirectToAction("Dashboard", "Customer");
                    }  
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username / password.");
                return View();
            }
        }

        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
    }

}