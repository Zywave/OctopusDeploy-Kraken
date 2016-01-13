namespace Kraken.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProjectBatchItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProjectBatch")]
        public int ProjectBatchId { get; set; }

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
        public string LatestReleaseId { get; set; }

        [StringLength(20)]
        public string LatestDeploymentId { get; set; }

        [StringLength(20)]
        public string LatestTaskId { get; set; }

        [StringLength(50)]
        public string NugetPackageId { get; set; }

        public ProjectBatch Batch { get; set; }
    }
}
