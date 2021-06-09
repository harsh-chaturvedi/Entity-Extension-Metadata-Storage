using System;
using System.Collections.Generic;
using System.Linq;
using MetadataStorage.Infrastructure.Contracts;
using MetadataStorage.Infrastructure.Model;

namespace MetadataStorage.Infrastructure.Services
{
    /// <summary>
    /// Transforms the MetadataField names and values for storage or public view
    /// </summary>
    public class MetadataFieldTransformer : IFieldTransformer
    {
        private readonly IMetaDataStorageService _metaDataService;

        public MetadataFieldTransformer(IMetaDataStorageService metaDataService)
        {
            _metaDataService = metaDataService;
        }

        /// <summary>
        /// Transform incoming public Field Values to meta fields for storage
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="publicFieldValues">Field public values</param>
        /// <returns></returns>
        public MetadataFieldTransformationResult TransformPublicFieldToMetadataField(Guid entityId, int tenantId, IEnumerable<PublicFieldValue> publicFieldValues)
        {
            var entityMetadataFields = _metaDataService.GetMetadataFieldsByEntityId(entityId);
            var entityMetadataSchema = _metaDataService.GetMetaDataSchemaDetailsByEntityId(entityId);
            var entityType = _metaDataService.GetMetadataEntityByStorageEntityId(entityId);

            if (publicFieldValues == null || !publicFieldValues.Any() || entityMetadataFields == null || !entityMetadataFields.Any() || entityMetadataSchema == null || entityType == null)
            {
                return new MetadataFieldTransformationResult
                {
                    ErrorMessage = Constants.ErrorMessage.MetadataFieldsNotFound,
                    OperationResult = OperationResult.Fail
                };
            }

            MetadataFieldTransformationResult transformationResult = new MetadataFieldTransformationResult
            {
                MetadataFieldValues = new List<EntityMetadataFieldValues>(),
                EntityId = entityId
            };

            foreach (var item in publicFieldValues)
            {
                // field match found proceed with transform
                if (entityMetadataFields.Any(x => x.PublicName.Equals(item.PublicDisplayName, StringComparison.OrdinalIgnoreCase)))
                {
                    var metadata = entityMetadataFields.FirstOrDefault(x => x.PublicName.Equals(item.PublicDisplayName, StringComparison.OrdinalIgnoreCase));
                    transformationResult.MetadataFieldValues.Add(new EntityMetadataFieldValues
                    {
                        FieldId = metadata.Id,
                        FieldValue = item.FieldValue,
                        SubjectId = entityId,
                        TenantId = tenantId,
                        EntityTypeId = entityType.Id
                    });
                }
                else
                {
                    transformationResult.OperationResult = OperationResult.DoneWithError;
                    // incoming field has no meta field in schema, send the error to metadata manager - schema error
                    if (transformationResult.Errors == null)
                        transformationResult.Errors = new List<string>();

                    transformationResult.Errors.Add($"{Constants.ErrorMessage.NoMetadataField} {item.PublicDisplayName}");
                }
            }

            var unavailableMandatoryMetadataFields = entityMetadataFields.Where(x => x.IsMandatory && !publicFieldValues.Select(t => t.PublicDisplayName).Contains(x.PublicName));
            if (unavailableMandatoryMetadataFields.Any())
            {
                transformationResult.OperationResult = OperationResult.Fail;
                transformationResult.MetadataFieldErrors = new List<MetadataFieldError>();

                foreach (var field in unavailableMandatoryMetadataFields)
                {
                    //incoming data is missing mandatory fields - schema error
                    transformationResult.MetadataFieldErrors.Add(new MetadataFieldError
                    {
                        ErrorPhrase = Constants.ErrorMessage.MetadataFieldValueNotAvailable,
                        MetadataField = field
                    });
                }
                // add error on mandatory fields missing
            }

            return transformationResult;
        }

