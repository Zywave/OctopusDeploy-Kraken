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

        public bool Login(string userName, string password)
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
            catch
            {
                return false;
            }

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

        private readonly string _octopusServerAddress;
        private readonly OctopusRepository _repository;
    }
}
