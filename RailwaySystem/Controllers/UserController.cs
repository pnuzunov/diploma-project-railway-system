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
        UsersRepository repo = new UsersRepository();
        public ActionResult Index()
        {
            if (Session["loggedUser"] == null || ((User)Session["loggedUser"])?.Id != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["items"] = repo.GetAll();

            return View();
        }

        public ActionResult Delete(int id)
        {
            if (Session["loggedUser"] == null || ((User)Session["loggedUser"])?.Id != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            repo.Delete(id);
            return RedirectToAction("Index", "User");
        }
    }
}