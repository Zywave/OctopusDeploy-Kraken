namespace Kraken.Services
{
    using Octopus.Client.Model;

    public interface IOctopusReleaseService
    {
        ReleaseResource CreateRelease(string projectId, string version = null);
    }
}
