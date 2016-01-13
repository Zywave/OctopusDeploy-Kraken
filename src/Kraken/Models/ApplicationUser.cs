namespace Kraken.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ApplicationUser
    {
        [Key]
        public string UserName { get; set; }

        public string OctopusApiKey { get; set; }
    }
}
