using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using RailwaySystem.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public class HomeController : Controller
    {
        UsersRepository repo = new UsersRepository();

        public ActionResult Index()
        {
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
            CheckModelIsValid(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User u = new User();
                
            u.Username = model.Username;
            u.Password = model.Password;
            u.FirstName = model.FirstName;
            u.LastName = model.LastName;
            u.Email = model.Email;
            u.Phone = model.Phone;
            u.RoleId = (int)UsersRepository.Levels.CUSTOMER_ACCESS;
            User match;
            do
            {
                u.ClientNumber = new Random().Next(100000, 1000000).ToString();
                match = repo.GetFirstOrDefault(i => u.ClientNumber.Equals(i.ClientNumber));
            } while (match != null);

            repo.Add(u);
            return RedirectToAction("Index", "Home");
        }

        private void CheckModelIsValid(RegisterVM model)
        {
            UsersRepository usersRepository = new UsersRepository();
            User u;
            u = usersRepository.GetFirstOrDefault(i => i.Username == model.Username);
            if(u != null)
            {
                ModelState.AddModelError("UsernameTakenError", "Username is taken.");
                return;
            }
            u = usersRepository.GetFirstOrDefault(i => i.Email == model.Email);
            if (u != null)
            {
                ModelState.AddModelError("UsernameTakenError", "Email address is taken.");
                return;
            }
            u = usersRepository.GetFirstOrDefault(i => i.Phone == model.Phone);
            if (u != null)
            {
                ModelState.AddModelError("UsernameTakenError", "Phone is taken.");
                return;
            }
        }

        public ActionResult Logout()
        {
            Session["loggedUser"] = null;

            return RedirectToAction("Index", "Home");
        }

    }

}