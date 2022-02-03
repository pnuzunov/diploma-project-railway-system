using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class RailwaySystemDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatType> SeatTypes { get; set; }
        public DbSet<TrainType> TrainTypes { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CreditRecord> CreditRecords { get; set; }

        public RailwaySystemDBContext()
            : base(@"Server=DESKTOP-H5B78FN\TEW_SQLEXPRESS;Database=RailwaySystemDB;User Id=sa;Password=mypass;")
        {
            Users = this.Set<User>();
            Trains = this.Set<Train>();
            Tracks = this.Set<Track>();
            Stations = this.Set<Station>();
            Schedules = this.Set<Schedule>();
            Tickets = this.Set<Ticket>();
            Seats = this.Set<Seat>();
            SeatTypes = this.Set<SeatType>();
            TrainTypes = this.Set<TrainType>();
            UserRoles = this.Set<UserRole>();
            CreditRecords = this.Set<CreditRecord>();
        }
    }
}