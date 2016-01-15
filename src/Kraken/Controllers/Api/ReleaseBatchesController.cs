namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.AspNet.Authorization;
    using Kraken.Models;
    using Kraken.Services;
    using Octopus.Client.Model;

    [Authorize]
    [Produces("application/json")]
    [Route("api/releasebatches")]
    public class ReleaseBatchesController : Controller
    {
        public ReleaseBatchesController(ApplicationDbContext context, IOctopusProxy octopusProxy)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _context = context;
            _octopusProxy = octopusProxy;
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
            catch (DbUpdateException e)
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

        // PUT: api/ReleaseBatches/5/LinkProject
        [HttpPut("{id}/LinkProject")]
        public async Task<IActionResult> LinkProjectToReleaseBatch([FromRoute] int id, [FromBody] string projectId)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            var projectResource = _octopusProxy.GetProject(projectId);

            var releaseBatchItem = new ReleaseBatchItem
            {
                ReleaseBatchId = id,
                ProjectId = projectResource.Id,
                ProjectName = projectResource.Name
            };
            
            _context.ReleaseBatchItems.Add(releaseBatchItem);
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
                //TODO: check for existence and throw bad request
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/UnlinkProject
        [HttpPut("{id}/UnlinkProject")]
        public async Task<IActionResult> UnlinkProjectFromReleaseBatch([FromRoute] int id, [FromBody] string projectId)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            var releaseBatchItem = await _context.ReleaseBatchItems.SingleOrDefaultAsync(e => e.ReleaseBatchId == id && e.ProjectId == projectId);
            if (releaseBatchItem != null)
            {
                _context.ReleaseBatchItems.Remove(releaseBatchItem);

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
            }
            else if (!ReleaseBatchExists(id))
            {
                return HttpNotFound();
            }
            else
            {
                return HttpBadRequest();
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/Sync
        [HttpPut("{id}/Sync")]
        public async Task<IActionResult> SyncReleaseBatch([FromRoute] int id, [FromBody] string environmentId = null)
        {
            if (!ReleaseBatchExists(id))
            {
                return new HttpStatusCodeResult(StatusCodes.Status404NotFound);
            }

            var releaseBatch = await _context.ReleaseBatches.Include(e => e.Items).SingleAsync(m => m.Id == id);
            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    ReleaseResource releaseResource;
                    if (String.IsNullOrEmpty(environmentId))
                    {
                        releaseResource = _octopusProxy.GetLastestRelease(releaseBatchItem.ProjectId);
                    }
                    else
                    {
                        releaseResource =
                            _octopusProxy.GetLastDeployedRelease(releaseBatchItem.ProjectId,
                                environmentId);
                    }
                    releaseBatchItem.ReleaseId = releaseResource?.Id;
                    releaseBatchItem.ReleaseVersion = releaseResource?.Version;
                }
                _context.Entry(releaseBatch).State = EntityState.Modified;
            }

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
        private readonly IOctopusProxy _octopusProxy;
    }
}