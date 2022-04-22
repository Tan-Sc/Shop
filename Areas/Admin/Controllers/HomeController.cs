using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doan_webfix.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using doan_webfix.Models;
using doan_webfix.Data;
using doan_webfix.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using doan_webfix.Utility;

namespace doan_webfix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<int> lsNotification = HttpContext.Session.Get<List<int>>("Notification");
            List<Appoinments> appoinmentslist = new List<Appoinments>();

            if (lsNotification != null)
            {
                foreach(int id in lsNotification)
                {
                    Appoinments appointment = _db.Appoinments.Where(m=>m.isConfirmed==false).FirstOrDefault(m => m.Id == id);
                    appoinmentslist.Add(appointment);
                }
            }
            return View(appoinmentslist);
        }
    }
}