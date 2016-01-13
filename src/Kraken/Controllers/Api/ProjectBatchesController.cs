namespace Kraken.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Kraken.Models;
    using Microsoft.AspNet.Authorization;

    [Authorize]
    [Produces("application/json")]
    [Route("api/projectbatches")]
    public class ProjectBatchesController : Controller
    {
        private ApplicationDbContext _context;

        public ProjectBatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectBatches
        [HttpGet]
        public IEnumerable<ProjectBatch> GetProjectBatches()
        {
            return _context.ProjectBatches;
        }

        // GET: api/ProjectBatches/5
        [HttpGet("{id}", Name = "GetProjectBatch")]
        public async Task<IActionResult> GetProjectBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var projectBatch = await _context.ProjectBatches.Include(e => e.Items).SingleAsync(m => m.Id == id);

            if (projectBatch == null)
            {
                return HttpNotFound();
            }

            return Ok(projectBatch);
        }

        // PUT: api/ProjectBatches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectBatch([FromRoute] int id, [FromBody] ProjectBatch projectBatch)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != projectBatch.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(projectBatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectBatchExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/ProjectBatches
        [HttpPost]
        public async Task<IActionResult> PostProjectBatch([FromBody] ProjectBatch projectBatch)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.ProjectBatches.Add(projectBatch);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProjectBatchExists(projectBatch.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetProjectBatch", new { id = projectBatch.Id }, projectBatch);
        }

        // DELETE: api/ProjectBatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var projectBatch = await _context.ProjectBatches.SingleAsync(m => m.Id == id);
            if (projectBatch == null)
            {
                return HttpNotFound();
            }

            _context.ProjectBatches.Remove(projectBatch);
            await _context.SaveChangesAsync();

            return Ok(projectBatch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectBatchExists(int id)
        {
            return _context.ProjectBatches.Count(e => e.Id == id) > 0;
        }
    }
}