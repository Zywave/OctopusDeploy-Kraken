namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kraken.Filters;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Microsoft.AspNet.Authorization;
    using Kraken.Models;
    using Kraken.Services;
    using Octopus.Client.Model;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
    [Route("api/releasebatches")]
    public class ReleaseBatchesController : Controller
    {
        public ReleaseBatchesController(ApplicationDbContext context, IOctopusProxy octopusProxy, INuGetProxy nuGetProxy, IOctopusReleaseService octopusReleaseService)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _context = context;
            _octopusProxy = octopusProxy;
            _nuGetProxy = nuGetProxy;
            _octopusReleaseService = octopusReleaseService;
        }

        // GET: api/ReleaseBatches
        [HttpGet]
        public IEnumerable<ReleaseBatch> GetReleaseBatches()
        {
            return _context.ReleaseBatches;
        }

        // GET: api/ReleaseBatches/5
        [HttpGet("{idOrName}", Name = "GetReleaseBatch")]
        public async Task<IActionResult> GetReleaseBatch([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true);

            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            return Ok(releaseBatch);
        }

        // PUT: api/ReleaseBatches/5
        [HttpPut("{idOrName}")]
        public async Task<IActionResult> PutReleaseBatch([FromRoute] string idOrName, [FromBody] ReleaseBatch releaseBatch)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var existingReleaseBatch = await GetReleaseBatch(idOrName, false);
            if (existingReleaseBatch == null)
            {
                return HttpNotFound();
            }

            existingReleaseBatch.Name = releaseBatch.Name;

            existingReleaseBatch.UpdateDateTime = DateTimeOffset.Now;
            existingReleaseBatch.UpdateUserName = User.Identity.Name;

            await _context.SaveChangesAsync();

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

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            _context.ReleaseBatches.Add(releaseBatch);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_context.ReleaseBatches.Count(e => e.Id == releaseBatch.Id) > 0)
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetReleaseBatch", new { idOrName = releaseBatch.Id }, releaseBatch);
        }

        // DELETE: api/ReleaseBatches/5
        [HttpDelete("{idOrName}")]
        public async Task<IActionResult> DeleteReleaseBatch([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            _context.ReleaseBatches.Remove(releaseBatch);
            await _context.SaveChangesAsync();

            return Ok(releaseBatch);
        }

        // PUT: api/ReleaseBatches/5/LinkProject
        [HttpPut("{idOrName}/LinkProject")]
        public async Task<IActionResult> LinkProjectToReleaseBatch([FromRoute] string idOrName, [FromBody] string projectIdOrSlugOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            var projectResource = _octopusProxy.GetProject(projectIdOrSlugOrName);
            if (projectResource == null)
            {
                return HttpBadRequest("Project Not Found");
            }

            var releaseBatchItem = new ReleaseBatchItem
            {
                ReleaseBatchId = releaseBatch.Id,
                ProjectId = projectResource.Id,
                ProjectName = projectResource.Name
            };
            
            _context.ReleaseBatchItems.Add(releaseBatchItem);

            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/UnlinkProject
        [HttpPut("{idOrName}/UnlinkProject")]
        public async Task<IActionResult> UnlinkProjectFromReleaseBatch([FromRoute] string idOrName, [FromBody] string projectIdOrSlugOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            var projectResource = _octopusProxy.GetProject(projectIdOrSlugOrName);
            if (projectResource == null)
            {
                return HttpBadRequest("Project Not Found");
            }

            var releaseBatchItem = await _context.ReleaseBatchItems.SingleOrDefaultAsync(e => e.ReleaseBatchId == releaseBatch.Id && e.ProjectId == projectResource.Id);
            if (releaseBatchItem != null)
            {
                _context.ReleaseBatchItems.Remove(releaseBatchItem);

                await _context.SaveChangesAsync();
            }
            else
            {
                return HttpBadRequest();
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/Sync
        [HttpPut("{idOrName}/Sync")]
        public async Task<IActionResult> SyncReleaseBatch([FromRoute] string idOrName, [FromBody] string environmentIdOrName = null)
        {
            var releaseBatch = await GetReleaseBatch(idOrName, true);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                EnvironmentResource environment = null;
                if (!String.IsNullOrEmpty(environmentIdOrName))
                {
                    environment = _octopusProxy.GetEnvironment(environmentIdOrName);
                    if (environment == null)
                    {
                        return HttpBadRequest("Environment Not Found");
                    }
                }

                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    ReleaseResource releaseResource;
                    if (environment == null)
                    {
                        releaseResource = _octopusProxy.GetLastestRelease(releaseBatchItem.ProjectId);
                    }
                    else
                    {      
                        releaseResource = _octopusProxy.GetLastDeployedRelease(releaseBatchItem.ProjectId, environment.Id);
                    }
                    releaseBatchItem.ReleaseId = releaseResource?.Id;
                    releaseBatchItem.ReleaseVersion = releaseResource?.Version;
                }

                releaseBatch.SyncDateTime = DateTimeOffset.Now;
                releaseBatch.SyncEnvironmentId = environment?.Id;
                releaseBatch.SyncEnvironmentName = environment?.Name ?? "(Latest)";
                releaseBatch.SyncUserName = User.Identity.Name;
            }

            await _context.SaveChangesAsync();

            return Ok(releaseBatch);
        }

        // POST: api/ReleaseBatches/5/Deploy
        [HttpPost("{idOrName}/Deploy")]
        public async Task<IActionResult> DeployReleaseBatch([FromRoute] string idOrName, [FromBody] string environmentIdOrName, [FromBody] bool allowRedeploy = false)
        {
            var releaseBatch = await GetReleaseBatch(idOrName, true);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            var environment = _octopusProxy.GetEnvironment(environmentIdOrName);
            if (environment == null)
            {
                return HttpBadRequest("Environment Not Found");
            }

            var deployments = new List<DeploymentResource>();

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items.Where(releaseBatchItem => !string.IsNullOrEmpty(releaseBatchItem.ReleaseId)))
                {
                    deployments.Add(_octopusProxy.DeployRelease(releaseBatchItem.ReleaseId, environment.Id, allowRedeploy));
                }
            }

            releaseBatch.DeployDateTime = DateTimeOffset.Now;
            releaseBatch.DeployEnvironmentId = environment.Id;
            releaseBatch.DeployEnvironmentName = environment.Name;
            releaseBatch.DeployUserName = User.Identity.Name;

            await _context.SaveChangesAsync();

            return Ok(deployments);
        }

        // GET: api/ReleaseBatches/5/GetNextReleases
        [HttpGet("{idOrName}/GetNextReleases")]
        public async Task<IActionResult> GetNextReleases([FromRoute] string idOrName)
        {
            var releaseBatch = await GetReleaseBatch(idOrName, true);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            var releases = new List<ReleaseResource>();

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    releases.Add(_octopusReleaseService.GetNextRelease(releaseBatchItem.ProjectId));
                }
            }

            return Ok(releases);
        }

        // POST: api/ReleaseBatches/CreateReleases
        [HttpPost("{idOrName}/CreateReleases")]
        public async Task<IActionResult> CreateReleases([FromRoute] string idOrName, [FromBody] IEnumerable<ReleaseResource> releases)
        {
            var releaseBatch = await GetReleaseBatch(idOrName, true);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    var release = _octopusProxy.CreateRelease(releases.First(r => r.ProjectId == releaseBatchItem.ProjectId));
                    release = _octopusProxy.CreateRelease(release);
                    if (release != null)
                    {
                        releaseBatchItem.ReleaseId = release.Id;
                        releaseBatchItem.ReleaseVersion = release.Version;
                    }
                }
            }
            
            releaseBatch.SyncDateTime = DateTimeOffset.Now;
            releaseBatch.SyncEnvironmentId = null;
            releaseBatch.SyncEnvironmentName = "(Latest)";
            releaseBatch.SyncUserName = User.Identity.Name;
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

        private async Task<ReleaseBatch> GetReleaseBatch(string idOrName, bool includeItems)
        {
            ReleaseBatch releaseBatch = null;

            IQueryable<ReleaseBatch> query = _context.ReleaseBatches;

            if (includeItems)
            {
                query = query.Include(b => b.Items);
            }

            int id;
            if (int.TryParse(idOrName, out id))
            {
                releaseBatch = await query.SingleOrDefaultAsync(b => b.Id == id);
            }

            if (releaseBatch == null)
            {
                releaseBatch = await query.SingleOrDefaultAsync(b => b.Name == idOrName);
            }

            return releaseBatch;
        }

        private readonly ApplicationDbContext _context;
        private readonly IOctopusProxy _octopusProxy;
        private readonly INuGetProxy _nuGetProxy ;
        private readonly IOctopusReleaseService _octopusReleaseService;
    }
}