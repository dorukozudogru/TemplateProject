using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Models;
using TemplateProject.Models.ViewModels;

namespace TemplateProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IdentityContext _context;

        public HomeController(IdentityContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.CustomerCount = _context.Customers.Count();
            ViewBag.PassiveInsuranceCount = _context.Insurances.Where(i => i.IsActive == false).Count();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var pathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            Exception exception = pathFeature?.Error;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, RequestMessage = exception.Message.ToString() });
        }
    }
}
