using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RailwaySystem.Repositories;
using RailwaySystem.Entities;
using RailwaySystem.ViewModels.Ticket;
using RailwaySystem.HelperClasses;
using PayPal.Api;

namespace RailwaySystem.Controllers
{
    public class TicketController : Controller
    {
        private bool CanAccessPage(BuyVM model)
        {
            User loggedUser = (User)Session["loggedUser"];

            if (loggedUser == null || model == null || model.UserId != loggedUser.Id)
            {
                return false;
            }
            return true;
        }

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
            if (model.SeatType == null)
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
            if (!CanAccessPage(model))
            {
                return RedirectToAction("Login", "Home");
            }

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

            if (!CanAccessPage(model))
            {
                Session["ticketBuyVM"] = null;
                return RedirectToAction("Login", "Home");
            }

            return View(model);
        }

        //[HttpPost]
        public ActionResult PayBySystemAccount()
        {
            BuyVM model = (BuyVM)Session["ticketBuyVM"];

            if (!CanAccessPage(model))
            {
                Session["ticketBuyVM"] = null;
                return RedirectToAction("Login", "Home");
            }

            TicketsRepository ticketsRepository = new TicketsRepository();
            Ticket ticket = new Ticket();
            BuildEntity(ticket, model);
            if(!ticketsRepository.ReserveTicket(ticket, model.Schedule, model.Seats, TicketsRepository.PaymentMethod.BY_SYSTEM_ACCOUNT))
            {
                ModelState.AddModelError("AuthError", "Ticket reservation failed.");
                return RedirectToAction("TicketOverview", "Ticket", new { id = model.Schedule.Id, dt = model.DepartureDate.Date.ToString("dd-MM-yyyy-HH-mm")});
            }
            Session["ticketBuyVM"] = null;
            return RedirectToAction("Index", "Ticket");
        }

        public ActionResult PayWithPayPal(string Cancel = null)
        {
            BuyVM model = (BuyVM)Session["ticketBuyVM"];
            if(!CanAccessPage(model))
            {
                return RedirectToAction("Index", "Ticket");
            }

            Ticket ticket = new Ticket();
            BuildEntity(ticket, model);

            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Ticket/PayWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    string redirectUrl = baseURI + "guid=" + guid;

                    PayPalPaymentBuilder paymentBuilder = new PayPalPaymentBuilder(apiContext, redirectUrl)
                        .AddItem(ticket);

                    var createdPayment = paymentBuilder.CreatePayment();
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = PayPalPaymentBuilder.ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        Session["BuyVMModelState"] = new KeyValuePair<String, String>("PayPalPaymentDeniedError", "There was an error in processing your request. Please try again.");
                        return RedirectToAction("TicketOverview", "Ticket");
                    }
                }
            }
            catch (Exception e)
            {
                Session["BuyVMModelState"] = new KeyValuePair<String, String>("PayPalPaymentDeniedError", "There was an error in processing your request. Please try again.");
                return RedirectToAction("TicketOverview", "Ticket");
            }
            Session["BuyVMModelState"] = null;

            TicketsRepository ticketsRepository = new TicketsRepository();
            ticketsRepository.ReserveTicket(ticket, model.Schedule, model.Seats, TicketsRepository.PaymentMethod.BY_PAY_PAL);

            return RedirectToAction("Index", "Ticket");
        }


        public ActionResult DownloadResource(int id)
        {
            TicketsRepository ticketsRepository = new TicketsRepository();
            Ticket ticket = ticketsRepository.GetById(id);

            if (ticket == null)
            {
                return RedirectToAction("Index", "Ticket");
            }

            BuyVM model = new BuyVM() { UserId = ticket.UserId };
            if (!CanAccessPage(model))
            {
                return RedirectToAction("Index", "Home");
            }

            TicketPdfBuilder ticketPdf = new TicketPdfBuilder();

            return new FileContentResult(ticketPdf.GeneratePdf(ticket), "application/pdf");
        }

    }
}