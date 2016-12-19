namespace Kraken.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Client;
    using NuGet.Configuration;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;

    public class NuGetProxy : INuGetProxy
    {
        public async Task<Version> GetLatestVersionForPackageAsync(string packageId, string nuGetSource)
        {
            var repo = ConstructNuGetSourceRepository(nuGetSource);
            var resource = await repo.GetResourceAsync<FindPackageByIdResource>();
            var versions = await resource.GetAllVersionsAsync(packageId, new SourceCacheContext(), null, CancellationToken.None);
            return versions.FirstOrDefault()?.Version;
        }

        private static SourceRepository ConstructNuGetSourceRepository(string nuGetSource)
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            var packageSource = new PackageSource(nuGetSource);
            return new SourceRepository(packageSource, providers);
        }
    }
}
