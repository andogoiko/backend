using System;
using BDD;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProcesoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalidadesController : ControllerBase
    {
        private readonly BaseTempoContext _context;

        public LocalidadesController(BaseTempoContext context)
        {
            _context = context;
        }

        // GET: api/Localidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Localidades>>> GetLocalidades()
        {
            return await _context.Localidades.ToListAsync();
        }

        // GET: api/Localidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Localidades>> GetLocalidades(string id)
        {
            var localidades = await _context.Localidades.FindAsync(id);

            if (localidades == null)
            {
                return NotFound();
            }

            return localidades;
        }



        private bool LocalidadesExists(string id)
        {
            return _context.Localidades.Any(e => e.Localidad == id);
        }
    }
}
