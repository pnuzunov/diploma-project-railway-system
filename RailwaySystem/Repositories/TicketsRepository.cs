using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RailwaySystem.Entities;
using System.Data.Entity;

namespace RailwaySystem.Repositories
{
    public class TicketsRepository : BaseRepository<Ticket>
    {
        public enum PaymentMethod
        {
            BY_SYSTEM_ACCOUNT = 0,
            BY_PAY_PAL = 1
        }

        public Ticket GetTicket(Ticket ticket)
        {
            DbSet<Ticket> tickets = Context.Set<Ticket>();
            IQueryable<Ticket> query = tickets;
            return query.Where(q => q.Equals(ticket))
                        .FirstOrDefault();
        }

        private CreditRecord BuildCreditRecord(Ticket ticket)
        {
            return new CreditRecord()
            {
                Amount = -(ticket.Price * ticket.Quantity),
                CustomerId = ticket.UserId,
                EmployeeId = null,
                TicketId = ticket.Id,
                Date = DateTime.Now
            };
        }

        private List<SeatReservation> BuildSeatReservations(Ticket ticket, Schedule schedule, List<Seat> seats, DateTime departure, DateTime arrival)
        {
            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            List<SeatReservation> seatReservationsList = new List<SeatReservation>();
            foreach (var seat in seats)
            {
                SeatReservation newReservation = new SeatReservation()
                {
                    ScheduleId = schedule.Id,
                    SeatId = seat.Id,
                    TicketId = ticket.Id,
                    Departure = departure,
                    Arrival = arrival
                };

                if (seatReservations.Where(sr => sr.ScheduleId == newReservation.ScheduleId &&
                                           sr.SeatId == newReservation.SeatId &&
                                           DateTime.Compare(sr.Arrival, newReservation.Departure) > 0 )
                    .FirstOrDefault() != null)
                {
                    return null;
                }
                seatReservationsList.Add(newReservation);
            }
            return seatReservationsList;
        }

        private PayPalPayment AddPayPalPaymentId(int ticketId, string paymentId)
        {
            DbSet<PayPalPayment> payPalPayments = this.Context.Set<PayPalPayment>();
            PayPalPayment entry = payPalPayments.Add(new PayPalPayment()
            {
                TicketId = ticketId,
                PaymentId = paymentId
            });
            this.Context.SaveChanges();
            return entry;
        }

        public bool ReserveTicket(Ticket ticket, Schedule schedule, List<Seat> seats, DateTime departure, DateTime arrival, PaymentMethod paymentMethod, string paymentId)
        {
            UsersRepository usersRepository = new UsersRepository();
            CreditRecord creditRecord = new CreditRecord();

            ticket.BuyDate = DateTime.Now;
            ticket = this.Add(ticket);

            if (paymentMethod == PaymentMethod.BY_SYSTEM_ACCOUNT)
            {
                if (!usersRepository.IsCreditValid(-(ticket.Price * ticket.Quantity), ticket.UserId))
                {
                    this.Delete(ticket.Id);
                    return false;
                }

                creditRecord = BuildCreditRecord(ticket);
                usersRepository.AddCreditRecord(creditRecord);

            }
            else if(paymentMethod == PaymentMethod.BY_PAY_PAL)
            {
                this.AddPayPalPaymentId(ticket.Id, paymentId);
            }


            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            List<SeatReservation> newReservations = BuildSeatReservations(ticket, schedule, seats, departure, arrival);
            seatReservations.AddRange(newReservations);

            Context.SaveChanges();
            return true;
        }

        public void DeleteCascade(int ticketId)
        {
            Ticket ticket = this.GetById(ticketId);
            var paymentMethod = (PaymentMethod)ticket.PaymentMethod;

            if (paymentMethod == PaymentMethod.BY_SYSTEM_ACCOUNT)
            {
                DbSet<CreditRecord> creditRecords = Context.Set<CreditRecord>();
                CreditRecord creditRecord = creditRecords.Where(cr => cr.TicketId == ticketId).FirstOrDefault();
                if (creditRecord != null)
                    creditRecords.Remove(creditRecord);
            }
            else if(paymentMethod == PaymentMethod.BY_PAY_PAL)
            {
                DbSet<PayPalPayment> payments = Context.Set<PayPalPayment>();
                PayPalPayment pppayment = payments.Where(p => p.TicketId == ticketId).FirstOrDefault();
                if (pppayment != null)
                {
                    var apiContext = HelperClasses.PaypalConfiguration.GetAPIContext();
                    HelperClasses.PayPalPaymentBuilder payPalPaymentBuilder = new HelperClasses.PayPalPaymentBuilder(apiContext, null);
                    payPalPaymentBuilder.Refund(pppayment.PaymentId);
                    payments.Remove(pppayment);
                }
                    
            }

            this.Delete(ticketId);
            Context.SaveChanges();
        }
    }
}