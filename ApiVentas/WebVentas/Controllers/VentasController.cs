using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

using System.Text.Json.Serialization;
using WebVentas.Models;

namespace WebVentas.Controllers
{
    public class VentasController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VentasController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient("ApiVentas");
            var clientes = await client.GetFromJsonAsync<List<ClienteVM>>("api/Clientes");
            var productos = await client.GetFromJsonAsync<List<ProductoVM>>("api/Productos");

            ViewBag.Clientes = clientes;
            ViewBag.Productos = productos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] VentaCreateVM venta)
        {
            try
            {
                // ✅ Deserializar los detalles que vienen como JSON desde el hidden input
                if (!string.IsNullOrEmpty(venta.DetallesJson))
                {
                    venta.Detalles = JsonConvert.DeserializeObject<List<DetalleVentaVM>>(venta.DetallesJson);
                }

                // Configurar el cliente para llamar a la API
                var client = _httpClientFactory.CreateClient("ApiVentas");
                var response = await client.PostAsJsonAsync("api/Ventas", new
                {
                    ClienteId = venta.ClienteId,
                    Detalles = venta.Detalles.Select(d => new
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    })
                });

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "✅ Venta registrada correctamente.";
                    return RedirectToAction("Historial");
                }
                else
                {
                    TempData["Error"] = "❌ No se pudo registrar la venta.";
                    return View(venta);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error inesperado: {ex.Message}";
                return View(venta);
            }
        }



        public async Task<IActionResult> Historial()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiVentas");

                // Ahora apunta a /api/Ventas/Historial (sin clienteId)
                var response = await client.GetFromJsonAsync<List<VentaHistorialVM>>("api/Ventas/Historial");

                return View(response ?? new List<VentaHistorialVM>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo obtener el historial: " + ex.Message;
                return View(new List<VentaHistorialVM>());
            }
        }



    }
}
