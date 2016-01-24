namespace Kraken.Services
{
    using Octopus.Client.Model;

    public interface IOctopusReleaseService
    {
        ReleaseResource GetNextRelease(string projectId);
    }
}
