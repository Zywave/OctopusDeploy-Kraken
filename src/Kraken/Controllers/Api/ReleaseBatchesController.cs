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
        public ReleaseBatchesController(ApplicationDbContext context, IOctopusProxy octopusProxy, INuGetProxy nuGetProxy)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _context = context;
            _octopusProxy = octopusProxy;
            _nuGetProxy = nuGetProxy;
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

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            _context.Entry(releaseBatch).State = EntityState.Modified;
            _context.Entry(releaseBatch).Property(b => b.SyncDateTime).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.SyncEnvironmentId).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.SyncEnvironmentName).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.SyncUserName).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.DeployDateTime).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.DeployEnvironmentId).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.DeployEnvironmentName).IsModified = false;
            _context.Entry(releaseBatch).Property(b => b.DeployUserName).IsModified = false;

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

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

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

        // PUT: api/ReleaseBatches/5/LinkProject
        [HttpPut("{id}/LinkProject")]
        public async Task<IActionResult> LinkProjectToReleaseBatch([FromRoute] int id, [FromBody] string projectIdOrSlugOrName)
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

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            var projectResource = _octopusProxy.GetProject(projectIdOrSlugOrName);
            if (projectResource == null)
            {
                return HttpBadRequest("Project Not Found");
            }

            var releaseBatchItem = new ReleaseBatchItem
            {
                ReleaseBatchId = id,
                ProjectId = projectResource.Id,
                ProjectName = projectResource.Name
            };
            
            _context.ReleaseBatchItems.Add(releaseBatchItem);

            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/UnlinkProject
        [HttpPut("{id}/UnlinkProject")]
        public async Task<IActionResult> UnlinkProjectFromReleaseBatch([FromRoute] int id, [FromBody] string projectIdOrSlugOrName)
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

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            var projectResource = _octopusProxy.GetProject(projectIdOrSlugOrName);
            if (projectResource == null)
            {
                return HttpBadRequest("Project Not Found");
            }

            var releaseBatchItem = await _context.ReleaseBatchItems.SingleOrDefaultAsync(e => e.ReleaseBatchId == id && e.ProjectId == projectResource.Id);
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
        [HttpPut("{id}/Sync")]
        public async Task<IActionResult> SyncReleaseBatch([FromRoute] int id, [FromBody] string environmentIdOrName = null)
        {
            if (!ReleaseBatchExists(id))
            {
                return new HttpStatusCodeResult(StatusCodes.Status404NotFound);
            }

            var releaseBatch = await _context.ReleaseBatches.Include(e => e.Items).SingleAsync(m => m.Id == id);
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

        // POST api/ReleaseBatches/5/Deploy
        [HttpPost("{id}/Deploy")]
        public async Task<IActionResult> DeployReleaseBatch([FromRoute] int id, [FromBody] string environmentIdOrName, [FromBody] bool allowRedeploy = false)
        {
            var releaseBatch = await _context.ReleaseBatches.Include(e => e.Items).SingleOrDefaultAsync(m => m.Id == id);
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

        // POST api/ReleaseBatches/5/ReleaseFromNuget
        [HttpPost("{id}/CreateReleases")]
        public async Task<IActionResult> CreateReleases([FromRoute] int id)
        {
            var releaseBatch = await _context.ReleaseBatches.Include(e => e.Items).SingleAsync(m => m.Id == id);

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    var nugetSteps = _octopusProxy.GetNuGetDeploymentStepResources(releaseBatchItem.ProjectId).ToList();
                    var nugetPackageIds = _octopusProxy.GetNugetPackageIdsFromSteps(nugetSteps);
                    var nugetPackageInfo = nugetPackageIds.ToDictionary(i => i, i => _nuGetProxy.GetLatestVersionForPackage(i));

                    var release = _octopusProxy.CreateReleases(releaseBatchItem.ProjectId, nugetSteps, nugetPackageInfo);
                    if (release != null)
                    {
                        releaseBatchItem.ReleaseId = release.Id;
                        releaseBatchItem.ReleaseVersion = release.Version;
                    }
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
        private readonly INuGetProxy _nuGetProxy ;
    }
}