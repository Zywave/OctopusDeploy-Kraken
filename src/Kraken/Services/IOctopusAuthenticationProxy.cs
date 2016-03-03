using Octopus.Client.Model;

namespace Kraken.Services
{
    public interface IOctopusAuthenticationProxy
    {
        bool Login(string username, string password, out UserResource user);

        string CreateApiKey();

        bool ValidateApiKey(string userName, string apiKey);

        bool ValidateApiKey(string apiKey, out string userName);
    }
}