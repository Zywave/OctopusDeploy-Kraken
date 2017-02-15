namespace Kraken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ReleaseBatch")]
    public sealed class ReleaseBatch
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string UpdateUserName { get; set; }

        public DateTimeOffset? UpdateDateTime { get; set; }

        [MaxLength(50)]
        public string SyncUserName { get; set; }

        [MaxLength(20)]
        public string SyncEnvironmentId { get; set; }

        [MaxLength(50)]
        public string SyncEnvironmentName { get; set; }

        public DateTimeOffset? SyncDateTime { get; set; }

        [MaxLength(50)]
        public string DeployUserName { get; set; }

        [MaxLength(20)]
        public string DeployEnvironmentId { get; set; }

        [MaxLength(50)]
        public string DeployEnvironmentName { get; set; }

        public bool IsLocked { get; set; }

        [MaxLength(50)]
        public string LockUserName { get; set; }

        [MaxLength(100)]
        public string LockComment { get; set; }

        public DateTimeOffset? DeployDateTime { get; set; }

        public List<ReleaseBatchItem> Items { get; set; }

        public ReleaseBatchLogo Logo { get; set; }
    }
}
