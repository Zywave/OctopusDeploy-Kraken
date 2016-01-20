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
        public NuGetProxy(IOptions<AppSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _nuGetPackageRepository = PackageRepositoryFactory.Default.CreateRepository(settings.Value.NuGetServerAddress);
        }

        public string GetLatestVersionForPackage(string packageId)
        {
            var latestNugetPackage = _nuGetPackageRepository.FindPackagesById(packageId).ToList().OrderByDescending(n => n.Published).First();
            return latestNugetPackage.Version.ToString();
        }

        private readonly IPackageRepository _nuGetPackageRepository;
    }
}
