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
            bool statusRegistration = false;
            string messageRegistration = string.Empty;
            try
            {
                WebAuthenticationEntities webAuthenticationEntities = new WebAuthenticationEntities();
                var dataitem = webAuthenticationEntities.tbllogins.Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefault();
                if (dataitem != null)
                {
                    FormsAuthentication.SetAuthCookie(dataitem.Username, false);
                    if (Url.IsLocalUrl(returnurl) && returnurl.Length > 1 && returnurl.StartsWith("/")
                        && !returnurl.StartsWith("//") && !returnurl.StartsWith("/\\"))
                    {
                        return Redirect(returnurl);
                    }
                    else
                    {
                        statusRegistration = true;
                        messageRegistration = "Login Successfully";
                        ViewBag.Message = messageRegistration;
                        ViewBag.status = statusRegistration;
                        return RedirectToAction("Index");                       

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Something Wrong : Username or Password invalid ^_^");
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }        
            
        }


        [Authorize]
        //[AllowAnonymous]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Registration(tblRegistration registrationView)
        {
            bool statusRegistration = false;
            string messageRegistration = string.Empty;

            if (ModelState.IsValid)
            {
                // Email Verification
                string userName = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("Warning Email", "Sorry: Email already Exists");
                    return View(registrationView);
                }

                //Save User Data 
                using (WebAuthenticationEntities dbContext = new WebAuthenticationEntities())
                {
                    var user = new tblRegistration()
                    {
                        Username = registrationView.Username,
                        FirstName = registrationView.FirstName,
                        LastName = registrationView.LastName,
                        Email = registrationView.Email,
                        Password = registrationView.Password,
                        // ActivationCode = Guid.NewGuid(),
                        ID = registrationView.ID,
                    };

                    dbContext.tblRegistrations.Add(user);

                    dbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;

                    // context.Entry(foo).State = EntityState.Modified;
                    // context.SaveChanges();

                    dbContext.SaveChanges();
                }

                //Verification Email
               // VerificationEmail(registrationView.Email, registrationView.ActivationCode.ToString());
                messageRegistration = "Your account has been created successfully. ^_^";
                statusRegistration = true;
            }
            else
            {
                messageRegistration = "Something Wrong!";
            }
            ViewBag.Message = messageRegistration;
            ViewBag.Status = statusRegistration;

            return View(registrationView);
        }


    }
}