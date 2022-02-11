using RailwaySystem.Entities;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RailwaySystem.Repositories
{
    public class UsersRepository : BaseRepository<User>
    {
        public enum Levels : int
        {
            FULL_ACCESS = 1,
            EMPLOYEE_ACCESS = 2,
            CUSTOMER_ACCESS = 3
        }

        public List<UserRole> GetUserRoles(Expression<Func<UserRole, bool>> filter = null)
        {
            DbSet<UserRole> userRoles = Context.Set<UserRole>();
            if(filter == null) return userRoles.ToList();
            IQueryable<UserRole> query = userRoles;
            return query.Where(filter).ToList();
        }

        public UserRole GetUserRole(Expression<Func<UserRole, bool>> filter)
        {
            DbSet<UserRole> userRoles = Context.Set<UserRole>();
            IQueryable<UserRole> query = userRoles;
            return query
                   .Where(filter)
                   .FirstOrDefault();
        }

        public bool CanAccess(int userId, Levels level)
        {
            UsersRepository users = new UsersRepository();
            User user = users.GetAll().Where(i => i.Id == userId).FirstOrDefault();
            if (user == null) return false;
            UserRole userRole = GetUserRole(u => u.Id == user.Id);
            if (userRole == null) return false;
            return userRole.LevelOfAccess <= ((int)level);
        }
    }
}