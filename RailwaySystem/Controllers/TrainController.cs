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
    public class TrainController : BaseController<Train, SearchVM, CreateVM, EditVM>
    {
        protected void CheckIsModelValid(CreateVM model)
        {
            TrainsRepository repo = new TrainsRepository();
            if (repo.GetFirstOrDefault(i => i.Name == model.Name) != null)
            {
                ModelState.AddModelError("CreateError", "Train already exists!");
            }
            if(model.SeatsFirstClass < 0 || model.RegularSeats <= 0)
            {
                ModelState.AddModelError("CreateError", "Insufficient seat count.");
            }
        }

        protected void CheckIsModelValid(EditVM model)
        {
            TrainsRepository repo = new TrainsRepository();
            Train check = repo.GetFirstOrDefault(i => i.Name == model.Name && i.Id != model.Id);
            if (check != null)
            {
                ModelState.AddModelError("EditError", "Train already exists!");
            }
        }

        protected void GenerateEntities(Train train, List<Seat> seats, CreateVM model)
        {
            TrainsRepository trainsRepository = new TrainsRepository();
            train.Name = model.Name;
            train.TypeId = model.TypeId;

            int seatNum = 1;
            for(int i = 0; i < model.SeatsFirstClass; i++)
            {
                seats.Add(new Seat()
                {
                    SeatNumber = (i + 1),
                    IsFirstClass = true
                });
                seatNum++;
            }
            for (int i = (seatNum-1); i < model.RegularSeats + (seatNum-1); i++)
            {
                seats.Add(new Seat()
                {
                    SeatNumber = (i + 1),
                    IsFirstClass = false
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
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.TypeId = entity.TypeId;
        }

        public override ActionResult Index()
        {
            if (!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            TrainsRepository trainsRepository = new TrainsRepository();
            ViewData["items"] = trainsRepository.GetAll();

            LoadExtraViewData();
            return View();
        }

        [HttpPost]
        public override ActionResult Index(SearchVM model)
        {
            if (!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }
            if (model.Name == null) model.Name = "";
            TrainsRepository repo = new TrainsRepository();
            List<Train> trains = repo.GetAll(s => s.Name.Contains(model.Name) || s.Name.Equals(model.Name));
            ViewData["items"] = trains;
            if (trains.Count == 0)
            {
                ModelState.AddModelError("NoRecordsFound", "No results matches your criteria.");
            }

            return View(model);
        }

        public override ActionResult Create()
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Login", "Home");
            }

            CreateVM model = new CreateVM();
            LoadExtraViewData();
            return View(model);
        }

        [HttpPost]
        public override ActionResult Create(CreateVM model)
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

        public override ActionResult Edit(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Index", "Train");
            }

            LoadExtraViewData();
            TrainsRepository trainsRepository = new TrainsRepository();
            Train train = trainsRepository.GetById(id);
            EditVM model = new EditVM();
            GenerateModel(model, train);

            return View(model);
        }

        [HttpPost]
        public override ActionResult Edit(EditVM model)
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
                LoadExtraViewData();
                return View(model);
            }

            TrainsRepository trainsRepository = new TrainsRepository();
            Train train = new Train();
            GenerateEntity(train, model);
            trainsRepository.Update(train);
            return RedirectToAction("Index", "Train");
        }

        public ActionResult Delete(int id)
        {

            if (!CanAccessPage(UsersRepository.Levels.FULL_ACCESS))
            {
                return RedirectToAction("Index", "Train");
            }

            TrainsRepository trainsRepository = new TrainsRepository();
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            Train train = trainsRepository.GetById(id);
            if(train == null)
            {
                return RedirectToAction("Index", "Train");
            }    
            Schedule schedule = schedulesRepository.GetFirstOrDefault(s => s.TrainId == id);
            if(schedule != null)
            {
                LoadExtraViewData();
                ModelState.AddModelError("DeleteError", "Cannot delete: This train is in use.");
                EditVM model = new EditVM();
                GenerateModel(model, train);
                return View("Edit", model);
            }

            trainsRepository.DeleteCascade(id);
            return RedirectToAction("Index", "Train");
        }

        private void LoadExtraViewData()
        {
            TrainsRepository trainsRepo = new TrainsRepository();
            ViewData["trainTypes"] = trainsRepo.GetTrainTypes();
        }
    }
}