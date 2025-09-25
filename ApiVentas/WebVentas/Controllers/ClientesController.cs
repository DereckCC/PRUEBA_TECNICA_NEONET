using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebVentas.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiVentas");
            var clientes = await client.GetFromJsonAsync<List<dynamic>>("api/Clientes");
            return View(clientes);
        }
    }
}
