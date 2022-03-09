﻿using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class SchedulesRepository : BaseRepository<Schedule>
    {
        public Station GetStartStation(int scheduleId)
        {
            Track track = GetTrack(scheduleId);
            if (track == null) return null;
            StationsRepository stationsRepository = new StationsRepository();
            Station station = stationsRepository.GetFirstOrDefault(st => st.Id == track.StartStationId);
            return station;
        }

        public Station GetEndStation(int scheduleId)
        {
            Track track = GetTrack(scheduleId);
            if (track == null) return null;
            StationsRepository stationsRepository = new StationsRepository();
            Station station = stationsRepository.GetFirstOrDefault(st => st.Id == track.EndStationId);
            return station;
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

        public List<SeatReservation> GetSeatReservations(int scheduleId)
        {
            Schedule schedule = this.GetById(scheduleId);
            if (schedule == null) return null;
            DbSet<SeatReservation> reservations = Context.Set<SeatReservation>();
            IQueryable<SeatReservation> query = reservations;
            return query.Where(r => r.ScheduleId == scheduleId).ToList();
        }

        public List<Schedule> GetFilteredSchedules(int trackId, TimeSpan time)
        {
            List<Schedule> schedules = this.GetAll()
                                            .Where(s => s.TrackId == trackId
                                                        && TimeSpan.Compare(s.Departure.TimeOfDay, time) >= 0)
                                            .ToList();
            return schedules;
        }
    }
}