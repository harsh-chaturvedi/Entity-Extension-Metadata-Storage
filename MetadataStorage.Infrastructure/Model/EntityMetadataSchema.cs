using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetadataStorage.Infrastructure.Model
{
    public class EntityMetadataSchema
    {
        /// <summary>
        /// The identifier for the stored entity
        /// </summary>
        public Guid SubjectId { get; set; }

        public Guid SchemaId { get; set; }
        [ForeignKey("SchemaId")]
        public virtual MetadataSchemaDetails MetadataSchemaDetails { get; set; }
    }
}
