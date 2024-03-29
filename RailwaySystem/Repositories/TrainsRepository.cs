﻿using RailwaySystem.Entities;
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

        public List<Seat> GetNonReservedSeats(Schedule schedule, DateTime departure, DateTime arrival, int quantity = 0, string seatClass = "", bool getAll = false)
        {
            if (seatClass == null) return new List<Seat>();

            SchedulesRepository schedulesRepository = new SchedulesRepository();
            List<SeatReservation> reservations = schedulesRepository.GetSeatReservations(schedule.Id, schedule.TrainId);
            bool ignoreClass = seatClass.Equals("");
            bool isFirstClass = seatClass.Equals(FIRST_CLASS);
            List<Seat> seats = GetSeats(s => s.TrainId == schedule.TrainId && (ignoreClass || s.IsFirstClass == isFirstClass));
            List<Seat> freeSeats = new List<Seat>();

            if (seats == null) return freeSeats;

            int counter = 0;
            foreach (var seat in seats)
            {
                if (!getAll && counter == quantity) break;
                if(!reservations.Any(res => res.SeatId == seat.Id &&
                                     DateTime.Compare(res.Departure, arrival) < 0 &&
                                     DateTime.Compare(departure, res.Arrival) < 0)) {
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