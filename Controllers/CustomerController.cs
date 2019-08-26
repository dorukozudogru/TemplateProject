﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Models;
using static TemplateProject.Helpers.ProcessCollectionHelper;

namespace TemplateProject.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IdentityContext _context;

        public CustomerController(IdentityContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(bool isActive)
        {
            var requestFormData = Request.Form;

            List<Customers> customers = await _context.Customers
                .Where(c => c.IsActive == isActive)
                .AsNoTracking()
                .ToListAsync();

            List<Customers> listItems = ProcessCollection(customers, requestFormData);

            var response = new PaginatedResponse<Customers>
            {
                Data = listItems,
                Draw = int.Parse(requestFormData["draw"]),
                RecordsFiltered = customers.Count,
                RecordsTotal = customers.Count
            };

            return Ok(response);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customers customers)
        {
            if (ModelState.IsValid)
            {
                customers.CreatedBy = GetLoggedUserId();

                _context.Add(customers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            return View(customers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customers customers)
        {
            if (id != customers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customers.UpdatedBy = GetLoggedUserId();

                    _context.Update(customers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomersExists(customers.Id))
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
            return View(customers);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customers == null)
            {
                return NotFound();
            }

            return View(customers);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasAnyInsurance = await _context.Insurances
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (hasAnyInsurance == null)
            {
                var customers = await _context.Customers.FindAsync(id);

                customers.DeletedBy = GetLoggedUserId();
                customers.IsActive = false;

                _context.Update(customers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            throw new TaskCanceledException("Bu müşteriye ait sigorta kayıtları bulunmaktadır.");
        }

        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
        public string GetLoggedUserId()
        {
            return this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

    }
}
