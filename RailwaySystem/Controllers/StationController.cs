using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using RailwaySystem.ViewModels.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public class StationController : Controller
    {
        private bool CanAccessPage(UsersRepository.Levels level)
        {
            //UsersRepository usersRepository = new UsersRepository();
            //User loggedUser = (User)Session["loggedUser"];
            //if (loggedUser == null || usersRepository.CanAccess(loggedUser.Id, level))
            //{
            //    return false;
            //}
            return true;
        }

        protected void CheckIsModelValid(CreateVM model)
        {
            StationsRepository repo = new StationsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name) != null)
            {
                ModelState.AddModelError("AuthError", "Station already exists!");
            }
            if (repo.GetFirstOrDefault(i => i.Latitude == model.Latitude && i.Longitude == model.Longitude) != null)
            {
                ModelState.AddModelError("AuthError", "There is already a station with these coordinates!");
            }
        }

        protected void CheckIsModelValid(EditVM model)
        {
            StationsRepository repo = new StationsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name && i.Id != model.Id) != null)
            {
                ModelState.AddModelError("AuthError", "Station already exists!");
            }
        }

        protected void GenerateEntity(Station entity, CreateVM model)
        {
            entity.Name = model.Name;
            entity.CityId = model.CityId;
            entity.Latitude = model.Latitude;
            entity.Longitude = model.Longitude;
        }

        protected void GenerateEntity(Station entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.CityId = model.CityId;
            entity.Longitude = model.Longitude;
            entity.Latitude = model.Latitude;
        }

        protected void GenerateModel(EditVM model, Station entity)
        {
            model.Name = entity.Name;
            model.CityId = entity.CityId;
            model.Longitude = entity.Longitude;
            model.Latitude = entity.Latitude;
        }

        protected void LoadExtraViewData()
        {
            StationsRepository stationsRepository = new StationsRepository();
            ViewData["cities"] = stationsRepository.GetCities().OrderBy(c => c.Name).ToList();
        }

        public virtual ActionResult Index()
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            StationsRepository repo = new StationsRepository();

            ViewData["items"] = repo.GetAll();
            LoadExtraViewData();
            return View();
        }

        public virtual ActionResult Create()
        {

            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            LoadExtraViewData();
            return View();
        }

        [HttpPost]
        public virtual ActionResult Create(CreateVM model)
        {

            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
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

            Station entity = new Station();
            StationsRepository repo = new StationsRepository();

            GenerateEntity(entity, model);

            repo.Add(entity);

            return RedirectToAction("Index");
        }

        public virtual ActionResult Edit(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            StationsRepository repo = new StationsRepository();
            Station entity = repo.GetById(id);
            EditVM model = new EditVM();

            model.Id = id;
            GenerateModel(model, entity);

            LoadExtraViewData();
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Edit(EditVM model)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
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

            Station entity = new Station();
            GenerateEntity(entity, model);
            StationsRepository repo = new StationsRepository();
            repo.Update(entity);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            StationsRepository repo = new StationsRepository();

            repo.Delete(id);
            return RedirectToAction("Index");
        }

    }
}