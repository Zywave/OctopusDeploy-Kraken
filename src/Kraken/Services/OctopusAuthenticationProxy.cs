namespace Kraken.Services
{
    using System;
    using Microsoft.Extensions.OptionsModel;
    using Octopus.Client;
    using Octopus.Client.Model;
    using Octopus.Client.Exceptions;

    public class OctopusAuthenticationProxy : IOctopusAuthenticationProxy
    {
        public OctopusAuthenticationProxy(IOptions<AppSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _octopusServerAddress = settings.Value.OctopusServerAddress;
            _repository = new OctopusRepository(new OctopusServerEndpoint(_octopusServerAddress));
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
                _repository.Users.SignIn(loginCommand);
            }
            catch (OctopusValidationException)
            {
                user = null;
                return false;
            }
            
            user = _repository.Users.GetCurrent();
            return true;
        }

        public string CreateApiKey()
        {
            var apiKeyResource = _repository.Users.CreateApiKey(_repository.Users.GetCurrent(), "Kraken");
            return apiKeyResource.ApiKey;
        }

        public bool ValidateApiKey(string userName, string apiKey)
        {
            var repository = new OctopusRepository(new OctopusServerEndpoint(_octopusServerAddress, apiKey));

            UserResource user;
            try
            {
                user = repository.Users.GetCurrent();
            }
            catch (OctopusSecurityException)
            {
                return false;
            }

            return user.Username == userName;
        }

        public bool ValidateApiKey(string apiKey, out string userName)
        {
            var repository = new OctopusRepository(new OctopusServerEndpoint(_octopusServerAddress, apiKey));

            UserResource user;
            try
            {
                user = repository.Users.GetCurrent();
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
        private readonly OctopusRepository _repository;
    }
}
