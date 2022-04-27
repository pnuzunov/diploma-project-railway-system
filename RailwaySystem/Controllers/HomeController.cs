using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using RailwaySystem.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PayPal.Api;
using RailwaySystem.HelperClasses;

namespace RailwaySystem.Controllers
{
    public class HomeController : Controller
    {
        UsersRepository repo = new UsersRepository();

        public ActionResult Index()
        {
            //testing purpose
            //User usr = repo.GetFirstOrDefault(u => u.Username == "admin" && u.Password == "admin");
            //Session["loggedUser"] = usr;

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["loggedUser"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                User usr = repo.GetFirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
                if (usr == null)
                    ModelState.AddModelError("AuthError", "Invalid username and password!");
                else
                    Session["loggedUser"] = usr;
            }

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            if (Session["loggedUser"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            RegisterVM model = new RegisterVM();
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            User u;
            u = repo.GetFirstOrDefault(i => i.Username == model.Username || i.Email == model.Email || i.Phone == model.Phone);

            if (u == null)
            {
                u = new User();
                u.Username = model.Username;
                u.Password = model.Password;
                u.FirstName = model.FirstName;
                u.LastName = model.LastName;
                u.Email = model.Email;
                u.Phone = model.Phone;
                u.RoleId = (int)UsersRepository.Levels.CUSTOMER_ACCESS;
                repo.Add(u);
            }

            else
            {
                if(u.Username == model.Username)
                    ModelState.AddModelError("UserExistsError", "Username is taken!");
                else if(u.Email == model.Email)
                    ModelState.AddModelError("EmailExistsError", "Email address is taken!");
                else if(u.Phone == model.Phone)
                    ModelState.AddModelError("PhoneExistsError", "Phone number is taken!");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session["loggedUser"] = null;

            return RedirectToAction("Index", "Home");
        }

    }

}