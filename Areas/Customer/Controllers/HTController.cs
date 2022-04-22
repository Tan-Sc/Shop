using doan_webfix.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doan_webfix.Areas.Customer.Controllers
{
    public class HTController : Controller
    {

        private readonly ApplicationDbContext _db;


        public HTController(ApplicationDbContext db)
        {
            _db = db;
        }

         
        public IActionResult Index()
        {
          

            return View();
        }


    }
}
