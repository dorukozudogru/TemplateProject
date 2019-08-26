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
    public class CarModelController : Controller
    {
        private readonly IdentityContext _context;

        public CarModelController(IdentityContext context)
        {
            _context = context;
        }

        
    }
}
