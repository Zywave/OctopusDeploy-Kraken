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
        [StringLength(20)]
        public string ProjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectName { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectSlug { get; set; }

        [StringLength(20)]
        public string ReleaseId { get; set; }

        [StringLength(20)]
        public string ReleaseVersion { get; set; }

        public ReleaseBatch Batch { get; set; }
    }
}
