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
        protected void CheckIsModelValid(ListVM model)
        {
            if (DateTime.Compare(DateTime.Now.Date, model.DepartureDate) >= 0)
            {
                ModelState.AddModelError("InvalidDateError", "Invalid date.");
                return;
            }
        }
        protected override void CheckIsModelValid(CreateVM model)
        {
            SchedulesRepository repo = new SchedulesRepository();
            //if (model.Arrival == null)
            //{
            //    ModelState.AddModelError("AuthError", "Invalid arrival date!");
            //}

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

            //if (model.Arrival < model.Departure)
            //{
            //    ModelState.AddModelError("AuthError", "Arrival date must be after departure date!");
            //}

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
            TracksRepository tracksRepository = new TracksRepository();
            TimeSpan time = tracksRepository.CalculateTravelTime(model.TrackId, tracksRepository.GetStartStation(model.TrackId).Id, tracksRepository.GetEndStation(model.TrackId).Id);
            DateTime arrival = model.Departure + time;
            entity.Arrival = arrival;
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
                string startCity = stations.GetCity(tracks.GetStartStation(track.Id).CityId).Name;
                string endCity = stations.GetCity(tracks.GetEndStation(track.Id).CityId).Name;
                routes.Add(track.Id, startCity + " - " + endCity);
            }

            ViewData["routes"] = routes;
            ViewData["trains"] = trains.GetAll();
            ViewData["trainTypes"] = trains.GetTrainTypes();
            ViewData["tracks"] = tracks.GetAll();
            ViewData["cities"] = stations.GetCities().OrderBy(i => i.Name).ToList();
            ViewData["stations"] = stations.GetAll().OrderBy(i => i.Name).ToList();
            ViewData["items"] = new List<Schedule>();

            if (model != null && model.StartCityId != 0 
                              && model.EndCityId != 0 ) {
                if(DateTime.Compare(DateTime.Now.Date, model.DepartureDate) >= 0)
                {
                    List<WayStation> wayStations = tracks.GetWayStations();
                    int startStationId = stations.GetFirstOrDefault(s => s.CityId == model.StartCityId).Id;
                    int endStationId = stations.GetFirstOrDefault(s => s.CityId == model.EndCityId).Id;
                    List<Track> trackList = tracks.FindTracks(startStationId, endStationId);
                    if (trackList.Count == 0)
                    {
                        ViewData["items"] = new List<Schedule>();
                        ModelState.AddModelError("NoRecordsFound", "No departures found matching your criteria.");
                        return;
                    }
                    List<Schedule> schedulesList = new List<Schedule>();
                    foreach (var track in trackList)
                    {
                        schedulesList.AddRange(schedules.GetFilteredSchedules(track.Id, model.DepartureDate.Date, startStationId, endStationId)
                                                 .OrderBy(s => s.Departure.TimeOfDay)
                                                 .ToList());
                    }
                    ViewData["items"] = schedulesList;
                    if ( ( (List<Schedule>)ViewData["items"] ).Count == 0 )
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

        [HttpPost]
        public ActionResult WayStations(CreateVM model)
        {
            Session["scheduleCreateVM"] = model;
            WayStationsVM wayStationsVM = new WayStationsVM();
            TracksRepository tracksRepository = new TracksRepository();
            List<WayStation> wayStations = tracksRepository.GetWayStations(model.TrackId);
            wayStationsVM.WayStations = wayStations;

            return View(wayStationsVM);
        }
    }
}