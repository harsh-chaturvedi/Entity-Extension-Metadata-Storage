using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetadataStorage.Infrastructure.Model
{
    public class MetadataSchemaDetails
    {
        [Key]
        public Guid Id { get; set; }

        public string SchemaName { get; set; }

        public string Description { get; set; }

        public int TenantId { get; set; }

        public Guid EntityId { get; set; }

        [ForeignKey("EntityId")]
        public virtual EntityMetadataDetails MetadataEntity { get; set; }

        public virtual ICollection<EntityMetadataSchema> EntityMetadataSchemas { get; set; }

        public virtual ICollection<MetadataSchemaFields> MetadataSchemaFields { get; set; }
    }
}
