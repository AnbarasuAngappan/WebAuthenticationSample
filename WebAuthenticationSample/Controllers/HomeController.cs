using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebAuthenticationSample.Models;

namespace WebAuthenticationSample.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tbllogin model,string returnurl)
        {
            WebAuthenticationEntities webAuthenticationEntities = new WebAuthenticationEntities();
            var dataitem = webAuthenticationEntities.tbllogins.Where(x => x.Username == model.Username && x.Password == model.Password).First();
            if(dataitem != null)
            {
                FormsAuthentication.SetAuthCookie(dataitem.Username, false);
                if(Url.IsLocalUrl(returnurl) && returnurl.Length > 1 && returnurl.StartsWith("/")
                    && !returnurl.StartsWith("//") && !returnurl.StartsWith("/\\"))
                {
                    return Redirect(returnurl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName / Password");
                return View();
            }
            
        }


        [Authorize]
        //[AllowAnonymous]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }


    }
}