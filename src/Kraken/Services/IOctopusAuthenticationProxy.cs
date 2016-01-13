namespace Kraken.Services
{
    public interface IOctopusAuthenticationProxy
    {
        bool Login(string username, string password);

        string CreateApiKey();
    }
}