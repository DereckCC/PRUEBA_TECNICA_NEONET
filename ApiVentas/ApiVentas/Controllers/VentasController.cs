using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiVentas.Data;
using ApiVentas.Models;

namespace ApiVentas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostVenta([FromBody] VentaCreateVM model)
        {
            if (model == null || model.Detalles == null || !model.Detalles.Any())
                return BadRequest("Datos incompletos");

            var venta = new Venta
            {
                Fecha = DateTime.Now,
                ClienteId = model.ClienteId,
                Detalles = model.Detalles.Select(d => new DetalleVenta
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Cantidad * d.PrecioUnitario
                }).ToList()
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Venta registrada", VentaId = venta.Id });
        }


        [HttpGet("Historial")]
        public async Task<ActionResult<IEnumerable<object>>> Historial()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)   // incluir cliente
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();

            return ventas.Select(v => new
            {
                VentaId = v.Id,
                ClienteNombre = v.Cliente.Nombre,
                Fecha = v.Fecha,
                Total = v.Detalles.Sum(d => d.Subtotal)
            }).ToList();
        }




    }
}