        /// <summary>
        /// Transform incoming public Field Values to meta fields for storage
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="publicFieldValue">Field public value</param>
        /// <returns></returns>
        public MetadataFieldTransformationResult TransformPublicFieldToMetadataField(Guid entityId, int tenantId, PublicFieldValue publicFieldValue)
        {
            var entityMetadataFields = _metaDataService.GetMetadataFieldsByEntityId(entityId);
            var entityMetadataSchema = _metaDataService.GetMetaDataSchemaDetailsByEntityId(entityId);
            var entityType = _metaDataService.GetMetadataEntityByStorageEntityId(entityId);

            if (publicFieldValue == null || entityMetadataFields == null || !entityMetadataFields.Any() || entityMetadataSchema == null || entityType == null)
            {
                return new MetadataFieldTransformationResult
                {
                    ErrorMessage = Constants.ErrorMessage.MetadataFieldsNotFound,
                    OperationResult = OperationResult.Fail
                };
            }

            MetadataFieldTransformationResult transformationResult = new MetadataFieldTransformationResult
            {
                MetadataFieldValues = new List<EntityMetadataFieldValues>(),
                EntityId = entityId
            };

            // field match found proceed with transform
            if (entityMetadataFields.Any(x => x.PublicName.Equals(publicFieldValue.PublicDisplayName, StringComparison.OrdinalIgnoreCase)))
            {
                var metadata = entityMetadataFields.FirstOrDefault(x => x.PublicName.Equals(publicFieldValue.PublicDisplayName, StringComparison.OrdinalIgnoreCase));
                transformationResult.MetadataFieldValues.Add(new EntityMetadataFieldValues
                {
                    FieldId = metadata.Id,
                    FieldValue = publicFieldValue.FieldValue,
                    SubjectId = entityId,
                    TenantId = tenantId,
                    EntityTypeId = entityType.Id,
                    Id = publicFieldValue.Id
                });
            }
            else
            {
                transformationResult.OperationResult = OperationResult.DoneWithError;
                // incoming field has no meta field in schema, send the error to metadata manager - schema error
                if (transformationResult.Errors == null)
                    transformationResult.Errors = new List<string>();

                transformationResult.Errors.Add($"{Constants.ErrorMessage.NoMetadataField} {publicFieldValue.PublicDisplayName}");
            }

            return transformationResult;
        }

        /// <summary>
        /// Transform outgoing metadata field to public field for view
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="metadataValues">Metadata Value</param>
        /// <returns></returns>
        public PublicFieldTransformationResult TransfromMetadataFieldToPublicField(Guid entityId, IEnumerable<EntityMetadataFieldValues> metadataValues)
        {
            var entityMetadataFields = _metaDataService.GetMetadataFieldsByEntityId(entityId).ToList();

            if (metadataValues == null || !metadataValues.Any() || entityMetadataFields == null || !entityMetadataFields.Any())
            {
                return new PublicFieldTransformationResult
                {
                    ErrorMessage = Constants.ErrorMessage.MetadataFieldsNotFound,
                    OperationResult = OperationResult.Fail
                };
            }

            PublicFieldTransformationResult transformationResult = new PublicFieldTransformationResult
            {
                PublicFieldValues = new List<PublicFieldValue>(),
                EntityId = entityId
            };

            foreach (var item in metadataValues)
            {
                if (entityMetadataFields.Any(x => x.Id.Equals(item.Id)))
                {
                    var metadata = entityMetadataFields.FirstOrDefault(x => x.Id.Equals(item.FieldId));
                    transformationResult.PublicFieldValues.Add(new PublicFieldValue
                    {
                        PublicDisplayName = metadata.PublicName,
                        FieldValue = item.FieldValue,
                        FieldType = metadata.FieldType,
                        Id = item.Id
                    });
                }
                else
                {
                    //extra field value - metadata removed from schema
                    transformationResult.OperationResult = OperationResult.DoneWithError;
                    // incoming field has no meta field in schema, send the error to metadata manager
                    if (transformationResult.Errors == null)
                        transformationResult.Errors = new List<string>();

                    transformationResult.Errors.Add($"{Constants.ErrorMessage.MetadataFieldWithNoSchema} {item.MetadataField.FieldName}");
                }
            }

            var unavailableMandatoryMetadataFields = entityMetadataFields.Where(x => x.IsMandatory && !metadataValues.Select(t => t.FieldId).Contains(x.Id));
            if (unavailableMandatoryMetadataFields.Any())
            {
                transformationResult.OperationResult = OperationResult.Fail;
                transformationResult.MetadataFieldErrors = new List<MetadataFieldError>();

                foreach (var field in unavailableMandatoryMetadataFields)
                {
                    //incoming data is missing mandatory fields - schema error
                    transformationResult.MetadataFieldErrors.Add(new MetadataFieldError
                    {
                        ErrorPhrase = Constants.ErrorMessage.MetadataFieldValueNotAvailable,
                        MetadataField = field
                    });
                    // add error on mandatory fields missing - value for mandtory field not available in db
                }
            }

            return transformationResult;
        }
    }
}