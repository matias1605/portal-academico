using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CoordinadorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Panel principal — lista de cursos
        public async Task<IActionResult> Index()
        {
            var cursos = await _db.Cursos.ToListAsync();
            return View(cursos);
        }

        // Crear curso — GET
        public IActionResult Crear()
        {
            return View();
        }

        // Crear curso — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Curso curso)
        {
            if (curso.HorarioFin <= curso.HorarioInicio)
                ModelState.AddModelError("", "El horario de fin debe ser posterior al de inicio.");

            if (curso.Creditos < 1)
                ModelState.AddModelError("Creditos", "Los créditos deben ser mayores a 0.");

            if (ModelState.IsValid)
            {
                curso.Activo = true;
                _db.Cursos.Add(curso);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Curso creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // Editar curso — GET
        public async Task<IActionResult> Editar(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // Editar curso — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Curso curso)
        {
            if (curso.HorarioFin <= curso.HorarioInicio)
                ModelState.AddModelError("", "El horario de fin debe ser posterior al de inicio.");

            if (curso.Creditos < 1)
                ModelState.AddModelError("Creditos", "Los créditos deben ser mayores a 0.");

            if (ModelState.IsValid)
            {
                _db.Cursos.Update(curso);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Curso actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // Desactivar curso — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = false;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Curso desactivado.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Lista de matrículas por curso
        public async Task<IActionResult> Matriculas(int cursoId)
        {
            var curso = await _db.Cursos.FindAsync(cursoId);
            if (curso == null) return NotFound();

            var matriculas = await _db.Matriculas
                .Where(m => m.CursoId == cursoId)
                .Include(m => m.Curso)
                .ToListAsync();

            ViewBag.CursoNombre = curso.Nombre;
            ViewBag.CursoId = cursoId;
            return View(matriculas);
        }

        // Confirmar matrícula — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int id, int cursoId)
        {
            var matricula = await _db.Matriculas.FindAsync(id);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Confirmada;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Matriculas), new { cursoId });
        }

        // Cancelar matrícula — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, int cursoId)
        {
            var matricula = await _db.Matriculas.FindAsync(id);
            if (matricula != null)
            {
                matricula.Estado = EstadoMatricula.Cancelada;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Matriculas), new { cursoId });
        }
    }
}