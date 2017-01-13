namespace Kraken.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ReleaseBatchLogo")]
    public class ReleaseBatchLogo
    {
        public int ReleaseBatchId { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public ReleaseBatch Batch { get; set; }
    }
}
