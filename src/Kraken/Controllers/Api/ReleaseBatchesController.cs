namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Kraken.Filters;
    using Kraken.Models;
    using Kraken.Services;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
    using Octopus.Client.Model;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
    [Route("api/releasebatches")]
    public class ReleaseBatchesController : Controller
    {
        public ReleaseBatchesController(ApplicationDbContext context, IOctopusProxy octopusProxy, IOctopusReleaseService octopusReleaseService)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (octopusProxy == null) throw new ArgumentNullException(nameof(octopusProxy));

            _context = context;
            _octopusProxy = octopusProxy;
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

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);

            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            return Ok(releaseBatch);
        }

        // GET: api/ReleaseBatches/5/Logo
        [HttpGet("{idOrName}/Logo")]
        public async Task<IActionResult> GetReleaseBatchLogo([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, true);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }
            
            if (releaseBatch.Logo != null)
            {
                return File(releaseBatch.Logo.Content, releaseBatch.Logo.ContentType);
            }

            return File("~/images/batch-logo.png", "images/png");
        }

        // PUT: api/ReleaseBatches/5
        [HttpPut("{idOrName}")]
        public async Task<IActionResult> PutReleaseBatch([FromRoute] string idOrName, [FromBody] ReleaseBatch releaseBatch)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var existingReleaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (existingReleaseBatch == null)
            {
                return HttpNotFound();
            }
            
            if (releaseBatch.Name != null)
            {
                existingReleaseBatch.Name = releaseBatch.Name;
            }
            if (releaseBatch.Description != null)
            {
                existingReleaseBatch.Description = releaseBatch.Description;
            }

            existingReleaseBatch.UpdateDateTime = DateTimeOffset.Now;
            existingReleaseBatch.UpdateUserName = User.Identity.Name;

            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/Logo
        [HttpPut("{idOrName}/Logo")]
        public async Task<IActionResult> PutReleaseBatchLogo([FromRoute] string idOrName, [FromBody] ReleaseBatchLogo releaseBatchLogo = null)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            var releaseBatch = await GetReleaseBatch(idOrName, false, true);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }
            
            if (releaseBatch.Logo == null)
            {
                if (releaseBatchLogo != null)
                {
                    releaseBatchLogo.ReleaseBatchId = releaseBatch.Id;
                    _context.ReleaseBatchLogos.Add(releaseBatchLogo);
                }
            }
            else
            {
                if (releaseBatchLogo != null)
                {
                    releaseBatch.Logo.Content = releaseBatchLogo.Content;
                    releaseBatch.Logo.ContentType = releaseBatchLogo.ContentType;
                }
                else
                {
                    _context.ReleaseBatchLogos.Remove(releaseBatch.Logo);
                }
            }

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

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

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
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
        public async Task<IActionResult> LinkProject([FromRoute] string idOrName, [FromBody] LinkProjectRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            var projectResource = _octopusProxy.GetProject(requestBody.ProjectIdOrSlugOrName);
            if (projectResource == null)
            {
                return HttpBadRequest("Project Not Found");
            }

            ReleaseResource releaseResource = null;
            if (requestBody.ReleaseVersion != null)
            {
                releaseResource = _octopusProxy.GetRelease(projectResource.Id, requestBody.ReleaseVersion);
                if (releaseResource == null)
                {
                    return HttpBadRequest("Release Not Found");
                }
            }

            var releaseBatchItem = await _context.ReleaseBatchItems.SingleOrDefaultAsync(e => e.ReleaseBatchId == releaseBatch.Id && e.ProjectId == projectResource.Id);
            if (releaseBatchItem == null)
            {
                releaseBatchItem = new ReleaseBatchItem
                {
                    ReleaseBatchId = releaseBatch.Id,
                    ProjectId = projectResource.Id
                };
                _context.ReleaseBatchItems.Add(releaseBatchItem);
            }

            releaseBatchItem.ProjectName = projectResource.Name;
            releaseBatchItem.ProjectSlug = projectResource.Slug;

            if (releaseResource != null)
            {
                releaseBatchItem.ReleaseId = releaseResource.Id;
                releaseBatchItem.ReleaseVersion = releaseResource.Version;
            }            

            releaseBatch.UpdateDateTime = DateTimeOffset.Now;
            releaseBatch.UpdateUserName = User.Identity.Name;

            await _context.SaveChangesAsync();

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/ReleaseBatches/5/UnlinkProject
        [HttpPut("{idOrName}/UnlinkProject")]
        public async Task<IActionResult> UnlinkProject([FromRoute] string idOrName, [FromBody] string projectIdOrSlugOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }
            
            var projectResource = _octopusProxy.GetProject(projectIdOrSlugOrName);
            if (projectResource == null)
            {
                return HttpBadRequest("Project Not Found");
            }
            
            var releaseBatchItem = await _context.ReleaseBatchItems.SingleOrDefaultAsync(e => e.ReleaseBatchId == releaseBatch.Id && e.ProjectId == projectResource.Id);
            if (releaseBatchItem != null)
            {
                _context.ReleaseBatchItems.Remove(releaseBatchItem);

                releaseBatch.UpdateDateTime = DateTimeOffset.Now;
                releaseBatch.UpdateUserName = User.Identity.Name;

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
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
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
                    var releaseResource = environment == null ? _octopusProxy.GetLatestRelease(releaseBatchItem.ProjectId) : _octopusProxy.GetLatestDeployedRelease(releaseBatchItem.ProjectId, environment.Id);
                    releaseBatchItem.ReleaseId = releaseResource?.Id;
                    releaseBatchItem.ReleaseVersion = releaseResource?.Version;

                    var projectResource = _octopusProxy.GetProject(releaseBatchItem.ProjectId);
                    releaseBatchItem.ProjectName = projectResource.Name;
                    releaseBatchItem.ProjectSlug = projectResource.Slug;
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
        public async Task<IActionResult> DeployReleaseBatch([FromRoute] string idOrName, [FromBody] string environmentIdOrName, [FromQuery] bool forceRedeploy = false)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            
            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
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
                    deployments.Add(_octopusProxy.DeployRelease(releaseBatchItem.ReleaseId, environment.Id, forceRedeploy));
                }
            }

            releaseBatch.DeployDateTime = DateTimeOffset.Now;
            releaseBatch.DeployEnvironmentId = environment.Id;
            releaseBatch.DeployEnvironmentName = environment.Name;
            releaseBatch.DeployUserName = User.Identity.Name;

            await _context.SaveChangesAsync();

            return Ok(deployments);
        }

        // GET: api/ReleaseBatches/5/PreviewReleases
        [HttpGet("{idOrName}/PreviewReleases")]
        public async Task<IActionResult> PreviewReleases([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
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

        // POST: api/ReleaseBatches/5/CreateReleases
        [HttpPost("{idOrName}/CreateReleases")]
        public async Task<IActionResult> CreateReleases([FromRoute] string idOrName, [FromBody] IEnumerable<ReleaseResource> releases)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return HttpNotFound();
            }

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    var release = _octopusProxy.CreateRelease(releases.First(r => r.ProjectId == releaseBatchItem.ProjectId));
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

        private async Task<ReleaseBatch> GetReleaseBatch(string idOrName, bool includeItems, bool includeLogo)
        {
            ReleaseBatch releaseBatch = null;

            IQueryable<ReleaseBatch> query = _context.ReleaseBatches;

            if (includeItems)
            {
                query = query.Include(b => b.Items);
            }

            if (includeLogo)
            {
                query = query.Include(b => b.Logo);
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
        private readonly IOctopusReleaseService _octopusReleaseService;

        public class LinkProjectRequestBody
        {
            public string ProjectIdOrSlugOrName { get; set; }
            public string ReleaseVersion { get; set; }
        }
    }
}