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
    public class StationController : BaseController<Station, StationsRepository, CreateVM, EditVM>
    {
        protected override void CheckIsModelValid(CreateVM model)
        {
            StationsRepository repo = new StationsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name) != null)
            {
                ModelState.AddModelError("AuthError", "Station already exists!");
            }
        }

        protected override void CheckIsModelValid(EditVM model)
        {
            StationsRepository repo = new StationsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name && i.Id != model.Id) != null)
            {
                ModelState.AddModelError("AuthError", "Station already exists!");
            }
        }

        protected override void GenerateEntity(Station entity, CreateVM model)
        {
            entity.Name = model.Name;
        }

        protected override void GenerateEntity(Station entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
        }

        protected override void GenerateModel(EditVM model, Station entity)
        {
            model.Name = entity.Name;
        }

    }
}