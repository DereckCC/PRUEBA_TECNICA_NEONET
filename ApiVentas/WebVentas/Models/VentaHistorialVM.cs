namespace WebVentas.Models
{
    public class VentaHistorialVM
    {
        public int VentaId { get; set; }
        public string ClienteNombre { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
    }
}
