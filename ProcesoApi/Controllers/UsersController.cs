using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BDD;
using Microsoft.AspNetCore.Authorization; //[AllowAnonymous]
using ProcesoApi.Auth;

namespace ProcesoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BaseTempoContext _context;

        private IAuthService _userService;
        

        public UsersController(BaseTempoContext context)
        {
            _context = context;
        }

        // GET: api/Users
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }*/

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Autohorrize] //<-- Error Atrrrributrrro
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
