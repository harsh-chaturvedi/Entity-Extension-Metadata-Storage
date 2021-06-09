using System.Collections.Generic;

namespace MetadataStorage.Infrastructure.Model
{
    public class MetadataFieldTransformationResult : BaseTransformationResult
    {
        public List<EntityMetadataFieldValues> MetadataFieldValues { get; set; }
    }

    public class PublicFieldTransformationResult : BaseTransformationResult
    {
        public List<PublicFieldValue> PublicFieldValues { get; set; }
    }
}