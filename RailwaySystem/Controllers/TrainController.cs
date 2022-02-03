using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using RailwaySystem.ViewModels.Train;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public class TrainController : BaseController<Train, TrainsRepository, CreateVM, EditVM>
    {
        protected override void CheckIsModelValid(CreateVM model)
        {
            TrainsRepository repo = new TrainsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name) != null)
            {
                ModelState.AddModelError("AuthError", "Train already exists!");
            }
        }

        protected override void CheckIsModelValid(EditVM model)
        {
            TrainsRepository repo = new TrainsRepository();
            Train check = repo.GetFirstOrDefault(i => i.Name == model.Name && i.Id != model.Id);
            if (check != null)
            {
                ModelState.AddModelError("AuthError", "Train already exists!");
            }
        }

        protected override void GenerateEntity(Train entity, CreateVM model)
        {
            entity.Name = model.Name;
            entity.TypeId = model.Type;
        }

        protected override void GenerateEntity(Train entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.TypeId = model.Type;
        }

        protected override void GenerateModel(EditVM model, Train entity)
        {
            model.Name = entity.Name;
            model.Type = entity.TypeId;
        }
    }
}