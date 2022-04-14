using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class SchedulesRepository : BaseRepository<Schedule>
    {
        public enum ScheduleMode
        {
            EVERY_DAY = 1,
            ONLY_WEEKDAYS = 2,
            ONLY_WEEKENDS = 3
        }

        public void Add(Schedule item, DateTime lastDate)
        {
            DateTime nextDeparture = item.Departure;
            DateTime nextArrival = item.Arrival;
            do
            {
                Schedule schedule = new Schedule()
                {
                    PricePerTicket = item.PricePerTicket,
                    ScheduleModeId = item.ScheduleModeId,
                    TrackId = item.TrackId,
                    TrainId = item.TrainId,
                    Arrival = nextArrival,
                    Departure = nextDeparture
                };

                ScheduleMode scheduleMode = (ScheduleMode)schedule.ScheduleModeId;
                this.Items.Add(schedule);
                if(item.ScheduleModeId == null)
                {
                    Context.SaveChanges();
                    return;
                }

                nextDeparture = nextDeparture.AddDays(1);
                nextArrival = nextArrival.AddDays(1);
                
                while(scheduleMode.Equals(ScheduleMode.ONLY_WEEKDAYS) &&
                     (nextDeparture.DayOfWeek.Equals(DayOfWeek.Saturday) ||
                      nextDeparture.DayOfWeek.Equals(DayOfWeek.Sunday)))
                {
                    nextDeparture = nextDeparture.AddDays(1);
                    nextArrival = nextArrival.AddDays(1);
                }

                while (scheduleMode.Equals(ScheduleMode.ONLY_WEEKENDS) &&
                       !nextDeparture.DayOfWeek.Equals(DayOfWeek.Saturday) &&
                       !nextDeparture.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    nextDeparture = nextDeparture.AddDays(1);
                    nextArrival = nextArrival.AddDays(1);
                }

            } while (DateTime.Compare(nextDeparture.Date, lastDate.Date) < 0);
            Context.SaveChanges();
        }

        public Track GetTrack(int scheduleId)
        {
            Schedule schedule = this.GetById(scheduleId);
            if (schedule == null) return null;
            TracksRepository tracksRepository = new TracksRepository();
            Track track = tracksRepository.GetFirstOrDefault(tr => tr.Id == schedule.TrackId);
            return track;
        }

        public Train GetTrain(int scheduleId)
        {
            Schedule schedule = this.GetById(scheduleId);
            if (schedule == null) return null;
            TrainsRepository trainsRepository = new TrainsRepository();
            Train train = trainsRepository.GetFirstOrDefault(t => t.Id == schedule.TrainId);
            return train;
        }

        public List<SeatReservation> GetSeatReservations(int scheduleId, int trainId)
        {
            Schedule schedule = this.GetById(scheduleId);
            if (schedule == null) return null;
            DbSet<SeatReservation> reservationsDb = Context.Set<SeatReservation>();
            TrainsRepository trainsRepository = new TrainsRepository();
            List<Seat> seats = trainsRepository.GetSeats(s => s.TrainId == trainId);
            List<SeatReservation> reservations = new List<SeatReservation>();

            foreach (var seat in seats)
            {
                SeatReservation seatReservation = reservationsDb.Where(r => r.SeatId == seat.Id && r.ScheduleId == scheduleId).FirstOrDefault();
                if (seatReservation != null)
                {
                    reservations.Add(seatReservation);
                }
            }
            return reservations;
        }

        public void Transform(Schedule schedule, int startStationId, int endStationId)
        {
            TracksRepository tracksRepository = new TracksRepository();
            Station firstStation = tracksRepository.GetStartStation(schedule.TrackId);
            schedule.Departure = schedule.Departure + tracksRepository.CalculateTravelTime(schedule.TrackId, firstStation.Id, startStationId);
            schedule.Arrival = schedule.Departure + tracksRepository.CalculateTravelTime(schedule.TrackId, startStationId, endStationId);
        }

        public List<Schedule> GetFilteredSchedules(int trackId, DateTime date, int startStationId, int endStationId)
        {
            TracksRepository tracksRepository = new TracksRepository();
            List<Schedule> schedules = this.GetAll()
                                            .Where(s => s.TrackId == trackId
                                                        && s.Departure.Year == date.Year
                                                        && s.Departure.Month == date.Month
                                                        && s.Departure.Day == date.Day)
                                            .ToList();
            foreach (var schedule in schedules)
            {
                Transform(schedule, startStationId, endStationId);
            }
            return schedules;
        }
    }
}