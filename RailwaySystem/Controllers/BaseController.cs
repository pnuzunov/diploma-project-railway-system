using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public abstract class BaseController<E, R, TCreateVM, TEditVM> : Controller
        where E : Entities.BaseEntity, new()
        where R : Repositories.BaseRepository<E>, new()
        where TCreateVM : ViewModels.BaseCreateVM, new()
        where TEditVM : ViewModels.BaseEditVM, new()
    {
        protected bool CanAccessPage(Repositories.UsersRepository.Levels level)
        {
            Repositories.UsersRepository usersRepository = new Repositories.UsersRepository();
            Entities.User loggedUser = (Entities.User)Session["loggedUser"];
            if (loggedUser != null && usersRepository.CanAccess(loggedUser.Id, level))
            {
                return true;
            }
            return false;
        }

        protected abstract void CheckIsModelValid(TCreateVM model);
        protected abstract void CheckIsModelValid(TEditVM model);
        protected abstract void GenerateEntity(E entity, TCreateVM model);
        protected abstract void GenerateEntity(E entity, TEditVM model);
        protected abstract void GenerateModel(TEditVM model, E entity);
        protected virtual void LoadExtraViewData() { }
        public virtual ActionResult Index()
        {

            R repo = new R();

            LoadExtraViewData();
            ViewData["items"] = repo.GetAll();
            return View();
        }

        public virtual ActionResult Create()
        {

            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            LoadExtraViewData();
            return View();
        }

        [HttpPost]
        public virtual ActionResult Create(TCreateVM model)
        {

            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                CheckIsModelValid(model);
            }

            if (!ModelState.IsValid)
            {
                LoadExtraViewData();
                return View(model);
            }

            E entity = new E();
            R repo = new R();

            GenerateEntity(entity, model);

            repo.Add(entity);

            return RedirectToAction("Index");
        }

        public virtual ActionResult Edit(int id)
        {
            R repo = new R();
            E entity = repo.GetById(id);
            TEditVM model = new TEditVM();

            model.Id = id;
            GenerateModel(model, entity);

            LoadExtraViewData();
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Edit(TEditVM model)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                CheckIsModelValid(model);
            }
            if (!ModelState.IsValid)
            {
                LoadExtraViewData();
                return View(model);
            }

            E entity = new E();
            GenerateEntity(entity, model);
            R repo = new R();
            repo.Update(entity);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            R repo = new R();

            repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}