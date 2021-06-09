using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MetadataStorage.Infrastructure.Model
{
    public class MetadataField
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FieldName { get; set; }

        public FieldType FieldType { get; set; }
        [Required]
        public string PublicName { get; set; }

        public bool IsMandatory { get; set; }

        public int TenantId { get; set; }

        public virtual ICollection<MetadataSchemaFields> MetadataSchemaFields { get; set; }

        public virtual ICollection<EntityMetadataFieldValues> EntityMetadataFieldValues { get; set; }
    }

    public enum FieldType
    {
        BOOL,
        INT,
        LONG,
        FLOAT,
        DOUBLE,
        STRING,
        JSON,
        EMAIL,
        DATETIME
    }
}
