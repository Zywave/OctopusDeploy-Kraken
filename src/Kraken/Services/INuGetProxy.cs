namespace Kraken.Services
{
    public interface INuGetProxy
    {
        string GetLatestVersionForPackage(string packageId, string nuGetSource);
    }
}
