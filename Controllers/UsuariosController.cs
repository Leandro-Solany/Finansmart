using Finansmart.Data;
using Finansmart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Finansmart.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DatabaseContext _context;

        public UsuariosController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return View(usuarios);
        }

        // Se quiser adicionar um formulário de cadastro:
        [HttpGet]
        public IActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Novo(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.DataCadastro = DateTime.Now;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }
    }
}
