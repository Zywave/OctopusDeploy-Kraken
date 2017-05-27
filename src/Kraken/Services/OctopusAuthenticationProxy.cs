namespace Kraken.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Octopus.Client;
    using Octopus.Client.Model;
    using Octopus.Client.Exceptions;

    public class OctopusAuthenticationProxy : IOctopusAuthenticationProxy
    {
        public OctopusAuthenticationProxy(IOptions<AppSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _octopusServerAddress = settings.Value.OctopusServerAddress;
            _repository = OctopusAsyncClient.Create(new OctopusServerEndpoint(_octopusServerAddress), new OctopusClientOptions()).Result.Repository;
        }

        public async Task<UserResource> Login(string userName, string password)
        {
            var loginCommand = new LoginCommand()
            {
                Username = userName,
                Password = password
            };

            try
            {
                await _repository.Users.SignIn(loginCommand);
            }
            catch (OctopusException e) when (e is OctopusValidationException || e is OctopusResourceNotFoundException)
            {
                return null;
            }

            var user = _repository.Users.GetCurrent().Result;
            return user;
        }

        public async Task<string> CreateApiKey()
        {
            var apiKeyResource = await _repository.Users.CreateApiKey(_repository.Users.GetCurrent().Result, "Kraken");
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

        private readonly string _octopusServerAddress;
        private readonly IOctopusAsyncRepository _repository;
    }
}
