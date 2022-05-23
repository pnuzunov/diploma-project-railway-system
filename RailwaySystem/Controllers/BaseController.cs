using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public abstract class BaseController<T, TSearchVM, TCreateVM, TEditVM> : Controller
        where T : BaseEntity, new()
        where TSearchVM : ViewModels.BaseIndexVM, new()
        where TCreateVM : ViewModels.BaseCreateVM, new()
        where TEditVM : ViewModels.BaseEditVM, new()
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

        public abstract ActionResult Index();

        [HttpPost]
        public abstract ActionResult Index(TSearchVM model);

        public abstract ActionResult Create();

        [HttpPost]
        public abstract ActionResult Create(TCreateVM model);

        public abstract ActionResult Edit(int id);

        [HttpPost]
        public abstract ActionResult Edit(TEditVM model);
    }
}