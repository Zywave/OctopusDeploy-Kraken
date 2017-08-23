namespace Kraken.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Octopus.Client;
    using Octopus.Client.Model;
    using Octopus.Client.Exceptions;

    public class OctopusAuthenticationProxy : IOctopusAuthenticationProxy, IDisposable
    {
        public OctopusAuthenticationProxy(IOptions<AppSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _octopusServerAddress = settings.Value.OctopusServerAddress;
        }

        public async Task<UserResource> Login(string userName, string password)
        {
            if (_loginClient == null)
            {
                _loginClient = await OctopusAsyncClient.Create(new OctopusServerEndpoint(_octopusServerAddress), new OctopusClientOptions());
            }

            var loginCommand = new LoginCommand()
            {
                Username = userName,
                Password = password
            };

            try
            {
                await _loginClient.Repository.Users.SignIn(loginCommand);
            }
            catch (OctopusException e) when (e is OctopusValidationException || e is OctopusResourceNotFoundException)
            {
                return null;
            }

            return await _loginClient.Repository.Users.GetCurrent();
        }

        public async Task<string> CreateApiKey()
        {
            if (_loginClient == null)
            {
                throw new InvalidOperationException("Login must be called before an API key can be created.");
            }

            var apiKeyResource = await _loginClient.Repository.Users.CreateApiKey(await _loginClient.Repository.Users.GetCurrent(), "Kraken");
            return apiKeyResource.ApiKey;
        }

        public async Task<bool> ValidateApiKey(string userName, string apiKey)
        {
            UserResource user;
            try
            {
                using (var client = await OctopusAsyncClient.Create(new OctopusServerEndpoint(_octopusServerAddress, apiKey)))
                {
                    user = await client.Repository.Users.GetCurrent();
                }
            }
            catch (OctopusSecurityException)
            {
                return false;
            }

            return user.Username == userName;
        }

        public async Task<string> ValidateApiKey(string apiKey)
        {
            UserResource user;
            try
            {
                using (var client = await OctopusAsyncClient.Create(new OctopusServerEndpoint(_octopusServerAddress, apiKey)))
                {
                    user = await client.Repository.Users.GetCurrent();
                }
            }
            catch (OctopusSecurityException)
            {
                return null;
            }

            return user.Username;
        }

        public void Dispose()
        {
            _loginClient?.Dispose();
        }

        private readonly string _octopusServerAddress;
        private IOctopusAsyncClient _loginClient;
    }
}
