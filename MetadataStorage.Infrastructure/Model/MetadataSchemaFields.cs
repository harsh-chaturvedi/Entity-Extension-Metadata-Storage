using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetadataStorage.Infrastructure.Model
{
    public class MetadataSchemaFields
    {
        public Guid SchemaId { get; set; }
        [ForeignKey("SchemaId")]
        public virtual MetadataSchemaDetails MetadataSchemaDetails { get; set; }

        public Guid FieldId { get; set; }
        [ForeignKey("FieldId")]
        public virtual MetadataField MetadataField { get; set; }
    }
}
