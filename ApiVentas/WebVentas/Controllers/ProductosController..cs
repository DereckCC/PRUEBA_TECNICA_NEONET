using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using WebVentas.Models;

namespace WebVentas.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductosController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiVentas");
            var productos = await client.GetFromJsonAsync<List<ProductoVM>>("api/Productos");
            return View(productos);
        }
    }
}
