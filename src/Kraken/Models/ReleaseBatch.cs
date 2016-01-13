namespace Kraken.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class ReleaseBatch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public List<ReleaseBatchItem> Items { get; set; }
    }
}
