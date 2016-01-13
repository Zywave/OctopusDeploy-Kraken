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

    [Produces("application/json")]
    [Route("api/ReleaseBatchItems")]
    public class ReleaseBatchItemsController : Controller
    {
        public ReleaseBatchItemsController(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        // GET: api/ReleaseBatchItems
        [HttpGet]
        public IEnumerable<ReleaseBatchItem> GetReleaseBatchItems()
        {
            return _context.ReleaseBatchItems;
        }

        // GET: api/ReleaseBatchItems/5
        [HttpGet("{id}", Name = "GetReleaseBatchItem")]
        public async Task<IActionResult> GetReleaseBatchItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatchItem = await _context.ReleaseBatchItems.SingleAsync(m => m.Id == id);

            if (releaseBatchItem == null)
            {
                return HttpNotFound();
            }

            return Ok(releaseBatchItem);
        }

        // PUT: api/ReleaseBatchItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReleaseBatchItem([FromRoute] int id, [FromBody] ReleaseBatchItem releaseBatchItem)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != releaseBatchItem.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(releaseBatchItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReleaseBatchItemExists(id))
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

        // POST: api/ReleaseBatchItems
        [HttpPost]
        public async Task<IActionResult> PostReleaseBatchItem([FromBody] ReleaseBatchItem releaseBatchItem)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.ReleaseBatchItems.Add(releaseBatchItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReleaseBatchItemExists(releaseBatchItem.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetReleaseBatchItem", new { id = releaseBatchItem.Id }, releaseBatchItem);
        }

        // DELETE: api/ReleaseBatchItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReleaseBatchItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatchItem = await _context.ReleaseBatchItems.SingleAsync(m => m.Id == id);
            if (releaseBatchItem == null)
            {
                return HttpNotFound();
            }

            _context.ReleaseBatchItems.Remove(releaseBatchItem);
            await _context.SaveChangesAsync();

            return Ok(releaseBatchItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReleaseBatchItemExists(int id)
        {
            return _context.ReleaseBatchItems.Count(e => e.Id == id) > 0;
        }

        private readonly ApplicationDbContext _context;
    }
}