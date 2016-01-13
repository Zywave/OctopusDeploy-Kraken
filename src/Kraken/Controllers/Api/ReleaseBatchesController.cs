namespace Kraken.Controllers.Api
{
    using System;
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
    [Route("api/releasebatches")]
    public class ReleaseBatchesController : Controller
    {
        public ReleaseBatchesController(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        // GET: api/ReleaseBatches
        [HttpGet]
        public IEnumerable<ReleaseBatch> GetReleaseBatches()
        {
            return _context.ReleaseBatches;
        }

        // GET: api/ReleaseBatches/5
        [HttpGet("{id}", Name = "GetReleaseBatch")]
        public async Task<IActionResult> GetReleaseBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await _context.ReleaseBatches.Include(e => e.Items).SingleAsync(m => m.Id == id);

            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            return Ok(releaseBatch);
        }

        // PUT: api/ReleaseBatches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReleaseBatch([FromRoute] int id, [FromBody] ReleaseBatch releaseBatch)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != releaseBatch.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(releaseBatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReleaseBatchExists(id))
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

        // POST: api/ReleaseBatches
        [HttpPost]
        public async Task<IActionResult> PostReleaseBatch([FromBody] ReleaseBatch releaseBatch)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.ReleaseBatches.Add(releaseBatch);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReleaseBatchExists(releaseBatch.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetReleaseBatch", new { id = releaseBatch.Id }, releaseBatch);
        }

        // DELETE: api/ReleaseBatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReleaseBatch([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await _context.ReleaseBatches.SingleAsync(m => m.Id == id);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            _context.ReleaseBatches.Remove(releaseBatch);
            await _context.SaveChangesAsync();

            return Ok(releaseBatch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReleaseBatchExists(int id)
        {
            return _context.ReleaseBatches.Count(e => e.Id == id) > 0;
        }

        private readonly ApplicationDbContext _context;
    }
}