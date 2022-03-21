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
        private bool GenerateModel(int scheduleId, DateTime departure, BuyVM model)
        {
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            TrainsRepository trainsRepository = new TrainsRepository();

            Schedule schedule = schedulesRepository.GetById(scheduleId);
            Train train = schedulesRepository.GetTrain(scheduleId);

            if (schedule == null || train == null)
            {
                return false;
            }

            string startStation = schedulesRepository.GetStartStation(scheduleId)?.Name;
            string endStation = schedulesRepository.GetEndStation(scheduleId)?.Name;
            
            TrainType trainType = trainsRepository.GetTrainType(train.Id);
            if (trainType == null)
            {
                return false;
            }

            model.Schedule = schedule;
            model.UserId = ((User)Session["loggedUser"]).Id;
            model.StartStationName = startStation;
            model.EndStationName = endStation;
            model.DepartureDate = departure;
            model.TrainName = train.Name;
            model.TrainType = trainType.Name;
            model.Price = schedule.PricePerTicket;
            return true;
        }

        private void CheckModelValid(BuyVM model)
        {
            if ("".Equals(model.SeatType.Trim()))
                ModelState.AddModelError("AuthError", "Please select a seat type.");
            
            if (model.Quantity <= 0)
                ModelState.AddModelError("AuthError", "Invalid number of seats.");

            TrainsRepository trainsRepository = new TrainsRepository();
            Train train = trainsRepository.GetFirstOrDefault(t => t.Name == model.TrainName);
            List<Seat> seats = trainsRepository.GetNonReservedSeats(model.Quantity, model.Schedule.Id);
            //seats = seats.Where(s => s.SeatType == model.SeatType).ToList();

            if(model.Quantity > seats.Count)
            {
                ModelState.AddModelError("AuthError", "There are not enough seats for this purchase.");
            }
        }

        private void BuildEntity(Ticket ticket, BuyVM model)
        {
            ticket.UserId = ((User)Session["loggedUser"]).Id;
            ticket.BeginStation = model.StartStationName;
            ticket.EndStation = model.EndStationName;
            ticket.Departure = model.DepartureDate;
            ticket.Price = model.Price;
            ticket.Quantity = model.Quantity;
            ticket.SeatType = model.SeatType;
            ticket.TrainName = model.TrainName;
            ticket.TrainType = model.TrainType;

            TrainsRepository trainsRepository = new TrainsRepository();
            List<Seat> seats = trainsRepository.GetNonReservedSeats(model.Quantity, model.Schedule.Id);
            foreach(var seat in seats)
            {
                ticket.SeatNumbers += "|" + seat.SeatNumber;
            }
            ticket.SeatNumbers += "|";
            model.Seats = seats;
        }

        public ActionResult Index()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            User loggedUser = (User)Session["loggedUser"];
            TicketsRepository ticketsRepository = new TicketsRepository();
            ViewData["items"] = ticketsRepository.GetAll(ticket => ticket.UserId == loggedUser.Id);
            return View();
        }

        public ActionResult Buy(int id, String dt)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            String[] tokens = dt.Split('-');

            DateTime dateTime = new DateTime(int.Parse(tokens[2]), int.Parse(tokens[1]), int.Parse(tokens[0]), int.Parse(tokens[3]), int.Parse(tokens[4]), 0);

            BuyVM model = new BuyVM();
            if(!GenerateModel(id, dateTime, model))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["route"] = model.StartStationName + " - " + model.EndStationName;
            return View(model);
        }

        [HttpPost]
        public ActionResult Buy(BuyVM model)
        {
            CheckModelValid(model);
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            Session["ticketBuyVM"] = model;
            return RedirectToAction("TicketOverview", "Ticket");
        }

        public ActionResult TicketOverview()
        {
            BuyVM model = (BuyVM)Session["ticketBuyVM"];
            return View(model);
        }

        [HttpPost]
        public ActionResult ConfirmBuy()
        {
            BuyVM model = (BuyVM)Session["ticketBuyVM"];
            TicketsRepository ticketsRepository = new TicketsRepository();
            Ticket ticket = new Ticket();
            BuildEntity(ticket, model);
            if(!ticketsRepository.ReserveTicket(ticket, model.Schedule, model.Seats))
            {
                ModelState.AddModelError("AuthError", "Ticket reservation failed.");
            }
            return RedirectToAction("Index", "Ticket");
        }
    }
}