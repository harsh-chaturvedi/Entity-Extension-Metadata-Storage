using System;
using System.Collections.Generic;
using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Contracts
{
    /// <summary>
    /// Transforms the MetadataField names and values for storage or public view
    /// </summary>
    public interface IFieldTransformer
    {
        /// <summary>
        /// Transform outgoing metadata field to public field for view
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="metadataValues">Metadata Value</param>
        /// <returns></returns>
        PublicFieldTransformationResult TransfromMetadataFieldToPublicField(Guid entityId, IEnumerable<EntityMetadataFieldValues> metadataValues);

        /// <summary>
        /// Transform incoming public Field Values to meta fields for storage
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="publicFieldValues">Field public values</param>
        /// <returns></returns>
        MetadataFieldTransformationResult TransformPublicFieldToMetadataField(Guid entityId, int tenantId, IEnumerable<PublicFieldValue> publicFieldValues);

        /// <summary>
        /// Transform incoming public Field Values to meta fields for storage
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="publicFieldValues">Field public values</param>
        /// <returns></returns>
        MetadataFieldTransformationResult TransformPublicFieldToMetadataField(Guid entityId, int tenantId, PublicFieldValue publicFieldValues);
    }
}