using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using doan_webfix.Data;
using doan_webfix.Models.ViewModel;
using doan_webfix.Models;
using Microsoft.EntityFrameworkCore;
using doan_webfix.Extensions;
using Microsoft.AspNetCore.Http;


namespace doan_webfix.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int PageSize = 16;
        public ShopController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int productpage=1,string searchName = null,string searchType=null)
        {

            ProductViewPage productVM = new ProductViewPage()
            {
                Products = new List<Models.Product>(),
            };

            StringBuilder param = new StringBuilder();

            param.Append("/Customer/Shop?productPage=:"); //phân trang


            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            param.Append("&searchType=");
            if (searchType != null)
            {
                param.Append(searchType);
            }

            productVM.Products = _db.Products.Include(m => m.ProductTypes).ToList();

             if (searchName != null)
            {
                productVM.Products = await _db.Products.Include(a=>a.ProductTypes).Where(a => a.ProductName.ToLower().Contains(searchName.ToLower())).ToListAsync();
            }
            if (searchType != null)
            {
                productVM.Products = await _db.Products.Where(a => a.ProductTypes.ProductTypeName == searchType).ToListAsync();
            }


            var count = productVM.Products.Count;
            productVM.Products = productVM.Products.OrderBy(p => p.ProductId)
                .Skip((productpage - 1) * PageSize)
                .Take(PageSize).ToList();

            productVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productpage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            return View(productVM);
        }



        // chi tiet
        public async Task<IActionResult> Details(int id)
        {
            var Products = await _db.Products.Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.ProductId == id);
            CartItem item = new CartItem();
            item.itemCart = Products;
            if (Products == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPOST(int id,int quantity)
        {
            CartItem item = new CartItem();
            item.itemCart = await _db.Products.Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.ProductId == id);
            item.quantity = quantity;
            item.totalItem = quantity * item.itemCart.Price;
            if (item.itemCart.newPrice!=0)
            {
                item.totalItem = quantity * item.itemCart.newPrice;
            }
            
            List<CartItem> lsShopngCart = HttpContext.Session.Get<List<CartItem>>("ssShopingCart");
            if (lsShopngCart==null)
            {
                lsShopngCart = new List<CartItem>();
            }
            lsShopngCart.Add(item);
            HttpContext.Session.Set("ssShopingCart", lsShopngCart);
            return RedirectToAction("Index","Shop",new { area="Customer"});
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
    }
}