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
        public Ticket GetTicket(Ticket ticket)
        {
            DbSet<Ticket> tickets = Context.Set<Ticket>();
            IQueryable<Ticket> query = tickets;
            return query.Where(q => q.Equals(ticket))
                        .FirstOrDefault();
        }

        public bool ReserveTicket(Ticket ticket, Schedule schedule, List<Seat> seats)
        {
            UsersRepository usersRepository = new UsersRepository();
            if (!usersRepository.IsCreditValid(-(ticket.Price), ticket.UserId))
            {
                return false;
            }

            CreditRecord creditRecord = new CreditRecord()
            {
                Amount = -(ticket.Price),
                CustomerId = ticket.UserId,
                EmployeeId = null,
                Date = DateTime.Now
            };

            usersRepository.AddCreditRecord(creditRecord);

            ticket.BuyDate = DateTime.Now;
            this.Add(ticket);

            ticket = this.GetFirstOrDefault(t => t.BuyDate.Equals(ticket.BuyDate) 
                                              && t.UserId == ticket.UserId);

            creditRecord = usersRepository.GetCreditRecord(cr => cr.Date.Equals(creditRecord.Date)
                                                              && cr.CustomerId == creditRecord.CustomerId);

            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            foreach(var seat in seats)
            {
                SeatReservation seatReservation = new SeatReservation()
                {
                    ScheduleId = schedule.Id,
                    SeatId = seat.Id,
                    Departure = ticket.Departure,
                    TicketId = ticket.Id
                };
                
                if(seatReservations.Where(sr => sr.ScheduleId == seatReservation.ScheduleId
                && sr.SeatId == seatReservation.SeatId
                && sr.Departure == seatReservation.Departure).FirstOrDefault() != null)
                {
                    this.Delete(ticket.Id);
                    usersRepository.Delete(creditRecord.Id);
                    return false;
                }
                seatReservations.Add(seatReservation);
            }
            Context.SaveChanges();
            return true;
        }
    }
}