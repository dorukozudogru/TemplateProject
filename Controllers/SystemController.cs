using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Models;

namespace TemplateProject.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        private readonly IdentityContext _context;

        public SystemController(IdentityContext context)
        {
            _context = context;
        }

        #region CarBrand
        public async Task<IActionResult> CarBrandIndex()
        {
            return View(await _context.CarBrands.OrderBy(x => x.Name).ToListAsync());
        }

        public IActionResult CarBrandCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CarBrandCreate(CarBrands carBrands)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carBrands);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CarBrandIndex));
            }
            return View(carBrands);
        }

        public async Task<IActionResult> CarBrandEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrands = await _context.CarBrands.FindAsync(id);
            if (carBrands == null)
            {
                return NotFound();
            }
            return View(carBrands);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CarBrandEdit(int id, CarBrands carBrands)
        {
            if (id != carBrands.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carBrands);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarBrandsExists(carBrands.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CarBrandIndex));
            }
            return View(carBrands);
        }

        public async Task<IActionResult> CarBrandDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrands = await _context.CarBrands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carBrands == null)
            {
                return NotFound();
            }

            return View(carBrands);
        }

        [HttpPost, ActionName("CarBrandDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CarBrandDeleteConfirmed(int id)
        {
            var hasAnyInsurance = await _context.Insurances
                .FirstOrDefaultAsync(m => m.CarModel.CarBrandId == id);

            if (hasAnyInsurance == null)
            {
                var carBrands = await _context.CarBrands.FindAsync(id);
                _context.CarBrands.Remove(carBrands);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CarBrandIndex));
            }
            throw new TaskCanceledException("Bu markaya ait sigorta kayıtları bulunmaktadır.");
        }

        private bool CarBrandsExists(int id)
        {
            return _context.CarBrands.Any(e => e.Id == id);
        }

        public ActionResult GetCarBrands()
        {
            return Json(_context.CarBrands.OrderBy(x => x.Name).ToList());
        }
        #endregion

        #region CarModel
        public async Task<IActionResult> CarModelIndex()
        {
            var identityContext = _context.CarModels.Include(c => c.CarBrand);
            return View(await identityContext.OrderBy(x => x.CarBrandId).ToListAsync());
        }

        public IActionResult CarModelCreate()
        {
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CarModelCreate(CarModels carModels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carModels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CarModelIndex));
            }
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", carModels.CarBrandId);
            return View(carModels);
        }

        public async Task<IActionResult> CarModelEdit(int? id)
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
        public async Task<IActionResult> CarModelEdit(int id, CarModels carModels)
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
                return RedirectToAction(nameof(CarModelIndex));
            }
            ViewData["CarBrandId"] = new SelectList(_context.CarBrands, "Id", "Name", carModels.CarBrandId);
            return View(carModels);
        }

        public async Task<IActionResult> CarModelDelete(int? id)
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

        [HttpPost, ActionName("CarModelDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CarModelDeleteConfirmed(int id)
        {
            var hasAnyInsurance = await _context.Insurances
                .FirstOrDefaultAsync(m => m.CarModelId == id);

            if (hasAnyInsurance == null)
            {
                var carModels = await _context.CarModels.FindAsync(id);
                _context.CarModels.Remove(carModels);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CarModelIndex));
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
        #endregion
    }
}