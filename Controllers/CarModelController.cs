﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Models;

namespace TemplateProject.Controllers
{
    [Authorize]
    public class CarModelController : Controller
    {
        private readonly IdentityContext _context;

        public CarModelController(IdentityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var identityContext = _context.CarModels.Include(c => c.CarBrand);
            return View(await identityContext.OrderBy(x => x.CarBrandId).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarModels carModels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carModels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", carModels.CarBrandId);
            return View(carModels);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModels = await _context.CarModels.FindAsync(id);
            if (carModels == null)
            {
                return NotFound();
            }
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", carModels.CarBrandId);
            return View(carModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarModels carModels)
        {
            if (id != carModels.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carModels);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarModelsExists(carModels.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", carModels.CarBrandId);
            return View(carModels);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carModels = await _context.CarModels
                .Include(c => c.CarBrand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carModels == null)
            {
                return NotFound();
            }

            return View(carModels);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasAnyInsurance = await _context.Insurances
                .FirstOrDefaultAsync(m => m.CarModelId == id);
            
            if (hasAnyInsurance == null)
            {
                var carModels = await _context.CarModels.FindAsync(id);
                _context.CarModels.Remove(carModels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            throw new TaskCanceledException("Bu modele ait sigorta kayıtları bulunmaktadır.");
        }

        private bool CarModelsExists(int id)
        {
            return _context.CarModels.Any(e => e.Id == id);
        }

        public ActionResult GetCarModelsById(int Id)
        {
            return Json(_context.CarModels.Where(x => x.CarBrandId == Id).OrderBy(x => x.Name).ToList());
        }
    }
}
