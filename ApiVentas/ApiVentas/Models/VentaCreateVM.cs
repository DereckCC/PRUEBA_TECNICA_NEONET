namespace ApiVentas.Models
{
    public class VentaCreateVM
    {
        public int ClienteId { get; set; }
        public List<DetalleVentaVM> Detalles { get; set; }
    }
}
