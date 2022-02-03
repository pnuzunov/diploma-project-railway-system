using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using RailwaySystem.Entities;

namespace RailwaySystem.Repositories
{
    public class ReadOnlyRepository<T> where T : BaseEntity
    {
        protected DbContext Context;
        protected DbSet<T> Items;

        public ReadOnlyRepository()
        {
            Context = new RailwaySystemDBContext();
            Items = Context.Set<T>();
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null) return Items.ToList();
            IQueryable<T> query = Items;
            return query.Where(filter).ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = Items;
            return query
                   .Where(filter)
                   .FirstOrDefault();
        }

        public T GetById(int id)
        {
            IQueryable<T> query = Items;
            return query
                        .Where(i => i.Id == id)
                        .FirstOrDefault();
        }
    }
}