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
    public class TrackController : BaseController
    {
        protected void CheckIsModelValid(CreateVM model)
        {
            if (model.WayStations == null || model.WayStations.Count < 2)
            {
                ModelState.AddModelError("AuthError", "Please select at least two stations!");
                return;
            }

            var hasDuplicate = model.WayStations.GroupBy(ws => ws).Any(g => g.Count() > 1);
            if (hasDuplicate)
            {
                ModelState.AddModelError("AuthError", "Duplicate stations found.");
                return;
            }
        }

        private void CheckIsModelValid(EditVM model)
        {

        }

        protected void GenerateEntity(Track entity, CreateVM model)
        {
            entity.Description = model.Description;
            List<WayStation> wayStations = new List<WayStation>();

            for (int i = 0; i < model.WayStations.Count; i++)
            {
                WayStation newWayStation = new WayStation()
                {
                    StationId = model.WayStations[i],
                    ConsecutiveNumber = i

                };
                wayStations.Add(newWayStation);
            }

            TracksRepository tracksRepository = new TracksRepository();
            tracksRepository.Add(entity, wayStations);
        }

        protected void GenerateEntity(Track entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.Description = model.Description;
        }

        protected void GenerateModel(EditVM model, Track entity)
        {
            model.Id = entity.Id;
            model.Description = entity.Description;
        }

        protected void LoadExtraViewData()
        {
            StationsRepository stations = new StationsRepository();
            ViewData["stations"] = stations.GetAll();
        }

        private void LoadExtraViewData(int id)
        {
            LoadExtraViewData();
            TracksRepository tracksRepository = new TracksRepository();
            StationsRepository stationsRepository = new StationsRepository();
            Dictionary<int, string> wayStations = new Dictionary<int, string>();
            var wsList = tracksRepository.GetWayStations(id);
            foreach (var item in wsList)
            {
                string stationName = stationsRepository.GetById(item.StationId).Name;
                wayStations.Add(item.ConsecutiveNumber + 1, stationName);
            }
            ViewData["wayStations"] = wayStations;
        }

        private List<ListItemVM> GetListItems(SearchVM model = null)
        {
            TracksRepository tracksRepository = new TracksRepository();
            StationsRepository stationsRepository = new StationsRepository();
            var routes = new List<ListItemVM>();
            List<Track> filteredTracks;
            if (model == null)
                filteredTracks = tracksRepository.GetAll();
            else
                filteredTracks = tracksRepository.FindTracks(model.StartStationId, model.EndStationId);

            foreach (var track in filteredTracks)
            {
                var match = tracksRepository.GetAsKeyValuePairs(t => t.Id == track.Id);
                if (match != null)
                {
                    var listItem = new ListItemVM()
                    {
                        Id = track.Id,
                        Value = match[track.Id],
                        WayStations = new List<WayStationVM>()
                    };

                    var wayStations = tracksRepository.GetWayStations(track.Id)
                                                      .OrderBy(ws => ws.ConsecutiveNumber)
                                                      .ToList();
                    foreach (var ws in wayStations)
                    {
                        listItem.WayStations.Add(new WayStationVM()
                        {
                            StationName = stationsRepository.GetFirstOrDefault(s => s.Id == ws.StationId).Name,
                            TrackId = track.Id
                        });
                    }

                    routes.Add(listItem);
                }

            }
            return routes;
        }

        public ActionResult Index()
        {
            if (!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            LoadExtraViewData();
            ViewData["routes"] = GetListItems(null);

            return View();
        }

        [HttpPost]
        public ActionResult Index(SearchVM model)
        {
            LoadExtraViewData();
            var routes = new List<ListItemVM>();
            if (model.StartStationId > 0 && model.EndStationId > 0)
            {
                ViewData["routes"] = GetListItems(model);
            }
            else
            {
                ModelState.AddModelError("InvalidStations", "Please select both stations of departure and arrival.");
                ViewData["routes"] = GetListItems(null);
            }
            return View(model);
        }

        public ActionResult Create()
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            LoadExtraViewData();
            CreateVM model = new CreateVM();
            model.WayStations = new List<int>();
            Session["trackCreateVM"] = model;

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CreateVM model)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            CheckIsModelValid(model);
            if(!ModelState.IsValid)
            {
                LoadExtraViewData();
                return View(model);
            }

            TracksRepository tracksRepository = new TracksRepository();
            Track track = new Track();
            GenerateEntity(track, model);
            //tracksRepository.Add();

            return RedirectToAction("Index", "Track");
        }

        public ActionResult Edit(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            LoadExtraViewData(id);
            TracksRepository tracksRepository = new TracksRepository();

            EditVM model = new EditVM();
            GenerateModel(model, tracksRepository.GetById(id));

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditVM model)
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
                LoadExtraViewData(model.Id);
                return View(model);
            }

            Track entity = new Track();
            GenerateEntity(entity, model);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            TracksRepository repo = new TracksRepository();
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            var schedule = schedulesRepository.GetFirstOrDefault(s => s.TrackId == id);
            if(schedule != null)
            {
                LoadExtraViewData(id);
                
                EditVM model = new EditVM();
                Track track = repo.GetFirstOrDefault(t => t.Id == id);
                GenerateModel(model, track);
                ModelState.AddModelError("DeleteError", "Cannot delete: Track is in use.");
                return View("Edit", model);
            }

            repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}