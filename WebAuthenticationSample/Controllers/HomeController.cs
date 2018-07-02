using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebAuthenticationSample.Models;

namespace WebAuthenticationSample.Controllers
{
    public class HomeController : Controller
    {
        public bool statuslogin = false;
        public string messagelogin = string.Empty;
        WebAuthenticationEntities entities = new WebAuthenticationEntities();

        [Authorize(Roles="Create")]
        public ActionResult Index()
        {          
            statuslogin = true;
            messagelogin = "Login Successfully";
            ViewBag.Message = messagelogin;
            ViewBag.status = statuslogin;
            return View();
        }

        //[Authorize]
        //public ActionResult Create()
        //{
        //    return View();
        //}
        

        //[Authorize]
        //public ActionResult Delete()
        //{
        //    return View();
        //}


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
                        if (dataitem.Role == "Create")
                        {
                            return RedirectToAction("Registration");
                        }
                        else if (dataitem.Role == "Read")
                        {
                            return RedirectToAction("Read");
                        }
                        else if (dataitem.Role == "Update")
                        {
                            return RedirectToAction("EditView");
                        }
                        else if (dataitem.Role == "Delete")
                        {
                            return RedirectToAction("Deleteview");
                        }
                        else
                            return RedirectToAction("Index");
                    }                   
                }
                else
                {
                    ModelState.AddModelError("", "Something Wrong : Username or Password invalid !!");
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


        [Authorize(Roles = "Create")]
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
                //string userName = Membership.GetUserNameByEmail(registrationView.Email);
                //if (!string.IsNullOrEmpty(userName))
                //{
                //    ModelState.AddModelError("Warning Email", "Sorry: Email already Exists");
                //    return View(registrationView);
                //}

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
                        ConfirmPassword = registrationView.ConfirmPassword,
                        // ActivationCode = Guid.NewGuid(),
                        ID = registrationView.ID,
                    };

                    var login = new tbllogin()
                    {
                        ID = registrationView.ID,
                        Username = registrationView.Username,
                        Password = registrationView.Password,
                        Role = null,

                    };
                    dbContext.tbllogins.Add(login);
                    dbContext.tblRegistrations.Add(user);

                    dbContext.SaveChanges();
                    //dbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    // context.Entry(foo).State = EntityState.Modified;
                    // context.SaveChanges();
                    //dbContext.SaveChanges();
                }
                //Verification Email
                // VerificationEmail(registrationView.Email, registrationView.ActivationCode.ToString());
                messageRegistration = "Your account has been created successfully.";
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

        [Authorize(Roles = "Read")]
        public ActionResult Read()
        {
            List<tblRegistration> registrations = entities.tblRegistrations.ToList();
            return View(registrations);
        }

        [Authorize(Roles = "Delete", Users ="kiru")]
        public ActionResult Deleteview()
        {
            List<tblRegistration> partialViews = entities.tblRegistrations.ToList();
            return View(partialViews);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRegistration partialView = entities.tblRegistrations.Find(id);
            if (partialView == null)
            {
                return HttpNotFound();
            }
            return View(partialView);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            try
            {
                tblRegistration tblRegistration = entities.tblRegistrations.Find(id);
                entities.tblRegistrations.Remove(tblRegistration);

                tbllogin tbllogin = entities.tbllogins.Find(id);
                entities.tbllogins.Remove(tbllogin);
                
                entities.SaveChanges();
                return RedirectToAction("Deleteview");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [Authorize(Roles="Update")]
        public ActionResult EditView()
        {
            List<tblRegistration> tblRegistrations = entities.tblRegistrations.ToList();
            return View(tblRegistrations);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblRegistration tblRegistration = entities.tblRegistrations.Find(id);
            if (tblRegistration == null)
            {
                return HttpNotFound();
            }
            return View(tblRegistration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()//[Bind(Include = "CustomerID,CustomerName,ContactMidName,PhoneNumber,EmailID,Password,ConfirmPassword")]Customer customer
        {
            if (ModelState.IsValid)
            {
                tblRegistration tblRegistration = new tblRegistration();              
                UpdateModel(tblRegistration);//UpdateModel<Customer>(customers);               
                entities.Entry(tblRegistration).State = EntityState.Modified;                
                entities.SaveChanges();
                return RedirectToAction("EditView");
            }
            return View();
        }
    }
}