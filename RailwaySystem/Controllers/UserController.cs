using RailwaySystem.Entities;
using RailwaySystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RailwaySystem.ViewModels.User;

namespace RailwaySystem.Controllers
{
    public class UserController : Controller
    {
        private bool CanAccessPage(UsersRepository.Levels level)
        {
            UsersRepository usersRepository = new UsersRepository();
            User loggedUser = (User)Session["loggedUser"];
            if (loggedUser == null || !usersRepository.CanAccess(loggedUser.Id, level))
            {
                return false;
            }
            return true;
        }

        private void GenerateModel(DetailsVM model, User user, decimal credit)
        {
            if (user == null) return;

            model.Id = user.Id;
            model.Username = user.Username;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Password = user.Password;
            model.Phone = user.Phone;
            model.Email = user.Email;
            model.Credit = credit;
        }

        private void BuildEntity(CreditRecord creditRecord, DetailsVM model, int employeeId)
        {
            creditRecord.CustomerId = model.Id;
            creditRecord.Amount = model.Credit;
            if(employeeId > 0)
            {
                creditRecord.EmployeeId = employeeId;
            }
        }

        private void CheckIsModelValid(DetailsVM model)
        {
            UsersRepository usersRepository = new UsersRepository();
            if (!usersRepository.IsCreditValid(model.Credit, model.Id))
                ModelState.AddModelError("Error", "Insufficient credit amount.");
            else if(model.Credit <= 0.0m)
            {
                ModelState.AddModelError("Error", "Invalid credit amount.");
            }
        }

        public ActionResult Index()
        {
            if (!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Index", "Home");
            }

            UsersRepository usersRepository = new UsersRepository();
            ViewData["items"] = usersRepository.GetAll();

            return View();
        }

        [HttpPost]
        public ActionResult Index(ViewModels.BaseEditVM model)
        {
            if(!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Index", "Home");
            }

            UsersRepository usersRepository = new UsersRepository();
            User loggedUser = (User)Session["loggedUser"];

            User query = usersRepository.GetFirstOrDefault(i => i.ClientNumber == model.Id.ToString());
            if(query == null || query.RoleId < loggedUser.RoleId)
            {
                ModelState.AddModelError("UserNotFoundError", "No user found.");
                return View();
            }
            return RedirectToAction("Details", "User", new { id = query.Id });
        }

        public ActionResult Details(int? id)
        {
            UsersRepository usersRepository = new UsersRepository();
            User selectedUser = null;

            if (id == null)
            {
                if(Session["loggedUser"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                selectedUser = (User)Session["loggedUser"];
            }
            else
            {
                if(!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
                {
                    return RedirectToAction("Index", "Home");
                }
                selectedUser = usersRepository.GetById((int)id);
                if(selectedUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            DetailsVM model = new DetailsVM();
            GenerateModel(model, selectedUser, usersRepository.GetTotalCredit(selectedUser.Id));
            return View(model);

        }

        public ActionResult AddCredit(int id)
        {
            if(!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Index", "Home");
            }

            UsersRepository usersRepository = new UsersRepository();
            User selectedUser = usersRepository.GetFirstOrDefault(u => u.Id == id);
            DetailsVM model = new DetailsVM();
            GenerateModel(model, selectedUser, usersRepository.GetTotalCredit(selectedUser.Id));
            model.Credit = 0.0m;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddCredit(DetailsVM model)
        {
            if (!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Index", "Home");
            }

            UsersRepository usersRepository = new UsersRepository();
            CheckIsModelValid(model);
            if(!ModelState.IsValid)
            {
                ViewData["items"] = usersRepository.GetAll();
                return View(model);
            }
            CreditRecord creditRecord = new CreditRecord();
            User loggedUser = (User)Session["loggedUser"];
            BuildEntity(creditRecord, model, loggedUser.Id);
            usersRepository.AddCreditRecord(creditRecord);

            return RedirectToAction("Details", "User", new { id = model.Id });
        }

        public ActionResult Delete(int id)
        {
            if (!CanAccessPage(UsersRepository.Levels.EMPLOYEE_ACCESS))
            {
                return RedirectToAction("Index", "Home");
            }

            UsersRepository repo = new UsersRepository();

            User loggedUser = (User)Session["loggedUser"];
            if (loggedUser.Id == id) Session["loggedUser"] = null;
            repo.Delete(id);
            return RedirectToAction("Index", "User");
        }
    }
}