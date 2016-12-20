using Octopus.Client.Model;

namespace Kraken.Services
{
    using System.Threading.Tasks;

    public interface IOctopusAuthenticationProxy
    {
        Task<UserResource> Login(string username, string password);

        Task<string> CreateApiKey();

        Task<bool> ValidateApiKey(string userName, string apiKey);

        Task<string> ValidateApiKey(string apiKey);
    }
}