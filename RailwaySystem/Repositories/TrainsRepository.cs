using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class TrainsRepository : BaseRepository<Train>
    {
        public static string FIRST_CLASS = "First Class";
        public static string REGULAR_CLASS = "Regular Class";
        //----------------------------------------------------------------------------------
        #region Seat Methods

        public List<Seat> GetSeats(Expression<Func<Seat, bool>> filter = null)
        {
            DbSet<Seat> seats = Context.Set<Seat>();
            if (filter == null) return seats.ToList();
            IQueryable<Seat> query = seats;
            return query.Where(filter).ToList();
        }

        public List<Seat> GetNonReservedSeats(int quantity, int scheduleId)
        {
            SchedulesRepository schedulesRepository = new SchedulesRepository();
            TrainsRepository trainsRepository = new TrainsRepository();
            List<SeatReservation> reservations = schedulesRepository.GetSeatReservations(scheduleId);
            List<Seat> seats = trainsRepository.GetSeats();
            List<Seat> freeSeats = new List<Seat>();

            if (seats == null) return freeSeats;

            int counter = 0;
            foreach (var seat in seats)
            {
                if (counter == quantity) break;
                if(!reservations.Any(res => res.SeatId == seat.Id)) {
                    freeSeats.Add(seat);
                    counter++;
                }
            }

            return freeSeats;
        }

        public Seat GetSeat(Expression<Func<Seat, bool>> filter)
        {
            DbSet<Seat> seats = Context.Set<Seat>();
            IQueryable<Seat> query = seats;
            return query
                   .Where(filter)
                   .FirstOrDefault();
        }

        #endregion
        //----------------------------------------------------------------------------------
        #region Train Type Methods

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

        #endregion
        //----------------------------------------------------------------------------------
        #region Train Methods

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

        #endregion
        //----------------------------------------------------------------------------------
    }
}