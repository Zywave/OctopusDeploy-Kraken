namespace Kraken.Services
{
    using System;
    using Microsoft.Extensions.OptionsModel;
    using Octopus.Client;
    using Octopus.Client.Model;

    public class OctopusAuthenticationProxy : IOctopusAuthenticationProxy
    {
        public OctopusAuthenticationProxy(IOptions<AppSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            
            var endpoint = new OctopusServerEndpoint(settings.Value.OctopusServerAddress);
            _repository = new OctopusRepository(endpoint);
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
        
        private readonly OctopusRepository _repository;
    }
}
