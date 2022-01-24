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
    public class ProvinciasController : ControllerBase
    {
        private readonly BaseTempoContext _context;

        public ProvinciasController(BaseTempoContext context)
        {
            _context = context;
        }

        // GET: api/Provincias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Provincias>>> GetProvincias()
        {
            return await _context.Provincias.ToListAsync();
        }

        // GET: api/Provincias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Provincias>> GetProvincias(string id)
        {
            var provincias = await _context.Provincias.FindAsync(id);

            if (provincias == null)
            {
                return NotFound();
            }

            return provincias;
        }


        private bool ProvinciasExists(string id)
        {
            return _context.Provincias.Any(e => e.Provincia == id);
        }
    }
}
