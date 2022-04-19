using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class TracksRepository : BaseRepository<Track>
    {
        public void Add(Track track, List<WayStation> wayStations)
        {
            track = this.Items.Add(track);

            DbSet<WayStation> dbWayStations = this.Context.Set<WayStation>();
            for (int i = 0; i < wayStations.Count; i++)
            {
                wayStations[i].TrackId = track.Id;
                dbWayStations.Add(wayStations[i]);
            }
            Context.SaveChanges();
        }

        public WayStation GetWayStation(int trackId, int stationId)
        {
            DbSet<WayStation> dbWayStations = this.Context.Set<WayStation>();
            IQueryable<WayStation> query = dbWayStations;
            return query.Where(q => q.TrackId == trackId && q.StationId == stationId).FirstOrDefault();
        }

        public WayStation GetWayStationByConsecNumber(int trackId, int consecNumber)
        {
            DbSet<WayStation> dbWayStations = this.Context.Set<WayStation>();
            IQueryable<WayStation> query = dbWayStations;
            return query.Where(q => q.TrackId == trackId && q.ConsecutiveNumber == consecNumber).FirstOrDefault();
        }

        public List<WayStation> GetWayStations(int trackId)
        {
            DbSet<WayStation> dbWayStations = this.Context.Set<WayStation>();
            IQueryable<WayStation> query = dbWayStations;
            return query.Where(q => q.TrackId == trackId).ToList();
        }

        public List<WayStation> GetWayStations()
        {
            DbSet<WayStation> dbWayStations = this.Context.Set<WayStation>();
            IQueryable<WayStation> query = dbWayStations;
            return query.ToList();
        }

        public List<WayStation> GetWayStations(Expression<Func<WayStation,bool>> filter)
        {
            DbSet<WayStation> dbWayStations = this.Context.Set<WayStation>();
            IQueryable<WayStation> query = dbWayStations;
            return query.Where(filter).ToList();
        }

        public Station GetStartStation(int trackId)
        {
            List<WayStation> wayStations = this.GetWayStations(trackId);
            WayStation ws = wayStations.Where(q => q.ConsecutiveNumber == 0).FirstOrDefault();
            StationsRepository stationsRepository = new StationsRepository();
            return stationsRepository.GetById(ws.StationId);
        }

        public Station GetEndStation(int trackId)
        {
            List<WayStation> wayStations = this.GetWayStations(trackId);
            int esid = wayStations.Max(q => q.ConsecutiveNumber);
            WayStation ws = wayStations.Where(q => q.ConsecutiveNumber == esid).FirstOrDefault();
            StationsRepository stationsRepository = new StationsRepository();
            return stationsRepository.GetById(ws.StationId);
        }

        public List<Track> FindTracks(int startStationId, int endStationId)
        {
            List<WayStation> startStations = this.GetWayStations(ws => ws.StationId == startStationId);
            List<WayStation> endStations = this.GetWayStations(ws => ws.StationId == endStationId);
            List<Track> tracks = new List<Track>();

            foreach (var ss in startStations)
            {
                foreach (var es in endStations)
                {
                    if(ss.TrackId == es.TrackId && ss.ConsecutiveNumber < es.ConsecutiveNumber)
                    {
                        TracksRepository tracksRepository = new TracksRepository();
                        tracks.Add(tracksRepository.GetById(ss.TrackId));
                    }
                }
            }
            return tracks;
        }
    }
}