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
    public class TrainController : Controller
    {
        protected void CheckIsModelValid(CreateVM model)
        {
            TrainsRepository repo = new TrainsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name) != null)
            {
                ModelState.AddModelError("AuthError", "Train already exists!");
            }
            if(model.SeatsFirstClass < 0 || model.SeatsSecondClass <= 0)
            {
                ModelState.AddModelError("AuthError", "Insufficient seat count.");
            }
        }

        protected void CheckIsModelValid(EditVM model)
        {
            TrainsRepository repo = new TrainsRepository();
            Train check = repo.GetFirstOrDefault(i => i.Name == model.Name && i.Id != model.Id);
            if (check != null)
            {
                ModelState.AddModelError("AuthError", "Train already exists!");
            }
        }

        protected void GenerateEntities(Train train, List<Seat> seats, CreateVM model)
        {
            TrainsRepository trainsRepository = new TrainsRepository();
            List<SeatType> seatTypes = trainsRepository.GetSeatTypes();
            train.Name = model.Name;
            train.TypeId = model.TypeId;

            int seatNum = 1;
            for(int i = 0; i < model.SeatsFirstClass; i++)
            {
                seats.Add(new Seat()
                {
                    Reserved = false,
                    SeatNumber = (i + 1),
                    SeatTypeId = seatTypes.Where(st => st.Name == TrainsRepository.FIRST_CLASS).FirstOrDefault().Id,
                });
                seatNum++;
            }
            for (int i = (seatNum-1); i < model.SeatsSecondClass + seatNum; i++)
            {
                seats.Add(new Seat()
                {
                    Reserved = false,
                    SeatNumber = (i + 1),
                    SeatTypeId = seatTypes.Where(st => st.Name == TrainsRepository.SECOND_CLASS).FirstOrDefault().Id,
                });
            }
        }

        protected void GenerateEntity(Train entity, EditVM model)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.TypeId = model.TypeId;
        }

        protected void GenerateModel(EditVM model, Train entity)
        {
            model.Name = entity.Name;
            model.TypeId = entity.TypeId;
        }

        public ActionResult Index()
        {
            TrainsRepository trainsRepository = new TrainsRepository();
            ViewData["items"] = trainsRepository.GetAll();

            LoadExtraViewData();
            return View();
        }

        public ActionResult Create()
        {
            CreateVM model = new CreateVM();
            LoadExtraViewData();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CreateVM model)
        {
            if (ModelState.IsValid)
            {
                CheckIsModelValid(model);
            }

            if (!ModelState.IsValid)
            {
                LoadExtraViewData();
                return View(model);
            }
            Train train = new Train();
            List<Seat> seats = new List<Seat>();

            GenerateEntities(train, seats, model);

            TrainsRepository trainsRepository = new TrainsRepository();
            trainsRepository.Add(train, seats);

            return RedirectToAction("Index", "Train");
        }

        public ActionResult Delete(int id)
        {

            TrainsRepository trainsRepository = new TrainsRepository();

            trainsRepository.DeleteCascade(id);
            return RedirectToAction("Index", "Train");
        }

        protected void LoadExtraViewData()
        {
            TrainsRepository trainsRepo = new TrainsRepository();
            ViewData["trainTypes"] = trainsRepo.GetTrainTypes();
        }
    }
}