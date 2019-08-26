﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Models;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using TemplateProject.Helpers;
using static TemplateProject.Helpers.ProcessCollectionHelper;

namespace TemplateProject.Controllers
{
    [Authorize]
    public class InsuranceController : Controller
    {
        private readonly IdentityContext _context;

        public InsuranceController(IdentityContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            FakeSession.Instance.Obj = JsonConvert.SerializeObject(_context.Insurances.Where(i => i.IsActive == true));
            return View();
        }

        public IActionResult PassiveInsurances()
        {
            FakeSession.Instance.Obj = JsonConvert.SerializeObject(_context.Insurances.Where(i => i.IsActive == false));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(bool isActive)
        {
            var requestFormData = Request.Form;

            List<Insurances> insurances = await _context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .Where(i => i.IsActive == isActive)
                .AsNoTracking()
                .ToListAsync();

            List<Insurances> listItems = ProcessCollection(insurances, requestFormData);

            var response = new PaginatedResponse<Insurances>
            {
                //Data = listItems,
                Draw = int.Parse(requestFormData["draw"]),
                RecordsFiltered = insurances.Count,
                RecordsTotal = insurances.Count
            };

            return Ok(response);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.Id == id);

            #region InsuranceType
            if (insurance.InsuranceType == 0)
            {
                insurance.InsuranceTypeName = "SIFIR";
            }
            else if (insurance.InsuranceType == 1)
            {
                insurance.InsuranceTypeName = "YENİLEME";
            }
            else if (insurance.InsuranceType == 2)
            {
                insurance.InsuranceTypeName = "2. EL";
            }
            #endregion

            #region InsurancePaymentType
            if (insurance.InsurancePaymentType == 0)
            {
                insurance.InsurancePaymentTypeName = "NAKİT";
            }
            else if (insurance.InsurancePaymentType == 1)
            {
                insurance.InsurancePaymentTypeName = "KREDİ KARTI";
            }
            else if (insurance.InsurancePaymentType == 2)
            {
                insurance.InsurancePaymentTypeName = "BEDELSİZ";
            }
            #endregion

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Insurances insurance)
        {
            if (ModelState.IsValid)
            {
                insurance.CreatedBy = GetLoggedUserId();

                insurance.CarModelId = _context.CarModels.FirstOrDefault(x => x.Name == insurance.CarModel.Name).Id;
                insurance.CarModel.CarBrandId = _context.CarBrands.FirstOrDefault(x => x.Name == insurance.CarModel.CarBrand.Name).Id;

                _context.Add(insurance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(insurance);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (insurance == null)
            {
                return NotFound();
            }

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName");
            ViewBag.CarBrands = new SelectList(_context.CarBrands.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.CarModels = new SelectList(_context.CarModels.OrderBy(x => x.Name), "Name", "Name");

            return View(insurance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Insurances insurance)
        {
            if (id != insurance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    insurance.UpdatedBy = GetLoggedUserId();

                    insurance.CarModelId = _context.CarModels.FirstOrDefault(x => x.Name == insurance.CarModel.Name).Id;

                    _context.Update(insurance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceExists(insurance.Id))
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
            return View(insurance);
        }

        public async Task<IActionResult> Passive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.Id == id);

            #region InsuranceType
            if (insurance.InsuranceType == 0)
            {
                insurance.InsuranceTypeName = "SIFIR";
            }
            else if (insurance.InsuranceType == 1)
            {
                insurance.InsuranceTypeName = "YENİLEME";
            }
            else if (insurance.InsuranceType == 2)
            {
                insurance.InsuranceTypeName = "2. EL";
            }
            #endregion

            #region InsurancePaymentType
            if (insurance.InsurancePaymentType == 0)
            {
                insurance.InsurancePaymentTypeName = "NAKİT";
            }
            else if (insurance.InsurancePaymentType == 1)
            {
                insurance.InsurancePaymentTypeName = "KREDİ KARTI";
            }
            else if (insurance.InsurancePaymentType == 2)
            {
                insurance.InsurancePaymentTypeName = "BEDELSİZ";
            }
            #endregion

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        [HttpPost, ActionName("Passive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PassiveConfirmed(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);

            insurance.IsActive = false;
            insurance.DeletedBy = GetLoggedUserId();

            _context.Update(insurance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.Id == id);

            #region InsuranceType
            if (insurance.InsuranceType == 0)
            {
                insurance.InsuranceTypeName = "SIFIR";
            }
            else if (insurance.InsuranceType == 1)
            {
                insurance.InsuranceTypeName = "YENİLEME";
            }
            else if (insurance.InsuranceType == 2)
            {
                insurance.InsuranceTypeName = "2. EL";
            }
            #endregion

            #region InsurancePaymentType
            if (insurance.InsurancePaymentType == 0)
            {
                insurance.InsurancePaymentTypeName = "NAKİT";
            }
            else if (insurance.InsurancePaymentType == 1)
            {
                insurance.InsurancePaymentTypeName = "KREDİ KARTI";
            }
            else if (insurance.InsurancePaymentType == 2)
            {
                insurance.InsurancePaymentTypeName = "BEDELSİZ";
            }
            #endregion

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);

            _context.Insurances.Remove(insurance);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Revoke(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        [HttpPost, ActionName("Revoke")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeConfirmed(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);

            insurance.IsActive = true;
            insurance.UpdatedBy = GetLoggedUserId();

            _context.Update(insurance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public bool InsuranceExists(int id)
        {
            return _context.Insurances.Any(e => e.Id == id);
        }

        [Route("insurance/insurance-export")]
        public ActionResult ExportAllInsurances()
        {
            var stream = ExportInsurance(JsonConvert.DeserializeObject<List<Insurances>>(FakeSession.Instance.Obj), 1);
            string fileName = String.Format("{0}.xlsx", "all_insurances_report");
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;
            return File(stream, fileType, fileName);
        }

        [Route("insurance/insurance-active-export")]
        public ActionResult ExportAllActiveInsurances()
        {
            var stream = ExportInsurance(_context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .Where(i => i.IsActive == true).ToList(), 0);
            string fileName = String.Format("{0}.xlsx", "all_active_insurances_report");
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;
            return File(stream, fileType, fileName);
        }

        [Route("insurance/insurance-passive-export")]
        public ActionResult ExportAllPassiveInsurances()
        {
            var stream = ExportInsurance(_context.Insurances
                .Include(cu => cu.Customer)
                .Include(c => c.CarModel)
                .Include(cb => cb.CarModel.CarBrand)
                .Where(i => i.IsActive == false).ToList(), 0);
            string fileName = String.Format("{0}.xlsx", "all_passive_insurances_report");
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;
            return File(stream, fileType, fileName);
        }

        public MemoryStream ExportInsurance(List<Insurances> items, byte type)
        {
            var stream = new System.IO.MemoryStream();

            using (var p = new ExcelPackage(stream))
            {
                var ws = p.Workbook.Worksheets.Add("Poliçeler");

                using (var range = ws.Cells[1, 1, 1, 16])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(color: Color.Black);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                ws.Cells[1, 1].Value = "ID";
                ws.Cells[1, 2].Value = "Poliçe Tipi";
                ws.Cells[1, 3].Value = "Poliçe Numarası";
                ws.Cells[1, 4].Value = "Sigorta Firması";
                ws.Cells[1, 5].Value = "Marka";
                ws.Cells[1, 6].Value = "Model";
                ws.Cells[1, 7].Value = "Plaka";
                ws.Cells[1, 8].Value = "Müşteri Adı Soyadı";
                ws.Cells[1, 9].Value = "Müşteri Telefonu";
                ws.Cells[1, 10].Value = "Müşteri E-Postası";
                ws.Cells[1, 11].Value = "Poliçe Başlama Tarihi";
                ws.Cells[1, 12].Value = "Poliçe Bitiş Tarihi";
                ws.Cells[1, 13].Value = "Poliçe Tutarı";
                ws.Cells[1, 14].Value = "Poliçe Primi";
                ws.Cells[1, 15].Value = "Sıfır/Yenileme";
                ws.Cells[1, 16].Value = "Nakit/Kredi Kartı";

                ws.Column(11).Style.Numberformat.Format = "dd-mmmm-yyyy";
                ws.Column(12).Style.Numberformat.Format = "dd-mmmm-yyyy";

                ws.Row(1).Style.Font.Bold = true;

                for (int c = 2; c < items.Count + 2; c++)
                {
                    ws.Cells[c, 1].Value = items[c - 2].Id;
                    ws.Cells[c, 2].Value = "";
                    ws.Cells[c, 3].Value = "";
                    ws.Cells[c, 4].Value = "";
                    ws.Cells[c, 5].Value = items[c - 2].CarModel.CarBrand.Name;
                    ws.Cells[c, 6].Value = items[c - 2].CarModel.Name;
                    ws.Cells[c, 7].Value = items[c - 2].LicencePlate;
                    ws.Cells[c, 8].Value = items[c - 2].Customer.FullName;
                    ws.Cells[c, 9].Value = items[c - 2].Customer.Phone;
                    ws.Cells[c, 10].Value = items[c - 2].Customer.Email;
                    ws.Cells[c, 11].Value = items[c - 2].InsuranceStartDate;
                    ws.Cells[c, 12].Value = items[c - 2].InsuranceFinishDate;
                    ws.Cells[c, 13].Value = items[c - 2].InsuranceAmount;
                    ws.Cells[c, 14].Value = items[c - 2].InsuranceBonus;

                    #region InsuranceType
                    if (items[c - 2].InsuranceType == 0)
                    {
                        ws.Cells[c, 15].Value = "SIFIR";
                    }
                    else if (items[c - 2].InsuranceType == 1)
                    {
                        ws.Cells[c, 15].Value = "YENİLEME";
                    }
                    else if (items[c - 2].InsuranceType == 2)
                    {
                        ws.Cells[c, 15].Value = "2. EL";
                    }
                    #endregion

                    #region InsurancePaymentType
                    if (items[c - 2].InsurancePaymentType == 0)
                    {
                        ws.Cells[c, 16].Value = "NAKİT";
                    }
                    else if (items[c - 2].InsurancePaymentType == 1)
                    {
                        ws.Cells[c, 16].Value = "KREDİ KARTI";
                    }
                    else if (items[c - 2].InsurancePaymentType == 2)
                    {
                        ws.Cells[c, 16].Value = "BEDELSİZ";
                    }
                    #endregion
                }

                var lastRow = ws.Dimension.End.Row;
                var lastColumn = ws.Dimension.End.Column;

                if (type == 1)
                {
                    using (var range = ws.Cells[lastRow + 1, 1, lastRow + 1, lastColumn])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(color: Color.Gray);
                        range.Style.Font.Color.SetColor(Color.White);
                    }

                    ws.Cells[lastRow + 1, 12].Value = "Toplam:";
                    ws.Cells[lastRow + 1, 13].Formula = String.Format("SUM(M2:M{0})", lastRow);
                    ws.Cells[lastRow + 1, 14].Formula = String.Format("SUM(N2:N{0})", lastRow);
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Cells["A1:P" + items.Count + 2].AutoFilter = true;

                ws.Column(16).PageBreak = true;
                ws.PrinterSettings.PaperSize = ePaperSize.A4;
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.Scale = 50;

                p.Save();
            }
            return stream;
        }

        //public InsurancePolicies GetInsurancePoliciesById(int Id)
        //{
        //    return _context.InsurancePolicies.FirstOrDefault(x => x.Id == Id);
        //}

        //public InsuranceCompanies GetInsuranceCompaniesById(int Id)
        //{
        //    return _context.InsuranceCompanies.FirstOrDefault(x => x.Id == Id);
        //}

        public string GetLoggedUserId()
        {
            return this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public class FakeSession
        {
            private static FakeSession _instance;

            public static FakeSession Instance
            {
                get
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }
                    _instance = new FakeSession();
                    return _instance;
                }
            }

            public string Obj { get; set; }
            public byte[] Buffer { get; set; }

        }
    }
}
