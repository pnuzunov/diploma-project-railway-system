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
        public bool ReserveTicket(Ticket ticket, Schedule schedule, List<Seat> seats)
        {
            ticket.BuyDate = DateTime.Now;
            this.Add(ticket);

            ticket = this.GetFirstOrDefault(t => t.BuyDate == ticket.BuyDate && t.UserId == ticket.UserId);

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
                    return false;
                }
                seatReservations.Add(seatReservation);
            }
            Context.SaveChanges();
            return true;
        }
    }
}