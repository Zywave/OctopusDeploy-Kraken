namespace Kraken.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public sealed class ReleaseBatch
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string UpdateUserName { get; set; }

        public DateTimeOffset? UpdateDateTime { get; set; }

        [StringLength(50)]
        public string SyncUserName { get; set; }

        [StringLength(20)]
        public string SyncEnvironmentId { get; set; }

        [StringLength(50)]
        public string SyncEnvironmentName { get; set; }

        public DateTimeOffset? SyncDateTime { get; set; }

        [StringLength(50)]
        public string DeployUserName { get; set; }

        [StringLength(20)]
        public string DeployEnvironmentId { get; set; }

        [StringLength(50)]
        public string DeployEnvironmentName { get; set; }

        public bool IsLocked { get; set; }

        [StringLength(50)]
        public string LockUserName { get; set; }

        [StringLength(100)]
        public string LockComment { get; set; }

        public DateTimeOffset? DeployDateTime { get; set; }

        public List<ReleaseBatchItem> Items { get; set; }

        public ReleaseBatchLogo Logo { get; set; }
    }
}
