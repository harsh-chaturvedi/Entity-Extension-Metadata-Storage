using System;
using System.Collections.Generic;
using System.Linq;
using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Extensions
{
    public static class MetadataModelValidator
    {
        public static (bool, string) Validate(this EntityMetadataDetails metadataEntity)
        {
            if (metadataEntity == null)
            {
                return (false, "Metadata entity cannot be empty");
            }
            if (string.IsNullOrEmpty(metadataEntity.EntityName))
            {
                return (false, "Entity name cannot be empty");
            }
            if (metadataEntity.TenantId <= 0)
            {
                return (false, "Invalid tenant identifier");
            }
            return (true, string.Empty);
        }

        public static (bool, string) Validate(this MetadataField metadataField)
        {
            if (metadataField == null)
            {
                return (false, "Metadata field cannot be empty");
            }
            if (string.IsNullOrEmpty(metadataField.FieldName))
            {
                return (false, "Metadata field name cannot be empty");
            }
            if (string.IsNullOrEmpty(metadataField.PublicName))
            {
                return (false, "Metadata public name cannot be empty");
            }
            if (metadataField.TenantId <= 0)
            {
                return (false, "Invalid tenant identifier");
            }
            return (true, string.Empty);
        }

        public static (bool, string) Validate(this MetadataSchemaDetails metadataSchema)
        {
            if (metadataSchema == null)
            {
                return (false, "Metadata schema cannot be empty");
            }
            if (string.IsNullOrEmpty(metadataSchema.SchemaName))
            {
                return (false, "Metadata schema name cannot be empty");
            }
            if (metadataSchema.TenantId <= 0)
            {
                return (false, "Invalid tenant identifier");
            }
            if (metadataSchema.EntityId == Guid.Empty)
            {
                return (false, "Invalid entity identifier");
            }
            return (true, string.Empty);
        }

        public static (bool, string) Validate(this IEnumerable<PublicFieldValue> publicFieldValues)
        {
            if (publicFieldValues == null || !publicFieldValues.Any())
            {
                return (false, "Field values cannot be empty");
            }
            foreach (var item in publicFieldValues)
            {
                if (string.IsNullOrEmpty(item.PublicDisplayName))
                    return (false, "Display name cannot be empty");
            }
            return (true, string.Empty);
        }

        public static (bool, string) Validate(this PublicFieldValue publicFieldValue)
        {
            if (string.IsNullOrEmpty(publicFieldValue.PublicDisplayName))
                return (false, "Display name cannot be empty");

            if (string.IsNullOrEmpty(publicFieldValue.FieldValue))
                return (false, "Field value cannot be empty");

            return (true, string.Empty);
        }
    }
}