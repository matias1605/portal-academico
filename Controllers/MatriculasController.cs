using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;

        public MatriculasController(ApplicationDbContext db, UserManager<IdentityUser> um)
        {
            _db = db;
            _um = um;
        }

        // GET: mostrar formulario de confirmación
        public async Task<IActionResult> Inscribir(int cursoId)
        {
            var curso = await _db.Cursos.FindAsync(cursoId);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: procesar inscripción con validaciones
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribir(int cursoId, string confirm)
        {
            var userId = _um.GetUserId(User)!;
            var curso = await _db.Cursos.FindAsync(cursoId);
            if (curso == null) return NotFound();

            // Validación 1: ya está matriculado en este curso
            bool yaInscrito = await _db.Matriculas
                .AnyAsync(m => m.CursoId == cursoId
                            && m.UsuarioId == userId
                            && m.Estado != EstadoMatricula.Cancelada);
            if (yaInscrito)
            {
                TempData["Error"] = "Ya estás inscrito en este curso.";
                return View(curso);
            }

            // Validación 2: cupo máximo
            int inscritos = await _db.Matriculas
                .CountAsync(m => m.CursoId == cursoId
                              && m.Estado != EstadoMatricula.Cancelada);
            if (inscritos >= curso.CupoMaximo)
            {
                TempData["Error"] = "El curso no tiene cupos disponibles.";
                return View(curso);
            }

            // Validación 3: solapamiento de horario con otros cursos matriculados
            var misCursos = await _db.Matriculas
                .Where(m => m.UsuarioId == userId
                         && m.Estado != EstadoMatricula.Cancelada)
                .Include(m => m.Curso)
                .ToListAsync();

            bool solapa = misCursos.Any(m =>
                m.Curso.HorarioInicio < curso.HorarioFin &&
                m.Curso.HorarioFin > curso.HorarioInicio);

            if (solapa)
            {
                TempData["Error"] = "El horario se solapa con otro curso en el que ya estás inscrito.";
                return View(curso);
            }

            // Todo OK: crear matrícula en estado Pendiente
            _db.Matriculas.Add(new Matricula
            {
                CursoId = cursoId,
                UsuarioId = userId,
                Estado = EstadoMatricula.Pendiente,
                FechaRegistro = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Te inscribiste exitosamente en {curso.Nombre}. Estado: Pendiente.";
            return RedirectToAction("Index", "Cursos");
        }

        // GET: mis matrículas
        public async Task<IActionResult> MisMatriculas()
        {
            var userId = _um.GetUserId(User)!;
            var lista = await _db.Matriculas
                .Where(m => m.UsuarioId == userId)
                .Include(m => m.Curso)
                .ToListAsync();
            return View(lista);
        }
    }
}