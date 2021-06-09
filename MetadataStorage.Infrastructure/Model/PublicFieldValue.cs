using System;

namespace MetadataStorage.Infrastructure.Model
{
    public class PublicFieldValue
    {
        public Guid Id { get; set; }

        public string PublicDisplayName { get; set; }

        public string FieldValue { get; set; }

        public FieldType FieldType { get; set; }
    }
}