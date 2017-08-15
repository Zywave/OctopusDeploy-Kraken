namespace Kraken.Controllers.Api
{
    using System;
    using System.Threading.Tasks;
    using Kraken.Filters;
    using Kraken.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
    [Route("api/users")]
    public class UsersController : Controller
    {
        public UsersController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Users/me123
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetUser([FromRoute] string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return NotFound();
            }

            user.OctopusApiKey = null;

            return Ok(user);
        }

        private readonly ApplicationDbContext _context;
    }
}
