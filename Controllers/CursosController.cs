//using Finansmart.Data;
//using Finansmart.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Finansmart.Controllers
//{
//    public class CursosController : Controller
//    {
//        private readonly DatabaseContext _context;

//        public CursosController(DatabaseContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
//        {
//            var total = await _context.Cursos.CountAsync();
//            var cursos = await _context.Cursos
//                .OrderBy(c => c.Nome)
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync();

//            var model = new CursosListViewModel
//            {
//                Cursos = cursos,
//                Page = page,
//                PageSize = pageSize,
//                Total = total
//            };

//            return View(model);
//        }
//    }
//}
