using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using RailwaySystem.ViewModels.User;

namespace RailwaySystem.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            if(Session["loggedUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            UsersRepository usersRepository = new UsersRepository();
            User loggedUser = (User)Session["loggedUser"];
            User user = usersRepository.GetFirstOrDefault(u => u.Id == loggedUser.Id);

            DetailsVM model = new DetailsVM();
            model.Username = user.Username;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Password = user.Password;
            
            return View(model);
        }

    }
}