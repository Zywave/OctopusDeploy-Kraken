namespace Kraken.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ReleaseBatchItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ReleaseBatch")]
        public int ReleaseBatchId { get; set; }

        [Required]
        [StringLength(20)]
        public string ProjectId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectName { get; set; }

        [Required]
        [StringLength(20)]
        public string ProjectVersion { get; set; }
        
        [StringLength(20)]
        public string ReleaseId { get; set; }

        public ReleaseBatch Batch { get; set; }
    }
}
