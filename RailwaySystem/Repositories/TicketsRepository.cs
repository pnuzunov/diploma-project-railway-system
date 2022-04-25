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

        public bool ReserveTicket(Ticket ticket, Schedule schedule, List<Seat> seats, DateTime departure, DateTime arrival, PaymentMethod paymentMethod)
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

            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            List<SeatReservation> newReservations = BuildSeatReservations(ticket, schedule, seats, departure, arrival);
            if(newReservations == null)
            {
                if (paymentMethod == PaymentMethod.BY_SYSTEM_ACCOUNT && !creditRecord.Equals(new CreditRecord()))
                {
                    creditRecord = usersRepository.GetCreditRecord(cr => cr.Date.Equals(creditRecord.Date)
                                                  && cr.CustomerId == creditRecord.CustomerId);
                    usersRepository.Delete(creditRecord.Id);
                }
                this.Delete(ticket.Id);
                return false;
            }
            seatReservations.AddRange(newReservations);

            Context.SaveChanges();
            return true;
        }

        public void DeleteCascade(int ticketId)
        {
            DbSet<CreditRecord> creditRecords = Context.Set<CreditRecord>();
            CreditRecord creditRecord = creditRecords.Where(cr => cr.TicketId == ticketId).FirstOrDefault();
            if (creditRecord != null)
                creditRecords.Remove(creditRecord);
            this.Delete(ticketId);
            Context.SaveChanges();
        }
    }
}