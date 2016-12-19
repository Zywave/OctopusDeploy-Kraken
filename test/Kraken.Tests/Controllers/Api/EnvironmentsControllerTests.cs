using Kraken.Controllers.Api;
using Kraken.Services;
using Moq;
using Octopus.Client.Model;
using Xunit;

namespace Kraken.Tests.Controllers.Api
{
    using System.Threading.Tasks;

    public class EnvironmentsControllerTests
    {
        [Fact]
        public async Task TestGetEnvironmentsReturnsOctopusData()
        {
            var octopusApi = new Mock<IOctopusProxy>(MockBehavior.Strict);
            octopusApi.Setup(o => o.GetEnvironmentsAsync()).ReturnsAsync(new[] { new EnvironmentResource() });

            var controller = new EnvironmentsController(octopusApi.Object);

            var result = await controller.GetEnvironments();

            Assert.NotNull(result);

            octopusApi.VerifyAll();
        }
    }
}
