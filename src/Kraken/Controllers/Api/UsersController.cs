namespace Kraken.Controllers.Api
{
    using System;
    using System.Threading.Tasks;
    using Kraken.Filters;
    using Kraken.Models;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
    [Route("api/users")]
    public class UsersController : Controller
    {
        public UsersController(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        // GET: api/Users/me123
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetUser([FromRoute] string userName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.OctopusApiKey = null;

            return Ok(user);
        }

        private readonly ApplicationDbContext _context;
    }
}
