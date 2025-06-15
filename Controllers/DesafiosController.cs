using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Finansmart.Controllers
{
    public class DesafiosController : Controller
    {
        private readonly DatabaseContext _context;

        public DesafiosController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: /Desafios
        public async Task<IActionResult> Index()
        {
            var desafios = await _context.Desafios.ToListAsync();
            return View(desafios);
        }
    }
}
