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

        //protected void CheckIsModelValid(EditVM model)
        //{
        //    TracksRepository tracksRepository = new TracksRepository();
        //    SchedulesRepository schedulesRepository = new SchedulesRepository();
        //    Track originalTrack = tracksRepository.GetById(model.Id);
        //    Schedule schedule = schedulesRepository.GetFirstOrDefault(s => s.TrackId == originalTrack.Id);
        //    List<WayStation> wayStations = tracksRepository.GetWayStations(originalTrack.Id);
        //    wayStations = wayStations.OrderBy(ws => ws.ConsecutiveNumber).ToList();

        //    if(schedule != null)
        //    {
        //        for(int i = 0; i < model.WayStations.Count; i++)
        //        {
        //            var ws = tracksRepository.GetWayStationByConsecNumber(originalTrack.Id, i);
        //            if(ws == null || ws.StationId != model.WayStations[i])
        //            {
        //                ModelState.AddModelError("CannotModifyWS", "This track is already in use and the way stations cannot be modified.");
        //            }
        //        }
        //    }

        //}

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

        //protected void GenerateEntity(Track entity, EditVM model)
        //{
        //    entity.Id = model.Id;
        //    entity.Description = model.Description;

        //    TracksRepository repo = new TracksRepository();
        //    repo.Update(entity);
        //}

        //protected void GenerateModel(EditVM model, Track entity)
        //{
        //    TracksRepository tracksRepository = new TracksRepository();
        //    List<WayStation> wayStations = new List<WayStation>();
        //    wayStations = tracksRepository.GetWayStations(entity.Id).OrderBy(ws => ws.ConsecutiveNumber).ToList();
        //    model.WayStations = new List<int>();
        //    for(int i = 0; i < wayStations.Count; i++)
        //    {
        //        model.WayStations.Add(wayStations[i].StationId);
        //    }
        //    model.Id = entity.Id;
        //    model.Description = entity.Description;
        //}

        protected void LoadExtraViewData()
        {
            StationsRepository stations = new StationsRepository();
            ViewData["stations"] = stations.GetAll();
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

        //public ActionResult Edit(int id)
        //{
        //    if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    //SchedulesRepository schedulesRepository = new SchedulesRepository();
        //    //if(schedulesRepository.GetFirstOrDefault(s => s.TrackId == id) != null)
        //    //{
        //    //    return RedirectToAction("Index", "Track");
        //    //}

        //    StationsRepository stations = new StationsRepository();
        //    TracksRepository tracksRepository = new TracksRepository();
            
        //    ViewData["stations"] = stations.GetAll();
        //    ViewData["wayStations"] = tracksRepository.GetWayStations(id);
        //    EditVM model = new EditVM();
        //    GenerateModel(model, tracksRepository.GetById(id));

        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Edit(EditVM model)
        //{
        //    if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        CheckIsModelValid(model);
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        LoadExtraViewData();
        //        return View(model);
        //    }

        //    Track entity = new Track();
        //    GenerateEntity(entity, model);

        //    return RedirectToAction("Index");
        //}

        public ActionResult Delete(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            TracksRepository repo = new TracksRepository();

            repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}