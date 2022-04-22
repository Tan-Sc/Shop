using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using doan_webfix.Data;
using doan_webfix.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


//using Microsoft.AspNetCore.Hosting.Internal;
using System.IO;
using doan_webfix.Utility;
using System.Text;
using doan_webfix.Models;

namespace doan_webfix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        
         private readonly IWebHostEnvironment _hostingEnvironment;
        //    private readonly HostingEnvironment _hostingEnvironment;
        private int PageSize = 20;

        [BindProperty]
        public ProductViewModels ProductsVM { get; set; }
        public ProductViewPage pvp { get; set; }


       

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductViewModels()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                Products = new Models.Product(),
            };
            pvp = new ProductViewPage() {
                Products = new List<Models.Product>()
            };
           
        }

      


        public async Task<IActionResult> Index(int productpage= 1)
        {

            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Product?productPage=:"); //phân trang

            pvp.Products = await _db.Products.Include(m => m.ProductTypes).ToListAsync();
            var count = pvp.Products.Count;    
            pvp.Products = pvp.Products.OrderBy(p => p.ProductId)

                .Skip((productpage - 1) * PageSize)
                .Take(PageSize).ToList();



            pvp.PagingInfo = new PagingInfo
            {
                CurrentPage = productpage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            return View(pvp);
        }


        public IActionResult Create()
        {
            return View(ProductsVM);
        }
        [HttpPost, ActionName("Create")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }


            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();

            //Image will be safe here
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var ProductsFromDB = _db.Products.Find(ProductsVM.Products.ProductId);
            if (files.Count != 0)
            {
                //Image has been upload
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.ProductId + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                ProductsFromDB.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.ProductId + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.ProductId + ".png");
                ProductsFromDB.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.ProductId + ".png";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        ///Get Action Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.ProductId == id);
          
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.ProductId == ProductsVM.Products.ProductId).FirstOrDefault();

                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.ProductId + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.ProductId + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.ProductId + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    ProductsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.ProductId + extension_new;
                }

                if (ProductsVM.Products.Image != null)
                {
                    productFromDb.Image = ProductsVM.Products.Image;
                }

                productFromDb.ProductName = ProductsVM.Products.ProductName;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.newPrice = ProductsVM.Products.newPrice;
                productFromDb.ProductType_Id = ProductsVM.Products.ProductType_Id;
                productFromDb.ProductSize = ProductsVM.Products.ProductSize;
                productFromDb.amount = ProductsVM.Products.amount;
                productFromDb.Mota = ProductsVM.Products.Mota;
                productFromDb.isNew = ProductsVM.Products.isNew;
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ProductsVM);
        }


        //get details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.ProductId == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.ProductId == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }
        //POST : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, products.ProductId + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.ProductId + extension));
                }
                _db.Products.Remove(products);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}