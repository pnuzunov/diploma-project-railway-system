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
    public class BaseRepository<T> where T : BaseEntity
    {
        protected DbContext Context;
        protected DbSet<T> Items;

        public BaseRepository()
        {
            Context = new RailwaySystemDBContext();
            Items = Context.Set<T>();
        }

        public virtual List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null) return Items.ToList();
            IQueryable<T> query = Items;
            return query.Where(filter).ToList();
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = Items;
            return query
                   .Where(filter)
                   .FirstOrDefault();
        }

        public virtual T GetById(int id)
        {
            IQueryable<T> query = Items;
            return query
                        .Where(i => i.Id == id)
                        .FirstOrDefault();
        }

        public virtual T Add(T item)
        {
            T entry = Items.Add(item);
            Context.SaveChanges();
            return entry;
        }

        public virtual void Delete(int id)
        {
            T item = Items.Where(i => i.Id == id).FirstOrDefault();
            Items.Remove(item);
            Context.SaveChanges();
        }

        public virtual void Update(T item)
        {
            DbEntityEntry entry = Context.Entry(item);
            entry.State = EntityState.Modified;
            Context.SaveChanges();
        }

    }
}