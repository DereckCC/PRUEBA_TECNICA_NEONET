using System.Collections.Generic;

namespace WebVentas.Models
{
    public class VentaCreateVM
    {
        public int ClienteId { get; set; }

        // El JSON que viene desde el hidden input en Create.cshtml
        public string DetallesJson { get; set; }

        // La lista deserializada de detalles de venta
        public List<DetalleVentaVM> Detalles { get; set; } = new List<DetalleVentaVM>();
    }
}
