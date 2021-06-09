using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MetadataStorage.Infrastructure.Model
{
    public class EntityMetadataDetails
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string EntityName { get; set; }

        public string Description { get; set; }

        public int TenantId { get; set; }

        public virtual ICollection<MetadataSchemaDetails> MetadataSchemaDetails { get; set; }

        public virtual ICollection<EntityMetadataFieldValues> EntityMetadataFieldValues { get; set; }
    }
}
