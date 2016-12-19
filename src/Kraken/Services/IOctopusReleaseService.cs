namespace Kraken.Services
{
    using System.Threading.Tasks;
    using Octopus.Client.Model;

    public interface IOctopusReleaseService
    {
        Task<ReleaseResource> GetNextReleaseAsync(string projectId);
    }
}
