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
            if (model.WayStations == null || model.WayStations.Count < 2)
            {
                ModelState.AddModelError("AuthError", "Please select at least two stations!");
                return;
            }

            foreach (var pc in model.PriceChanges)
            {
                if(pc < 0.0m)
                {
                    ModelState.AddModelError("AuthError", "Invalid price change!");
                    return;
                }
            }

            foreach (var min in model.MinutesToArrive)
            {
                if(min < 0)
                {
                    ModelState.AddModelError("AuthError", "Incorrectly assigned time!");
                    return;
                }
            }

            var hasDuplicate = model.WayStations.GroupBy(ws => ws).Any(g => g.Count() > 1);
            if (hasDuplicate)
            {
                ModelState.AddModelError("AuthError", "Duplicate stations found.");
                return;
            }
        }

        protected override void CheckIsModelValid(EditVM model)
        {

        }

        protected override void GenerateEntity(Track entity, CreateVM model)
        {
            List<WayStation> wayStations = new List<WayStation>();

            //foreach(var ws in model.WayStations)
            //{
            //    WayStation newWayStation = new WayStation()
            //    {
            //        StationId = ws,

            //    };
            //    wayStations.Add(newWayStation);
            //}

            for (int i = 0; i < model.WayStations.Count; i++)
            {
                WayStation newWayStation = new WayStation()
                {
                    StationId = model.WayStations[i],
                    MinutesToArrive = model.MinutesToArrive[i],
                    TicketPriceChange = model.PriceChanges[i],
                    ConsecutiveNumber = i

                };
                wayStations.Add(newWayStation);
            }

            TracksRepository tracksRepository = new TracksRepository();
            tracksRepository.Add(entity, wayStations);
        }

        protected override void GenerateEntity(Track entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.StandardTicketPrice = model.StandardTicketPrice;
        }

        protected override void GenerateModel(EditVM model, Track entity)
        {
            List<WayStation> wayStations = new List<WayStation>();

            foreach (var ws in model.WayStations)
            {
                WayStation newWayStation = new WayStation()
                {
                    StationId = ws
                };
                wayStations.Add(newWayStation);
            }
        }

        protected override void LoadExtraViewData()
        {
            StationsRepository stations = new StationsRepository();
            ViewData["stations"] = stations.GetAll();
        }

        public override ActionResult Create()
        {
            LoadExtraViewData();
            CreateVM model = new CreateVM();
            model.WayStations = new List<int>();
            Session["trackCreateVM"] = model;

            return View(model);
        }

        [HttpPost]
        public override ActionResult Create(CreateVM model)
        {
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

        public override ActionResult Edit(int id)
        {
            StationsRepository stations = new StationsRepository();
            TracksRepository tracksRepository = new TracksRepository();
            ViewData["stations"] = stations.GetAll();
            ViewData["wayStations"] = tracksRepository.GetWayStations(id);
            EditVM model = new EditVM();
            GenerateModel(model, tracksRepository.GetById(id));

            return View(model);
        }
    }
}