using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Contracts
{
    public interface IMetadataValidator
    {
        (bool, string) ValidatePublicFieldValuesForType(MetadataFieldTransformationResult transformationResult);

        (bool, string) ValidateMetadataFieldValuesForSchema(MetadataFieldTransformationResult transformationResult);

        (bool, string) ValidateDatabaseFieldValuesForSchema(PublicFieldTransformationResult transformationResult);
    }
}