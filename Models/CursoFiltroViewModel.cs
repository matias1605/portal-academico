namespace PortalAcademico.Models
{
    public class CursoFiltroViewModel
    {
        public string? Nombre { get; set; }
        public int? CreditosMin { get; set; }
        public int? CreditosMax { get; set; }
        public string? HorarioDesde { get; set; }
        public string? HorarioHasta { get; set; }
        public IEnumerable<Curso> Cursos { get; set; } = new List<Curso>();
    }
}