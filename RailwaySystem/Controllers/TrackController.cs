using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using RailwaySystem.ViewModels.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RailwaySystem.Controllers
{
    public class TrackController : BaseController<Track, TracksRepository, CreateVM, EditVM>
    {
        protected override void CheckIsModelValid(CreateVM model)
        {
            TracksRepository repo = new TracksRepository();
            if (model.StartStationId == model.EndStationId)
            {
                ModelState.AddModelError("AuthError", "Track must have different start and end stations!");
            }

            if (
                repo.GetFirstOrDefault(i => i.StartStationId == model.StartStationId && i.EndStationId == model.EndStationId) != null)
            {
                ModelState.AddModelError("AuthError", "Track already exists!");
            }
        }

        protected override void CheckIsModelValid(EditVM model)
        {
            TracksRepository repo = new TracksRepository();
            if (model.StartStationId == model.EndStationId)
            {
                ModelState.AddModelError("AuthError", "Track must have different start and end stations!");
            }

            Track check = repo.GetFirstOrDefault(i =>
               (
                (i.StartStationId == model.StartStationId && i.EndStationId == model.EndStationId) && i.Id != model.Id));
            if (check != null)
            {
                ModelState.AddModelError("AuthError", "Track already exists!");
            }
        }

        protected override void GenerateEntity(Track entity, CreateVM model)
        {
            entity.StartStationId = model.StartStationId;
            entity.EndStationId = model.EndStationId;
        }

        protected override void GenerateEntity(Track entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.StartStationId = model.StartStationId;
            entity.EndStationId = model.EndStationId;
        }

        protected override void GenerateModel(EditVM model, Track entity)
        {
            model.StartStationId = entity.StartStationId;
            model.EndStationId = entity.EndStationId;
        }

        protected override void LoadExtraViewData()
        {
            StationsRepository stations = new StationsRepository();
            ViewData["stations"] = stations.GetAll();
        }

    }
}