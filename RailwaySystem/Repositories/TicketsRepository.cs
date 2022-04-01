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
                Date = DateTime.Now
            };
        }

        private List<SeatReservation> BuildSeatReservations(Ticket ticket, Schedule schedule, List<Seat> seats)
        {
            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            List<SeatReservation> seatReservationsList = new List<SeatReservation>();
            foreach (var seat in seats)
            {
                SeatReservation newReservation = new SeatReservation()
                {
                    ScheduleId = schedule.Id,
                    SeatId = seat.Id,
                    Departure = ticket.Departure,
                    TicketId = ticket.Id
                };

                if (seatReservations.Where(sr => sr.ScheduleId == newReservation.ScheduleId
                && sr.SeatId == newReservation.SeatId
                && sr.Departure == newReservation.Departure).FirstOrDefault() != null)
                {
                    return null;
                }
                seatReservationsList.Add(newReservation);
            }
            return seatReservationsList;
        }

        public bool ReserveTicket(Ticket ticket, Schedule schedule, List<Seat> seats, PaymentMethod paymentMethod)
        {
            UsersRepository usersRepository = new UsersRepository();
            CreditRecord creditRecord = new CreditRecord();

            if (paymentMethod == PaymentMethod.BY_SYSTEM_ACCOUNT)
            {
                if (!usersRepository.IsCreditValid(-(ticket.Price * ticket.Quantity), ticket.UserId))
                {
                    return false;
                }

                creditRecord = BuildCreditRecord(ticket);
                usersRepository.AddCreditRecord(creditRecord);

            }

            ticket.BuyDate = DateTime.Now;
            this.Add(ticket);

            ticket = this.GetFirstOrDefault(t => t.BuyDate.Equals(ticket.BuyDate) 
                                              && t.UserId == ticket.UserId);

            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            List<SeatReservation> newReservations = BuildSeatReservations(ticket, schedule, seats);
            if(newReservations == null)
            {
                this.Delete(ticket.Id);
                if (paymentMethod == PaymentMethod.BY_SYSTEM_ACCOUNT && !creditRecord.Equals(new CreditRecord()))
                {
                    creditRecord = usersRepository.GetCreditRecord(cr => cr.Date.Equals(creditRecord.Date)
                                                  && cr.CustomerId == creditRecord.CustomerId);
                    usersRepository.Delete(creditRecord.Id);
                }
                return false;
            }
            seatReservations.AddRange(newReservations);

            Context.SaveChanges();
            return true;
        }
    }
}