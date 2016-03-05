namespace Kraken.Models
{
    public class ReleaseBatchLogo
    {
        public int ReleaseBatchId { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public ReleaseBatch Batch { get; set; }
    }
}
