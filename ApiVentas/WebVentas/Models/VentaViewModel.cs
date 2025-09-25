namespace WebVentas.Models
{
    public class VentaViewModel
    {
        public int ClienteId { get; set; }
        public List<int> Productos { get; set; } = new List<int>();
    }
}
