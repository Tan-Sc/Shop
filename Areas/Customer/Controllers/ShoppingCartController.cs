using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using doan_webfix.Data;
using doan_webfix.Models.ViewModel;
using doan_webfix.Extensions;
using Microsoft.AspNetCore.Http;
using doan_webfix.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace doan_webfix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;

            ShoppingCartVM = new ShoppingCartViewModel()
            {
                cartItems = new List<Models.ViewModel.CartItem>(),
                customer = new ApplicationCustomerAccount(),
            };

        }
        //get Index Shopping Cart

        public async Task<IActionResult> Index()
        {

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser userFromDb = new ApplicationUser();
            if (claims != null)
            {
                userFromDb = await _db.ApplicationUsers.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();
                ShoppingCartVM.customer.Name = userFromDb.Name;
                ShoppingCartVM.customer.Email = userFromDb.Email;
                ShoppingCartVM.customer.PhoneNumber = userFromDb.PhoneNumber;
                ShoppingCartVM.customer.Address = userFromDb.Address;
            }


            List<CartItem> lstShoppingCart = HttpContext.Session.Get<List<CartItem>>("ssShopingCart");
            if (lstShoppingCart != null)
            {
                foreach (CartItem item in lstShoppingCart)
                {
                    CartItem cart = new CartItem();
                    Product pro = _db.Products.Include(p => p.ProductTypes).Where(p => p.ProductId == item.itemCart.ProductId).FirstOrDefault();

                    cart.itemCart = pro;
                    cart.quantity = item.quantity;
                    cart.totalItem = pro.Price * item.quantity;
                    if (pro.newPrice != 0)
                    {
                        cart.totalItem = pro.newPrice * item.quantity;
                    }
                    ShoppingCartVM.TotalApointment += cart.totalItem;
                    ShoppingCartVM.cartItems.Add(cart);
                }
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<CartItem> lstCartItems = HttpContext.Session.Get<List<CartItem>>("ssShopingCart");

            ShoppingCartVM.Appoinments.AppoinmentDate = ShoppingCartVM.Appoinments.AppoinmentDate;
            ShoppingCartVM.Appoinments.AppointmentTime = ShoppingCartVM.Appoinments.AppointmentTime;
            ShoppingCartVM.Appoinments.TotalAppointment = ShoppingCartVM.TotalApointment;


            Appoinments appointments = ShoppingCartVM.Appoinments;
            _db.Appoinments.Add(appointments);
            _db.SaveChanges();

            int appointmentId = appointments.Id;

            //tạo session thông báo
            List<int> lsNotification = HttpContext.Session.Get<List<int>>("Notification");
            if(lsNotification==null)
            {
                lsNotification = new List<int>();
            }
            lsNotification.Add(appointmentId);
            HttpContext.Session.Set("Notification", lsNotification);

            ShoppingCartVM.TotalApointment = 0;

            foreach (CartItem item in lstCartItems)
            {
                ProductAddToAppointment productsSelectedForAppointment = new ProductAddToAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = item.itemCart.ProductId,
                    Count = item.quantity,
                    Total = item.totalItem,
                };
                _db.ProductAddToAppointment.Add(productsSelectedForAppointment);
            }
            _db.SaveChanges();
            lstCartItems = new List<CartItem>();
            HttpContext.Session.Set("ssShopingCart", lstCartItems);

            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });

        }
        public IActionResult Remove(int id)
        {
            List<CartItem> lsShopngCart = HttpContext.Session.Get<List<CartItem>>("ssShopingCart");
            CartItem item = new CartItem();
            if (lsShopngCart != null)
            {
                if (lsShopngCart.Exists(x => x.itemCart.ProductId == id))
                {
                    item = lsShopngCart.Find(m => m.itemCart.ProductId == id);
                    lsShopngCart.Remove(item);
                }
            }

            HttpContext.Session.Set("ssShopingCart", lsShopngCart);
            return RedirectToAction("Index", "Shop", new { area = "Customer" });
        }

        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appoinments = _db.Appoinments.Where(a => a.Id == id).FirstOrDefault();
            List<ProductAddToAppointment> objProdList = _db.ProductAddToAppointment.Where(p => p.AppointmentId == id).ToList();

       
            ShoppingCartVM.TotalApointment = 0;
            foreach (ProductAddToAppointment prodAptObj in objProdList)
            {
                CartItem cart = new CartItem();
                cart.itemCart = _db.Products.Include(p => p.ProductTypes).Where(p => p.ProductId == prodAptObj.ProductId).FirstOrDefault();
                cart.quantity = prodAptObj.Count;
                cart.totalItem = prodAptObj.Total;
                ShoppingCartVM.cartItems.Add(cart);
                ShoppingCartVM.TotalApointment += cart.totalItem;
            }

            return View(ShoppingCartVM);
        }

    }
}