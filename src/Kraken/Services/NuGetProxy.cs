namespace Kraken.Services
{
    using System.Linq;
    using NuGet;

    public class NuGetProxy : INuGetProxy
    {
        public string GetLatestVersionForPackage(string packageId, string nuGetSource)
        {
            var nuGetPackageRepository = PackageRepositoryFactory.Default.CreateRepository(nuGetSource);
            var latestNuGetPackage = nuGetPackageRepository.FindPackagesById(packageId).ToList().OrderByDescending(n => n.Published).First();
            return latestNuGetPackage.Version.ToString();
        }
    }
}
