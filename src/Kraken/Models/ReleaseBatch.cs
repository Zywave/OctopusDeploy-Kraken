namespace Kraken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class ReleaseBatch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(50)]
        public string SyncUserName { get; set; }

        [StringLength(50)]
        public string SyncEnvironmentId { get; set; }

        public DateTimeOffset? SyncDateTime { get; set; }

        public List<ReleaseBatchItem> Items { get; set; }
    }
}
