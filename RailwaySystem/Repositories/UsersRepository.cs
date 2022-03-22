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
        //----------------------------------------------------------------------------------
        #region User Roles Methods
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
            UserRole userRole = GetUserRole(u => u.Id == user.RoleId);
            if (userRole == null) return false;
            return userRole.LevelOfAccess <= ((int)level);
        }

        #endregion
        //----------------------------------------------------------------------------------
        #region Credit Records Methods

        public decimal GetTotalCredit(int userId)
        {
            DbSet<CreditRecord> creditRecords = Context.Set<CreditRecord>();
            IQueryable<CreditRecord> listOfCreditRecords = creditRecords.Where(cr => cr.CustomerId == userId);
            if (listOfCreditRecords.Count() == 0) return 0.0M;

            decimal totalCredit = 0.0M;
            foreach (var record in listOfCreditRecords)
            {
                totalCredit += record.Amount;
            }

            return totalCredit;
        }

        public bool IsCreditValid(decimal credit, int customerId)
        {
            decimal currentTotal = GetTotalCredit(customerId);
            if (currentTotal + credit < 0.0M)
            {
                return false;
            }
            return true;
        }

        public bool AddCreditRecord(CreditRecord creditRecord)
        {
            if(!IsCreditValid(creditRecord.Amount, creditRecord.CustomerId))
            {
                return false;
            }
            DbSet<CreditRecord> creditRecords = Context.Set<CreditRecord>();
            creditRecord.Date = DateTime.Now;
            creditRecords.Add(creditRecord);
            Context.SaveChanges();
            return true;
        }

        #endregion
    }
}