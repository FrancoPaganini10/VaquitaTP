namespace Vaquita.Entidades
{
    public class Rubro
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public List<Gasto> Gastos { get; set; } = new List<Gasto>();
    }
}
