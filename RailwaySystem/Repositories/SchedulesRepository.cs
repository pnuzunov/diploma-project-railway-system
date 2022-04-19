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

        public void Add(Schedule item, DateTime lastDate, List<ScheduledWayStation> scheduledWS)
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
                schedule = this.Items.Add(schedule);

                AddScheduledWS(schedule, scheduledWS);

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

        public void AddScheduledWS(Schedule schedule, List<ScheduledWayStation> scheduledWS)
        {
            DbSet<ScheduledWayStation> scheduledsDbSet = Context.Set<ScheduledWayStation>();

            foreach (var sws in scheduledWS)
            {
                sws.ScheduleId = schedule.Id;
                if(!sws.Departure.Equals(new DateTime())) 
                    sws.Departure = schedule.Departure.Date + sws.Departure.TimeOfDay;
                if (!sws.Arrival.Equals(new DateTime()))
                    sws.Arrival = sws.Departure.Date + sws.Arrival.TimeOfDay;
                scheduledsDbSet.Add(sws);
            }
            Context.SaveChanges();
        }

        public void CascadeDelete(int scheduleId)
        {
            DbSet<ScheduledWayStation> scheduledsDbSet = Context.Set<ScheduledWayStation>();
            List<ScheduledWayStation> swsToDelete = scheduledsDbSet.Where(sws => sws.ScheduleId == scheduleId).ToList();
            foreach (var sws in swsToDelete)
            {
                scheduledsDbSet.Remove(sws);
            }
            Context.SaveChanges();
            this.Delete(scheduleId);
        }

        public List<ScheduledWayStation> GetScheduledWayStations(int scheduleId)
        {
            DbSet<ScheduledWayStation> scheduledsDbSet = Context.Set<ScheduledWayStation>();
            IQueryable<ScheduledWayStation> query = scheduledsDbSet;
            return query.Where(q => q.ScheduleId == scheduleId).ToList();
        }

        public ScheduledWayStation GetScheduledWayStation(Schedule schedule, int stationId)
        {
            DbSet<ScheduledWayStation> scheduledsDbSet = Context.Set<ScheduledWayStation>();
            DbSet<WayStation> wayStationsDbSet = Context.Set<WayStation>();

            WayStation wayStation = wayStationsDbSet.Where(ws => ws.StationId == stationId && ws.TrackId == schedule.TrackId).FirstOrDefault();

            if (wayStation == null) return null;

            IQueryable<ScheduledWayStation> query = scheduledsDbSet;
            return query.Where(q => q.ScheduleId == schedule.Id && q.WayStationId == wayStation.Id).FirstOrDefault();
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
            ScheduledWayStation firstSWS = GetScheduledWayStation(schedule, startStationId);
            ScheduledWayStation lastSWS = GetScheduledWayStation(schedule, endStationId);
            schedule.Departure = schedule.Departure.Date + firstSWS.Departure.TimeOfDay;
            schedule.Arrival = schedule.Departure.Date + lastSWS.Arrival.TimeOfDay;
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

        public void DeleteCascade(int scheduleId)
        {
            DbSet<ScheduledWayStation> scheduledWayStations = Context.Set<ScheduledWayStation>();
            IEnumerable<ScheduledWayStation> query = scheduledWayStations.Where(q => q.ScheduleId == scheduleId);

            if(query != null && query.Count() > 0)
                scheduledWayStations.RemoveRange(query);
            this.Delete(scheduleId);
            Context.SaveChanges();
        }

    }
}