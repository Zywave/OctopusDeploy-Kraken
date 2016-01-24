namespace Kraken.Services
{
    using System;
    using System.Linq;
    using Microsoft.AspNet.Http;
    using Microsoft.Extensions.OptionsModel;
    using NuGet;
    using Octopus.Client;
    using Security;

    public class NuGetProxy : INuGetProxy
    {
        public string GetLatestVersionForPackage(string packageId, string nuGetSource)
        {
            var nuGetPackageRepository = PackageRepositoryFactory.Default.CreateRepository(nuGetSource);
            var latestNugetPackage = nuGetPackageRepository.FindPackagesById(packageId).ToList().OrderByDescending(n => n.Published).First();
            return latestNugetPackage.Version.ToString();
        }
    }
}
