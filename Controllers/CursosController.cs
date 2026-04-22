using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CursosController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(CursoFiltroViewModel filtro)
        {
            var query = _db.Cursos.Where(c => c.Activo).AsQueryable();

            if (!string.IsNullOrEmpty(filtro.Nombre))
                query = query.Where(c => c.Nombre.Contains(filtro.Nombre));

            if (filtro.CreditosMin.HasValue)
            {
                if (filtro.CreditosMin.Value < 1)
                    ModelState.AddModelError("CreditosMin", "Los créditos no pueden ser negativos.");
                else
                    query = query.Where(c => c.Creditos >= filtro.CreditosMin.Value);
            }

            if (filtro.CreditosMax.HasValue)
            {
                if (filtro.CreditosMax.Value < 1)
                    ModelState.AddModelError("CreditosMax", "Los créditos no pueden ser negativos.");
                else
                    query = query.Where(c => c.Creditos <= filtro.CreditosMax.Value);
            }

            if (!string.IsNullOrEmpty(filtro.HorarioDesde) &&
                TimeSpan.TryParse(filtro.HorarioDesde, out var desde))
            {
                query = query.Where(c => c.HorarioInicio >= desde);
            }

            if (!string.IsNullOrEmpty(filtro.HorarioHasta) &&
                TimeSpan.TryParse(filtro.HorarioHasta, out var hasta))
            {
                query = query.Where(c => c.HorarioFin <= hasta);
            }

            filtro.Cursos = await query.ToListAsync();
            return View(filtro);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _db.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }
    }
}