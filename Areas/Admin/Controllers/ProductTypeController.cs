using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using doan_webfix.Data;
using doan_webfix.Models;

namespace doan_webfix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.ProductTypes.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType model)
        {
            if (ModelState.IsValid)
            {
                _db.Add(model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        //Get Edit method
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = await _db.ProductTypes.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductType model)
        {
            if (id != model.ProductTypeId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpGet]
        //GET Detail Method
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = await _db.ProductTypes.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = await _db.ProductTypes.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMethod(int id)
        {
            var model = await _db.ProductTypes.FindAsync(id);
            _db.Remove(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}