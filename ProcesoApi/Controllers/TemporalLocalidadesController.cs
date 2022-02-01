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
    public class TemporalLocalidadesController : ControllerBase
    {
        private readonly BaseTempoContext _context;

        public TemporalLocalidadesController(BaseTempoContext context)
        {
            _context = context;
        }

        // GET: api/TemporalLocalidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemporalLocalidades>>> GetTemporalLocalidades()
        {
            return await _context.TemporalLocalidades.ToListAsync();
        }

        // GET: api/TemporalLocalidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemporalLocalidades>> GetTemporalLocalidades(string id)
        {
            Console.WriteLine(id.Replace("%2F", "/"));
            var temporalLocalidades = await _context.TemporalLocalidades.FindAsync(id.Replace("%2F", "/").Replace("%20", " "));

            if (temporalLocalidades == null)
            {
                return NotFound();
            }

            return temporalLocalidades;
        }


        private bool TemporalLocalidadesExists(string id)
        {
            return _context.TemporalLocalidades.Any(e => e.Localidad == id);
        }
    }
}
