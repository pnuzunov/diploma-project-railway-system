using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            UsersRepository repo = new UsersRepository();
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["items"] = repo.GetAll();

            return View();
        }

        public ActionResult Delete(int id)
        {
            UsersRepository repo = new UsersRepository();
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            repo.Delete(id);
            return RedirectToAction("Index", "User");
        }
    }
}