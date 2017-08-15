namespace Kraken.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Kraken.Filters;
    using Kraken.Models;
    using Kraken.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Octopus.Client.Exceptions;
    using Octopus.Client.Model;

    [Authorize]
    [Produces("application/json")]
    [ServiceFilter(typeof(ResponseTextExceptionFilter))]
    [Route("api/releasebatches")]
    public class ReleaseBatchesController : Controller
    {
        public ReleaseBatchesController(ApplicationDbContext context, IOctopusProxy octopusProxy, IOctopusReleaseService octopusReleaseService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _octopusProxy = octopusProxy ?? throw new ArgumentNullException(nameof(octopusProxy));
            _octopusReleaseService = octopusReleaseService;
        }

        // GET: api/ReleaseBatches
        [HttpGet]
        public IEnumerable<ReleaseBatch> GetReleaseBatches()
        {
            return _context.ReleaseBatches.OrderBy(rb => rb.Name);
        }

        // GET: api/ReleaseBatches/5
        [HttpGet("{idOrName}", Name = "GetReleaseBatch")]
        public async Task<IActionResult> GetReleaseBatch([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);

            if (releaseBatch == null)
            {
                return NotFound();
            }

            return Ok(releaseBatch);
        }

        // GET: api/ReleaseBatches/5/Logo
        [HttpGet("{idOrName}/Logo")]
        public async Task<IActionResult> GetReleaseBatchLogo([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, true);
            if (releaseBatch == null)
            {
                return NotFound();
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
                return BadRequest(ModelState);
            }

            var existingReleaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (existingReleaseBatch == null)
            {
                return NotFound();
            }

            if (existingReleaseBatch.IsLocked)
            {
                return GetLockedForbiddenUpdateResult(existingReleaseBatch);
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

            return new NoContentResult();
        }

        [HttpPut("{idOrName}/LockReleaseBatch")]
        public async Task<IActionResult> LockReleaseBatch([FromRoute] string idOrName, [FromBody] string comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            releaseBatch.IsLocked = true;
            releaseBatch.LockComment = comment;
            releaseBatch.LockUserName = User.Identity.Name;

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        [HttpPut("{idOrName}/UnlockReleaseBatch")]
        public async Task<IActionResult> UnlockReleaseBatch([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            releaseBatch.IsLocked = false;
            releaseBatch.LockComment = String.Empty;
            releaseBatch.LockUserName = String.Empty;

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        // PUT: api/ReleaseBatches/5/Logo
        [HttpPut("{idOrName}/Logo")]
        public async Task<IActionResult> PutReleaseBatchLogo([FromRoute] string idOrName, [FromBody] ReleaseBatchLogo releaseBatchLogo = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, true);
            if (releaseBatch == null)
            {
                return NotFound();
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

            return new NoContentResult();
        }

        // POST: api/ReleaseBatches
        [HttpPost]
        public async Task<IActionResult> PostReleaseBatch([FromBody] ReleaseBatch releaseBatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
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
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            if (releaseBatch.IsLocked)
            {
                return GetLockedForbiddenUpdateResult(releaseBatch);
            }

            _context.ReleaseBatches.Remove(releaseBatch);
            await _context.SaveChangesAsync();

            return Ok(releaseBatch);
        }

        // DELETE: api/ReleaseBatches/5/Copy
        [HttpPost("{idOrName}/Copy")]
        public async Task<IActionResult> CopyReleaseBatch([FromRoute] string idOrName, [FromBody] string copyName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, true);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            var copyReleaseBatch = new ReleaseBatch();
            copyReleaseBatch.Name = copyName;

            copyReleaseBatch.Description = releaseBatch.Description;

            if (releaseBatch.Logo != null)
            {
                copyReleaseBatch.Logo = new ReleaseBatchLogo();
                copyReleaseBatch.Logo.Content = releaseBatch.Logo.Content;
                copyReleaseBatch.Logo.ContentType = releaseBatch.Logo.ContentType;
            }

            if (releaseBatch.Items != null)
            {
                copyReleaseBatch.Items = new List<ReleaseBatchItem>();
                foreach (var item in releaseBatch.Items)
                {
                    var copyItem = new ReleaseBatchItem();
                    copyItem.ProjectId = item.ProjectId;
                    copyItem.ProjectName = item.ProjectName;
                    copyItem.ProjectSlug = item.ProjectSlug;
                    copyItem.ReleaseId = item.ReleaseId;
                    copyItem.ReleaseVersion = item.ReleaseVersion;
                    copyReleaseBatch.Items.Add(copyItem);
                }
            }

            copyReleaseBatch.UpdateDateTime = DateTimeOffset.Now;
            copyReleaseBatch.UpdateUserName = User.Identity.Name;

            _context.ReleaseBatches.Add(copyReleaseBatch);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetReleaseBatch", new { idOrName = copyReleaseBatch.Id }, copyReleaseBatch);
        }

        // PUT: api/ReleaseBatches/5/LinkProject
        [HttpPut("{idOrName}/LinkProject")]
        public async Task<IActionResult> LinkProject([FromRoute] string idOrName, [FromBody] LinkProjectRequestBody requestBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            if (releaseBatch.IsLocked)
            {
                return GetLockedForbiddenUpdateResult(releaseBatch);
            }

            var projectResource = await _octopusProxy.GetProjectAsync(requestBody.ProjectIdOrSlugOrName);
            if (projectResource == null)
            {
                return BadRequest("Project Not Found");
            }

            ReleaseResource releaseResource = null;
            if (requestBody.ReleaseVersion != null)
            {
                releaseResource = await _octopusProxy.GetReleaseAsync(projectResource.Id, requestBody.ReleaseVersion);
                if (releaseResource == null)
                {
                    return BadRequest("Release Not Found");
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

            return new NoContentResult();
        }

        // PUT: api/ReleaseBatches/5/UnlinkProject
        [HttpPut("{idOrName}/UnlinkProject")]
        public async Task<IActionResult> UnlinkProject([FromRoute] string idOrName, [FromBody] string projectIdOrSlugOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, false, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            if (releaseBatch.IsLocked)
            {
                return GetLockedForbiddenUpdateResult(releaseBatch);
            }

            var projectResource = await _octopusProxy.GetProjectAsync(projectIdOrSlugOrName);
            if (projectResource == null)
            {
                return BadRequest("Project Not Found");
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
                return BadRequest();
            }

            return new NoContentResult();
        }

        // PUT: api/ReleaseBatches/5/Sync
        [HttpPut("{idOrName}/Sync")]
        public async Task<IActionResult> SyncReleaseBatch([FromRoute] string idOrName, [FromBody] string environmentIdOrName = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            if (releaseBatch.IsLocked)
            {
                return GetLockedForbiddenUpdateResult(releaseBatch);
            }

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                EnvironmentResource environment = null;
                if (!String.IsNullOrEmpty(environmentIdOrName))
                {
                    environment = await _octopusProxy.GetEnvironmentAsync(environmentIdOrName);
                    if (environment == null)
                    {
                        return BadRequest("Environment Not Found");
                    }
                }

                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    var releaseResource =
                        await
                        (environment == null
                            ? _octopusProxy.GetLatestReleaseAsync(releaseBatchItem.ProjectId)
                            : _octopusProxy.GetLatestDeployedReleaseAsync(releaseBatchItem.ProjectId, environment.Id));
                    releaseBatchItem.ReleaseId = releaseResource?.Id;
                    releaseBatchItem.ReleaseVersion = releaseResource?.Version;

                    var projectResource = await _octopusProxy.GetProjectAsync(releaseBatchItem.ProjectId);
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
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            var environment = await _octopusProxy.GetEnvironmentAsync(environmentIdOrName);
            if (environment == null)
            {
                return BadRequest("Environment Not Found");
            }

            var responseBody = new DeployBatchResponseBody();

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items.Where(releaseBatchItem => !string.IsNullOrEmpty(releaseBatchItem.ReleaseId)))
                {
                    try
                    {
                        await _octopusProxy.DeployReleaseAsync(releaseBatchItem.ReleaseId, environment.Id, forceRedeploy);
                        responseBody.SuccessfulItems.Add(new DeployBatchItem
                        {
                            Name = releaseBatchItem.ProjectName
                        });
                    }
                    catch (OctopusException ex)
                    {
                        responseBody.FailedItems.Add(new DeployBatchItem
                        {
                            Name = releaseBatchItem.ProjectName,
                            Message = ex.Message
                        });
                    }
                }
            }

            releaseBatch.DeployDateTime = DateTimeOffset.Now;
            releaseBatch.DeployEnvironmentId = environment.Id;
            releaseBatch.DeployEnvironmentName = environment.Name;
            releaseBatch.DeployUserName = User.Identity.Name;

            await _context.SaveChangesAsync();
            
            return Ok(responseBody);
        }

        // GET: api/ReleaseBatches/5/PreviewReleases
        [HttpGet("{idOrName}/PreviewReleases")]
        public async Task<IActionResult> PreviewReleases([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            var releases = new List<ReleaseResource>();

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    releases.Add(await _octopusReleaseService.GetNextReleaseAsync(releaseBatchItem.ProjectId));
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
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                foreach (var releaseBatchItem in releaseBatch.Items)
                {
                    var release = await _octopusProxy.CreateReleaseAsync(releases.First(r => r.ProjectId == releaseBatchItem.ProjectId));
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

        [HttpGet("{idOrName}/GetProgression")]
        public async Task<IActionResult> GetProgression([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            var environmentIds = (await _octopusProxy.GetEnvironmentsAsync()).Select(e => e.Id);
            var progress = new List<ProjectProgressResponseBody>();

            if (releaseBatch.Items != null && releaseBatch.Items.Any())
            {
                var dashboard = await _octopusProxy.GetDynamicDashboardAsync(releaseBatch.Items.Select(i => i.ProjectId), environmentIds);
                progress = dashboard.Items.Select(d => new ProjectProgressResponseBody
                {
                    ProjectId = d.ProjectId,
                    EnvironmentId = d.EnvironmentId,
                    DeploymentId = d.DeploymentId,
                    State = d.State,
                    ReleaseId = d.ReleaseId,
                    ReleaseVersion = d.ReleaseVersion,
                    CompletedTime = d.CompletedTime
                }).ToList();
            }

            return Ok(progress);
        }

        [HttpGet("{idOrName}/GetBatchEnvironments")]
        public async Task<IActionResult> GetBatchEnvironments([FromRoute] string idOrName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var releaseBatch = await GetReleaseBatch(idOrName, true, false);
            if (releaseBatch == null)
            {
                return NotFound();
            }

            var permissionsToGet = new[] { Permission.EnvironmentView, Permission.DeploymentCreate };

            var environmentsWithPermissions =
                await
                    _octopusProxy.GetEnvironmentsWithPermissionsAsync(permissionsToGet,
                        releaseBatch.Items.Select(i => i.ProjectId));

            var retVal = new EnvironmentsWithPermissionsResponseBody
            {
                View = environmentsWithPermissions[Permission.EnvironmentView].Select(e => new EnvironmentMapping
                {
                    Id = e.Id,
                    Name = e.Name
                }).ToList(),
                Deploy = environmentsWithPermissions[Permission.DeploymentCreate].Select(e => new EnvironmentMapping
                {
                    Id = e.Id,
                    Name = e.Name
                }).ToList(),
            };

            return Ok(retVal);
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

            return releaseBatch ?? await query.SingleOrDefaultAsync(b => b.Name == idOrName);
        }

        private ObjectResult GetLockedForbiddenUpdateResult(ReleaseBatch releaseBatch)
        {
            var lockedReason = $"Locked by: {releaseBatch.LockUserName}{(!string.IsNullOrEmpty(releaseBatch.LockComment) ? $" ({releaseBatch.LockComment})" : String.Empty)}";
            return new ObjectResult(lockedReason)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        private readonly ApplicationDbContext _context;
        private readonly IOctopusProxy _octopusProxy;
        private readonly IOctopusReleaseService _octopusReleaseService;

        public class LinkProjectRequestBody
        {
            public string ProjectIdOrSlugOrName { get; set; }
            public string ReleaseVersion { get; set; }
        }

        public class DeployBatchResponseBody
        {
            public List<DeployBatchItem> SuccessfulItems { get; set; } = new List<DeployBatchItem>();
            public List<DeployBatchItem> FailedItems { get; set; } = new List<DeployBatchItem>();
        }

        public class DeployBatchItem
        {
            public string Name { get; set; }
            
            public string Message { get; set; }
        }

        public class ProjectProgressResponseBody
        {
            public string ProjectId { get; set; }
            public string DeploymentId { get; set; }
            public TaskState State { get; set; }
            public string EnvironmentId { get; set; }
            public string ReleaseId { get; set; }
            public string ReleaseVersion { get; set; }
            public DateTimeOffset? CompletedTime { get; set; }
        }

        public class EnvironmentsWithPermissionsResponseBody
        {
            public List<EnvironmentMapping> View { get; set; } = new List<EnvironmentMapping>();
            public List<EnvironmentMapping> Deploy { get; set; } = new List<EnvironmentMapping>();
        }
        
        public class EnvironmentMapping
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}