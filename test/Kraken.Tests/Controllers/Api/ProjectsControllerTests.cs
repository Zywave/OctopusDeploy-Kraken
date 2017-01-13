using Kraken.Controllers.Api;
using Kraken.Services;
using Moq;
using Octopus.Client.Model;
using Xunit;

namespace Kraken.Tests.Controllers.Api
{
    using System.Threading.Tasks;

    public class ProjectsControllerTests
    {
        [Fact]
        public async Task TestGetProjectsWithNoFilterReturnsOctopusData()
        {
            var octopusApi = new Mock<IOctopusProxy>(MockBehavior.Strict);
            octopusApi.Setup(o => o.GetProjectsAsync("")).ReturnsAsync(new[] { new ProjectResource() });

            var controller = new ProjectsController(octopusApi.Object);

            var result = await controller.GetProjects();

            Assert.NotNull(result);

            octopusApi.VerifyAll();
        }

        [Fact]
        public async Task TestGetProjectsWithFilterReturnsOctopusData()
        {
            var octopusApi = new Mock<IOctopusProxy>(MockBehavior.Strict);
            octopusApi.Setup(o => o.GetProjectsAsync("project-1")).ReturnsAsync(new[] { new ProjectResource() });

            var controller = new ProjectsController(octopusApi.Object);

            var result = await controller.GetProjects("project-1");

            Assert.NotNull(result);

            octopusApi.VerifyAll();
        }
    }
}
