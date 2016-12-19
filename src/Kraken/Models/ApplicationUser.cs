namespace Kraken.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ApplicationUser")]
    public class ApplicationUser
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string OctopusApiKey { get; set; }
    }
}
