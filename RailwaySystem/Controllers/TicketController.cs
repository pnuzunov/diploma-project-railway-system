using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RailwaySystem.Repositories;
using RailwaySystem.Entities;
using RailwaySystem.ViewModels.Ticket;

namespace RailwaySystem.Controllers
{
    public class TicketController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public bool GenerateModel(int scheduleId, BuyVM model)
        {
            SchedulesRepository schedules = new SchedulesRepository();
            TrainsRepository trains = new TrainsRepository();
            Schedule schedule = schedules.GetById(scheduleId);
            if (schedule == null)
            {
                return false;
            }
            string startStation = schedules.GetStartStation(scheduleId)?.Name;
            string endStation = schedules.GetEndStation(scheduleId)?.Name;

            model.StartStationName = startStation;
            model.EndStationName = endStation;
            model.DepartureDate = schedule.Departure;
            model.UserId = ((User)Session["loggedUser"]).Id;
            Train train = schedules.GetTrain(scheduleId);

            model.TrainName = train?.Name;
            model.TrainType = trains.GetTrainType(train.Id)?.Name;
            return true;
        }

        public ActionResult Buy(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            BuyVM model = new BuyVM();
            if(!GenerateModel(id, model))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["route"] = model.StartStationName + " - " + model.EndStationName;

            return View(model);
        }

        [HttpPost]
        public ActionResult Buy()
        {
            return RedirectToAction("Index", "Ticket");
        }
    }
}