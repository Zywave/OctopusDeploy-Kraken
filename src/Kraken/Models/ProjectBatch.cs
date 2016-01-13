namespace Kraken.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class ProjectBatch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public List<ProjectBatchItem> Items { get; set; }
    }
}
