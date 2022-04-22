using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using doan_webfix.Data;
using doan_webfix.Extensions;
using doan_webfix.Models;
using doan_webfix.Models.ViewModel;
using doan_webfix.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doan_webfix.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private int PageSize = 5;
        private readonly ApplicationDbContext _db;
        

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int productPage = 1, string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            //kiểm tra tài khoản đang đăng nhập, lấy thông tin của tài khoản

            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Models.Appoinments>(),
            };

            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Appointments?productPage=:"); //phân trang
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }



            appointmentVM.Appointments = _db.Appoinments.Include(a => a.SalePerson).ToList();
            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalePersonId == claim.Value).ToList(); // HIỂN THỊ DANH SÁCH CÁC ĐƠN HÀNG MÀ USER ĐANG ĐĂNG NHẬP ĐƯỢC GIAO
            }


            if (searchName != null)
            {
                appointmentVM.Appointments = _db.Appoinments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = _db.Appoinments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = _db.Appoinments.Where(a => a.CustomerPhone.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = _db.Appoinments.Where(a => a.AppoinmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception)
                {

                }

            }

            var count = appointmentVM.Appointments.Count;

            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppoinmentDate)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();

            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };


            return View(appointmentVM);
        }




     
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
          
            var productList = (IEnumerable<Product>)(from p in _db.Products
                                                      join a in _db.ProductAddToAppointment
                                                      on p.ProductId equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p);
            List<CartItem> cart = new List<CartItem>();
            foreach (Product item in productList)
            {
                CartItem halo = new CartItem();
                var prd = _db.ProductAddToAppointment.Where(m => m.AppointmentId == id && m.ProductId == item.ProductId).FirstOrDefault();
                halo.itemCart = item;
                halo.quantity = prd.Count;
                halo.totalItem = prd.Total;

                cart.Add(halo);

            }

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appoinments.Include(a => a.SalePerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationAdminUser.ToList(),
                Products = cart,
            };

            return View(objAppointmentVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objAppointmentVM)
        {
            if (ModelState.IsValid)
            {
               
                var appointmentFromDb = _db.Appoinments.Where(a => a.Id == objAppointmentVM.Appointment.Id).FirstOrDefault();

                appointmentFromDb.CustomerName = objAppointmentVM.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = objAppointmentVM.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhone = objAppointmentVM.Appointment.CustomerPhone;
                appointmentFromDb.AppoinmentDate = objAppointmentVM.Appointment.AppoinmentDate;
                appointmentFromDb.AppointmentTime = objAppointmentVM.Appointment.AppointmentTime;
                appointmentFromDb.isConfirmed = objAppointmentVM.Appointment.isConfirmed;
                appointmentFromDb.status = objAppointmentVM.Appointment.status;
                if (User.IsInRole(SD.SuperAdminEndUser))
                {
                    appointmentFromDb.SalePersonId = objAppointmentVM.Appointment.SalePersonId;
                }
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));

            }
            return View(objAppointmentVM);
        }



        //GET Detials
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Product>)(from p in _db.Products
                                                     join a in _db.ProductAddToAppointment
                                                     on p.ProductId equals a.ProductId
                                                     where a.AppointmentId == id
                                                     select p).Include("ProductTypes");


            List<CartItem> cart = new List<CartItem>();
            foreach (Product item in productList)
            {
                CartItem halo = new CartItem();
                var prd = _db.ProductAddToAppointment.Where(m => m.AppointmentId == id && m.ProductId == item.ProductId).FirstOrDefault();
                halo.itemCart = item;
                halo.quantity = prd.Count;
                halo.totalItem = prd.Total;

                cart.Add(halo);
            }


            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appoinments.Include(a => a.SalePerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationAdminUser.ToList(),
                Products = cart,
            };

            return View(objAppointmentVM);

        }



        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Product>)(from p in _db.Products
                                                     join a in _db.ProductAddToAppointment
                                                     on p.ProductId equals a.ProductId
                                                     where a.AppointmentId == id
                                                     select p).Include("ProductTypes");


            List<CartItem> cart = new List<CartItem>();
            foreach (Product item in productList)
            {
                CartItem halo = new CartItem();
                var prd = _db.ProductAddToAppointment.Where(m => m.AppointmentId == id && m.ProductId == item.ProductId).FirstOrDefault();
                halo.itemCart = item;
                halo.quantity = prd.Count;
                halo.totalItem = prd.Total;

                cart.Add(halo);
            }


            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = _db.Appoinments.Include(a => a.SalePerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationAdminUser.ToList(),
                Products = cart,
            };

            return View(objAppointmentVM);

        }


        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _db.Appoinments.FindAsync(id);
            _db.Appoinments.Remove(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}