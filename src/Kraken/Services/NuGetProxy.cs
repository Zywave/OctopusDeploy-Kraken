namespace Kraken.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    public class NuGetProxy : INuGetProxy
    {
        public async Task<Version> GetLatestVersionForPackageAsync(string packageId, string nuGetSource)
        {
            var repo = ConstructNuGetSourceRepository(nuGetSource);

            // shamelessly stolen from the way Install-Package finds the latest version
            var dependencyInfoResource = await repo.GetResourceAsync<DependencyInfoResource>();

            var packages =
                await
                    dependencyInfoResource.ResolvePackages(packageId, NuGetFramework.AnyFramework, new Logger(),
                        CancellationToken.None);
            var latestVersion = packages.Where(package => package.Listed)
                .OrderByDescending(package => package.Version, VersionComparer.Default)
                .Select(package => package.Version)
                .FirstOrDefault();

            return latestVersion?.Version;
        }

        private static SourceRepository ConstructNuGetSourceRepository(string nuGetSource)
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            var packageSource = new PackageSource(nuGetSource);
            return new SourceRepository(packageSource, providers);
        }

        /// <summary>
        /// NuGet requires an instance of their ILogger for most calls; could use to log errors if desired
        /// </summary>
        private class Logger : ILogger
        {
            public void LogDebug(string data)
            {
            }

            public void LogVerbose(string data)
            {
            }

            public void LogInformation(string data)
            {
            }

            public void LogMinimal(string data)
            {
            }

            public void LogWarning(string data)
            {
            }

            public void LogError(string data)
            {
            }

            public void LogInformationSummary(string data)
            {
            }

            public void LogErrorSummary(string data)
            {
            }
        }
    }

}
