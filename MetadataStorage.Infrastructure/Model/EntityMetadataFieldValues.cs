using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetadataStorage.Infrastructure.Model
{
    public class EntityMetadataFieldValues
    {
        [Key]
        public Guid Id { get; set; }

        ///<summary>
        /// Identifier for the storage entity
        ///</summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// Entity Type Id for quick search
        /// </summary>
        public Guid EntityTypeId { get; set; }
        [ForeignKey("EntityTypeId")]
        public virtual EntityMetadataDetails MetadataEntity { get; set; }

        // public string FieldName { get; set; }

        public Guid FieldId { get; set; }
        [ForeignKey("FieldId")]
        public virtual MetadataField MetadataField { get; set; }

        public string FieldValue { get; set; }

        public int TenantId { get; set; }
    }
}
