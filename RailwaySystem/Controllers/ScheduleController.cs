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
    public class ScheduleController : Controller
    {
        protected void CheckIsModelValid(ListVM model)
        {

        }
        protected void CheckIsModelValid(CreateVM model)
        {

        }

        protected void GenerateEntity(Schedule entity, CreateVM model)
        {
            entity.TrackId = model.TrackId;
            entity.TrainId = model.TrainId;
            entity.ScheduleModeId = model.ScheduleMode;
            entity.PricePerTicket = model.PricePerTicket;
        }

        void GenerateEntities(List<Schedule> schedules, EditVM model)
        {
            foreach (var item in schedules)
            {
                item.TrainId = model.TrainId;
                item.Cancelled = model.Cancelled;
            }
        }

        protected void LoadFilteredData(ListVM model)
        {
            TrainsRepository trainsRepository = new TrainsRepository();
            TracksRepository tracksRepository = new TracksRepository();
            StationsRepository stationsRepository = new StationsRepository();
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            Dictionary<int, string> routes = new Dictionary<int, string>();

            foreach (var track in tracksRepository.GetAll())
            {
                string startStation = stationsRepository.GetById(model.StartStationId).Name;
                string endStation = stationsRepository.GetById(model.EndStationId).Name;
                string description = tracksRepository.GetById(track.Id).Description;
                string value = startStation + " - " + endStation + ((description == null || "".Equals(description)) ? "" : " (" + description + ")");
                routes.Add(track.Id, value);
            }

            ViewData["routes"] = routes;
            ViewData["trains"] = trainsRepository.GetAll();
            ViewData["trainTypes"] = trainsRepository.GetTrainTypes();
            ViewData["tracks"] = tracksRepository.GetAll();
            ViewData["stations"] = stationsRepository.GetAll().OrderBy(i => i.Name).ToList();

            ViewData["items"] = new List<ListItemVM>();

            var trains = trainsRepository.GetAll();
            var trainTypes = trainsRepository.GetTrainTypes();
            var items = (List<ListItemVM>)ViewData["items"];

            if (model != null && model.StartStationId != 0 
                              && model.EndStationId != 0 ) {
                    List<Track> trackList = tracksRepository.FindTracks(model.StartStationId, model.EndStationId);
                    if (trackList.Count == 0)
                    {
                        items = new List<ListItemVM>();
                        ModelState.AddModelError("NoRecordsFound", "No departures found matching your criteria.");
                        return;
                    }
                    List<Schedule> schedulesList = new List<Schedule>();
                    foreach (var track in trackList)
                    {
                        schedulesList.AddRange(schedulesRepository
                                                 .GetFilteredSchedules(track.Id, 
                                                                       model.StartStationId, 
                                                                       model.DepartureDate, 
                                                                       SchedulesRepository.DateCompareMode.SAME_DATE)
                                                 .ToList());
                    }

                foreach (var schedule in schedulesList)
                {
                    var listItem = new ListItemVM();
                    listItem.Departure = schedulesRepository.GetDepartureDate(schedule.Id, model.StartStationId);
                    listItem.Arrival = schedulesRepository.GetArrivalDate(schedule.Id, model.EndStationId);
                    listItem.Route = routes.FirstOrDefault(r => r.Key == schedule.TrackId).Value;
                    listItem.Schedule = schedule;
                    Train train = trains.FirstOrDefault(t => t.Id == schedule.TrainId);
                    TrainType trainType = trainTypes.FirstOrDefault(tt => tt.Id == train.TypeId);
                    listItem.Train = train.Name + "(" + trainType.Name + ")";

                    listItem.ScheduledWayStations = new List<SwsVM>();
                    List<WayStation> wayStations = tracksRepository.GetWayStations(listItem.Schedule.TrackId);
                    foreach (var ws in wayStations)
                    {
                        string name = stationsRepository.GetById(ws.StationId).Name;
                        ScheduledWayStation scheduledWayStation = schedulesRepository.GetScheduledWayStation(schedule, ws.StationId);
                        listItem.ScheduledWayStations.Add(new SwsVM()
                        {
                            StationName = name,
                            Arrival = scheduledWayStation.Arrival,
                            Departure = scheduledWayStation.Departure
                        });
                    }
                    listItem.ScheduledWayStations = listItem.ScheduledWayStations.OrderBy(sws => sws.Arrival).ToList();

                    items.Add(listItem);
                }

                    if ( items.Count == 0 )
                    {
                        ModelState.AddModelError("NoRecordsFound", "No departures found matching your criteria.");
                    }
                    else
                    {
                        items = items.OrderBy(i => i.Departure).ToList();
                        ViewData["items"] = items;
                    }
            }
        }

        protected void LoadExtraViewData()
        {
            LoadFilteredData(null);
        }

        public ActionResult Index()
        {
            StationsRepository stationsRepository = new StationsRepository();
            ViewData["stations"] = stationsRepository.GetAll().OrderBy(i => i.Name).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Index(ListVM model)
        {
            CheckIsModelValid(model);
            if (!ModelState.IsValid)
            {
                StationsRepository stationsRepository = new StationsRepository();
                ViewData["stations"] = stationsRepository.GetAll().OrderBy(i => i.Name).ToList();
                return View();
            }
            LoadFilteredData(model);
            return View(model);
        }

        public ActionResult Create()
        {
            LoadExtraViewData();
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateVM model)
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

            Session["scheduleCreateVM"] = model;
            return RedirectToAction("SetWayStations");
        }

        public ActionResult SetWayStations()
        {
            CreateVM model = (CreateVM)Session["scheduleCreateVM"];
            TracksRepository tracksRepository = new TracksRepository();
            StationsRepository stationsRepository = new StationsRepository();
            List<WayStation> wayStations = tracksRepository.GetWayStations(model.TrackId);
            foreach (var ws in wayStations)
            {
                ws.Station = stationsRepository.GetById(ws.StationId);
            }
            model.WayStations = wayStations;

            return View(model);
        }

        [HttpPost]
        public ActionResult SetWayStations(CreateVM model)
        {
            CreateVM savedModel = (CreateVM)Session["scheduleCreateVM"];
            model.LastDateToCreate = savedModel.LastDateToCreate;
            model.PricePerTicket = savedModel.PricePerTicket;
            model.ScheduleMode = savedModel.ScheduleMode;
            model.TrackId = savedModel.TrackId;
            model.TrainId = savedModel.TrainId;
            model.DepartDate = savedModel.DepartDate;
            model.WayStations = savedModel.WayStations;

            List<ScheduledWayStation> scheduledWS = new List<ScheduledWayStation>();

            for(int i = 0; i < model.Arrivals.Count; i++)
            {
                model.Arrivals[i] = new DateTime(model.DepartDate.Ticks).Date.AddHours(model.Arrivals[i].Hour).AddMinutes(model.Arrivals[i].Minute);
                model.Departures[i] = new DateTime(model.DepartDate.Ticks).Date.AddHours(model.Departures[i].Hour).AddMinutes(model.Departures[i].Minute);
            }

            for (int i = 0; i < model.WayStations.Count; i++)
            {
                ScheduledWayStation newSWS = new ScheduledWayStation();
                newSWS.WayStationId = model.WayStations[i].Id;
                if(i != 0)
                {
                    newSWS.Arrival = model.Arrivals[i - 1];
                }
                else
                {
                    newSWS.Arrival = new DateTime();
                }
                if(i != model.WayStations.Count - 1)
                {
                    newSWS.Departure = model.Departures[i];
                }
                else
                {
                    newSWS.Departure = new DateTime();
                }
                scheduledWS.Add(newSWS);
            }

            Schedule schedule = new Schedule();
            GenerateEntity(schedule, model);

            SchedulesRepository schedulesRepository = new SchedulesRepository();
            schedulesRepository.Add(schedule, model.DepartDate, model.LastDateToCreate, scheduledWS);

            Session["scheduleCreateVM"] = null;
            return RedirectToAction("Index", "Schedule");
        }

        public ActionResult Edit(int id)
        {
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            TracksRepository tracksRepository = new TracksRepository();
            TrainsRepository trainsRepository = new TrainsRepository();
            Schedule schedule = schedulesRepository.GetById(id);
            Track track = tracksRepository.GetById(schedule.TrackId);
            Station start = tracksRepository.GetStartStation(track.Id);
            EditVM model = new EditVM();

            model.Id = schedule.Id;
            model.TrainId = schedule.TrainId;
            model.DepartDate = schedulesRepository.GetDepartureDate(id, start.Id);
            model.Cancelled = schedule.Cancelled;

            ViewData["trains"] = trainsRepository.GetAll();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditVM model)
        {
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            List<Schedule> schedules = new List<Schedule>();

            switch (model.EditOption)
            {
                case (EditVM.EditOptions.ONLY_THIS_ENTRY):
                    schedules.Add(schedulesRepository.GetById(model.Id));
                    break;
                case (EditVM.EditOptions.BY_DEFINED_PERIOD):
                    schedules.AddRange(schedulesRepository
                                            .GetFilteredSchedules(model.Id,
                                                                  model.LastDateToApply,
                                                                  SchedulesRepository.DateCompareMode.BEFORE));
                    break;
                case (EditVM.EditOptions.ALL_MATCHING_ENTRIES):
                    schedules.AddRange(schedulesRepository
                        .GetFilteredSchedules(model.Id,
                                              new DateTime(9999, 12, 31),
                                              SchedulesRepository.DateCompareMode.BEFORE));
                    break;
                default: break;
            }

            GenerateEntities(schedules, model);
            schedulesRepository.Update(schedules);

            return RedirectToAction("Index", "Schedule");
        }

    }
}