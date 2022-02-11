using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class TrainsRepository : BaseRepository<Train>
    {
        public static string FIRST_CLASS = "First Class";
        public static string SECOND_CLASS = "Second Class";

        public List<TrainType> GetTrainTypes(Expression<Func<TrainType, bool>> filter = null)
        {
            DbSet<TrainType> trainTypes = Context.Set<TrainType>();
            if (filter == null) return trainTypes.ToList();
            IQueryable<TrainType> query = trainTypes;
            return query.Where(filter).ToList();
        }

        public TrainType GetTrainType(int trainId)
        {
            Train train = GetById(trainId);
            if (train == null) return null;

            DbSet<TrainType> trainTypes = Context.Set<TrainType>();
            IQueryable<TrainType> query = trainTypes;
            return query.FirstOrDefault(tt => tt.Id == train.TypeId);
        }

        public List<Seat> GetSeats(Expression<Func<Seat, bool>> filter = null)
        {
            DbSet<Seat> seats = Context.Set<Seat>();
            if (filter == null) return seats.ToList();
            IQueryable<Seat> query = seats;
            return query.Where(filter).ToList();
        }

        public Seat GetSeat(Expression<Func<Seat, bool>> filter)
        {
            DbSet<Seat> seats = Context.Set<Seat>();
            IQueryable<Seat> query = seats;
            return query
                   .Where(filter)
                   .FirstOrDefault();
        }

        public List<SeatType> GetSeatTypes(Expression<Func<SeatType, bool>> filter = null)
        {
            DbSet<SeatType> seatTypes = Context.Set<SeatType>();
            if (filter == null) return seatTypes.ToList();
            IQueryable<SeatType> query = seatTypes;
            return query.Where(filter).ToList();
        }

        public SeatType GetSeatType(Expression<Func<SeatType, bool>> filter)
        {
            DbSet<SeatType> seatTypes = Context.Set<SeatType>();
            IQueryable<SeatType> query = seatTypes;
            return query
                   .Where(filter)
                   .FirstOrDefault();
        }

        public void Add(Train train, List<Seat> seats)
        {
            Items.Add(train);
            Context.SaveChanges();
            IQueryable<Train> query = Context.Set<Train>();
            int trainId = query
                                .Where(t => t.Name == train.Name)
                                .FirstOrDefault().Id;
            DbSet<Seat> seatsForAdd = Context.Set<Seat>();
            foreach (var seat in seats)
            {
                seat.TrainId = trainId;
                seatsForAdd.Add(seat);
            }
            Context.SaveChanges();
        }

        public void DeleteCascade(int id)
        {
            DbSet<Seat> seats = Context.Set<Seat>();
            List<Seat> seatsForDelete = GetSeats(st => st.TrainId == id).ToList();
            foreach (var seatForDelete in seatsForDelete)
            {
                seats.Remove(seatForDelete);
            }
            Train train = Items.Where(t => t.Id == id).FirstOrDefault();
            Items.Remove(train);
            Context.SaveChanges();
        }
    }
}