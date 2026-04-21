using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PortalAcademico.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Curso> Cursos { get; set; } = null!;
    public DbSet<Matricula> Matriculas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Código único en Curso
        builder.Entity<Curso>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        // Un usuario no puede estar 2 veces en el mismo curso
        builder.Entity<Matricula>()
            .HasIndex(m => new { m.CursoId, m.UsuarioId })
            .IsUnique();

        // Seed: 3 cursos activos
        builder.Entity<Curso>().HasData(
            new Curso { Id = 1, Codigo = "MAT101", Nombre = "Matemáticas I",
                Creditos = 4, CupoMaximo = 30,
                HorarioInicio = new TimeSpan(8, 0, 0),
                HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
            new Curso { Id = 2, Codigo = "FIS201", Nombre = "Física II",
                Creditos = 3, CupoMaximo = 25,
                HorarioInicio = new TimeSpan(10, 0, 0),
                HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
            new Curso { Id = 3, Codigo = "PRG301", Nombre = "Programación Web",
                Creditos = 5, CupoMaximo = 20,
                HorarioInicio = new TimeSpan(14, 0, 0),
                HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
        );
    }
}
