using System.Linq;
using System.Threading.Tasks;
using EstoqueWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstoqueWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly EstoqueWebContext _context;
        public HomeController(EstoqueWebContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            // variavel pedido receber todos pedidos
            var pedidos = await _context.Pedidos
            .Where(p => !p.DataPedido.HasValue)
            .Include(p => p.Cliente )
            .OrderByDescending(p => p.IdPedido)
            .AsNoTracking().ToListAsync();
            return View(pedidos);

        }

    }

}