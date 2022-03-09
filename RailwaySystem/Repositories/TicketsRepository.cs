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
            DbSet<SeatReservation> seatReservations = Context.Set<SeatReservation>();
            foreach(var seat in seats)
            {
                SeatReservation seatReservation = new SeatReservation()
                {
                    ScheduleId = schedule.Id,
                    SeatId = seat.Id,
                    Departure = ticket.Departure
                };
                
                if(seatReservations.Where(sr => sr.ScheduleId == seatReservation.ScheduleId
                && sr.SeatId == seatReservation.SeatId
                && sr.Departure == seatReservation.Departure).FirstOrDefault() != null)
                {
                    return false;
                }
                seatReservations.Add(seatReservation);
            }
            this.Add(ticket);
            return true;
        }
    }
}