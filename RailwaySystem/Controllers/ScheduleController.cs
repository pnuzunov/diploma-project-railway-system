using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RailwaySystem.Repositories;
using RailwaySystem.Entities;
using RailwaySystem.ViewModels.Schedule;

namespace RailwaySystem.Controllers
{
    public class ScheduleController : BaseController<Schedule, SchedulesRepository, CreateVM, EditVM>
    {
        protected override void CheckIsModelValid(CreateVM model)
        {
            SchedulesRepository repo = new SchedulesRepository();
            if (model.Arrival == null)
            {
                ModelState.AddModelError("AuthError", "Invalid arrival date!");
            }

            if (model.Departure == null)
            {
                ModelState.AddModelError("AuthError", "Invalid departure date!");
            }

            if (model.Arrival < model.Departure)
            {
                ModelState.AddModelError("AuthError", "Arrival date must be after departure date!");
            }

            if (repo.GetFirstOrDefault(i =>
                                i.Departure == model.Departure &&
                                i.TrackId == model.TrackId) != null)
            {
                ModelState.AddModelError("AuthError", "Schedule cannot be created due to conflict!");
            }
        }

        protected override void CheckIsModelValid(EditVM model)
        {
            SchedulesRepository repo = new SchedulesRepository();
            if (model.Arrival == null)
            {
                ModelState.AddModelError("AuthError", "Invalid arrival date!");
            }

            if (model.Departure == null)
            {
                ModelState.AddModelError("AuthError", "Invalid departure date!");
            }

            if (model.Arrival < model.Departure)
            {
                ModelState.AddModelError("AuthError", "Arrival date must be after departure date!");
            }

            Schedule check = repo.GetFirstOrDefault(i =>
                                i.Departure == model.Departure && i.TrackId == model.TrackId && i.Id != model.Id);

            if (check != null)
            {
                ModelState.AddModelError("AuthError", "Schedule cannot be created due to conflict!");
            }
        }

        protected override void GenerateEntity(Schedule entity, CreateVM model)
        {
            entity.Arrival = model.Arrival;
            entity.Departure = model.Departure;
            entity.TrackId = model.TrackId;
            entity.TrainId = model.TrainId;
        }

        protected override void GenerateEntity(Schedule entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.Arrival = model.Arrival;
            entity.Departure = model.Departure;
            entity.TrackId = model.TrackId;
            entity.TrainId = model.TrainId;
        }

        protected override void GenerateModel(EditVM model, Schedule entity)
        {
            model.Departure = entity.Departure;
            model.Arrival = entity.Arrival;
            model.TrackId = entity.TrackId;
            model.TrainId = entity.TrainId;
        }

        protected void LoadFilteredData(ListVM model)
        {
            TrainsRepository trains = new TrainsRepository();
            TracksRepository tracks = new TracksRepository();
            StationsRepository stations = new StationsRepository();
            SchedulesRepository schedules = new SchedulesRepository();
            Dictionary<int, string> routes = new Dictionary<int, string>();

            foreach (var item in tracks.GetAll())
            {
                routes[item.Id] =
                    stations.GetFirstOrDefault(s => s.Id == item.StartStationId).Name
                    + " - " +
                    stations.GetFirstOrDefault(s => s.Id == item.EndStationId).Name;
            }
     
            ViewData["routes"] = routes;
            ViewData["trains"] = trains.GetAll();
            ViewData["trainTypes"] = trains.GetTrainTypes();
            ViewData["tracks"] = tracks.GetAll();
            ViewData["stations"] = stations.GetAll().OrderBy(i => i.Name).ToList();
            ViewData["items"] = schedules.GetAll();

            if (model != null && model.StartStationId != 0 && model.EndStationId != 0 ) {
                Track track = tracks.GetFirstOrDefault(tr => tr.StartStationId == model.StartStationId && tr.EndStationId == model.EndStationId);
                if (track == null)
                {
                    ViewData["items"] = new List<Schedule>();
                    return;
                }
                //ViewData["items"] = schedules.GetAll(i => i.TrackId == track.Id);
                List<Schedule> schedulesByTrack = schedules.GetAll(i => i.TrackId == track.Id);
                ViewData["items"] = new List<Schedule>();
                if (schedulesByTrack.Count > 0)
                {
                    schedulesByTrack = schedulesByTrack.Where(s => DateTime.Compare(s.Departure, model.DepartureDate) >= 0).ToList();
                    ViewData["items"] = schedulesByTrack;
                }
            }
        }

        protected override void LoadExtraViewData()
        {
            LoadFilteredData(null);
        }

        [HttpPost]
        public ActionResult Index(ListVM model)
        {
            LoadFilteredData(model);
            return View(model);
        }
    }
}