using RailwaySystem.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class StationsRepository : BaseRepository<Station>
    {
        public List<City> GetCities(Expression<Func<City, bool>> filter = null)
        {
            DbSet<City> cities = Context.Set<City>();
            IQueryable<City> query = cities;
            if (filter == null)
                return query.ToList();
            return query.Where(filter).ToList();
            
        }
    }
}