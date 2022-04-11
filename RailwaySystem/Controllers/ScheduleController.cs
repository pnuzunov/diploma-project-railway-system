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

            if (model.Departure > model.LastDateToCreate)
            {
                ModelState.AddModelError("AuthError", "The last scheduled date must be on or after the departure date.");
            }

            if(model.ScheduleMode == 0)
            {
                ModelState.AddModelError("AuthError", "Invalid schedule type.");
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
            entity.ScheduleModeId = model.ScheduleMode;
            entity.PricePerTicket = model.PricePerTicket;
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

            foreach(var track in tracks.GetAll())
            {
                string startStation = stations.GetById(track.StartStationId).Name;
                string endStation = stations.GetById(track.EndStationId).Name;
                routes.Add(track.Id, startStation + " - " + endStation);
            }

            ViewData["routes"] = routes;
            ViewData["trains"] = trains.GetAll();
            ViewData["trainTypes"] = trains.GetTrainTypes();
            ViewData["tracks"] = tracks.GetAll();
            ViewData["stations"] = stations.GetAll().OrderBy(i => i.Name).ToList();
            ViewData["items"] = new List<Schedule>();

            if (model != null && model.StartStationId != 0 
                              && model.EndStationId != 0 ) {
                if(DateTime.Compare(DateTime.Now.Date, model.DepartureDate) <= 0)
                {
                    Track track = tracks.GetFirstOrDefault(tr => tr.StartStationId == model.StartStationId 
                                                              && tr.EndStationId == model.EndStationId);
                    if (track == null)
                    {
                        ViewData["items"] = new List<Schedule>();
                        ModelState.AddModelError("NoRecordsFound", "No departures found matching your criteria.");
                        return;
                    }
                    ViewData["items"] = schedules.GetFilteredSchedules(track.Id, model.DepartureDate.Date)
                                                 .OrderBy(s => s.Departure.TimeOfDay)
                                                 .ToList();
                    if( ( (List<Schedule>)ViewData["items"] ).Count == 0 )
                    {
                        ModelState.AddModelError("NoRecordsFound", "No departures found matching your criteria.");
                    }
                    else
                    {
                        foreach (var schedule in (List<Schedule>)ViewData["items"])
                        {
                            ViewData["freeSeats" + schedule.Id] = trains.GetNonReservedSeats(schedule.Id, schedule.TrainId, getAll: true);
                        }
                    }
                }
            }
        }

        protected override void LoadExtraViewData()
        {
            LoadFilteredData(null);
        }

        public override ActionResult Index()
        {
            LoadExtraViewData();
            return View();
        }

        [HttpPost]
        public ActionResult Index(ListVM model)
        {
            LoadFilteredData(model);
            return View(model);
        }

        [HttpPost]
        public override ActionResult Create(CreateVM model)
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

            Schedule entity = new Schedule();
            SchedulesRepository repo = new SchedulesRepository();

            GenerateEntity(entity, model);

            repo.Add(entity, model.LastDateToCreate);

            return RedirectToAction("Index");
        }
    }
}