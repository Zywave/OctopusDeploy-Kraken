namespace Kraken.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ReleaseBatchItem")]
    public class ReleaseBatchItem
    {
        public int Id { get; set; }

        public int ReleaseBatchId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ProjectId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProjectName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProjectSlug { get; set; }

        [MaxLength(20)]
        public string ReleaseId { get; set; }

        [MaxLength(50)]
        public string ReleaseVersion { get; set; }

        public ReleaseBatch Batch { get; set; }
    }
}
