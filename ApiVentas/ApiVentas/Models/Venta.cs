namespace ApiVentas.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public List<DetalleVenta> Detalles { get; set; }
    }
}
