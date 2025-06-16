using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Finansmart.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly DatabaseContext _context;

        public AvaliacaoController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int cursoId)
        {
            var avaliacoes = await _context.Avaliacoes
                .Where(a => a.CursoId == cursoId)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            ViewBag.CursoId = cursoId;
            return View(avaliacoes);
        }
       
        [HttpGet]
        public IActionResult Nova(int cursoId)
        {
            ViewBag.CursoId = cursoId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nova(Avaliacao model)
        {
            if (ModelState.IsValid)
            {
                model.DataAvaliacao = DateTime.Now;
                _context.Avaliacoes.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { cursoId = model.CursoId });
            }
            ViewBag.CursoId = model.CursoId;
            return View(model);
        }
    }
}
