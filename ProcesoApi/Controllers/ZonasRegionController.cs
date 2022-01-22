using System;
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
    public class ZonasRegionController : ControllerBase
    {
        private readonly BaseTempoContext _context;

        public ZonasRegionController(BaseTempoContext context)
        {
            _context = context;
        }

        // GET: api/ZonasRegion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZonasRegion>>> GetZonasRegion()
        {
            return await _context.ZonasRegion.ToListAsync();
        }

        // GET: api/ZonasRegion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ZonasRegion>> GetZonasRegion(string id)
        {
            var zonasRegion = await _context.ZonasRegion.FindAsync(id);

            if (zonasRegion == null)
            {
                return NotFound();
            }

            return zonasRegion;
        }

        private bool ZonasRegionExists(string id)
        {
            return _context.ZonasRegion.Any(e => e.Zona == id);
        }
    }
}
