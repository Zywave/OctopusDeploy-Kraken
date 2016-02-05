using Kraken.Controllers.Api;
using Kraken.Services;
using Moq;
using Octopus.Client.Model;
using Xunit;

namespace Kraken.Tests.Controllers.Api
{
    public class ProjectsControllerTests
    {
        [Fact]
        public void TestGetProjectsWithNoFilterReturnsOctopusData()
        {
            var octopusApi = new Mock<IOctopusProxy>(MockBehavior.Strict);
            octopusApi.Setup(o => o.GetProjects("")).Returns(new[] { new ProjectResource() });

            var controller = new ProjectsController(octopusApi.Object);

            var result = controller.GetProjects();

            Assert.NotNull(result);

            octopusApi.VerifyAll();
        }

        [Fact]
        public void TestGetProjectsWithFilterReturnsOctopusData()
        {
            var octopusApi = new Mock<IOctopusProxy>(MockBehavior.Strict);
            octopusApi.Setup(o => o.GetProjects("project-1")).Returns(new[] { new ProjectResource() });

            var controller = new ProjectsController(octopusApi.Object);

            var result = controller.GetProjects("project-1");

            Assert.NotNull(result);

            octopusApi.VerifyAll();
        }
    }
}
