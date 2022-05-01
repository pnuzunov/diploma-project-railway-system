using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool CanAccessPage(UsersRepository.Levels level)
        {
            UsersRepository usersRepository = new UsersRepository();
            User loggedUser = (User)Session["loggedUser"];
            if (loggedUser == null || !usersRepository.CanAccess(loggedUser.Id, level))
            {
                return false;
            }
            return true;
        }
    }
}