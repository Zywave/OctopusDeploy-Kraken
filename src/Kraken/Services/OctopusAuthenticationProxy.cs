namespace Kraken.Services
{
    using System;
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

        public bool Login(string userName, string password, out UserResource user)
        {
            var loginCommand = new LoginCommand()
            {
                Username = userName,
                Password = password
            };

            try
            {
                _repository.Users.SignIn(loginCommand).Wait();
            }
            catch (OctopusValidationException)
            {
                user = null;
                return false;
            }
            
            user = _repository.Users.GetCurrent().Result;
            return true;
        }

        public string CreateApiKey()
        {
            var apiKeyResource = _repository.Users.CreateApiKey(_repository.Users.GetCurrent().Result, "Kraken").Result;
            return apiKeyResource.ApiKey;
        }

        public bool ValidateApiKey(string userName, string apiKey)
        {
            UserResource user;
            try
            {
                user = _repository.Users.GetCurrent().Result;
            }
            catch (OctopusSecurityException)
            {
                return false;
            }

            return user.Username == userName;
        }

        public bool ValidateApiKey(string apiKey, out string userName)
        {
            UserResource user;
            try
            {
                user = _repository.Users.GetCurrent().Result;
            }
            catch (OctopusSecurityException)
            {
                userName = null;
                return false;
            }

            userName = user.Username;
            return true;
        }

        private readonly string _octopusServerAddress;
        private readonly IOctopusAsyncRepository _repository;
    }
}
