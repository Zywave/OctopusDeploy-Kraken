namespace Kraken.Services
{
    using System;
    using System.Threading.Tasks;

    public interface INuGetProxy
    {
        Task<Version> GetLatestVersionForPackageAsync(string packageId, string nuGetSource);
    }
}
