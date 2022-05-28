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

        private bool GenerateModel(int scheduleId, BuyVM model)
        {
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            TrainsRepository trainsRepository = new TrainsRepository();
            TracksRepository tracksRepository = new TracksRepository();

            Schedule schedule = schedulesRepository.GetById(scheduleId);

            if (schedule == null || schedule.Cancelled)
            {
                return false;
            }

            Train train = trainsRepository.GetById(schedule.TrainId);

            TrainType trainType = trainsRepository.GetTrainType(train.Id);
            WayStation start = tracksRepository.GetWayStation(schedule.TrackId, model.StartStation.Id);
            WayStation end = tracksRepository.GetWayStation(schedule.TrackId, model.EndStation.Id);

            if (start == null || end == null)
            {
                return false;
            }

            model.Schedule = schedule;
            model.DepartureDate = schedulesRepository.GetDepartureDate(scheduleId, model.StartStation.Id);
            model.ArrivalDate = schedulesRepository.GetArrivalDate(scheduleId, model.EndStation.Id);

            model.UserId = ((User)Session["loggedUser"]).Id;
            model.TrainName = train.Name;
            model.TrainType = trainType.Name;
            model.Price = schedule.PricePerTicket;
            return true;
        }

        private void CheckModelValid(BuyVM model)
        {
            if (model.SeatType == null)
                ModelState.AddModelError("ModelError", "Please select a seat type.");
            
            if (model.Quantity <= 0)
                ModelState.AddModelError("ModelError", "Invalid number of tickets.");

            TrainsRepository trainsRepository = new TrainsRepository();
            Train train = trainsRepository.GetFirstOrDefault(t => t.Name == model.TrainName);
            List<Seat> seats = trainsRepository.GetNonReservedSeats(model.Schedule, model.DepartureDate, model.ArrivalDate, seatClass: model.SeatType, quantity: model.Quantity);
            //seats = seats.Where(s => s.SeatType == model.SeatType).ToList();

            if(model.Quantity > seats.Count)
            {
                ModelState.AddModelError("ModelError", "There are not enough seats for this purchase.");
            }
            model.Seats = seats;
        }

        private void BuildEntity(Ticket ticket, BuyVM model)
        {
            ticket.UserId = ((User)Session["loggedUser"]).Id;
            ticket.BeginStation = model.StartStation.Name;
            ticket.EndStation = model.EndStation.Name;
            ticket.Departure = model.DepartureDate;
            ticket.Arrival = model.ArrivalDate;
            ticket.Price = model.Price;
            ticket.Quantity = model.Quantity;
            ticket.SeatType = model.SeatType;
            ticket.TrainName = model.TrainName;
            ticket.TrainType = model.TrainType;
            ticket.ScheduleId = model.Schedule.Id;

            TrainsRepository trainsRepository = new TrainsRepository();
            List<Seat> seats = trainsRepository.GetNonReservedSeats(model.Schedule, model.DepartureDate, model.ArrivalDate, seatClass: model.SeatType, quantity: model.Quantity);
            model.Seats = seats;

            foreach (var seat in model.Seats)
            {
                ticket.SeatNumbers += "|" + seat.SeatNumber;
            }
            ticket.SeatNumbers += "|";

            QRCodeBuilder qRCodeBuilder = new QRCodeBuilder();
            ticket.QRCode = qRCodeBuilder.GenerateCodeContent(ticket.BeginStation + "|" + ticket.EndStation + "|" + ticket.Departure.ToString());
        }

        private RedirectToRouteResult ReserveTicket(Ticket ticket, BuyVM model, string paymentId)
        {
            TicketsRepository ticketsRepository = new TicketsRepository();
            if (!ticketsRepository.ReserveTicket(ticket, 
                                                 model.Schedule, 
                                                 model.Seats, 
                                                 model.DepartureDate, 
                                                 model.ArrivalDate, 
                                                 (TicketsRepository.PaymentMethod)ticket.PaymentMethod, 
                                                 paymentId))
            {
                Session["BuyVMModelState"] = "Ticket reservation failed.";
                return RedirectToAction("TicketOverview", "Ticket");
            }
            Session["ticketBuyVM"] = null;
            Session["BuyVMModelState"] = "";
            return RedirectToAction("Index", "Ticket");
        }

        public void LoadExtraViewData()
        {
            User loggedUser = (User)Session["loggedUser"];
            TicketsRepository ticketsRepository = new TicketsRepository();
            ViewData["items"] = ticketsRepository.GetAll(ticket => ticket.UserId == loggedUser.Id);
            if (((List<Ticket>)ViewData["items"]).Count == 0)
                ModelState.AddModelError("NoTickets", "You currently have no bought tickets.");
        }

        public ActionResult Index()
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            LoadExtraViewData();
            return View();
        }

        public ActionResult Buy(int id, int ssid, int esid)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            BuyVM model = new BuyVM();
            StationsRepository stationsRepository = new StationsRepository();

            model.StartStation = stationsRepository.GetFirstOrDefault(s => s.Id == ssid);
            model.EndStation = stationsRepository.GetFirstOrDefault(s => s.Id == esid);
            if (!GenerateModel(id, model))
            {
                return RedirectToAction("Index", "Home");
            }
            string startStationName = stationsRepository.GetById(model.StartStation.Id).Name;
            string endStationName = stationsRepository.GetById(model.EndStation.Id).Name;
            ViewData["route"] = startStationName + " - " + endStationName;

            TrainsRepository trainsRepository = new TrainsRepository();
            var freeSeats = trainsRepository.GetNonReservedSeats(model.Schedule, model.DepartureDate, model.ArrivalDate, getAll: true);
            if(freeSeats.Count == 0)
            {
                return RedirectToAction("Index", "Schedule");
            }

            Session["ticketBuyVM"] = model;
            ViewData["numberOfFreeSeats"] = freeSeats;
            return View(model);
        }

        [HttpPost]
        public ActionResult Buy(BuyVM model)
        {
            BuyVM savedModel = (BuyVM)Session["ticketBuyVM"];
            savedModel.Quantity = model.Quantity;
            savedModel.SeatType = model.SeatType;

            if (!CanAccessPage(savedModel))
            {
                return RedirectToAction("Login", "Home");
            }

            CheckModelValid(savedModel);
            if(!ModelState.IsValid)
            {
                return View(savedModel);
            }

            Session["ticketBuyVM"] = savedModel;
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

        public ActionResult Delete(int id)
        {
            if (Session["loggedUser"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            TicketsRepository ticketsRepository = new TicketsRepository();
            TracksRepository tracksRepository = new TracksRepository();
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            Ticket ticket = ticketsRepository.GetById(id);
            Schedule schedule = schedulesRepository.GetById(ticket.ScheduleId);

            Session["ticketCancelled"] = "Ticket for route " + tracksRepository.GetDetails(schedule.TrackId) 
                                         + " has been cancelled. Your payment has been refunded.";
            ticketsRepository.DeleteCascade(id);

            LoadExtraViewData();
            return RedirectToAction("Index");
        }

        public ActionResult PayBySystemAccount()
        {
            BuyVM model = (BuyVM)Session["ticketBuyVM"];

            if (!CanAccessPage(model))
            {
                Session["ticketBuyVM"] = null;
                return RedirectToAction("Login", "Home");
            }

            Ticket ticket = new Ticket();
            ticket.PaymentMethod = (int)TicketsRepository.PaymentMethod.BY_SYSTEM_ACCOUNT;
            BuildEntity(ticket, model);

            return ReserveTicket(ticket, model, null);
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
            string guid = "";

            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Ticket/PayWithPayPal?";
                    /*var*/ guid = Convert.ToString((new Random()).Next(100000));
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
                    /*var*/ guid = Request.Params["guid"];
                    var executedPayment = PayPalPaymentBuilder.ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("TicketOverview", "Ticket");
                    }
                }
            }
            catch (Exception e)
            {
                Session["BuyVMModelState"] = "There was an error in processing your request. Please try again.";
                return RedirectToAction("TicketOverview", "Ticket");
            }

            ticket.PaymentMethod = (int)TicketsRepository.PaymentMethod.BY_PAY_PAL;
            return ReserveTicket(ticket, model, Session[guid] as string);
        }

        public ActionResult CancelPayment()
        {
            Session["ticketBuyVM"] = null;
            return RedirectToAction("Index", "Schedule");
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